using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TheraEngine.Core.Memory
{
    //Stores a reference to unmanaged data
    public class DataSource : TObjectSlim, IDisposable
    {
        public event Action Modified;

        /// <summary>
        /// If true, this data source references memory that was allocated somewhere else.
        /// </summary>
        public bool External { get; }
        public int Length { get; set; }
        public VoidPtr Address { get; set; }

        public DataSource(VoidPtr address, int length, bool copyInternal = false)
        {
            if (length < 0)
                throw new Exception("Cannot have a source with a negative size.");
            Length = length;
            if (copyInternal)
            {
                Address = Marshal.AllocHGlobal(Length);
                Memory.Move(Address, address, (uint)length);
                External = false;
            }
            else
            {
                Address = address;
                External = true;
            }
        }

        public DataSource(int length, bool zeroMemory = false)
        {
            if (length < 0)
                throw new Exception("Cannot allocate a negative size.");
            Length = length;
            Address = Marshal.AllocHGlobal(Length);
            if (zeroMemory)
                Memory.Fill(Address, (uint)Length, 0);
            External = false;
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
                try
                {
                    if (!External && Address != null)
                    {
                        Marshal.FreeHGlobal(Address);
                        Address = null;
                    }
                }
                catch (Exception e)
                {
                    Engine.PrintLine(e.ToString());
                }

                _disposedValue = true;
            }
        }
        
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
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}