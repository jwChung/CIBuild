namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using Experiment;
    using Experiment.AutoFixture;
    using Experiment.Xunit;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Kernel;
    using Xunit.Sdk;

    public class TestAttribute : TestBaseAttribute
    {
        private readonly CIBuildFixtureFactory factory = new CIBuildFixtureFactory();
        private readonly RunOn runOn;

        public TestAttribute() : this(RunOn.Any)
        {
        }

        public TestAttribute(RunOn runOn) : base(new CompositeTestCommandFactory(
            new TestCaseCommandFactory(),
            new DataAttributeCommandFactory(),
            new ParameterizedCommandFactory(),
            new ForceFixtureCommandFactory()))
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
            return this.factory.Create(context);
        }

        private class ForceFixtureCommandFactory : ITestCommandFactory
        {
            public IEnumerable<ITestCommand> Create(
                IMethodInfo testMethod, ITestFixtureFactory fixtureFactory)
            {
                yield return new ParameterizedCommand(
                    new ForceFitxtureContext(testMethod, fixtureFactory));
            }
        }

        private class ForceFitxtureContext : ParameterizedCommandContext
        {
            private readonly ITestFixtureFactory factory;

            public ForceFitxtureContext(IMethodInfo testMethod, ITestFixtureFactory factory)
                : base(testMethod, factory, new object[0])
            {
                this.factory = factory;
            }

            public override ITestMethodContext GetMethodContext(object testObject)
            {
                var methodContext = base.GetMethodContext(testObject);
                this.factory.Create(methodContext);
                return methodContext;
            }
        }

        private class CIBuildFixtureFactory : TestFixtureFactory
        {
            protected override ICustomization GetCustomization(ITestMethodContext context)
            {
                return new CompositeCustomization(
                    base.GetCustomization(context),
                    new TestClassCustomization(context));
            }
        }

        private class TestClassCustomization : ICustomization
        {
            private readonly ITestMethodContext context;

            public TestClassCustomization(ITestMethodContext context)
            {
                this.context = context;
            }

            public void Customize(IFixture fixture)
            {
                var customization = this.context.TestObject as ICustomization;
                if (customization != null)
                    fixture.Customize(customization);
            }
        }
    }
}