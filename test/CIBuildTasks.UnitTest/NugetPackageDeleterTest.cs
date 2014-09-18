namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
    using Xunit;

    public class NugetPackageDeleterTest : TestClassBase<NugetPackageDeleter>
    {
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

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.IdOrEmail);
            yield return this.Properties.Select(x => x.Password);
            yield return this.Properties.Select(x => x.Identifier);
        }
    }
}