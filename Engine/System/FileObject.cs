using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;

namespace System
{
    public abstract class FileObject : ObjectBase
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
            PropertyInfo[] infos = GetType().GetProperties(BindingFlags.Public);
            return size;
        }
        public void Rebuild()
        {

        }
        public void Export(string path)
        {
            
        }
        public virtual async Task Write(VoidPtr address, int length, IProgress<float> progress)
        {
            await Task.Delay(100);
        }
#endif
        protected bool _isLoaded, _isLoading;
        protected string _filePath;
        public virtual void Unload()
        {
            _isLoaded = false;
        }
        public void Load()
        {
            FileMap map = FileMap.FromFile(FilePath);
        }
        public virtual void Load(VoidPtr address)
        {
            _isLoading = true;

            _isLoading = false;
            _isLoaded = true;
        }
        [Default]
        public string FilePath { get { return _filePath; } }
        public bool IsLoading { get { return _isLoading; } }
        [State]
        public bool IsLoaded { get { return _isLoaded; } }

        public static T FromXML<T>(string filePath) where T : FileObject
        {
            if (!File.Exists(filePath))
                return default(T);
            using (FileStream stream = File.OpenRead(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T obj = serializer.Deserialize(stream) as T;
                obj._isLoaded = true;
                return obj;
            }
        }
        public void ToXML(string filePath)
        {
            _isWriting = true;
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
            _isWriting = false;
        }
    }
}
