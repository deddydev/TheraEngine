﻿using Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Files.XML;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using static TheraEngine.Rendering.Models.Collada.COLLADA;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryAnimations.Animation;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryImages;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryVisualScenes;
using static TheraEngine.Rendering.Models.Collada.Source;

namespace TheraEngine.Rendering.Models
{
    public partial class Collada
    {
        public class ImportResult
        {
            public List<ModelScene> Models { get; set; }
            public List<BasePropAnim> PropertyAnimations { get; set; }
            public IActor Actor { get; set; }
        }

        public static Task<ImportResult> ImportAsync(
            string filePath,
            ColladaImportOptions options)
            => ImportAsync(filePath, options, null, CancellationToken.None);

        public static async Task<ImportResult> ImportAsync(
            string filePath,
            ColladaImportOptions options,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            if (!File.Exists(filePath))
                return null;

            Engine.Out("Importing Collada scene on thread " + Thread.CurrentThread.ManagedThreadId + ".");

            ImportResult result = new ImportResult();

            var root = await XMLSchemaDefinition<COLLADA>.ImportAsync(filePath, (ulong)options.IgnoreFlags, progress, cancel);
            if (root is null)
                return result;
            
            Matrix4 baseTransform = options.InitialTransform.Matrix;
            var asset = root.AssetElement;
            if (asset != null)
            {
                var unit = asset.UnitElement;
                var coord = asset.UpAxisElement;

                if (unit != null)
                {
                    Engine.Out("Units: {0} (to meters: {1})", unit.Name, unit.Meter.ToString());
                    baseTransform = baseTransform * Matrix4.CreateScale(unit.Meter);
                }
                if (coord != null)
                {
                    Engine.Out("Up axis: " + coord.StringContent.Value.ToString());
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
            }

            COLLADA.Scene scene = root.GetChild<COLLADA.Scene>();
            if (scene is null)
                return result;
            
            result.Models = new List<ModelScene>();
            var visualScenes = scene.GetChildren<COLLADA.Scene.InstanceVisualScene>();
            foreach (var visualSceneRef in visualScenes)
            {
                var visualScene = visualSceneRef.GetUrlInstance();
                if (visualScene != null)
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
                        Bone b = EnumNode(null, node, nodes, objects, baseTransform, Matrix4.Identity, lights, cameras, options.IgnoreFlags);
                        if (b != null)
                            rootBones.Add(b);
                    }

                    //Create meshes after all bones have been created
                    if (rootBones.Count == 0)
                    {
                        modelScene.StaticModel = new StaticModel()
                        {
                            Name = Path.GetFileNameWithoutExtension(filePath)
                        };
                        modelScene.SkeletalModel = null;
                        modelScene.Skeleton = null;
                        foreach (ObjectInfo obj in objects)
                            if (!obj.UsesController)
                                obj.Initialize(modelScene.StaticModel, visualScene);
                            else
                                Engine.LogWarning("Object " + obj._node.Name + " needs bones, but no bones were found.");

                        //modelScene.StaticModel.CollisionShape = ConvexDecomposition.Calculate(
                        //    modelScene.StaticModel.RigidChildren.Select(x => x.LODs[0].PrimitivesRef.File));
                    }
                    else
                    {
                        modelScene.StaticModel = null;
                        modelScene.Skeleton = new Skeleton(rootBones.ToArray());
                        modelScene.SkeletalModel = new SkeletalModel()
                        {
                            Name = Path.GetFileNameWithoutExtension(filePath),
                        };
                        modelScene.SkeletalModel.SkeletonRef.File = modelScene.Skeleton;
                        foreach (ObjectInfo obj in objects)
                            obj.Initialize(modelScene.SkeletalModel, visualScene);
                    }

                    result.Models.Add(modelScene);

                    if (!options.IgnoreFlags.HasFlag(EIgnoreFlags.Animations))
                    {
                        SkeletalAnimation anim = null;
                        float animationLength = 0.0f;
                        foreach (LibraryAnimations lib in root.GetLibraries<LibraryAnimations>())
                            foreach (LibraryAnimations.Animation animElem in lib.AnimationElements)
                            {
                                if (anim is null)
                                {
                                    result.PropertyAnimations = new List<BasePropAnim>();
                                    anim = new SkeletalAnimation()
                                    {
                                        Name = Path.GetFileNameWithoutExtension(filePath),
                                        Looped = true,
                                    };
                                }
                                ParseAnimation(animElem, anim, visualScene, result.PropertyAnimations, ref animationLength);
                            }

                        if (anim != null && animationLength > 0.0f)
                        {
                            anim.SetLength(animationLength, false);
                            Engine.Out("Model animation imported: " + animationLength.ToString() + " seconds / " + Math.Ceiling(animationLength * 60.0f).ToString() + " frames long at 60fps.");
                            result.Models[0].Animation = anim;
                        }
                    }
                }
            }
            
            return result;
        }
        private enum EInterpType
        {
            STEP,
            LINEAR,
            HERMITE,
            BEZIER,
        }

        private static Bone EnumNode(
            Bone parent,
            Node node,
            Node[] nodes,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix,
            Matrix4 invParent,
            List<LightComponent> lights,
            List<Camera> cameras,
            EIgnoreFlags ignore)
        {
            Bone rootBone = null;

            Matrix4 nodeMatrix = node.GetTransformMatrix();
            bindMatrix *= nodeMatrix;

            Matrix4 inv = invParent;
            if (node.Type == Node.EType.JOINT)
            {
                Matrix4 matrix = invParent * bindMatrix;
                Bone bone = new Bone(node.Name ?? node.ID, matrix.DeriveTRS());
                node.UserData = bone;
                if (parent is null)
                    rootBone = bone;
                else
                    bone.Parent = parent;
                parent = bone;
                inv = bindMatrix.Inverted();
            }

            foreach (Node e in node.NodeElements)
            {
                Bone b = EnumNode(parent, e, nodes, objects, bindMatrix, inv, lights, cameras, ignore);
                if (rootBone is null && b != null)
                    rootBone = b;
            }

            foreach (IInstanceElement inst in node.InstanceElements)
            {
                //Rigged/morphed mesh?
                if (inst is InstanceController controllerRef)
                {
                    if (ignore.HasFlag(EIgnoreFlags.Controllers))
                        continue;
                    var controller = controllerRef.GetUrlInstance();
                    var child = controller?.SkinOrMorphElement;
                    if (child != null)
                    {
                        //Rigged mesh
                        if (child is LibraryControllers.Controller.Skin skin)
                        {
                            IID element = skin.Source.GetElement(skin.Root);
                            if (element is LibraryGeometries.Geometry geometry)
                                objects.Add(new ObjectInfo(geometry, skin, bindMatrix, controllerRef, parent, node));
                            else if (element is LibraryControllers.Controller.Morph morph)
                                objects.Add(new ObjectInfo(morph, skin, bindMatrix, controllerRef, parent, node));
                            else
                                Engine.LogWarning(skin.Source.URI + " does not point to a valid geometry or morph controller entry.");
                        }
                        //Static morphed mesh
                        else if (child is LibraryControllers.Controller.Morph morph)
                        {
                            objects.Add(new ObjectInfo(morph, null, bindMatrix, controllerRef, parent, node));
                        }
                        else
                        {
                            Engine.LogWarning("Instanced controller does not have a valid child entry.");
                        }
                    }
                    else
                    {
                        Engine.LogWarning("Instanced controller does not have a valid child entry.");
                    }
                }
                //Static mesh?
                else if (inst is InstanceGeometry geomRef)
                {
                    if (ignore.HasFlag(EIgnoreFlags.Geometry))
                        continue;
                    var geometry = geomRef.GetUrlInstance();
                    if (geometry != null)
                        objects.Add(new ObjectInfo(geometry, null, bindMatrix, geomRef, parent, node));
                    else
                        Engine.LogWarning(geomRef.Url.URI + " does not point to a valid geometry entry.");
                }
                //Camera?
                else if (inst is InstanceCamera camRef)
                {
                    if (ignore.HasFlag(EIgnoreFlags.Cameras))
                        continue;
                    var camera = camRef.GetUrlInstance();

                }
                //Light?
                else if (inst is InstanceLight lightRef)
                {
                    if (ignore.HasFlag(EIgnoreFlags.Lights))
                        continue;
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
            public LibraryControllers.Controller.Morph _morphController;
            public LibraryGeometries.Geometry _geoEntry;
            public LibraryControllers.Controller.Skin _rig;
            public Matrix4 _bindMatrix;
            public IInstanceMesh _inst;
            public Node _node;
            public Bone _parent;

            public ObjectInfo(
                LibraryGeometries.Geometry geoEntry,
                LibraryControllers.Controller.Skin rig,
                Matrix4 bindMatrix,
                IInstanceMesh inst,
                Bone parent,
                Node node)
            {
                _morphController = null;
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _rig = rig;
                _node = node;
                _inst = inst;
                _parent = parent;
            }
            public ObjectInfo(
               LibraryControllers.Controller.Morph morphController,
               LibraryControllers.Controller.Skin rig,
               Matrix4 bindMatrix,
               IInstanceMesh inst,
               Bone parent,
               Node node)
            {
                _morphController = morphController;
                _geoEntry = null;
                _bindMatrix = bindMatrix;
                _rig = rig;
                _node = node;
                _inst = inst;
                _parent = parent;
            }

            /// <summary>
            /// If true, object has bone skinning information.
            /// If false, object is static.
            /// </summary>
            public bool UsesController => _rig != null;

            public void Initialize(SkeletalModel model, VisualScene scene, bool addBinormals = true, bool addTangents = true)
            {
                TMesh data = null;
                if (_rig != null)
                {
                    if (_geoEntry != null)
                        data = DecodePrimitivesWeighted(scene, _bindMatrix, _geoEntry, _rig);
                    else if (_morphController != null)
                        data = DecodeMorphedPrimitivesWeighted(scene, _bindMatrix, _morphController, _rig);
                    else
                        throw new InvalidOperationException("No valid geometry or morph controller entry for object.");
                }
                else
                {
                    if (_geoEntry != null)
                        data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);
                    else if (_morphController != null)
                        data = DecodeMorphedPrimitivesUnweighted(_bindMatrix, _morphController);
                    else
                        throw new InvalidOperationException("No valid geometry or morph controller entry for object.");
                }
                if (data is null)
                {
                    //Something went wrong and the mesh couldn't be created
                    return;
                }

                if (addBinormals || addTangents)
                    data.GenerateBinormalTangentBuffers(0, 0, 0, addBinormals, addTangents);
                
                TMaterial m = null;
                var instanceMats = _inst?.BindMaterialElement?.TechniqueCommonElement?.InstanceMaterialElements;
                if (instanceMats != null && instanceMats.Length > 0)
                {
                    var instanceMat = instanceMats[0];
                    var mat = instanceMat?.Target?.GetElement<LibraryMaterials.Material>(scene.Root);
                    if (mat != null)
                        m = CreateMaterial(mat);
                }

                if (m is null)
                    m = TMaterial.CreateLitColorMaterial();

                Sphere sphere = null;
                DataBuffer posBuf = data[EBufferType.Position];
                if (posBuf != null)
                {
                    Remapper remap = posBuf.GetData(out Vec3[] positions, true);
                    sphere = new Sphere(remap.GetFirstAppearanceBuffer<Vec3>());
                }

                RenderInfo3D renderInfo = new RenderInfo3D(true, false) { CullingVolume = sphere };
                model.RigidChildren.Add(new SkeletalRigidSubMesh(_node.Name ?? (_node.ID ?? _node.SID), renderInfo, ERenderPass.OpaqueDeferredLit, data, m));
            }

            public void Initialize(StaticModel model, VisualScene scene, bool addBinormals = true, bool addTangents = true)
            {
                TMesh data;
                if (_rig != null)
                {
                    if (_geoEntry != null)
                        data = DecodePrimitivesWeighted(scene, _bindMatrix, _geoEntry, _rig);
                    else if (_morphController != null)
                        data = DecodeMorphedPrimitivesWeighted(scene, _bindMatrix, _morphController, _rig);
                    else
                        throw new InvalidOperationException("No valid geometry or morph controller entry for object.");
                }
                else
                {
                    if (_geoEntry != null)
                        data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);
                    else if (_morphController != null)
                        data = DecodeMorphedPrimitivesUnweighted(_bindMatrix, _morphController);
                    else
                        throw new InvalidOperationException("No valid geometry or morph controller entry for object.");
                }
                if (data is null)
                {
                    //Something went wrong and the mesh couldn't be created
                    return;
                }

                if (addBinormals || addTangents)
                    data.GenerateBinormalTangentBuffers(0, 0, 0, addBinormals, addTangents);

                TMaterial m = null;
                var instanceMats = _inst?.BindMaterialElement?.TechniqueCommonElement?.InstanceMaterialElements;
                if (instanceMats != null && instanceMats.Length > 0)
                {
                    var instanceMat = instanceMats[0];
                    var mat = instanceMat?.Target?.GetElement<LibraryMaterials.Material>(scene.Root);
                    if (mat != null)
                        m = CreateMaterial(mat);
                }

                if (m is null)
                    m = TMaterial.CreateLitColorMaterial();

                Sphere sphere = null;
                DataBuffer posBuf = data[EBufferType.Position];
                if (posBuf != null)
                {
                    Remapper remap = posBuf.GetData(out Vec3[] positions, true);
                    sphere = new Sphere(remap.GetFirstAppearanceBuffer<Vec3>());
                }

                RenderInfo3D renderInfo = new RenderInfo3D(true, false) { CullingVolume = sphere };
                model.RigidChildren.Add(new StaticRigidSubMesh(_node.Name ?? (_node.ID ?? _node.SID), renderInfo, ERenderPass.OpaqueDeferredLit, data, m));
            }
        }
        private static TMaterial CreateMaterial(LibraryMaterials.Material colladaMaterial)
        {
            List<TexRef2D> texRefs = new List<TexRef2D>();

            //Find effect
            TMaterial m = null;
            if (colladaMaterial.InstanceEffectElement != null)
            {
                var eff = colladaMaterial.InstanceEffectElement.Url.GetElement<LibraryEffects.Effect>(colladaMaterial.Root);
                //var profiles = eff.ProfileElements;
                var profileCommon = eff.GetChild<LibraryEffects.Effect.ProfileCommon>();
                if (profileCommon != null)
                {
                    var lightingType = profileCommon.TechniqueElement.LightingTypeElement;
                    var colorTextures = lightingType.GetChildren<BaseFXColorTexture>();
                    foreach (BaseFXColorTexture ct in colorTextures)
                    {
                        var tex = ct.TextureElement;
                        if (tex != null)
                        {
                            var image = tex.Root.GetIDEntry<IImage>(tex.TextureID);
                            if (image is null)
                                continue;
                            TexRef2D texRef = new TexRef2D();
                            if (image is Image14X img14x)
                            {
                                var source = img14x.GetChild<Image14X.ISource>();
                                if (source is Image14X.InitFrom initFrom)
                                {
                                    string path = initFrom.StringContent.Value;
                                    texRef = new TexRef2D(Path.GetFileNameWithoutExtension(path), path);
                                }
                                else if (source is Image14X.Data d)
                                {
                                    Engine.Out("Internal image data not supported");
                                    texRef = new TexRef2D();
                                }
                            }
                            else if (image is Image15X img15x)
                            {
                                var source = img15x.GetChild<Image15X.InitFrom>();
                                if (source.RefElement != null)
                                {
                                    string path = source.RefElement.StringContent.Value;
                                    texRef = new TexRef2D(Path.GetFileNameWithoutExtension(path), path);
                                }
                                else if (source.EmbeddedElement != null)
                                {
                                    Engine.Out("Internal image data not supported");
                                    texRef = new TexRef2D();
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
                TMaterial.CreateLitTextureMaterial() :
                TMaterial.CreateLitColorMaterial(Color.Magenta);

            m.Textures = new EventList<BaseTexRef>(texRefs);
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
        private static void ParseAnimation(LibraryAnimations.Animation animElem, SkeletalAnimation anim, VisualScene visualScene, List<BasePropAnim> propAnims, ref float animationLength)
        {
            foreach (var animElemChild in animElem.AnimationElements)
                ParseAnimation(animElemChild, anim, visualScene, propAnims, ref animationLength);

            foreach (var channel in animElem.ChannelElements)
            {
                var sampler = channel.Source.GetElement<Sampler>(animElem.Root);
                ISID target = channel.Target.GetElement(animElem.Root, out string selector);
                if (!(target is IStringElement))
                    continue;

                float[] inputData = null, outputData = null, inTanData = null, outTanData = null;
                string[] interpTypeData = null;
                foreach (var input in sampler.InputElements)
                {
                    Source source = input.Source.GetElement<Source>(sampler.Root);
                    switch (input.CommonSemanticType)
                    {
                        case ESemantic.INPUT:
                            inputData = source.GetArrayElement<FloatArray>().StringContent.Values;
                            break;
                        case ESemantic.OUTPUT:
                            outputData = source.GetArrayElement<FloatArray>().StringContent.Values;
                            break;
                        case ESemantic.INTERPOLATION:
                            interpTypeData = source.GetArrayElement<NameArray>().StringContent.Values;
                            break;
                        case ESemantic.IN_TANGENT:
                            inTanData = source.GetArrayElement<FloatArray>().StringContent.Values;
                            break;
                        case ESemantic.OUT_TANGENT:
                            outTanData = source.GetArrayElement<FloatArray>().StringContent.Values;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(selector))
                {
                    //Animation is targeting only part of the value

                    int matrixIndex = selector.IndexOf('(');
                    if (matrixIndex >= 0)
                    {
                        int rowIndex = -1, colIndex = -1;
                        int rowEnd = selector.IndexOf(')');
                        string row = selector.Substring(matrixIndex + 1, rowEnd - matrixIndex - 1);
                        rowIndex = int.Parse(row);
                    }
                    else
                    {
                        var s = selector.ParseAs<Channel.ESelector>();
                        if (s == Channel.ESelector.ANGLE)
                        {
                            if (!(target is Node.Rotate rotate))
                                Engine.LogWarning("ANGLE channel selector expects Node.Rotate element, but got " + target.GetType().GetFriendlyName());
                            else
                            {
                                Vec4 axisAngle = rotate.StringContent.Value;
                                //bool xAxis = axisAngle.X == 1.0f && axisAngle.Y == 0.0f && axisAngle.Z == 0.0f;
                                //bool yAxis = axisAngle.X == 0.0f && axisAngle.Y == 1.0f && axisAngle.Z == 0.0f;
                                //bool zAxis = axisAngle.X == 0.0f && axisAngle.Y == 0.0f && axisAngle.Z == 1.0f;
                                //if (!xAxis && !yAxis && !zAxis)
                                //{
                                //    Engine.LogWarning("Animation rotation axes that are not the one of the three unit axes are not supported.");
                                //}
                                //else
                                //{
                                Node node = rotate.Parent;
                                string targetName = node.Name ?? (node.ID ?? node.SID);
                                if (node.Type == Node.EType.JOINT)
                                {
                                    BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);
                                    if (!wasFound)
                                    {

                                    }

                                    int x = 0;
                                    for (int i = 0; i < inputData.Length; ++i, x += 2)
                                    {
                                        float second = inputData[i];
                                        float value = outputData[i];
                                        Quat rot = Quat.FromAxisAngleDeg(axisAngle.Xyz, value);
                                        EInterpType type = interpTypeData[i].AsEnum<EInterpType>();
                                        ERadialInterpType rType;

                                        //TODO: parse tangents for slerp
                                        Quat inTan = Quat.Identity, outTan = Quat.Identity;
                                        switch (type)
                                        {
                                            case EInterpType.STEP:
                                                rType = ERadialInterpType.Step;
                                                break;
                                            case EInterpType.LINEAR:
                                                rType = ERadialInterpType.Linear;
                                                break;
                                            case EInterpType.HERMITE:
                                                rType = ERadialInterpType.CubicBezier;
                                                //inTan = inTanData[i];
                                                //outTan = outTanData[i];
                                                break;
                                            case EInterpType.BEZIER:
                                                rType = ERadialInterpType.CubicBezier;
                                                //inTan = (inTanData[x + 1] - value) / (inTanData[x] - second);
                                                //outTan = (outTanData[x + 1] - value) / (outTanData[x] - second);
                                                //if (float.IsNaN(inTan) || float.IsInfinity(inTan))
                                                //{
                                                //    inTan = 0.0f;
                                                //    //Engine.PrintLine("Invalid in-tangent calculated");
                                                //}
                                                //if (float.IsNaN(outTan) || float.IsInfinity(outTan))
                                                //{
                                                //    outTan = 0.0f;
                                                //    //Engine.PrintLine("Invalid out-tangent calculated");
                                                //}
                                                break;
                                            default:
                                                continue;
                                        }

                                        QuatKeyframe kf = new QuatKeyframe(second, rot, inTan, outTan, rType);
                                        animationLength = Math.Max(animationLength, second);
                                        bone.Rotation.Keyframes.Add(kf);
                                    }
                                }
                                else
                                {
                                    Engine.LogWarning("Non-joint axis-angle rotation animation not supported.");
                                }
                                //}
                            }
                        }
                        else if (s == Channel.ESelector.TIME)
                        {
                            Engine.LogWarning("TIME animation selector not supported.");
                        }
                        else
                        {
                            int valueIndex = ((int)s) & 0b11;
                            if (target is Node.Translate translate)
                            {
                                Node node = translate.Parent;
                                string targetName = node.Name ?? (node.ID ?? node.SID);
                                if (node.Type == Node.EType.JOINT)
                                {
                                    BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                    int x = 0;
                                    for (int i = 0; i < inputData.Length; ++i, x += 2)
                                    {
                                        float second = inputData[i];
                                        float value = outputData[i];
                                        EInterpType type = interpTypeData[i].AsEnum<EInterpType>();
                                        EVectorInterpType pType = (EVectorInterpType)(int)type;

                                        float inTan = 0.0f, outTan = 0.0f;
                                        switch (pType)
                                        {
                                            case EVectorInterpType.CubicHermite:
                                                inTan = inTanData[i];
                                                outTan = outTanData[i];
                                                break;
                                            case EVectorInterpType.CubicBezier:
                                                inTan = (inTanData[x + 1] - value) / (inTanData[x] - second);
                                                outTan = (outTanData[x + 1] - value) / (outTanData[x] - second);
                                                if (float.IsNaN(inTan) || float.IsInfinity(inTan))
                                                {
                                                    inTan = 0.0f;
                                                    //Engine.PrintLine("Invalid in-tangent calculated");
                                                }
                                                if (float.IsNaN(outTan) || float.IsInfinity(outTan))
                                                {
                                                    outTan = 0.0f;
                                                    //Engine.PrintLine("Invalid out-tangent calculated");
                                                }
                                                break;
                                        }

                                        FloatKeyframe kf = new FloatKeyframe(second, value, inTan, outTan, pType);
                                        animationLength = Math.Max(animationLength, second);
                                        switch (valueIndex)
                                        {
                                            case 0: bone.TranslationX.Keyframes.Add(kf); break;
                                            case 1: bone.TranslationY.Keyframes.Add(kf); break;
                                            case 2: bone.TranslationZ.Keyframes.Add(kf); break;
                                        }
                                    }
                                }
                                else
                                {
                                    Engine.LogWarning("Non-joint single-axis translation animation not supported.");
                                }
                            }
                            else if (target is Node.Scale scale)
                            {
                                Node node = scale.Parent;
                                string targetName = node.Name ?? (node.ID ?? node.SID);
                                if (node.Type == Node.EType.JOINT)
                                {
                                    BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                    int x = 0;
                                    for (int i = 0; i < inputData.Length; ++i, x += 2)
                                    {
                                        float second = inputData[i];
                                        float value = outputData[i];
                                        EInterpType type = interpTypeData[i].AsEnum<EInterpType>();
                                        EVectorInterpType pType = (EVectorInterpType)(int)type;

                                        float inTan = 0.0f, outTan = 0.0f;
                                        switch (pType)
                                        {
                                            case EVectorInterpType.CubicHermite:
                                                inTan = inTanData[i];
                                                outTan = outTanData[i];
                                                break;
                                            case EVectorInterpType.CubicBezier:
                                                inTan = (inTanData[x + 1] - value) / (inTanData[x] - second);
                                                outTan = (outTanData[x + 1] - value) / (outTanData[x] - second);
                                                if (float.IsNaN(inTan) || float.IsInfinity(inTan))
                                                {
                                                    inTan = 0.0f;
                                                    //Engine.PrintLine("Invalid in-tangent calculated");
                                                }
                                                if (float.IsNaN(outTan) || float.IsInfinity(outTan))
                                                {
                                                    outTan = 0.0f;
                                                    //Engine.PrintLine("Invalid out-tangent calculated");
                                                }
                                                break;
                                        }

                                        FloatKeyframe kf = new FloatKeyframe(second, value, inTan, outTan, pType);
                                        animationLength = Math.Max(animationLength, second);
                                        switch (valueIndex)
                                        {
                                            case 0: bone.ScaleX.Keyframes.Add(kf); break;
                                            case 1: bone.ScaleY.Keyframes.Add(kf); break;
                                            case 2: bone.ScaleZ.Keyframes.Add(kf); break;
                                        }
                                    }
                                }
                                else
                                {
                                    Engine.LogWarning("Non-joint single-axis scale animation not supported.");
                                }
                            }
                            else if (target is BaseFXColorTexture.Color color)
                            {
                                Engine.LogWarning("Reading animation for " + color.GetType().GetFriendlyName() + " is not supported.");
                            }
                            else
                            {
                                Engine.LogWarning("Reading single-axis animation for " + target.GetType().GetFriendlyName() + " is not supported.");
                            }
                        }
                    }
                }
                else
                {
                    if (target is Node.Matrix mtx)
                    {
                        Node node = mtx.Parent;
                        string targetName = node.Name ?? (node.ID ?? node.SID);

                        if (node.Type == Node.EType.JOINT)
                        {
                            int x = 0;
                            for (int i = 0; i < inputData.Length; ++i, x += 16)
                            {
                                float second = inputData[i];
                                EInterpType type = interpTypeData[i].AsEnum<EInterpType>();
                                EVectorInterpType pType = (EVectorInterpType)(int)type;

                                float inTan = 0.0f, outTan = 0.0f;
                                switch (pType)
                                {
                                    case EVectorInterpType.CubicHermite:
                                        inTan = inTanData[i];
                                        outTan = outTanData[i];
                                        break;
                                    case EVectorInterpType.CubicBezier:
                                        Engine.LogWarning("Matrix has bezier interpolation");
                                        //inTan = (inTanData[x + 1] - value) / (inTanData[x] - second);
                                        //outTan = (outTanData[x + 1] - value) / (outTanData[x] - second);
                                        //if (float.IsNaN(inTan) || float.IsInfinity(inTan) || float.IsNaN(outTan) || float.IsInfinity(outTan))
                                        //    Engine.PrintLine("Invalid tangent calculated");
                                        break;
                                }

                                Matrix4 matrix = new Matrix4(
                                        outputData[x + 00], outputData[x + 01], outputData[x + 02], outputData[x + 03],
                                        outputData[x + 04], outputData[x + 05], outputData[x + 06], outputData[x + 07],
                                        outputData[x + 08], outputData[x + 09], outputData[x + 10], outputData[x + 11],
                                        outputData[x + 12], outputData[x + 13], outputData[x + 14], outputData[x + 15]);

                                TTransform transform = matrix.DeriveTRS();

                                BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                bone.TranslationX.Keyframes.Add(new FloatKeyframe(second, transform.Translation.X, inTan, outTan, pType));
                                bone.TranslationY.Keyframes.Add(new FloatKeyframe(second, transform.Translation.Y, inTan, outTan, pType));
                                bone.TranslationZ.Keyframes.Add(new FloatKeyframe(second, transform.Translation.Z, inTan, outTan, pType));

                                //TODO: proper tangent
                                bone.Rotation.Keyframes.Add(new QuatKeyframe(second, transform.Rotation.Value, transform.Rotation.Value, ERadialInterpType.CubicBezier));

                                bone.ScaleX.Keyframes.Add(new FloatKeyframe(second, transform.Scale.X, inTan, outTan, pType));
                                bone.ScaleY.Keyframes.Add(new FloatKeyframe(second, transform.Scale.Y, inTan, outTan, pType));
                                bone.ScaleZ.Keyframes.Add(new FloatKeyframe(second, transform.Scale.Z, inTan, outTan, pType));

                                animationLength = Math.Max(animationLength, second);
                            }
                        }
                        else
                        {
                            //Engine.LogWarning("Non-joint transform matrix animation not supported.");
                        }
                    }
                    else if (target is Node.Translate trans)
                    {
                        if (trans.Parent.Type == Node.EType.JOINT)
                        {
                            Engine.LogWarning("Joint all-axes translation animation not supported.");
                        }
                        else
                        {
                            Engine.LogWarning("Non-joint all-axes translation animation not supported.");
                        }
                    }
                    else if (target is Node.Rotate rot)
                    {
                        if (rot.Parent.Type == Node.EType.JOINT)
                        {
                            Engine.LogWarning("Joint all-axes rotation animation not supported.");
                        }
                        else
                        {
                            Engine.LogWarning("Non-joint all-axes rotation animation not supported.");
                        }
                    }
                    else if (target is Node.Scale scale)
                    {
                        if (scale.Parent.Type == Node.EType.JOINT)
                        {
                            Engine.LogWarning("Joint all-axes scale animation not supported.");
                        }
                        else
                        {
                            Engine.LogWarning("Non-joint all-axes scale animation not supported.");
                        }
                    }
                }
            }
        }
    }
}
