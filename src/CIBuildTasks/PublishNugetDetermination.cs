namespace Jwc.CIBuildTasks
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to determine whether packages of a certain version can be
    /// published.
    /// </summary>
    public class PublishNugetDetermination : Task
    {
        private string identifier;

        /// <summary>
        /// Gets or sets the Nuget identifier, which consists of id and version.
        /// </summary>
        [Required]
        public string Identifier
        {
            get
            {
                return this.identifier;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether packages can publish.
        /// </summary>
        [Output]
        public bool CanPush { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            this.DetermineCanPush();
            return true;
        }

        private void DetermineCanPush()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(this.GetUrl());
                using (request.GetResponse())
                {
                }

                this.CanPush = false;
            }
            catch (WebException exception)
            {
                if (((HttpWebResponse)exception.Response).StatusCode != HttpStatusCode.NotFound)
                    throw;

                this.CanPush = true;
            }
        }

        private string GetUrl()
        {
            return "https://www.nuget.org/packages/" + Regex.Replace(this.Identifier, @"\s+", "/");
        }
    }
}