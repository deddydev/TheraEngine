using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;
using Skill.FbxSDK.Arrays;
using System.IO;

namespace ImportScene
{
    class Program
    {




        #region Constructor

        public Program(string filename)
        {
            FbxSdkManager SdkManager = null;
            FbxScene Scene = null;
            bool Result;



            // Prepare the FBX SDK.
            InitializeSdkObjects(out SdkManager, out Scene);
            // Load the scene.

            // The example can take a FBX file as an argument.

            Result = LoadScene(SdkManager, Scene, filename, "");
            if (Result == false)
            {
                Writer.WriteLine("\n\nAn error occured while loading the scene...");
                return;
            }
            else
            {
                // Display the scene.
                DisplayMetaData(Scene);

                Writer.WriteLine("\n\n---------------------\nGlobal Light Settings\n---------------------\n\n");
                Console.WriteLine("Global Light Settings");

                DisplayGlobalLightSettings(Scene.GlobalLightSettings);

                Writer.WriteLine("\n\n----------------------\nGlobal Camera Settings\n----------------------\n\n");
                Console.WriteLine("Global Camera Settings");

                DisplayGlobalCameraSettings(Scene.GlobalCameraSettings);

                Writer.WriteLine("\n\n--------------------\nGlobal Time Settings\n--------------------\n\n");
                Console.WriteLine("Global Time Settings");

                DisplayGlobalTimeSettings(Scene.GlobalTimeSettings);

                Writer.WriteLine("\n\n---------\nHierarchy\n---------\n\n");
                Console.WriteLine("Hierarchy");

                DisplayHierarchy(Scene);

                Writer.WriteLine("\n\n------------\nNode Content\n------------\n\n");
                Console.WriteLine("Node Content");

                DisplayContent(Scene);

                Writer.WriteLine("\n\n----\nPose\n----\n\n");
                Console.WriteLine("Pose");

                DisplayPose(Scene);

                Writer.WriteLine("\n\n---------\nAnimation\n---------\n\n");
                Console.WriteLine("Animation");

                DisplayAnimation(Scene);

                //now display generic information

                Writer.WriteLine("\n\n---------\nGeneric Information\n---------\n\n");
                Console.WriteLine("Generic Information");


                DisplayGenericInfo(Scene);
            }

            // Destroy all objects created by the FBX SDK.
            DestroySdkObjects(SdkManager);

            // Console.ReadLine();

        }
        #endregion

        #region Main

        static void Main(string[] args)
        {
            Program p;
            if (args.Length > 1)
            {
                Writer.WriteLine(args[1]);

                p = new Program(args[1]);
            }
            else
            {
                p = new Program("c://cube.fbx");

                Writer.WriteLine("\n\nUsage: ImportScene <FBX file name>\n\n");
            }
            Writer.flush();
            Writer.close();




        }

        #endregion

        #region Display


        private void DisplayMetaData(FbxScene Scene)
        {
            FbxDocumentInfo sceneInfo = Scene.SceneInfo;
            if (sceneInfo != null)
            {
                Writer.WriteLine("\n\n--------------------\nMeta-Data\n--------------------\n\n");
                Writer.WriteLine(string.Format("    Title: {0}\n", sceneInfo.Title));
                Writer.WriteLine(string.Format("    Subject: {0}\n", sceneInfo.Subject));
                Writer.WriteLine(string.Format("    Author: {0}\n", sceneInfo.Author));
                Writer.WriteLine(string.Format("    Keywords: {0}\n", sceneInfo.Keywords));
                Writer.WriteLine(string.Format("    Revision: {0}\n", sceneInfo.Revision));
                Writer.WriteLine(string.Format("    Comment: {0}\n", sceneInfo.Comment));

                FbxThumbnail thumbnail = sceneInfo.SceneThumbnail;
                if (thumbnail != null)
                {
                    Writer.WriteLine("    Thumbnail:\n");

                    switch (thumbnail.Data_Format)
                    {
                        case FbxThumbnail.DataFormat.RGBA_32:
                            Writer.WriteLine("        Format: RGB");
                            break;

                        case FbxThumbnail.DataFormat.RGB_24:
                            Writer.WriteLine("        Format: RGBA");
                            break;
                        default:
                            break;

                    }




                    switch (thumbnail.Size)
                    {
                        case FbxThumbnail.ImageSize.NotSet:
                            Writer.WriteLine("        Size: no dimensions specified (%ld bytes)\n" + thumbnail.SizeInBytes);
                            break;
                        case FbxThumbnail.ImageSize.E64x64:
                            Writer.WriteLine("        Size: 64 x 64 pixels (%ld bytes)\n" + thumbnail.SizeInBytes);
                            break;
                        case FbxThumbnail.ImageSize.E128x128:
                            Writer.WriteLine("        Size: 128 x 128 pixels (%ld bytes)\n" + thumbnail.SizeInBytes);
                            break;
                    }
                }
            }
        }

        void DisplayContent(FbxScene Scene)
        {
            int i;
            FbxNode Node = Scene.RootNode;

            if (Node != null)
            {
                for (i = 0; i < Node.GetChildCount(); i++)
                {
                    DisplayContent(Node.GetChild(i));
                }
            }
        }

        void DisplayContent(FbxNode Node)
        {
            FbxNodeAttribute.AttributeType AttributeType;
            int i;

            if (Node.NodeAttribute == null)
            {
                Writer.WriteLine("NULL Node Attribute\n\n");
            }
            else
            {
                AttributeType = Node.NodeAttribute.AttribType;

                switch (AttributeType)
                {
                    case FbxNodeAttribute.AttributeType.Marker:
                        DisplayMarker(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Skeleton:
                        DisplaySkeleton(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Mesh:
                        DisplayMesh(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Nurb:
                        DisplayNurb(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Patch:
                        DisplayPatch(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Camera:
                        DisplayCamera(Node);
                        break;

                    case FbxNodeAttribute.AttributeType.Light:
                        DisplayLight(Node);
                        break;
                }
            }

            DisplayUserProperties(Node);
            DisplayTarget(Node);
            DisplayPivotsAndLimits(Node);
            DisplayTransformPropagation(Node);
            DisplayGeometricTransform(Node);
            DisplayDefaultAnimation(Node);

            for (i = 0; i < Node.GetChildCount(); i++)
            {
                DisplayContent(Node.GetChild(i));
            }
        }

        void DisplayTarget(FbxNode Node)
        {
            if (Node.Target != null)
            {
                Writer.Write("    Target Name: " + Node.Target.Name);
            }
        }


        void DisplayTransformPropagation(FbxNode Node)
        {
            Writer.WriteLine("    Transformation Propagation");

            // 
            // Rotation Space
            //
            FbxRotationOrder RotationOrder = FbxRotationOrder.EulerXYZ;
            Node.GetRotationOrder(FbxNode.PivotSet.SourceSet, ref RotationOrder);

            Writer.Write("        Rotation Space: ");
            Writer.WriteLine(RotationOrder.ToString());






            //
            // Use the Rotation space only for the limits
            // (keep using eEULER_XYZ for the rest)
            //
            Writer.WriteLine("        Use the Rotation Space for Limit specification only: " +
                Node.GetUseRotationSpaceForLimitOnly(FbxNode.PivotSet.SourceSet));


            //
            // Inherit Type
            //
            FbxTransformInheritType InheritType;
            InheritType = Node.TransformationInheritType;

            Writer.Write("        Transformation Inheritance: ");

            Writer.WriteLine(InheritType.ToString());



        }

        void DisplayGeometricTransform(FbxNode Node)
        {
            FbxVector4 TmpVector;

            Writer.WriteLine("    Geometric Transformations");

            //
            // Translation
            //
            TmpVector = Node.GetGeometricTranslation(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Translation:" + TmpVector.ToString());

            //
            // Rotation
            //
            TmpVector = Node.GetGeometricRotation(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Rotation:    " + TmpVector);

            //
            // Scaling
            //
            TmpVector = Node.GetGeometricScaling(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Scaling:     " + TmpVector);
        }




        #endregion

        #region Common
        public bool InitializeSdkObjects(out FbxSdkManager sdkManager, out FbxScene scene)
        {
            // Create the FBX SDK memory manager object.
            // The SDK Manager allocates and frees memory
            // for almost all the classes in the SDK.
            sdkManager = FbxSdkManager.Create();

            if (sdkManager == null)
            {
                Writer.WriteLine("Unable to create the FBX SDK manager");
                sdkManager = null;
                scene = null;
                return false;
            }

            // Create the scene object. The scene will hold the data
            // in the file to be imported.
            scene = FbxScene.Create(sdkManager, "");
            if (scene == null)
            {
                Writer.WriteLine("Unable to create the Scene");
                sdkManager = null;
                scene = null;
                return false;
            }
            return true;
        }

        public void DestroySdkObjects(FbxSdkManager sdkManager)
        {
            // Delete the FBX SDK manager.
            // All the objects that
            // (1) have been allocated by the memory manager, AND that
            // (2) have not been explicitly destroyed
            // will be automatically destroyed.                        
            if (sdkManager != null)
                sdkManager.Destroy();

        }

        public bool LoadScene(FbxSdkManager sdkManager, FbxScene scene, string importFileName, string password)
        {
            Version fileVersion;
            Version sdkVersion;
            int fileFormat = -1;
            int i, takeCount;
            bool status = true;

            // Use memory manager to create an object
            // to store the options for the import of FBX files only.
            Skill.FbxSDK.IO.FbxStreamOptionsFbxReader importOptions =
                Skill.FbxSDK.IO.FbxStreamOptionsFbxReader.Create(sdkManager, "");

            // Get the version number of the FBX files generated by the
            // version of FBX SDK that you are using.
            sdkVersion = Skill.FbxSDK.IO.FbxIO.CurrentVersion;

            // Create an importer.
            Skill.FbxSDK.IO.FbxImporter importer = Skill.FbxSDK.IO.FbxImporter.Create(sdkManager, "");

            //Detect the file format of the file to be imported            
            if (!sdkManager.IOPluginRegistry.DetectFileFormat(importFileName, ref fileFormat))
            {
                // Unrecognizable file format.
                // Try to fall back to SDK's native file format (an FBX binary file).
                fileFormat =
                    sdkManager.IOPluginRegistry.NativeReaderFormat;
            }
            importer.FileFormat = fileFormat;

            // Initialize the importer by providing a filename.
            bool importStatus = importer.Initialize(importFileName);

            // Get the version number of the FBX file format.
            fileVersion = importer.FileVersion;

            if (!importStatus)  // Problem with the file to be imported
            {
                Writer.WriteLine("Call to FbxImporter.Initialize() failed.");
                Writer.WriteLine(string.Format("Error returned: {0}", importer.LastErrorString));

                if (importer.LastErrorID ==
                        Skill.FbxSDK.IO.FbxIO.Error.FileVersionNotSupportedYet ||
                    importer.LastErrorID ==
                        Skill.FbxSDK.IO.FbxIO.Error.FileVersionNotSupportedAnymore)
                {
                    Writer.WriteLine(string.Format("FBX version number for this FBX SDK is {0}.{1}.{2}",
                               sdkVersion.Major, sdkVersion.Minor, sdkVersion.Revision));
                    Writer.WriteLine(string.Format("FBX version number for file {0} is {1}.{2}.{3}",
                               importFileName, fileVersion.Major, fileVersion.Minor, fileVersion.Revision));
                }
                return false;
            }

            Writer.WriteLine(string.Format("FBX version number for this FBX SDK is {0}.{1}.{2}",
                               sdkVersion.Major, sdkVersion.Minor, sdkVersion.Revision));

            if (importer.IsFBX)
            {
                Writer.WriteLine(string.Format("FBX version number for file {0} is {1}.{2}.{3}",
                               importFileName, fileVersion.Major, fileVersion.Minor, fileVersion.Revision));

                // In FBX, a scene can have one or more "takes". A take is a
                // container for animation data.
                // You can access a file's take information without
                // the overhead of loading the entire file into the scene.

                Writer.WriteLine("Take Information");

                takeCount = importer.TakeCount;

                Writer.WriteLine(string.Format("    Number of takes: {0}", takeCount));
                Writer.WriteLine(string.Format("    Current take: \"{0}\"",
                               importer.CurrentTakeName));

                for (i = 0; i < takeCount; i++)
                {
                    FbxTakeInfo takeInfo = importer.GetTakeInfo(i);

                    Writer.WriteLine(string.Format("    Take {0}", i));
                    Writer.WriteLine(string.Format("         Name: \"{0}\"", takeInfo.Name.ToString()));
                    Writer.WriteLine(string.Format("         Description: \"{0}\"",
                                        takeInfo.Description.ToString()));

                    // Change the value of the import name if the take should
                    // be imported under a different name.
                    Writer.WriteLine(string.Format("         Import Name: \"{0}\"", takeInfo.ImportName.ToString()));

                    // Set the value of the import state to false
                    // if the take should be not be imported.
                    Writer.WriteLine(string.Format("         Import State: {0}", takeInfo.Select ? "true" : "false"));
                }

                // Import options determine what kind of data is to be imported.
                // The default is true, but here we set the options explictly.                

                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.MATERIAL, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.TEXTURE, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.LINK, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.SHAPE, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GOBO, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.ANIMATION, true);
                importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.GLOBAL_SETTINGS, true);
            }

            // Import the scene.
            status = importer.Import(scene, importOptions);

            if (status == false &&     // The import file may have a password
               importer.LastErrorID == Skill.FbxSDK.IO.FbxIO.Error.PasswordError)
            {
                Console.Write("Please enter password: ");

                if (password != null)
                {
                    importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.PASSWORD, password);
                    importOptions.SetOption(Skill.FbxSDK.IO.FbxStreamOptionsFbx.PASSWORD_ENABLE, true);
                    status = importer.Import(scene, importOptions);

                    if (status == false && importer.LastErrorID == Skill.FbxSDK.IO.FbxIO.Error.PasswordError)
                    {
                        Console.Write("Incorrect password: file not imported.");
                    }
                }
            }

            // Destroy the importer.
            if (importOptions != null)
            {
                importOptions.Destroy();
                importOptions = null;
            }

            // Destroy the importer
            importer.Destroy();
            return status;
        }


        public bool SaveScene(FbxSdkManager sdkManager, FbxScene scene, string exportFileName, int writeFileFormat, bool embedMedia)
        {
            bool status = true;

            // Create an exporter.
            Skill.FbxSDK.IO.FbxExporter exporter = Skill.FbxSDK.IO.FbxExporter.Create(sdkManager, "");

            // Initialize the exporter by providing a filename.
            if (exporter.Initialize(exportFileName) == false)
            {
                Writer.WriteLine("Call to FbxExporter.Initialize() failed.");
                Writer.WriteLine(string.Format("Error returned: {0}", exporter.LastErrorString));
                return false;
            }

            Version version = Skill.FbxSDK.IO.FbxIO.CurrentVersion;
            Writer.WriteLine(string.Format("FBX version number for this FBX SDK is {0}.{1}.{2}",
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

        #endregion

        #region GlobalSettings

        public void DisplayGlobalLightSettings(FbxGlobalLightSettings GlobalLightSettings)
        {
            int i, Count;

            Writer.WriteLine("Ambient Color: " + GlobalLightSettings.AmbientColor.ToString());
            Writer.WriteLine("Fog Options");
            Writer.WriteLine("    Fog Enable: " + GlobalLightSettings.FogEnable.ToString());



            Writer.WriteLine("    Fog Mode: " + GlobalLightSettings.FogMode.ToString());

            if (GlobalLightSettings.FogMode == FbxGlobalLightSettings.LightFogMode.Linear)
            {
                Writer.WriteLine("    Fog Start: " + GlobalLightSettings.FogStart.ToString());
                Writer.WriteLine("    Fog End: " + GlobalLightSettings.FogEnd.ToString());
            }
            else
            {
                Writer.WriteLine("    Fog Density: " + GlobalLightSettings.FogDensity.ToString());
            }

            Writer.WriteLine("    Fog Color: " + GlobalLightSettings.FogColor.ToString());

            Count = GlobalLightSettings.ShadowPlaneCount;

            if (Count > 0)
            {
                Writer.WriteLine("    Shadow Planes");
                Writer.WriteLine("        Enable: " + GlobalLightSettings.ShadowEnable.ToString());
                Writer.WriteLine("        Intensity: " + GlobalLightSettings.ShadowIntensity.ToString());

                for (i = 0; i < Count; i++)
                {
                    Writer.WriteLine("        Shadow Plane " + i);
                    Writer.WriteLine("            Enable: " + GlobalLightSettings.GetShadowPlane(i).Enable);
                    Writer.WriteLine("            Origin: " + GlobalLightSettings.GetShadowPlane(i).Origin);
                    Writer.WriteLine("            Normal: " + GlobalLightSettings.GetShadowPlane(i).Normal);
                }
            }

            Writer.WriteLine("");
        }


        public void DisplayGlobalCameraSettings(FbxGlobalCameraSettings GlobalCameraSettings)
        {
            Writer.WriteLine("Default Camera: " + GlobalCameraSettings.GetDefaultCamera().ToString());



            Writer.WriteLine("Default Viewing Mode: " + GlobalCameraSettings.DefaultViewingMode.ToString());

            DisplayCamera(GlobalCameraSettings.CameraProducerPerspective, "Producer Perspective", null, null);
            DisplayCamera(GlobalCameraSettings.CameraProducerTop, "Producer Top", null, null);
            DisplayCamera(GlobalCameraSettings.CameraProducerFront, "Producer Front", null, null);
            DisplayCamera(GlobalCameraSettings.CameraProducerRight, "Producer Back", null, null);

            Writer.WriteLine("");
        }


        void DisplayGlobalTimeSettings(FbxGlobalTimeSettings GlobalTimeSettings)
        {
            string TimeString = "";
            int i, Count;


            Writer.WriteLine("Time Mode: " + GlobalTimeSettings.Mode);

            //char* lTimeProtocols[] = { "SMPTE", "Frame", "Default Protocol" };

            Writer.WriteLine("Time Protocol: " + GlobalTimeSettings.Protocol);
            Writer.WriteLine("Snap On Frame: " + GlobalTimeSettings.SnapOnFrame);

            Count = GlobalTimeSettings.TimeMarkerCount;

            if (Count != 0)
            {
                Writer.WriteLine("Time Markers");
                Writer.WriteLine("    Current Time Marker: " + GlobalTimeSettings.CurrentTimeMarker);

                for (i = 0; i < Count; i++)
                {
                    Writer.WriteLine("    Time Marker " + i);
                    Writer.WriteLine("        Name: " + GlobalTimeSettings.GetTimeMarker(i).Name);
                    Writer.WriteLine("        Time: " + GlobalTimeSettings.GetTimeMarker(i).Time.GetTimeString(TimeString, 5, FbxTime.TimeMode.DefaultMode, FbxTime.TimeProtocol.DefaultProtocol, 0.0));
                    Writer.WriteLine("        Loop: " + GlobalTimeSettings.GetTimeMarker(i).Loop);
                }
            }

            Writer.WriteLine("");
        }

        #endregion

        #region Hierarchy

        void DisplayHierarchy(FbxScene Scene)
        {
            int i;
            FbxNode RootNode = Scene.RootNode;

            for (i = 0; i < RootNode.GetChildCount(); i++)
            {
                DisplayHierarchy(RootNode.GetChild(i), 0);
            }
        }

        void DisplayHierarchy(FbxNode Node, int Depth)
        {
            String Str = string.Empty;
            int i;

            for (i = 0; i < Depth; i++)
            {
                Str += "     ";
            }

            Str += Node.Name;
            //   Str += "\n";

            Writer.WriteLine(Str);

            for (i = 0; i < Node.GetChildCount(); i++)
            {
                DisplayHierarchy(Node.GetChild(i), Depth + 1);
            }
        }
        #endregion

        #region Marker


        public void DisplayMarker(FbxNode Node)
        {
            FbxMarker Marker = (FbxMarker)Node.NodeAttribute;
            string Str;

            Writer.Write("Marker Name: " + Node.Name);
            DisplayMetaDataConnections(Marker);

            // Type
            Str = "    Marker Type: ";
            Str += Marker.Type.ToString();

            Writer.Write(Str);

            // Look
            Str = "    Marker Look: ";
            Str += Marker.MarkerLook.ToString();

            Writer.Write(Str);

            // Size
            Str = "    Size: " + Marker.Size.ToString();
            Writer.Write(Str);

            // Color
            FbxDouble3 c = Marker.Color.Get();
            FbxColor color = new FbxColor(c.X, c.Y, c.Z);
            Writer.WriteLine("    Color: " + color.ToString());

            // IKPivot
            Writer.WriteLine("    IKPivot: " + Marker.IKPivot.Get().ToString());
        }



        #endregion

        #region DisplaySkeleton
        void DisplaySkeleton(FbxNode Node)
        {
            FbxSkeleton Skeleton = (FbxSkeleton)Node.NodeAttribute;

            Writer.WriteLine("Skeleton Name: " + Node.Name);
            DisplayMetaDataConnections(Skeleton);

            Writer.WriteLine("    Type: " + Skeleton.Skeleton_Type);

            if (Skeleton.Skeleton_Type == FbxSkeleton.SkeletonType.Limb)
            {
                Writer.WriteLine("    Limb Length: " + Skeleton.LimbLength);
            }
            else if (Skeleton.Skeleton_Type == FbxSkeleton.SkeletonType.LimbNode)
            {
                Writer.WriteLine("    Limb Node Size: " + Skeleton.Size);
            }
            else if (Skeleton.Skeleton_Type == FbxSkeleton.SkeletonType.Root)
            {
                Writer.WriteLine("    Limb Root Size: " + Skeleton.Size);
            }

            Writer.WriteLine("    Color: " + Skeleton.GetLimbNodeColor().ToString());
        }



        #endregion

        #region DisplayMesh

        void DisplayMesh(FbxNode Node)
        {
            FbxMesh Mesh = (FbxMesh)Node.NodeAttribute;



            Writer.Write("Mesh Name: " + Node.Name);
            DisplayMetaDataConnections(Mesh);
            DisplayControlsPoints(Mesh);
            DisplayPolygons(Mesh);
            DisplayMaterialMapping(Mesh);
            DisplayMaterial(Mesh);
            DisplayTexture(Mesh);
            DisplayLink(Mesh);
            DisplayShape(Mesh);
        }



        void DisplayControlsPoints(FbxMesh Mesh)
        {
            int i, ControlPointsCount = Mesh.ControlPointsCount;
            FbxVector4[] ControlPoints = Mesh.ControlPoints;

            Writer.WriteLine("    Control Points:" + ControlPointsCount);

            for (i = 0; i < ControlPointsCount; i++)
            {
                Writer.WriteLine("        Control Point " + i);
                Writer.WriteLine("            Coordinates: " + ControlPoints[i].ToString());

                for (int j = 0; j < Mesh.LayerCount; j++)
                {
                    FbxLayerElementNormal eNormals = Mesh.GetLayer(j).Normals;
                    if (eNormals != null)
                    {
                        if (eNormals.Mapping_Mode == FbxLayerElement.MappingMode.ByControlPoint)
                        {
                            string header;
                            header = string.Format("            Normal Vector (on layer {0}): ", j);
                            if (eNormals.Reference_Mode == FbxLayerElement.ReferenceMode.Direct)
                                Writer.WriteLine(header + eNormals.DirectArray.GetAt(i));
                        }
                    }
                }
            }

            Writer.WriteLine("");
        }


        void DisplayPolygons(FbxMesh Mesh)
        {
            int i, j, PolygonCount = Mesh.PolygonCount;
            FbxVector4[] ControlPoints = Mesh.ControlPoints;
            string header;

            Writer.WriteLine("    Polygons");

            int vertexId = 0;
            for (i = 0; i < PolygonCount; i++)
            {
                Writer.WriteLine("        Polygon " + i);
                int l;

                for (l = 0; l < Mesh.LayerCount; l++)
                {
                    FbxLayerElementPolygonGroup lePolgrp = Mesh.GetLayer(l).PolygonGroups;
                    if (lePolgrp != null)
                    {
                        switch (lePolgrp.Mapping_Mode)
                        {
                            case FbxLayerElement.MappingMode.ByPolygon:
                                if (lePolgrp.Reference_Mode == FbxLayerElement.ReferenceMode.Index)
                                {
                                    header = string.Format("        Assigned to group (on layer {0}): ", l);
                                    int polyGroupId = lePolgrp.IndexArray.GetAt(i);
                                    Writer.WriteLine(header + polyGroupId);

                                }
                                break;
                            default:
                                // any other mapping modes don't make sense
                                Writer.WriteLine("        \"unsupported group assignment\"");
                                break;
                        }
                    }
                }

                int PolygonSize = Mesh.GetPolygonSize(i);

                for (j = 0; j < PolygonSize; j++)
                {
                    int ControlPointIndex = Mesh.GetPolygonVertex(i, j);

                    Writer.Write("            Coordinates: " + ControlPoints[ControlPointIndex]);

                    for (l = 0; l < Mesh.LayerCount; l++)
                    {
                        FbxLayerElementVertexColor leVtxc = Mesh.GetLayer(l).VertexColors;
                        if (leVtxc != null)
                        {
                            header = string.Format("            Color vertex (on layer {0}): ", l);

                            switch (leVtxc.Mapping_Mode)
                            {
                                case FbxLayerElement.MappingMode.ByControlPoint:
                                    switch (leVtxc.Reference_Mode)
                                    {
                                        case FbxLayerElement.ReferenceMode.Direct:
                                            Writer.Write(header + leVtxc.DirectArray.GetAt(ControlPointIndex));
                                            break;
                                        case FbxLayerElement.ReferenceMode.IndexToDirect:
                                            {
                                                int id = leVtxc.IndexArray.GetAt(ControlPointIndex);
                                                Writer.Write(header + leVtxc.DirectArray.GetAt(id));
                                            }
                                            break;
                                    }
                                    break;

                                case FbxLayerElement.MappingMode.ByPolygonVertex:
                                    {
                                        switch (leVtxc.Reference_Mode)
                                        {
                                            case FbxLayerElement.ReferenceMode.Direct:
                                                Writer.Write(header + leVtxc.DirectArray.GetAt(vertexId));
                                                break;
                                            case FbxLayerElement.ReferenceMode.IndexToDirect:
                                                {
                                                    int id = leVtxc.IndexArray.GetAt(vertexId);
                                                    Writer.Write(header + leVtxc.DirectArray.GetAt(id));
                                                }
                                                break;
                                            default:
                                                break; // other reference modes not shown here!
                                        }
                                    }
                                    break;

                                case FbxLayerElement.MappingMode.ByPolygon: // doesn't make much sense for UVs
                                case FbxLayerElement.MappingMode.AllSame:   // doesn't make much sense for UVs
                                case FbxLayerElement.MappingMode.None:       // doesn't make much sense for UVs
                                    break;
                            }
                        }

                        FbxLayerElementUV leUV = Mesh.GetLayer(l).GetUVs();
                        if (leUV != null)
                        {
                            header = string.Format("            Texture UV (on layer {0}): ", l);

                            switch (leUV.Mapping_Mode)
                            {
                                case FbxLayerElement.MappingMode.ByControlPoint:
                                    switch (leUV.Reference_Mode)
                                    {
                                        case FbxLayerElement.ReferenceMode.Direct:
                                            Writer.Write(header + leUV.DirectArray.GetAt(ControlPointIndex));
                                            break;
                                        case FbxLayerElement.ReferenceMode.IndexToDirect:
                                            {
                                                int id = leUV.IndexArray.GetAt(ControlPointIndex);
                                                Writer.Write(header + leUV.DirectArray.GetAt(id));
                                            }
                                            break;
                                        default:
                                            break; // other reference modes not shown here!
                                    }
                                    break;

                                case FbxLayerElement.MappingMode.ByPolygonVertex:
                                    {
                                        int TextureUVIndex = Mesh.GetTextureUVIndex(i, j);
                                        switch (leUV.Reference_Mode)
                                        {
                                            case FbxLayerElement.ReferenceMode.Direct:
                                            case FbxLayerElement.ReferenceMode.IndexToDirect:
                                                {
                                                    Writer.Write(header + leUV.DirectArray.GetAt(TextureUVIndex).ToString());
                                                }
                                                break;
                                            default:
                                                break; // other reference modes not shown here!
                                        }
                                    }
                                    break;

                                case FbxLayerElement.MappingMode.ByPolygon: // doesn't make much sense for UVs
                                case FbxLayerElement.MappingMode.AllSame:   // doesn't make much sense for UVs
                                case FbxLayerElement.MappingMode.None:       // doesn't make much sense for UVs
                                    break;
                            }
                        }
                    } // for layer
                    vertexId++;
                } // for polygonSize




            } // for polygonCount


            //check visibility for the edges of the mesh
            for (int l = 0; l < Mesh.LayerCount; ++l)
            {
                FbxLayerElementVisibility leVisibility = Mesh.GetLayer(0).Visibility;
                if (leVisibility != null)
                {
                    header = string.Format("    Edge Visibilty (on layer {0}): ", l);
                    Writer.WriteLine(header);
                    switch (leVisibility.Mapping_Mode)
                    {
                        //should be eBY_EDGE
                        case FbxLayerElement.MappingMode.ByEdge:
                            //should be eDIRECT
                            for (j = 0; j != Mesh.MeshEdgeCount; ++j)
                            {
                                Writer.WriteLine("        Edge " + j);
                                Writer.WriteLine("              Edge visibilty: " + leVisibility.DirectArray.GetAt(j));
                            }

                            break;
                    }

                }
            }
            Writer.Write("");
        }


        void DisplayMaterialMapping(FbxMesh Mesh)
        {


            int MtrlCount = 0;
            FbxNode Node = null;
            if (Mesh != null)
            {
                Node = Mesh.Node;
                if (Node != null)
                    MtrlCount = Node.MaterialCount;
            }

            for (int l = 0; l < Mesh.LayerCount; l++)
            {
                FbxLayerElementMaterial leMat = Mesh.GetLayer(l).Materials;
                if (leMat != null)
                {
                    string header;
                    header = string.Format("    Material layer {0}: ", l);
                    Writer.Write(header);


                    Writer.Write("           Mapping: " + leMat.Mapping_Mode);
                    Writer.Write("           ReferenceMode: " + leMat.Reference_Mode);

                    int MaterialCount = 0;
                    String Str;

                    if (leMat.Reference_Mode == FbxLayerElement.ReferenceMode.Direct ||
                        leMat.Reference_Mode == FbxLayerElement.ReferenceMode.IndexToDirect)
                    {
                        MaterialCount = MtrlCount;
                    }

                    if (leMat.Reference_Mode == FbxLayerElement.ReferenceMode.Index ||
                        leMat.Reference_Mode == FbxLayerElement.ReferenceMode.IndexToDirect)
                    {
                        int i;

                        Str = "           Indices: ";

                        int IndexArrayCount = leMat.IndexArray.Count;
                        for (i = 0; i < IndexArrayCount; i++)
                        {
                            Str += leMat.IndexArray.GetAt(i);

                            if (i < IndexArrayCount - 1)
                            {
                                Str += ", ";
                            }
                        }



                        Writer.WriteLine(Str);
                    }
                }
            }

            Writer.WriteLine("");
        }

        #endregion

        #region DisplayMaterial
        void DisplayMaterial(FbxGeometry Geometry)
        {
            int MaterialCount = 0;
            FbxNode Node = null;
            if (Geometry != null)
            {
                Node = Geometry.Node;
                if (Node != null)
                    MaterialCount = Node.MaterialCount;
            }

            for (int l = 0; l < Geometry.LayerCount; l++)
            {
                FbxLayerElementMaterial leMat = Geometry.GetLayer(l).Materials;
                if (leMat != null)
                {
                    if (leMat.Reference_Mode == FbxLayerElement.ReferenceMode.Index)
                        // Materials are in an undefined external table
                        continue;

                    if (MaterialCount > 0)
                    {
                        string header;
                        FbxDouble3 Double3;
                        double Double1;
                        FbxColor theColor = new FbxColor();

                        header = "    Materials on layer %d: " + l;
                        Writer.Write(header);

                        for (int Count = 0; Count < MaterialCount; Count++)
                        {
                            Writer.Write("        Material " + Count);

                            FbxSurfaceMaterial Material = Node.GetMaterial(Count);

                            Writer.Write("            Name: \"" + Material.Name + "\"");
                            //FbxSurfaceLambert Lambert = null;/////////// new FbxSurfaceLambert();
                            //FbxSurfacePhong Phong = null;/////////// new FbxSurfacePhong();
                            if (Material.GetClassId().Is(FbxSurfaceLambert.ClassId))
                            {
                                // We found a Lambert material. Display its properties.
                                // Display the Ambient Color
                                Double3 = ((FbxSurfaceLambert)Material).AmbientColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Ambient: " + theColor);

                                // Display the Diffuse Color
                                Double3 = ((FbxSurfaceLambert)Material).DiffuseColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Diffuse: " + theColor);

                                // Display the Emissive
                                Double3 = ((FbxSurfaceLambert)Material).EmissiveColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Emissive: " + theColor);

                                // Display the Opacity
                                Double1 = ((FbxSurfaceLambert)Material).TransparencyFactor;
                                Writer.WriteLine("            Opacity: " + (1.0 - Double1));
                            }

                            else if (Material.GetClassId().Is(FbxSurfacePhong.ClassId))
                            {
                                // We found a Phong material.  Display its properties.

                                // Display the Ambient Color
                                Double3 = ((FbxSurfacePhong)Material).AmbientColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Ambient: " + theColor);

                                // Display the Diffuse Color
                                Double3 = ((FbxSurfacePhong)Material).DiffuseColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Diffuse: " + theColor);

                                // Display the Specular Color (unique to Phong materials)
                                Double3 = ((FbxSurfacePhong)Material).SpecularColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Specular: " + theColor);

                                // Display the Emissive Color
                                Double3 = ((FbxSurfacePhong)Material).EmissiveColor;
                                theColor.Set(Double3.X, Double3.Y, Double3.Z);
                                Writer.WriteLine("            Emissive: " + theColor);

                                //Opacity is Transparency factor now
                                Double1 = ((FbxSurfacePhong)Material).TransparencyFactor;
                                Writer.WriteLine("            Opacity: " + (1.0 - Double1));

                                // Display the Shininess
                                Double1 = ((FbxSurfacePhong)Material).Shininess;
                                Writer.WriteLine("            Shininess: " + Double1);

                                // Display the Reflectivity
                                Double1 = ((FbxSurfacePhong)Material).ReflectionFactor;
                                Writer.WriteLine("            Reflectivity: " + Double1);
                            }
                            else
                                Writer.WriteLine("Unkown type of Material");

                            string Str;
                            Str = Material.ShadingModel;
                            Writer.WriteLine("            Shading Model: " + Str);
                            Writer.WriteLine("");
                        }
                    }
                }
            }
        }
        #endregion

        #region DisplayTexture
        void DisplayTextureInfo(FbxTexture Texture, int BlendMode)
        {


            Writer.WriteLine("            Name: \"" + Texture.Name + "\"");
            Writer.WriteLine("            File Name: \"" + Texture.FileName + "\"");
            Writer.WriteLine("            Scale U: " + Texture.ScaleU);
            Writer.WriteLine("            Scale V: " + Texture.ScaleV);
            Writer.WriteLine("            Translation U: " + Texture.TranslationU);
            Writer.WriteLine("            Translation V: " + Texture.TranslationV);
            Writer.WriteLine("            Swap UV: " + Texture.SwapUV);
            Writer.WriteLine("            Rotation U: " + Texture.RotationU);
            Writer.WriteLine("            Rotation V: " + Texture.RotationV);
            Writer.WriteLine("            Rotation W: " + Texture.RotationW);



            Writer.WriteLine("            Alpha Source: " + Texture.AlphaSrc);
            Writer.WriteLine("            Cropping Left: " + Texture.CroppingLeft);
            Writer.WriteLine("            Cropping Top: " + Texture.CroppingTop);
            Writer.WriteLine("            Cropping Right: " + Texture.CroppingRight);
            Writer.WriteLine("            Cropping Bottom: " + Texture.CroppingBottom);


            Writer.WriteLine("            Mapping Type: " + Texture.Mapping);

            if (Texture.Mapping == FbxTexture.MappingType.Planar)
            {

                Writer.WriteLine("            Planar Mapping Normal: " + Texture.PlanarMappingNormalType);
            }


            if (BlendMode >= 0)
                Writer.Write("            Blend Mode: " + BlendMode);
            Writer.WriteLine("            Alpha: " + Texture.DefaultAlpha);



            Writer.WriteLine("            Material Use: " + Texture.MaterialUseType);



            Writer.WriteLine("            Texture Use: " + Texture.TextureUseType);
            Writer.WriteLine("");

        }

        void FindAndDisplayTextureInfoByProperty(FbxProperty Property, ref bool DisplayHeader, int MaterialIndex)
        {



            if (Property.IsValid)
            {

                FbxTexture Texture = null;

                //Here we have to check if it's layeredtextures, or just textures:
                int LayeredTextureCount = Property.GetSrcObjectCount(FbxLayeredTexture.ClassId);
                if (LayeredTextureCount > 0)
                {
                    for (int j = 0; j < LayeredTextureCount; ++j)
                    {
                        Writer.WriteLine("    Layered Texture: " + j);
                        FbxLayeredTexture LayeredTexture = (FbxLayeredTexture)Property.GetSrcObject(FbxLayeredTexture.ClassId, j);
                        int NbTextures = LayeredTexture.GetSrcObjectCount(FbxTexture.ClassId);
                        for (int k = 0; k < NbTextures; ++k)
                        {
                            Texture = (FbxTexture)LayeredTexture.GetSrcObject(FbxTexture.ClassId, j);
                            if (Texture != null)
                            {

                                if (DisplayHeader)
                                {
                                    Writer.WriteLine("    Textures connected to Material " + MaterialIndex);
                                    DisplayHeader = false;
                                }

                                //NOTE the blend mode is ALWAYS on the LayeredTexture and NOT the one on the texture.
                                //Why is that?  because one texture can be shared on different layered textures and might
                                //have different blend modes.

                                FbxLayeredTexture.BlendMode Blend_Mode;
                                Blend_Mode = LayeredTexture[k];
                                Writer.WriteLine("    Textures for " + Property.Name);
                                Writer.WriteLine("        Texture " + k);
                                DisplayTextureInfo(Texture, (int)Blend_Mode);
                            }

                        }
                    }
                }
                else
                {
                    //no layered texture simply get on the property
                    int NbTextures = Property.GetSrcObjectCount(FbxTexture.ClassId);
                    for (int j = 0; j < NbTextures; ++j)
                    {

                        Texture = (FbxTexture)Property.GetSrcObject(FbxTexture.ClassId, j);
                        if (Texture != null)
                        {
                            //display connectMareial header only at the first time
                            if (DisplayHeader)
                            {
                                Writer.WriteLine("    Textures connected to Material " + MaterialIndex);
                                DisplayHeader = false;
                            }

                            Writer.WriteLine("    Textures for " + Property.Name);
                            Writer.WriteLine("        Texture " + j);
                            DisplayTextureInfo(Texture, -1);
                        }
                    }
                }


                int NbTex = Property.GetSrcObjectCount(FbxTexture.ClassId);
                for (int TextureIndex = 0; TextureIndex < NbTex; TextureIndex++)
                {
                    Texture = (FbxTexture)Property.GetSrcObject(FbxTexture.ClassId, TextureIndex);


                }//end for 
            }//end if pProperty

        }




        void DisplayTexture(FbxGeometry Geometry)
        {


            Writer.WriteLine(" DisplayTexture Not implement");
            int MaterialIndex;
            int TextureIndex;
            FbxProperty Property;
            int NbMat = Geometry.Node.GetSrcObjectCount(FbxSurfaceMaterial.ClassId);
            for (MaterialIndex = 0; MaterialIndex < NbMat; MaterialIndex++)
            {
                FbxSurfaceMaterial Material = (FbxSurfaceMaterial)Geometry.Node.GetSrcObject(FbxSurfaceMaterial.ClassId, MaterialIndex);
                int NbTex;
                FbxTexture Texture = null;
                bool DisplayHeader = true;

                //go through all the possible textures
                if (Material != null)
                {
                    Writer.WriteLine("    Textures connected to Material " + MaterialIndex);

                    //Diffuse Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SDiffuse);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //DiffuseFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SDiffuseFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Emissive Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SEmissive);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //EmissiveFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SEmissiveFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Ambient Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SAmbient);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //AmbientFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SAmbientFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Specular Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SSpecular);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //SpecularFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SSpecularFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Shininess Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SShininess);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Bump Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SBump);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Normal Map Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SNormalMap);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Transparent Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.STransparentColor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //TransparencyFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.STransparencyFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //Reflection Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SReflection);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                    //ReflectionFactor Textures
                    Property = Material.FindProperty(FbxSurfaceMaterial.SReflectionFactor);
                    FindAndDisplayTextureInfoByProperty(Property, ref DisplayHeader, MaterialIndex);

                }//end if(lMaterial)

            }// end for MaterialIndex     
        }


        #endregion

        #region DisplayLink
        void DisplayLink(FbxGeometry Geometry)
        {
            //Display cluster now

            //int i, lLinkCount;
            //KFbxLink* lLink;

            int i, j;
            int SkinCount = 0;
            int ClusterCount = 0;
            FbxCluster Cluster;

            SkinCount = Geometry.GetDeformerCount(FbxDeformer.DeformerType.Skin);



            //lLinkCount = pGeometry.GetLinkCount();
            for (i = 0; i != SkinCount; ++i)
            {
                ClusterCount = ((FbxSkin)Geometry.GetDeformer(i, FbxDeformer.DeformerType.Skin)).ClusterCount;
                for (j = 0; j != ClusterCount; ++j)
                {
                    Writer.WriteLine("    Cluster " + i);

                    Cluster = ((FbxSkin)Geometry.GetDeformer(i, FbxDeformer.DeformerType.Skin)).GetCluster(j);
                    //lLink = pGeometry.GetLink(i);    



                    Writer.WriteLine("    Mode: " + Cluster.Mode.ToString());

                    if (Cluster.Link != null)
                    {
                        Writer.WriteLine("        Name: " + Cluster.Link.Name);
                    }

                    String String1 = "        Link Indices: ";
                    String String2 = "        Weight Values: ";

                    int k, IndexCount = Cluster.ControlPointIndicesCount;
                    int[] Indices = Cluster.GetControlPointIndicesArray();
                    double[] Weights = Cluster.GetControlPointWeightsArray();

                    for (k = 0; k < IndexCount; k++)
                    {
                        String1 += Indices[k];
                        String2 += (float)Weights[k];

                        if (k < IndexCount - 1)
                        {
                            String1 += ", ";
                            String2 += ", ";
                        }
                    }

                    String1 += "\n";
                    String2 += "\n";

                    Writer.WriteLine(String1);
                    Writer.WriteLine(String2);

                    Writer.WriteLine("");

                    FbxXMatrix Matrix;

                    Matrix = Cluster.TransformMatrix;
                    Writer.WriteLine("        Transform Translation: " + Matrix.T);
                    Writer.WriteLine("        Transform Rotation: " + Matrix.R);
                    Writer.WriteLine("        Transform Scaling: " + Matrix.S);

                    Matrix = Cluster.TransformLinkMatrix;
                    Writer.WriteLine("        Transform Link Translation: " + Matrix.T);
                    Writer.WriteLine("        Transform Link Rotation: " + Matrix.R);
                    Writer.WriteLine("        Transform Link Scaling: " + Matrix.S);

                    if (Cluster.AssociateModel != null)
                    {
                        Matrix = Cluster.TransformAssociateModelMatrix;
                        Writer.WriteLine("        Associate Model: " + Cluster.AssociateModel.Name);
                        Writer.WriteLine("        Associate Model Translation: " + Matrix.T);
                        Writer.WriteLine("        Associate Model Rotation: " + Matrix.R);
                        Writer.WriteLine("        Associate Model Scaling: " + Matrix.S);
                    }

                    Writer.WriteLine("");
                }
            }
        }



        #endregion

        #region DisplayShape
        void DisplayShape(FbxGeometry Geometry)
        {
            int i, ShapeCount;
            FbxShape Shape;

            ShapeCount = Geometry.ShapeCount;

            for (i = 0; i < ShapeCount; i++)
            {
                Writer.WriteLine("    Shape " + Geometry.GetShapeName(i));
                Writer.WriteLine("        Default Animation Value: " + Geometry.GetDefaultShape(i).ToString());

                Shape = Geometry.GetShape(i);

                int j, ControlPointsCount = Shape.ControlPointsCount;
                FbxVector4[] ControlPoints = Shape.ControlPoints;
                FbxLayerElementArrayTemplateVector4 Normals = null;

                Normals = Shape.Normals;

                for (j = 0; j < ControlPointsCount; j++)
                {
                    Writer.WriteLine("        Control Point " + j);
                    Writer.WriteLine("            Coordinates: " + ControlPoints[j]);

                    if (Normals != null && Normals.Count == ControlPointsCount)
                    {
                        Writer.WriteLine("            Normal Vector: " + Normals.GetAt(j));
                    }
                }

                Writer.WriteLine("");
            }
        }



        #endregion

        #region DisplayPatch
        void DisplayPatch(FbxNode Node)
        {
            FbxPatch Patch = (FbxPatch)Node.NodeAttribute;

            Writer.WriteLine("Patch Name: " + Node.Name);
            DisplayMetaDataConnections(Patch);



            Writer.WriteLine("    Surface Mode: " + Patch.Surface_Mode);

            int i, ControlPointsCount = Patch.ControlPointsCount;
            FbxVector4[] ControlPoints = Patch.ControlPoints;

            for (i = 0; i < ControlPointsCount; i++)
            {
                Writer.WriteLine("    Control Point " + i);
                Writer.WriteLine("        Coordinates: " + ControlPoints[i]);
                Writer.WriteLine("        Weight: " + ControlPoints[i][3]);
            }



            Writer.WriteLine("    Patch U Type: " + Patch.PatchUType);
            Writer.WriteLine("    U Count: " + Patch.UCount);
            Writer.WriteLine("    Patch V Type: " + Patch.PatchVType);
            Writer.WriteLine("    V Count: " + Patch.VCount);
            Writer.WriteLine("    U Step: " + Patch.UStep);
            Writer.WriteLine("    V Step: " + Patch.VStep);
            Writer.WriteLine("    U Closed: " + Patch.UClosed);
            Writer.WriteLine("    V Closed: " + Patch.VClosed);
            Writer.WriteLine("    U Capped Top: " + Patch.UCappedTop);
            Writer.WriteLine("    U Capped Bottom: " + Patch.UCappedBottom);
            Writer.WriteLine("    V Capped Top: " + Patch.VCappedTop);
            Writer.WriteLine("    V Capped Bottom: " + Patch.VCappedBottom);

            Writer.WriteLine("");

            DisplayTexture(Patch);
            DisplayMaterial(Patch);
            DisplayLink(Patch);
            DisplayShape(Patch);
        }



        #endregion

        #region DisplayCamera
        void DisplayCamera(FbxNode Node)
        {
            DisplayCamera((FbxCamera)Node.NodeAttribute, Node.Name, Node.Target, Node.TargetUp);
        }


        void DisplayCamera(FbxCamera Camera, string Name, FbxNode TargetNode, FbxNode TargetUNode)
        {
            Writer.WriteLine("Camera Name: " + Name);
            if (Camera == null || !Camera.IsValid)
            {
                Writer.WriteLine("NOT FOUND");
                return;
            }
            DisplayMetaDataConnections(Camera);

            DisplayCameraPositionAndOrientation(Camera, TargetNode, TargetUNode);



            Writer.WriteLine("    Projection Type: " + Camera.ProjectionType.ToString());


            DisplayViewingAreaControls(Camera);

            // If camera projection type is set to KFbxCamera::eORTHOGONAL, the 
            // aperture and film controls are not relevant.
            if (Camera.ProjectionType != FbxCamera.CameraProjectionType.Orthogonal)
            {
                DisplayApertureAndFilmControls(Camera);
            }

            DisplayBackgroundProperties(Camera);
            DisplayCameraViewOptions(Camera);
            DisplayRenderOptions(Camera);
            DisplayDefaultAnimationValues(Camera);
        }


        void DisplayCameraPositionAndOrientation(FbxCamera Camera, FbxNode TargetNode, FbxNode TargetUNode)
        {
            Writer.WriteLine("    Camera Position and Orientation");
            Writer.WriteLine("        Position: " + Camera.Position.Get());

            if (TargetNode != null)
            {
                Writer.WriteLine("        Camera Interest: " + TargetNode.Name);
            }
            else
            {
                Writer.WriteLine("        Default Camera Interest Position: " + Camera.InterestPosition);
            }

            if (TargetUNode != null)
            {
                Writer.WriteLine("        Camera Up Target: " + TargetUNode.Name);
            }
            else
            {
                Writer.WriteLine("        Up Vector: " + Camera.UpVector.Get());
            }

            Writer.WriteLine("        Roll: " + Camera.Roll.Get());
        }


        void DisplayViewingAreaControls(FbxCamera Camera)
        {
            Writer.WriteLine("    Viewing Area Controls");



            Writer.WriteLine("        Format: " + Camera.Format);



            Writer.WriteLine("        Aspect Ratio Mode: " + Camera.AspectRatioMode);

            // If the ratio mode is eWINDOW_SIZE, both width and height values aren't relevant.
            if (Camera.AspectRatioMode != FbxCamera.CameraAspectRatioMode.WindowSize)
            {
                Writer.WriteLine("        Aspect Width: " + Camera.AspectWidth.Get());
                Writer.WriteLine("        Aspect Height: " + Camera.AspectHeight.Get());
            }

            Writer.WriteLine("        Pixel Ratio: " + Camera.PixelAspectRatio.Get());
            Writer.WriteLine("        Near Plane: " + Camera.NearPlane);
            Writer.WriteLine("        Far Plane: " + Camera.FarPlane);
            Writer.WriteLine("        Mouse Lock: " + Camera.LockMode);
        }


        void DisplayApertureAndFilmControls(FbxCamera Camera)
        {
            Writer.WriteLine("    Aperture and Film Controls");



            Writer.WriteLine("        Aperture Format: " + Camera.ApertureFormat);



            Writer.WriteLine("        Aperture Mode: " + Camera.ApertureMode);

            Writer.WriteLine("        Aperture Width: " + Camera.ApertureWidth + " inches");
            Writer.WriteLine("        Aperture Height: " + Camera.ApertureHeight + " inches");
            Writer.WriteLine("        Squeeze Ratio: " + Camera.SqueezeRatio);
            Writer.WriteLine("        Focal Length: " + Camera.FocalLength.Get().ToString() + " mm");
            Writer.WriteLine("        Field of View: " + Camera.FieldOfView.Get().ToString() + " degrees");
        }


        void DisplayBackgroundProperties(FbxCamera Camera)
        {
            Writer.WriteLine("    Background Properties");

            Writer.WriteLine("        Background File Name: \"" + Camera.BackgroundFileName + "\"");



            Writer.WriteLine("        Background Display Mode: " + Camera.ViewFrustumBackPlaneMode);



            Writer.WriteLine("        Background Drawing Mode: " + Camera.BackgroundMode);
            Writer.WriteLine("        Foreground Matte Threshold Enable: " + Camera.ShowFrontPlate);

            // This option is only relevant if background drawing mode is set to eFOREGROUND or eBACKGROUND_AND_FOREGROUND.
            if (Camera.BackgroundMode != FbxCamera.CameraBackgroundDrawingMode.Background &&
            Camera.ForegroundAlpha != null)
            {
                Writer.WriteLine("        Foreground Matte Threshold: " + Camera.BackgroundAlphaTreshold);
            }

            String BackgroundPlacementOptions = "";

            if (((FbxCamera.CameraBackgroundPlacementOptions)Camera.BackgroundPlacementOptions & FbxCamera.CameraBackgroundPlacementOptions.Fit) != 0)
            {
                BackgroundPlacementOptions += " Fit,";
            }
            if (((FbxCamera.CameraBackgroundPlacementOptions)Camera.BackgroundPlacementOptions & FbxCamera.CameraBackgroundPlacementOptions.Center) != 0)
            {
                BackgroundPlacementOptions += " Center,";
            }
            if (((FbxCamera.CameraBackgroundPlacementOptions)Camera.BackgroundPlacementOptions & FbxCamera.CameraBackgroundPlacementOptions.KeepRatio) != 0)
            {
                BackgroundPlacementOptions += " Keep Ratio,";
            }
            if (((FbxCamera.CameraBackgroundPlacementOptions)Camera.BackgroundPlacementOptions & FbxCamera.CameraBackgroundPlacementOptions.Crop) != 0)
            {
                BackgroundPlacementOptions += " Crop,";
            }
            if (BackgroundPlacementOptions != string.Empty)
            {
                String Str = BackgroundPlacementOptions.Substring(0, BackgroundPlacementOptions.Length - 1);
                Writer.WriteLine("        Background Placement Options: " + Str);
            }

            Writer.WriteLine("        Background Distance: " + Camera.BackPlaneDistance.Get().ToString());



            Writer.WriteLine("        Background Distance Mode: " + Camera.BackPlaneDistanceMode);
        }


        void DisplayCameraViewOptions(FbxCamera Camera)
        {
            Writer.WriteLine("    Camera View Options");

            Writer.WriteLine("        View Camera Interest: " + Camera.ViewCameraToLookAt);
            Writer.WriteLine("        View Near Far Planes: " + Camera.ViewFrustumNearFarPlane);
            Writer.WriteLine("        Show Grid: " + Camera.ShowGrid);
            Writer.WriteLine("        Show Axis: " + Camera.ShowAzimut);
            Writer.WriteLine("        Show Name: " + Camera.ShowName);
            Writer.WriteLine("        Show Info on Moving: " + Camera.ShowInfoOnMoving);
            Writer.WriteLine("        Show Time Code: " + Camera.ShowTimeCode);
            Writer.WriteLine("        Display Safe Area: " + Camera.DisplaySafeArea);


            FbxColor color;
            FbxDouble3 c;

            Writer.WriteLine("        Safe Area Style: " + Camera.SafeAreaDisplayStyle);
            Writer.WriteLine("        Show Audio: " + Camera.ShowAudio);

            c = Camera.BackgroundColor.Get();
            color = new FbxColor(c.X, c.Y, c.Z);
            Writer.WriteLine("        Background Color: " + color.ToString());

            c = Camera.AudioColor.Get();
            color = new FbxColor(c.X, c.Y, c.Z);
            Writer.WriteLine("        Audio Color: " + color);

            Writer.WriteLine("        Use Frame Color: " + Camera.UseFrameColor);

            c = Camera.FrameColor.Get();
            color = new FbxColor(c.X, c.Y, c.Z);
            Writer.WriteLine("        Frame Color: " + color);
        }


        void DisplayRenderOptions(FbxCamera Camera)
        {
            Writer.WriteLine("    Render Options");



            Writer.WriteLine("        Render Options Usage Time: " + Camera.UseRealTimeDOFAndAA);
            Writer.WriteLine("        Use Antialiasing: " + Camera.UseAntialiasing);
            Writer.WriteLine("        Antialiasing Intensity: " + Camera.AntialiasingIntensity.Get());



            Writer.WriteLine("        Antialiasing Method: " + Camera.AntialiasingMethod);

            // This option is only relevant if antialiasing method is set to eOVERSAMPLING_ANTIALIASING.
            if (Camera.AntialiasingMethod == FbxCamera.CameraAntialiasingMethod.OversamplingAntialiasing)
            {
                Writer.WriteLine("        Number of Samples: " + Camera.FrameSamplingCount);
            }



            Writer.WriteLine("        Sampling Type: " + Camera.FrameSamplingType);
            Writer.WriteLine("        Use Accumulation Buffer: " + Camera.UseAccumulationBuffer);
            Writer.WriteLine("        Use Depth of Field: " + Camera.UseDepthOfField);



            Writer.WriteLine("        Focus Distance Source: " + Camera.FocusSource);

            // This parameter is only relevant if focus distance source is set to eSPECIFIC_DISTANCE.
            if (Camera.FocusSource == FbxCamera.CameraFocusDistanceSource.SpecificDistance)
            {
                Writer.WriteLine("        Specific Distance: " + Camera.FocusDistance);
            }

            Writer.WriteLine("        Focus Angle: " + Camera.FocusAngle.Get() + " degrees");
        }


        void DisplayDefaultAnimationValues(FbxCamera Camera)
        {
            Writer.WriteLine("    Default Animation Values");

            Writer.WriteLine("        Default Field of View: " + Camera.FieldOfView.Get());
            Writer.WriteLine("        Default Field of View X: " + Camera.FieldOfViewX.Get());
            Writer.WriteLine("        Default Field of View Y: " + Camera.FieldOfViewY.Get());
            Writer.WriteLine("        Default Optical Center X: " + Camera.OpticalCenterX.Get());
            Writer.WriteLine("        Default Optical Center Y: " + Camera.OpticalCenterY.Get());
            Writer.WriteLine("        Default Roll: " + Camera.Roll.Get());
        }



        #endregion

        #region DisplayLight

        void DisplayLight(FbxNode Node)
        {
            FbxLight Light = (FbxLight)Node.NodeAttribute;

            Writer.WriteLine("Light Name: " + Node.Name);
            DisplayMetaDataConnections(Light);



            Writer.WriteLine("    Type: " + Light.Light_Type);
            Writer.WriteLine("    Cast Light: " + Light.CastLight);

            if (!(Light.FileName.Get().IsEmpty))
            {
                Writer.WriteLine("    Gobo");

                Writer.WriteLine("        File Name: \"" + Light.FileName.Get().ToString() + "\"");
                Writer.WriteLine("        Ground Projection: " + Light.DrawGroundProjection);
                Writer.WriteLine("        Volumetric Projection: " + Light.DrawVolumetricLight);
                Writer.WriteLine("        Front Volumetric Projection: " + Light.DrawFrontFacingVolumetricLight);
            }

            DisplayDefaultAnimationValues(Light);
        }

        void DisplayDefaultAnimationValues(FbxLight Light)
        {
            Writer.WriteLine("    Default Animation Values");

            FbxDouble3 c = Light.Color.Get();
            FbxColor Color = new FbxColor(c.X, c.Y, c.Z);
            Writer.WriteLine("        Default Color: " + Color);
            Writer.WriteLine("        Default Intensity: " + Light.Intensity);
            Writer.WriteLine("        Default Cone Angle: " + Light.ConeAngle);
            Writer.WriteLine("        Default Fog: " + Light.Fog);
        }

        #endregion

        #region DisplayPivotsAndLimits
        void DisplayPivotsAndLimits(FbxNode Node)
        {
            FbxVector4 TmpVector;

            //
            // Pivots
            //
            Writer.WriteLine("    Pivot Information\n");

            FbxNode.PivotState PivotState = FbxNode.PivotState.Active;
            Node.GetPivotState(FbxNode.PivotSet.SourceSet, ref PivotState);
            //Writer.WriteLine("        Pivot State: " + PivotState == FbxNode.PivotState.Active ? "Active" : "Reference");
            Writer.WriteLine("        Pivot State: " + PivotState);

            TmpVector = Node.GetPreRotation(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Pre-Rotation: \n" + TmpVector);//.X+ TmpVector.Y+ TmpVector.Z);

            TmpVector = Node.GetPostRotation(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Post-Rotation: \n" + TmpVector);//.X+ TmpVector.Y+ TmpVector.Z);

            TmpVector = Node.GetRotationPivot(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Rotation Pivot: \n" + TmpVector);//.X, TmpVector.Y, TmpVector.Z);

            TmpVector = Node.GetRotationOffset(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Rotation Offset:\n" + TmpVector);//.X, TmpVector.Y, TmpVector.Z);

            TmpVector = Node.GetScalingPivot(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Scaling Pivot: \n" + TmpVector);//[0], TmpVector[1], TmpVector[2]);

            TmpVector = Node.GetScalingOffset(FbxNode.PivotSet.SourceSet);
            Writer.WriteLine("        Scaling Offset: \n" + TmpVector);//[0], TmpVector[1], TmpVector[2]);

            //
            // Limits
            //
            FbxNodeLimits Limits = Node.Limits;
            bool IsActive = false, MinXActive = false, MinYActive = false, MinZActive = false;
            bool MaxXActive, MaxYActive, MaxZActive;
            FbxVector4 MinValues, MaxValues;

            Writer.WriteLine("    Limits Information");

            IsActive = Limits.TranslationLimitActive;
            Limits.TranslationLimits.GetLimitMinActive(out MinXActive, out MinYActive, out MinZActive);
            Limits.TranslationLimits.GetLimitMaxActive(out MaxXActive, out  MaxYActive, out  MaxZActive);
            MinValues = Limits.TranslationLimits.LimitMin;
            MaxValues = Limits.TranslationLimits.LimitMax;

            Writer.WriteLine("        Translation limits: \n" + IsActive);
            Writer.WriteLine("            X\n");
            Writer.WriteLine("                Min Limit: \n" + MinXActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.X);
            Writer.WriteLine("                Max Limit: \n" + MaxXActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.X);
            Writer.WriteLine("            Y\n");
            Writer.WriteLine("                Min Limit: \n" + MinYActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Y);
            Writer.WriteLine("                Max Limit: \n" + MaxYActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Y);
            Writer.WriteLine("            Z\n");
            Writer.WriteLine("                Min Limit: \n" + MinZActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Z);
            Writer.WriteLine("                Max Limit: \n" + MaxZActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Z);

            IsActive = Limits.RotationLimitActive;
            Limits.RotationLimits.GetLimitMinActive(out MinXActive, out  MinYActive, out  MinZActive);
            Limits.RotationLimits.GetLimitMaxActive(out MaxXActive, out  MaxYActive, out  MaxZActive);
            MinValues = Limits.RotationLimits.LimitMin;
            MaxValues = Limits.RotationLimits.LimitMax;

            Writer.WriteLine("        Rotation limits: \n" + IsActive);
            Writer.WriteLine("            X\n");
            Writer.WriteLine("                Min Limit: \n" + MinXActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.X);
            Writer.WriteLine("                Max Limit: \n" + MaxXActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.X);
            Writer.WriteLine("            Y\n");
            Writer.WriteLine("                Min Limit: \n" + MinYActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Y);
            Writer.WriteLine("                Max Limit: \n" + MaxYActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Y);
            Writer.WriteLine("            Z\n");
            Writer.WriteLine("                Min Limit: \n" + MinZActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Z);
            Writer.WriteLine("                Max Limit: \n" + MaxZActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Z);

            IsActive = Limits.ScalingLimitActive;
            Limits.ScalingLimits.GetLimitMinActive(out MinXActive, out  MinYActive, out  MinZActive);
            Limits.ScalingLimits.GetLimitMaxActive(out MaxXActive, out  MaxYActive, out  MaxZActive);
            MinValues = Limits.ScalingLimits.LimitMin;
            MaxValues = Limits.ScalingLimits.LimitMax;

            Writer.WriteLine("        Scaling limits: \n" + IsActive);
            Writer.WriteLine("            X\n");
            Writer.WriteLine("                Min Limit: \n" + MinXActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.X);
            Writer.WriteLine("                Max Limit: \n" + MaxXActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.X);
            Writer.WriteLine("            Y\n");
            Writer.WriteLine("                Min Limit: \n" + MinYActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Y);
            Writer.WriteLine("                Max Limit: \n" + MaxYActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Y);
            Writer.WriteLine("            Z\n");
            Writer.WriteLine("                Min Limit: \n" + MinZActive);
            Writer.WriteLine("                Min Limit Value: \n" + MinValues.Z);
            Writer.WriteLine("                Max Limit: \n" + MaxZActive);
            Writer.WriteLine("                Max Limit Value: \n" + MaxValues.Z);
        }


        #endregion

        #region DisplayPose

        void DisplayPose(FbxScene Scene)
        {
            int i, j, k, PoseCount;
            String Name;

            PoseCount = Scene.PoseCount;

            for (i = 0; i < PoseCount; i++)
            {
                FbxPose Pose = Scene.GetPose(i);

                Name = Pose.Name;
                Writer.WriteLine("Pose Name: " + Name);

                Writer.WriteLine("    Is a bind pose: " + Pose.IsBindPose);

                Writer.WriteLine("    Number of items in the pose: " + Pose.Count);

                Writer.WriteLine("" + "");

                for (j = 0; j < Pose.Count; j++)
                {
                    Name = Pose.GetNodeName(j).CurrentName;
                    Writer.WriteLine("    Item name: " + Name);

                    if (!Pose.IsBindPose)
                    {
                        // Rest pose can have local matrix
                        Writer.WriteLine("    Is local space matrix: " + Pose.IsLocalMatrix(j));
                    }

                    Writer.WriteLine("    Matrix value: " + "");

                    string MatrixValue = "";

                    for (k = 0; k < 4; k++)
                    {
                        FbxMatrix Matrix = Pose.GetMatrix(j);
                        FbxVector4 Row = Matrix.GetRow(k);
                        string RowValue;
                        RowValue = Row.ToString();


                        MatrixValue += "        " + RowValue;
                    }

                    Writer.WriteLine("" + MatrixValue);
                }
            }

            PoseCount = Scene.CharacterPoseCount;

            for (i = 0; i < PoseCount; i++)
            {
                FbxCharacterPose Pose = Scene.GetCharacterPose(i);
                FbxCharacter Character = Pose.Character;

                if (Character == null) break;

                Writer.WriteLine("Character Pose Name: " + Character.Name);

                FbxCharacterLink CharacterLink = null;
                FbxCharacterNodeId NodeId = FbxCharacterNodeId.CharacterHips;

                while (Character.GetCharacterLink(NodeId, CharacterLink))
                {
                    FbxXMatrix GlobalPosition;
                    GlobalPosition = CharacterLink.Node.GetGlobalFromDefaultTake(FbxNode.PivotSet.SourceSet);

                    Writer.WriteLine("    Matrix value: " + "");

                    String MatrixValue = "";

                    for (k = 0; k < 4; k++)
                    {
                        FbxVector4 Row = GlobalPosition.GetRow(k);
                        string RowValue;
                        RowValue = Row.ToString();

                        MatrixValue += "        " + RowValue;
                    }

                    Writer.WriteLine("" + MatrixValue);

                    NodeId = (FbxCharacterNodeId)(NodeId + 1);// ECharacterNodeId(int(lNodeId) + 1);
                }
            }
        }

        #endregion

        #region DisplayAnimation


        void DisplayAnimation(FbxScene Scene)
        {


            //Writer.WriteLine("DisplayAnimation Not implement");
            FbxStringRefArray TakeNameArray = new FbxStringRefArray();
            int i;

            Scene.FillTakeNameArray(TakeNameArray);
            for (i = 0; i < TakeNameArray.Count; i++)
            {
                // It's useless to display the default animation because it is always empty.
                if (TakeNameArray[i].Compare("Default") == 0)
                {
                    continue;
                }

                String OutputString = "Take Name: ";

                Scene.SetCurrentTake(TakeNameArray[i].ToString());
                OutputString += Scene.GetCurrentTakeName();
                OutputString += "\n\n";
                Writer.WriteLine(OutputString);

                DisplayAnimation(Scene.RootNode, true);
                DisplayAnimation(Scene.RootNode, false);
            }

            TakeNameArray.DeleteAndClear();
        }

        void DisplayAnimation(FbxNode Node, bool isSwitcher)
        {
            string CurrentTakeName = Node.CurrentTakeNodeName;
            string DefaultTakeName = Node.GetTakeNodeName(0);
            int ModelCount;

            // Display nothing if the current take node points to default values.
            if (CurrentTakeName != null && CurrentTakeName != DefaultTakeName)
            {
                string OutputString;

                OutputString = "     Node Name: ";
                OutputString += Node.Name;
                OutputString += "\n";
                Writer.WriteLine(OutputString);
                DisplayCurve dc = DisplayCurveKeys;
                DisplayListCurve Dl = DisplayListCurveKeys;
                DisplayChannels(Node, CurrentTakeName, dc, Dl, isSwitcher);

                Writer.WriteLine("");
            }

            for (ModelCount = 0; ModelCount < Node.GetChildCount(); ModelCount++)
            {
                DisplayAnimation(Node.GetChild(ModelCount), isSwitcher);
            }
        }

        void DisplayDefaultAnimation(FbxNode Node)
        {
            string DefaultTakeName = Node.GetTakeNodeName(0);

            Writer.WriteLine("    Default Animation");

            if (DefaultTakeName != null)
            {
                DisplayCurve dc = DisplayCurveDefault;
                DisplayListCurve Dl = DisplayListCurveDefault;
                DisplayChannels(Node, DefaultTakeName, dc, Dl, false);
            }

            Writer.WriteLine("");
        }


        #region DisplayChannels
        delegate void DisplayCurve(FbxCurve Curve);
        delegate void DisplayListCurve(FbxCurve Curve, FbxProperty Property);

        void DisplayChannels(FbxNode Node, string TakeName, DisplayCurve displayCurve, DisplayListCurve displayListCurve, bool isSwitcher)
        {
            FbxCurve Curve = new FbxCurve();


            // Display general curves.
            if (!isSwitcher)
            {
                Curve = Node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_X, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        TX");
                    displayCurve(Curve);
                }
                Curve = Node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Y, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        TY");
                    displayCurve(Curve);
                }
                Curve = Node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Z, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        TZ");
                    displayCurve(Curve);
                }

                Curve = Node.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_X, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        RX");
                    displayCurve(Curve);
                }
                Curve = Node.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Y, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        RY");
                    displayCurve(Curve);
                }
                Curve = Node.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Z, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        RZ");
                    displayCurve(Curve);
                }

                Curve = Node.LclScaling.GetKFCurve(FbxCurve.FBXCURVE_S_X, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        SX");
                    displayCurve(Curve);
                }
                Curve = Node.LclScaling.GetKFCurve(FbxCurve.FBXCURVE_S_Y, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        SY");
                    displayCurve(Curve);
                }
                Curve = Node.LclScaling.GetKFCurve(FbxCurve.FBXCURVE_S_Z, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        SZn");
                    displayCurve(Curve);
                }
            }


            // Display curves specific to a light or marker.
            FbxNodeAttribute NodeAttribute = Node.NodeAttribute;

            if (NodeAttribute != null)
            {
                Curve = NodeAttribute.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_RED, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        Red\n");
                    displayCurve(Curve);
                }
                Curve = NodeAttribute.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_GREEN, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        Green\n");
                    displayCurve(Curve);
                }
                Curve = NodeAttribute.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_BLUE, TakeName);
                if (Curve != null)
                {
                    Writer.WriteLine("        Blue\n");
                    displayCurve(Curve);
                }

                // Display curves specific to a light.
                FbxLight light = Node.Light;
                if (light != null)
                {
                    Curve = light.Intensity.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Intensity\n");
                        displayCurve(Curve);
                    }

                    Curve = light.ConeAngle.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Cone Angle\n");
                        displayCurve(Curve);
                    }

                    Curve = light.Fog.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Fog\n");
                        displayCurve(Curve);
                    }
                }

                // Display curves specific to a camera.
                FbxCamera camera = Node.Camera;
                if (camera != null)
                {
                    Curve = camera.FieldOfView.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Field of View\n");
                        displayCurve(Curve);
                    }

                    Curve = camera.FieldOfViewX.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Field of View X\n");
                        displayCurve(Curve);
                    }

                    Curve = camera.FieldOfViewY.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Field of View Y\n");
                        displayCurve(Curve);
                    }

                    Curve = camera.OpticalCenterX.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Optical Center X\n");
                        displayCurve(Curve);
                    }

                    Curve = camera.OpticalCenterY.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Optical Center Y\n");
                        displayCurve(Curve);
                    }

                    Curve = camera.Roll.GetKFCurve(null, TakeName);
                    if (Curve != null)
                    {
                        Writer.WriteLine("        Roll\n");
                        displayCurve(Curve);
                    }
                }

                // Display curves specific to a geometry.
                if (NodeAttribute.AttribType == FbxNodeAttribute.AttributeType.Mesh ||
                    NodeAttribute.AttribType == FbxNodeAttribute.AttributeType.Nurb ||
                    NodeAttribute.AttribType == FbxNodeAttribute.AttributeType.Patch)
                {
                    FbxGeometry Geometry = (FbxGeometry)NodeAttribute;

                    int Count;

                    for (Count = 0; Count < Geometry.ShapeCount; Count++)
                    {
                        string ShapeName = Geometry.GetShapeName(Count);

                        Curve = Geometry.GetShapeChannel(Count, true, TakeName);

                        if (Curve != null)
                        {
                            Writer.WriteLine("        Shape %s\n" + ShapeName);
                            displayCurve(Curve);
                        }
                    }
                }
            }

            // Display curves specific to properties
            FbxProperty Property = Node.GetFirstProperty();
            while (Property.IsValid)
            {
                if (Property.GetFlag(FbxProperty.FbxPropertyFlagsType.User))
                {
                    String FbxFCurveNodeName = Property.Name.ToString();
                    FbxCurveNode FbxFCurveNode = Property.GetKFCurveNode(false, TakeName);

                    if (FbxFCurveNode == null)
                    {
                        Property = Node.GetNextProperty(Property);
                        continue;
                    }

                    FbxDataType DataType = Property.PropertyDataType;
                    FbxProperty.UserPropertyType EUserPropertyType = FbxProperty.DataTypeToUserPropertyType(DataType);
                    switch (EUserPropertyType)
                    {

                        case FbxProperty.UserPropertyType.Bool:
                        case FbxProperty.UserPropertyType.Real:
                        case FbxProperty.UserPropertyType.Integer:
                            {
                                String Message;
                                Curve = FbxFCurveNode.FCurveGet();

                                Message = "        Property ";
                                Message += Property.Name;
                                if (Property.GetLabel(true).Length > 0)
                                {
                                    Message += " (Label: ";
                                    Message += Property.GetLabel(true);
                                    Message += ")";
                                };

                                Writer.WriteLine(Message);
                                displayCurve(Curve);
                            }
                            break;

                        case FbxProperty.UserPropertyType.Vector:
                        case FbxProperty.UserPropertyType.Color:
                            {
                                string ComponentName1 = EUserPropertyType == FbxProperty.UserPropertyType.Color ? FbxCurve.FBXCURVE_COLOR_RED : "X";
                                string ComponentName2 = EUserPropertyType == FbxProperty.UserPropertyType.Color ? FbxCurve.FBXCURVE_COLOR_GREEN : "Y";
                                string ComponentName3 = EUserPropertyType == FbxProperty.UserPropertyType.Color ? FbxCurve.FBXCURVE_COLOR_BLUE : "Z";
                                FbxCurveNode FbxComponentFCurveNode = null;
                                String Message;

                                Message = "        Property ";
                                Message += Property.Name;
                                if (Property.GetLabel(true).Length > 0)
                                {
                                    Message += " (Label: ";
                                    Message += Property.GetLabel(true);
                                    Message += ")";
                                }

                                Writer.WriteLine(Message);

                                FbxComponentFCurveNode = FbxFCurveNode.FindRecursive(ComponentName1);
                                if (FbxComponentFCurveNode != null)
                                {
                                    Curve = FbxComponentFCurveNode.FCurveGet();
                                    Writer.WriteLine("        Component " + ComponentName1);
                                    displayCurve(Curve);
                                }

                                FbxComponentFCurveNode = FbxFCurveNode.FindRecursive(ComponentName2);
                                if (FbxComponentFCurveNode != null)
                                {
                                    Curve = FbxComponentFCurveNode.FCurveGet();
                                    Writer.WriteLine("        Component " + ComponentName2);
                                    displayCurve(Curve);
                                }

                                FbxComponentFCurveNode = FbxFCurveNode.FindRecursive(ComponentName3);
                                if (FbxComponentFCurveNode != null)
                                {
                                    Curve = FbxComponentFCurveNode.FCurveGet();
                                    Writer.WriteLine("        Component " + ComponentName3);
                                    displayCurve(Curve);
                                }
                            }
                            break;

                        case FbxProperty.UserPropertyType.List:
                            {
                                String Message;
                                Curve = FbxFCurveNode.FCurveGet();

                                Message = "        Property ";
                                Message += Property.Name;
                                if (Property.GetLabel(true).Length > 0)
                                {
                                    Message += " (Label: ";
                                    Message += Property.GetLabel(true);
                                    Message += ")";
                                };

                                Writer.WriteLine(Message);
                                displayListCurve(Curve, Property);
                            }
                            break;
                    } // switch
                }

                Property = Node.GetNextProperty(Property);
            } // while

        }
        #endregion

        static int InterpolationFlagToIndex(int flags)
        {
            if (((FbxCurveKey.KeyInterpolation)flags & FbxCurveKey.KeyInterpolation.Constant) == FbxCurveKey.KeyInterpolation.Constant)
                return 1;
            if (((FbxCurveKey.KeyInterpolation)flags & FbxCurveKey.KeyInterpolation.Linear) == FbxCurveKey.KeyInterpolation.Linear)
                return 2;
            if (((FbxCurveKey.KeyInterpolation)flags & FbxCurveKey.KeyInterpolation.Cubic) == FbxCurveKey.KeyInterpolation.Cubic)
                return 3;
            return 0;
        }

        static int ConstantmodeFlagToIndex(int flags)
        {
            if (((FbxCurveKey.KeyConstantMode)flags & FbxCurveKey.KeyConstantMode.STANDARD) == FbxCurveKey.KeyConstantMode.STANDARD)
                return 1;
            if (((FbxCurveKey.KeyConstantMode)flags & FbxCurveKey.KeyConstantMode.NEXT) == FbxCurveKey.KeyConstantMode.NEXT)
                return 2;
            return 0;
        }

        static int TangeantmodeFlagToIndex(int flags)
        {
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.TangentAuto) == FbxCurveKey.KeyTangentMode.TangentAuto)
                return 1;
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.TangentAutoBreak) == FbxCurveKey.KeyTangentMode.TangentAutoBreak)
                return 2;
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.TangentTcb) == FbxCurveKey.KeyTangentMode.TangentTcb)
                return 3;
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.TangentUser) == FbxCurveKey.KeyTangentMode.TangentUser)
                return 4;
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.GenericBreak) == FbxCurveKey.KeyTangentMode.GenericBreak)
                return 5;
            if (((FbxCurveKey.KeyTangentMode)flags & FbxCurveKey.KeyTangentMode.TangentBreak) == FbxCurveKey.KeyTangentMode.TangentBreak)
                return 6;
            return 0;
        }

        static int TangeantweightFlagToIndex(int flags)
        {
            if (((FbxCurveKey.KeyTangentWeightMode)flags & FbxCurveKey.KeyTangentWeightMode.None) == FbxCurveKey.KeyTangentWeightMode.None)
                return 1;
            if (((FbxCurveKey.KeyTangentWeightMode)flags & FbxCurveKey.KeyTangentWeightMode.Right) == FbxCurveKey.KeyTangentWeightMode.Right)
                return 2;
            if (((FbxCurveKey.KeyTangentWeightMode)flags & FbxCurveKey.KeyTangentWeightMode.NextLeft) == FbxCurveKey.KeyTangentWeightMode.NextLeft)
                return 3;
            return 0;
        }

        static int TangeantVelocityFlagToIndex(int flags)
        {
            if (((FbxCurveKey.KeyTangentVelocityMode)flags & FbxCurveKey.KeyTangentVelocityMode.None) == FbxCurveKey.KeyTangentVelocityMode.None)
                return 1;
            if (((FbxCurveKey.KeyTangentVelocityMode)flags & FbxCurveKey.KeyTangentVelocityMode.Right) == FbxCurveKey.KeyTangentVelocityMode.Right)
                return 2;
            if (((FbxCurveKey.KeyTangentVelocityMode)flags & FbxCurveKey.KeyTangentVelocityMode.NextLeft) == FbxCurveKey.KeyTangentVelocityMode.NextLeft)
                return 3;
            return 0;
        }

        void DisplayCurveKeys(FbxCurve Curve)
        {


            FbxTime KeyTime;
            float KeyValue;
            string TimeString = "";
            string OutputString;
            int Count;

            int KeyCount = Curve.KeyCount;

            for (Count = 0; Count < KeyCount; Count++)
            {
                KeyValue = Curve.KeyGetValue(Count);
                KeyTime = Curve.KeyGetTime(Count);

                OutputString = "            Key Time: ";
                OutputString += KeyTime.GetTimeString(TimeString, 5, FbxTime.TimeMode.DefaultMode, FbxTime.TimeProtocol.DefaultProtocol, 0.0);
                OutputString += ".... Key Value: ";
                OutputString += KeyValue;
                OutputString += " [ ";
                OutputString += InterpolationFlagToIndex((int)Curve.KeyGetInterpolation(Count));
                if ((Curve.KeyGetInterpolation(Count) & FbxCurveKey.KeyInterpolation.Constant) == FbxCurveKey.KeyInterpolation.Constant)
                {
                    OutputString += " | ";
                    OutputString += ConstantmodeFlagToIndex((int)Curve.KeyGetConstantMode(Count));
                    OutputString += " ]";
                }
                else
                    if ((Curve.KeyGetInterpolation(Count) & FbxCurveKey.KeyInterpolation.Cubic) == FbxCurveKey.KeyInterpolation.Cubic)
                    {
                        OutputString += " | ";
                        OutputString += TangeantmodeFlagToIndex((int)Curve.KeyGetTangentMode(Count));
                        OutputString += " | ";
                        OutputString += TangeantweightFlagToIndex((int)Curve.KeyGetTangentWeightMode(Count));
                        OutputString += " | ";
                        OutputString += TangeantVelocityFlagToIndex((int)Curve.KeyGetTangentVelocityMode(Count));
                        OutputString += " ]";
                    }
                OutputString += "\n";
                Writer.WriteLine(OutputString);
            }
        }
        void DisplayCurveDefault(FbxCurve Curve)
        {
            string OutputString;

            OutputString = "            Default Value: ";
            OutputString += Curve.Value;

            Writer.WriteLine(OutputString);
        }
        void DisplayListCurveKeys(FbxCurve Curve, FbxProperty Property)
        {
            FbxTime KeyTime;
            int KeyValue;
            string TimeString = "";
            //string ListValue;
            string OutputString;
            int Count;

            int KeyCount = Curve.KeyCount;

            for (Count = 0; Count < KeyCount; Count++)
            {
                KeyValue = (int)Curve.KeyGetValue(Count);
                KeyTime = Curve.KeyGetTime(Count);

                OutputString = "            Key Time: ";
                OutputString += KeyTime.GetTimeString(TimeString, 5, FbxTime.TimeMode.DefaultMode, FbxTime.TimeProtocol.DefaultProtocol, 0.0);
                OutputString += ".... Key Value: ";
                OutputString += KeyValue;
                OutputString += " (";
                OutputString += Property.GetEnumValue(KeyValue);
                OutputString += ")";

                OutputString += "\n";
                Writer.WriteLine(OutputString);
            }
        }
        void DisplayListCurveDefault(FbxCurve Curve, FbxProperty Property)
        {
            DisplayCurveDefault(Curve);
        }

        #endregion

        #region DisplayGenericInfo
        void DisplayGenericInfo(FbxScene Scene)
        {
            int i;
            FbxNode RootNode = Scene.RootNode;

            for (i = 0; i < RootNode.GetChildCount(); i++)
            {
                DisplayGenericInfo(RootNode.GetChild(i), 0);
            }
        }


        void DisplayGenericInfo(FbxNode Node, int Depth)
        {
            string Str = "";
            int i;

            for (i = 0; i < Depth; i++)
            {
                Str += "     ";
            }

            Str += Node.Name;
            Str += "\n";

            Writer.WriteLine(Str);

            //Display generic info about that Node
            DisplayProperties(Node);
            Writer.WriteLine("");
            for (i = 0; i < Node.GetChildCount(); i++)
            {
                DisplayGenericInfo(Node.GetChild(i), Depth + 1);
            }
        }

        void DisplayProperties(FbxObject Object)
        {

            Writer.WriteLine("Name: " + Object.Name);

            // Display all the properties
            int i, Count = 0;
            FbxProperty Property = Object.GetFirstProperty();
            while (Property.IsValid)
            {
                Count++;
                Property = Object.GetNextProperty(Property);
            }

            string TitleStr = "    Property Count: ";

            if (Count == 0)
                return; // there are no properties to display

            Writer.WriteLine(TitleStr + Count);

            i = 0;
            while (Property.IsValid)
            {
                // exclude user properties

                string Str;
                Writer.WriteLine("        Property " + i);
                Str = Property.GetLabel(true).ToString();
                Writer.WriteLine("            Display Name: " + Str);
                Str = Property.Name.ToString();
                Writer.WriteLine("            Internal Name: " + Str);
                Str = Property.PropertyDataType.Name;
                Writer.WriteLine("            Type: " + Str);
                Writer.WriteLine("            Min Limit: " + Property.MinLimit);
                Writer.WriteLine("            Max Limit: " + Property.MaxLimit);
                Writer.WriteLine("            Is Animatable: " + Property.GetFlag(FbxPropertyFlags.FbxPropertyFlagsType.AnimaTable));
                Writer.WriteLine("            Is Temporary: " + Property.GetFlag(FbxPropertyFlags.FbxPropertyFlagsType.Temporary));



                switch (Property.PropertyDataType.Type)
                {
                    case FbxType.Bool1:
                        Writer.WriteLine("            Default Value: " + Property.GetValueAsBool());
                        break;

                    case FbxType.Double1:
                        Writer.WriteLine("            Default Value: " + Property.GetValueAsDouble());
                        break;

                    case FbxType.Double4:
                        {
                            FbxColor Default;
                            //  char      lBuf[64];

                            Default = Property.GetValueAsColor();
                            Writer.WriteLine(string.Format("R={0}, G={1}, B={2} A={3}", Default.Red, Default.Green, Default.Blue, Default.Alpha));

                        }
                        break;

                    case FbxType.Integer1:
                        Writer.WriteLine("            Default Value: " + Property.GetValueAsInt());
                        break;

                    case FbxType.Double3:
                        {
                            FbxDouble3 Default;


                            Default = Property.GetValueAsDouble3();
                            Writer.WriteLine(string.Format("X={0}, Y={1}, Z={2}", Default.X, Default.Y, Default.Z));

                        }
                        break;

                    //case DTEnum:
                    //	DisplayInt("            Default Value: ", KFbxGet <int> (Property));
                    //break;

                    case FbxType.Float1:
                        Writer.WriteLine("            Default Value: " + Property.GetValueAsFloat());
                        break;
                    case FbxType.String:
                        Str = Property.GetValueAsString();
                        Writer.WriteLine("            Default Value: " + Str);
                        break;

                    default:
                        Writer.WriteLine("            Default Value: UNIDENTIFIED");
                        break;
                }
                i++;
                Property = Object.GetNextProperty(Property);
            }
        }


        #endregion


        #region DisplayNurb
        void DisplayNurb(FbxNode Node)
        {
            FbxNurb Nurb = (FbxNurb)Node.NodeAttribute;
            int i;

            Writer.WriteLine("Nurb Name: " + Node.Name);
            DisplayMetaDataConnections(Nurb);



            Writer.WriteLine("    Surface Mode: " + Nurb.Surface_Mode);

            int ControlPointsCount = Nurb.ControlPointsCount;
            FbxVector4[] ControlPoints = Nurb.ControlPoints;

            for (i = 0; i < ControlPointsCount; i++)
            {
                Writer.WriteLine("    Control Point " + i);
                Writer.WriteLine("        Coordinates: " + ControlPoints[i]);
                Writer.WriteLine("        Weight: " + ControlPoints[i].Z);
            }



            Writer.WriteLine("    Nurb U Type: " + Nurb.NurbUType);
            Writer.WriteLine("    U Count: " + Nurb.UCount);
            Writer.WriteLine("    Nurb V Type: " + Nurb.NurbVType);
            Writer.WriteLine("    V Count: " + Nurb.VCount);
            Writer.WriteLine("    U Order: " + Nurb.UOrder);
            Writer.WriteLine("    V Order: " + Nurb.VOrder);
            Writer.WriteLine("    U Step: " + Nurb.UStep);
            Writer.WriteLine("    V Step: " + Nurb.VStep);

            string Str;
            int UKnotCount = Nurb.UKnotCount;
            int VKnotCount = Nurb.VKnotCount;
            int UMultiplicityCount = Nurb.UCount;
            int VMultiplicityCount = Nurb.VCount;
            double[] UKnotVector = Nurb.UKnotVectorArray;
            double[] VKnotVector = Nurb.VKnotVectorArray;
            int[] UMultiplicityVector = Nurb.UMultiplicityVectorArray;
            int[] VMultiplicityVector = Nurb.VMultiplicityVectorArray;

            Str = "    U Knot Vector: ";

            for (i = 0; i < UKnotCount; i++)
            {
                Str += (float)UKnotVector[i];

                if (i < UKnotCount - 1)
                {
                    Str += ", ";
                }
            }

            Str += "\n";
            Writer.WriteLine(Str);

            Str = "    V Knot Vector: ";

            for (i = 0; i < VKnotCount; i++)
            {
                Str += (float)VKnotVector[i];

                if (i < VKnotCount - 1)
                {
                    Str += ", ";
                }
            }

            Str += "\n";
            Writer.WriteLine(Str);

            Str = "    U Multiplicity Vector: ";

            for (i = 0; i < UMultiplicityCount; i++)
            {
                Str += UMultiplicityVector[i];

                if (i < UMultiplicityCount - 1)
                {
                    Str += ", ";
                }
            }

            Str += "\n";
            Writer.WriteLine(Str);

            Str = "    V Multiplicity Vector: ";

            for (i = 0; i < VMultiplicityCount; i++)
            {
                Str += VMultiplicityVector[i];

                if (i < VMultiplicityCount - 1)
                {
                    Str += ", ";
                }
            }

            Str += "\n";
            Writer.WriteLine(Str);

            Writer.WriteLine("");

            DisplayTexture(Nurb);
            DisplayMaterial(Nurb);
            DisplayLink(Nurb);
            DisplayShape(Nurb);
        }







        #endregion


        #region UserProperties
        void DisplayUserProperties(FbxObject Object)
        {
            int Count = 0;
            String TitleStr = "    Property Count: ";

            FbxProperty Property = Object.GetFirstProperty();
            while (Property.IsValid)
            {
                if (Property.GetFlag(FbxProperty.FbxPropertyFlagsType.User))
                    Count++;

                Property = Object.GetNextProperty(Property);
            }

            if (Count == 0)
                return; // there are no user properties to display

            Writer.WriteLine(TitleStr + Count);

            Property = Object.GetFirstProperty();
            int i = 0;
            while (Property.IsValid)
            {
                if (Property.GetFlag(FbxProperty.FbxPropertyFlagsType.User))
                {
                    Writer.WriteLine("        Property " + i);
                    String Str = Property.GetLabel(true).ToString();
                    Writer.WriteLine("            Display Name: " + Str);
                    Str = Property.Name.ToString();
                    Writer.WriteLine("            Internal Name: " + Str);
                    Writer.WriteLine("            Type: " + Property.PropertyDataType.Name);
                    if (Property.HasMinLimit)
                        Writer.WriteLine("            Min Limit: " + Property.MinLimit);
                    if (Property.HasMaxLimit)
                        Writer.WriteLine("            Max Limit: " + Property.MaxLimit);
                    Writer.WriteLine("            Is Animatable: " + Property.GetFlag(FbxProperty.FbxPropertyFlagsType.AnimaTable));
                    Writer.WriteLine("            Is Temporary: " + Property.GetFlag(FbxProperty.FbxPropertyFlagsType.Temporary));

                    FbxDataType PropertyDataType = Property.PropertyDataType;

                    FbxProperty.UserPropertyType EUserPropertyType = FbxProperty.DataTypeToUserPropertyType(PropertyDataType);
                    //KFbxProperty aProperty;
                    switch (EUserPropertyType)
                    {
                        case FbxProperty.UserPropertyType.Bool:
                            Writer.WriteLine("            Default Value: " + Property.GetValueAsBool());
                            break;

                        case FbxProperty.UserPropertyType.Real:
                            Writer.WriteLine("            Default Value: " + Property.GetValueAsDouble());
                            break;

                        case FbxProperty.UserPropertyType.Color:
                            {
                                FbxColor Default;
                                //  char      lBuf[64];

                                Default = Property.GetValueAsColor();
                                Writer.WriteLine(string.Format("R={0}, G={1}, B={2} A={3}", Default.Red, Default.Green, Default.Blue, Default.Alpha));

                            }
                            break;

                        case FbxProperty.UserPropertyType.Integer:
                            Writer.WriteLine("            Default Value: " + Property.GetValueAsInt());
                            break;

                        case FbxProperty.UserPropertyType.Vector:
                            {
                                FbxDouble3 Default;


                                Default = Property.GetValueAsDouble3();
                                Writer.WriteLine(string.Format("X={0}, Y={1}, Z={2}", Default.X, Default.Y, Default.Z));

                            }
                            break;

                        //case DTEnum:
                        //	DisplayInt("            Default Value: ", KFbxGet <int> (Property));
                        //break;

                        case FbxProperty.UserPropertyType.List:
                            Writer.WriteLine("            Default Value: " + Property.GetValueAsInt());
                            break;
                        case FbxProperty.UserPropertyType.Unidentified:


                            Writer.WriteLine("            Default Value: UNIDENTIFIED");
                            break;





                    }
                    i++;
                }

                Property = Object.GetNextProperty(Property);
            }
        }
        #endregion


        void DisplayMetaDataConnections(FbxObject Obj)
        {

            int nbMetaData = FbxStatics._FbxObjectMetaData.FbxGetSrcCount(Obj); //KFbxGetSrcCount<KFbxObjectMetaData>(pObject);
            if (nbMetaData > 0)
                Writer.WriteLine("    MetaData connections ");

            for (int i = 0; i < nbMetaData; i++)
            {
                FbxObjectMetaData metaData = FbxStatics._FbxObjectMetaData.FbxGetSrc(Obj, i);
                Writer.WriteLine("        Name: " + metaData.Name);
            }
        }


    }
}
