using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    //Stores a reference to unmanaged data
    public class DataSource
    {
        private int _length;
        private IntPtr _address;

        public int Length { get { return _length; } }
        public IntPtr Address { get { return _address; } }

        public DataSource(IntPtr address, int length)
        {
            _length = length;
            _address = address;
        }
        ~DataSource() { Dispose(); }

        public static DataSource Allocate(int size)
        {
            if (size < 0)
                throw new Exception("Cannot allocate a negative size.");
            return new DataSource(Marshal.AllocHGlobal(size), size);
        }
        public void Dispose()
        {
            try
            {
                if (_address != null)
                {
                    Marshal.FreeHGlobal(_address);
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
    }
}
