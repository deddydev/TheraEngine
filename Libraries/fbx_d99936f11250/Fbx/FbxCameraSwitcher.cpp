#pragma once
#include "stdafx.h"
#include "FbxCameraSwitcher.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"
#include "FbxTypedProperty.h"



{
	namespace FbxSDK
	{									

		FBXOBJECT_DEFINITION(FbxCameraSwitcher,KFbxCameraSwitcher);

		void FbxCameraSwitcher::CollectManagedMemory()
		{
			_CameraIndex = nullptr;
			FbxNodeAttribute::CollectManagedMemory();
		}

		int FbxCameraSwitcher::DefaultCameraIndex::get()
		{
			return _Ref()->GetDefaultCameraIndex();
		} 
		void FbxCameraSwitcher::DefaultCameraIndex::set(int value)
		{
			_Ref()->SetDefaultCameraIndex(value);
		}			

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCameraSwitcher,CameraIndex,FbxInteger1TypedProperty,CameraIndex);

#ifndef DOXYGEN_SHOULD_SKIP_THIS		

		// Clone
		CLONE_DEFINITION(FbxCameraSwitcher,KFbxCameraSwitcher);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}