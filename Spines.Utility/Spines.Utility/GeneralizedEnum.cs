using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spines.Utility
{
    /// <summary>
    /// Base class for custom enumerations.
    /// Enumeration values have to be defined as public static fields in the derived class.
    /// </summary>
    /// <typeparam name="TEnum">The type of the derived class, the actual enumeration class.</typeparam>
    /// <typeparam name="TUnderlying">The underlying datatype which is used to identify enumeration values.</typeparam>
    /// <typeparam name="TContainer">A container for enumeration values.</typeparam>
    public abstract class GeneralizedEnum<TEnum, TUnderlying, TContainer>
        : IComparable<GeneralizedEnum<TEnum, TUnderlying, TContainer>>
        where TEnum : GeneralizedEnum<TEnum, TUnderlying, TContainer>
        where TUnderlying : IComparable<TUnderlying>, IEquatable<TUnderlying>
        where TContainer : IEnumContainer<TEnum>, new()
    {
        private static IEnumContainer<TEnum> _container;
        protected readonly TUnderlying Identifier;

        protected GeneralizedEnum(TUnderlying identifier)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Returns all enumeration values that are defined for <typeparamref name="TEnum" />.
        /// </summary>
        public static IReadOnlyList<TEnum> DefinedValues
        {
            get
            {
                Initialize();
                return _container.DefinedValues;
            }
        }

        public int CompareTo(GeneralizedEnum<TEnum, TUnderlying, TContainer> other)
        {
            return other.Identifier.CompareTo(Identifier);
        }

        private static void Initialize()
        {
            if (_container != null)
            {
                return;
            }
            var type = typeof (TEnum);
            var fields =
                type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static |
                               BindingFlags.GetField);
            var values = fields.Select(field => field.GetValue(null)).OfType<TEnum>();
            _container = new TContainer();
            _container.Initialize(values);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) ||
                   !ReferenceEquals(null, obj) &&
                   obj.GetType() == GetType() &&
                   Equals(obj as GeneralizedEnum<TEnum, TUnderlying, TContainer>);
        }

        protected bool Equals(GeneralizedEnum<TEnum, TUnderlying, TContainer> other)
        {
            return EqualityComparer<TUnderlying>.Default.Equals(Identifier, other.Identifier);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TUnderlying>.Default.GetHashCode(Identifier);
        }

        public static bool operator ==(
            GeneralizedEnum<TEnum, TUnderlying, TContainer> lhs, GeneralizedEnum<TEnum, TUnderlying, TContainer> rhs)
        {
            return lhs != null && rhs != null && lhs.Identifier.Equals(rhs.Identifier);
        }

        public static bool operator !=(
            GeneralizedEnum<TEnum, TUnderlying, TContainer> a, GeneralizedEnum<TEnum, TUnderlying, TContainer> b)
        {
            return !(a == b);
        }
    }
}