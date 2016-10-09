using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

using MyFbxObject = Skill.FbxSDK.FbxGenericNode;
using MyFbxMesh = Skill.FbxSDK.FbxMesh;
using System.IO;

namespace ExportScene03
{
    class Program
    {

        const string SAMPLE_FILENAME_MC = "ExportScene03_MC.fbx";
        const string SAMPLE_FILENAME_PC2 = "ExportScene03_PC2.fbx";
        const int PID_MY_LAYERELEMENT = 0;

        const bool ExportVertexCacheMCFormat = true;

        static void Main(string[] args)
        {
            FbxSdkManager sdkManager = null;
            FbxScene scene = null;
            string sampleFileName = null;
            bool result;

            // Prepare the FBX SDK.
            InitializeSdkObjects(out sdkManager, out scene);

            // The example can take an output file name as an argument.
            if (args.Length > 1)
            {
                sampleFileName = args[1];
            }
            // A default output file name is given otherwise.
            else
            {
                sampleFileName = ExportVertexCacheMCFormat ? SAMPLE_FILENAME_MC : SAMPLE_FILENAME_PC2;
            }

            // Create the scene.
            result = CreateScene(sdkManager, scene, sampleFileName);

            if (result == false)
            {
                Console.Write("\n\nAn error occured while creating the scene...\n");
                DestroySdkObjects(sdkManager);
                return;
            }

            // Save the scene.
            result = SaveScene(sdkManager, scene, sampleFileName, (int)Skill.FbxSDK.IO.FileFormat.FbxBinary, false);

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

        static bool CreateScene(FbxSdkManager sdkManager, FbxScene scene, string sampleFileName)
        {
            FbxNode cube = CreateCubeWithTexture(sdkManager, "Cube");
            FbxNode pyramid = CreatePyramidWithMaterials(sdkManager, "Pyramid");
            FbxNode triangle = CreateTriangle(sdkManager, "Triangle");
            FbxNode myKFbxMeshCube = CreateCubeWithMaterialAndMyKFbxMesh(sdkManager, "CubeMyKFbxMesh");
            MyFbxObject myFbxObject = MyFbxObject.Create(sdkManager, "MyFbxObject 1");

            MapShapeOnPyramid(sdkManager, pyramid);
            MapVertexCacheOnTriangle(sdkManager, triangle, sampleFileName);

            SetCubeDefaultPosition(cube);
            SetPyramidDefaultPosition(pyramid);
            SetTriangleDefaultPosition(triangle);
            SetMyKFbxMeshCubeDefaultPosition(myKFbxMeshCube);

            Animate(cube);
            Animate(pyramid);
            Animate(myKFbxMeshCube);
            AnimateVertexCacheOnTriangle(triangle, FbxTime.GetFrameRate(scene.GlobalTimeSettings.Mode));

            // Build the node tree.
            FbxNode rootNode = scene.RootNode;
            rootNode.AddChild(cube);
            rootNode.AddChild(pyramid);
            rootNode.AddChild(myKFbxMeshCube);
            rootNode.AddChild(triangle);
            rootNode.ConnectSrcObject(myFbxObject, FbxConnectionType.ConnectionDefault);

            // Identify current take when file is loaded.
            scene.SetCurrentTake("Show all faces");

            //Create a simple animated fcurve
            FbxProperty myProperty = myFbxObject.FindProperty("MyAnimatedPropertyName");
            if (myProperty.IsValid)
            {
                myProperty.Set(0.0);	//Default value
                myProperty.ModifyFlag(FbxPropertyFlags.FbxPropertyFlagsType.AnimaTable, true);
                myProperty.CreateKFCurveNode();
                FbxCurve myFCurve = myProperty.GetKFCurve();
                if (myFCurve != null)
                {
                    myFCurve.KeyAppendFast(FbxTime.Zero, -100);
                    myFCurve.KeyAppendFast(new FbxTime(100), 0);
                    myFCurve.KeyAppendFast(new FbxTime(200), 100);
                }
            }

            return true;
        }

        // Create a cube with a texture. 
        static FbxNode CreateCubeWithTexture(FbxSdkManager sdkManager, string name)
        {
            int i, j;
            FbxMesh mesh = FbxMesh.Create(sdkManager, name);

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

            // Here are two different ways to set the normal values.
            bool firstWayNormalCalculations = true;
            if (firstWayNormalCalculations)
            {
                // The first method is to set the actual normal value
                // for every control point.
                layerElementNormal.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

                layerElementNormal.DirectArray.Add(normalZPos);
                layerElementNormal.DirectArray.Add(normalZPos);
                layerElementNormal.DirectArray.Add(normalZPos);
                layerElementNormal.DirectArray.Add(normalZPos);
                layerElementNormal.DirectArray.Add(normalXPos);
                layerElementNormal.DirectArray.Add(normalXPos);
                layerElementNormal.DirectArray.Add(normalXPos);
                layerElementNormal.DirectArray.Add(normalXPos);
                layerElementNormal.DirectArray.Add(normalZNeg);
                layerElementNormal.DirectArray.Add(normalZNeg);
                layerElementNormal.DirectArray.Add(normalZNeg);
                layerElementNormal.DirectArray.Add(normalZNeg);
                layerElementNormal.DirectArray.Add(normalXNeg);
                layerElementNormal.DirectArray.Add(normalXNeg);
                layerElementNormal.DirectArray.Add(normalXNeg);
                layerElementNormal.DirectArray.Add(normalXNeg);
                layerElementNormal.DirectArray.Add(normalYPos);
                layerElementNormal.DirectArray.Add(normalYPos);
                layerElementNormal.DirectArray.Add(normalYPos);
                layerElementNormal.DirectArray.Add(normalYPos);
                layerElementNormal.DirectArray.Add(normalYNeg);
                layerElementNormal.DirectArray.Add(normalYNeg);
                layerElementNormal.DirectArray.Add(normalYNeg);
                layerElementNormal.DirectArray.Add(normalYNeg);
            }
            else
            {
                // The second method is to the possible values of the normals
                // in the direct array, and set the index of that value
                // in the index array for every control point.
                layerElementNormal.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;

                // Add the 6 different normals to the direct array
                layerElementNormal.DirectArray.Add(normalZPos);
                layerElementNormal.DirectArray.Add(normalXPos);
                layerElementNormal.DirectArray.Add(normalZNeg);
                layerElementNormal.DirectArray.Add(normalXNeg);
                layerElementNormal.DirectArray.Add(normalYPos);
                layerElementNormal.DirectArray.Add(normalYNeg);

                // Now for each control point, we need to specify which normal to use
                layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
                layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
                layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
                layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
                layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
                layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
                layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
                layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
                layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
                layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
                layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
                layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
                layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
                layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
                layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
                layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
                layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
                layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
                layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
                layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
                layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
                layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
                layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
                layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
            }

            layer.Normals = layerElementNormal;

            // Array of polygon vertices.
            int[] polygonVertices = { 0, 1, 2, 3,
		                        4, 5, 6, 7,
								8, 9, 10, 11,
								12, 13, 14, 15,
								16, 17, 18, 19,
								20, 21, 22, 23 };

            // Set texture mapping for diffuse channel.
            FbxLayerElementTexture textureDiffuseLayer = FbxLayerElementTexture.Create(mesh, "Diffuse Texture");
            textureDiffuseLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            textureDiffuseLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.DiffuseTextures = textureDiffuseLayer;

            // Set texture mapping for ambient channel.
            FbxLayerElementTexture textureAmbientLayer = FbxLayerElementTexture.Create(mesh, "Ambient Textures");
            textureAmbientLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            textureAmbientLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.AmbientTextures = textureAmbientLayer;

            // Set texture mapping for emissive channel.
            FbxLayerElementTexture textureEmissiveLayer = FbxLayerElementTexture.Create(mesh, "Emissive Textures");
            textureEmissiveLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            textureEmissiveLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.EmissiveTextures = textureEmissiveLayer;


            // Create UV for Diffuse channel
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


            // Create UV for Ambient channel
            FbxLayerElementUV UVAmbientLayer = FbxLayerElementUV.Create(mesh, "AmbientUV");

            UVAmbientLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            UVAmbientLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.SetUVs(UVAmbientLayer, FbxLayerElement.LayerElementType.AmbientTextures);

            vectors0.Set(0, 0);
            vectors1.Set(1, 0);
            vectors2.Set(0, 0.418586879968643);
            vectors3.Set(1, 0.418586879968643);

            UVAmbientLayer.DirectArray.Add(vectors0);
            UVAmbientLayer.DirectArray.Add(vectors1);
            UVAmbientLayer.DirectArray.Add(vectors2);
            UVAmbientLayer.DirectArray.Add(vectors3);

            // Create UV for Emissive channel
            FbxLayerElementUV UVEmissiveLayer = FbxLayerElementUV.Create(mesh, "EmissiveUV");

            UVEmissiveLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            UVEmissiveLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.SetUVs(UVEmissiveLayer, FbxLayerElement.LayerElementType.EmissiveTextures);

            vectors0.Set(0.2343, 0);
            vectors1.Set(1, 0.555);
            vectors2.Set(0.333, 0.999);
            vectors3.Set(0.555, 0.666);

            UVEmissiveLayer.DirectArray.Add(vectors0);
            UVEmissiveLayer.DirectArray.Add(vectors1);
            UVEmissiveLayer.DirectArray.Add(vectors2);
            UVEmissiveLayer.DirectArray.Add(vectors3);

            //Now we have set the UVs as eINDEX_TO_DIRECT reference and in eBY_POLYGON_VERTEX  mapping mode
            //we must update the size of the index array.
            UVDiffuseLayer.IndexArray.Count = 24;
            UVEmissiveLayer.IndexArray.Count = 24;
            UVAmbientLayer.IndexArray.Count = 24;

            //in the same way we with Textures, but we are in eBY_POLYGON and as we are doing a cube,
            //we should have 6 polygons (1 for each faces of the cube)
            textureDiffuseLayer.IndexArray.Count = 6;
            textureAmbientLayer.IndexArray.Count = 6;
            textureEmissiveLayer.IndexArray.Count = 6;

            // Create polygons. Assign texture and texture UV indices.
            for (i = 0; i < 6; i++)
            {
                //we won't use the default way of assigning textures, as we have
                //textures on more than just the default (diffuse) channel.
                mesh.BeginPolygon(-1, -1, -1, false);

                //Here we set the the index array for each channel
                textureDiffuseLayer.IndexArray.SetAt(i, 0);
                textureAmbientLayer.IndexArray.SetAt(i, 0);
                textureEmissiveLayer.IndexArray.SetAt(i, 0);

                for (j = 0; j < 4; j++)
                {
                    //this function points 
                    mesh.AddPolygon(polygonVertices[i * 4 + j] // Control point index. 
                        );
                    //Now we have to update the index array of the UVs for diffuse, ambient and emissive
                    UVDiffuseLayer.IndexArray.SetAt(i * 4 + j, j);
                    UVAmbientLayer.IndexArray.SetAt(i * 4 + j, j);
                    UVEmissiveLayer.IndexArray.SetAt(i * 4 + j, j);

                }

                mesh.EndPolygon();
            }

            CreateTexture(sdkManager, mesh);

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = mesh;
            node.Shading_Mode = FbxNode.ShadingMode.TextureShading;

            return node;
        }

        // Create a pyramid with materials. 
        static FbxNode CreatePyramidWithMaterials(FbxSdkManager sdkManager, string name)
        {
            int i, j;
            FbxMesh mesh = FbxMesh.Create(sdkManager, name);

            FbxVector4 controlPoint0 = new FbxVector4(-50, 0, 50);
            FbxVector4 controlPoint1 = new FbxVector4(50, 0, 50);
            FbxVector4 controlPoint2 = new FbxVector4(50, 0, -50);
            FbxVector4 controlPoint3 = new FbxVector4(-50, 0, -50);
            FbxVector4 controlPoint4 = new FbxVector4(0, 100, 0);

            FbxVector4 normalP0 = new FbxVector4(0, 1, 0);
            FbxVector4 normalP1 = new FbxVector4(0, 0.447, 0.894);
            FbxVector4 normalP2 = new FbxVector4(0.894, 0.447, 0);
            FbxVector4 normalP3 = new FbxVector4(0, 0.447, -0.894);
            FbxVector4 normalP4 = new FbxVector4(-0.894, 0.447, 0);

            // Create control points.
            mesh.InitControlPoints(16);
            FbxVector4[] controlPoints = new FbxVector4[mesh.ControlPointsCount];

            controlPoints[0] = controlPoint0;
            controlPoints[1] = controlPoint1;
            controlPoints[2] = controlPoint2;
            controlPoints[3] = controlPoint3;
            controlPoints[4] = controlPoint0;
            controlPoints[5] = controlPoint1;
            controlPoints[6] = controlPoint4;
            controlPoints[7] = controlPoint1;
            controlPoints[8] = controlPoint2;
            controlPoints[9] = controlPoint4;
            controlPoints[10] = controlPoint2;
            controlPoints[11] = controlPoint3;
            controlPoints[12] = controlPoint4;
            controlPoints[13] = controlPoint3;
            controlPoints[14] = controlPoint0;
            controlPoints[15] = controlPoint4;

            mesh.ControlPoints = controlPoints;

            // specify normals per control point.
            FbxLayer layer = mesh.GetLayer(0);
            if (layer == null)
            {
                mesh.CreateLayer();
                layer = mesh.GetLayer(0);
            }

            FbxLayerElementNormal normalLayer = FbxLayerElementNormal.Create(mesh, "");
            normalLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;
            normalLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP4);
            normalLayer.DirectArray.Add(normalP4);
            normalLayer.DirectArray.Add(normalP4);

            layer.Normals = normalLayer;

            // Array of polygon vertices.
            int[] polygonVertices = { 0, 3, 2, 1,
                                        4, 5, 6,
                                        7, 8, 9,
                                        10, 11, 12,
                                        13, 14, 15 };

            // Set material mapping.
            FbxLayerElementMaterial materialLayer = FbxLayerElementMaterial.Create(mesh, "");
            materialLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            materialLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.Materials = materialLayer;

            // Create polygons. Assign material indices.

            // Pyramid base.
            mesh.BeginPolygon(0, -1, -1, false); // Material index.

            for (j = 0; j < 4; j++)
            {
                mesh.AddPolygon(polygonVertices[j]); // Control point index.
            }

            mesh.EndPolygon();

            // Pyramid sides.
            for (i = 1; i < 5; i++)
            {
                mesh.BeginPolygon(i, -1, -1, false); // Material index.

                for (j = 0; j < 3; j++)
                {
                    mesh.AddPolygon(polygonVertices[4 + 3 * (i - 1) + j]); // Control point index.
                }

                mesh.EndPolygon();
            }


            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = mesh;

            CreateMaterials(sdkManager, mesh);

            return node;
        }

        static FbxNode CreateTriangle(FbxSdkManager sdkManager, string name)
        {
            FbxMesh mesh = FbxMesh.Create(sdkManager, name);

            // The three vertices
            FbxVector4 controlPoint0 = new FbxVector4(-50, 0, 50);
            FbxVector4 controlPoint1 = new FbxVector4(50, 0, 50);
            FbxVector4 controlPoint2 = new FbxVector4(0, 50, -50);

            // Create control points.
            mesh.InitControlPoints(3);
            FbxVector4[] controlPoints = new FbxVector4[mesh.ControlPointsCount];

            controlPoints[0] = controlPoint0;
            controlPoints[1] = controlPoint1;
            controlPoints[2] = controlPoint2;

            mesh.ControlPoints = controlPoints;

            // Create the triangle's polygon
            mesh.BeginPolygon();
            mesh.AddPolygon(0); // Control point 0
            mesh.AddPolygon(1); // Control point 1
            mesh.AddPolygon(2); // Control point 2
            mesh.EndPolygon();

            FbxNode node = FbxNode.Create(sdkManager, name);
            node.NodeAttribute = mesh;

            return node;
        }

        static FbxNode CreateCubeWithMaterialAndMyKFbxMesh(FbxSdkManager sdkManager, string name)
        {
            int i, j;

            //create a cube with our newly created class
            MyFbxMesh myKFbxMesh = MyFbxMesh.Create(sdkManager, name);
            FbxDouble3 vector3 = new FbxDouble3(0.1, 0.2, 0.3);
            FbxDouble4 vector4 = new FbxDouble4(0.1, 0.2, 0.3, 0.4);
            FbxDouble4 vector41 = new FbxDouble4(1.1, 1.2, 1.3, 1.4);
            FbxDouble4 vector42 = new FbxDouble4(2.1, 2.2, 2.3, 2.4);
            FbxDouble4 vector43 = new FbxDouble4(3.1, 3.2, 3.3, 3.4);
            FbxDouble4[] matrix = new FbxDouble4[4];
            matrix[0] = vector4;
            matrix[1] = vector41;
            matrix[2] = vector42;
            matrix[3] = vector43;

            FbxColor green = new FbxColor(0.0, 0.0, 1.0);

            FbxTime time = new FbxTime(333);
            //Set user-specific properties of our classes
            string str = "My Property 5 Value";
            //myKFbxMesh.Property((int) MyKFbxMesh::eMY_PROPERTY1).Set(true);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY2).Set((int) 1);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY3).Set((float)2.2);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY4).Set((double)3.3);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY5).Set(lString);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY6).Set(vector3);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY6).Set(lGreen);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY8).Set(vector4);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY9).Set(lMatrix);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY10).Set(3);
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY10).AddEnumValue("AAA");
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY10).AddEnumValue("BBB");
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY10).AddEnumValue("CCC");
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY10).AddEnumValue("DDD");
            //myKFbxMesh.GetProperty((int) MyKFbxMesh::eMY_PROPERTY11).Set(lTime);

            FbxVector4 controlPoint0 = new FbxVector4(-25, 0, 25);
            FbxVector4 controlPoint1 = new FbxVector4(25, 0, 25);
            FbxVector4 controlPoint2 = new FbxVector4(25, 50, 25);
            FbxVector4 controlPoint3 = new FbxVector4(-25, 50, 25);
            FbxVector4 controlPoint4 = new FbxVector4(-25, 0, -25);
            FbxVector4 controlPoint5 = new FbxVector4(25, 0, -25);
            FbxVector4 controlPoint6 = new FbxVector4(25, 50, -25);
            FbxVector4 controlPoint7 = new FbxVector4(-25, 50, -25);

            FbxVector4 normalXPos = new FbxVector4(1, 0, 0);
            FbxVector4 normalXNeg = new FbxVector4(-1, 0, 0);
            FbxVector4 normalYPos = new FbxVector4(0, 1, 0);
            FbxVector4 normalYNeg = new FbxVector4(0, -1, 0);
            FbxVector4 normalZPos = new FbxVector4(0, 0, 1);
            FbxVector4 normalZNeg = new FbxVector4(0, 0, -1);

            // Create control points.
            myKFbxMesh.InitControlPoints(24);
            FbxVector4[] controlPoints = new FbxVector4[myKFbxMesh.ControlPointsCount];

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

            myKFbxMesh.ControlPoints = controlPoints;

            // Set the normals on Layer 0.
            FbxLayer layer = myKFbxMesh.GetLayer(0);
            if (layer == null)
            {
                myKFbxMesh.CreateLayer();
                layer = myKFbxMesh.GetLayer(0);
            }

            // We want to have one normal for each vertex (or control point),
            // so we set the mapping mode to eBY_CONTROL_POINT.
            FbxLayerElementNormal layerElementNormal = FbxLayerElementNormal.Create(myKFbxMesh, "");
            layerElementNormal.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;


            // The second method is to the possible values of the normals
            // in the direct array, and set the index of that value
            // in the index array for every control point.
            layerElementNormal.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;

            // Add the 6 different normals to the direct array
            layerElementNormal.DirectArray.Add(normalZPos);
            layerElementNormal.DirectArray.Add(normalXPos);
            layerElementNormal.DirectArray.Add(normalZNeg);
            layerElementNormal.DirectArray.Add(normalXNeg);
            layerElementNormal.DirectArray.Add(normalYPos);
            layerElementNormal.DirectArray.Add(normalYNeg);

            // Now for each control point, we need to specify which normal to use
            layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
            layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
            layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
            layerElementNormal.IndexArray.Add(0); // index of normalZPos in the direct array.
            layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
            layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
            layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
            layerElementNormal.IndexArray.Add(1); // index of normalXPos in the direct array.
            layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
            layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
            layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
            layerElementNormal.IndexArray.Add(2); // index of normalZNeg in the direct array.
            layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
            layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
            layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
            layerElementNormal.IndexArray.Add(3); // index of normalXNeg in the direct array.
            layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
            layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
            layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
            layerElementNormal.IndexArray.Add(4); // index of normalYPos in the direct array.
            layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
            layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
            layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.
            layerElementNormal.IndexArray.Add(5); // index of normalYNeg in the direct array.

            layer.Normals = layerElementNormal;

            // Array of polygon vertices.
            int[] polygonVertices = { 0, 1, 2, 3,
                                        4, 5, 6, 7,
                                        8, 9, 10, 11,
                                        12, 13, 14, 15,
                                        16, 17, 18, 19,
                                        20, 21, 22, 23 };

            // Set material mapping.
            FbxLayerElementMaterial materialLayer = FbxLayerElementMaterial.Create(myKFbxMesh, "");
            materialLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            materialLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.Materials = materialLayer;

            // Create UV coordinates.
            FbxLayerElementUV UVLayer = FbxLayerElementUV.Create(myKFbxMesh, "");

            UVLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            UVLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.SetUVs(UVLayer, FbxLayerElement.LayerElementType.DiffuseTextures);

            FbxVector2 vectors0 = new FbxVector2(0, 0);
            FbxVector2 vectors1 = new FbxVector2(1, 0);
            FbxVector2 vectors2 = new FbxVector2(1, 1);
            FbxVector2 vectors3 = new FbxVector2(0, 1);

            UVLayer.DirectArray.Add(vectors0);
            UVLayer.DirectArray.Add(vectors1);
            UVLayer.DirectArray.Add(vectors2);
            UVLayer.DirectArray.Add(vectors3);

            for (i = 0; i < 6; i++)
            {
                //we created 6 lambert materials in the material layer of MyKFbxMesh
                //make each face use a different one
                myKFbxMesh.BeginPolygon(i, -1, -1, false);

                for (j = 0; j < 4; j++)
                {
                    myKFbxMesh.AddPolygon(polygonVertices[i * 4 + j], // Control point index. 
                                      j); // Valid texture UV index since texture UV mapping is by polygon vertex.
                }

                myKFbxMesh.EndPolygon();
            }


            //Add a User Data Layer Element
            //As of now, the types supported by a User Data Layer Element are: DTBool, DTInteger, DTFloat and DTDouble

            //For this example, we will create a layer element which possess 1 float and 1 bool

            //create a template array of KFbxDataTypes
            //KArrayTemplate<KFbxDataType> lArrayType;

            ////Create a template array of const char*
            //KArrayTemplate<const char*> lArrayNames;

            ////let's add our types and the names of each of the added types
            //lArrayType.Add(DTFloat);
            //lArrayNames.Add("My Float");

            //lArrayType.Add(DTBool);
            //lArrayNames.Add("My Bool");


            //Now we are ready to create the User Data Layer Element
            //FbxLayerElementUserData fbxLayerElementUserData = FbxLayerElementUserData.Create(myKFbxMesh, "My Layer Element",PID_MY_LAYERELEMENT,lArrayType, lArrayNames);

            ////For this example we will set the mapping mode to POLYGON_VERTEX
            //lKFbxLayerElementUserData.SetMappingMode(KFbxLayerElement::eBY_POLYGON_VERTEX);

            ////We add the layer element to layer (0)
            //lLayer.SetUserData(lKFbxLayerElementUserData);

            ////As we are using the eDirect Reference mode, and we are using polygon vertex Mapping mode
            ////we have to resize the direct array to the number of polygon vertex we have in this mesh
            //lKFbxLayerElementUserData.ResizeAllDirectArrays(myKFbxMesh.GetPolygonVertexCount());


            ////To change the values in the direct array, we simply get the array and modify what we need to
            //KFbxLayerElementArrayTemplate<void*>* directArrayF = lKFbxLayerElementUserData.GetDirectArrayVoid("My Float");
            //float *lDirectArrayFloat = NULL;
            //lDirectArrayFloat = directArrayF.GetLocked(lDirectArrayFloat);

            //KFbxLayerElementArrayTemplate<void*>* directArrayB = lKFbxLayerElementUserData.GetDirectArrayVoid("My Bool");
            //bool *lDirectArrayBool = NULL;
            //directArrayB.GetLocked(lDirectArrayBool);

            ////Modify every data for each polygon vertex on our mesh with some value
            //for(i=0; i<myKFbxMesh.GetPolygonVertexCount(); ++i)
            //{
            //    if(lDirectArrayFloat)
            //        lDirectArrayFloat[i]=(float)(i+0.5);
            //    if(lDirectArrayBool)
            //        lDirectArrayBool[i]= (i%2==0);
            //}

            //directArrayF.Release((void**)&lDirectArrayFloat);
            //directArrayB.Release((void**)&lDirectArrayBool);

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = myKFbxMesh;
            node.Shading_Mode = FbxNode.ShadingMode.TextureShading;

            //we have a layer material, let's create the materials
            //6 materials, 1 for each face of the cube
            CreateMaterialsWithMyKFbxMesh(sdkManager, myKFbxMesh);

            return node;
        }


        // Create texture for cube.
        static void CreateTexture(FbxSdkManager sdkManager, FbxMesh mesh)
        {
            FbxTexture texture = FbxTexture.Create(sdkManager, "Diffuse Texture");

            // Set texture properties.
            texture.SetFileName("scene03.jpg"); // Resource file is in current directory.
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Uv;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;
            texture.SetTranslation(0.0, 0.0);
            texture.SetScale(1.0, 1.0);
            texture.SetRotation(0.0, 0.0);

            // The Layer 0 and the KFbxLayerElementTexture has already been created
            // in the CreateCubeWithTexture function.
            mesh.GetLayer(0).DiffuseTextures.DirectArray.Add(texture);

            texture = FbxTexture.Create(sdkManager, "Ambient Texture");

            // Set texture properties.
            texture.SetFileName("gradient.jpg"); // Resource file is in current directory.
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Uv;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;
            texture.SetTranslation(0.0, 0.0);
            texture.SetScale(1.0, 1.0);
            texture.SetRotation(0.0, 0.0);

            // The Layer 0 and the KFbxLayerElementTexture has already been created
            // in the CreateCubeWithTexture function.
            mesh.GetLayer(0).GetTextures(FbxLayerElement.LayerElementType.AmbientTextures).DirectArray.Add(texture);

            texture = FbxTexture.Create(sdkManager, "Emissive Texture");

            // Set texture properties.
            texture.SetFileName("spotty.jpg"); // Resource file is in current directory.
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Uv;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;
            texture.SetTranslation(0.0, 0.0);
            texture.SetScale(1.0, 1.0);
            texture.SetRotation(0.0, 0.0);

            // The Layer 0 and the KFbxLayerElementTexture has already been created
            // in the CreateCubeWithTexture function.
            mesh.GetLayer(0).GetTextures(FbxLayerElement.LayerElementType.EmissiveTextures).DirectArray.Add(texture);
        }

        // Create materials for pyramid.
        static void CreateMaterials(FbxSdkManager sdkManager, FbxMesh mesh)
        {
            int i;

            for (i = 0; i < 5; i++)
            {
                string materialName = "material";
                string shadingName = "Phong";
                materialName += i;
                FbxDouble3 black = new FbxDouble3(0.0, 0.0, 0.0);
                FbxDouble3 red = new FbxDouble3(1.0, 0.0, 0.0);
                FbxDouble3 color = null;
                FbxSurfacePhong material = FbxSurfacePhong.Create(sdkManager, materialName);


                // Generate primary and secondary colors.
                material.EmissiveColor = black;
                material.AmbientColor = red;
                color = new FbxDouble3(i > 2 ? 1.0 : 0.0,
                                    i > 0 && i < 4 ? 1.0 : 0.0,
                                    i % 2 != 0 ? 0.0 : 1.0);
                material.DiffuseColor = color;
                material.TransparencyFactor = 0.0;
                material.ShadingModel = shadingName;
                material.Shininess = 0.5;

                //get the node of mesh, add material for it.
                FbxNode node = mesh.Node;
                if (node != null)
                    node.AddMaterial(material);
            }
        }

        static void CreateMaterialsWithMyKFbxMesh(FbxSdkManager sdkManager, MyFbxMesh myKFbxMesh)
        {
            int i;
            for (i = 0; i != 6; ++i)
            {
                string materialName = "material";
                string shadingModelName = i % 2 == 0 ? "Lambert" : "Phong";
                materialName += i;
                FbxDouble3 black = new FbxDouble3(0.0, 0.0, 0.0);
                FbxDouble3 red = new FbxDouble3(1.0, 0.0, 0.0);
                FbxDouble3 color;
                FbxSurfaceLambert material = FbxSurfaceLambert.Create(sdkManager, materialName);


                // Generate primary and secondary colors.

                material.EmissiveColor = black;
                material.AmbientColor = red;
                color = new FbxDouble3(i > 2 ? 1.0 : 0.0,
                                    i > 0 && i < 4 ? 1.0 : 0.0,
                                    i % 2 != 0 ? 0.0 : 1.0);
                material.DiffuseColor = color;
                material.TransparencyFactor = 0.0;
                material.ShadingModel = shadingModelName;

                //get the node of mesh, add material for it. 
                FbxNode node = myKFbxMesh.Node;
                if (node != null)
                    node.AddMaterial(material);

            }
        }

        // Map pyramid control points onto an upside down shape.
        static void MapShapeOnPyramid(FbxSdkManager sdkManager, FbxNode pyramid)
        {
            FbxShape shape = FbxShape.Create(sdkManager, "");

            FbxVector4 controlPoint0 = new FbxVector4(-50, 100, 50);
            FbxVector4 controlPoint1 = new FbxVector4(50, 100, 50);
            FbxVector4 controlPoint2 = new FbxVector4(50, 100, -50);
            FbxVector4 controlPoint3 = new FbxVector4(-50, 100, -50);
            FbxVector4 controlPoint4 = new FbxVector4(0, 0, 0);

            FbxVector4 normalP0 = new FbxVector4(0, 1, 0);
            FbxVector4 normalP1 = new FbxVector4(0, -0.447, 0.894);
            FbxVector4 normalP2 = new FbxVector4(0.894, -0.447, 0);
            FbxVector4 normalP3 = new FbxVector4(0, -0.447, -0.894);
            FbxVector4 normalP4 = new FbxVector4(-0.894, -0.447, 0);

            // Create control points.
            shape.InitControlPoints(16);
            FbxVector4[] controlPoints = new FbxVector4[shape.ControlPointsCount];

            controlPoints[0] = controlPoint0;
            controlPoints[1] = controlPoint1;
            controlPoints[2] = controlPoint2;
            controlPoints[3] = controlPoint3;
            controlPoints[4] = controlPoint0;
            controlPoints[5] = controlPoint1;
            controlPoints[6] = controlPoint4;
            controlPoints[7] = controlPoint1;
            controlPoints[8] = controlPoint2;
            controlPoints[9] = controlPoint4;
            controlPoints[10] = controlPoint2;
            controlPoints[11] = controlPoint3;
            controlPoints[12] = controlPoint4;
            controlPoints[13] = controlPoint3;
            controlPoints[14] = controlPoint0;
            controlPoints[15] = controlPoint4;

            shape.ControlPoints = controlPoints;
            // specify normals per control point.
            FbxLayer layer = shape.GetLayer(0);
            if (layer == null)
            {
                shape.CreateLayer();
                layer = shape.GetLayer(0);
            }

            FbxLayerElementNormal normalLayer = FbxLayerElementNormal.Create(shape, "");
            normalLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;
            normalLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP0);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP1);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP2);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP3);
            normalLayer.DirectArray.Add(normalP4);
            normalLayer.DirectArray.Add(normalP4);
            normalLayer.DirectArray.Add(normalP4);

            layer.Normals = normalLayer;

            pyramid.Mesh.AddShape(shape, "Upside down");
        }

        static void MapVertexCacheOnTriangle(FbxSdkManager sdkManager, FbxNode triangle, string sampleFileName)
        {
            // By convention, all cache files are created in a _fpc folder located at the same
            // place as the .fbx file. 
            string FBXAbsolutePath = Path.GetFullPath(sampleFileName);

            // Create a cache directory with the same name as the fbx file
            string FPCAbsoluteDirectory = "";

            FPCAbsoluteDirectory = Path.GetPathRoot(FBXAbsolutePath);
            FPCAbsoluteDirectory += "/";
            FPCAbsoluteDirectory += Path.GetFileNameWithoutExtension(sampleFileName);
            FPCAbsoluteDirectory += "_fpc";

            // Make this path the shortest possible
            //lFPCAbsoluteDirectory = KFbxCleanPath(lFPCAbsoluteDirectory);

            // Now get the point cache absolute and relative file name
            string AbsolutePCFileName = FPCAbsoluteDirectory + "/" + triangle.Name;
            AbsolutePCFileName += ExportVertexCacheMCFormat ? ".xml" : ".pc2";

            //string RelativePCFileName = FbxGetRelativeFilePath(KFbxExtractDirectory(lFBXAbsolutePath)+"/", lAbsolutePCFileName);

            //// Make sure the direcotry exist.
            //if (!KFbxEnsureDirectoryExistance(lAbsolutePCFileName))
            //{
            //    // Cannot create this directory. So do not create the point cache
            //    return;
            //}

            //
            // Create the cache file
            //
            //FbxCache cache = FbxCache.Create(sdkManager, triangle.Name);

            //cache.SetCacheFileName(RelativePCFileName, lAbsolutePCFileName);
            //cache.SetCacheFileFormat(gExportVertexCacheMCFormat ? KFbxCache::eMC : KFbxCache::ePC2);

            ////
            //// Create the vertex deformer
            ////
            //KFbxVertexCacheDeformer* lDeformer = KFbxVertexCacheDeformer::Create(pSdkManager, pTriangle.GetName());

            //lDeformer.SetCache(lCache);
            //lDeformer.SetCacheChannel(pTriangle.GetName());
            //lDeformer.SetActive(true);

            //// Apply the deformer on the mesh
            //pTriangle.GetGeometry().AddDeformer(lDeformer);
        }

        // Cube is translated to the left.
        static void SetCubeDefaultPosition(FbxNode cube)
        {
            cube.SetDefaultT(new FbxVector4(-75.0, -50.0, 0.0));
            cube.SetDefaultR(new FbxVector4(0.0, 0.0, 0.0));
            cube.SetDefaultS(new FbxVector4(1.0, 1.0, 1.0));
        }

        // Pyramid is translated to the right.
        static void SetPyramidDefaultPosition(FbxNode pyramid)
        {
            pyramid.SetDefaultT(new FbxVector4(75.0, -50.0, 0.0));
            pyramid.SetDefaultR(new FbxVector4(0.0, 0.0, 0.0));
            pyramid.SetDefaultS(new FbxVector4(1.0, 1.0, 1.0));
        }

        static void SetTriangleDefaultPosition(FbxNode triangle)
        {
            triangle.SetDefaultT(new FbxVector4(200.0, -50.0, 0.0));
            triangle.SetDefaultR(new FbxVector4(0.0, 0.0, 0.0));
            triangle.SetDefaultS(new FbxVector4(1.0, 1.0, 1.0));
        }

        static void SetMyKFbxMeshCubeDefaultPosition(FbxNode myKFbxCube)
        {
            myKFbxCube.SetDefaultT(new FbxVector4(-200.0, -25.0, 0.0));
            myKFbxCube.SetDefaultR(new FbxVector4(0.0, 0.0, 0.0));
            myKFbxCube.SetDefaultS(new FbxVector4(1.0, 1.0, 1.0));
        }

        // Displays 6 different angles.
        static void Animate(FbxNode node)
        {
            string takeName = string.Empty;
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            takeName = "Show all faces";

            node.CreateTakeNode(takeName);
            node.SetCurrentTakeNode(takeName);
            node.LclRotation.GetKFCurveNode(true, takeName);

            curve = node.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Y, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 0.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 90.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 180.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 270.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 2.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 360.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                curve.KeyModifyEnd();
            }

            curve = node.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_X, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 2.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 2.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 90.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 3.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, -90.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 4.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                curve.KeyModifyEnd();
            }


            // The upside down shape is at index 0 because it is the only one.
            // The cube has no shape so the function returns NULL is this case.
            FbxGeometry geometry = (FbxGeometry)node.NodeAttribute;
            curve = geometry.GetShapeChannel(0, true, takeName);

            if (curve != null)
            {
                curve.KeyModifyBegin();
                {
                    time.SecondDouble = 0.0;
                    keyIndex = curve.KeyAdd(time);
                    curve.KeySetValue(keyIndex, 0.0f);
                    curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                    time.SecondDouble = 2.0;
                    keyIndex = curve.KeyAdd(time);
                    curve.KeySetValue(keyIndex, 100.0f);
                    curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                    time.SecondDouble = 4.0;
                    keyIndex = curve.KeyAdd(time);
                    curve.KeySetValue(keyIndex, 0.0f);
                    curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);
                }
                curve.KeyModifyEnd();
            }
        }

        static void AnimateVertexCacheOnTriangle(FbxNode triangle, double frameRate)
        {
            //
            // Move the vertices from their original position to the center.
            //
            FbxVertexCacheDeformer deformer = (FbxVertexCacheDeformer)triangle.Geometry.GetDeformer(0, FbxDeformer.DeformerType.VertexCache);
            if (deformer == null)
                return;
            FbxCache cache = deformer.Cache;
            bool ret;

            // Write samples for 4 seconds
            FbxTime timeIncrement = new FbxTime();
            FbxTime currentTime = new FbxTime();
            FbxTime stopTime = new FbxTime();

            timeIncrement.SetTime(0, 0, 0, 1, 0, FbxTime.TimeMode.DefaultMode, 0); // 1 frame @ current frame rate
            stopTime.SetTime(0, 0, 4, 0, 0, FbxTime.TimeMode.DefaultMode, 0);         // 4 seconds

            uint frameCount = (uint)(stopTime.Get() / timeIncrement.Get());

            // Open the file for writing
            if (ExportVertexCacheMCFormat)
            {
                ret = cache.OpenFileForWrite(FbxCache.MCFileCount.OneFile, frameRate, triangle.Name);
            }
            else
            {
                ret = cache.OpenFileForWrite(0.0, frameRate, frameCount, 3);
            }

            if (!ret)
            {
                // print out the error
                Console.Write("File open error: {0}\n", cache.LastErrorString);
                return;
            }

            int channelIndex = cache.GetChannelIndex(triangle.Name);
            uint currentFrame = 0;

            while (currentTime <= stopTime)
            {
                double[,] vertices = new double[3, 3];
                double scaleFactor = 1.0 - (double)(currentTime.SecondDouble / stopTime.SecondDouble);

                vertices[0, 0] = -50.0 * scaleFactor;  // X
                vertices[0, 1] = 0.0;                   // Y
                vertices[0, 2] = 50.0 * scaleFactor;  // Z

                vertices[1, 0] = 50.0 * scaleFactor;  // X
                vertices[1, 1] = 0.0;                   // Y
                vertices[1, 2] = 50.0 * scaleFactor;  // Z

                vertices[2, 0] = 0.0 * scaleFactor;  // X
                vertices[2, 1] = 50.0 * scaleFactor;  // Y
                vertices[2, 2] = -50.0 * scaleFactor;  // Z

                //if (ExportVertexCacheMCFormat)
                //{
                //    cache.Write(channelIndex, currentTime, lVertices[0][0], 3);
                //}
                //else
                //{
                //    lCache.Write(lCurrentFrame, &lVertices[0][0]);
                //}

                //lCurrentTime += lTimeIncrement;
                //lCurrentFrame++;
            }

            if (!cache.CloseFile())
            {
                // print out the error
                Console.Write("File open error: {0}\n", cache.LastErrorString);
            }
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
