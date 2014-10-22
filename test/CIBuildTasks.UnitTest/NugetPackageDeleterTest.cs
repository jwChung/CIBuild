namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.Albedo;
    using Xunit;

    public class NugetPackageDeleterTest : TestBaseClass
    {
        [Test]
        public void SutIsNugetPackageDeletionInfo(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<INugetPackageDeletionInfo>(sut);
        }

        [Test]
        public void SutIsTask(NugetPackageDeleter sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
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
        public void IdOrEmailIsReadWritable(NugetPackageDeleter sut, string idOrEmail)
        {
            Assert.Null(sut.IdOrEmail);
            sut.IdOrEmail = idOrEmail;
            Assert.Equal(idOrEmail, sut.IdOrEmail);
        }

        [Test]
        public void IdOrEmailIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.IdOrEmail)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void PasswordIsReadWritable(NugetPackageDeleter sut, string password)
        {
            Assert.Null(sut.Password);

            sut.Password = password;

            Assert.Equal(password, sut.Password);
            Assert.NotEqual(sut.IdOrEmail, sut.Password);
        }

        [Test]
        public void PasswordIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.Password)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void IdentifierIsReadWritable(NugetPackageDeleter sut, string identifier)
        {
            Assert.Null(sut.Identifier);

            sut.Identifier = identifier;

            Assert.Equal(identifier, sut.Identifier);
            Assert.NotEqual(sut.Password, sut.Identifier);
            Assert.NotEqual(sut.IdOrEmail, sut.Identifier);
        }

        [Test]
        public void IdentifierIsRequired()
        {
            new Properties<NugetPackageDeleter>().Select(x => x.Identifier)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCorrectlyDeleteNugetPackage(
            NugetPackageDeleter sut, string id, string pwd, string package)
        {
            sut.ToMock().CallBase = false;
            sut.IdOrEmail = id;
            sut.Password = pwd;
            sut.Identifier = package;

            var actual = sut.Execute();

            Assert.True(actual);
            sut.ToMock().Protected().Verify("DeletePackage", Times.Once(), id, pwd, package);
        }

        [Test]
        public void ExecuteLogsCorrectMessage(NugetPackageDeleter sut, string package)
        {
            sut.ToMock().CallBase = false;
            sut.Identifier = package;

            sut.Execute();

            sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Once(),
                ItExpr.Is<string>(x => x.Contains(package)),
                MessageImportance.High);
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<NugetPackageDeleter>().Select(x => x.IdOrEmail);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.Password);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.Identifier);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.UserId);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.UserPassword);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.NugetId);
            yield return new Properties<NugetPackageDeleter>().Select(x => x.NugetVersion);
        }
    }
}