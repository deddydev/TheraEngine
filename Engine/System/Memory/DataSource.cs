using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
    //Stores a reference to unmanaged data
    public class DataSource : IDisposable
    {
        private bool _external;
        private int _length;
        private VoidPtr _address;

        public int Length { get { return _length; } set { _length = value; } }
        public VoidPtr Address { get { return _address; } set { _address = value; } }

        public event Action Modified;

        public DataSource(VoidPtr address, int length)
        {
            if (length < 0)
                throw new Exception("Cannot have a source with a negative size.");
            _length = length;
            _address = address;
            _external = true;
        }
        public DataSource(int length)
        {
            if (length < 0)
                throw new Exception("Cannot allocate a negative size.");
            _length = length;
            _address = Marshal.AllocHGlobal(_length);
            _external = false;
        }
        ~DataSource() { Dispose(); }

        public static DataSource Allocate(int size) { return new DataSource(size); }
        public void Dispose()
        {
            try
            {
                if (!_external && _address != null)
                {
                    Marshal.FreeHGlobal(_address);
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public void NotifyModified() { Modified?.Invoke(); }
    }
}