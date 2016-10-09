using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

namespace Layers
{
    class Program
    {

        const string SAMPLE_FILENAME = "Layers.fbx";
        const string BACKGROUND_IMAGE_NAME = "Spotty";
        const string BACKGROUND_IMAGE = "spotty.jpg";
        const string LAYER1_IMAGE_NAME = "One";
        const string LAYER1_IMAGE = "1.jpg";
        const string LAYER2_IMAGE_NAME = "Waffle";
        const string LAYER2_IMAGE = "waffle.jpg";


        static void Main(string[] args)
        {

            FbxSdkManager sdkManager = null;
            FbxScene scene = null;
            bool result;

            // Prepare the FBX SDK.
            InitializeSdkObjects(out sdkManager, out scene);

            // Create the scene.
            FbxNode cube = CreateCube(sdkManager, "Cube");

            // Build the node tree.
            FbxNode rootNode = scene.RootNode;
            rootNode.AddChild(cube);


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



        // Create a cube with materials and layered textures.
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

            // indices of the uvs per each polygon
            int[] uvsId = { 0, 1, 3, 2, 2, 3, 5, 4, 4, 5, 7, 6, 6, 7, 9, 8, 1, 10, 11, 3, 12, 0, 2, 13 };

            // colors
            FbxColor[] colors = {
		// colors used for the materials
		new FbxColor(1.0, 1.0, 1.0, 1.0),
		new FbxColor(1.0, 1.0, 0.0, 1.0),
		new FbxColor(1.0, 0.0, 1.0, 1.0),
		new FbxColor(0.0, 1.0, 1.0, 1.0),
		new FbxColor(0.0, 0.0, 1.0, 1.0),
		new FbxColor(1.0, 0.0, 0.0, 1.0),
		new FbxColor(0.0, 1.0, 0.0, 1.0),
		new FbxColor(0.0, 0.0, 0.0, 1.0)};

            // create the main structure.
            FbxMesh mesh = FbxMesh.Create(sdkManager, "");

            // Create control points.
            mesh.InitControlPoints(8);
            mesh.ControlPoints = controlPoints;

            /* ----------------------------------------------------------------------------

            NOTE: Layers must be filled sequentially. That is, elements in layer
            0 have to exist before using layer 1, 2, etc... In this example,
            the reader will notice that normals, polygroups, color vertices,
            materials and the first texture are all in layer 0. Then one more texture,
            with its corresponding UVs, is added in layer 1 and another in layer 2.

            ---------------------------------------------------------------------------
            */

            FbxLayer layer = mesh.GetLayer(0);
            if (layer == null)
            {
                mesh.CreateLayer();
                layer = mesh.GetLayer(0);
            }

            // create the materials.
            /* Each polygon face will be assigned a unique material.
            */

            FbxLayerContainer layerContainer = (FbxLayerContainer)mesh;
            FbxLayerElementMaterial matLayer = FbxLayerElementMaterial.Create(layerContainer, "");
            matLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            matLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            layer.Materials = matLayer;

            // Create polygons. Assign material indices.
            int vId = 0;
            for (int f = 0; f < 6; f++)
            {
                mesh.BeginPolygon(f, -1, -1, true);//Material index.
                for (int v = 0; v < 4; v++)
                    mesh.AddPolygon(vtxId[vId++]);
                mesh.EndPolygon();
            }


            // specify normals per control point.
            /* For compatibility, we follow the rules stated in the 
               layer class documentation: normals are defined on layer 0 and
               are assigned by control point.
            */

            FbxLayerElementNormal normLayer = FbxLayerElementNormal.Create(layerContainer, "");
            normLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;
            normLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            for (int n = 0; n < 8; n++)
                normLayer.DirectArray.Add(normals[n]);

            layer.Normals = normLayer;

            // create color vertices
            /* We chosed to define oen color per control point. The other choice would
               have been to use the eBY_POLYGON_VERTEX mapping mode. In this second case,
               the reference mode should become eINDEX_TO_DIRECT.
            */

            FbxLayerElementVertexColor vtxcLayer = FbxLayerElementVertexColor.Create(layerContainer, "");
            vtxcLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByControlPoint;
            vtxcLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Direct;

            for (int c = 0; c < 8; c++)
                vtxcLayer.DirectArray.Add(colors[c]);

            layer.VertexColors = vtxcLayer;


            // create polygroups. 
            /* We are going to make a first group with the 4 sides.
               And a second group with the top and bottom sides.

               NOTE that the only reference mode allowed is eINDEX
            */

            FbxLayerElementPolygonGroup pgrpLayer = FbxLayerElementPolygonGroup.Create(layerContainer, "");
            pgrpLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            pgrpLayer.Reference_Mode = FbxLayerElement.ReferenceMode.Index;
            pgrpLayer.IndexArray.Add(0); // front face assigned to group 0
            pgrpLayer.IndexArray.Add(0); // right side assigned to group 0
            pgrpLayer.IndexArray.Add(0); // back face assigned to group 0
            pgrpLayer.IndexArray.Add(0); // left side assigned to group 0
            pgrpLayer.IndexArray.Add(1); // bottom face assigned to group 1
            pgrpLayer.IndexArray.Add(1); // top face assigned to group 1

            layer.PolygonGroups = pgrpLayer;

            // create the UV textures mapping.
            FbxTexture texture;
            FbxLayerElementUV uvLayer;
            FbxLayerElementTexture texLayer;

            // On layer 0 all the faces have the same texture
            uvLayer = FbxLayerElementUV.Create(layerContainer, "");
            uvLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            uvLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;

            int i;
            for (i = 0; i < 14; i++)
                uvLayer.DirectArray.Add(UVs[i]);

            for (i = 0; i < 24; i++)
                uvLayer.IndexArray.Add(uvsId[i]);
            layer.SetUVs(uvLayer, FbxLayerElement.LayerElementType.DiffuseTextures);

            // Create texture.
            texture = CreateTexture(sdkManager, BACKGROUND_IMAGE_NAME, BACKGROUND_IMAGE);

            texLayer = FbxLayerElementTexture.Create(layerContainer, "");
            texLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            texLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            texLayer.DirectArray.Add(texture);
            for (i = 0; i < 6; i++)
                texLayer.IndexArray.Add(0);

            layer.DiffuseTextures = texLayer;

            // On layer 1 only one texture is mapped on the front face of the cube.
            mesh.CreateLayer();
            layer = mesh.GetLayer(1);

            uvLayer = FbxLayerElementUV.Create(layerContainer, "");
            uvLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygonVertex;
            uvLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            for (i = 0; i < 4; i++)
                uvLayer.DirectArray.Add(UVs[i]);

            for (i = 0; i < 24; i++)
                uvLayer.IndexArray.Add(uvsId[i % 4]);
            layer.SetUVs(uvLayer, FbxLayerElement.LayerElementType.DiffuseTextures);

            texture = CreateTexture(sdkManager, LAYER1_IMAGE_NAME, LAYER1_IMAGE);
            texLayer = FbxLayerElementTexture.Create(layerContainer, "");
            texLayer.Blend_Mode = FbxLayerElementTexture.BlendMode.Modulate;
            texLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            texLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            texLayer.DirectArray.Add(texture);

            for (i = 0; i < 6; i++)
                texLayer.IndexArray.Add(0); // no texture

            layer.DiffuseTextures = texLayer;


            // Make a third layer where only one texture is mapped on the
            // front face of the cube. 

            // create layer 2
            mesh.CreateLayer();
            layer = mesh.GetLayer(2);

            FbxLayerElementUV uvLayer1 = FbxLayerElementUV.Create(layerContainer, "");
            uvLayer1.CopyFrom(uvLayer);  // we re-use the UV mapping from pervious layer.
            // we cannot use a previously allocated LayerElementUV, a copy is required.
            layer.SetUVs(uvLayer1, FbxLayerElement.LayerElementType.DiffuseTextures);

            texture = CreateTexture(sdkManager, LAYER2_IMAGE_NAME, LAYER2_IMAGE);

            texLayer = FbxLayerElementTexture.Create(layerContainer, "");
            texLayer.Blend_Mode = FbxLayerElementTexture.BlendMode.Modulate;
            texLayer.Mapping_Mode = FbxLayerElement.MappingMode.ByPolygon;
            texLayer.Reference_Mode = FbxLayerElement.ReferenceMode.IndexToDirect;
            texLayer.DirectArray.Add(texture);

            texLayer.IndexArray.Add(0);
            for (i = 1; i < 6; i++)
                texLayer.IndexArray.Add(-1); // no texture

            layer.DiffuseTextures = texLayer;


            // Finally we create the node containing the mesh
            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = mesh;
            node.Shading_Mode = FbxNode.ShadingMode.TextureShading;

            for (i = 0; i < 6; i++)
            {
                string materialName = "material";
                materialName += i;

                FbxSurfacePhong material = FbxSurfacePhong.Create(sdkManager, materialName);

                // Generate primary and secondary colors.
                material.EmissiveColor = new FbxDouble3(0.0, 0.0, 0.0);
                material.AmbientColor = new FbxDouble3(colors[i].Red, colors[i].Green, colors[i].Blue);
                material.DiffuseColor = new FbxDouble3(1.0, 1.0, 1.0);
                material.SpecularColor = new FbxDouble3(0.0, 0.0, 0.0);
                material.TransparencyFactor = 0.0;
                material.Shininess = 0.5;
                material.ShadingModel = "phong";

                //get the node of mesh, add material for it.
                FbxNode meshNode = mesh.Node;
                if (meshNode != null)
                    meshNode.AddMaterial(material);
            }


            return node;
        }


        static FbxTexture CreateTexture(FbxSdkManager sdkManager, string name, string filename)
        {
            FbxTexture texture = FbxTexture.Create(sdkManager, name);
            texture.SetFileName(filename); // Resource file is in current directory.
            texture.TextureUseType = FbxTexture.TextureUse.Standard;
            texture.Mapping = FbxTexture.MappingType.Uv;
            texture.MaterialUseType = FbxTexture.MaterialUse.Model;
            texture.SwapUV = false;
            texture.SetTranslation(0.0, 0.0);
            texture.SetScale(1.0, 1.0);
            texture.SetRotation(0.0, 0.0);
            return texture;
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
