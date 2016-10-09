#pragma once
#include "stdafx.h"
#include "FbxSubDeformer.h"
#include "FbxError.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{

		void FbxSubDeformer::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxSubDeformer,KFbxSubDeformer);

		void FbxSubDeformer::MultiLayer::set(bool value)
		{
			_Ref()->SetMultiLayer(value);
		}



		bool FbxSubDeformer::MultiLayer::get()
		{
			return _Ref()->GetMultiLayer();
		}


		FbxSubDeformer::SubDeformerType FbxSubDeformer::SubdeformerType::get()
		{
			return (FbxSubDeformer::SubDeformerType)_Ref()->GetSubDeformerType();
		}


		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSubDeformer,GetError(),FbxErrorManaged,KError);


		FbxSubDeformer::Error FbxSubDeformer::LastErrorID::get() 
		{
			return (FbxSubDeformer::Error)_Ref()->GetLastErrorID();
		}

		String^ FbxSubDeformer::LastErrorString::get()
		{
			return  gcnew String(_Ref()->GetLastErrorString());
		}
	}
}