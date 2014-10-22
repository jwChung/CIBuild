namespace Jwc.CIBuildTasks
{
    /// <summary>
    /// Represents command for deleting a nuget package.
    /// </summary>
    public interface IDeletePackageCommand
    {
        /// <summary>
        /// Deletes the specified nuget package.
        /// </summary>
        /// <param name="deleteCommandArgs">
        /// The information to delete a nuget package .
        /// </param>
        void Delete(IDeletePackageCommandArgs deleteCommandArgs);
    }
}