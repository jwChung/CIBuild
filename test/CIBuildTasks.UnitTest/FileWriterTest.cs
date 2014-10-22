namespace Jwc.CIBuildTasks
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