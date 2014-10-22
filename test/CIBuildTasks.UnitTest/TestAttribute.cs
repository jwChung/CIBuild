namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using Experiment;
    using Experiment.AutoFixture;
    using Experiment.Xunit;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;
    using Ploeh.AutoFixture.Kernel;
    using Xunit.Sdk;

    public class TestAttribute : TestBaseAttribute
    {
        private readonly RunOn runOn;

        public TestAttribute() : this(RunOn.Any)
        {
        }

        public TestAttribute(RunOn runOn)
        {
            this.runOn = runOn;
        }

        public RunOn RunOn
        {
            get { return this.runOn; }
        }

        public override string Skip
        {
            get
            {
#if !CI
                if (base.Skip == null && this.runOn == RunOn.CI)
                    return "Run this test only on CI server.";
#endif
                return base.Skip;
            }

            set
            {
                base.Skip = value;
            }
        }

        protected override ITestFixture Create(ITestMethodContext context)
        {
            var customization = new CompositeCustomization(
                new AutoMoqCustomization(),
                new OmitAutoPropertiesCustomization(),
                new TestParametersCustomization(context.ActualMethod.GetParameters()),
                new MockedSutCustomization());
            var fixture = new Fixture().Customize(customization);
            return new TestFixture(fixture);
        }

        private class MockedSutCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Register(() => Mocked.Of<GitHubTagger2>());
                fixture.Register(() => Mocked.Of<SemanticVersioning>());
                fixture.Register(() => Mocked.Of<Base64StringToFile>());
            }
        }
    }
}