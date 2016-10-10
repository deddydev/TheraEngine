#pragma once
#include "stdafx.h"
#include "FbxNull.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxTypedProperty.h"


{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxNull,KFbxNull);

		void FbxNull::CollectManagedMemory()
		{
			_Size = nullptr;
			FbxNodeAttribute::CollectManagedMemory();
		}
		
		void FbxNull::Reset()
		{
			_Ref()->Reset();
		}

		double FbxNull::SizeDefaultValue::get()
		{
			return _Ref()->GetSizeDefaultValue();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNull,Size,FbxDouble1TypedProperty,Size);
		FbxNull::Look FbxNull::LookType::get()
		{
			return (FbxNull::Look) _Ref()->Look.Get();
		}
		void FbxNull::LookType::set(FbxNull::Look value)
		{
			_Ref()->Look.Set((KFbxNull::ELook)value);
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS		

		CLONE_DEFINITION(FbxNull,KFbxNull);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}