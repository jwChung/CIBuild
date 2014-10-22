namespace Jwc.CIBuildTasks
{
    /// <summary>
    /// Represents command for creating a tag.
    /// </summary>
    public interface ICreateTagCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="tagInfo">
        /// The tag information.
        /// </param>
        void Execute(ITagInfo tagInfo);
    }
}