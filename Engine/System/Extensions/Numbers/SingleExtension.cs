namespace System
{
    public static class SingleExtension
    {
        public const Single ZeroTolerance = 1e-6f;
        public static bool IsZero(this Single value)
        {
            return Math.Abs(value) < ZeroTolerance;
        }
        public static bool EqualTo(this Single value, Single other)
        {
            return Math.Abs(other - value) < ZeroTolerance;
        }
        public static unsafe Single Reverse(this Single value)
        {
            *(uint*)(&value) = ((uint*)&value)->Reverse();
            return value;
        }
        public static Single Clamp(this Single value, Single min, Single max)
        {
            return value <= min ? min : value >= max ? max : value;
        }
        public static Single ClampMin(this Single value, Single min)
        {
            return value <= min ? min : value;
        }
        public static Single ClampMax(this Single value, Single max)
        {
            return value >= max ? max : value;
        }
        /// <summary>
        /// Remaps values outside of a range into the first multiple of that range.
        /// When it comes to signed numbers, negative is highest.
        /// For example, -128 (0xFF) vs 127 (0x7F).
        /// Because of this, the max value is non-inclusive while the min value is.
        /// </summary>
        public static Single RemapToRange(this Single value, Single min, Single max)
        {
            //Check if the value is already in the range
            if (value < max && value >= min)
                return value;

            //Get the distance between max and min
            Single range = max - min;

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
        public static bool EqualTo(this Single value, Single other, Single tolerance = 0.0001f)
        {
            return Math.Abs(value - other) < tolerance;
        }
        public static Single FeetToMeters(this Single value)
        {
            return value * 0.3048f;
        }
        /// <summary>
        /// Positive is to a bigger unit (ex Meters to Kilometers is 2 steps)
        /// Negative is to a smaller unit (ex Meters to Cenimeters is -2 steps)
        /// </summary>
        public static Single MetricUnitScale(this Single value, int steps)
        {
            if (steps == 0)
                return value;
            float scale = 1.0f;
            if (steps > 0)
                scale = (float)Math.Pow(10.0f, steps);
            else
                scale = (float)Math.Pow(0.1f, -steps);
            return value * scale;
        }
        public static Single MetersToFeet(this Single value)
        {
            return value * 3.280839895f;
        }
        public static Single FeetToYards(this Single value)
        {
            return value * 0.33333333333f;
        }
        public static Single YardsToFeet(this Single value)
        {
            return value * 3.0f;
        }
        public static Single MilesToKilometers(this Single value)
        {
            return value * 1.60934f;
        }
        public static Single KilometersToMiles(this Single value)
        {
            return value * 0.6213727366498068f;
        }
    }
}
