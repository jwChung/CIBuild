namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Moq;
    using Moq.Protected;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture;
    using Xunit;

    public class Base64StringToFileTest : TestClassBase, ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var sut = Mocked.Of<Base64StringToFile>();
                sut.ToMock().Protected().Setup(
                    "WriteAllBytes",
                    ItExpr.IsAny<string>(),
                    ItExpr.IsAny<byte[]>());

                return sut;
            });
        }

        [Test]
        public void SutIsTask(Base64StringToFile sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void InputIsReadWritable(Base64StringToFile sut, string input)
        {
            Assert.Null(sut.Input);
            sut.Input = input;
            Assert.Equal(input, sut.Input);
        }

        [Test]
        public void InputIsRequired()
        {
            new Properties<Base64StringToFile>().Select(x => x.Input)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void OutputFileIsReadWritable(Base64StringToFile sut, string outputFile)
        {
            Assert.Null(sut.OutputFile);
            sut.OutputFile = outputFile;
            Assert.Equal(outputFile, sut.OutputFile);
        }

        [Test]
        public void OutputFileIsRequired()
        {
            new Properties<Base64StringToFile>().Select(x => x.OutputFile)
                .AssertGet<RequiredAttribute>();
        }

        [Test]
        public void ExecuteCreatesCorrectTextFile(
            Base64StringToFile sut,
            byte[] value,
            string outputFile)
        {
            sut.Input = Convert.ToBase64String(value);
            sut.OutputFile = outputFile;

            var actual = sut.Execute();

            Assert.True(actual);
            sut.ToMock().Protected().Verify("WriteAllBytes", Times.Once(), outputFile, value);
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<Base64StringToFile>().Select(x => x.Input);
            yield return new Properties<Base64StringToFile>().Select(x => x.OutputFile);
        }
    }
}