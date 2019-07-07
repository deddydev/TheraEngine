using System;

namespace Extensions
{
    public static partial class Ext
    {
        public static double RoundToNearestMultiple(this double value, double multiple)
            => Math.Round((value / multiple), MidpointRounding.AwayFromZero) * multiple;
        public static double Round(this double value, int digits = 0, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
            => Math.Round(value, digits, midpointRounding);

        public const Double ZeroToleranceD = 1e-6;
        public static bool IsZero(this Double value, double tolerance = ZeroToleranceD)
        {
            return Math.Abs(value) < tolerance;
        }
        public static bool EqualTo(this Double value, Double other, double tolerance = ZeroToleranceD)
        {
            return Math.Abs(other - value) < tolerance;
        }
        public static unsafe Double Reverse(this Double value)
        {
            *(uint*)(&value) = ((uint*)&value)->Reverse();
            return value;
        }
        public static Double Clamp(this Double value, Double min, Double max)
        {
            return value <= min ? min : value >= max ? max : value;
        }
        public static Double ClampMin(this Double value, Double min)
        {
            return value <= min ? min : value;
        }
        public static Double ClampMax(this Double value, Double max)
        {
            return value >= max ? max : value;
        }
        /// <summary>
        /// Remaps values outside of a range into the first multiple of that range.
        /// When it comes to signed numbers, negative is highest.
        /// For example, -128 (0xFF) vs 127 (0x7F).
        /// Because of this, the max value is non-inclusive while the min value is.
        /// </summary>
        public static Double RemapToRange(this Double value, Double min, Double max)
        {
            //Check if the value is already in the range
            if (value < max && value >= min)
                return value;

            //Get the distance between max and min
            Double range = max - min;

            //First figure out how many multiples of the range there are.
            //Dividing the value by the range and cutting off the decimal places
            //will return the number of multiples of whole ranges in the value.
            //Those multiples need to be subtracted out.
            value -= range * (int)(value / range);

            //Now the value is in the range of +range to -range.
            //The value needs to be within +(range/2) to -(range/2).
            value += value > max ? -range : value < min ? range : 0;

            //Max value is non-inclusive
            if (value == max)
                value = min;

            return value;
        }
        public static bool CompareEquality(this Double value, Double other, Double tolerance = 0.0001f)
        {
            return Math.Abs(value - other) < tolerance;
        }
    }
}
