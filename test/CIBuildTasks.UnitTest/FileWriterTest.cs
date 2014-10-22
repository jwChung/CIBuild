namespace Jwc.CIBuild
{
    using Xunit;

    public class FileWriterTest : TestBaseClass
    {
        [Test]
        public void SutIsFileWriter(FileWriter sut)
        {
            Assert.IsAssignableFrom<IFileWriter>(sut);
        } 
    }
}