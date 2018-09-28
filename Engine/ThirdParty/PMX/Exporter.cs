using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

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
        
        public unsafe void Export(string path)
        {
            int size = CalcSize();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.SequentialScan))
            {
                stream.SetLength(size);
                using (FileMap map = FileMap.FromStream(stream))
                {
                    VoidPtr baseAddr = map.Address;
                    Header* pmx = (Header*)baseAddr;
                    string magic = "PMX ";
                    magic.Write(pmx->_magic);
                    pmx->_version = 2.1f;
                    pmx->_globalsCount = 8;
                    pmx->StringEncoding = EStringEncoding.UTF16LE; //TODO: determine encoding from all strings
                    pmx->ExtraVec4Count = 0;
                    pmx->RigidBodyIndexSize = 0;
                    pmx->MorphIndexSize = 0;
                    //Collect all relevant texture names
                    HashSet<string> texNames = new HashSet<string>();

                    BaseSubMesh[] meshes = _model.CollectAllMeshes();
                    HashSet<string> materialPaths = new HashSet<string>();
                    List<TMaterial> materials = new List<TMaterial>();

                    foreach (BaseSubMesh mesh in meshes)
                        foreach (var lod in mesh.LODs)
                        {
                            var mref = lod.MaterialRef;
                            TMaterial mat = mref?.File;
                            if (mat != null)
                            {
                                string refPathAbs = mref.ReferencePathAbsolute;
                                if (!string.IsNullOrEmpty(refPathAbs) && refPathAbs.IsDirectoryPath() == false)
                                {
                                    if (materialPaths.Contains(refPathAbs))
                                    {

                                    }
                                }
                                if (mat.Textures != null)
                                    foreach (var tex in lod.MaterialRef.File.Textures)
                                    {
                                        if (tex is TexRef2D tex2D)
                                            foreach (var mip in tex2D.Mipmaps)
                                                if (!string.IsNullOrEmpty(mip.ReferencePath) &&
                                                    mip.ReferencePath.IsDirectoryPath() == false)
                                                    texNames.Add(mip.ReferencePath);
                                    }
                            }
                        }

                    int matCount = 0;
                    int texCount = texNames.Count;
                    int boneCount = _skeleton.BoneNameCache.Count;

                    pmx->MaterialIndexSize = (byte)(matCount > sbyte.MaxValue ? (matCount > short.MaxValue ? 4 : 2) : 1);
                    pmx->BoneIndexSize = (byte)(boneCount > sbyte.MaxValue ? (boneCount > short.MaxValue ? 4 : 2) : 1);
                    pmx->TextureIndexSize = (byte)(texCount > sbyte.MaxValue ? (texCount > short.MaxValue ? 4 : 2) : 1);
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
