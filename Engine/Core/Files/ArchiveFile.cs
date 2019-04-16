using System;
using System.IO;

namespace TheraEngine.Core.Files
{
    public class ArchiveFile : TFileObject
    {
        public static ArchiveFile FromDirectory(string dirPath, bool deleteDirectory = false)
        {
            ArchiveFile file = new ArchiveFile();
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

            }
        }
    }
}