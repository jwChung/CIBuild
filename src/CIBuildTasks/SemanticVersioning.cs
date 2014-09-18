namespace Jwc.CIBuildTasks
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to get semantic version from AssemblyInfo file.
    /// </summary>
    public class SemanticVersioning : Task
    {
        private string assemblyInfo;
        private string semanticVersion;

        /// <summary>
        /// Gets or sets the file path of the assembly information.
        /// </summary>
        [Required]
        public string AssemblyInfo
        {
            get
            {
                return this.assemblyInfo;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.assemblyInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the semantic version.
        /// </summary>
        [Output]
        public string SemanticVersion
        {
            get
            {
                return this.semanticVersion;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.semanticVersion = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Jwc.CIBuildTasks.SemanticVersioning.LogError(System.String,System.Object[])", Justification = "As there are very few violations for this rule, passing literal is simpler.")]
        public override bool Execute()
        {
            var content = File.ReadAllText(this.AssemblyInfo, Encoding.UTF8);
            
            var match = Regex.Match(
                content,
                @"\[assembly:\s+AssemblyInformationalVersion\(\s*""(?<semver>.+?)""\s*\)]");

            if (!match.Success)
                match = Regex.Match(
                    content,
                    @"\[assembly:\s+AssemblyVersion\(\s*""(?<semver>.+?)""\s*\)]");

            if (!match.Success)
            {
                this.LogError(
                    "The AssemblyInfo '{0}' does not have valid semantic version.",
                    this.assemblyInfo);

                return true;
            }
            
            this.semanticVersion = match.Result("${semver}");
            return true;
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">
        /// The message format.
        /// </param>
        /// <param name="messageArgs">
        /// The arguments of the message format.
        /// </param>
        protected virtual void LogError(string message, params object[] messageArgs)
        {
            Log.LogError(message, messageArgs);
        }
    }
}