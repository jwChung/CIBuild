namespace Jwc.CIBuildTasks
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class GitHubTagger : Task, ITagInfo
    {
        private readonly ICreateTagCommand createCommand;
        private readonly ITaskLogger logger;
        private string accessToken;
        private string owner;
        private string repository;
        private string refOrSha = "refs/heads/master";
        private string tagName;
        private string releaseNotes;
        private string authorName;
        private string authorEmail;

        public GitHubTagger()
            : this(new CreateTagCommand(), new TaskLogger())
        {
        }

        public GitHubTagger(ICreateTagCommand createCommand, ITaskLogger logger)
        {
            if (createCommand == null)
                throw new ArgumentNullException("createCommand");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.createCommand = createCommand;
            this.logger = logger;
        }

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The AccessToken property should not be empty string.");

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The Owner property should not be empty string.");

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The Repository property should not be empty string.");

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

                if (value.Length == 0)
                    return;

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The TagName property should not be empty string.");

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The ReleaseNotes property should not be empty string.");

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The AuthorName property should not be empty string.");

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

                if (value.Length == 0)
                    throw new ArgumentException(
                        "The AuthorEmail property should not be empty string.");

                this.authorEmail = value;
            }
        }

        public ICreateTagCommand CreateCommand
        {
            get { return this.createCommand; }
        }

        public ITaskLogger Logger
        {
            get { return this.logger; }
        }

        public override bool Execute()
        {
            try
            {
                this.createCommand.Execute(this);
            }
            catch (WebException exception)
            {
                throw new InvalidOperationException(
                    GetExceptionMessage(exception),
                    exception);
            }

            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The '{0}' tag was created.",
                this.tagName);

            this.Logger.Log(this, message, MessageImportance.High);

            return true;
        }

        private static string GetExceptionMessage(WebException exception)
        {
            return exception.Message
                + Environment.NewLine
                + new StreamReader(
                    exception.Response.GetResponseStream(),
                    Encoding.UTF8).ReadToEnd();
        }
    }
}