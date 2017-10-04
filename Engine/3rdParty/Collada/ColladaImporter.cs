using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using static TheraEngine.Rendering.Models.Collada.COLLADA;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryImages;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryVisualScenes;

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
                COLLADA.Scene scene = root.GetChild<COLLADA.Scene>();
                if (scene != null)
                {
                    data.Models = new List<ModelScene>();
                    var visualScenes = scene.GetChildren<COLLADA.Scene.InstanceVisualScene>();
                    foreach (var visualSceneRef in visualScenes)
                    {
                        var visualScene = visualSceneRef.GetUrlInstance();
                        if (visualScene != null)
                        {
                            if (options.ImportModels)
                            {
                                ModelScene modelScene = new ModelScene();
                                var nodes = visualScene.NodeElements;

                                //Collect information for objects and root bones
                                List<Bone> rootBones = new List<Bone>();
                                List<ObjectInfo> objects = new List<ObjectInfo>();
                                List<Camera> cameras = new List<Camera>();
                                List<LightComponent> lights = new List<LightComponent>();
                                foreach (var node in nodes)
                                {
                                    Bone b = EnumNode(null, node, nodes, objects, Matrix4.Identity, Matrix4.Identity, baseTransform, lights, cameras);
                                    if (b != null)
                                        rootBones.Add(b);
                                }

                                //Create meshes after all bones have been created
                                if (rootBones.Count == 0)
                                {
                                    modelScene.StaticModel = new StaticMesh()
                                    {
                                        Name = Path.GetFileNameWithoutExtension(filePath)
                                    };
                                    modelScene.SkeletalModel = null;
                                    modelScene.Skeleton = null;
                                    foreach (ObjectInfo obj in objects)
                                        if (!obj.UsesController)
                                            obj.Initialize(modelScene.StaticModel, visualScene);
                                }
                                else
                                {
                                    modelScene.SkeletalModel = new SkeletalMesh()
                                    {
                                        Name = Path.GetFileNameWithoutExtension(filePath)
                                    };
                                    modelScene.StaticModel = null;
                                    modelScene.Skeleton = new Skeleton(rootBones.ToArray());
                                    foreach (ObjectInfo obj in objects)
                                        if (obj.UsesController)
                                            obj.Initialize(modelScene.SkeletalModel, visualScene);
                                }
                                data.Models.Add(modelScene);
                            }
                            if (options.ImportAnimations)
                            {
                                ModelAnimation anim = new ModelAnimation()
                                {
                                    Name = Path.GetFileNameWithoutExtension(filePath)
                                };
                                foreach (LibraryAnimations lib in root.GetLibraries<LibraryAnimations>())
                                {
                                    lib.anim
                                    ParseAnimation(shell, e, scene.Animation, scene.Skeleton);
                                }
                                data.ModelAnimations.Add(anim);
                            }
                        }
                    }
                }
            }
            
            return data;
        }

        private static void ParseAnimation(DecoderShell shell, AnimationEntry e, ModelAnimation c, Skeleton skel)
        {
            foreach (AnimationEntry e2 in e._animations)
                ParseAnimation(shell, e2, c, skel);

            foreach (ChannelEntry channel in e._channels)
            {
                SamplerEntry sampler = e._samplers.FirstOrDefault(x => x._id == channel._source);

                string[] sidRef = channel._target.Split('/');
                string targetId = sidRef[0];
                if (targetId == ".")
                    continue;

                BaseColladaElement entry = shell._idEntries.ContainsKey(targetId) ? shell._idEntries[targetId] : null;
                if (entry == null)
                    continue;

                string targetName = entry._name ?? entry._id;

                BoneAnimation b;
                if (c._boneAnimations.ContainsKey(targetName))
                    b = c._boneAnimations[targetName];
                else
                    b = c.CreateBoneAnimation(targetName);

                //if (skel[targetId] == null)
                //    continue;

                string targetSID = sidRef[1];
                List<BaseColladaElement> sidEntries = shell._sidEntries.ContainsKey(targetSID) ? shell._sidEntries[targetSID] : null;
                if (sidEntries.Count == 0)
                {
                    throw new Exception("No sid found: " + targetSID);
                }

                float[] timeData = null, outputData = null, inTanData = null, outTanData = null;
                string[] interpData = null;
                foreach (InputEntry input in sampler._inputs)
                {
                    SourceEntry source = e._sources.FirstOrDefault(x => x._id == input._source);

                    switch (input._semantic)
                    {
                        case SemanticType.INPUT:
                            timeData = (float[])source._arrayData;
                            break;
                        case SemanticType.OUTPUT:
                            outputData = (float[])source._arrayData;
                            break;
                        case SemanticType.INTERPOLATION:
                            interpData = (string[])source._arrayData;
                            break;
                        case SemanticType.IN_TANGENT:
                            inTanData = (float[])source._arrayData;
                            break;
                        case SemanticType.OUT_TANGENT:
                            outTanData = (float[])source._arrayData;
                            break;
                    }
                }
                if (targetSID == "matrix")
                {
                    int x = 0;
                    for (int i = 0; i < timeData.Length; ++i, x += 16)
                    {
                        float second = timeData[i];
                        InterpType type = interpData[i].AsEnum<InterpType>();
                        PlanarInterpType pType = (PlanarInterpType)type;
                        Matrix4 matrix = new Matrix4(
                                outputData[x + 00], outputData[x + 01], outputData[x + 02], outputData[x + 03],
                                outputData[x + 04], outputData[x + 05], outputData[x + 06], outputData[x + 07],
                                outputData[x + 08], outputData[x + 09], outputData[x + 10], outputData[x + 11],
                                outputData[x + 12], outputData[x + 13], outputData[x + 14], outputData[x + 15]);
                        FrameState transform = FrameState.DeriveTRS(matrix);
                        b._translation.Add(new Vec3Keyframe(second, transform.Translation, pType));
                        b._scale.Add(new Vec3Keyframe(second, transform.Scale, pType));
                        b._rotation.Add(new QuatKeyframe(second, transform.Quaternion, RadialInterpType.Linear));
                    }
                }
                else if (targetSID == "visibility")
                {
                    for (int i = 0; i < timeData.Length; ++i)
                    {
                        float second = timeData[i];
                        float vis = outputData[i];
                        InterpType type = interpData[i].AsEnum<InterpType>();
                    }
                }
            }
        }
        private enum InterpType
        {
            LINEAR,
            BEZIER,
            HERMITE,
            STEP,
        }

        private static Bone EnumNode(
            Bone parent,
            COLLADA.Node node,
            COLLADA.Node[] nodes,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix,
            Matrix4 invParent,
            Matrix4 rootMatrix,
            List<LightComponent> lights,
            List<Camera> cameras)
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
                EnumNode(parent, e, nodes, objects, bindMatrix, inv, Matrix4.Identity, lights, cameras);

            foreach (IInstanceElement inst in node.InstanceElements)
            {
                //Rigged/morphed mesh?
                if (inst is InstanceController controllerRef)
                {
                    var controller = controllerRef.GetUrlInstance();
                    var child = controller.SkinOrMorphElement;
                    if (child != null)
                    {
                        if (child is COLLADA.LibraryControllers.Controller.Skin skin)
                        {
                            if (skin.Source.GetElement(skin.Root) is COLLADA.LibraryGeometries.Geometry geometry)
                                objects.Add(new ObjectInfo(geometry, skin, bindMatrix, controllerRef, parent, node));
                            else
                                Engine.PrintLine(skin.Source.URI + " does not point to a valid geometry entry.");
                        }
                        else if (child is COLLADA.LibraryControllers.Controller.Morph morph)
                        {
                            //var baseMesh = morph.BaseMeshUrl.GetElement(morph.Root) as COLLADA.LibraryGeometries.Geometry;
                            Engine.PrintLine("Importing morphs is not yet supported.");
                        }
                    }
                }
                //Static mesh?
                else if (inst is InstanceGeometry geomRef)
                {
                    var geometry = geomRef.GetUrlInstance();
                    if (geometry != null)
                        objects.Add(new ObjectInfo(geometry, null, bindMatrix, geomRef, parent, node));
                    else
                        Engine.PrintLine(geomRef.Url.URI + " does not point to a valid geometry entry.");
                   
                }
                //Camera?
                else if (inst is InstanceCamera camRef)
                {
                    var camera = camRef.GetUrlInstance();

                }
                //Light?
                else if (inst is InstanceLight lightRef)
                {
                    var light = lightRef.GetUrlInstance();

                }
                //Another node tree?
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
            public COLLADA.LibraryGeometries.Geometry _geoEntry;
            public COLLADA.LibraryControllers.Controller.ControllerChild _rig;
            public Matrix4 _bindMatrix;
            public IInstanceMesh _inst;
            public COLLADA.Node _node;
            public Bone _parent;

            public ObjectInfo(
                COLLADA.LibraryGeometries.Geometry geoEntry,
                COLLADA.LibraryControllers.Controller.ControllerChild rig,
                Matrix4 bindMatrix,
                IInstanceMesh inst,
                Bone parent,
                COLLADA.Node node)
            {
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _rig = rig;
                _node = node;
                _inst = inst;
                _parent = parent;
            }

            public bool UsesController => _rig != null;

            public void Initialize(SkeletalMesh model, VisualScene scene)
            {
                PrimitiveData data;
                if (_rig != null)
                    data = DecodePrimitivesWeighted(scene, _bindMatrix, _geoEntry, _rig);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst?.
                    BindMaterialElement?.
                    TechniqueCommonElement?.
                    InstanceMaterialElements?[0].
                    Target.GetElement(scene.Root)
                    is COLLADA.LibraryMaterials.Material mat)
                    m = CreateMaterial(mat);

                if (m == null)
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new SkeletalRigidSubMesh(_node.Name ?? (_node.ID ?? _node.SID), data, m, true));
            }
            public void Initialize(StaticMesh model, VisualScene scene)
            {
                PrimitiveData data;
                if (_rig != null)
                    data = DecodePrimitivesWeighted(scene, _bindMatrix, _geoEntry, _rig);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst?.
                    BindMaterialElement?.
                    TechniqueCommonElement?.
                    InstanceMaterialElements?[0].
                    Target.GetElement(scene.Root)
                    is COLLADA.LibraryMaterials.Material mat)
                    m = CreateMaterial(mat);

                if (m == null)
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new StaticRigidSubMesh(_node.Name ?? (_node.ID ?? _node.SID), data, null, m));
            }
        }
        private static Material CreateMaterial(COLLADA.LibraryMaterials.Material colladaMaterial)
        {
            List<TextureReference> texRefs = new List<TextureReference>();

            //Find effect
            Material m = null;
            if (colladaMaterial.InstanceEffectElement != null)
            {
                var eff = colladaMaterial.InstanceEffectElement.Url.GetElement<COLLADA.LibraryEffects.Effect>(colladaMaterial.Root);
                //var profiles = eff.ProfileElements;
                var profileCommon = eff.GetChild<COLLADA.LibraryEffects.Effect.ProfileCommon>();
                if (profileCommon != null)
                {
                    var lightingType = profileCommon.TechniqueElement.LightingTypeElement;
                    var colorTextures = lightingType.GetChildren<BaseFXColorTexture>();
                    foreach (BaseFXColorTexture ct in colorTextures)
                    {
                        var tex = ct.TextureElement;
                        if (tex != null)
                        {
                            var image = tex.Root.GetIDEntry(tex.TextureID);
                            TextureReference texRef = null;
                            if (image is Image14X img14x)
                            {
                                var source = img14x.GetChild<Image14X.ISource>();
                                if (source is Image14X.InitFrom initFrom)
                                {
                                    string path = initFrom.StringContent.Value;
                                    texRef = new TextureReference(Path.GetFileNameWithoutExtension(path), path);
                                }
                                else if (source is Image14X.Data d)
                                {
                                    Engine.PrintLine("Internal image data not supported");
                                    texRef = new TextureReference();
                                }
                            }
                            else if (image is Image15X img15x)
                            {
                                var source = img15x.GetChild<Image15X.InitFrom>();
                                if (source.RefElement != null)
                                {
                                    string path = source.RefElement.StringContent.Value;
                                    texRef = new TextureReference(Path.GetFileNameWithoutExtension(path), path);
                                }
                                else if (source.EmbeddedElement != null)
                                {
                                    Engine.PrintLine("Internal image data not supported");
                                    texRef = new TextureReference();
                                }
                            }
                            texRefs.Add(texRef);
                        }
                    }
                    if (lightingType is Constant constant)
                    {

                    }
                    else if (lightingType is Lambert lambert)
                    {

                    }
                    else if (lightingType is Phong phong)
                    {

                    }
                    else if (lightingType is Blinn blinn)
                    {
                        //m = Material.GetBlinnMaterial();
                    }
                }
            }

            m = texRefs.Count > 0 ?
                Material.GetLitTextureMaterial() :
                Material.GetLitColorMaterial(Color.Magenta);
            m.TexRefs = texRefs.ToArray();
            m.Name = colladaMaterial.Name ?? (colladaMaterial.ID ?? "Unnamed Material");

            return m;
        }
        private enum EInterpolation
        {
            LINEAR,
            BEZIER,
            HERMITE,
            CARDINAL,
            BSPLINE,
        }
    }
}
