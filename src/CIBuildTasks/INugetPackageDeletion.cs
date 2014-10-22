﻿namespace Jwc.CIBuildTasks
{
    /// <summary>
    /// Represents implementation for deleting a nuget package.
    /// </summary>
    public interface INugetPackageDeletion
    {
        /// <summary>
        /// Deletes the specified nuget package.
        /// </summary>
        /// <param name="nugetInfo">
        /// The nuget package information to be deleted.
        /// </param>
        void Delete(INugetPackageDeletionInfo nugetInfo);
    }
}