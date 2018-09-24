using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models;

namespace TheraEngine.ThirdParty.PMX
{
    public class PMXExporter
    {
        private SkeletalModel _model;
        private Skeleton _skeleton;

        public PMXExporter(SkeletalModel model)
        {
            _model = model;
            _skeleton = _model?.SkeletonRef?.File;
        }
        public PMXExporter(SkeletalModel model, Skeleton skeleton)
        {
            _model = model;
            _skeleton = skeleton;
        }
        
        public void Export(string path)
        {
            int size = CalcSize();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.SequentialScan))
            {
                stream.SetLength(size);
                using (FileMap map = FileMap.FromStream(stream))
                {

                }
            }
        }

        private int CalcSize()
        {
            int size = 0;
            return size;
        }
    }
}
