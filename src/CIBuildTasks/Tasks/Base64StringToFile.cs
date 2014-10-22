namespace Jwc.CIBuild.Tasks
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to save bytes from base 64 string to a file.
    /// </summary>
    public class Base64StringToFile : Task
    {
        private readonly IFileWriter fileWriter;
        private string input;
        private string outputFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Base64StringToFile"/> class.
        /// </summary>
        public Base64StringToFile() : this(new FileWriter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base64StringToFile"/> class.
        /// </summary>
        /// <param name="fileWriter">
        /// The file writer.
        /// </param>
        public Base64StringToFile(IFileWriter fileWriter)
        {
            if (fileWriter == null)
                throw new ArgumentNullException("fileWriter");

            this.fileWriter = fileWriter;
        }

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
        /// Gets the file writer.
        /// </summary>
        public IFileWriter FileWriter
        {
            get { return this.fileWriter; }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public sealed override bool Execute()
        {
            this.fileWriter.Write(this.outputFile, Convert.FromBase64String(this.input));
            return true;
        }
    }
}