using TheraEngine.Animation;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        public class Data
        {
            public List<ModelScene> Models { get; set; }
            public List<ModelAnimation> ModelAnimations { get; set; }
            public List<BasePropertyAnimation> PropertyAnimations { get; set; }
        }
        public static Data Import(string filePath, ModelImportOptions options)
        {
            if (!File.Exists(filePath))
                return null;

            Engine.PrintLine("Importing Collada scene on thread " + Thread.CurrentThread.ManagedThreadId + ".");

            Data data = new Data();

            var root = new XMLSchemeReader<COLLADA>().Import(filePath, false);
            if (root != null)
            {
                Matrix4 baseTransform = options.InitialTransform.Matrix;

                var asset = root.AssetElement;
                if (asset != null)
                {
                    var unit = asset.UnitElement;
                    var coord = asset.UpAxisElement;
                    
                    Engine.PrintLine("Units: {0} (to meters: {1})", unit.Name, unit.Meter.ToString());
                    Engine.PrintLine("Up axis: " + coord.StringContent.Value.ToString());

                    baseTransform = baseTransform * Matrix4.CreateScale(unit.Meter);
                    switch (coord.StringContent.Value)
                    {
                        case Asset.EUpAxis.X_UP:
                            baseTransform = Matrix4.XupToYup * baseTransform;
                            break;
                        case Asset.EUpAxis.Y_UP:
                            break;
                        case Asset.EUpAxis.Z_UP:
                            baseTransform = Matrix4.ZupToYup * baseTransform;
                            break;
                    }
                }

                if (options.ImportModels)
                {
                    if (root != null)
                    {
                        COLLADA.Scene scene = root.GetChild<COLLADA.Scene>();
                        if (scene != null)
                        {
                            var visualScenes = scene.GetChildren<COLLADA.Scene.InstanceVisualScene>();
                            foreach (var visualSceneRef in visualScenes)
                            {
                                var visualScene = visualSceneRef.GetUrlInstance();
                                if (visualScene != null)
                                {
                                    ModelScene modelScene = new ModelScene();
                                    var nodes = visualScene.NodeElements;
                                    foreach (var node in nodes)
                                    {
                                        
                                    }
                                    data.Models.Add(modelScene);
                                }
                            }
                        }
                    }
                }
            }

            //Matrix4 baseTransform = options.InitialTransform.Matrix;
            //if (shell._assets.Count > 0)
            //{
            //    AssetEntry e = shell._assets[0];
            //    baseTransform = baseTransform * Matrix4.CreateScale(e._meter);
            //    if (e._upAxis == EUpAxis.Z_UP)
            //        baseTransform = Matrix4.ZupToYup * baseTransform;
            //}

            //if (options.ImportModels)
            //{
            //    #region Material Extraction
            //    foreach (MaterialEntry mat in shell._materials)
            //    {
            //        List<ImageEntry> imgEntries = new List<ImageEntry>();

            //        //Find effect
            //        if (mat._effect != null)
            //            foreach (EffectEntry eff in shell._effects)
            //                if (eff._id == mat._effect) //Attach textures and effects to material
            //                    if (eff._shader != null)
            //                        foreach (LightEffectEntry l in eff._shader._effects)
            //                            if (l._type == LightEffectType.diffuse && l._texture != null)
            //                            {
            //                                string path = l._texture;
            //                                foreach (EffectNewParam p in eff._newParams)
            //                                    if (p._sid == l._texture)
            //                                    {
            //                                        path = p._sampler2D._url;
            //                                        if (!string.IsNullOrEmpty(p._sampler2D._source))
            //                                            foreach (EffectNewParam p2 in eff._newParams)
            //                                                if (p2._sid == p._sampler2D._source)
            //                                                    path = p2._path;
            //                                    }

            //                                foreach (ImageEntry img in shell._images)
            //                                    if (img._id == path)
            //                                    {
            //                                        imgEntries.Add(img);
            //                                        break;
            //                                    }
            //                            }

            //        Material m = imgEntries.Count > 0 ? Material.GetLitTextureMaterial() : Material.GetLitColorMaterial();//new Material(mat._name != null ? mat._name : mat._id, s);
            //        mat._node = m;

            //        TextureReference[] t = new TextureReference[imgEntries.Count];
            //        for (int i = 0; i < imgEntries.Count; ++i)
            //        {
            //            ImageEntry img = imgEntries[i];
            //            t[i] = new TextureReference(Path.GetFileNameWithoutExtension(img._path), img._path)
            //            {
            //                UWrap = options.TexCoordWrap,
            //                VWrap = options.TexCoordWrap,
            //            };
            //        }
            //        m.TexRefs = t;
            //    }
            //    #endregion

            //    List<ObjectInfo> objects = new List<ObjectInfo>();
            //    List<Bone> rootBones = new List<Bone>();

            //    //Extract bones and objects and create bone tree
            //    foreach (VisualSceneEntry s in shell._visualScenes)
            //        foreach (NodeEntry node in s._nodes)
            //        {
            //            Bone b = EnumNode(null, node, s._nodes, shell, objects, Matrix4.Identity, Matrix4.Identity, baseTransform);
            //            if (b != null)
            //                rootBones.Add(b);
            //        }

            //    foreach (NodeEntry node in shell._nodes)
            //    {
            //        Bone b = EnumNode(null, node, shell._nodes, shell, objects, Matrix4.Identity, Matrix4.Identity, baseTransform);
            //        if (b != null)
            //            rootBones.Add(b);
            //    }

            //    //Create meshes after all bones have been created
            //    if (rootBones.Count == 0)
            //    {
            //        scene.StaticModel = new StaticMesh()
            //        {
            //            Name = Path.GetFileNameWithoutExtension(filePath)
            //        };
            //        scene.SkeletalModel = null;
            //        scene.Skeleton = null;
            //        foreach (ObjectInfo obj in objects)
            //            obj.Initialize(scene.StaticModel, shell);
            //    }
            //    else
            //    {
            //        scene.SkeletalModel = new SkeletalMesh()
            //        {
            //            Name = Path.GetFileNameWithoutExtension(filePath)
            //        };
            //        scene.StaticModel = null;
            //        scene.Skeleton = new Skeleton(rootBones.ToArray());
            //        foreach (ObjectInfo obj in objects)
            //            obj.Initialize(scene.SkeletalModel, shell);
            //    }
            //}

            //if (options.ImportAnimations)
            //{
            //    scene.Animation = new ModelAnimation()
            //    {
            //        Name = Path.GetFileNameWithoutExtension(filePath),
            //        //RootFolder = new AnimFolder("Skeleton"),
            //    };
            //    foreach (AnimationEntry e in shell._animations)
            //        ParseAnimation(shell, e, scene.Animation, scene.Skeleton);
            //}

            return data;
        }

        //private static void ParseAnimation(DecoderShell shell, AnimationEntry e, ModelAnimation c, Skeleton skel)
        //{
        //    foreach (AnimationEntry e2 in e._animations)
        //        ParseAnimation(shell, e2, c, skel);

        //    foreach (ChannelEntry channel in e._channels)
        //    {
        //        SamplerEntry sampler = e._samplers.FirstOrDefault(x => x._id == channel._source);

        //        string[] sidRef = channel._target.Split('/');
        //        string targetId = sidRef[0];
        //        if (targetId == ".")
        //            continue;

        //        BaseColladaElement entry = shell._idEntries.ContainsKey(targetId) ? shell._idEntries[targetId] : null;
        //        if (entry == null)
        //            continue;

        //        string targetName = entry._name ?? entry._id;

        //        BoneAnimation b;
        //        if (c._boneAnimations.ContainsKey(targetName))
        //            b = c._boneAnimations[targetName];
        //        else
        //            b = c.CreateBoneAnimation(targetName);

        //        //if (skel[targetId] == null)
        //        //    continue;

        //        string targetSID = sidRef[1];
        //        List<BaseColladaElement> sidEntries = shell._sidEntries.ContainsKey(targetSID) ? shell._sidEntries[targetSID] : null;
        //        if (sidEntries.Count == 0)
        //        {
        //            throw new Exception("No sid found: " + targetSID);
        //        }

        //        float[] timeData = null, outputData = null, inTanData = null, outTanData = null;
        //        string[] interpData = null;
        //        foreach (InputEntry input in sampler._inputs)
        //        {
        //            SourceEntry source = e._sources.FirstOrDefault(x => x._id == input._source);

        //            switch (input._semantic)
        //            {
        //                case SemanticType.INPUT:
        //                    timeData = (float[])source._arrayData;
        //                    break;
        //                case SemanticType.OUTPUT:
        //                    outputData = (float[])source._arrayData;
        //                    break;
        //                case SemanticType.INTERPOLATION:
        //                    interpData = (string[])source._arrayData;
        //                    break;
        //                case SemanticType.IN_TANGENT:
        //                    inTanData = (float[])source._arrayData;
        //                    break;
        //                case SemanticType.OUT_TANGENT:
        //                    outTanData = (float[])source._arrayData;
        //                    break;
        //            }
        //        }
        //        if (targetSID == "matrix")
        //        {
        //            int x = 0;
        //            for (int i = 0; i < timeData.Length; ++i, x += 16)
        //            {
        //                float second = timeData[i];
        //                InterpType type = interpData[i].AsEnum<InterpType>();
        //                PlanarInterpType pType = (PlanarInterpType)type;
        //                Matrix4 matrix = new Matrix4(
        //                        outputData[x + 00], outputData[x + 01], outputData[x + 02], outputData[x + 03],
        //                        outputData[x + 04], outputData[x + 05], outputData[x + 06], outputData[x + 07],
        //                        outputData[x + 08], outputData[x + 09], outputData[x + 10], outputData[x + 11],
        //                        outputData[x + 12], outputData[x + 13], outputData[x + 14], outputData[x + 15]);
        //                FrameState transform = FrameState.DeriveTRS(matrix);
        //                b._translation.Add(new Vec3Keyframe(second, transform.Translation, pType));
        //                b._scale.Add(new Vec3Keyframe(second, transform.Scale, pType));
        //                b._rotation.Add(new QuatKeyframe(second, transform.Quaternion, RadialInterpType.Linear));
        //            }
        //        }
        //        else if (targetSID == "visibility")
        //        {
        //            for (int i = 0; i < timeData.Length; ++i)
        //            {
        //                float second = timeData[i];
        //                float vis = outputData[i];
        //                InterpType type = interpData[i].AsEnum<InterpType>();
        //            }
        //        }
        //    }
        //}
        //private enum InterpType
        //{
        //    LINEAR,
        //    BEZIER,
        //    HERMITE,
        //    STEP,
        //}

        private static Bone EnumNode(
            Bone parent,
            COLLADA.Node node,
            List<COLLADA.Node> nodes,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix,
            Matrix4 invParent,
            Matrix4 rootMatrix)
        {
            Bone rootBone = null;

            Matrix4 nodeMatrix = node.GetTransformMatrix();
            bindMatrix = rootMatrix * bindMatrix * nodeMatrix;

            if (node.Type == COLLADA.Node.EType.JOINT)
            {
                Bone bone = new Bone(node.Name ?? node.ID, FrameState.DeriveTRS(rootMatrix * nodeMatrix/*invParent * bindMatrix*/));
                node.UserData = bone;
                if (parent == null)
                    rootBone = bone;
                else
                    parent.ChildBones.Add(bone);
                parent = bone;
            }

            Matrix4 inv = bindMatrix.Inverted();
            foreach (COLLADA.Node e in node.NodeElements)
                EnumNode(parent, e, nodes, objects, bindMatrix, inv, Matrix4.Identity);

            foreach (IInstanceElement inst in node.InstanceElements)
            {
                if (inst is InstanceController controllerRef)
                {
                    var controller = controllerRef.GetUrlInstance();
                    foreach (SkinEntry skin in shell._skins)
                        if (skin._id == inst._url)
                        {
                            foreach (GeometryEntry g in shell._geometry)
                                if (g._id == skin._skinSource)
                                {
                                    objects.Add(new ObjectInfo(true, g, bindMatrix, skin, nodes, inst, parent, node));
                                    break;
                                }
                            break;
                        }
                }
                else if (inst is InstanceGeometry geomRef)
                {
                    foreach (GeometryEntry g in shell._geometry)
                        if (g._id == inst._url)
                        {
                            objects.Add(new ObjectInfo(false, g, bindMatrix, null, null, inst, parent, node));
                            break;
                        }
                }
                else if (inst is InstanceCamera camRef)
                {

                }
                else if (inst is InstanceLight lightRef)
                {

                }
                else if (inst is InstanceNode nodeRef)
                {
                    var actualNode = nodeRef.GetUrlInstance();
                    var proxyNode = nodeRef.GetProxyInstance();
                }
            }
            return rootBone;
        }

        private class ObjectInfo
        {
            public bool _weighted;
            public COLLADA.LibraryGeometries.Geometry _geoEntry;
            public Matrix4 _bindMatrix;
            public SkinEntry _skin;
            public IInstanceElement _inst;
            public List<COLLADA.Node> _nodes;
            public COLLADA.Node _node;
            public Bone _parent;

            public ObjectInfo(
                bool weighted,
                COLLADA.LibraryGeometries.Geometry geoEntry,
                Matrix4 bindMatrix,
                SkinEntry skin,
                List<COLLADA.Node> nodes,
                IInstanceElement inst,
                Bone parent,
                COLLADA.Node node)
            {
                _weighted = weighted;
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _skin = skin;
                _nodes = nodes;
                _node = node;
                _inst = inst;
                _parent = parent;
            }

            public void Initialize(SkeletalMesh model)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _nodes);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new SkeletalRigidSubMesh(_node._name ?? _node._id, data, m, true));
            }
            public void Initialize(StaticMesh model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _nodes);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new StaticRigidSubMesh(_node._name ?? _node._id, data, null, m));
            }
        }
        private enum EInterpolation
        {
            LINEAR,
            BEZIER,
            HERMITE,
            CARDINAL,
            BSPLINE,
        }
        private enum ESemantic
        {
            POSITION,
            VERTEX,
            NORMAL,
            TEXCOORD,
            COLOR,
            WEIGHT,
            JOINT,
            INV_BIND_MATRIX,
            TEXTANGENT,
            TEXBINORMAL,
            INPUT,
            OUTPUT,
            IN_TANGENT,
            OUT_TANGENT,
            INTERPOLATION,
            CONTINUITY,
            LINEAR_STEPS,
        }
    }
}
