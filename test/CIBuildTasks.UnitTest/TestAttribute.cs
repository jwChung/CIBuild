namespace Jwc.CIBuildTasks
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Experiment;
    using Experiment.Xunit;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;
    using Ploeh.AutoFixture.Kernel;
    using Ploeh.AutoFixture.Xunit;
    using Xunit.Sdk;

    public class TestAttribute : TestBaseAttribute
    {
        public TestAttribute() : base(new CompositeTestCommandFactory(
            new TestCaseCommandFactory(),
            new DataAttributeCommandFactory(),
            new ParameterizedCommandFactory(),
            new ForceFixtureCommandFactory()))
        {
        }

        protected override ITestFixture Create(ITestMethodContext context)
        {
            var fixture = new Fixture().Customize(new CompositeCustomization(
                new AutoMoqCustomization(),
                new OmitAutoPropertiesCustomization(),
                new ParameterCustomization(context),
                new TestClassCustomization(context),
                new SutSpecimenCustomization(context)));

            return new TestFixture(fixture);
        }

        private class TestFixture : ITestFixture
        {
            private readonly ISpecimenContext context;

            public TestFixture(IFixture fixture)
            {
                this.InjectTestFixture(fixture);
                this.context = new SpecimenContext(fixture);
            }

            public object Create(object request)
            {
                return this.context.Resolve(request);
            }

            private void InjectTestFixture(IFixture fixture)
            {
                fixture.Inject(this);
                fixture.Inject<ITestFixture>(this);
            }
        }

        private class ForceFixtureCommandFactory : ITestCommandFactory
        {
            public IEnumerable<ITestCommand> Create(IMethodInfo testMethod, ITestFixtureFactory fixtureFactory)
            {
                yield return new ParameterizedCommand(new ForceFitxtureContext(testMethod, fixtureFactory));
            }
        }

        private class ForceFitxtureContext : ITestCommandContext
        {
            private readonly ITestFixtureFactory factory;
            private readonly ITestCommandContext commandContext;

            public ForceFitxtureContext(IMethodInfo testMethod, ITestFixtureFactory factory)
            {
                this.factory = factory;
                this.commandContext = new TestCommandContext(testMethod, factory, new object[0]);
            }

            public IMethodInfo TestMethod
            {
                get { return this.commandContext.TestMethod; }
            }

            public IEnumerable<object> GetArguments(ITestMethodContext context)
            {
                return this.commandContext.GetArguments(context);
            }

            public ITestMethodContext GetMethodContext(object testObject)
            {
                var methodContext = this.commandContext.GetMethodContext(testObject);
                this.factory.Create(methodContext);
                return methodContext;
            }
        }

        private class OmitAutoPropertiesCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.OmitAutoProperties = true;
            }
        }

        private class ParameterCustomization : ICustomization
        {
            private readonly ITestMethodContext context;

            public ParameterCustomization(ITestMethodContext context)
            {
                this.context = context;
            }

            public void Customize(IFixture fixture)
            {
                var customization = new CompositeCustomization(
                    this.context.ActualMethod.GetParameters().SelectMany(
                        p => p.GetCustomAttributes(typeof(CustomizeAttribute), false)
                            .Cast<CustomizeAttribute>()
                            .Select(a => a.GetCustomization(p))));

                fixture.Customize(customization);
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