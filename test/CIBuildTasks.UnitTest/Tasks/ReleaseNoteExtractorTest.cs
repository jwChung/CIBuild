namespace Jwc.CIBuild.Tasks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
    using Xunit;

    public class ReleaseNoteExtractorTest : TestBaseClass
    {
        [Test]
        public void SutIsTask(ReleaseNoteExtractor sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void AssemblyInfoIsReadWritable(ReleaseNoteExtractor sut, string assemblyInfo)
        {
            Assert.Null(sut.AssemblyInfo);
            sut.AssemblyInfo = assemblyInfo;
            Assert.Equal(assemblyInfo, sut.AssemblyInfo);
        }

        [Test]
        public void AssemblyInfoIsRequired()
        {
            new Properties<ReleaseNoteExtractor>().Select(x => x.AssemblyInfo)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ReleaseNotesIsReadWritable(ReleaseNoteExtractor sut, string releaseNotes)
        {
            Assert.Null(sut.ReleaseNotes);
            sut.ReleaseNotes = releaseNotes;
            Assert.Equal(releaseNotes, sut.ReleaseNotes);
        }

        [Test]
        public void ReleaseNotesIsOutput()
        {
            new Properties<ReleaseNoteExtractor>().Select(x => x.ReleaseNotes)
                .AssertGet<OutputAttribute>();
        }

        [Test]
        public void XmlEscapedReleaseNotesIsReadWritable(ReleaseNoteExtractor sut, string xmlEscapedReleaseNotes)
        {
            Assert.Null(sut.XmlEscapedReleaseNotes);
            sut.XmlEscapedReleaseNotes = xmlEscapedReleaseNotes;
            Assert.Equal(xmlEscapedReleaseNotes, sut.XmlEscapedReleaseNotes);
        }

        [Test]
        public void XmlEscapedReleaseNotesIsOutput()
        {
            new Properties<ReleaseNoteExtractor>().Select(x => x.XmlEscapedReleaseNotes)
                .AssertGet<OutputAttribute>();
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
            return TestCases.WithArgs(testData).WithAuto<string, ReleaseNoteExtractor>().Create(
                (data, fileName, sut) =>
                {
                    try
                    {
                        File.WriteAllText(fileName, data.AssemblyInfoContent);
                        sut.AssemblyInfo = fileName;

                        var actual = sut.Execute();

                        Assert.True(actual);
                        Assert.Equal(
                            data.ReleaseNotes.Replace("\r", string.Empty),
                            sut.ReleaseNotes.Replace("\r", string.Empty));
                    }
                    finally
                    {
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                    }
                });
        }

        [Test]
        public void ExecuteExtractsCorrectXmlEscapedReleaseNotes(ReleaseNoteExtractor sut, string fileName)
        {
            var assemblyInfoContent = @"/*expected Func<string> expected */";
            var escapedExpected = "expected Func&lt;string&gt; expected";
            var expected = "expected Func<string> expected";
            try
            {
                File.WriteAllText(fileName, assemblyInfoContent);
                sut.AssemblyInfo = fileName;

                var actual = sut.Execute();

                Assert.True(actual);
                Assert.Equal(escapedExpected, sut.XmlEscapedReleaseNotes);
                Assert.Equal(expected, sut.ReleaseNotes);
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<ReleaseNoteExtractor>().Select(x => x.AssemblyInfo);
            yield return new Properties<ReleaseNoteExtractor>().Select(x => x.ReleaseNotes);
            yield return new Properties<ReleaseNoteExtractor>().Select(x => x.XmlEscapedReleaseNotes);
        }
    }
}