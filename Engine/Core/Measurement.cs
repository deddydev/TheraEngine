namespace System
{
    public class FeetInches
    {
        private int _feet;
        private float _inches;
        public FeetInches(int feet, float inches)
        {
            _feet = feet;
            _inches = inches;
        }
        //public static FeetInches FromInches(float inches)
        //{
        //    float feet = inches / 12.0f;
        //    float f = Math.Floor(feet);
        //    return new FeetInches();
        //}
        public float ToFeet()
            => _feet + _inches / 12.0f;
        public float ToInches()
            => _feet * 12.0f + _inches;
        public float ToMeters()
            => ToFeet().FeetToMeters();
    }
}
