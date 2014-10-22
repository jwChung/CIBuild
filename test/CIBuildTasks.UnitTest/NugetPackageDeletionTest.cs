namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Experiment.Xunit;
    using Xunit;

    public class NugetPackageDeletionTest : TestBaseClass
    {
        [Test]
        public void SutIsNugetPackageDeletion(NugetPackageDeletion sut)
        {
            Assert.IsAssignableFrom<INugetPackageDeletion>(sut);
        }

        [Test(Skip = "Specify the user id and password for nuget site, explicitly run this test and verify whether the nuget package is actually deleted on the nuget website.")]
        public void DeleteCorrectlyDeletesNugetPackage(
            NugetPackageDeletion sut,
            INugetPackageDeletionInfo nugetInfo)
        {
            SetupVallidNugetPackageDeletionInfo(nugetInfo);

            Assert.DoesNotThrow(() => sut.Delete(nugetInfo));
        }

        [Test(Skip = "Specify the user id and password for nuget site, explicitly run this test.")]
        public IEnumerable<ITestCase> DeleteThrowsWhenNugetPackageDeletionInfoIsInvalid()
        {
            yield return TestCase.WithAuto<NugetPackageDeletion, INugetPackageDeletionInfo>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.UserId == "userid");
                    Assert.Throws<InvalidOperationException>(() => sut.Delete(nugetInfo));
                });

            yield return TestCase.WithAuto<NugetPackageDeletion, INugetPackageDeletionInfo>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.UserPassword == "userPassword");
                    Assert.Throws<InvalidOperationException>(() => sut.Delete(nugetInfo));
                });

            yield return TestCase.WithAuto<NugetPackageDeletion, INugetPackageDeletionInfo>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.NugetId == "nugetId");
                    Assert.Throws<InvalidOperationException>(() => sut.Delete(nugetInfo));
                });

            yield return TestCase.WithAuto<NugetPackageDeletion, INugetPackageDeletionInfo>()
                .Create((sut, nugetInfo) =>
                {
                    SetupVallidNugetPackageDeletionInfo(nugetInfo);
                    nugetInfo.Of(x => x.NugetVersion == "nugetVersion");
                    Assert.Throws<WebException>(() => sut.Delete(nugetInfo));
                });
        }

        private static void SetupVallidNugetPackageDeletionInfo(INugetPackageDeletionInfo nugetInfo)
        {
            nugetInfo.Of(x =>
                x.UserId == "*****"
                && x.UserPassword == "*****"
                && x.NugetId == "CIBuild.Scripts"
                && x.NugetVersion == "0.0.1");
        }
    }
}