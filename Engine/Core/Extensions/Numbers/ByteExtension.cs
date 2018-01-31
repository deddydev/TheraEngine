namespace System
{
    public static class ByteExtension
    {
        /// <summary>
        /// Finds the first differing bit starting from the lsb.
        /// </summary>
        /// <returns>The index of the bit.</returns>
        public static int CompareBits(this byte b1, byte b2)
        {
            for (int i = 8, b = 0x80; i-- != 0; b >>= 1)
                if ((b1 & b) != (b2 & b))
                    return i;
            return 0;
        }
        public static int CountBits(this byte b)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
                if (((b >> i) & 1) != 0)
                    ++count;
            return count;
        }
        public static byte Clamp(this byte value, byte min, byte max)
        {
            return value < min ? min : value > max ? max : value;
        }
        public static byte ClampMin(this byte value, byte min)
        {
            return value <= min ? min : value;
        }
        public static byte ClampMax(this byte value, byte max)
        {
            return value >= max ? max : value;
        }
        public static byte RoundDownToEven(this byte value)
        {
            return (byte)(value - (value % 2));
        }
        public static byte RoundUpToEven(this byte value)
        {
            return (byte)(value + (value % 2));
        }
        public static byte SetBit(this byte value, int bitIndex, bool set)
            => (byte)(set ? value | (1 << bitIndex) : value & ~(1 << bitIndex));
    }
}
