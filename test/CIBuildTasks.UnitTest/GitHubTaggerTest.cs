namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
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
                    Assert.Null(data.Get(sut));
                    
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
        public IEnumerable<ITestCase> PropertriesAreRequired()
        {
            return TestCases.WithArgs(GetProperties()).Create(property =>
            {
                property.AssertGet<RequiredAttribute>();
            });
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

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            return GetProperties();
        }
    }
}