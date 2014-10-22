﻿namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture.Xunit;
    using Xunit;

    public class GitHubTaggerTest : TestBaseClass
    {
        [Test]
        public void SutIsTask(GitHubTagger sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void TagInfoIsReadWritable(GitHubTagger sut, ITaskItem tagInfo)
        {
            Assert.Null(sut.TagInfo);
            sut.TagInfo = tagInfo;
            Assert.Equal(tagInfo, sut.TagInfo);
        }

        [Test]
        public void TagInfoIsRequired()
        {
            new Properties<GitHubTagger>().Select(x => x.TagInfo).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCorrectlyCreatesTag(
            GitHubTagger sut,
            ITaskItem tagInfo)
        {
            sut.ToMock().CallBase = false;
            sut.TagInfo = tagInfo;

            var actual = sut.Execute();

            Assert.True(actual);
            sut.ToMock().Protected().Verify(
                "CreateTag",
                Times.Once(),
                tagInfo);
        }

        [Test]
        public void ExecuteLogsCorrectMessage(
            GitHubTagger sut,
            ITaskItem tagInfo,
            string tagName)
        {
            sut.ToMock().CallBase = false;
            sut.TagInfo = tagInfo.Of(x => x.ItemSpec == tagName);

            sut.Execute();

            sut.ToMock().Protected().Verify(
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

            return TestCases.WithArgs(testData).WithAuto<string, ITaskItem, GitHubTagger>().Create(
                (data, value, tagInfo, sut) =>
                {
                    tagInfo.Of(x => x.GetMetadata(It.IsAny<string>()) == value
                        && x.ItemSpec == value
                        && x.GetMetadata(data.Name) == data.Value);
                    sut.TagInfo = tagInfo;
                    Assert.Equal(data.Value, tagInfo.GetMetadata(data.Name));

                    var e = Assert.Throws<ArgumentException>(() => sut.Execute());
                    Assert.Contains(data.Name, e.Message);
                }).Concat(TestCases.WithArgs(new[] { null, string.Empty })
                    .WithAuto<string, ITaskItem, GitHubTagger>().Create(
                        (data, value, tagInfo, sut) =>
                        {
                            tagInfo.Of(x => x.GetMetadata(It.IsAny<string>()) == value
                                && x.ItemSpec == data);
                            sut.TagInfo = tagInfo;

                            var e = Assert.Throws<ArgumentException>(() => sut.Execute());
                            Assert.Contains("Name", e.Message);
                        }));
        }

        [Test(Skip = "Specify the github AccessToken, explicitly run this test and verify whether the tag is actually created on the github website.")]
        public void ExecuteCorrectlyCreatesActualTag(
             GitHubTagger sut,
            ITaskItem tagInfo)
        {
            // Fixture setup
            tagInfo.Of(
                i => i.GetMetadata("AccessToken") == "**************"
                    && i.GetMetadata("Owner") == "jwChung"
                    && i.GetMetadata("Repository") == "CIBuild"
                    && i.GetMetadata("ReleaseNotes") == "test"
                    && i.GetMetadata("AuthorName") == "Jin-Wook Chung"
                    && i.GetMetadata("AuthorEmail") == "abc@abc.com"
                    && i.ItemSpec == Guid.NewGuid().ToString("N"));
            sut.TagInfo = tagInfo;

            // Exercise system
            sut.Execute();

            // Verify outcome
            sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Never(),
                ItExpr.IsAny<string>(),
                ItExpr.IsAny<MessageImportance>());
        }

        [Test]
        public void ExecuteLogsCorrectMessageWhenCreatingTagFails(
            GitHubTagger sut,
            [Frozen] WebResponse response,
            [Greedy] WebException exception,
            string message)
        {
            response.Of(r => r.GetResponseStream()
                == new MemoryStream(Encoding.UTF8.GetBytes(message)));
            sut.ToMock().Protected().Setup("CreateTag", ItExpr.IsAny<ITaskItem>())
                .Throws(exception);
            var expectedMessage = exception.Message + Environment.NewLine + message;

            sut.Execute();

            sut.ToMock().Protected().Verify(
                "LogMessageFromText",
                Times.Once(),
                expectedMessage,
                MessageImportance.High);
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<GitHubTagger>().Select(x => x.TagInfo);
        }
    }
}