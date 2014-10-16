namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Experiment;
    using Experiment.Idioms;
    using Experiment.Xunit;
    using Ploeh.Albedo;
    using Ploeh.AutoFixture;

    public abstract class TestBaseClass
    {
        private readonly Type sutType;

        public TestBaseClass()
        {
            var testTypeName = this.GetType().FullName;
            var sutName = testTypeName.Substring(0, testTypeName.Length - 4);
            this.sutType = typeof(Base64StringToFile).Assembly.GetType(sutName);

            if (this.SutType == null)
                throw new InvalidOperationException(string.Format(
                    "SUT of the test class '{0}' was not found.",
                    testTypeName));
        }

        public TestBaseClass(Type sutType)
        {
            this.sutType = sutType;
        }

        public Type SutType
        {
            get { return this.sutType; }
        }

        [Test]
        public IEnumerable<ITestCase> SutHasAppropriateGuards()
        {
            var members = this.SutType.GetIdiomaticMembers().Except(
                this.ExceptToVerifyGuardClause());

            return TestCases.WithArgs(members).WithAuto<GuardClauseAssertion>()
                .Create((m, a) => a.Verify(m));
        }

        [Test]
        public IEnumerable<ITestCase> SutCorrectlyInitializesMembers()
        {
            var members = this.SutType.GetIdiomaticMembers().Except(
                this.ExceptToVerifyInitialization());

            return TestCases.WithArgs(members).WithAuto<MemberInitializationAssertion>()
                .Create((m, a) => a.Verify(m));
        }

        protected virtual IEnumerable<MemberInfo> ExceptToVerifyGuardClause()
        {
            yield break;
        }

        protected virtual IEnumerable<MemberInfo> ExceptToVerifyInitialization()
        {
            yield break;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Same type name with generic argument")]
    public abstract class TestBaseClass<TSut> : TestBaseClass
    {
        private readonly Properties<TSut> properties = new Properties<TSut>();

        public TestBaseClass() : base(typeof(TSut))
        {
        }
    }
}