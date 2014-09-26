namespace Jwc.CIBuildTasks
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    internal static class PropertyInfoExtensions
    {
        public static void AssertGet<TAttribute>(this PropertyInfo property)
            where TAttribute : Attribute
        {
            Assert.NotNull(
                property.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault());
        }
    }
}