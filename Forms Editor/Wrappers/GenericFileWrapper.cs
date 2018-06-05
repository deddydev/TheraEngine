using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TheraEngine.Files;

namespace TheraEditor.Wrappers
{
    public class GenericFileWrapper : BaseFileWrapper
    {
        public GenericFileWrapper() : base() { }
        public GenericFileWrapper(ContextMenuStrip menu) : base(menu) { }
        public GenericFileWrapper(string path) : base()
        {
            Text = Path.GetFileName(path);
            FilePath = Name = path;
        }
        public override TFileObject SingleInstance { get => null; set { } }
        public override bool IsLoaded => false;
        public override Type FileType => null;
        public override IGlobalFileRef SingleInstanceRef => throw new NotImplementedException();
        public override TFileObject GetNewInstance() { return null; }
        protected internal override void FixPath(string parentFolderPath) { }

        //Let Windows decide how the file should be edited
        public override void EditResource() => Process.Start(FilePath);
    }
}
