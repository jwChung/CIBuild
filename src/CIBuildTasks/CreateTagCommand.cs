namespace Jwc.CIBuildTasks
{
    using System;

    public class CreateTagCommand : ICreateTagCommand
    {
        public void Execute(ITagInfo tagInfo)
        {
            if (tagInfo == null)
                throw new ArgumentNullException("tagInfo");

            throw new System.NotImplementedException();
        }
    }
}