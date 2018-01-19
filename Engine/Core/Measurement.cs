namespace System
{
    public class FeetInches
    {
        public int Feet { get; set; }
        public float Inches { get; set; }
        
        public FeetInches(int feet, float inches)
        {
            Feet = feet;
            Inches = inches;
        }
        public static FeetInches FromInches(float inches)
            => FromFeet(inches / 12.0f);
        public static FeetInches FromFeet(float feet)
        {
            int ift = (int)Math.Floor(feet);
            return new FeetInches(ift, (feet - ift) * 12.0f);
        }
        public float ToFeet()
            => Feet + Inches / 12.0f;
        public float ToInches()
            => Feet * 12.0f + Inches;
        public float ToMeters()
            => ToFeet().FeetToMeters();
    }
}
