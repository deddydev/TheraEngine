#pragma once
#include "stdafx.h"
#include "FbxPlug.h"
#include "FbxSdkManager.h"
#include "FbxClassID.h"
#include "FbxString.h"


{
	namespace FbxSDK
	{				
		FBXPLUG_DEFINITION(FbxPlug,KFbxPlug);

		void FbxPlug::CollectManagedMemory()
		{
			this->_RuntimeClassId = nullptr;
			sdkManager = nullptr;			
		}

		FbxPlug::FbxPlug(KFbxPlug* p) : Events::FbxEmitter(p)
		{
			_Free = false;
		}
		void FbxPlug::Destroy(bool recursive, bool dependents)
		{
			_Ref()->Destroy(recursive, dependents);
			_Free = false;
		}				

		void FbxPlug::Destroy()
		{
			_Ref()->Destroy();
			_Free = false;
		}

		FbxSdkManagerManaged^ FbxPlug::SdkManager::get()
		{
			return sdkManager;			
		}

		bool FbxPlug::Is(FbxClassId^ classId)
		{
			return _Ref()->Is(*classId->_Ref());
		}
		bool FbxPlug::IsRuntime(FbxClassId^ classId)
		{
			return _Ref()->IsRuntime(*classId->_Ref());
		}
		bool FbxPlug::SetRuntimeClassId(FbxClassId^ classId)
		{
			return _Ref()->SetRuntimeClassId(*classId->_Ref());
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxPlug,GetRuntimeClassId(),FbxClassId,RuntimeClassId);		


		VALUE_PROPERTY_GET_DEFINATION(FbxPlug,IsRuntimePlug(),bool,IsRuntimePlug);		

		FbxPlugInfo::FbxPlugInfo(const KFbxPlug* p)
		{
			plug = p;
		}		
	}
}