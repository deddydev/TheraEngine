using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AutoWrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string rootPath;
            Folder root = null;

            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "Select the C++ directory";
            //if (fbd.ShowDialog() == DialogResult.OK)
                root = GetFolders(null, rootPath =
                    Environment.MachineName == "DAVID-DESKTOP" ?
                    "X:\\Program Files\\Autodesk\\FBX\\FBX SDK\\2017.0.1\\include" :
                    "C:\\Program Files\\Autodesk\\FBX\\FBX SDK\\2017.0.1\\include" 
                    /*fbd.SelectedPath*/);
            //else
            //{
            //    Application.Exit();
            //    return;
            //}

            root._name = null;

            //fbd.Description = "Select the output CLR directory";
            //if (fbd.ShowDialog() == DialogResult.OK)
                WriteFolders(root, rootPath,
                    Environment.MachineName == "DAVID-DESKTOP" ?
                    "X:\\Desktop\\New Folder (2)" :
                    "C:\\Users\\David\\Desktop\\New Folder" 
                    /*fbd.SelectedPath*/);
        }

        private static Folder GetFolders(Folder parent, string path)
        {
            Folder f = new Folder()
            {
                _name = Path.GetFileName(path),
                _parentFolder = parent
            };

            foreach (string p in Directory.EnumerateFiles(path))
                if (Path.GetExtension(p).ToLower().Equals(".h"))
                    f._headerNames.Add(Path.GetFileNameWithoutExtension(p));

            foreach (string dir in Directory.EnumerateDirectories(path))
                f._childFolders.Add(GetFolders(f, dir));

            return f;
        }

        private static void WriteFolders(Folder folder, string inputRootDir, string outputRootDir)
        {
            string outputPath = folder.GetFullDirectoryPath(outputRootDir);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            foreach (string file in folder._headerNames)
                CodeConverter.Convert(file, folder.GetFullDirectoryPath(inputRootDir), outputPath);

            foreach (Folder f in folder._childFolders)
                WriteFolders(f, inputRootDir, outputRootDir);
        }
    }

    public class Folder
    {
        public string _name;
        public Folder _parentFolder = null;
        public List<Folder> _childFolders = new List<Folder>();
        public List<string> _headerNames = new List<string>();

        public string GetRelativePath()
        {
            if (_parentFolder == null)
                return _name;
            return _parentFolder.GetRelativePath() + "\\" + _name;
        }

        public string GetFullDirectoryPath(string rootPath)
        {
            if (_name == null)
                return rootPath;
            return rootPath + GetRelativePath();
        }
    }
}
