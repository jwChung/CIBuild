namespace Jwc.CIBuildTasks
{
    using System;
    using System.Globalization;
    using System.Net;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a MSBuild task to create a tag on a repository of GitHub.
    /// </summary>
    public class GitHubTagger : Task
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
        public override bool Execute()
        {
            this.CreateTag(this.tagInfo);

            this.LogMessageFromText(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "The '{0}' tag was created...",
                    this.tagInfo.GetMetadata("Name")),
                MessageImportance.High);

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
            new Tagger(this.ConverToTag(tagInformation)).CreateTag();
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
        protected virtual void LogMessageFromText(string lineOfText, MessageImportance messageImportance)
        {
            Log.LogMessageFromText(lineOfText, messageImportance);
        }

        private Tag ConverToTag(ITaskItem tagInformation)
        {
            return new Tag
            {
                AccessToken = tagInformation.GetMetadata("AccessToken"),
                Owner = tagInformation.GetMetadata("Owner"),
                Repository = tagInformation.GetMetadata("Repository"),
                Reference = tagInformation.GetMetadata("Reference"),
                Name = tagInformation.ItemSpec,
                ReleaseNotes = tagInformation.GetMetadata("ReleaseNotes"),
                AuthorName = tagInformation.GetMetadata("AuthorName"),
                AuthorEmail = tagInformation.GetMetadata("AuthorEmail")
            };
        }

        private class Tagger
        {
            private readonly WebClient client = new WebClientWithCookies();
            private readonly Tag tag;

            public Tagger(Tag tag)
            {
                this.tag = tag;
            }

            public void CreateTag()
            {
                var createReferenceInput = new
                {
                    @ref = "refs/tags/" + this.tag.Name,
                    sha = this.CreateObject()
                };

                this.Refresh();
                this.client.UploadString(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "repos/{0}/{1}/git/refs",
                        this.tag.Owner,
                        this.tag.Repository),
                    JsonConvert.SerializeObject(createReferenceInput));
            }

            private string CreateObject()
            {
                var createObjectInput = new
                {
                    tag = this.tag.Name,
                    message = this.tag.ReleaseNotes,
                    @object = this.GetReferenceSha(),
                    type = "commit",
                    tagger = new
                    {
                        name = this.tag.AuthorName,
                        email = this.tag.AuthorEmail,
                        date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszz")
                    }
                };

                this.Refresh();
                var createObjectResult = this.client.UploadString(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "repos/{0}/{1}/git/tags",
                        this.tag.Owner,
                        this.tag.Repository),
                    JsonConvert.SerializeObject(createObjectInput));

                return JsonConvert.DeserializeObject<JObject>(createObjectResult)["sha"].ToString();
            }

            private string GetReferenceSha()
            {
                this.Refresh();
                var getRefernceResult = this.client.DownloadString(string.Format(
                    CultureInfo.CurrentCulture,
                    "repos/{0}/{1}/git/refs/{2}",
                    this.tag.Owner,
                    this.tag.Repository,
                    this.tag.Reference));

                return JsonConvert.DeserializeObject<JObject>(getRefernceResult)["object"]["sha"].ToString();
            }

            private void Refresh()
            {
                this.client.BaseAddress = "https://api.github.com";
                this.client.Headers["Authorization"] = string.Format("token {0}", this.tag.AccessToken);
                this.client.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                this.client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";
                this.client.Headers["Accept-Language"] = "en-US,en;q=0.8,el;q=0.6";
            }
        }

        private class Tag
        {
            private string accessToken;
            private string owner;
            private string repository;
            private string reference;
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
                    this.EnsureNotNullOrEmpty(value);
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
                    this.EnsureNotNullOrEmpty(value);
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
                    this.EnsureNotNullOrEmpty(value);
                    this.repository = value;
                }
            }

            public string Reference
            {
                get
                {
                    return this.reference;
                }

                set
                {
                    this.reference = !string.IsNullOrEmpty(value) ? value : "heads/master";
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
                    this.EnsureNotNullOrEmpty(value);
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
                    this.EnsureNotNullOrEmpty(value);
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
                    this.EnsureNotNullOrEmpty(value);
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
                    this.EnsureNotNullOrEmpty(value);
                    this.authorEmail = value;
                }
            }

            public void EnsureNotNullOrEmpty(string value)
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();
            }
        }
    }
}