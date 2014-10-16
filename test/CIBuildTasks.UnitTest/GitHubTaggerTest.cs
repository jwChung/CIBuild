﻿namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.AutoFixture;
    using Xunit;

    public class GitHubTaggerTest : TestBaseClass<GitHubTagger>, ICustomization
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
                tagInfo);
        }

        [Test]
        public void ExecuteLogsCorrectMessage(ITaskItem tagInfo, string tagName)
        {
            this.Sut.TagInfo = tagInfo.Of(x => x.ItemSpec == tagName);

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
                new { Name = "ReleaseNotes", Value = (string)null },
                new { Name = "ReleaseNotes", Value = string.Empty },
                new { Name = "AuthorName", Value = (string)null },
                new { Name = "AuthorName", Value = string.Empty },
                new { Name = "AuthorEmail", Value = (string)null },
                new { Name = "AuthorEmail", Value = string.Empty }
            };

            return TestCases.WithArgs(testData).WithAuto<string, ITaskItem>().Create(
                (data, value, tagInfo) =>
                {
                    var sut = new GitHubTagger();
                    tagInfo.Of(x => x.GetMetadata(It.IsAny<string>()) == value
                        && x.ItemSpec == value
                        && x.GetMetadata(data.Name) == data.Value);
                    sut.TagInfo = tagInfo;
                    Assert.Equal(data.Value, tagInfo.GetMetadata(data.Name));

                    var e = Assert.Throws<ArgumentException>(() => sut.Execute());
                    Assert.Contains(data.Name, e.Message);
                }).Concat(
                    TestCases.WithArgs(new[] { null, string.Empty }).WithAuto<string, ITaskItem>().Create(
                    (data, value, tagInfo) =>
                    {
                        var sut = new GitHubTagger();
                        tagInfo.Of(x => x.GetMetadata(It.IsAny<string>()) == value
                            && x.ItemSpec == data);
                        sut.TagInfo = tagInfo;

                        var e = Assert.Throws<ArgumentException>(() => sut.Execute());
                        Assert.Contains("Name", e.Message);
                    }));
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.TagInfo);
        }
    }
}