namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
    using Xunit;

    public class PublishNugetDeterminationTest : TestBaseClass
    {
        [Test]
        public void SutIsTask(PublishNugetDetermination sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void IdentifierIsReadWritable(PublishNugetDetermination sut, string identifier)
        {
            Assert.Null(sut.Identifier);
            sut.Identifier = identifier;
            Assert.Equal(identifier, sut.Identifier);
        }

        [Test]
        public void IdentifierIsRequired()
        {
            new Properties<PublishNugetDetermination>().Select(x => x.Identifier)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void CanPushIsReadWritable(PublishNugetDetermination sut, bool canPublish)
        {
            sut.CanPush = canPublish;
            Assert.Equal(canPublish, sut.CanPush);
        }

        [Test]
        public void CanPushIsOutput()
        {
            new Properties<PublishNugetDetermination>().Select(x => x.CanPush)
                .AssertGet<OutputAttribute>();
        }

        [Test(RunOnCI = true)]
        public IEnumerable<ITestCase> ExecuteDeterminesCorrectCanPush()
        {
            var testData = new[]
            {
                new { Identifier = "Experiment.Xunit abc", CanPush = true },
                new { Identifier = "Experiment.Xunit 2.0.0-pre02", CanPush = false },
                new { Identifier = "Experiment.Idioms    2.0.0-pre02", CanPush = false }
            };
            return TestCases.WithArgs(testData).WithAuto<PublishNugetDetermination>().Create(
                (data, sut) =>
                {
                    sut.Identifier = data.Identifier;

                    var actual = sut.Execute();

                    Assert.True(actual);
                    Assert.Equal(data.CanPush, sut.CanPush);
                });
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<PublishNugetDetermination>().Select(x => x.Identifier);
            yield return new Properties<PublishNugetDetermination>().Select(x => x.CanPush);
        }
    }
}