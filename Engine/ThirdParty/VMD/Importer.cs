using System;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;

namespace TheraEngine.ThirdParty.VMD
{
    public class VMDImporter
    {
        private SkeletalModel _model;
        private Skeleton _skeleton;
        
        public VMDImporter()
        {

        }
        
        public unsafe async Task<SkeletalAnimation> Import(string path)
        {
            FileMap map = FileMap.FromFile(path);

            VoidPtr baseAddr = map.Address;
            Header* vmd = (Header*)baseAddr;
            if (vmd->Magic != Header.MagicString)
            {
                return null;
            }
            uint keyframeCount = vmd->KeyframeCount;
            string targetModel = vmd->ModelName;
            return null;
        }
    }
}
