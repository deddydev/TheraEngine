#pragma once
#include "stdafx.h"
#include "FbxCamera.h"
#include "FbxCameraManipulator.h"
#include "FbxScene.h"
#include "FbxVector4.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"




{
	namespace FbxSDK
	{	
		void FbxCameraManipulator::CollectManagedMemory()
		{
			_Camera = nullptr;
			FbxObjectManaged::CollectManagedMemory();
		}		

		FBXOBJECT_DEFINITION(FbxCameraManipulator,KFbxCameraManipulator);

		void FbxCameraManipulator::SetCamera(FbxCamera^ camera, bool validateLookAtPos)
		{
			_Ref()->SetCamera(*camera->_Ref(),validateLookAtPos);
			_Camera = camera;
		}

		FbxCamera^ FbxCameraManipulator::Camera::get()
		{
			return _Camera;
		}

		void FbxCameraManipulator::SetUpVector(FbxVector4^ upVector)
		{
			_Ref()->SetUpVector(*upVector->_Ref());
		}
		void FbxCameraManipulator::FrameAll(FbxSceneManaged^ scene)
		{
			_Ref()->FrameAll(*scene->_Ref());
		}
		void FbxCameraManipulator::FrameSelected(FbxSceneManaged^ scene)
		{
			_Ref()->FrameSelected(*scene->_Ref());
		}
		bool FbxCameraManipulator::OrbitBegin(int mouseX, int mouseY)
		{
			return _Ref()->OrbitBegin(mouseX,mouseY);
		}
		bool FbxCameraManipulator::OrbitNotify(int mouseX, int mouseY)
		{
			return _Ref()->OrbitNotify(mouseX,mouseY);
		}
		void FbxCameraManipulator::OrbitEnd()
		{
			_Ref()->OrbitEnd();
		}
		bool FbxCameraManipulator::DollyBegin(int mouseX, int mouseY)
		{
			return _Ref()->DollyBegin(mouseX,mouseY);
		}
		bool FbxCameraManipulator::DollyNotify(int mouseX, int mouseY)
		{
			return _Ref()->DollyNotify(mouseX,mouseY);
		}
		void FbxCameraManipulator::DollyEnd()
		{
			_Ref()->DollyEnd();
		}
		bool FbxCameraManipulator::PanBegin(int mouseX, int mouseY)
		{
			return _Ref()->PanBegin(mouseX,mouseY);
		}
		bool FbxCameraManipulator::PanNotify(int mouseX, int mouseY)
		{
			return _Ref()->PanNotify(mouseX,mouseY);
		}
		void FbxCameraManipulator::PanEnd()
		{
			_Ref()->PanEnd();
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS						
		CLONE_DEFINITION(FbxCameraManipulator,KFbxCameraManipulator);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

	}
}