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

    public abstract class TestClassBase
    {
        private readonly Type sutType;

        public TestClassBase()
        {
            var testTypeName = this.GetType().FullName;
            var sutName = testTypeName.Substring(0, testTypeName.Length - 4);
            this.sutType = typeof(Base64StringToFile).Assembly.GetType(sutName);

            if (this.SutType == null)
                throw new InvalidOperationException(string.Format(
                    "SUT of the test class '{0}' was not found.",
                    testTypeName));
        }

        public TestClassBase(Type sutType)
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
            var members = this.SutType.GetIdiomaticMembers().Except(this.ExceptToVerifyGuardClause());
            return TestCases.WithArgs(members).WithAuto<GuardClauseAssertion>()
                .Create((m, a) => a.Verify(m));
        }

        [Test]
        public IEnumerable<ITestCase> SutCorrectlyInitializesMembers()
        {
            var members = this.SutType.GetIdiomaticMembers().Except(this.ExceptToVerifyInitialization());
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
    public abstract class TestClassBase<TSut> : TestClassBase, ISutSpecimen<TSut>
    {
        private readonly Properties<TSut> properties = new Properties<TSut>();

        public TestClassBase() : base(typeof(TSut))
        {
        }

        public TSut Sut { get; set; }

        public Properties<TSut> Properties
        {
            get { return this.properties; }
        }
    }
}