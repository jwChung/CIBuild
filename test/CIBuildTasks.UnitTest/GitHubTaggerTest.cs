namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.AutoFixture;
    using Xunit;

    public class GitHubTaggerTest : TestClassBase<GitHubTagger>, ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var sut = Mocked.Of<GitHubTagger>();

                sut.ToMock().Protected().Setup(
                    "CreateTag",
                    ItExpr.IsAny<ITaskItem>());

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
        public void TagInfoIsReadWritable(ITaskItem tagInfo)
        {
            Assert.Null(this.Sut.TagInfo);
            this.Sut.TagInfo = tagInfo;
            Assert.Equal(tagInfo, this.Sut.TagInfo);
        }

        [Test]
        public void TagInfoIsRequired()
        {
            this.Properties.Select(x => x.TagInfo).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCorrectlyCreateTag(ITaskItem tagInfo)
        {
            this.Sut.TagInfo = tagInfo;

            var actual = this.Sut.Execute();

            Assert.True(actual);
            this.Sut.ToMock().Protected().Verify(
                "CreateTag",
                Times.Once(),
                ItExpr.IsAny<ITaskItem>());
        }

        [Test]
        public void ExecuteLogsCorrectMessage(ITaskItem taskInfo, string tagName)
        {
            this.Sut.TagInfo = taskInfo.Of(x => x.GetMetadata("TagName") == tagName);

            this.Sut.Execute();

            this.Sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Once(),
                ItExpr.Is<string>(x => x.Contains(tagName)),
                MessageImportance.High);
        }

        [Test]
        public IEnumerable<ITestCase> ExecuteThrowsIfMetadataNamesOfTagInfoIsIncorrect()
        {
            ////var testData = new[]
            ////{
            ////    new []{ "Repository", "Referene", "TagName", "ReleaseNotes", "AccessToken", "AuthorName", "AuthorEmail" }
            ////};

            ////return TestCases.WithArgs(testData).WithAuto<ITaskItem>().Create(
            ////    (data, tagInfo) =>
            ////    {
            ////        tagInfo.Of(x => x.MetadataNames = data);
            ////    });
            yield break;
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.TagInfo);
        }
    }
}