namespace System
{
    public class FeetInches
    {
        private float _feet, _inches;
        public FeetInches(float feet, float inches)
        {
            _feet = feet;
            _inches = inches;
        }
        public float ToFeet()
            => _feet + _inches / 12.0f;
        public float ToInches()
            => _feet * 12.0f + _inches;
        public float ToMeters()
            => ToFeet().FeetToMeters();
    }
}
