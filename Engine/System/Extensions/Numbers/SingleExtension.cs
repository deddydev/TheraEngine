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
        public static bool CompareEquality(this Single value, Single other, Single tolerance = 0.0001f)
        {
            return Math.Abs(value - other) < tolerance;
        }
    }
}
