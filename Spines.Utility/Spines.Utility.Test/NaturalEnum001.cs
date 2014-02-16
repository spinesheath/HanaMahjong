namespace Spines.Utility.Test
{
    sealed class NaturalEnum001 : NaturalEnum<NaturalEnum001>
    {
        public static NaturalEnum001 Value0 = new NaturalEnum001(0);
        public static NaturalEnum001 Value1 = new NaturalEnum001(0);
        public static NaturalEnum001 Value2 = new NaturalEnum001(1);

        private NaturalEnum001(int identifier)
            : base(identifier)
        {
        }
    }
}