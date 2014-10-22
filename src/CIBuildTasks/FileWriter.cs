namespace Jwc.CIBuildTasks
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents implementation of file writer.
    /// </summary>
    public class FileWriter : IFileWriter
    {
        /// <summary>
        /// Writes byte array to the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="value">
        /// The byte array.
        /// </param>
        public void Write(string path, byte[] value)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (value == null)
                throw new ArgumentNullException("value");

            File.WriteAllBytes(path, value);
        }
    }
}