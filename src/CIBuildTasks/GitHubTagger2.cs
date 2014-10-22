namespace Jwc.CIBuildTasks
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a MSBuild task to create a tag on a repository of GitHub.
    /// </summary>
    public class GitHubTagger2 : Task
    {
        private ITaskItem tagInfo;

        /// <summary>
        /// Gets or sets the tag information.
        /// </summary>
        [Required]
        public ITaskItem TagInfo
        {
            get
            {
                return this.tagInfo;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.tagInfo = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public sealed override bool Execute()
        {
            try
            {
                this.CreateTag(this.tagInfo);

                this.LogMessageFromText(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "The '{0}' tag was created.",
                    this.tagInfo.ItemSpec),
                MessageImportance.High);
            }
            catch (WebException exception)
            {
                var message = GetExceptionMessage(exception);

                this.LogMessageFromText(
                    message,
                    MessageImportance.High);
            }

            return true;
        }

        /// <summary>
        /// Creates a tag.
        /// </summary>
        /// <param name="tagInformation">
        /// The tag information.
        /// </param>
        protected virtual void CreateTag(ITaskItem tagInformation)
        {
            using (var tagger = new Tagger(ConvertToTagInformation(tagInformation)))
            {
                tagger.CreateTag();
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

        private static string GetExceptionMessage(WebException exception)
        {
            return exception.Message
                + Environment.NewLine
                + new StreamReader(
                    exception.Response.GetResponseStream(),
                    Encoding.UTF8).ReadToEnd();
        }

        private static TagInformation ConvertToTagInformation(ITaskItem tagInformation)
        {
            return new TagInformation
            {
                AccessToken = tagInformation.GetMetadata("AccessToken"),
                Owner = tagInformation.GetMetadata("Owner"),
                Repository = tagInformation.GetMetadata("Repository"),
                RefOrSha = tagInformation.GetMetadata("RefOrSha"),
                Name = tagInformation.ItemSpec,
                ReleaseNotes = tagInformation.GetMetadata("ReleaseNotes"),
                AuthorName = tagInformation.GetMetadata("AuthorName"),
                AuthorEmail = tagInformation.GetMetadata("AuthorEmail")
            };
        }

        private sealed class Tagger : IDisposable
        {
            private readonly WebClient client = new WebClientWithCookies();
            private readonly TagInformation tagInformation;
            private bool disposed;

            public Tagger(TagInformation tagInformation)
            {
                this.tagInformation = tagInformation;
            }

            public void CreateTag()
            {
                var createReferenceInput = new
                {
                    @ref = "refs/tags/" + this.tagInformation.Name,
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
                    tag = this.tagInformation.Name,
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

        private class TagInformation
        {
            private string accessToken;
            private string owner;
            private string repository;
            private string refOrSha;
            private string name;
            private string releaseNotes;
            private string authorName;
            private string authorEmail;

            public string AccessToken
            {
                get
                {
                    return this.accessToken;
                }

                set
                {
                    EnsureNotNullOrEmpty("AccessToken", value);
                    this.accessToken = value;
                }
            }

            public string Owner
            {
                get
                {
                    return this.owner;
                }

                set
                {
                    EnsureNotNullOrEmpty("Owner", value);
                    this.owner = value;
                }
            }

            public string Repository
            {
                get
                {
                    return this.repository;
                }

                set
                {
                    EnsureNotNullOrEmpty("Repository", value);
                    this.repository = value;
                }
            }

            public string RefOrSha
            {
                get
                {
                    return this.refOrSha;
                }

                set
                {
                    this.refOrSha = !string.IsNullOrEmpty(value) ? value : "refs/heads/master";
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    EnsureNotNullOrEmpty("Name", value);
                    this.name = value;
                }
            }

            public string ReleaseNotes
            {
                get
                {
                    return this.releaseNotes;
                }

                set
                {
                    EnsureNotNullOrEmpty("ReleaseNotes", value);
                    this.releaseNotes = value;
                }
            }

            public string AuthorName
            {
                get
                {
                    return this.authorName;
                }

                set
                {
                    EnsureNotNullOrEmpty("AuthorName", value);
                    this.authorName = value;
                }
            }

            public string AuthorEmail
            {
                get
                {
                    return this.authorEmail;
                }

                set
                {
                    EnsureNotNullOrEmpty("AuthorEmail", value);
                    this.authorEmail = value;
                }
            }

            public static void EnsureNotNullOrEmpty(string name, string value)
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture,
                        "The '{0}' value cannot be null or empty.",
                        name));
            }
        }
    }
}