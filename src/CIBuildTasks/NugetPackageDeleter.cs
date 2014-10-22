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
            this.deleteCommand.Execute(this);

            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The '{0} {1}' package was deleted.",
                this.nugetId,
                this.nugetVersion);

            this.logger.Log(this, message, MessageImportance.High);

            return true;
        }
    }
}