using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Spines.Utility
{
    /// <summary>
    /// A container for natural enumeration values.
    /// It only allows consecutive integer identifiers from 0 upwards.
    /// </summary>
    /// <typeparam name="TEnum">The type of the stored enumeration values.</typeparam>
    public sealed class NaturalEnumContainer<TEnum> : IEnumContainer<TEnum>
        where TEnum : NaturalEnum<TEnum>
    {
        private const string DuplicateFormat = "The natural enumeration {0} has duplicate identifiers.";

        private const string ConsecutiveFormat =
            "The natural enumeration {0} must have consecutive identifiers from 0 upwards.";

        private static IReadOnlyList<TEnum> _definedValues;

        public void Initialize(IEnumerable<TEnum> values)
        {
            var type = typeof(TEnum);
            _definedValues = values.ToList();
            if (!_definedValues.Any())
            {
                return;
            }
            if (_definedValues.HasDuplicates())
            {
                throw new InitializationException(String.Format(DuplicateFormat, type.FullName));
            }
            if (_definedValues.Min(v => v.Id) < 0 || _definedValues.Max(v => v.Id) >= _definedValues.Count)
            {
                throw new InitializationException(String.Format(ConsecutiveFormat, type.FullName));
            }
        }

        public IReadOnlyList<TEnum> DefinedValues
        {
            get
            {
                return _definedValues;
            }
        }
    }
}