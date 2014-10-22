namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Experiment.Xunit;
    using Xunit;

    public class DeletePackageCommandTest : TestBaseClass
    {
        [Test]
        public void SutIsNugetPackageDeletion(DeletePackageCommand sut)
        {
            Assert.IsAssignableFrom<IDeletePackageCommand>(sut);
        }

        [Test(Skip = "Specify the user id and password for nuget site, explicitly run this test and verify whether the nuget package is actually deleted on the nuget website.")]
        public void DeleteCorrectlyDeletesNugetPackage(
            DeletePackageCommand sut,
            IDeletePackageCommandArgs args)
        {
            SetupVallidNugetPackageDeletionInfo(args);

            Assert.DoesNotThrow(() => sut.Execute(args));
        }

        [Test(Skip = "Specify the user id and password for nuget site, explicitly run this test.")]
        public IEnumerable<ITestCase> DeleteThrowsWhenNugetPackageDeletionInfoIsInvalid()
        {
            yield return TestCase.WithAuto<DeletePackageCommand, IDeletePackageCommandArgs>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.UserId == "userid");
                    Assert.Throws<InvalidOperationException>(() => sut.Execute(nugetInfo));
                });

            yield return TestCase.WithAuto<DeletePackageCommand, IDeletePackageCommandArgs>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.UserPassword == "userPassword");
                    Assert.Throws<InvalidOperationException>(() => sut.Execute(nugetInfo));
                });

            yield return TestCase.WithAuto<DeletePackageCommand, IDeletePackageCommandArgs>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.NugetId == "nugetId");
                    Assert.Throws<InvalidOperationException>(() => sut.Execute(nugetInfo));
                });

            yield return TestCase.WithAuto<DeletePackageCommand, IDeletePackageCommandArgs>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.NugetVersion == "nugetVersion");
                    Assert.Throws<WebException>(() => sut.Execute(nugetInfo));
                });
        }

        private static void SetupVallidNugetPackageDeletionInfo(IDeletePackageCommandArgs args)
        {
            args.Of(x =>
                x.UserId == "*****"
                && x.UserPassword == "*****"
                && x.NugetId == "CIBuild.Scripts"
                && x.NugetVersion == "0.0.1");
        }
    }
}