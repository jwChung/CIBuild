namespace Jwc.CIBuildTasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents task logger.
    /// </summary>
    public interface ITaskLogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="task">
        /// A task to provide actual logger.
        /// </param>
        /// <param name="message">
        /// Message.
        /// </param>
        /// <param name="messageImportance">
        /// Message importance.
        /// </param>
        void Log(Task task, string message, MessageImportance messageImportance);
    }
}