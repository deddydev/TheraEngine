using System;
using System.IO;
using System.Runtime.InteropServices;
using TheraEngine;

namespace TheraEngine.Core.Memory
{
    //Stores a reference to unmanaged data
    public class DataSource : IDisposable
    {
        public event Action Modified;

        private bool _external;
        private int _length;
        private VoidPtr _address;

        /// <summary>
        /// If true, this data source references memory that was allocated somewhere else.
        /// </summary>
        public bool External => _external;
        public int Length
        {
            get => _length;
            set => _length = value;
        }
        public VoidPtr Address
        {
            get => _address;
            set => _address = value;
        }

        public DataSource(VoidPtr address, int length, bool copyInternal = false)
        {
            if (length < 0)
                throw new Exception("Cannot have a source with a negative size.");
            _length = length;
            if (copyInternal)
            {
                _address = Marshal.AllocHGlobal(_length);
                Memory.Move(_address, address, (uint)length);
                _external = false;
            }
            else
            {
                _address = address;
                _external = true;
            }
        }

        public DataSource(int length, bool zeroMemory = false)
        {
            if (length < 0)
                throw new Exception("Cannot allocate a negative size.");
            _length = length;
            _address = Marshal.AllocHGlobal(_length);
            if (zeroMemory)
                Memory.Fill(_address, (uint)_length, 0);
            _external = false;
        }

        public static DataSource Allocate(int size, bool zeroMemory = false)
            => new DataSource(size, zeroMemory);

        public unsafe UnmanagedMemoryStream AsStream()
            => new UnmanagedMemoryStream((byte*)Address, Length);

        public void NotifyModified()
            => Modified?.Invoke();

        #region IDisposable Support
        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                try
                {
                    if (!_external && _address != null)
                    {
                        Marshal.FreeHGlobal(_address);
                        _address = null;
                    }
                }
                catch (Exception e)
                {
                    Engine.PrintLine(e.ToString());
                }

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DataSource()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}