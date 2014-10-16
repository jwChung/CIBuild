namespace Jwc.CIBuildTasks
{
    using System;
    using System.IO;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to save bytes from base 64 string to a file.
    /// </summary>
    public class Base64StringToFile : Task
    {
        private string input;
        private string outputFile;

        /// <summary>
        /// Gets or sets the base 64 string.
        /// </summary>
        [Required]
        public string Input
        {
            get
            {
                return this.input;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.input = value;
            }
        }

        /// <summary>
        /// Gets or sets the file path to save bytes from base 64 string.
        /// </summary>
        [Required]
        public string OutputFile
        {
            get
            {
                return this.outputFile;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.outputFile = value;
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
            this.WriteAllBytes(this.outputFile, Convert.FromBase64String(this.input));
            return true;
        }

        /// <summary>
        /// Writes all bytes to a file.
        /// </summary>
        /// <param name="path">
        /// A file path to save the bytes.
        /// </param>
        /// <param name="value">
        /// The bytes.
        /// </param>
        protected virtual void WriteAllBytes(string path, byte[] value)
        {
            File.WriteAllBytes(path, value);
        }
    }
}