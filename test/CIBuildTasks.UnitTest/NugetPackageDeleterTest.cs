namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture.Xunit;
    using Xunit;

    public class NugetPackageDeleterTest : TestBaseClass
    {
        [Test]
        public void SutIsNugetPackageDeletionInfo(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<IDeletePackageCommandArgs>(sut);
        }

        [Test]
        public void SutIsTask(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void NugetPackageDeletionIsCorrect(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<DeletePackageCommand>(sut.DeleteCommand);
        }

        [Test]
        public void LoggerIsCorrect(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<TaskLogger>(sut.Logger);
        }

        [Test]
        public void UserIdIsReadWritable(NugetPackageDeleter sut, string userId)
        {
            Assert.Null(sut.UserId);
            sut.UserId = userId;
            Assert.Equal(userId, sut.UserId);
        }

        [Test]
        public void UserIdIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.UserId)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void UserPasswordIsReadWritable(NugetPackageDeleter sut, string userPassword)
        {
            Assert.Null(sut.UserPassword);
            sut.UserPassword = userPassword;
            Assert.Equal(userPassword, sut.UserPassword);
        }

        [Test]
        public void UserPasswordIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.UserPassword)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void NugetIdIsReadWritable(NugetPackageDeleter sut, string nugetId)
        {
            Assert.Null(sut.NugetId);
            sut.NugetId = nugetId;
            Assert.Equal(nugetId, sut.NugetId);
        }

        [Test]
        public void NugetIdIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.NugetId)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void NugetVersionIsReadWritable(NugetPackageDeleter sut, string nugetVersion)
        {
            Assert.Null(sut.NugetVersion);
            sut.NugetVersion = nugetVersion;
            Assert.Equal(nugetVersion, sut.NugetVersion);
        }

        [Test]
        public void NugetVersionIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.NugetVersion)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCorrectlyDeletesNugetPackage([Greedy] NugetPackageDeleter sut)
        {
            var actual = sut.Execute();

            Assert.True(actual);
            sut.DeleteCommand.ToMock().Verify(x => x.Delete(sut), Times.Once());
        }

        [Test]
        public void ExecuteLogsCorrectMessage(
            [Greedy] NugetPackageDeleter sut, string nugetId, string nugetVersion)
        {
            sut.NugetId = nugetId;
            sut.NugetVersion = nugetVersion;

            sut.Execute();

            sut.Logger.ToMock().Verify(x => x.Log(
                sut,
                It.Is<string>(p => p.Contains(nugetId) && p.Contains(nugetVersion)),
                MessageImportance.High));
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<NugetPackageDeleter>().Select(x => x.UserId);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.UserPassword);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.NugetId);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.NugetVersion);
        }
    }
}