﻿namespace Jwc.CIBuild.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Experiment.Xunit;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
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
            return TestCases.WithArgs(testData).WithAuto<SemanticVersioning>().Create(
                (data, sut) =>
                {
                    try
                    {
                        File.WriteAllText(sut.AssemblyInfo, data.assemblyInfoContent);
                        var actual = sut.Execute();
                        Assert.True(actual);
                    }
                    finally
                    {
                        if (File.Exists(sut.AssemblyInfo))
                            File.Delete(sut.AssemblyInfo);
                    }
                });
        }

        [Test]
        public void ExecuteThrowsWhenSemanticVersionIsNotDefined(
            SemanticVersioning sut, string content)
        {
            try
            {
                File.WriteAllText(sut.AssemblyInfo, content);

                var e = Assert.Throws<InvalidOperationException>(() => sut.Execute());
                Assert.Contains(sut.AssemblyInfo, e.Message);
            }
            finally
            {
                if (File.Exists(sut.AssemblyInfo))
                    File.Delete(sut.AssemblyInfo);
            }
        }
        
        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<SemanticVersioning>().Select(x => x.AssemblyInfo);
            yield return new Properties<SemanticVersioning>().Select(x => x.SemanticVersion);
        }
    }
}