using System;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Files;
using TheraEngine.ComponentModel;

namespace TheraEngine.Core
{
    [TFileDef("Computer Information")]
    [TFileExt("compi")]
    public class ComputerInfo : TFileObject
    {
        [TSerialize]
        public int ProcessorCount { get; private set; }
        [TSerialize]
        public bool Is64Bit { get; private set; }
        [TSerialize]
        public OperatingSystem OSVersion { get; private set; }
        [TSerialize]
        public Endian.EOrder Endian { get; private set; } = Memory.Endian.EOrder.Big;
        [TSerialize]
        public int MaxTextureUnits { get; internal set; }

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
