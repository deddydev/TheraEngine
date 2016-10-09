#pragma once
#include "stdafx.h"
#include "FbxGlobalCameraSettings.h"
#include "FbxString.h"
#include "FbxCamera.h"
#include "FbxCameraSwitcher.h"
#include "FbxError.h"


namespace Skill
{
	namespace FbxSDK
	{	
		void FbxGlobalCameraSettings::CollectManagedMemory()
		{
			_KError = nullptr;
			_CameraSwitcher = nullptr;
			_CameraProducerPerspective = nullptr;
			_CameraProducerTop = nullptr;
			_CameraProducerBottom = nullptr;
			_CameraProducerBack = nullptr;
			_CameraProducerFront = nullptr;
			_CameraProducerRight = nullptr;
			_CameraProducerLeft = nullptr;
		}		
		bool FbxGlobalCameraSettings::SetDefaultCamera(String^ cameraName)
		{
			STRINGTOCHAR_ANSI(n,cameraName);			
			bool b = _Ref()->SetDefaultCamera(n);
			FREECHARPOINTER(n);
			return b;
		}
		String^ FbxGlobalCameraSettings::GetDefaultCamera()
		{
			return gcnew String(_Ref()->GetDefaultCamera());
		}
		void FbxGlobalCameraSettings::RestoreDefaultSettings()
		{
			_Ref()->RestoreDefaultSettings();
		}
		FbxGlobalCameraSettings::ViewingMode FbxGlobalCameraSettings::DefaultViewingMode::get()
		{
			return (ViewingMode)_Ref()->GetDefaultViewingMode(); 
		}
		void FbxGlobalCameraSettings::DefaultViewingMode::set(ViewingMode value)
		{
			_Ref()->SetDefaultViewingMode((KFbxGlobalCameraSettings::EViewingMode)value);
		}
		void FbxGlobalCameraSettings::CreateProducerCameras()
		{
			_Ref()->CreateProducerCameras();
		}
		void FbxGlobalCameraSettings::DestroyProducerCameras()
		{
			_Ref()->DestroyProducerCameras();
		}
		bool FbxGlobalCameraSettings::IsProducerCamera(FbxCamera^ camera)
		{
			return _Ref()->IsProducerCamera(camera->_Ref());
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCameraSwitcher,GetCameraSwitcher(),FbxCameraSwitcher,CameraSwitcher);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxGlobalCameraSettings,SetCameraSwitcher,FbxCameraSwitcher,CameraSwitcher);


		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerPerspective(),FbxCamera,CameraProducerPerspective);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerTop(),FbxCamera,CameraProducerTop);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerBottom(),FbxCamera,CameraProducerBottom);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerFront(),FbxCamera,CameraProducerFront);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerBack(),FbxCamera,CameraProducerBack);				
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerRight(),FbxCamera,CameraProducerRight);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGlobalCameraSettings,KFbxCamera,GetCameraProducerLeft(),FbxCamera,CameraProducerLeft);
				
		void FbxGlobalCameraSettings::CopyFrom(FbxGlobalCameraSettings^ settings)
		{
			*this->_FbxGlobalCameraSettings = * settings->_Ref();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGlobalCameraSettings,GetError(),FbxErrorManaged,KError);
		
		FbxGlobalCameraSettings::Error FbxGlobalCameraSettings::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}				
		String^ FbxGlobalCameraSettings::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}				

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		bool FbxGlobalCameraSettings::CopyProducerCamera(FbxStringManaged^ cameraName, FbxCamera^ camera)
		{
			return _Ref()->CopyProducerCamera(*cameraName->_Ref(), camera->_Ref());
		}
		int  FbxGlobalCameraSettings::GetProducerCamerasCount() { return 7; }

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		

	}
}