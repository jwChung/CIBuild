namespace Jwc.CIBuild.Tasks
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a task to create a tag on GitHub.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubTagger"/> class.
        /// </summary>
        public GitHubTagger()
            : this(new CreateTagCommand(), new TaskLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubTagger"/> class.
        /// </summary>
        /// <param name="createCommand">
        /// The command for creating a tag.
        /// </param>
        /// <param name="logger">
        /// The task logger.
        /// </param>
        public GitHubTagger(ICreateTagCommand createCommand, ITaskLogger logger)
        {
            if (createCommand == null)
                throw new ArgumentNullException("createCommand");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.createCommand = createCommand;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reference name or SHA.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the author email.
        /// </summary>
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

        /// <summary>
        /// Gets the command for creating a tag.
        /// </summary>
        public ICreateTagCommand CreateCommand
        {
            get { return this.createCommand; }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Jwc.CIBuild.ITaskLogger.Log(Microsoft.Build.Utilities.Task,System.String,Microsoft.Build.Framework.MessageImportance)", Justification = "The literal can be passed as localized parameters.")]
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