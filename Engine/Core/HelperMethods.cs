namespace TheraEngine.Core
{
    public static class THelpers
    {
        /// <summary>
        /// Swaps the two values with each other.
        /// </summary>
        public static void Swap<T>(ref T value1, ref T value2)
        {
            T temp = value1;
            value1 = value2;
            value2 = temp;
        }
    }
}
