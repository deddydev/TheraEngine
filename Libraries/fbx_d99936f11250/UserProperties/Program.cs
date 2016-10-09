using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

using FbxUserProperty = Skill.FbxSDK.FbxProperty;
using DTBool = Skill.FbxSDK.FbxDataType;
using DTFloat = Skill.FbxSDK.FbxDataType;
using DTColor3 = Skill.FbxSDK.FbxDataType;
using DTInteger = Skill.FbxSDK.FbxDataType;
using DTStringList = Skill.FbxSDK.FbxDataType;
using DTDouble4 = Skill.FbxSDK.FbxDataType;

namespace UserProperties
{
    class Program
    {
        const string SAMPLE_FILENAME = "UserProperties.fbx";
        const string TAKE_ANIMATE_LIST = "Animate Cube List";
        const string TAKE_ANIMATE_CUBE = "Animate Cube";
        const string TAKE_ANIMATE_PYRAMID = "Animate Pyramid";

        static void Main(string[] args)
        {
            FbxSdkManager sdkManager = null;
            FbxScene scene = null;
            bool result;

            // Prepare the FBX SDK.
            InitializeSdkObjects(out sdkManager, out scene);

            // Create the scene.
            FbxNode cube = CreateCube(sdkManager, "Cube");
            FbxNode pyramid = CreatePyramid(sdkManager, "Pyramid");

            // Build the node tree.
            FbxNode rootNode = scene.RootNode;
            rootNode.AddChild(cube);
            rootNode.AddChild(pyramid);

            // Create the user properties on the Cube.
            CreateUserProperties(sdkManager, cube);

            // animate the list "MyList"
            FbxUserProperty p6 = cube.FindProperty("MyList", false);
            AnimateList(scene, cube, p6);

            // Constraint (position constraint) the pyramid to the cube.
            FbxConstraintPosition positionConstraint = (FbxConstraintPosition)CreatePositionConstraint(sdkManager, cube, pyramid);
            FbxStatics.FbxConnectDst(positionConstraint, scene);

            // Animate the cube: the pyramid will follow, because of the position constraint.
            AnimateCube(scene, cube);

            // Animate the pyramid: it doesn't actually move, because it is constrained to the immobile cube.
            AnimatePyramid(scene, pyramid);

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


        // Create a cube.
        static FbxNode CreateCube(FbxSdkManager sdkManager, string name)
        {
            // indices of the vertices per each polygon
            int[] vtxId = {
		0,1,2,3, // front  face  (Z+)
        1,5,6,2, // right  side  (X+)
		5,4,7,6, // back   face  (Z-)
		4,0,3,7, // left   side  (X-)
		0,4,5,1, // bottom face  (Y-)
		3,2,6,7  // top    face  (Y+)
	};

            // control points
            FbxVector4[] controlPoints = {
		new FbxVector4( -50.0,  0.0,  50.0, 1.0),
        new FbxVector4( 50.0,  0.0,  50.0, 1.0),
        new FbxVector4( 50.0,100.0,  50.0, 1.0),
        new FbxVector4( -50.0,100.0,  50.0, 1.0), 
		new FbxVector4( -50.0,  0.0, -50.0, 1.0),
        new FbxVector4( 50.0,  0.0, -50.0, 1.0),
        new FbxVector4( 50.0,100.0, -50.0, 1.0),
        new FbxVector4( -50.0,100.0, -50.0, 1.0) 
	};

            // normals
            FbxVector4[] normals = {
		new FbxVector4( -0.577350258827209,-0.577350258827209, 0.577350258827209, 1.0), 
		new FbxVector4(  0.577350258827209,-0.577350258827209, 0.577350258827209, 1.0), 
		new FbxVector4(  0.577350258827209, 0.577350258827209, 0.577350258827209, 1.0),
		new FbxVector4( -0.577350258827209, 0.577350258827209, 0.577350258827209, 1.0), 
		new FbxVector4( -0.577350258827209,-0.577350258827209,-0.577350258827209, 1.0), 
		new FbxVector4(  0.577350258827209,-0.577350258827209,-0.577350258827209, 1.0),
		new FbxVector4(  0.577350258827209, 0.577350258827209,-0.577350258827209, 1.0),
		new FbxVector4( -0.577350258827209, 0.577350258827209,-0.577350258827209, 1.0)
	};

            // uvs
            FbxVector2[] UVs = {
		new FbxVector2( 0.0, 0.0), 
		new FbxVector2( 1.0, 0.0), 
		new FbxVector2( 0.0, 1.0),
		new FbxVector2( 1.0, 1.0), 
		new FbxVector2( 0.0, 2.0),
		new FbxVector2( 1.0, 2.0),
		new FbxVector2( 0.0, 3.0),
		new FbxVector2( 1.0, 3.0),
		new FbxVector2( 0.0, 4.0),
		new FbxVector2( 1.0, 4.0),
		new FbxVector2( 2.0, 0.0),
		new FbxVector2( 2.0, 1.0),
		new FbxVector2(-1.0, 0.0),
		new FbxVector2(-1.0, 1.0)
	};

            // create the main structure.
            FbxMesh mesh = FbxMesh.Create(sdkManager, "");

            // Create control points.
            mesh.InitControlPoints(8);
            mesh.ControlPoints = controlPoints;

            // create the polygons
            int vId = 0;
            for (int f = 0; f < 6; f++)
            {
                mesh.BeginPolygon();
                for (int v = 0; v < 4; v++)
                    mesh.AddPolygon(vtxId[vId++]);
                mesh.EndPolygon();
            }

            // specify normals per control point.
            /* For compatibility, we follow the rules stated in the 
               layer class documentation: normals are defined on layer 0 and
               are assigned by control point.
            */
            FbxLayer layer = mesh.GetLayer(0);

            FbxLayerContainer layerContainer = (FbxLayerContainer)mesh;
            FbxLayerElementNormal normLayer = FbxLayerElementNormal.Create(layerContainer, "");
            normLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;
            normLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            for (int n = 0; n < 8; n++)
                normLayer.DirectArray.Add(normals[n]);

            layer.Normals = normLayer;


            // Finally we create the node containing the mesh
            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = mesh;

            return node;
        }


        // Create a pyramid.
        static FbxNode CreatePyramid(FbxSdkManager sdkManager, string name)
        {
            int i, j;
            FbxMesh mesh = FbxMesh.Create(sdkManager, "");

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

            FbxLayerContainer layerContainer = (FbxLayerContainer)mesh;
            FbxLayerElementNormal normalLayer = FbxLayerElementNormal.Create(layerContainer, "");
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

            // Create polygons.

            // Pyramid base.
            mesh.BeginPolygon();
            for (j = 0; j < 4; j++)
            {
                mesh.AddPolygon(polygonVertices[j]); // Control point index.
            }

            mesh.EndPolygon();

            // Pyramid sides.
            for (i = 1; i < 5; i++)
            {
                mesh.BeginPolygon();

                for (j = 0; j < 3; j++)
                {
                    mesh.AddPolygon(polygonVertices[4 + 3 * (i - 1) + j]); // Control point index.
                }

                mesh.EndPolygon();
            }

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = mesh;

            // Translate the pyramid
            FbxVector4 translation = new FbxVector4(-150, 0, 0, 0);
            node.SetDefaultT(translation);

            return node;
        }


        static void CreateUserProperties(FbxSdkManager sdkManager, FbxNode node)
        {
            // Now we create the user properties 
            DTBool dTBool = FbxDataType.Create("MyBooleanProperty", FbxType.Bool1);
            DTFloat dTFloat = FbxDataType.Create("MyRealProperty", FbxType.Float1);
            DTColor3 dTColor3 = FbxDataType.Create("MyColorProperty", FbxType.Double3);
            DTInteger dTInteger = FbxDataType.Create("MyInteger", FbxType.Integer1);
            DTDouble4 dTDouble4 = FbxDataType.Create("MyVector", FbxType.Double4);
            DTStringList dTStringList = FbxDataType.Create("MyList", FbxType.String);

            FbxProperty p1 = FbxProperty.Create(node, "MyBooleanProperty", dTBool, "My Bool");
            FbxProperty p2 = FbxProperty.Create(node, "MyRealProperty", dTFloat, "My floating point number");
            FbxProperty p3 = FbxProperty.Create(node, "MyColorProperty", dTColor3, "My Color");
            FbxProperty p4 = FbxProperty.Create(node, "MyInteger", dTInteger, "");
            FbxProperty p5 = FbxProperty.Create(node, "MyVector", dTDouble4, "");
            FbxProperty p6 = FbxProperty.Create(node, "MyList", dTStringList, "");

            /* 
               NOTE: The properties labels exists only while the property object is in memory.
               The label is not saved in the FBX file. When loading properties from the FBX file
               it will take the same value as the property name.
            */

            // we now fill the properties. All the properties are user properties so we set the
            // correct flag

            p1.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);
            p2.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);
            p3.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);
            p4.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);
            p5.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);
            p6.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.User, true);

            // let's make MyColorProperty, MyVector and MyList animatables
            p3.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.AnimaTable, true);
            p5.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.AnimaTable, true);
            p6.ModifyFlag(FbxUserProperty.FbxPropertyFlagsType.AnimaTable, true);

            // we set the default values
            FbxColor red = new FbxColor(1.0, 0.0, 0.0);
            p1.Set(false);
            p2.Set(3.33f);
            p3.Set(red);
            p4.Set(11);
            p5.Set(new FbxDouble3(-1.1, 2.2, -3.3));
            p6.Set(2);

            // and some limits
            p4.SetLimits(-5.0, 9.0);
            p5.SetLimits(0.0, 2.1);

            // add elements to the list
            p6.AddEnumValue("one");
            p6.AddEnumValue("two");
            p6.AddEnumValue("three");
            p6.AddEnumValue("Four");
            p6.InsertEnumValue(0, "zero");
        }


        // Animate the user property given by pList.
        static void AnimateList(FbxScene scene, FbxNode node, FbxUserProperty list)
        {

            scene.CreateTake(TAKE_ANIMATE_LIST);

            node.CreateTakeNode(TAKE_ANIMATE_LIST);
            node.SetCurrentTakeNode(TAKE_ANIMATE_LIST);

            FbxCurveNode fbxFCurveNode = list.GetKFCurveNode(true, TAKE_ANIMATE_LIST);

            if (fbxFCurveNode != null)
            {
                FbxTime keyTime = new FbxTime();
                FbxCurveKey key = new FbxCurveKey();
                FbxCurve fCurve = fbxFCurveNode.FCurveGet();

                key.Interpolation = FbxCurveKey.KeyInterpolation.Constant;
                key.ConstantMode = FbxCurveKey.KeyConstantMode.STANDARD;

                fCurve.KeyModifyBegin();
                fCurve.ResizeKeyBuffer(5);

                // One way of setting a keyframe value
                keyTime.SecondDouble = 0.0;
                key.Time = keyTime;
                key.Value = 0.0f;
                fCurve.KeySet(0, key);

                keyTime.SecondDouble = 1.0;
                key.Time = keyTime;
                key.Value = 1.0f;
                fCurve.KeySet(1, key);

                // an other way of setting a keyframe value
                keyTime.SecondDouble = 2.0;
                fCurve.KeySet(2, keyTime, 2.0f, (uint)FbxCurveKey.KeyInterpolation.Constant, (uint)FbxCurveKey.KeyConstantMode.STANDARD);

                keyTime.SecondDouble = 3.0;
                fCurve.KeySet(3, keyTime, 3.0f, (uint)FbxCurveKey.KeyInterpolation.Constant, (uint)FbxCurveKey.KeyConstantMode.STANDARD);

                keyTime.SecondDouble = 4.0;
                fCurve.KeySet(4, keyTime, 0.0f, (uint)FbxCurveKey.KeyInterpolation.Constant, (uint)FbxCurveKey.KeyConstantMode.STANDARD);

                fCurve.KeyModifyEnd();
            }
        }


        // Create a position constraint whith pSourceNode as source node and pConstraintedNode as constrained node.
        static FbxConstraintPosition CreatePositionConstraint(FbxSdkManager sdkManager, FbxNode sourceNode, FbxNode constrainedNode)
        {
            FbxConstraintPosition positionConstraint = FbxConstraintPosition.Create(sdkManager, "Position");

            // set constrained object
            positionConstraint.ConstrainedObject = constrainedNode;

            // set source
            positionConstraint.AddConstraintSource(sourceNode, 100.0);

            // Constrain the position in X, Y and Z
            positionConstraint.AffectX = true;
            positionConstraint.AffectY = true;
            positionConstraint.AffectZ = true;

            // keep offset between source and constrained object
            FbxVector4 positionSource = new FbxVector4();
            FbxVector4 positionConstrainedObj = new FbxVector4();
            sourceNode.GetDefaultT(positionSource);
            constrainedNode.GetDefaultT(positionConstrainedObj);
            FbxVector4 offset = positionConstrainedObj - positionSource;
            positionConstraint.Offset = offset;

            // activate property
            FbxUserProperty active = positionConstraint.FindProperty("Active", false);
            active.Set(true);

            return positionConstraint;
        }


        // Animate the cube by translating it in X, Y and Z.
        static void AnimateCube(FbxScene scene, FbxNode node)
        {
            scene.CreateTake(TAKE_ANIMATE_CUBE);
            node.CreateTakeNode(TAKE_ANIMATE_CUBE);
            node.SetCurrentTakeNode(TAKE_ANIMATE_CUBE);
            AnimateNode(node, TAKE_ANIMATE_CUBE);
        }


        static void AnimatePyramid(FbxScene scene, FbxNode node)
        {
            scene.CreateTake(TAKE_ANIMATE_PYRAMID);
            node.CreateTakeNode(TAKE_ANIMATE_PYRAMID);
            node.SetCurrentTakeNode(TAKE_ANIMATE_PYRAMID);

            AnimateNode(node, TAKE_ANIMATE_PYRAMID);
        }


        // Animate a given take node.
        static void AnimateNode(FbxNode node, string takeName)
        {
            FbxCurve curveX = null;
            FbxCurve curveY = null;
            FbxCurve curveZ = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            node.LclTranslation.GetKFCurveNode(true, takeName);
            curveX = node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_X, takeName);
            curveY = node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Y, takeName);
            curveZ = node.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Z, takeName);

            if (curveX != null)
            {
                curveX.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curveX.KeyAdd(time);
                curveX.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 1.0;
                keyIndex = curveX.KeyAdd(time);
                curveX.KeySet(keyIndex, time, 100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 2.0;
                keyIndex = curveX.KeyAdd(time);
                curveX.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 3.0;
                keyIndex = curveX.KeyAdd(time);
                curveX.KeySet(keyIndex, time, -100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 4.0;
                keyIndex = curveX.KeyAdd(time);
                curveX.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curveX.KeyModifyEnd();
            }

            if (curveY != null)
            {
                curveY.KeyModifyBegin();

                time.SecondDouble = 2.0;
                keyIndex = curveY.KeyAdd(time);
                curveY.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 3.0;
                keyIndex = curveY.KeyAdd(time);
                curveY.KeySet(keyIndex, time, 100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 4.0;
                keyIndex = curveY.KeyAdd(time);
                curveY.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 5.0;
                keyIndex = curveY.KeyAdd(time);
                curveY.KeySet(keyIndex, time, -100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 6.0;
                keyIndex = curveY.KeyAdd(time);
                curveY.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curveY.KeyModifyEnd();
            }

            if (curveZ != null)
            {
                curveZ.KeyModifyBegin();

                time.SecondDouble = 5.0;
                keyIndex = curveZ.KeyAdd(time);
                curveZ.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 6.0;
                keyIndex = curveZ.KeyAdd(time);
                curveZ.KeySet(keyIndex, time, 100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 7.0;
                keyIndex = curveZ.KeyAdd(time);
                curveZ.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 8.0;
                keyIndex = curveZ.KeyAdd(time);
                curveZ.KeySet(keyIndex, time, -100.0f, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 9.0;
                keyIndex = curveZ.KeyAdd(time);
                curveZ.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curveZ.KeyModifyEnd();
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
