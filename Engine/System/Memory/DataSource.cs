using System.Runtime.InteropServices;

namespace System
{
    //Stores a reference to unmanaged data
    public class DataSource : IDisposable
    {
        private int _length;
        private VoidPtr _address;

        public int Length { get { return _length; } }
        public VoidPtr Address { get { return _address; } }

        public event Action Modified;

        public DataSource(VoidPtr address, int length)
        {
            if (length < 0)
                throw new Exception("Cannot have a source with a negative size.");
            _length = length;
            _address = address;
        }
        public DataSource(int length)
        {
            if (length < 0)
                throw new Exception("Cannot allocate a negative size.");
            _length = length;
            _address = Marshal.AllocHGlobal(_length);
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
        public void NotifyModified() { Modified?.Invoke(); }
    }
}