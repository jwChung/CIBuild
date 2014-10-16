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

    public class SemanticVersioningTest : TestBaseClass
    {
        [Test]
        public void SutIsTask(SemanticVersioning sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void AssemblyInfoIsReadWritable(SemanticVersioning sut, string assemblyInfo)
        {
            Assert.Null(sut.AssemblyInfo);
            sut.AssemblyInfo = assemblyInfo;
            Assert.Equal(assemblyInfo, sut.AssemblyInfo);
        }

        [Test]
        public void AssemblyInfoIsRequired()
        {
            new Properties<SemanticVersioning>().Select(x => x.AssemblyInfo)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void SemanticVersionIsReadWritable(SemanticVersioning sut, string semanticVersion)
        {
            Assert.Null(sut.SemanticVersion);
            sut.SemanticVersion = semanticVersion;
            Assert.Equal(semanticVersion, sut.SemanticVersion);
        }

        [Test]
        public void SemanticVersionIsOutput()
        {
            new Properties<SemanticVersioning>().Select(x => x.SemanticVersion)
                .AssertGet<OutputAttribute>();
        }

        [Test]
        public IEnumerable<ITestCase> ExecuteBuildsCorrectSemanticVersion()
        {
            var testData = new[]
            {
                new
                {
                    assemblyInfoContent = "   [assembly: AssemblyVersion(\"0.0.1\")]   ",
                    SemanticVersion = "0.0.1"
                },
                new
                {
                    assemblyInfoContent = "   [assembly:   AssemblyVersion( \"0.0.1\" )]   ",
                    SemanticVersion = "0.0.1"
                },
                new
                {
                    assemblyInfoContent = "   [assembly: AssemblyInformationalVersion(\"0.0.1\")]   ",
                    SemanticVersion = "0.0.1"
                },
                new
                {
                    assemblyInfoContent = "  [assembly: AssemblyInformationalVersion(\"0.0.2\")]  [assembly: AssemblyVersion(\"0.0.1\")]   ",
                    SemanticVersion = "0.0.2"
                },
                new
                {
                    assemblyInfoContent = "   [assembly: AssemblyVersion(\"0.0.3\")][assembly: ABC(\"0.0.1\")]   ",
                    SemanticVersion = "0.0.3"
                },
                ////new
                ////{
                ////    assemblyInfoContent = File.ReadAllText(@"..\..\..\..\CommonAssemblyInfo.cs"),
                ////    SemanticVersion = "0.0.1-pre06"
                ////}
            };
            return TestCases.WithArgs(testData).WithAuto<SemanticVersioning, string>().Create(
                (data, sut, fileName) =>
                {
                    try
                    {
                        File.WriteAllText(fileName, data.assemblyInfoContent);
                        sut.AssemblyInfo = fileName;

                        var actual = sut.Execute();

                        Assert.True(actual);
                        Assert.Equal(data.SemanticVersion, sut.SemanticVersion);
                        sut.ToMock().Protected().Verify(
                           "LogError",
                           Times.Never(),
                           ItExpr.IsAny<string>(),
                           ItExpr.IsAny<object[]>());
                    }
                    finally
                    {
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                    }
                });
        }

        [Test]
        public void ExecuteThrowsIfSemanticVersionIsNotDefined(
            SemanticVersioning sut,
            string fileName,
            string content)
        {
            try
            {
                File.WriteAllText(fileName, content);
                sut.AssemblyInfo = fileName;

                var actual = sut.Execute();

                Assert.True(actual);
                sut.ToMock().Protected().Verify(
                    "LogError",
                    Times.Once(),
                    "The AssemblyInfo '{0}' does not have valid semantic version.",
                    new object[] { fileName });
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<SemanticVersioning>().Select(x => x.AssemblyInfo);
            yield return new Properties<SemanticVersioning>().Select(x => x.SemanticVersion);
        }
    }
}