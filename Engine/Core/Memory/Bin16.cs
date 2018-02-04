using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bin16
    {
        public bushort _data;

        public Bin16(ushort val) { _data = val; }

        public static implicit operator ushort(Bin16 val) { return val._data; }
        public static implicit operator Bin16(ushort val) { return new Bin16(val); }
        //public static implicit operator short(Bin16 val) { return (short)val._data; }
        //public static implicit operator Bin16(short val) { return new Bin16((ushort)val); }

        public override string ToString()
        {
            int i = 0;
            string val = "";
            while (i++ < 16)
            {
                val += (_data >> (16 - i)) & 1;
                if (i % 4 == 0 && i != 16)
                    val += " ";
            }
            return val;
        }

        public bool this[int index]
        {
            get { return (_data >> index & 1) != 0; }
            set
            {
                if (value)
                    _data = (ushort)((ushort)_data | (ushort)(1 << index));
                else
                    _data = (ushort)((ushort)_data & ~(ushort)(1 << index));
            }
        }

        //public ushort this[int shift, int mask]
        //{
        //    get { return (ushort)(data >> shift & mask); }
        //    set { data = (ushort)((data & ~(mask << shift)) | ((value & mask) << shift)); }
        //}

        public ushort this[int shift, int bitCount]
        {
            get
            {
                int mask = 0;
                for (int i = 0; i < bitCount; i++)
                    mask |= 1 << i;
                return (ushort)((_data >> shift) & mask);
            }
            set
            {
                int mask = 0;
                for (int i = 0; i < bitCount; i++)
                    mask |= 1 << i;
                _data = (ushort)((_data & ~(mask << shift)) | ((value & mask) << shift));
            }
        }

        public static Bin16 FromString(string s)
        {
            char[] delims = new char[] { ',', '(', ')', ' ' };

            ushort b = 0;
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            for (int len = 0; len < arr.Length; len++)
            {
                byte bit = 0;
                for (int i = 0; i < arr[len].Length; i++)
                {
                    b <<= 1;
                    byte.TryParse(arr[len][i].ToString(), out bit);
                    bit = bit.Clamp(0, 1);
                    b += bit;
                }
            }

            return new Bin16(b);
        }
    }
}
