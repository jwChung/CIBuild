namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.AutoFixture;
    using Xunit;

    public class NugetPackageDeleterTest : TestBaseClass<NugetPackageDeleter>, ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var sut = Mocked.Of<NugetPackageDeleter>();

                sut.ToMock().Protected().Setup(
                    "DeletePackage",
                    ItExpr.IsAny<string>(),
                    ItExpr.IsAny<string>(),
                    ItExpr.IsAny<string>());

                sut.ToMock().Protected().Setup(
                    "LogMessageFromText",
                    ItExpr.IsAny<string>(),
                    ItExpr.IsAny<MessageImportance>());

                return sut;
            });
        }

        [Test]
        public void SutIsTask()
        {
            Assert.IsAssignableFrom<Task>(this.Sut);
        }

        [Test]
        public void IdOrEmailIsReadWritable(string idOrEmail)
        {
            Assert.Null(this.Sut.IdOrEmail);
            this.Sut.IdOrEmail = idOrEmail;
            Assert.Equal(idOrEmail, this.Sut.IdOrEmail);
        }

        [Test]
        public void IdOrEmailIsRequired()
        {
            this.Properties.Select(x => x.IdOrEmail).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void PasswordIsReadWritable(string password)
        {
            Assert.Null(this.Sut.Password);

            this.Sut.Password = password;

            Assert.Equal(password, this.Sut.Password);
            Assert.NotEqual(this.Sut.IdOrEmail, this.Sut.Password);
        }

        [Test]
        public void PasswordIsRequired()
        {
            this.Properties.Select(x => x.Password).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void IdentifierIsReadWritable(string identifier)
        {
            Assert.Null(this.Sut.Identifier);

            this.Sut.Identifier = identifier;

            Assert.Equal(identifier, this.Sut.Identifier);
            Assert.NotEqual(this.Sut.Password, this.Sut.Identifier);
            Assert.NotEqual(this.Sut.IdOrEmail, this.Sut.Identifier);
        }

        [Test]
        public void IdentifierIsRequired()
        {
            this.Properties.Select(x => x.Identifier).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCorrectlyDeleteNugetPackage(string id, string pwd, string package)
        {
            this.Sut.IdOrEmail = id;
            this.Sut.Password = pwd;
            this.Sut.Identifier = package;

            var actual = this.Sut.Execute();

            Assert.True(actual);
            this.Sut.ToMock().Protected().Verify("DeletePackage", Times.Once(), id, pwd, package);
        }

        [Test]
        public void ExecuteLogCorrectMessage(string package)
        {
            this.Sut.Identifier = package;

            this.Sut.Execute();

            this.Sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Once(),
                ItExpr.Is<string>(x => x.Contains(package)),
                MessageImportance.High);
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.IdOrEmail);
            yield return this.Properties.Select(x => x.Password);
            yield return this.Properties.Select(x => x.Identifier);
        }
    }
}