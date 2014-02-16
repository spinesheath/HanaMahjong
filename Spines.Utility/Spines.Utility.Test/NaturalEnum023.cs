namespace Spines.Utility.Test
{
    sealed class NaturalEnum023 : NaturalEnum<NaturalEnum023>
    {
        public static NaturalEnum023 Value0 = new NaturalEnum023(0);
        public static NaturalEnum023 Value1 = new NaturalEnum023(2);
        public static NaturalEnum023 Value2 = new NaturalEnum023(3);

        private NaturalEnum023(int identifier)
            : base(identifier)
        {
        }
    }
}