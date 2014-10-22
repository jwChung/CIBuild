namespace Jwc.CIBuildTasks
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents task logger.
    /// </summary>
    public class TaskLogger : ITaskLogger
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
        public void Log(Task task, string message, MessageImportance messageImportance)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (message == null)
                throw new ArgumentNullException("message");

            task.Log.LogMessageFromText(message, messageImportance);
        }
    }
}