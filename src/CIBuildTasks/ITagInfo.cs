namespace Jwc.CIBuildTasks
{
    public interface ITagInfo
    {
        string AccessToken { get; }

        string Owner { get; }

        string Repository { get; }

        string RefOrSha { get; }

        string TagName { get; }

        string ReleaseNotes { get; }

        string AuthorName { get; }

        string AuthorEmail { get; }
    }
}