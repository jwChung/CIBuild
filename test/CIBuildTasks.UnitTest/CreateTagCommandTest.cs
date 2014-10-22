namespace Jwc.CIBuildTasks
{
    using Xunit;

    public class CreateTagCommandTest : TestBaseClass
    {
        [Test]
        public void SutIsCreateTagCommand(CreateTagCommand sut)
        {
            Assert.IsAssignableFrom<ICreateTagCommand>(sut);
        }
    }
}