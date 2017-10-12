using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using static TheraEngine.Rendering.Models.Collada.COLLADA;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryAnimations.Animation;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryImages;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryVisualScenes;
using static TheraEngine.Rendering.Models.Collada.Source;

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
            //using (FileMap map = FileMap.FromFile(filePath, FileMapProtect.Read))
            //using (XMLReader reader = new XMLReader(map.Address, map.Length, true))
            {
                var schemeReader = new XMLSchemeReader<COLLADA>();
                var root = schemeReader.Import(filePath, false);
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
                    Scene scene = root.GetChild<Scene>();
                    if (scene != null)
                    {
                        data.Models = new List<ModelScene>();
                        var visualScenes = scene.GetChildren<Scene.InstanceVisualScene>();
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
                                            else
                                                Engine.PrintLine("Model contains no bones, but has one or more controllers.");
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
                                            obj.Initialize(modelScene.SkeletalModel, visualScene);
                                    }
                                    data.Models.Add(modelScene);
                                }
                                if (options.ImportAnimations)
                                {
                                    data.ModelAnimations = new List<ModelAnimation>();
                                    data.PropertyAnimations = new List<BasePropertyAnimation>();
                                    ModelAnimation anim = new ModelAnimation()
                                    {
                                        Name = Path.GetFileNameWithoutExtension(filePath),
                                        Looped = true,
                                    };
                                    float animationLength = 0.0f;
                                    foreach (LibraryAnimations lib in root.GetLibraries<LibraryAnimations>())
                                        foreach (LibraryAnimations.Animation animElem in lib.AnimationElements)
                                            ParseAnimation(animElem, anim, visualScene, data.PropertyAnimations, ref animationLength);
                                    anim.SetLength(animationLength, false);
                                    Engine.PrintLine("Model animation imported: " + animationLength.ToString() + " seconds / " + Math.Ceiling(animationLength * 60.0f).ToString() + " frames long at 60fps.");
                                    data.ModelAnimations.Add(anim);
                                }
                            }
                        }
                    }
                }
            }
            return data;
        }

        private static void ParseAnimation(LibraryAnimations.Animation animElem, ModelAnimation anim, VisualScene visualScene, List<BasePropertyAnimation> propAnims, ref float animationLength)
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
                            if (target is Node.Rotate rotate)
                            {
                                Vec4 axisAngle = rotate.StringContent.Value;
                                bool xAxis = axisAngle.X == 1.0f && axisAngle.Y == 0.0f && axisAngle.Z == 0.0f;
                                bool yAxis = axisAngle.X == 0.0f && axisAngle.Y == 1.0f && axisAngle.Z == 0.0f;
                                bool zAxis = axisAngle.X == 0.0f && axisAngle.Y == 0.0f && axisAngle.Z == 1.0f;
                                if (!xAxis && !yAxis && !zAxis)
                                {
                                    Engine.PrintLine("Animation rotation axes that are not the unit axes are not supported.");
                                }
                                else
                                {
                                    Node node = rotate.ParentElement;
                                    string targetName = node.Name ?? (node.ID ?? node.SID);
                                    if (node.Type == Node.EType.JOINT)
                                    {
                                        BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);
                                        //if (!wasFound)
                                        //    bone.EulerOrder = RotationOrder.PYR;
                                        
                                        int x = 0;
                                        for (int i = 0; i < inputData.Length; ++i, x += 2)
                                        {
                                            float second = inputData[i];
                                            float value = outputData[i];
                                            InterpType type = interpTypeData[i].AsEnum<InterpType>();
                                            PlanarInterpType pType = (PlanarInterpType)(int)type;

                                            float inTan = 0.0f, outTan = 0.0f;
                                            switch (pType)
                                            {
                                                case PlanarInterpType.CubicHermite:
                                                    inTan = inTanData[i];
                                                    outTan = outTanData[i];
                                                    break;
                                                case PlanarInterpType.CubicBezier:
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

                                            animationLength = Math.Max(animationLength, second);
                                            if (xAxis)
                                                bone.RotationX.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                            else if (yAxis)
                                                bone.RotationY.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                            else
                                                bone.RotationZ.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                        }
                                    }
                                    else
                                    {
                                        Engine.PrintLine("Generic node selective-rotation animation not supported.");
                                    }
                                }
                            }
                            else
                                Engine.PrintLine("ANGLE channel selector expects Node.Rotate element, but got " + target.GetType().GetFriendlyName());
                        }
                        else if (s == Channel.ESelector.TIME)
                        {
                            Engine.PrintLine("TIME animation selector not supported.");
                        }
                        else
                        {
                            int valueIndex = ((int)s) & 0b11;
                            if (target is Node.Translate translate)
                            {
                                Node node = translate.ParentElement;
                                string targetName = node.Name ?? (node.ID ?? node.SID);
                                if (node.Type == Node.EType.JOINT)
                                {
                                    BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                    int x = 0;
                                    for (int i = 0; i < inputData.Length; ++i, x += 2)
                                    {
                                        float second = inputData[i];
                                        float value = outputData[i];
                                        InterpType type = interpTypeData[i].AsEnum<InterpType>();
                                        PlanarInterpType pType = (PlanarInterpType)(int)type;

                                        float inTan = 0.0f, outTan = 0.0f;
                                        switch (pType)
                                        {
                                            case PlanarInterpType.CubicHermite:
                                                inTan = inTanData[i];
                                                outTan = outTanData[i];
                                                break;
                                            case PlanarInterpType.CubicBezier:
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

                                        animationLength = Math.Max(animationLength, second);
                                        switch (valueIndex)
                                        {
                                            case 0:
                                                bone.TranslationX.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                            case 1:
                                                bone.TranslationY.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                            case 2:
                                                bone.TranslationZ.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    Engine.PrintLine("Generic node selective-translation animation not supported.");
                                }
                            }
                            else if (target is Node.Scale scale)
                            {
                                Node node = scale.ParentElement;
                                string targetName = node.Name ?? (node.ID ?? node.SID);
                                if (node.Type == Node.EType.JOINT)
                                {
                                    BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                    int x = 0;
                                    for (int i = 0; i < inputData.Length; ++i, x += 2)
                                    {
                                        float second = inputData[i];
                                        float value = outputData[i];
                                        InterpType type = interpTypeData[i].AsEnum<InterpType>();
                                        PlanarInterpType pType = (PlanarInterpType)(int)type;

                                        float inTan = 0.0f, outTan = 0.0f;
                                        switch (pType)
                                        {
                                            case PlanarInterpType.CubicHermite:
                                                inTan = inTanData[i];
                                                outTan = outTanData[i];
                                                break;
                                            case PlanarInterpType.CubicBezier:
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

                                        animationLength = Math.Max(animationLength, second);
                                        switch (valueIndex)
                                        {
                                            case 0:
                                                bone.ScaleX.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                            case 1:
                                                bone.ScaleY.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                            case 2:
                                                bone.ScaleZ.Add(new FloatKeyframe(second, value, inTan, outTan, pType));
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    Engine.PrintLine("Generic node selective-scale animation not supported.");
                                }
                            }
                            else if (target is BaseFXColorTexture.Color color)
                            {
                                Engine.PrintLine("Reading animation for " + color.GetType().GetFriendlyName() + " is not supported.");
                            }
                            else
                            {
                                Engine.PrintLine("Reading selective animation for " + target.GetType().GetFriendlyName());
                            }
                        }
                    }
                }
                else
                {
                    if (target is Node.Matrix mtx)
                    {
                        Node node = mtx.ParentElement;
                        string targetName = node.Name ?? (node.ID ?? node.SID);
                        if (node.Type == Node.EType.JOINT)
                        {
                            int x = 0;
                            for (int i = 0; i < inputData.Length; ++i, x += 16)
                            {
                                float second = inputData[i];
                                InterpType type = interpTypeData[i].AsEnum<InterpType>();
                                PlanarInterpType pType = (PlanarInterpType)(int)type;

                                float inTan = 0.0f, outTan = 0.0f;
                                switch (pType)
                                {
                                    case PlanarInterpType.CubicHermite:
                                        inTan = inTanData[i];
                                        outTan = outTanData[i];
                                        break;
                                    case PlanarInterpType.CubicBezier:
                                        Engine.PrintLine("Matrix has bezier interpolation");
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

                                LocalRotTransform transform = LocalRotTransform.DeriveTRS(matrix);

                                BoneAnimation bone = anim.FindOrCreateBoneAnimation(targetName, out bool wasFound);

                                bone.TranslationX.Add(new FloatKeyframe(second, transform.Translation.X, inTan, outTan, pType));
                                bone.TranslationY.Add(new FloatKeyframe(second, transform.Translation.Y, inTan, outTan, pType));
                                bone.TranslationZ.Add(new FloatKeyframe(second, transform.Translation.Z, inTan, outTan, pType));

                                bone.RotationY.Add(new FloatKeyframe(second, transform.Rotation.Yaw, inTan, outTan, pType));
                                bone.RotationX.Add(new FloatKeyframe(second, transform.Rotation.Pitch, inTan, outTan, pType));
                                bone.RotationZ.Add(new FloatKeyframe(second, transform.Rotation.Roll, inTan, outTan, pType));

                                bone.ScaleX.Add(new FloatKeyframe(second, transform.Scale.X, inTan, outTan, pType));
                                bone.ScaleY.Add(new FloatKeyframe(second, transform.Scale.Y, inTan, outTan, pType));
                                bone.ScaleZ.Add(new FloatKeyframe(second, transform.Scale.Z, inTan, outTan, pType));

                                animationLength = Math.Max(animationLength, second);
                            }
                        }
                        else
                        {
                            Engine.PrintLine("Generic node matrix animation not supported.");
                        }
                    }
                    else if (target is Node.Translate trans)
                    {
                        if (trans.ParentElement.Type == Node.EType.JOINT)
                        {
                            Engine.PrintLine("Bone full-translation animation not supported.");
                        }
                        else
                        {
                            Engine.PrintLine("Generic node translation animation not supported.");
                        }
                    }
                    else if (target is Node.Rotate rot)
                    {
                        if (rot.ParentElement.Type == Node.EType.JOINT)
                        {
                            Engine.PrintLine("Bone full-rotation animation not supported.");
                        }
                        else
                        {
                            Engine.PrintLine("Generic node rotation animation not supported.");
                        }
                    }
                    else if (target is Node.Scale scale)
                    {
                        if (scale.ParentElement.Type == Node.EType.JOINT)
                        {
                            Engine.PrintLine("Bone full-scale animation not supported.");
                        }
                        else
                        {
                            Engine.PrintLine("Generic node scale animation not supported.");
                        }
                    }
                }
            }
        }
        private enum InterpType
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
            Matrix4 rootMatrix,
            List<LightComponent> lights,
            List<Camera> cameras)
        {
            Bone rootBone = null;

            Matrix4 nodeMatrix = node.GetTransformMatrix();
            bindMatrix = rootMatrix * bindMatrix * nodeMatrix;

            if (node.Type == Node.EType.JOINT)
            {
                Bone bone = new Bone(node.Name ?? node.ID, LocalRotTransform.DeriveTRS(rootMatrix * nodeMatrix/*invParent * bindMatrix*/));
                node.UserData = bone;
                if (parent == null)
                    rootBone = bone;
                else
                    parent.ChildBones.Add(bone);
                parent = bone;
            }

            Matrix4 inv = bindMatrix.Inverted();
            foreach (Node e in node.NodeElements)
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
                        if (child is LibraryControllers.Controller.Skin skin)
                        {
                            if (skin.Source.GetElement(skin.Root) is LibraryGeometries.Geometry geometry)
                                objects.Add(new ObjectInfo(geometry, skin, bindMatrix, controllerRef, parent, node));
                            else
                                Engine.PrintLine(skin.Source.URI + " does not point to a valid geometry entry.");
                        }
                        else if (child is LibraryControllers.Controller.Morph morph)
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
            public LibraryGeometries.Geometry _geoEntry;
            public LibraryControllers.Controller.ControllerChild _rig;
            public Matrix4 _bindMatrix;
            public IInstanceMesh _inst;
            public Node _node;
            public Bone _parent;

            public ObjectInfo(
                LibraryGeometries.Geometry geoEntry,
                LibraryControllers.Controller.ControllerChild rig,
                Matrix4 bindMatrix,
                IInstanceMesh inst,
                Bone parent,
                Node node)
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
                //if (_inst?.
                //    BindMaterialElement?.
                //    TechniqueCommonElement?.
                //    InstanceMaterialElements?[0].
                //    Target.GetElement(scene.Root)
                //    is LibraryMaterials.Material mat)
                //    m = CreateMaterial(mat);

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
                //if (_inst?.
                //    BindMaterialElement?.
                //    TechniqueCommonElement?.
                //    InstanceMaterialElements?[0].
                //    Target.GetElement(scene.Root)
                //    is LibraryMaterials.Material mat)
                //    m = CreateMaterial(mat);

                if (m == null)
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new StaticRigidSubMesh(_node.Name ?? (_node.ID ?? _node.SID), data, null, m));
            }
        }
        private static Material CreateMaterial(LibraryMaterials.Material colladaMaterial)
        {
            List<TextureReference> texRefs = new List<TextureReference>();

            //Find effect
            Material m = null;
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
