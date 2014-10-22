namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using HtmlAgilityPack;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Represents command implementation for deleting a nuget package.
    /// </summary>
    public class DeletePackageCommand : IDeletePackageCommand
    {
        /// <summary>
        /// Deletes the specified nuget package.
        /// </summary>
        /// <param name="deleteCommandArgs">
        /// The nuget package information to be deleted.
        /// </param>
        public void Execute(IDeletePackageCommandArgs deleteCommandArgs)
        {
            if (deleteCommandArgs == null)
                throw new ArgumentNullException("deleteCommandArgs");

            new InnerDeletePackageCommand(deleteCommandArgs).Delete();
        }

        private class InnerDeletePackageCommand
        {
            private readonly IDeletePackageCommandArgs deletePackageCommandArgs;

            public InnerDeletePackageCommand(IDeletePackageCommandArgs deletePackageCommandArgs)
            {
                this.deletePackageCommandArgs = deletePackageCommandArgs;
            }

            public void Delete()
            {
                using (var client = new WebClientWithCookies())
                {
                    this.PostSignIn(client);
                    this.PostDelete(client);
                }
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
                    "SignIn.UserNameOrEmail=" + this.deletePackageCommandArgs.UserId,
                    "SignIn.Password=" + this.deletePackageCommandArgs.UserPassword
                };

                var result = client.UploadString(
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
                    this.deletePackageCommandArgs.NugetId,
                    this.deletePackageCommandArgs.NugetVersion);
                return packageUrl;
            }
        }
    }
}