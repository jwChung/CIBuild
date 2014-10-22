namespace Jwc.CIBuildTasks
{
    using Xunit;

    public class TaskLoggerTest : TestBaseClass
    {
        [Test]
        public void SutIsTaskLogger(TaskLogger sut)
        {
            Assert.IsAssignableFrom<ITaskLogger>(sut);
        }
    }
}