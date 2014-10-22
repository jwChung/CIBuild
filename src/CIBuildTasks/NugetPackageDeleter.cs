namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to delete a specified nuget package on the server.
    /// </summary>
    public class NugetPackageDeleter : Task, IDeletePackageCommandArgs
    {
        private readonly IDeletePackageCommand deleteCommand;
        private readonly ITaskLogger logger;
        private string userId;
        private string userPassword;
        private string nugetId;
        private string nugetVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="NugetPackageDeleter"/> class.
        /// </summary>
        public NugetPackageDeleter() : this(new DeletePackageCommand(), new TaskLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NugetPackageDeleter"/> class.
        /// </summary>
        /// <param name="deleteCommand">
        /// The command for deleting a nuget package.
        /// </param>
        /// <param name="logger">
        /// The task logger.
        /// </param>
        public NugetPackageDeleter(IDeletePackageCommand deleteCommand, ITaskLogger logger)
        {
            if (deleteCommand == null)
                throw new ArgumentNullException("deleteCommand");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.deleteCommand = deleteCommand;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Required]
        public string UserId
        {
            get
            {
                return this.userId;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.userId = value;
            }
        }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        [Required]
        public string UserPassword
        {
            get
            {
                return this.userPassword;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.userPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets the nuget identifier.
        /// </summary>
        [Required]
        public string NugetId
        {
            get
            {
                return this.nugetId;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.nugetId = value;
            }
        }

        /// <summary>
        /// Gets or sets the nuget version.
        /// </summary>
        [Required]
        public string NugetVersion
        {
            get
            {
                return this.nugetVersion;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.nugetVersion = value;
            }
        }

        /// <summary>
        /// Gets the command for deleting a nuget package.
        /// </summary>
        public IDeletePackageCommand DeleteCommand
        {
            get { return this.deleteCommand; }
        }

        /// <summary>
        /// Gets the task logger.
        /// </summary>
        public ITaskLogger Logger
        {
            get { return this.logger; }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public sealed override bool Execute()
        {
            this.deleteCommand.Delete(this);

            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The '{0} {1}' package was deleted.",
                this.nugetId,
                this.nugetVersion);

            this.logger.Log(this, message, MessageImportance.High);

            return true;
        }

        /// <summary>
        /// Deletes a package.
        /// </summary>
        /// <param name="id">
        /// User id.
        /// </param>
        /// <param name="pwd">
        /// User password.
        /// </param>
        /// <param name="package">
        /// A package identifier.
        /// </param>
        protected virtual void DeletePackage(string id, string pwd, string package)
        {
            using (var client = new WebClientWithCookies())
            {
                this.PostSignIn(client);
                this.PostDelete(client);
            }
        }

        /// <summary>
        /// Logs message.
        /// </summary>
        /// <param name="lineOfText">
        /// The line of text.
        /// </param>
        /// <param name="messageImportance">
        /// The message importance.
        /// </param>
        protected virtual void LogMessageFromText(
            string lineOfText, MessageImportance messageImportance)
        {
            Log.LogMessageFromText(lineOfText, messageImportance);
        }

        private static HtmlDocument GetLogOnDocument(WebClientWithCookies client)
        {
            var content = client.DownloadString(
                "https://www.nuget.org/users/account/LogOn?returnUrl=%2F");
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        private void PostSignIn(WebClientWithCookies client)
        {
            var node = GetLogOnDocument(client).DocumentNode
                .SelectNodes("//input[@name='__RequestVerificationToken']")[1];
            var formValues = new List<string>
            {
                "__RequestVerificationToken=" + node.Attributes["Value"].Value,
                "ReturnUrl=/",
                "LinkingAccount=False",
                "SignIn.UserNameOrEmail=" + this.userId,
                "SignIn.Password=" + this.userPassword
            };

            client.UploadString(
                "https://www.nuget.org/users/account/SignIn",
                string.Join("&", formValues.ToArray()));
        }

        private void PostDelete(WebClientWithCookies client)
        {
            var node = this.GetDeleteDocument(client).DocumentNode
                .SelectNodes("//input[@name='__RequestVerificationToken']").Single();
            var formValues = new List<string>
            {
                "__RequestVerificationToken=" + node.Attributes["Value"].Value,
                "Listed=false"
            };
            client.UploadString(this.GetPackageUrl(), string.Join("&", formValues.ToArray()));
        }

        private HtmlDocument GetDeleteDocument(WebClientWithCookies client)
        {
            var content = client.DownloadString(this.GetPackageUrl());
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        private string GetPackageUrl()
        {
            var packageUrl = string.Format(
                CultureInfo.CurrentCulture,
                "https://www.nuget.org/packages/{0}/{1}/Delete",
                this.nugetId,
                this.nugetVersion);
            return packageUrl;
        }
    }
}