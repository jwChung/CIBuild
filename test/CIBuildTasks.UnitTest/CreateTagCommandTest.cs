namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using Jwc.Experiment.Xunit;
    using Xunit;

    public class CreateTagCommandTest : TestBaseClass
    {
        [Test]
        public void SutIsCreateTagCommand(CreateTagCommand sut)
        {
            Assert.IsAssignableFrom<ICreateTagCommand>(sut);
        }

        [Test(Skip = "Specify the github AccessToken, explicitly run this test and verify whether the tag is actually created on the github website.")]
        public void ExecuteCorrectlyCreatesTag(
            CreateTagCommand sut,
            ITagInfo tagInfo)
        {
            tagInfo.Of(
                i => i.AccessToken == "*******"
                    && i.Owner == "jwChung"
                    && i.Repository == "CIBuild"
                    && i.ReleaseNotes == "test"
                    && i.AuthorName == "Jin-Wook Chung"
                    && i.AuthorEmail == "jwchung@hotmail.com"
                    && i.RefOrSha == "refs/heads/master"
                    && i.TagName == Guid.NewGuid().ToString("N"));

            Assert.DoesNotThrow(() => sut.Execute(tagInfo));
        }
    }
}