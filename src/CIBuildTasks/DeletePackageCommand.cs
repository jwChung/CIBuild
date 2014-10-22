namespace Jwc.CIBuild
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using HtmlAgilityPack;

    /// <summary>
    /// Represents command implementation for deleting a nuget package.
    /// </summary>
    public class DeletePackageCommand : IDeletePackageCommand
    {
        /// <summary>
        /// Deletes the specified nuget package.
        /// </summary>
        /// <param name="nugetPackageInfo">
        /// The nuget package information to be deleted.
        /// </param>
        public void Execute(INugetPackageInfo nugetPackageInfo)
        {
            if (nugetPackageInfo == null)
                throw new ArgumentNullException("nugetPackageInfo");

            new InnerDeletePackageCommand(nugetPackageInfo).Delete();
        }

        private class InnerDeletePackageCommand
        {
            private readonly INugetPackageInfo nugetPackageInfo;

            public InnerDeletePackageCommand(INugetPackageInfo nugetPackageInfo)
            {
                this.nugetPackageInfo = nugetPackageInfo;
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
                    "SignIn.UserNameOrEmail=" + this.nugetPackageInfo.UserId,
                    "SignIn.Password=" + this.nugetPackageInfo.UserPassword
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
                    this.nugetPackageInfo.NugetId,
                    this.nugetPackageInfo.NugetVersion);
                return packageUrl;
            }
        }
    }
}