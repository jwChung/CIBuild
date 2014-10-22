namespace Jwc.CIBuild
{
    using System.Reflection;
    using Experiment.Idioms;

    public class AssemblyTest
    {
        [Test]
        public void SutReferencesOnlySpecifiedAssemblies()
        {
            new RestrictiveReferenceAssertion(
                Assembly.Load("mscorlib"),
                Assembly.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
                Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
                Assembly.Load("System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
                Assembly.Load("Microsoft.Build.Utilities.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                Assembly.Load("Microsoft.Build.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
                Assembly.Load("Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed"),
                Assembly.Load("HtmlAgilityPack, Version=1.4.9.0, Culture=neutral, PublicKeyToken=bd319b19eaf3b43a"))
                .Verify(Assembly.Load("Jwc.CIBuildTasks"));
        }

        [Test]
        public void SutDoesNotExposeAnyTypesOfSpecifiedAssemblies()
        {
            new IndirectReferenceAssertion(
                Assembly.Load("Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed"),
                Assembly.Load("HtmlAgilityPack, Version=1.4.9.0, Culture=neutral, PublicKeyToken=bd319b19eaf3b43a"))
                .Verify(Assembly.Load("Jwc.Experiment.Idioms"));
        }
    }
}
