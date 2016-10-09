using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

namespace ExportScene02
{
    class Program
    {
        const string SAMPLE_FILENAME = "ExportScene02.fbx";

        static void Main(string[] args)
        {

            FbxSdkManager sdkManager = null;
            FbxScene scene = null;
            bool result;

            // Prepare the FBX SDK.
            InitializeSdkObjects(out sdkManager, out scene);

            // Create the scene.

            result = CreateScene(sdkManager, scene);

            if (result == false)
            {
                Console.Write("\n\nAn error occured while creating the scene...\n");
                DestroySdkObjects(sdkManager);
                return;
            }

            // Save the scene.

            // The example can take an output file name as an argument.
            if (args != null && args.Length > 1)
            {
                result = SaveScene(sdkManager, scene, args[1],(int)Skill.FbxSDK.IO.FileFormat.FbxBinary,false);
            }
            // A default output file name is given otherwise.
            else
            {
                result = SaveScene(sdkManager, scene, SAMPLE_FILENAME, (int)Skill.FbxSDK.IO.FileFormat.FbxBinary, false);
            }

            if (result == false)
            {
                Console.Write("\n\nAn error occured while saving the scene...\n");
                DestroySdkObjects(sdkManager);
                return;
            }

            // Destroy all objects created by the FBX SDK.
            DestroySdkObjects(sdkManager);
        }

        static bool CreateScene(FbxSdkManager sdkManager, FbxScene scene)
        {
            FbxNode nurbs = CreateNurbs(sdkManager, "Nurbs");

            MapStretchedShape(sdkManager, nurbs);
            MapBoxShape(sdkManager, nurbs);
            MapTexture(sdkManager, nurbs);
            MapMaterial(sdkManager, nurbs);

            AnimateNurbs(nurbs);

            // Build the node tree.
            FbxNode rootNode = scene.RootNode;
            rootNode.AddChild(nurbs);

            // Identify current take when file is loaded.
            scene.SetCurrentTake("Morph sphere into box");

            return true;
        }

        // Create a sphere. 
        static FbxNode CreateNurbs(FbxSdkManager sdkManager, string name)
        {
            FbxNurb nurbs = FbxNurb.Create(sdkManager, name);

            // Set nurbs properties.
            nurbs.SetOrder(4, 4);
            nurbs.SetStep(2, 2);
            nurbs.InitControlPoints(8, FbxNurb.NurbType.Periodic, 7, FbxNurb.NurbType.Open);

            double[] UKnotVector = { -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0 };            
            System.Runtime.InteropServices.Marshal.Copy(UKnotVector, 0, nurbs.UKnotVector, UKnotVector.Length);

            double[] VKnotVector = { 0.0, 0.0, 0.0, 0.0, 1.0, 2.0, 3.0, 4.0, 4.0, 4.0, 4.0 };
            System.Runtime.InteropServices.Marshal.Copy(VKnotVector, 0, nurbs.VKnotVector, VKnotVector.Length);

            FbxVector4[] vector4 = new FbxVector4[nurbs.ControlPointsCount];
            int i, j;
            double scale = 20.0;
            double[] yAngle = { 90.0, 90.0, 52.0, 0.0, -52.0, -90.0, -90.0 };
            double[] radius = { 0.0, 0.283, 0.872, 1.226, 0.872, 0.283, 0.0 };

            for (i = 0; i < 7; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    double x = scale * radius[i] * Math.Cos(Math.PI / 4 * j);
                    double y = scale * Math.Sin(2 * Math.PI / 360 * yAngle[i]);
                    double z = scale * radius[i] * Math.Sin(Math.PI / 4 * j);
                    double weight = 1.0;

                    vector4[8 * i + j] = new FbxVector4(x, y, z, weight);
                }
            }

            nurbs.ControlPoints = vector4;
            // Create Layer 0 where material and texture will be applied
            FbxLayer l0 = nurbs.GetLayer(0);



            if (l0 == null)
            {
                nurbs.CreateLayer();
            }

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = nurbs;

            return node;
        }
        // Map nurbs control points onto a stretched shape.
        static void MapStretchedShape(FbxSdkManager sdkManager, FbxNode nurbs)
        {
            FbxShape shape = FbxShape.Create(sdkManager, "");

            FbxVector4 extremeRight = new FbxVector4(-250.0, 0.0, 0.0);
            FbxVector4 extremeLeft = new FbxVector4(250.0, 0.0, 0.0);

            shape.InitControlPoints(8 * 7);

            FbxVector4[] vector4 = new FbxVector4[shape.ControlPointsCount];

            int i, j;

            for (i = 0; i < 7; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    if (j < 3 || j > 6)
                    {
                        vector4[8 * i + j] = extremeLeft;
                    }
                    else
                    {
                        vector4[8 * i + j] = extremeRight;
                    }
                }
            }

            shape.ControlPoints = vector4;

            nurbs.Nurb.AddShape(shape, "Stretched");
        }


        // Map nurbs control points onto a box shape.
        static void MapBoxShape(FbxSdkManager sdkManager, FbxNode nurbs)
        {
            FbxShape shape = FbxShape.Create(sdkManager, "");

            shape.InitControlPoints(8 * 7);

            FbxVector4[] vector4 = new FbxVector4[shape.ControlPointsCount];

            int i, j;
            double scale = 20.0;
            double weight = 1.0;
            double[] x = { 0.9, 1.1, 0.0, -1.1, -0.9, -1.1, 0.0, 1.1 };
            double[] z = { 0.0, 1.1, 0.9, 1.1, 0.0, -1.1, -0.9, -1.1 };

            // Top control points.
            for (i = 0; i < 8; i++)
            {
                vector4[i] = new FbxVector4(0.0, scale, 0.0, weight);
            }

            // Middle control points.
            for (i = 1; i < 6; i++)
            {
                double y = 1.0 - 0.5 * (i - 1);

                for (j = 0; j < 8; j++)
                {
                    vector4[8 * i + j] = new FbxVector4(scale * x[j], scale * y, scale * z[j], weight);
                }
            }

            // Bottom control points.
            for (i = 48; i < 56; i++)
            {
                vector4[i] = new FbxVector4(0.0, -scale, 0.0, weight);
            }
            shape.ControlPoints = vector4;

            nurbs.Nurb.AddShape(shape, "Box");
        }


        // Map texture over sphere.
        static void MapTexture(FbxSdkManager sdkManager, FbxNode nurbs)
        {
            FbxTexture texture = FbxTexture.Create(sdkManager, "scene02.jpg");

            // The texture won't be displayed if node shading mode isn't set to KFbxNode::eTEXTURE_SHADING.
            nurbs.Shading_Mode = FbxNode.ShadingMode.TextureShading;

            // Set texture properties.
            texture.SetFileName("scene02.jpg"); // Resource file is in current directory.
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Cylindrical;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;
            texture.SetTranslation(0.45, -0.05);
            texture.SetScale(4.0, 1.0);
            texture.SetRotation(0.0, 0.0);

            // Create LayerElementTexture on Layer 0
            FbxLayerContainer layerContainer = nurbs.Nurb;
            FbxLayerElementTexture layerElementTexture = layerContainer.GetLayer(0).DiffuseTextures;

            if (layerElementTexture == null)
            {
                layerElementTexture = FbxLayerElementTexture.Create(layerContainer, "");
                layerContainer.GetLayer(0).DiffuseTextures = layerElementTexture;
            }

            // The texture is mapped to the whole Nurbs
            layerElementTexture.Mapping_Mode = FbxLayerElement.MappingMode.AllSame;

            // And the texture is avalible in the Direct array
            layerElementTexture.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;
            layerElementTexture.DirectArray.Add(texture);

            //now, we can try to map a texture on the AMBIENT channel of a material.

            //It is important to create a NEW texture and not to simply change the
            //properties of lTexture.

            //Set the Texture properties
            texture = FbxTexture.Create(sdkManager, "grandient.jpg");
            texture.SetFileName("gradient.jpg");
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Cylindrical;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;

            //now we have to get the LayerElementTexture on the channel Ambient
            //if it doesn't exist, we have to create it
            layerElementTexture = layerContainer.GetLayer(0).AmbientTextures;
            if (layerElementTexture == null)
            {
                layerElementTexture = FbxLayerElementTexture.Create(layerContainer, "");
                layerContainer.GetLayer(0).AmbientTextures = layerElementTexture;
            }

            //set the mapping mode to the whole nurb
            layerElementTexture.Mapping_Mode = FbxLayerElement.MappingMode.AllSame;

            //add the texture to the direct array
            layerElementTexture.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;
            layerElementTexture.DirectArray.Add(texture);
        }

        // Map material over sphere.
        static void MapMaterial(FbxSdkManager sdkManager, FbxNode nurbs)
        {
            FbxSurfacePhong material = FbxSurfacePhong.Create(sdkManager, "scene02");
            FbxDouble3 blue = new FbxDouble3(0.0, 0.0, 1.0);
            FbxDouble3 black = new FbxDouble3(0.0, 0.0, 0.0);

            material.EmissiveColor = blue;
            material.AmbientColor = black;
            material.SpecularColor = black;
            material.TransparencyFactor = 0.0;
            material.Shininess = 0.0;
            material.ReflectionFactor = 0.0;



            // Create LayerElementMaterial on Layer 0
            FbxLayerContainer layerContainer = nurbs.Nurb;
            FbxLayerElementMaterial layerElementMaterial = layerContainer.GetLayer(0).Materials;

            if (layerElementMaterial == null)
            {
                layerElementMaterial = FbxLayerElementMaterial.Create(layerContainer, "");
                layerContainer.GetLayer(0).Materials = layerElementMaterial;
            }

            // The material is mapped to the whole Nurbs
            layerElementMaterial.Mapping_Mode = FbxLayerElement.MappingMode.AllSame;

            // And the material is avalible in the Direct array
            layerElementMaterial.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;
            nurbs.AddMaterial(material);
        }

        // Morph sphere into box shape.
        static void AnimateNurbs(FbxNode nurbs)
        {
            string takeName = string.Empty;
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            takeName = "Morph sphere into box";

            nurbs.CreateTakeNode(takeName);
            nurbs.SetCurrentTakeNode(takeName);
            FbxGeometry nurbsAttribute = (FbxGeometry)nurbs.NodeAttribute;

            // The stretched shape is at index 0 because it was added first to the nurbs.
            curve = nurbsAttribute.GetShapeChannel(0, true, takeName);
            curve.KeyModifyBegin();
            {
                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 75.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.25;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);
            }
            curve.KeyModifyEnd();

            // The box shape is at index 1 because it was added second to the nurbs.
            curve = nurbsAttribute.GetShapeChannel(1, true, takeName);
            curve.KeyModifyBegin();
            {
                time.SecondDouble = 1.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.25;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 100.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);
            }
            curve.KeyModifyEnd();
        }

        static bool InitializeSdkObjects(out FbxSdkManager sdkManager, out FbxScene scene)
        {
            // Create the FBX SDK memory manager object.
            // The SDK Manager allocates and frees memory
            // for almost all the classes in the SDK.
            sdkManager = FbxSdkManager.Create();

            if (sdkManager == null)
            {
                Console.Write("Unable to create the FBX SDK manager");
                sdkManager = null;
                scene = null;
                return false;
            }

            // Create the scene object. The scene will hold the data
            // in the file to be imported.
            scene = FbxScene.Create(sdkManager, "");
            if (scene == null)
            {
                Console.Write("Unable to create the Scene");
                sdkManager = null;
                scene = null;
                return false;
            }
            return true;
        }

        static void DestroySdkObjects(FbxSdkManager sdkManager)
        {
            // Delete the FBX SDK manager.
            // All the objects that
            // (1) have been allocated by the memory manager, AND that
            // (2) have not been explicitly destroyed
            // will be automatically destroyed.                        
            if (sdkManager != null)
                sdkManager.Destroy();

        }

        static bool SaveScene(FbxSdkManager sdkManager, FbxScene scene, string exportFileName, int writeFileFormat, bool embedMedia)
        {
            bool status = true;

            // Create an exporter.
            Skill.FbxSDK.IO.FbxExporter exporter = Skill.FbxSDK.IO.FbxExporter.Create(sdkManager, "");

            // Initialize the exporter by providing a filename.
            if (exporter.Initialize(exportFileName) == false)
            {
                Console.Write("Call to FbxExporter.Initialize() failed.");
                Console.Write(string.Format("Error returned: {0}", exporter.LastErrorString));
                return false;
            }

            Version version = Skill.FbxSDK.IO.FbxIO.CurrentVersion;
            Console.Write(string.Format("FBX version number for this FBX SDK is {0}.{1}.{2}",
                      version.Major, version.Minor, version.Revision));


            if (writeFileFormat < 0 ||
                writeFileFormat >=
                    sdkManager.IOPluginRegistry.WriterFormatCount)
            {
                // Write in fall back format if pEmbedMedia is true
                writeFileFormat =
                    sdkManager.IOPluginRegistry.NativeWriterFormat;

                if (!embedMedia)
                {
                    //Try to export in ASCII if possible
                    int formatIndex, formatCount =
                        sdkManager.IOPluginRegistry.
                            WriterFormatCount;

                    for (formatIndex = 0; formatIndex < formatCount; formatIndex++)
                    {
                        if (sdkManager.IOPluginRegistry.
                            WriterIsFBX(formatIndex))
                        {
                            string desc = sdkManager.IOPluginRegistry.
                                GetWriterFormatDescription(formatIndex);
                            if (desc.Contains("ascii"))
                            {
                                writeFileFormat = formatIndex;
                                break;
                            }
                        }
                    }
                }
            }

            // Set the file format
            exporter.FileFormat = writeFileFormat;

            Skill.FbxSDK.IO.FbxStreamOptionsFbxWriter exportOptions =
                Skill.FbxSDK.IO.FbxStreamOptionsFbxWriter.Create(sdkManager, "");

            if (sdkManager.IOPluginRegistry.WriterIsFBX(writeFileFormat))
            {
                // Export options determine what kind of data is to be imported.
                // The default (except for the option eEXPORT_TEXTURE_AS_EMBEDDED)
                // is true, but here we set the options explictly.
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MATERIAL, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.TEXTURE, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.EMBEDDED, embedMedia);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.LINK, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.SHAPE, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GOBO, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.ANIMATION, true);
                exportOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GLOBAL_SETTINGS, true);
            }

            // Export the scene.
            status = exporter.Export(scene, exportOptions);

            if (exportOptions != null)
            {
                exportOptions.Destroy();
                exportOptions = null;
            }

            // Destroy the exporter.
            exporter.Destroy();

            return status;
        }
        
    }
}
