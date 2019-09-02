using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.PMX
{
    public enum EStringEncoding
    {
        UTF16LE = 0,
        UTF8 = 1,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct StringValue
    {
        public int _length;

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        public byte* StringAddress => (byte*)(Address + 4);
        public int TotalLength => 4 + _length;
        public string GetString(EStringEncoding encoding)
        {
            Encoding f;
            if (encoding == EStringEncoding.UTF8)
                f = Encoding.UTF8;
            else
                f = Encoding.Unicode;
            return f.GetString(StringAddress, _length);
        }
    }
}
