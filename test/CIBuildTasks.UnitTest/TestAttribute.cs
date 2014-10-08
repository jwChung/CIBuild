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
        private readonly bool runOnCI = false;

        public TestAttribute() : base(new CompositeTestCommandFactory(
            new TestCaseCommandFactory(),
            new DataAttributeCommandFactory(),
            new ParameterizedCommandFactory(),
            new ForceFixtureCommandFactory()))
        {
        }

        public bool RunOnCI
        {
            get
            {
                return this.runOnCI;
            }

            set
            {
#if !CI
                if (value)
                    this.Skip = "Run this test only on CI server.";
#endif
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
                    new TestClassCustomization(context),
                    new SutSpecimenCustomization(context));
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

        private class SutSpecimenCustomization : ICustomization
        {
            private readonly ITestMethodContext context;

            public SutSpecimenCustomization(ITestMethodContext context)
            {
                this.context = context;
            }

            public void Customize(IFixture fixture)
            {
                var testObject = this.context.ActualObject as TestBaseClass;
                if (testObject != null)
                    TrySetSutSpecimen(fixture, testObject);
            }

            private static void TrySetSutSpecimen(IFixture fixture, TestBaseClass testObject)
            {
                var sutType = testObject.SutType;
                var sutSpecimenType = typeof(TestBaseClass<>).MakeGenericType(sutType);
                if (sutSpecimenType.IsInstanceOfType(testObject))
                {
                    sutSpecimenType.GetProperty("Sut").SetValue(
                        testObject,
                        new SpecimenContext(fixture).Resolve(sutType),
                        null);
                }
            }
        }
    }
}