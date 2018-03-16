using AutoFixture.Kernel;
using System;
using System.Linq;
using System.Reflection;

namespace AutoFixture
{
    public static class AutoFixtureExtensions
    {
        public class ParameterInfoEquatable : IEquatable<ParameterInfo>
        {
            private readonly ParameterInfo _actual;

            public ParameterInfoEquatable(ParameterInfo actual)
                => _actual = actual ?? throw new ArgumentNullException(nameof(actual));

            public bool Equals(ParameterInfo other)
            {
                return
                    _actual.Name == other.Name &&
                    _actual.ParameterType == other.ParameterType &&
                    _actual.Member.DeclaringType == other.Member.DeclaringType;
            }
        }

        public static IFixture CustomizeCtorParameter<T>(
            this IFixture @this,
            string paramName,
            object value,
            Type[] ctorSelectionCriteria = null)
        {
            if (!typeof(T).IsClass)
                throw new ArgumentException(string.Format("The specified object type is not a class: {0}", typeof(T)));

            var ctorInfo = default(ConstructorInfo);

            if (ctorSelectionCriteria != null)
                ctorInfo = typeof(T).GetConstructor(ctorSelectionCriteria);
            else
                ctorInfo = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(e => e.GetParameters().Any(p => p.Name == paramName))
                            .FirstOrDefault();

            if (ctorInfo == null)
                throw new ArgumentException(
                    string.Format("No ctor found for {0} with parameter name: {1}", typeof(T), paramName),
                    paramName);

            var paramInfo = ctorInfo
                            .GetParameters()
                            .First(e => e.Name == paramName);

            if (value != null && !paramInfo.ParameterType.IsAssignableFrom(value.GetType()) ||
                value == null && !IsNullable(paramInfo.ParameterType))
            {
                throw new ArgumentException(
                    string.Format("The selected parameter is of type {0} and the value passed is of type {1}",
                        paramInfo.ParameterType,
                        value != null
                            ? value.GetType().ToString()
                            : "null"),
                    paramName);
            }

            @this.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(value),
                    new ParameterSpecification(new ParameterInfoEquatable(paramInfo))));

            return @this;
        }

        #region Private Methods

        private static bool IsNullable(Type type)
        {
            // Reference Type
            if (!type.IsValueType)
                return true;

            // Nullable<T>
            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            return false;
        }

        #endregion Private Methods
    }
}