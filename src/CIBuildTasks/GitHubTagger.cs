namespace Jwc.CIBuildTasks
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class GitHubTagger : Task, ITagInfo
    {
        private string accessToken;
        private string owner;
        private string repository;
        private string refOrSha;
        private string tagName;
        private string releaseNotes;
        private string authorName;
        private string authorEmail;

        [Required]
        public string AccessToken
        {
            get
            {
                return this.accessToken;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.accessToken = value;
            }
        }

        [Required]
        public string Owner
        {
            get
            {
                return this.owner;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.owner = value;
            }
        }

        [Required]
        public string Repository
        {
            get
            {
                return this.repository;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.repository = value;
            }
        }

        [Required]
        public string RefOrSha
        {
            get
            {
                return this.refOrSha;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.refOrSha = value;
            }
        }

        [Required]
        public string TagName
        {
            get
            {
                return this.tagName;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.tagName = value;
            }
        }

        [Required]
        public string ReleaseNotes
        {
            get
            {
                return this.releaseNotes;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.releaseNotes = value;
            }
        }

        [Required]
        public string AuthorName
        {
            get
            {
                return this.authorName;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.authorName = value;
            }
        }

        [Required]
        public string AuthorEmail
        {
            get
            {
                return this.authorEmail;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.authorEmail = value;
            }
        }

        public override bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}