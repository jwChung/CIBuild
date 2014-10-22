namespace Jwc.CIBuild.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture.Xunit;
    using Xunit;

    public class Base64StringToFileTest : TestBaseClass
    {
        [Test]
        public void SutIsTask(Base64StringToFile sut)
        {
            Assert.IsAssignableFrom<Task>(sut);
        }

        [Test]
        public void FileWriterIsCorrect(Base64StringToFile sut)
        {
            Assert.IsAssignableFrom<FileWriter>(sut.FileWriter);
        }

        [Test]
        public void InputIsReadWritable(Base64StringToFile sut, string input)
        {
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
            [Greedy] Base64StringToFile sut,
            byte[] value)
        {
            sut.Input = Convert.ToBase64String(value);

            var actual = sut.Execute();

            Assert.True(actual);
            sut.FileWriter.ToMock().Verify(x => x.Write(sut.OutputFile, value));
        }

        protected override IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield return new Properties<Base64StringToFile>().Select(x => x.Input);
            yield return new Properties<Base64StringToFile>().Select(x => x.OutputFile);
        }
    }
}