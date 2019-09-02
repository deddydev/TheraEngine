using Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files
{
    [TFileExt("tarc")]
    [TFileDef("Archive", "File containing various folders and files.")]
    public class ArchiveFile : TFileObject
    {
        internal ArchiveFile()
        {
            Entries = new Dictionary<string, Entry>();
        }
        public ArchiveFile(string baseDirectory) : this()
        {
            FilePath = Path.Combine(baseDirectory, "Archive");
        }

        [TSerialize(DeserializeAsync = true)]
        public Dictionary<string, Entry> Entries { get; set; }

        public static ArchiveFile FromDirectory(string dirPath, bool deleteDirectory = false)
        {
            ArchiveFile file = new ArchiveFile(dirPath);
            string[] paths = Directory.GetFileSystemEntries(dirPath);
            foreach (string path in paths)
                file.ImportPath(path, deleteDirectory);
            return file;
        }
        public void ImportPath(string path, bool delete)
        {
            bool? pathType = path?.IsExistingDirectoryPath();
            if (pathType is null)
                return;

            bool dir = pathType.Value;
            if (dir)
            {
                string[] paths = Directory.GetFileSystemEntries(path);
                foreach (string subPath in paths)
                    ImportPath(subPath, delete);
            }
            else
            {
                string dirPath = DirectoryPath;
                string relPath = path.MakeAbsolutePathRelativeTo(dirPath);
                TypeProxy type = DetermineType(path, out EFileFormat format);
                byte[] fileBytes = File.ReadAllBytes(path);
                Entry entry = new Entry(fileBytes, format, type);
                Entries.Add(relPath, entry);
            }
        }
        public class Entry : TFileObject
        {
            [TSerialize]
            public EFileFormat Format { get; internal set; }
            [TSerialize]
            public TypeProxy Type { get; internal set; }
            [TSerialize]
            public byte[] Bytes { get; internal set; }

            private TFileObject _file = null;
            public TFileObject File
            {
                get
                {
                    if (_file == null)
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
                Bytes = data;
                Format = format;
                Type = type;
            }
        }
    }
}