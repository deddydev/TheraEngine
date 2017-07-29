using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        {
            return _feet + _inches / 12.0f;
        }
        public float ToInches()
        {
            return _feet * 12.0f + _inches;
        }
        public float ToMeters()
        {
            return ToFeet().FeetToMeters();
        }
    }
}
