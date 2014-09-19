namespace Jwc.CIBuildTasks
{
    using System;
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
        public void ExecuteLogsCorrectMessage(ITaskItem tagInfo, string tagName)
        {
            this.Sut.TagInfo = tagInfo.Of(x => x.GetMetadata("Name") == tagName);

            this.Sut.Execute();

            this.Sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Once(),
                ItExpr.Is<string>(x => x.Contains(tagName)),
                MessageImportance.High);
        }

        [Test]
        public IEnumerable<ITestCase> ExecuteThrowsIfMetadataNamesOfTagInfoIsInvalid()
        {
            var testData = new[]
            {
                new { Name = "AccessToken", Value = (string)null },
                new { Name = "AccessToken", Value = string.Empty },
                new { Name = "Owner", Value = (string)null },
                new { Name = "Owner", Value = string.Empty },
                new { Name = "Repository", Value = (string)null },
                new { Name = "Repository", Value = string.Empty },
                new { Name = "Reference", Value = (string)null },
                new { Name = "Reference", Value = string.Empty },
                new { Name = "Name", Value = (string)null },
                new { Name = "Name", Value = string.Empty },
                new { Name = "ReleaseNotes", Value = (string)null },
                new { Name = "ReleaseNotes", Value = string.Empty },
                new { Name = "AuthorName", Value = (string)null },
                new { Name = "AuthorName", Value = string.Empty },
                new { Name = "AuthorEmail", Value = (string)null },
                new { Name = "AuthorEmail", Value = string.Empty }
            };

            return TestCases.WithArgs(testData).WithAuto<ITaskItem>().Create(
                (data, tagInfo) =>
                {
                    var sut = new GitHubTagger();
                    tagInfo.Of(x => x.GetMetadata("AccessToken") == "AccessToken"
                        && x.GetMetadata("Owner") == "Owner"
                        && x.GetMetadata("Repository") == "Repository"
                        && x.GetMetadata("Reference") == "Reference"
                        && x.GetMetadata("Name") == "Name"
                        && x.GetMetadata("ReleaseNotes") == "ReleaseNotes"
                        && x.GetMetadata("AuthorName") == "AuthorName"
                        && x.GetMetadata("AuthorEmail") == "AuthorEmail"
                        && x.GetMetadata(data.Name) == data.Value);
                    sut.TagInfo = tagInfo;
                    Assert.Equal(data.Value, tagInfo.GetMetadata(data.Name));

                    Assert.Throws<ArgumentException>(() => sut.Execute());
                });
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.TagInfo);
        }
    }
}