﻿using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class FileObject : ObjectBase
    {
#if EDITOR
        private int _calculatedSize;
        bool _isCalculatingSize, _isWriting;

        public int CalculatedSize { get { return _calculatedSize; } }

        public bool IsCalculatingSize { get { return _isCalculatingSize; } }
        public bool IsSaving { get { return IsCalculatingSize || IsWriting; } }
        public bool IsWriting { get { return _isWriting; } }

        public int CalculateSize(bool force)
        {
            if (IsDirty || force)
                Rebuild();
            int size = OnCalculateSize();
            return size;
        }
        public int OnCalculateSize()
        {
            int size = 0;
            return size;
        }
        public void Rebuild()
        {

        }
        public void Rebuild(string path)
        {
            
        }
        public Task Write(VoidPtr address, int length, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }
#endif
        private bool _isLoaded, _isLoading;
        private string _filePath;
        public virtual async Task Unload()
        {
            _isLoaded = false;
        }
        public virtual async Task Load()
        {
            _isLoading = true;

            _isLoading = false;
            _isLoaded = true;
        }
        public string FilePath { get { return _filePath; } }
        public bool IsLoading { get { return _isLoading; } }
        public bool IsLoaded { get { return _isLoaded; } }
    }
}
