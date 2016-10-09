using System;
using System.Collections.Generic;
using System.Text;
using Skill.FbxSDK;
using Skill.FbxSDK.IO;



namespace SceneTreeView
{
    public class SdkUtility
    {
        public FbxSdkManager SdkManager { get; private set; }
        public FbxScene Scene { get; private set; }

        public SdkUtility()
        {
            SdkManager = null;
            Scene = null;
        }

        public bool LoadFBXScene(string FbxFilePath)
        {
            // Initialize the KFbxSdkManager and the KFbxScene
            if (InitializeSdkObjects() == false) return false;

            // Load the scene.
            if (LoadScene(FbxFilePath) == false) return false;

            return true;
        }

        private bool InitializeSdkObjects()
        {
            // The first thing to do is to create the FBX SDK manager which is the 
            // object allocator for almost all the classes in the SDK.
            SdkManager = FbxSdkManager.Create();

            if (SdkManager == null)
            {
                return false;
            }

            // Create the entity that will hold the scene.
            Scene = FbxScene.Create(SdkManager, "");

            if (Scene == null) return false;

            return true;
        }

        // to destroy an instance of the SDK manager
        public void DestroySdkObjects()
        {
            // Delete the FBX SDK manager. All the objects that have been allocated 
            // using the FBX SDK manager and that haven't been explicitely destroyed 
            // are automatically destroyed at the same time.
            if (SdkManager != null)
            {
                SdkManager.Destroy();
                SdkManager = null;
            }
        }

        private bool LoadScene(string FbxFilePath)
        {
            int fileFormat = -1;
            bool status;

            FbxStreamOptionsFbxReader importOptions = FbxStreamOptionsFbxReader.Create(SdkManager, "");

            // Create an importer.
            FbxImporter importer = FbxImporter.Create(SdkManager, "");

            if (!SdkManager.IOPluginRegistry.DetectFileFormat(FbxFilePath, ref fileFormat))
            {
                // Unrecognizable file format. Try to fall back to native format.
                fileFormat = SdkManager.IOPluginRegistry.NativeReaderFormat;
            }

            importer.FileFormat = fileFormat;

            // Initialize the importer by providing a filename.
            bool importStatus = importer.Initialize(FbxFilePath);

            if (!importStatus)
            {
                // Destroy the import options
                if (importOptions != null)
                {
                    importOptions.Destroy();
                    importOptions = null;
                }

                // Destroy the importer
                importer.Destroy();

                return false;
            }

            if (importer.IsFBX)
            {
                // Set the import states. By default, the import states are always set to 
                // true. The code below shows how to change these states.
                importOptions.SetOption(FbxStreamOptionsFbx.MATERIAL, true);
                importOptions.SetOption(FbxStreamOptionsFbx.TEXTURE, true);
                importOptions.SetOption(FbxStreamOptionsFbx.LINK, true);
                importOptions.SetOption(FbxStreamOptionsFbx.SHAPE, true);
                importOptions.SetOption(FbxStreamOptionsFbx.GOBO, true);
                importOptions.SetOption(FbxStreamOptionsFbx.ANIMATION, true);
                importOptions.SetOption(FbxStreamOptionsFbx.GLOBAL_SETTINGS, true);
            }

            // Import the scene
            status = importer.Import(Scene, importOptions);

            // Destroy the import options
            if (importOptions != null)
            {
                importOptions.Destroy();
                importOptions = null;
            }

            // Destroy the importer
            importer.Destroy();

            return status;
        }

        // to get the filters for the <Open file> dialog (description + file extention)
        public static string GetReaderOFNFilters()
        {
            // create a temp KFbxSdkManager
            FbxSdkManager sdkManager = FbxSdkManager.Create();
            int nbReaders = sdkManager.IOPluginRegistry.ReaderFormatCount;

            string s = string.Empty;

            for (int i = 0; i < nbReaders; i++)
            {
                s += sdkManager.IOPluginRegistry.GetReaderFormatDescription(i);
                s += "|*.";
                s += sdkManager.IOPluginRegistry.GetReaderFormatExtension(i);
                if (i < nbReaders - 1)
                    s += "|";
            }

            // Delete the FBX SDK manager.
            if (sdkManager != null)
            {
                sdkManager.Destroy();
                sdkManager = null;
            }
            return s;
        }

        // to get a string from the node name and attribute type
        public static string GetNodeNameAndAttributeTypeName(FbxNode node)
        {
            string s = node.Name;

            FbxNodeAttribute.AttributeType attributeType;
            FbxNodeAttribute nodeAttribute = node.NodeAttribute;

            if (nodeAttribute == null)
            {
                s += " (No node attribute type)";
            }
            else
            {
                attributeType = nodeAttribute.AttribType;

                switch (attributeType)
                {
                    case FbxNodeAttribute.AttributeType.Marker: s += " (Marker)"; break;
                    case FbxNodeAttribute.AttributeType.Skeleton: s += " (Skeleton)"; break;
                    case FbxNodeAttribute.AttributeType.Mesh: s += " (Mesh)"; break;
                    case FbxNodeAttribute.AttributeType.Camera: s += " (Camera)"; break;
                    case FbxNodeAttribute.AttributeType.Light: s += " (Light)"; break;
                    case FbxNodeAttribute.AttributeType.Boundary: s += " (Boundary)"; break;
                    case FbxNodeAttribute.AttributeType.Constraint: s += " (Constraint)"; break;
                    case FbxNodeAttribute.AttributeType.OpticalMarker: s += " (Optical marker)"; break;
                    case FbxNodeAttribute.AttributeType.OpticalReference: s += " (Optical reference)"; break;
                    case FbxNodeAttribute.AttributeType.CameraSwitcher: s += " (Camera switcher)"; break;
                    case FbxNodeAttribute.AttributeType.Null: s += " (Null)"; break;
                    case FbxNodeAttribute.AttributeType.Patch: s += " (Patch)"; break;
                    case FbxNodeAttribute.AttributeType.Nurb: s += " (NURB)"; break;
                    case FbxNodeAttribute.AttributeType.NurbsSurface: s += " (Nurbs surface)"; break;
                    case FbxNodeAttribute.AttributeType.NurbsCurve: s += " (NURBS curve)"; break;
                    case FbxNodeAttribute.AttributeType.TrimNurbsSurface: s += " (Trim nurbs surface)"; break;
                    case FbxNodeAttribute.AttributeType.Unidentified: s += " (Unidentified)"; break;
                }
            }

            return s;
        }

        // to get a string from the node default translation values
        public static string GetDefaultTranslationInfo(FbxNode node)
        {
            FbxVector4 v4 = new FbxVector4();
            node.GetDefaultT(v4);
            return "Translation (X,Y,Z): " + v4.X.ToString() + ", " + v4.Y.ToString() + ", " + v4.Z.ToString();
        }

        // to get a string from the node visibility value
        public static string GetNodeVisibility(FbxNode node)
        {
            return "Visibility: " + ((node.Visible) ? "Yes" : "No").ToString();
        }
    }
}
