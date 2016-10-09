using System;
using System.Collections.Generic;
using System.Text;
using Skill.FbxSDK;
using Skill.FbxSDK.IO;



namespace CubeCreator
{
    public class SdkUtility
    {
        public FbxSdkManager SdkManager { get; private set; }
        public FbxScene Scene { get; private set; }
        public FbxTexture Texture { get; private set; }
        public FbxSurfacePhong Material { get; private set; }

        /// <summary>
        /// Cube Number
        /// </summary>
        public int CubeNumber { get; private set; }
        /// <summary>
        /// Cube Rotation Axis 0==X, 1==Y, 2==Z
        /// </summary>
        public int CubeRotationAxis { get; private set; }
        /// <summary>
        /// initial CubXPos
        /// </summary>
        public double CubeXPos { get; private set; }
        /// <summary>
        /// initial CubeYPos
        /// </summary>
        public double CubeYPos { get; private set; }
        /// <summary>
        /// initial CubeZPos
        /// </summary>
        public double CubeZPos { get; private set; }

        /// <summary>
        /// take name
        /// </summary>
        public string TakeName { get; private set; }
        /// <summary>
        /// path where the application started
        /// </summary>
        public string AppPath { get; private set; }

        public SdkUtility()
        {
            SdkManager = null;
            Scene = null;
            SetInitialCubeData();
            TakeName = null;
            AppPath = null;
        }

        public bool InitializeSdkObjects()
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

        // to create a basic scene
        public bool CreateScene()
        {
            // Initialize the KFbxSdkManager and the KFbxScene
            if (InitializeSdkObjects() == false)
            {
                return false;
            }

            // set the take name
            TakeName = "Take camera animation";

            // set this take name as the current take
            Scene.SetCurrentTake(TakeName);

            // create a marker
            FbxNode marker = CreateMarker("Marker");

            // create a camera
            FbxNode camera = CreateCamera("Camera");

            // create a single texture shared by all cubes
            CreateTexture();

            // create a material shared by all faces of all cubes
            CreateMaterial();

            // set the camera point of interest on the marker
            SetCameraPointOfInterest(camera, marker);

            // set the marker position
            SetMarkerDefaultPosition(marker);

            // set the camera position
            SetCameraDefaultPosition(camera);

            // animate the camera
            AnimateCamera(camera, TakeName);

            // build a minimum scene graph
            FbxNode rootNode = Scene.RootNode;
            rootNode.AddChild(marker);
            rootNode.AddChild(camera);

            // set camera switcher as the default camera
            Scene.GlobalCameraSettings.SetDefaultCamera(camera.Name);
            return true;
        }

        // create a new cube
        public void CreateCube(bool withMaterial, bool withTexture, bool animate)
        {
            // make a new cube name
            string cubeName = "Cube number ";
            cubeName += CubeNumber;

            // create a new cube
            CreateCubeDetailed(cubeName,
                CubeXPos, CubeYPos, CubeZPos,
                CubeRotationAxis, withMaterial, withTexture, animate);

            // compute for next cube creation    
            CubeNumber++; // cube number

            // set next pos
            if (CubeXPos >= 0.0)
            {
                CubeXPos += 50.0;
                CubeXPos *= -1.0;
                CubeRotationAxis++; // change rotation axis
            }
            else
            {
                CubeXPos *= -1.0;
            }

            // go up
            CubeYPos += 30.0;

            if (CubeRotationAxis > 2) CubeRotationAxis = 0; // cube rotation
        }

        // to remove cubes only
        public void RemoveCubes()
        {
            if (SdkManager == null) return;

            // get the node count
            int nc = Scene.NodeCount;

            // we want to keep the root node, the marker node and the camera node
            if (nc <= 3) return;

            // remove other nodes (cube nodes)
            // start from the end
            for (int i = nc - 1; i >= 3; i--)
            {
                FbxNode node = Scene.GetNode(i);
                Scene.RemoveNode(node);

                // remove from memory
                node.Destroy(true, true);
            }

            // reset cube data
            SetInitialCubeData();
        }


        private void CreateCubeDetailed(string cubeName, double x, double y, double z, int rotationAxis, bool withMaterial, bool withTexture, bool animate)
        {
            FbxNode cube = CreateCubeMesh(cubeName);

            // set the cube position
            cube.LclTranslation.Set(new FbxDouble3(x, y, z));

            if (animate)
            {
                AnimateCube(cube, TakeName, rotationAxis);
            }

            if (withTexture)
            {
                // use already created texture
                AddTexture(cube.Mesh);
            }

            if (withMaterial)
            {
                // use already created material
                AddMaterials(cube.Mesh);
            }

            Scene.RootNode.AddChild(cube);
        }

        // to save a scene to a FBX file
        public bool Export(string filename, int fileFormat)
        {
            return SaveScene(filename, fileFormat, true); // true -> embed texture file
        }

        public bool SaveScene(string filename, int fileFormat, bool embedMedia)
        {
            if (SdkManager == null) return false;
            if (Scene == null) return false;
            if (filename == null) return false;

            bool status = true;

            // Create an exporter.
            FbxExporter exporter = FbxExporter.Create(SdkManager, "");

            // Initialize the exporter by providing a filename.
            if (exporter.Initialize(filename) == false)
            {
                return false;
            }

            if (fileFormat < 0 || fileFormat >= SdkManager.IOPluginRegistry.WriterFormatCount)
            {
                // Write in fall back format if pEmbedMedia is true
                fileFormat = SdkManager.IOPluginRegistry.NativeWriterFormat;

                if (!embedMedia)
                {
                    //Try to export in ASCII if possible
                    int formatIndex, formatCount = SdkManager.IOPluginRegistry.WriterFormatCount;

                    for (formatIndex = 0; formatIndex < formatCount; formatIndex++)
                    {
                        if (SdkManager.IOPluginRegistry.WriterIsFBX(formatIndex))
                        {
                            string desc = SdkManager.IOPluginRegistry.GetWriterFormatDescription(formatIndex);
                            string ASCII = "ascii";
                            if (desc.Contains(ASCII))
                            {
                                fileFormat = formatIndex;
                                break;
                            }
                        }
                    }
                }
            }

            // Set the file format
            exporter.FileFormat = fileFormat;

            FbxStreamOptionsFbxWriter exportOptions = FbxStreamOptionsFbxWriter.Create(SdkManager, "");
            if (SdkManager.IOPluginRegistry.WriterIsFBX(fileFormat))
            {
                // Set the export states. By default, the export states are always set to 
                // true except for the option eEXPORT_TEXTURE_AS_EMBEDDED. The code below 
                // shows how to change these states.

                exportOptions.SetOption(FbxStreamOptionsFbx.MATERIAL, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.TEXTURE, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.EMBEDDED, embedMedia);
                exportOptions.SetOption(FbxStreamOptionsFbx.MEDIA, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.LINK, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.SHAPE, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.GOBO, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.ANIMATION, true);
                exportOptions.SetOption(FbxStreamOptionsFbx.GLOBAL_SETTINGS, true);
            }

            // Export the scene.
            status = exporter.Export(Scene, exportOptions);

            if (exportOptions != null && exportOptions.IsValid)
            {
                exportOptions.Destroy();
            }

            // Destroy the exporter.
            exporter.Destroy();

            return status;
        }

        // to get the filters for the <Save file> dialog (description + file extention)
        public static string GetWriterSFNFilters()
        {
            // create a temp FbxSdkManager
            FbxSdkManager sdkManager = FbxSdkManager.Create();
            FbxIOPluginRegistry iop = sdkManager.IOPluginRegistry;
            int nbWriters = iop.WriterFormatCount;

            string s = string.Empty;
            int i = 0;

            for (i = 0; i < nbWriters; i++)
            {
                s += iop.GetWriterFormatDescription(i);
                s += "|*.";
                s += iop.GetWriterFormatExtension(i);
                if (i < nbWriters - 1)
                    s += "|";
            }

            // Delete the FBX SDK manager.
            if (sdkManager != null && sdkManager.IsValid)
            {
                sdkManager.Destroy();
                sdkManager = null;
            }
            return s;
        }


        // to get a file extention for a WriteFileFormat
        public static string GetFileFormatExt(int writeFileFormat)
        {
            // create a temp KFbxSdkManager
            FbxSdkManager sdkManager = FbxSdkManager.Create();

            // add a starting point .
            string s = ".";
            s += sdkManager.IOPluginRegistry.GetWriterFormatExtension(writeFileFormat);

            // Delete the FBX SDK manager.
            if (sdkManager != null && sdkManager.IsValid)
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

        // to get a string with info about material, texture, animation
        public string GetNodeInfo(FbxNode node)
        {
            string s = string.Empty;

            // check for material
            if (node.MaterialCount > 0)
                s += "[Material: Yes] ";
            else
                s += "[Material: No] ";

            // check for textures
            FbxMesh mesh = node.Mesh;
            if (mesh != null)
            {
                FbxLayer layer = mesh.GetLayer(0);
                if (layer != null && layer.DiffuseTextures != null && layer.DiffuseTextures.DirectArray.Count > 0)
                {
                    s += "[Texture: Yes] ";
                }
                else
                {
                    s += "[Texture: No] ";
                }
            }
            else
            {
                s += "[Texture: No] ";
            }

            // check for animation
            bool anim = false;
            FbxCurveNode curveNode = null;

            // check rotation FCurve node
            curveNode = node.LclRotation.GetKFCurveNode(false, TakeName);
            if (curveNode != null) anim = true;

            // check Translation FCurve node
            curveNode = node.LclTranslation.GetKFCurveNode(false, TakeName);
            if (curveNode != null) anim = true;

            if (anim == true)
            {
                s += "[Animation: Yes] ";
            }
            else
            {
                s += "[Animation: No] ";
            }

            return s;
        }
        // Create a marker to use a point of interest for the camera. 
        private FbxNode CreateMarker(string name)
        {
            FbxMarker marker = FbxMarker.Create(SdkManager, name);
            FbxNode node = FbxNode.Create(SdkManager, name);
            node.NodeAttribute = marker;
            return node;
        }
        private FbxNode CreateCamera(string name)
        {
            FbxCamera camera = FbxCamera.Create(SdkManager, name);

            // Set camera property for a classic TV projection with aspect ratio 4:3
            camera.Format = FbxCamera.CameraFormat.Ntsc;

            FbxNode node = FbxNode.Create(SdkManager, name);

            node.NodeAttribute = camera;

            return node;
        }
        private void SetCameraPointOfInterest(FbxNode camera, FbxNode pointOfInterest)
        {
            // Set the camera to always point at this node.
            camera.Target = pointOfInterest;
        }
        private void SetMarkerDefaultPosition(FbxNode marker)
        {
            //The marker is positioned above the origin. There is no rotation and no scaling.
            FbxDouble3 d = new FbxDouble3(0.0, 40.0, 0.0);
            marker.LclTranslation.Set(d);

            d.Y = 0.0;
            marker.LclRotation.Set(d);

            d.X = 1.0; d.Y = 1.0; d.Z = 1.0;
            marker.LclScaling.Set(d);
        }

        // Compute the camera position.
        private void SetCameraDefaultPosition(FbxNode camera)
        {
            // set the initial camera position
            FbxDouble3 cameraLocation = new FbxDouble3(0.0, 200.0, -100.0);
            camera.LclTranslation.Set(cameraLocation);
        }

        // The camera move on X and Y axis.
        private void AnimateCamera(FbxNode camera, string takeName)
        {
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            camera.CreateTakeNode(takeName);
            camera.SetCurrentTakeNode(takeName);
            camera.LclTranslation.GetKFCurveNode(true, takeName);

            // X translation.
            curve = camera.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_X, takeName);
            if (curve != null && curve.IsValid)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear,
                    FbxCurveKey.KeyTangentMode.TangentAll, 0, 0, FbxCurveKey.KeyTangentWeightMode.None,
                    FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT, FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT,
                    FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY, FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY);

                time.SecondDouble = 20;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 500.0f, FbxCurveKey.KeyInterpolation.Linear,
                    FbxCurveKey.KeyTangentMode.TangentAll, 0, 0, FbxCurveKey.KeyTangentWeightMode.None,
                    FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT, FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT,
                    FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY, FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY);

                curve.KeyModifyEnd();
            }

            //// Y translation.
            curve = camera.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Y, takeName);
            if (curve != null && curve.IsValid)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear,
                    FbxCurveKey.KeyTangentMode.TangentAuto, 0, 0, FbxCurveKey.KeyTangentWeightMode.None,
                    FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT, FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT,
                    FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY, FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY);

                time.SecondDouble = 20;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 800.0f, FbxCurveKey.KeyInterpolation.Linear,
                    FbxCurveKey.KeyTangentMode.TangentAuto, 0, 0, FbxCurveKey.KeyTangentWeightMode.None,
                    FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT, FbxCurveKey.FBXCURVE_DEFAULT_WEIGHT,
                    FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY, FbxCurveKey.FBXCURVE_DEFAULT_VELOCITY);

                curve.KeyModifyEnd();
            }
        }

        private void AnimateCube(FbxNode cube, string takeName, int rotationAxis)
        {
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            cube.CreateTakeNode(takeName);
            cube.SetCurrentTakeNode(takeName);
            cube.LclRotation.GetKFCurveNode(true, takeName);

            if (rotationAxis == 0) curve = cube.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_X, takeName);
            else if (rotationAxis == 1) curve = cube.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Y, takeName);
            else if (rotationAxis == 2) curve = cube.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Z, takeName);

            if (curve != null && curve.IsValid)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 20.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, -3500f, FbxCurveKey.KeyInterpolation.Linear);
                curve.KeyModifyEnd();
            }
        }

        private FbxNode CreateCubeMesh(string cubeName)
        {
            int i, j;
            FbxMesh mesh = FbxMesh.Create(SdkManager, cubeName);

            FbxVector4 controlPoint0 = new FbxVector4(-50, 0, 50);
            FbxVector4 controlPoint1 = new FbxVector4(50, 0, 50);
            FbxVector4 controlPoint2 = new FbxVector4(50, 100, 50);
            FbxVector4 controlPoint3 = new FbxVector4(-50, 100, 50);
            FbxVector4 controlPoint4 = new FbxVector4(-50, 0, -50);
            FbxVector4 controlPoint5 = new FbxVector4(50, 0, -50);
            FbxVector4 controlPoint6 = new FbxVector4(50, 100, -50);
            FbxVector4 controlPoint7 = new FbxVector4(-50, 100, -50);

            FbxVector4 normalXPos = new FbxVector4(1, 0, 0);
            FbxVector4 normalXNeg = new FbxVector4(-1, 0, 0);
            FbxVector4 normalYPos = new FbxVector4(0, 1, 0);
            FbxVector4 normalYNeg = new FbxVector4(0, -1, 0);
            FbxVector4 normalZPos = new FbxVector4(0, 0, 1);
            FbxVector4 normalZNeg = new FbxVector4(0, 0, -1);

            // Create control points.
            mesh.InitControlPoints(24);
            FbxVector4[] controlPoints = new FbxVector4[mesh.ControlPointsCount];

            controlPoints[0] = controlPoint0;
            controlPoints[1] = controlPoint1;
            controlPoints[2] = controlPoint2;
            controlPoints[3] = controlPoint3;
            controlPoints[4] = controlPoint1;
            controlPoints[5] = controlPoint5;
            controlPoints[6] = controlPoint6;
            controlPoints[7] = controlPoint2;
            controlPoints[8] = controlPoint5;
            controlPoints[9] = controlPoint4;
            controlPoints[10] = controlPoint7;
            controlPoints[11] = controlPoint6;
            controlPoints[12] = controlPoint4;
            controlPoints[13] = controlPoint0;
            controlPoints[14] = controlPoint3;
            controlPoints[15] = controlPoint7;
            controlPoints[16] = controlPoint3;
            controlPoints[17] = controlPoint2;
            controlPoints[18] = controlPoint6;
            controlPoints[19] = controlPoint7;
            controlPoints[20] = controlPoint1;
            controlPoints[21] = controlPoint0;
            controlPoints[22] = controlPoint4;
            controlPoints[23] = controlPoint5;

            mesh.ControlPoints = controlPoints;

            // Set the normals on Layer 0.
            FbxLayer layer = mesh.GetLayer(0);
            if (layer == null)
            {
                mesh.CreateLayer();
                layer = mesh.GetLayer(0);
            }

            // We want to have one normal for each vertex (or control point),
            // so we set the mapping mode to eBY_CONTROL_POINT.
            FbxLayerElementNormal layerElementNormal = FbxLayerElementNormal.Create(mesh, "");

            layerElementNormal.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;

            // Set the normal values for every control point.
            layerElementNormal.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            FbxLayerElementArrayTemplateVector4 directArray = layerElementNormal.DirectArray;

            directArray.Add(normalZPos);
            directArray.Add(normalZPos);
            directArray.Add(normalZPos);
            directArray.Add(normalZPos);
            directArray.Add(normalXPos);
            directArray.Add(normalXPos);
            directArray.Add(normalXPos);
            directArray.Add(normalXPos);
            directArray.Add(normalZNeg);
            directArray.Add(normalZNeg);
            directArray.Add(normalZNeg);
            directArray.Add(normalZNeg);
            directArray.Add(normalXNeg);
            directArray.Add(normalXNeg);
            directArray.Add(normalXNeg);
            directArray.Add(normalXNeg);
            directArray.Add(normalYPos);
            directArray.Add(normalYPos);
            directArray.Add(normalYPos);
            directArray.Add(normalYPos);
            directArray.Add(normalYNeg);
            directArray.Add(normalYNeg);
            directArray.Add(normalYNeg);
            directArray.Add(normalYNeg);
            
            layer.Normals = layerElementNormal;

            // Array of polygon vertices.
            int[] polygonVertices = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,12, 13, 
        14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };


            // Create UV for Diffuse channel.
            FbxLayerElementUV UVDiffuseLayer = FbxLayerElementUV.Create(mesh, "DiffuseUV");
            UVDiffuseLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            UVDiffuseLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.SetUVs(UVDiffuseLayer, FbxLayerElement.LayerElementType.DiffuseTextures);

            FbxVector2 vectors0 = new FbxVector2(0, 0);
            FbxVector2 vectors1 = new FbxVector2(1, 0);
            FbxVector2 vectors2 = new FbxVector2(1, 1);
            FbxVector2 vectors3 = new FbxVector2(0, 1);

            UVDiffuseLayer.DirectArray.Add(vectors0);
            UVDiffuseLayer.DirectArray.Add(vectors1);
            UVDiffuseLayer.DirectArray.Add(vectors2);
            UVDiffuseLayer.DirectArray.Add(vectors3);

            //Now we have set the UVs as eINDEX_TO_DIRECT reference and in eBY_POLYGON_VERTEX  mapping mode
            //we must update the size of the index array.
            UVDiffuseLayer.IndexArray.Count = 24;

            //Create polygons. Assign texture and texture UV indices.
            for (i = 0; i < 6; i++)
            {
                // all faces of the cube have the same texture
                mesh.BeginPolygon(-1, -1, -1, false);

                for (j = 0; j < 4; j++)
                {
                    // Control point index
                    mesh.AddPolygon(polygonVertices[i * 4 + j]);

                    // update the index array of the UVs that map the texture to the face
                    UVDiffuseLayer.IndexArray.SetAt(i * 4 + j, j);
                }

                mesh.EndPolygon();
            }

            // create a KFbxNode
            FbxNode node = FbxNode.Create(SdkManager, cubeName);

            // set the node attribute
            node.NodeAttribute = mesh;

            // set the shading mode to view texture
            node.Shading_Mode = FbxNode.ShadingMode.TextureShading;

            // rescale the cube
            node.LclScaling.Set(new FbxDouble3(0.3, 0.3, 0.3));

            // return the KFbxNode
            return node;
        }

        private void CreateTexture()
        {
            Texture = FbxTexture.Create(SdkManager, "Diffuse Texture");

            // Resource file must be in the application's directory.
            String texPath = AppPath + "\\Crate.jpg";

            // Set texture properties.
            Texture.SetFileName(texPath);
            Texture.TextureUseType = FbxTexture.TextureUse.Standard;
            Texture.Mapping = FbxTexture.MappingType.Uv;
            Texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            Texture.SwapUV = false;
            Texture.SetTranslation(0.0, 0.0);
            Texture.SetScale(1.0, 1.0);
            Texture.SetRotation(0.0, 0.0);
        }

        private void AddTexture(FbxMesh mesh)
        {

            // Set texture mapping for diffuse channel.
            FbxLayerElementTexture textureDiffuseLayer = FbxLayerElementTexture.Create(mesh, "Diffuse Texture");
            textureDiffuseLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            textureDiffuseLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            FbxLayer layer = mesh.GetLayer(0);
            if (layer != null)
                layer.DiffuseTextures = textureDiffuseLayer;
            else
                return;

            // We are in eBY_POLYGON, so there's only need for 6 index (a cube has 6 polygons).
            textureDiffuseLayer.IndexArray.Count = 6;

            // The Layer 0 and the KFbxLayerElementTexture has already been created
            // in the CreateCube function.
            layer.DiffuseTextures.DirectArray.Add(Texture);

            // Set the Index 0 to 6 to the texture in position 0 of the direct array.
            for (int i = 0; i < 6; ++i)
                layer.DiffuseTextures.IndexArray.SetAt(i, 0);
        }

        // Create global material for cube.
        private void CreateMaterial()
        {
            string materialName = "material";
            string shadingName = "Phong";
            FbxDouble3 black = new FbxDouble3(0.0, 1.0, 0.0);
            FbxDouble3 red = new FbxDouble3(1.0, 0.0, 0.0);
            FbxDouble3 diffuseColor = new FbxDouble3(0.75, 0.75, 0.0);
            Material = FbxSurfacePhong.Create(SdkManager, materialName);

            // Generate primary and secondary colors.
            Material.EmissiveColor = black;
            Material.AmbientColor = red;
            Material.DiffuseColor = diffuseColor;
            Material.TransparencyFactor = 40.5;
            Material.ShadingModel = shadingName;
            Material.Shininess = 0.5;
        }

        private void AddMaterials(FbxMesh mesh)
        {

            // Set material mapping.
            FbxLayerElementMaterial materialLayer = FbxLayerElementMaterial.Create(mesh, "");
            materialLayer.Mapping_Mode  = FbxLayerElement.MappingMode.ByPolygon;
            materialLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            FbxLayer layer = mesh.GetLayer(0);
            if (layer != null)
                layer.Materials = materialLayer;
            else
                return;

            //get the node of mesh, add material for it.
            FbxNode node = mesh.Node;
            if (node == null)
                return;
            node.AddMaterial(Material);

            // We are in eBY_POLYGON, so there's only need for 6 index (a cube has 6 polygons).
            materialLayer.IndexArray.Count = 6;

            // Set the Index 0 to 6 to the material in position 0 of the direct array.
            for (int i = 0; i < 6; ++i)
                materialLayer.IndexArray.SetAt(i, 0);

        }

        private void SetInitialCubeData()
        {
            CubeNumber = 1;     // Cube Number
            CubeRotationAxis = 1;     // Cube Rotation Axis 0==X, 1==Y, 2==Z
            CubeXPos = 0.0;   // initial CubXPos
            CubeYPos = 20.0;  // initial CubeYPos
            CubeZPos = 0.0;   // initial CubeZPos
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

        // to get a string from the node visibility value
        public string GetNodeVisibility(FbxNode node)
        {
            return "Visibility: " + ((node.Visible) ? "Yes" : "No").ToString();
        }
    }
}
