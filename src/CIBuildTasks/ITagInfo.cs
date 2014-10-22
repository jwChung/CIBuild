namespace Jwc.CIBuildTasks
{
    /// <summary>
    /// Represents tag information.
    /// </summary>
    public interface ITagInfo
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        string Owner { get; }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        string Repository { get; }

        /// <summary>
        /// Gets the reference name or SHA.
        /// </summary>
        string RefOrSha { get; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        string TagName { get; }

        /// <summary>
        /// Gets the release notes.
        /// </summary>
        string ReleaseNotes { get; }

        /// <summary>
        /// Gets the name of the author.
        /// </summary>
        string AuthorName { get; }

        /// <summary>
        /// Gets the author email.
        /// </summary>
        string AuthorEmail { get; }
    }
}