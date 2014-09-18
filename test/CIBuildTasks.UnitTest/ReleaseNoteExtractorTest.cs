namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture;
    using Xunit;

    public class ReleaseNoteExtractorTest : TestClassBase<ReleaseNoteExtractor>
    {
        [Test]
        public void SutIsTask()
        {
            Assert.IsAssignableFrom<Task>(this.Sut);
        }

        [Test]
        public void AssemblyInfoIsReadWritable(string assemblyInfo)
        {
            Assert.Null(this.Sut.AssemblyInfo);
            this.Sut.AssemblyInfo = assemblyInfo;
            Assert.Equal(assemblyInfo, this.Sut.AssemblyInfo);
        }

        [Test]
        public void AssemblyInfoIsRequired()
        {
            this.Properties.Select(x => x.AssemblyInfo).AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ReleaseNotesIsReadWritable(string releaseNotes)
        {
            Assert.Null(this.Sut.ReleaseNotes);
            this.Sut.ReleaseNotes = releaseNotes;
            Assert.Equal(releaseNotes, this.Sut.ReleaseNotes);
        }

        [Test]
        public void ReleaseNotesIsOutput()
        {
            this.Properties.Select(x => x.ReleaseNotes).AssertGet<OutputAttribute>();
        }

        [Test]
        public void XmlEscapedReleaseNotesIsReadWritable(string xmlEscapedReleaseNotes)
        {
            Assert.Null(this.Sut.XmlEscapedReleaseNotes);
            this.Sut.XmlEscapedReleaseNotes = xmlEscapedReleaseNotes;
            Assert.Equal(xmlEscapedReleaseNotes, this.Sut.XmlEscapedReleaseNotes);
        }

        [Test]
        public void XmlEscapedReleaseNotesIsOutput()
        {
            this.Properties.Select(x => x.XmlEscapedReleaseNotes).AssertGet<OutputAttribute>();
        }

        [Test]
        public IEnumerable<ITestCase> ExecuteExtractsCorrectReleaseNotes()
        {
            var testData = new[]
            {
                new { AssemblyInfoContent = string.Empty, ReleaseNotes = string.Empty },
                new
                {
                    AssemblyInfoContent = @"asdfafdafd asdfasd
asd
fas
df
asdf
a",
                    ReleaseNotes = string.Empty
                },
                new
                {
                    AssemblyInfoContent = @"/*expected*/",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"/*  expected */",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"/*  
expected
*/",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"sdfsdf 
sdfsdf

/*expected*/

sdfs

sdfsd
",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"sdfsdf 
sdfsdf

/*

                expected
*/

/*expected2*/

sdfs

sdfsd
",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"
/*
 * expected
 */
",
                    ReleaseNotes = "expected"
                },
                new
                {
                    AssemblyInfoContent = @"
/**
 *   expected
 *   - test: sadadfa  
 *     asfasdasfd
 */
",
                    ReleaseNotes = @"expected
- test: sadadfa
  asfasdasfd"
                },
                new
                {
                    AssemblyInfoContent = @"
/****
 *     
 *   expected
 *   - test: sadadfa  
 *     asfasdasfd
 *
 */
",
                    ReleaseNotes = @"expected
- test: sadadfa
  asfasdasfd"
                },
                new
                {
                    AssemblyInfoContent = @"
/****
 *     
 * ***
 *
 */
",
                    ReleaseNotes = string.Empty
                }
            };
            return TestCases.WithArgs(testData).WithAuto<string>().Create(
                (data, fileName) =>
                {
                    try
                    {
                        File.WriteAllText(fileName, data.AssemblyInfoContent);
                        this.Sut.AssemblyInfo = fileName;

                        var actual = this.Sut.Execute();

                        Assert.True(actual);
                        Assert.Equal(data.ReleaseNotes, this.Sut.ReleaseNotes);
                    }
                    finally
                    {
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                    }
                });
        }

        [Test]
        public void ExecuteExtractsCorrectXmlEscapedReleaseNotes(string fileName)
        {
            var assemblyInfoContent = @"/*expected Func<string> expected */";
            var escapedExpected = "expected Func&lt;string&gt; expected";
            var expected = "expected Func<string> expected";
            try
            {
                File.WriteAllText(fileName, assemblyInfoContent);
                this.Sut.AssemblyInfo = fileName;

                var actual = this.Sut.Execute();

                Assert.True(actual);
                Assert.Equal(escapedExpected, this.Sut.XmlEscapedReleaseNotes);
                Assert.Equal(expected, this.Sut.ReleaseNotes);
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return this.Properties.Select(x => x.AssemblyInfo);
            yield return this.Properties.Select(x => x.ReleaseNotes);
            yield return this.Properties.Select(x => x.XmlEscapedReleaseNotes);
        }
    }
}