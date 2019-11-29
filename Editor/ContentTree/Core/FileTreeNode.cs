using Microsoft.VisualBasic.FileIO;
using System.IO;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    public class FileTreeNode : ContentTreeNode
    {
        public FileTreeNode(string path) : base(path) { }

        public override void Delete()
        {
            if (Parent is null)
                return;

            ResourceTree tree = TreeView;
            try
            {
                tree.WatchProjectDirectory = false;
                if (new FileInfo(FilePath).Length > 0)
                    FileSystem.DeleteFile(FilePath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                else
                    File.Delete(FilePath);
                Remove();
            }
            catch
            {

            }
            finally
            {
                tree.WatchProjectDirectory = true;
            }
        }

        protected internal override void SetPath(string parentFolderPath)
        {
            string fileName = Text;
            if (parentFolderPath[parentFolderPath.Length - 1] != Path.DirectorySeparatorChar)
                parentFolderPath += Path.DirectorySeparatorChar;
            FilePath = parentFolderPath + fileName;
        }

        internal protected override void OnExpand() { }
        internal protected override void OnCollapse() { }
    }
}
