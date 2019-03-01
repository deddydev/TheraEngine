using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header
    {
        public const int Size = 54;

        private fixed byte _magic[30]; //Vocaloid Motion Data 0002 + 5 null bytes
        private fixed byte _modelName[20];
        private uint _keyframeCount;
        
        public uint KeyframeCount
        {
            get => _keyframeCount;
            set => _keyframeCount = value;
        }
        public string Magic
        {
            get
            {
                byte[] bytes = new byte[30];
                for (int i = 0; i < 30; ++i)
                    bytes[i] = _magic[i];
                char[] chars = new char[30];
                Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 30, chars, 0, true);
                return new string(chars);
            }
            set
            {
                byte[] bytes = new byte[30];
                Encoding.ASCII.GetEncoder().GetBytes(value.ToCharArray(), 0, value.Length, bytes, 0, true);
                for (int i = 0; i < 30; ++i)
                    _magic[i] = bytes[i];
            }
        }
        public string ModelName
        {
            get
            {
                byte[] bytes = new byte[20];
                for (int i = 0; i < 20; ++i)
                    bytes[i] = _modelName[i];
                char[] chars = new char[20];
                Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 30, chars, 0, true);
                return new string(chars);
            }
            set
            {
                byte[] bytes = new byte[20];
                Encoding.ASCII.GetEncoder().GetBytes(value.ToCharArray(), 0, value.Length, bytes, 0, true);
                for (int i = 0; i < 20; ++i)
                    _modelName[i] = bytes[i];
            }
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
