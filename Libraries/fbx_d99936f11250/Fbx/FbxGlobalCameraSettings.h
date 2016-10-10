#pragma once
#include "stdafx.h"
#include "Fbx.h"



{
	namespace FbxSDK
	{
		ref class FbxStringManaged;
		ref class FbxCamera;
		ref class FbxCameraSwitcher;
		ref class FbxErrorManaged;
		/** This class contains the global camera settings.
		* \nosubgrouping
		*/
		public ref class FbxGlobalCameraSettings : IFbxNativePointer
		{			
			INTERNAL_CLASS_DECLARE(FbxGlobalCameraSettings,KFbxGlobalCameraSettings);
			REF_DECLARE(FbxGlobalCameraSettings,KFbxGlobalCameraSettings);
			DESTRUCTOR_DECLARE_2(FbxGlobalCameraSettings);
			INATIVEPOINTER_DECLARE(FbxGlobalCameraSettings,KFbxGlobalCameraSettings);		

			/**
			* \name Default viewing settings Camera
			*/
			//@{
		public:

			static const String^ Producer_Perspective  = "Producer Perspective";
			static const String^ Producer_Top = "Producer Top";
			static const String^ Producer_Front = "Producer Front";
			static const String^ Producer_Back = "Producer Back";
			static const String^ Producer_Right = "Producer Right";
			static const String^ Producer_Left = "Producer Left";
			static const String^ Producer_Bottom = "Producer Bottom";
			static const String^ Camera_Switcher = "Camera Switcher";

			/** Set the default camera
			* \param pCameraName     Name of the default camera.
			* \return                \c true if camera name is valid, \c false otherwise.
			* \remarks               A valid camera name is either one of the defined tokens (PRODUCER_PERSPECTIVE,
			*                        PRODUCER_TOP, PRODUCER_FRONT, PRODUCER_RIGHT, CAMERA_SWITCHER) or the name
			*                        of a camera inserted in the node tree under the scene's root node.
			*/
			bool SetDefaultCamera(String^ cameraName);

			/** Get default camera name
			* \return     The default camera name, or an empty string if the camera name has not been set
			*/
			String^ GetDefaultCamera();

			//! Restore default settings.
			void RestoreDefaultSettings();

			/** \enum EViewingMode Viewing modes.
			* - \e eSTANDARD
			* - \e eXRAY
			* - \e eMODELS_ONLY
			*/
			enum class ViewingMode
			{
				Standard,
				Xray,
				ModelsOnly
			};

			/** Get default viewing mode.
			* \return     The currently set Viewing mode.
			*/
			/** Set default viewing mode.
			* \param pViewingMode     Set default viewing mode to either eSTANDARD, eXRAY or eMODELS_ONLY.
			*/
			property ViewingMode DefaultViewingMode
			{
				ViewingMode get();
				void set(ViewingMode value);
			}

			/**
			* \name Producer Cameras
			* Producer cameras are global cameras used in MotionBuilder to view the scene.
			* They are not animatable but their default position can be set.
			*/
			//@{

			/** Create the default producer cameras.
			*/
			void CreateProducerCameras();

			/** Destroy the default producer cameras.
			*/
			void DestroyProducerCameras();

			/** Check if the camera is one of the producer cameras
			* \return     true if it is a producer camera false if not
			*/
			bool IsProducerCamera(FbxCamera^ camera);

			/** Get the camera switcher node.
			* \return Pointer to the camera switcher
			* \remarks This node has a node attribute of type \c KFbxNodeAttribute::eCAMERA_SWITCHER.
			* This node isn't saved if the scene contains no camera.
			* Nodes inserted below are never saved.
			*
			* Camera indices start at 1. Out of range indices are clamped between 1 and the
			* number of cameras in the scene. The index of a camera refers to its order of
			* appearance when searching the node tree depth first.
			*
			* Use function KFbxTakeNode::GetCameraIndex() to get and set the camera index.
			* If a camera is added or removed after camera indices have been set, the camera
			* indices must be updated. It's much simpler to set the camera indices once all
			* cameras have been set.
			*
			* Camera index keys must be set with constant interpolation to make sure camera
			* switches occur exaclty at key time.
			*/
			/** Set the camera the camera switcher.
			* \return     The reference to the internal Perspective camera.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxCameraSwitcher,CameraSwitcher);						

			/** Get a reference to producer perspective camera.
			* \return     The reference to the internal Perspective camera.
			*/
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerPerspective);

			/** Get a reference to producer top camera.
			* \return     The reference to the internal Top camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerTop);

			/** Get a reference to producer bottom camera.
			* \return     The reference to the internal Bottom camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerBottom);

			/** Get reference to producer front camera.
			* \return     The reference to the internal Front camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerFront);

			/** Get reference to producer back camera.
			* \return     The reference to the internal Back camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerBack);

			/** Get reference to producer right camera.
			* \return     The reference to the internal Right camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerRight);

			/** Get reference to producer left camera.
			* \return     The reference to the internal Left camera.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxCamera,CameraProducerLeft);

			//! Assignment operator.
			void CopyFrom(FbxGlobalCameraSettings^ settings);

			/**
			* \name Error Management
			* The same error object is shared among instances of this class.
			*/
			//@{

			/** Retrieve error object.
			*  \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);			

			/** \enum EError Error identifiers Most of these are only used internally.
			* - \e eNULL_PARAMETER
			* - \e eUNKNOWN_CAMERA_NAME
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				NullParameter = KFbxGlobalCameraSettings::eNULL_PARAMETER,
				UnknownCameraName = KFbxGlobalCameraSettings::eUNKNOWN_CAMERA_NAME,
				ErrorCount = KFbxGlobalCameraSettings::eERROR_COUNT
			};

			/** Get last error code.
			*  \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			*  \return     Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}	

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			bool CopyProducerCamera(FbxStringManaged^ cameraName, FbxCamera^ camera);
			int  GetProducerCamerasCount();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}