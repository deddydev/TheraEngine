#pragma once
#include "stdafx.h"
#include "FbxGenericNode.h"
#include "FbxError.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"



{
	namespace FbxSDK
	{	
		FBXOBJECT_DEFINITION(FbxGenericNode,KFbxGenericNode);

		void FbxGenericNode::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGenericNode,GetError(),FbxErrorManaged,KError);
		
		FbxGenericNode::Error FbxGenericNode::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}				
		String^ FbxGenericNode::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}				
#ifndef DOXYGEN_SHOULD_SKIP_THIS
		// Clone
		CLONE_DEFINITION(FbxGenericNode,KFbxGenericNode);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS


	}
}