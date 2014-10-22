namespace Jwc.CIBuild
{
    /// <summary>
    /// Represents command for deleting a nuget package.
    /// </summary>
    public interface IDeletePackageCommand
    {
        /// <summary>
        /// Deletes the specified nuget package.
        /// </summary>
        /// <param name="nugetPackageInfo">
        /// The information to delete a nuget package .
        /// </param>
        void Execute(INugetPackageInfo nugetPackageInfo);
    }
}