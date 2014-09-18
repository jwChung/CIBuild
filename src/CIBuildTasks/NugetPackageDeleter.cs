namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to delete a specified nuget package on the server.
    /// </summary>
    public class NugetPackageDeleter : Task
    {
        private string idOrEmail;
        private string password;
        private string identifier;

        /// <summary>
        /// Gets or sets user id(or email).
        /// </summary>
        [Required]
        public string IdOrEmail
        {
            get
            {
                return this.idOrEmail;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.idOrEmail = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.password = value;
            }
        }

        /// <summary>
        /// Gets or sets the nuget package identifier.
        /// </summary>
        [Required]
        public string Identifier
        {
            get
            {
                return this.identifier;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.identifier = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            using (var client = new WebClientWithCookies())
            {
                this.PostSignIn(client);
                this.PostDelete(client);
            }

            Log.LogMessageFromText("Delete: " + this.identifier, MessageImportance.High);
            return true;
        }

        private static HtmlDocument GetLogOnDocument(WebClientWithCookies client)
        {
            var content = client.DownloadString("https://www.nuget.org/users/account/LogOn?returnUrl=%2F");
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
                "SignIn.UserNameOrEmail=" + this.idOrEmail,
                "SignIn.Password=" + this.password
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
                "https://www.nuget.org/packages/{0}/Delete",
                Regex.Replace(this.identifier, @"\s+", "/"));
            return packageUrl;
        }

        private class WebClientWithCookies : WebClient
        {
            private CookieContainer cookieContainer = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri address)
            {
                var webRequest = base.GetWebRequest(address);
                var httpWebRequest = webRequest as HttpWebRequest;
                if (httpWebRequest != null)
                    httpWebRequest.CookieContainer = this.cookieContainer;

                if (this.Headers[HttpRequestHeader.ContentType] == null)
                    this.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                return webRequest;
            }
        }
    }
}