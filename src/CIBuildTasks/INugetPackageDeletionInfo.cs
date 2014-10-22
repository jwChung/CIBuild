namespace Jwc.CIBuildTasks
{
    public interface INugetPackageDeletionInfo
    {
        string UserId { get; }

        string UserPassword { get; }

        string NugetId { get; }

        string NugetVersion { get; }
    }
}