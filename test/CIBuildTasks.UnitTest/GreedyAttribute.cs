namespace Jwc.CIBuild
{
    using System;
    using System.Reflection;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Kernel;
    using Ploeh.AutoFixture.Xunit;

    /// <summary>
    /// An attribute that can be applied to parameters in an <see cref="AutoDataAttribute"/>-driven
    /// Theory to indicate that the parameter value should be created using the most greedy
    /// constructor that can be satisfied by an <see cref="IFixture"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class GreedyAttribute : CustomizeAttribute
    {
        /// <summary>
        /// Gets a customization that associates a <see cref="GreedyConstructorQuery"/> with the
        /// <see cref="Type"/> of the parameter.
        /// </summary>
        /// <param name="parameter">The parameter for which the customization is requested.</param>
        /// <returns>
        /// A customization that associates a <see cref="GreedyConstructorQuery"/> with the
        /// <see cref="Type"/> of the parameter.
        /// </returns>
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            
            return new ConstructorCustomization(parameter.ParameterType, new GreedyConstructorQuery());
        }

        /// <summary>
        /// A customization that uses a particular constructor selection mechanism to pick and invoke
        /// a constructor to create specimens of the targeted type.
        /// </summary>
        private class ConstructorCustomization : ICustomization
        {
            private readonly Type targetType;
            private readonly IMethodQuery query;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorCustomization"/> class.
            /// </summary>
            /// <param name="targetType">
            /// The <see cref="Type"/> for which <paramref name="query"/> should be used to select the
            /// most appropriate constructor.
            /// </param>
            /// <param name="query">
            /// The query that selects a constructor for <paramref name="targetType"/>.
            /// </param>
            public ConstructorCustomization(Type targetType, IMethodQuery query)
            {
                if (targetType == null)
                    throw new ArgumentNullException("targetType");

                if (query == null)
                    throw new ArgumentNullException("query");

                this.targetType = targetType;
                this.query = query;
            }

            /// <summary>
            /// Gets the <see cref="Type"/> for which <see cref="Query"/> should be used to select the
            /// most appropriate constructor.
            /// </summary>
            public Type TargetType
            {
                get { return this.targetType; }
            }

            /// <summary>
            /// Gets the query that selects a constructor for <see cref="TargetType"/>.
            /// </summary>
            public IMethodQuery Query
            {
                get { return this.query; }
            }

            /// <summary>
            /// Customizes the specified fixture by modifying <see cref="TargetType"/> to use
            /// <see cref="Query"/> as the strategy for creating new specimens.
            /// </summary>
            /// <param name="fixture">The fixture to customize.</param>
            public void Customize(IFixture fixture)
            {
                if (fixture == null)
                {
                    throw new ArgumentNullException("fixture");
                }

                var factory = new MethodInvoker(this.Query);
                var builder = SpecimenBuilderNodeFactory.CreateTypedNode(
                    this.targetType,
                    factory);

                fixture.Customizations.Insert(
                    0,
                    new Postprocessor(
                        builder,
                        new AutoPropertiesCommand()));
            }
        }
    }
}