namespace Jwc.CIBuildTasks
{
    using System;
    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents command implementation for creating a tag.
    /// </summary>
    public class CreateTagCommand : ICreateTagCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="tagInfo">
        /// The tag information.
        /// </param>
        public void Execute(ITagInfo tagInfo)
        {
            if (tagInfo == null)
                throw new ArgumentNullException("tagInfo");

            using (var tagger = new Tagger(tagInfo))
            {
                tagger.CreateTag();
            }
        }

        private sealed class Tagger : IDisposable
        {
            private readonly WebClient client = new WebClientWithCookies();
            private readonly ITagInfo tagInformation;
            private bool disposed;

            public Tagger(ITagInfo tagInformation)
            {
                this.tagInformation = tagInformation;
            }

            public void CreateTag()
            {
                var createReferenceInput = new
                {
                    @ref = "refs/tags/" + this.tagInformation.TagName,
                    sha = this.CreateObject()
                };

                this.Refresh();
                this.client.UploadString(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "repos/{0}/{1}/git/refs",
                        this.tagInformation.Owner,
                        this.tagInformation.Repository),
                    JsonConvert.SerializeObject(createReferenceInput));
            }

            public void Dispose()
            {
                if (this.disposed)
                    return;

                this.client.Dispose();

                this.disposed = true;
            }

            private string CreateObject()
            {
                var createObjectInput = new
                {
                    tag = this.tagInformation.TagName,
                    message = this.tagInformation.ReleaseNotes,
                    @object = this.GetShaForTag(),
                    type = "commit",
                    tagger = new
                    {
                        name = this.tagInformation.AuthorName,
                        email = this.tagInformation.AuthorEmail,
                        date = DateTime.UtcNow.ToString(
                            "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.CurrentCulture)
                    }
                };

                this.Refresh();
                var createObjectResult = this.client.UploadString(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "repos/{0}/{1}/git/tags",
                        this.tagInformation.Owner,
                        this.tagInformation.Repository),
                    JsonConvert.SerializeObject(createObjectInput));

                return JsonConvert.DeserializeObject<JObject>(createObjectResult)["sha"]
                    .ToString();
            }

            private string GetShaForTag()
            {
                return this.tagInformation.RefOrSha.StartsWith(
                    "refs", StringComparison.CurrentCulture)
                    ? this.GetShaForTagFromReference()
                    : this.tagInformation.RefOrSha;
            }

            private string GetShaForTagFromReference()
            {
                this.Refresh();
                var getRefernceResult = this.client.DownloadString(string.Format(
                    CultureInfo.CurrentCulture,
                    "repos/{0}/{1}/git/{2}",
                    this.tagInformation.Owner,
                    this.tagInformation.Repository,
                    this.tagInformation.RefOrSha));

                return JsonConvert.DeserializeObject<JObject>(getRefernceResult)["object"]["sha"]
                    .ToString();
            }

            private void Refresh()
            {
                this.client.BaseAddress = "https://api.github.com";
                this.client.Headers["Authorization"] = string.Format(
                    CultureInfo.CurrentCulture, "token {0}", this.tagInformation.AccessToken);
                this.client.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                this.client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";
                this.client.Headers["Accept-Language"] = "en-US,en;q=0.8,el;q=0.6";
            }
        }
    }
}