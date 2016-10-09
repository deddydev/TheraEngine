using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.FbxSDK;

namespace ExportScene04
{
    class Program
    {
        const string SAMPLE_FILENAME = "ExportScene04.fbx";

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
            FbxNode lightGroup = CreateLightGroup(sdkManager, "LightGroup");
            FbxNode marker = CreateMarker(sdkManager, "Marker");
            FbxNode camera1 = CreateCamera(sdkManager, "Camera1");
            FbxNode camera2 = CreateCamera(sdkManager, "Camera2");

            SetGlobalLightSettings(scene);

            SetCameraPointOfInterest(camera1, marker);
            SetCameraPointOfInterest(camera2, camera1);

            SetLightGroupDefaultPosition(lightGroup);
            SetMarkerDefaultPosition(marker);
            SetCamera1DefaultPosition(camera1);
            SetCamera2DefaultPosition(camera2);

            string takeName = "Rotating lights";

            // Identify current take when file is loaded.
            scene.SetCurrentTake(takeName);

            AnimateLightGroup(lightGroup, takeName);
            AnimateCamera(camera1, takeName);
            AnimateCameraSwitcher(scene.RootNode.CameraSwitcher, takeName);

            // Build the scene graph.
            FbxNode rootNode = scene.RootNode;
            rootNode.AddChild(lightGroup);
            rootNode.AddChild(marker);
            rootNode.AddChild(camera1);
            camera1.AddChild(camera2);


            // Set camera switcher as the default camera.
            scene.GlobalCameraSettings.SetDefaultCamera(FbxGlobalCameraSettings.Camera_Switcher);

            return true;
        }

        // Create 6 lights and set global light settings.
        static FbxNode CreateLightGroup(FbxSdkManager sdkManager, string name)
        {
            string lightName = "";
            FbxNode group = null;
            FbxNode node = null;
            FbxLight light = null;
            int i;

            group = FbxNode.Create(sdkManager, name);

            for (i = 0; i < 6; i++)
            {
                lightName = name;
                lightName += "-Light";
                lightName += i;

                node = CreateLight(sdkManager, lightName);
                group.AddChild(node);
            }

            for (i = 0; i < 6; i++)
            {
                light = (FbxLight)group.GetChild(i).NodeAttribute;
                light.FileName.Set("gobo.tif");
                light.DrawGroundProjection = true;
                light.DrawVolumetricLight = true;
                light.DrawFrontFacingVolumetricLight = false;
            }

            return group;
        }

        // Create a spotlight. 
        static FbxNode CreateLight(FbxSdkManager sdkManager, string name)
        {
            FbxLight light = FbxLight.Create(sdkManager, name);

            light.Light_Type = FbxLight.LightType.Spot;
            light.CastLight = true;

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = light;

            return node;
        }

        // Create a marker to use a point of interest for the camera. 
        static FbxNode CreateMarker(FbxSdkManager sdkManager, string name)
        {
            FbxMarker marker = FbxMarker.Create(sdkManager, name);

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = marker;

            return node;
        }

        static FbxNode CreateCamera(FbxSdkManager sdkManager, string name)
        {
            FbxCamera camera = FbxCamera.Create(sdkManager, name);

            // Modify some camera default settings.
            camera.ApertureMode = FbxCamera.CameraApertureMode.Vertical;
            camera.ApertureWidth = 0.816;
            camera.ApertureHeight = 0.612;
            camera.SqueezeRatio = 0.5;

            FbxNode node = FbxNode.Create(sdkManager, name);

            node.NodeAttribute = camera;

            return node;
        }

        static void SetGlobalLightSettings(FbxScene scene)
        {
            scene.GlobalLightSettings.AmbientColor = new FbxColor(1.0, 0.5, 0.2);
            scene.GlobalLightSettings.FogEnable = true;
            scene.GlobalLightSettings.FogMode = FbxGlobalLightSettings.LightFogMode.Linear;
            scene.GlobalLightSettings.FogDensity = 0.0020;
            scene.GlobalLightSettings.FogStart = 0.0;
            scene.GlobalLightSettings.FogEnd = 1000.0;
            scene.GlobalLightSettings.FogColor = new FbxColor(1.0, 0.0, 1.0);
        }

        static void SetCameraPointOfInterest(FbxNode camera, FbxNode pointOfInterest)
        {
            // Set the camera to always point at this node.
            camera.Target = pointOfInterest;
        }

        // The light group is just over the XZ plane.
        static void SetLightGroupDefaultPosition(FbxNode lightGroup)
        {
            int i;

            for (i = 0; i < lightGroup.GetChildCount(); i++)
            {
                SetLightDefaultPosition(lightGroup.GetChild(i), i);
            }

            lightGroup.LclTranslation.Set(new FbxDouble3(0.0, 15.0, 0.0));
            lightGroup.LclRotation.Set(new FbxDouble3(0.0, 0.0, 0.0));
            lightGroup.LclScaling.Set(new FbxDouble3(1.0, 1.0, 1.0));
        }

        static void SetLightDefaultPosition(FbxNode light, int index)
        {
            // Set light location depending of it's index.
            light.LclTranslation.Set(new FbxDouble3((Math.Cos((double)index) * 40.0), 0.0, (Math.Sin((double)index) * 40.0)));
            light.LclRotation.Set(new FbxDouble3(20.0, (90.0 - index * 60.0), 0.0));
            light.LclScaling.Set(new FbxDouble3(1.0, 1.0, 1.0));

            // Set light attributes depending of it's index.
            FbxDouble3[] color = 
	{
		new FbxDouble3(1.0, 0.0, 0.0), 
		new FbxDouble3(1.0, 1.0, 0.0), 
		new FbxDouble3(0.0, 1.0, 0.0), 
		new FbxDouble3(0.0, 1.0, 1.0), 
		new FbxDouble3(0.0, 0.0, 1.0), 
		new FbxDouble3(1.0, 0.0, 1.0)
	};

            FbxLight l = light.Light;
            if (l != null)
            {
                l.Color.Set(color[index % 6]);
                l.Intensity.Set(33.0);
                l.ConeAngle.Set(90.0);
                l.Fog.Set(100.0);
            }
        }

        static void SetMarkerDefaultPosition(FbxNode marker)
        {
            // The marker is at the origin.
            marker.LclTranslation.Set(new FbxDouble3(0.0, 0.0, 0.0));
            marker.LclRotation.Set(new FbxDouble3(0.0, 0.0, 0.0));
            marker.LclScaling.Set(new FbxDouble3(1.0, 1.0, 1.0));
        }

        // The code below shows how to compute the camera rotation.
        // In the present case, it wouldn't be necessary since the
        // camera is set to point to the marker. 
        static void SetCamera1DefaultPosition(FbxNode camera)
        {
            FbxVector4 cameraLocation = new FbxVector4(0.0, 100.0, -300.0);
            FbxVector4 defaultPointOfInterest = new FbxVector4(1.0, 100.0, -300.0);
            FbxVector4 newPointOfInterest = new FbxVector4(0, 0, 0);
            FbxVector4 rotation = new FbxVector4();
            FbxVector4 scaling = new FbxVector4(1.0, 1.0, 1.0);

            FbxVector4.AxisAlignmentInEulerAngle(cameraLocation, defaultPointOfInterest, newPointOfInterest, rotation);

            camera.LclTranslation.Set(cameraLocation);
            camera.LclRotation.Set(rotation);
            camera.LclScaling.Set(scaling);
        }

        static void SetCamera2DefaultPosition(FbxNode camera)
        {
            camera.LclTranslation.Set(new FbxVector4(-150.0, 0.0, 75.0));
        }

        // The light group rises and rotates.
        static void AnimateLightGroup(FbxNode lightGroup, string takeName)
        {
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int i;
            int keyIndex = 0;

            for (i = 0; i < lightGroup.GetChildCount(); i++)
            {
                AnimateLight(lightGroup.GetChild(i), i, takeName);
            }

            lightGroup.CreateTakeNode(takeName);
            lightGroup.SetCurrentTakeNode(takeName);
            lightGroup.LclRotation.GetKFCurveNode(true, takeName);
            lightGroup.LclTranslation.GetKFCurveNode(true, takeName);

            // Y axis rotation.
            curve = lightGroup.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_Y, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, (float)(5 * 360.0));

                curve.KeyModifyEnd();
            }

            // Y axis translation.
            curve = lightGroup.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Y, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 15.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 5.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 200.0f);

                curve.KeyModifyEnd();
            }
        }

        // The lights are changing color, intensity, orientation and cone angle.
        static void AnimateLight(FbxNode light, int index, string takeName)
        {
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int i, j;
            int keyIndex = 0;

            FbxLight l = light.Light;

            light.CreateTakeNode(takeName);
            light.SetCurrentTakeNode(takeName);
            l.Intensity.GetKFCurveNode(true, takeName);

            // Intensity fade in/out.
            curve = l.Intensity.GetKFCurve(null, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 3.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 33.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 7.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 33.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);


                curve.KeyModifyEnd();
            }

            // Fog fade in/out
            l.Fog.GetKFCurveNode(true, takeName);
            curve = l.Fog.GetKFCurve(null, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 3.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 33.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 7.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 33.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 0.0f);

                curve.KeyModifyEnd();
            }

            // X rotation swoops & cone angle woobles.
            {
                light.LclRotation.GetKFCurveNode(true, takeName);
                l.ConeAngle.GetKFCurveNode(true, takeName);

                curve = light.LclRotation.GetKFCurve(FbxCurve.FBXCURVE_R_X, takeName);
                FbxCurve coneCurve = l.ConeAngle.GetKFCurve(null, takeName);
                double value;

                curve.KeyModifyBegin();
                coneCurve.KeyModifyBegin();

                for (i = 0; i < 8; i++)
                {
                    time.SecondDouble = (double)i * 0.833333;
                    value = Math.Cos((((double)i) + (((double)index) * 60.0)) * 72.0);

                    keyIndex = curve.KeyAdd(time);
                    curve.KeySetValue(keyIndex, (float)((value - 0.4) * 30.0));
                    curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);
                    keyIndex = coneCurve.KeyAdd(time);
                    coneCurve.KeySetValue(keyIndex, (float)((2.0 - (value + 1.0)) * 45.0));
                    coneCurve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Linear);
                }

                // Finally, have the lights spread out and lose focus.
                time.SecondDouble = 10.0;

                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, -90.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                keyIndex = coneCurve.KeyAdd(time);
                coneCurve.KeySetValue(keyIndex, 180.0f);
                coneCurve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Linear);

                curve.KeyModifyEnd();
                coneCurve.KeyModifyEnd();
            }

            // Color cycling.
            {
                FbxDouble3[] color = 
		{
			new FbxDouble3(1.0, 0.0, 0.0), 
			new FbxDouble3(1.0, 1.0, 0.0), 
			new FbxDouble3(0.0, 1.0, 0.0), 
			new FbxDouble3(0.0, 1.0, 1.0), 
			new FbxDouble3(0.0, 0.0, 1.0), 
			new FbxDouble3(1.0, 0.0, 1.0)
		};

                FbxCurve[] curves = new FbxCurve[3];
                l.Color.GetKFCurveNode(true, takeName);
                curves[0] = l.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_RED, takeName);
                curves[1] = l.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_GREEN, takeName);
                curves[2] = l.Color.GetKFCurve(FbxCurve.FBXCURVE_COLOR_BLUE, takeName);

                if (curves[0] != null && curves[1] != null && curves[2] != null)
                {
                    curves[0].KeyModifyBegin();
                    curves[1].KeyModifyBegin();
                    curves[2].KeyModifyBegin();

                    for (i = 0; i < 24; i++)
                    {
                        j = i + index;

                        while (j > 5)
                        {
                            j -= 6;
                        }

                        time.SecondDouble = ((double)i * 0.4166666);

                        keyIndex = curves[0].KeyAdd(time);
                        curves[0].KeySetValue(keyIndex, (float)color[j].X);
                        curves[0].KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                        keyIndex = curves[1].KeyAdd(time);
                        curves[1].KeySetValue(keyIndex, (float)color[j].Y);
                        curves[1].KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);

                        keyIndex = curves[2].KeyAdd(time);
                        curves[2].KeySetValue(keyIndex, (float)color[j].Z);
                        curves[2].KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Cubic);
                    }

                    curves[0].KeyModifyEnd();
                    curves[1].KeyModifyEnd();
                    curves[2].KeyModifyEnd();
                }
            }
        }

        // The camera is rising and rolling twice.
        static void AnimateCamera(FbxNode camera, string takeName)
        {
            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            camera.CreateTakeNode(takeName);
            camera.SetCurrentTakeNode(takeName);
            camera.LclTranslation.GetKFCurveNode(true, takeName);

            // X translation.
            curve = camera.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_X, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 200.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curve.KeyModifyEnd();
            }

            // Y translation.
            curve = camera.LclTranslation.GetKFCurve(FbxCurve.FBXCURVE_T_Y, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 300.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curve.KeyModifyEnd();
            }

            // Camera roll.
            FbxCamera cam = camera.Camera;
            cam.Roll.GetKFCurveNode(true, takeName);

            curve = cam.Roll.GetKFCurve(null, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 0.0f, FbxCurveKey.KeyInterpolation.Linear);

                time.SecondDouble = 10.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySet(keyIndex, time, 2 * 360.0f, FbxCurveKey.KeyInterpolation.Cubic);

                curve.KeyModifyEnd();
            }
        }

        // Alternate between camera 1 and camera 2.
        static void AnimateCameraSwitcher(FbxCameraSwitcher cameraSwitcher, string takeName)
        {
            if (cameraSwitcher == null)
                return;

            FbxCurve curve = null;
            FbxTime time = new FbxTime();
            int keyIndex = 0;

            // Camera index keys must be set with constant interpolation to 
            // make sure camera switches occur exaclty at key time.
            cameraSwitcher.CameraIndex.GetKFCurveNode(true, takeName);
            curve = cameraSwitcher.CameraIndex.GetKFCurve(null, takeName);
            if (curve != null)
            {
                curve.KeyModifyBegin();

                time.SecondDouble = 0.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 2.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Constant);

                time.SecondDouble = 2.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 1.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Constant);

                time.SecondDouble = 5.0;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 2.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Constant);

                time.SecondDouble = 7.5;
                keyIndex = curve.KeyAdd(time);
                curve.KeySetValue(keyIndex, 1.0f);
                curve.KeySetInterpolation(keyIndex, FbxCurveKey.KeyInterpolation.Constant);

                curve.KeyModifyEnd();
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
