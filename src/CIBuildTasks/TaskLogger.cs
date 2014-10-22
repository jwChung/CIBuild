namespace Jwc.CIBuildTasks
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class TaskLogger : ITaskLogger
    {
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