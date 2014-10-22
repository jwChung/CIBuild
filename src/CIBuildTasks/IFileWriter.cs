namespace Jwc.CIBuildTasks
{
    /// <summary>
    /// Represents file writer.
    /// </summary>
    public interface IFileWriter
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
        void Write(string path, byte[] value);
    }
}