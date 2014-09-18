namespace Jwc.CIBuildTasks
{
    using System;
    using System.Net;

    internal class WebClientWithCookies : WebClient
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