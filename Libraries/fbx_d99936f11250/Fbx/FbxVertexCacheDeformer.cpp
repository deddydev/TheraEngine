#pragma once
#include "stdafx.h"
#include "FbxVertexCacheDeformer.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxCache.h"



{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxVertexCacheDeformer,KFbxVertexCacheDeformer);
		void FbxVertexCacheDeformer::CollectManagedMemory()
		{
			_Cache = nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxVertexCacheDeformer,KFbxCache,GetCache(),FbxCacheManaged,Cache);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxVertexCacheDeformer,SetCache,FbxCacheManaged,Cache);

		String^ FbxVertexCacheDeformer::CacheChannel::get()
		{
			FbxString kstr = _Ref()->GetCacheChannel();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		void FbxVertexCacheDeformer::CacheChannel::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);			
			_Ref()->SetCacheChannel(v);
			FREECHARPOINTER(v);
		}

		bool FbxVertexCacheDeformer::IsActive::get()
		{
			return _Ref()->IsActive();
		}
		void FbxVertexCacheDeformer::IsActive::set(bool value)
		{
			_Ref()->SetActive(value);
		}
#ifndef DOXYGEN_SHOULD_SKIP_THIS	
		CLONE_DEFINITION(FbxVertexCacheDeformer,KFbxVertexCacheDeformer);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}