namespace Jwc.CIBuild
{
    /// <summary>
    /// Represents information to delete a nuget package.
    /// </summary>
    public interface INugetPackageInfo
    {
        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        string UserPassword { get; }

        /// <summary>
        /// Gets the nuget identifier.
        /// </summary>
        string NugetId { get; }

        /// <summary>
        /// Gets the nuget version.
        /// </summary>
        string NugetVersion { get; }
    }
}