using Extensions;
using System.Collections.Generic;
using System.IO;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    [TFileExt("tarc")]
    [TFileDef("Archive", "File containing various folders and files.")]
    public class ArchiveFile : TFileObject
    {
        public ArchiveFile()
        {
            Entries = new Dictionary<string, Entry>();
        }
        public ArchiveFile(string baseDirectory) : this()
        {
            FilePath = baseDirectory + ".trc";
        }

        [TSerialize(DeserializeAsync = true)]
        public Dictionary<string, Entry> Entries { get; set; }

        public static ArchiveFile FromDirectory(string dirPath, bool deleteDirectory = false)
        {
            ArchiveFile file = new ArchiveFile(dirPath);
            
            string[] paths = Directory.GetFileSystemEntries(dirPath);
            foreach (string path in paths)
                file.ImportPath(dirPath, path, deleteDirectory);

            return file;
        }
        public void ImportPath(string dirPath, string path, bool delete)
        {
            bool? pathType = path?.IsExistingDirectoryPath();
            if (pathType is null)
                return;

            bool dir = pathType.Value;
            if (dir)
            {
                string[] paths = Directory.GetFileSystemEntries(path);
                foreach (string subPath in paths)
                    ImportPath(dirPath, subPath, delete);
            }
            else
            {
                TypeProxy type = DetermineType(path, out EFileFormat format);
                byte[] fileBytes = File.ReadAllBytes(path);
                Entry entry = new Entry(fileBytes, format, type);

                string relPath = path.MakeAbsolutePathRelativeTo(dirPath);
                Entries.Add(relPath, entry);
            }
        }
        public class Entry : TFileObject
        {
            [TSerialize(Order = 0)]
            public EFileFormat Format { get; set; }

            [TSerialize(Order = 1)]
            public TypeProxy Type { get; set; }

            [TSerialize(Order = 2)]
            public byte[] Bytes 
            {
                get => _bytes;
                set
                {
                    _bytes = value;
                    ReadBytes();
                }
            }

            private TFileObject _file = null;
            private byte[] _bytes;

            public TFileObject File
            {
                get
                {
                    if (_file is null)
                        ReadBytes();
                    return _file;
                }
                set
                {
                    _file = value;
                    WriteBytes();
                }
            }

            private void ReadBytes()
            {

            }
            private void WriteBytes()
            {

            }

            public Entry() { }
            public Entry(byte[] data, EFileFormat format, TypeProxy type)
            {
                Format = format;
                Type = type;
                Bytes = data;
            }
        }
    }
}