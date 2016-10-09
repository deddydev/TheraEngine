using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

namespace ExportScene05
{
    class Program
    {

        const string SAMPLE_FILENAME = "ExportScene05.fbx";

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
                result = SaveScene(sdkManager, scene, args[1], (int)Skill.FbxSDK.IO.FileFormat.FbxBinary, false);
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

            return;
        }

        static bool CreateScene(FbxSdkManager sdkManager, FbxScene scene)
        {
            FbxVector4 t = new FbxVector4();
            FbxVector4 r = new FbxVector4();
            FbxVector4 s = new FbxVector4();
            FbxXMatrix gM = new FbxXMatrix();

            // Create nodes.
            FbxNode nodeA = FbxNode.Create(sdkManager, "A");
            FbxNode nodeB = FbxNode.Create(sdkManager, "B");
            FbxNode nodeC = FbxNode.Create(sdkManager, "C");
            FbxNode nodeD = FbxNode.Create(sdkManager, "D");

            // Create node attributes.
            FbxSkeleton skeletonA = FbxSkeleton.Create(sdkManager, "");
            skeletonA.Skeleton_Type = FbxSkeleton.SkeletonType.Root;
            nodeA.NodeAttribute = skeletonA;

            FbxSkeleton skeletonB = FbxSkeleton.Create(sdkManager, "");
            skeletonB.Skeleton_Type = FbxSkeleton.SkeletonType.LimbNode;
            nodeB.NodeAttribute = skeletonB;

            FbxSkeleton skeletonC = FbxSkeleton.Create(sdkManager, "");
            skeletonC.Skeleton_Type = FbxSkeleton.SkeletonType.LimbNode;
            nodeC.NodeAttribute = skeletonC;

            FbxSkeleton skeletonD = FbxSkeleton.Create(sdkManager, "");
            skeletonD.Skeleton_Type = FbxSkeleton.SkeletonType.LimbNode;
            nodeD.NodeAttribute = skeletonD;


            // On node A we set translation limits
            nodeA.Limits.TranslationLimitActive = true;
            nodeA.Limits.TranslationLimits.SetLimitMinActive(true, true, true);
            nodeA.Limits.TranslationLimits.LimitMin = new FbxVector4(0.1, 0.2, 0.3);
            nodeA.Limits.TranslationLimits.SetLimitMaxActive(true, true, true);
            nodeA.Limits.TranslationLimits.LimitMax = new FbxVector4(5.0, 1.0, 0.0);

            // On node B we set the rotation order and the pre/post pivots
            // (for these value to have an effect, we need to enable the RotationActive flag)
            nodeB.RotationActive = true;
            // alternatively, if we need to set rotation limits, we can activate the
            // RotationActive flag using the following call: 
            // pNodeB->GetLimits().SetRotationLimitActive(true);
            nodeB.Limits.RotationLimits.SetLimitMaxActive(true, false, false);
            nodeB.Limits.RotationLimits.LimitMax = new FbxVector4(33.3, 0.0, 0.0);

            nodeB.SetPivotState(FbxNode.PivotSet.SourceSet, FbxNode.PivotState.Active);
            nodeB.SetRotationOrder(FbxNode.PivotSet.SourceSet, FbxRotationOrder.SphericXYZ);
            nodeB.SetUseRotationSpaceForLimitOnly(FbxNode.PivotSet.SourceSet, false);
            nodeB.SetUseQuaternionForInterpolation(FbxNode.PivotSet.SourceSet, true);
            nodeB.SetRotationPivot(FbxNode.PivotSet.SourceSet, new FbxVector4(11.1, 22.2, 33.3));
            nodeB.SetPreRotation(FbxNode.PivotSet.SourceSet, new FbxVector4(15.0, 30.0, 45.0));
            nodeB.SetPostRotation(FbxNode.PivotSet.SourceSet, new FbxVector4(-45.0, -30.0, -15.0));

            // Set node hierarchy.
            scene.RootNode.AddChild(nodeA);
            nodeA.AddChild(nodeB);
            nodeB.AddChild(nodeC);
            nodeC.AddChild(nodeD);

            // Set global position of node A.
            t.Set(0.0, 0.0, 0.0); gM.T = t;
            r.Set(0.0, 0.0, 45.0); gM.R = r;
            SetGlobalDefaultPosition(nodeA, gM);

            // Set global position of node B.
            t.Set(30.0, 20.0, 0.0); gM.T = t;
            r.Set(0.0, 0.0, 0.0); gM.R = r;
            SetGlobalDefaultPosition(nodeB, gM);

            // Set global position of node C.
            t.Set(55.0, 20.0, 0.0); gM.T = t;
            r.Set(0.0, 0.0, -40.0); gM.R = r;
            SetGlobalDefaultPosition(nodeC, gM);

            // Set global position of node D.
            t.Set(70.0, 10.0, 0.0); gM.T = t;
            r.Set(0.0, 0.0, 0.0); gM.R = r;
            SetGlobalDefaultPosition(nodeD, gM);

            // Set meta-data on some of the nodes.
            //
            // For this sample, we'll use a hiearchical set of meta-data:
            // 
            // Family
            // 		Type
            // 			Instance
            // 
            // Family contains all the common properties, and the lower levels override various
            // values.
            //
            FbxObjectMetaData familyMetaData = FbxObjectMetaData.Create(scene, "Family");

            FbxDataType level = FbxDataType.Create("Level", FbxType.String);
            FbxProperty.Create(familyMetaData, "Level", level, "Level").Set("Family");

            FbxDataType type = FbxDataType.Create("Type", FbxType.String);
            FbxProperty.Create(familyMetaData, "Type", type, "Type").Set("Wall");

            FbxDataType width = FbxDataType.Create("Width", FbxType.Float1);
            FbxProperty.Create(familyMetaData, "Width", width, "Width").Set(10.0f);

            FbxDataType weight = FbxDataType.Create("Weight", FbxType.Double1);
            FbxProperty.Create(familyMetaData, "Weight", weight, "Weight").Set(25.0);

            FbxDataType cost = FbxDataType.Create("Cost", FbxType.Double1);
            FbxProperty.Create(familyMetaData, "Cost", cost, "Cost").Set(1.25);

            FbxObjectMetaData typeMetaData = familyMetaData.TypedClone(scene, FbxObject.CloneType.Reference);

            typeMetaData.Name = "Type";

            // On this level we'll just override two properties
            typeMetaData.FindProperty("Cost").Set(2500.0);
            typeMetaData.FindProperty("Level").Set("Type");

            FbxObjectMetaData instanceMetaData = typeMetaData.TypedClone(scene, FbxObject.CloneType.Reference);

            instanceMetaData.Name = "Instance";

            // And on this level, we'll go in and add a brand new property, too.
            FbxDataType sku = FbxDataType.Create("Sku", FbxType.String);
            FbxProperty.Create(instanceMetaData, "Sku", sku, "Sku#").Set("143914-10");
            instanceMetaData.FindProperty("Width").Set(1100.50f);
            instanceMetaData.FindProperty("Type").Set("Super Heavy Duty Wall");
            instanceMetaData.FindProperty("Level").Set("Instance");

            // Finally connect metadata information to some of our nodes.
            nodeA.ConnectSrcObject(instanceMetaData, FbxConnectionType.ConnectionDefault);
            nodeC.ConnectSrcObject(instanceMetaData, FbxConnectionType.ConnectionDefault);	// Share the same object

            nodeD.ConnectSrcObject(typeMetaData, FbxConnectionType.ConnectionDefault);

            return true;
        }

        // Function to get a node's global default position.
        // As a prerequisite, parent node's default local position must be already set.
        static void SetGlobalDefaultPosition(FbxNode node, FbxXMatrix globalPosition)
        {
            FbxXMatrix localPosition;
            FbxXMatrix parentGlobalPosition;

            if (node.Parent != null)
            {
                parentGlobalPosition = GetGlobalDefaultPosition(node.Parent);
                localPosition = parentGlobalPosition.Inverse() * globalPosition;
            }
            else
            {
                localPosition = globalPosition;
            }

            node.SetDefaultT(localPosition.T);
            node.SetDefaultR(localPosition.R);
            node.SetDefaultS(localPosition.S);
        }

        // Recursive function to get a node's global default position.
        // As a prerequisite, parent node's default local position must be already set.
        static FbxXMatrix GetGlobalDefaultPosition(FbxNode node)
        {
            FbxXMatrix localPosition = new FbxXMatrix();
            FbxXMatrix globalPosition = new FbxXMatrix();
            FbxXMatrix parentGlobalPosition = new FbxXMatrix();

            FbxVector4 t = new FbxVector4();
            FbxVector4 r = new FbxVector4();
            FbxVector4 s = new FbxVector4();

            node.GetDefaultT(t);
            localPosition.T = t;

            node.GetDefaultR(r);
            localPosition.R = r;
            node.GetDefaultS(s);
            localPosition.S = s;

            if (node.Parent != null)
            {
                parentGlobalPosition = GetGlobalDefaultPosition(node.Parent);
                globalPosition = parentGlobalPosition * localPosition;
            }
            else
            {
                globalPosition = localPosition;
            }

            return globalPosition;
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
