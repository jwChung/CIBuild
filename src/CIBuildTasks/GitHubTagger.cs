namespace Jwc.CIBuildTasks
{
    using System;
    using System.Globalization;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

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
                    this.tagInfo.GetMetadata("TagName")),
                MessageImportance.High);

            return true;
        }

        /// <summary>
        /// Creates a tag.
        /// </summary>
        /// <param name="tag">
        /// The tag information.
        /// </param>
        protected virtual void CreateTag(ITaskItem tag)
        {
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
    }
}