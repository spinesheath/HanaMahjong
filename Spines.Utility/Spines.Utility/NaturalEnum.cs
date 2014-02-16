
namespace Spines.Utility
{
    /// <summary>
    /// Base class for natural enumerations.
    /// A natural enumeration is based on integers.
    /// Its values use unique consecutive identifiers from 0 upwards.
    /// </summary>
    /// <typeparam name="TEnum">The type of the derived enumeration.</typeparam>
    public abstract class NaturalEnum<TEnum> : GeneralizedEnum<TEnum, int, NaturalEnumContainer<TEnum>>
        where TEnum : NaturalEnum<TEnum>
    {
        protected NaturalEnum(int identifier) : base(identifier)
        {
        }

        /// <summary>
        /// The identifier of the enumeration value.
        /// </summary>
        public int Id { get { return Identifier; }}
    }
}