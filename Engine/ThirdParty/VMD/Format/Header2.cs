using System.Runtime.InteropServices;
using System.Text;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;

namespace TheraEngine.ThirdParty.VMD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Header2
    {
        public const int Size = 50;
        public static readonly string MagicString = "Vocaloid Motion Data 0002";

        //Vocaloid Motion Data 0002 + 5 null bytes
        private fixed byte _magic[30];
        private fixed byte _modelName[20];

        public KeyframeList<BoneKeyframe>* BoneKeyframes => (KeyframeList<BoneKeyframe>*)(
            Address + Size);
        public KeyframeList<MorphKeyframe>* MorphKeyframes => (KeyframeList<MorphKeyframe>*)(
            Address + Size + 
            BoneKeyframes->GetSize());
        public KeyframeList<CameraKeyframe>* CameraKeyframes => (KeyframeList<CameraKeyframe>*)(
            Address + Size +
            BoneKeyframes->GetSize() + 
            MorphKeyframes->GetSize());
        public KeyframeList<LightKeyframe>* LightKeyframes => (KeyframeList<LightKeyframe>*)(
            Address + Size +
            BoneKeyframes->GetSize() +
            MorphKeyframes->GetSize() +
            CameraKeyframes->GetSize());

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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Keyframe
    {
        private fixed byte _boneName[15];
        private uint _frameIndex;
        private Vec3 _position;
        private Vec4 _rotation;

        public string BoneName
        {
            get
            {
                byte[] bytes = new byte[15];
                for (int i = 0; i < 15; ++i)
                    bytes[i] = _boneName[i];
                char[] chars = new char[15];
                Encoding.ASCII.GetDecoder().GetChars(bytes, 0, 15, chars, 0, true);
                return new string(chars);
            }
            set
            {
                byte[] bytes = new byte[15];
                Encoding.ASCII.GetEncoder().GetBytes(value.ToCharArray(), 0, value.Length, bytes, 0, true);
                for (int i = 0; i < 15; ++i)
                    _boneName[i] = bytes[i];
            }
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
