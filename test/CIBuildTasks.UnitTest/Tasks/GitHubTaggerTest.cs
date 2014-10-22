namespace Jwc.CIBuild.Tasks
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
        public void SutIsTagInfo(GitHubTagger sut)
        {
            Assert.IsAssignableFrom<ITagInfo>(sut);
        }

        [Test]
        public void CreateTagCommandIsCorrect(GitHubTagger sut)
        {
            Assert.IsAssignableFrom<CreateTagCommand>(sut.CreateCommand);
        }

        [Test]
        public void LoggerIsCorrect(GitHubTagger sut)
        {
            Assert.IsAssignableFrom<TaskLogger>(sut.Logger);
        }

        [Test]
        public IEnumerable<ITestCase> RefOrShaIsCorrect(GitHubTagger sut)
        {
            yield return TestCase.Create(() => Assert.Equal("refs/heads/master", sut.RefOrSha));

            sut.RefOrSha = string.Empty;
            yield return TestCase.Create(() => Assert.Equal("refs/heads/master", sut.RefOrSha));
        }

        [Test]
        public IEnumerable<ITestCase> PropertiesAreReadWritable()
        {
            var testData = new[]
            {
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.AccessToken),
                    Set = new Action<GitHubTagger, string>((x, y) => x.AccessToken = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.Owner),
                    Set = new Action<GitHubTagger, string>((x, y) => x.Owner = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.Repository),
                    Set = new Action<GitHubTagger, string>((x, y) => x.Repository = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.RefOrSha),
                    Set = new Action<GitHubTagger, string>((x, y) => x.RefOrSha = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.TagName),
                    Set = new Action<GitHubTagger, string>((x, y) => x.TagName = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.ReleaseNotes),
                    Set = new Action<GitHubTagger, string>((x, y) => x.ReleaseNotes = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.AuthorName),
                    Set = new Action<GitHubTagger, string>((x, y) => x.AuthorName = y)
                },
                new
                {
                    Get = new Func<GitHubTagger, string>(x => x.AuthorEmail),
                    Set = new Action<GitHubTagger, string>((x, y) => x.AuthorEmail = y)
                },
            };

            return TestCases.WithArgs(testData).WithAuto<GitHubTagger, string>().Create(
                (data, sut, value) =>
                {
                    data.Set(sut, value);

                    Assert.Equal(value, data.Get(sut));
                    testData.Except(new[] { data }).All(x =>
                    {
                        Assert.NotEqual(value, x.Get(sut));
                        return true;
                    });
                });
        }

        [Test]
        public IEnumerable<ITestCase> PropertiesAreRequired()
        {
            return TestCases.WithArgs(GetProperties()).Create(property =>
            {
                property.AssertGet<RequiredAttribute>();
            });
        }

        [Test]
        public IEnumerable<ITestCase> PropertiesThrowsWhenInitializedWithEmptyString()
        {
            var testData = new[]
            {
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.AccessToken = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.Owner = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.Repository = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.TagName = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.ReleaseNotes = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.AuthorName = y)
                },
                new
                {
                    Set = new Action<GitHubTagger, string>((x, y) => x.AuthorEmail = y)
                },
            };

            return TestCases.WithArgs(testData).WithAuto<GitHubTagger>().Create(
                (data, sut) =>
                {
                    Assert.Throws<ArgumentException>(() => data.Set(sut, string.Empty));
                });
        }

        [Test]
        public void ExecuteCorrectlyCreatesTag(
            [Greedy] GitHubTagger sut)
        {
            var actual = sut.Execute();
            Assert.True(actual);
            sut.CreateCommand.ToMock().Verify(x => x.Execute(sut));
        }

        [Test]
        public void ExecuteLogsCorrectMessage(
            [Greedy] GitHubTagger sut,
            string tagName)
        {
            sut.TagName = tagName;

            sut.Execute();

            sut.Logger.ToMock().Verify(x => x.Log(
                sut,
                It.Is<string>(p => p.Contains(tagName)),
                MessageImportance.High));
        }

        [Test]
        public void ExecuteLogsCorrectMessageWhenCreatingTagFails(
            [Greedy] GitHubTagger sut,
            [Frozen] WebResponse response,
            [Greedy] WebException exception,
            string message)
        {
            // Fixture setup
            response.Of(r => r.GetResponseStream()
                == new MemoryStream(Encoding.UTF8.GetBytes(message)));
            sut.CreateCommand.ToMock().Setup(x => x.Execute(It.IsAny<ITagInfo>()))
                .Throws(exception);
            var expectedMessage = exception.Message + Environment.NewLine + message;

            // Exercise system and verify outcome
            var e = Assert.Throws<InvalidOperationException>(() => sut.Execute());
            Assert.Equal(expectedMessage, e.Message);
            Assert.Equal(exception, e.InnerException);
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            return GetProperties();
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
            yield return new Properties<GitHubTagger>().Select(x => x.AccessToken);
            yield return new Properties<GitHubTagger>().Select(x => x.Owner);
            yield return new Properties<GitHubTagger>().Select(x => x.Repository);
            yield return new Properties<GitHubTagger>().Select(x => x.RefOrSha);
            yield return new Properties<GitHubTagger>().Select(x => x.TagName);
            yield return new Properties<GitHubTagger>().Select(x => x.ReleaseNotes);
            yield return new Properties<GitHubTagger>().Select(x => x.AuthorName);
            yield return new Properties<GitHubTagger>().Select(x => x.AuthorEmail);
        }
    }
}