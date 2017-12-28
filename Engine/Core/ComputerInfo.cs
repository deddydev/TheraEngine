namespace System
{
    public class ComputerInfo
    {
        public int ProcessorCount { get; private set; }
        public bool Is64Bit { get; private set; }
        public OperatingSystem OSVersion { get; private set; }
        public Endian.EOrder Endian { get; private set; } = System.Endian.EOrder.Big;

        public static ComputerInfo Analyze()
        {
            ComputerInfo c = new ComputerInfo()
            {
                ProcessorCount = Environment.ProcessorCount,
                OSVersion = Environment.OSVersion,
                Is64Bit = Environment.Is64BitOperatingSystem
            };
            return c;
        }
    }
}
