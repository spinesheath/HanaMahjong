namespace Spines.Utility.Test
{
    sealed class NaturalEnum012 : NaturalEnum<NaturalEnum012>
    {
        public static NaturalEnum012 Value0 = new NaturalEnum012(0);
        public static NaturalEnum012 Value1 = new NaturalEnum012(1);
        public static NaturalEnum012 Value2 = new NaturalEnum012(2);

        private NaturalEnum012(int identifier) : base(identifier)
        {
        }
    }
}