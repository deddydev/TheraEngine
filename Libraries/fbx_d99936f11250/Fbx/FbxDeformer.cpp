#pragma once
#include "stdafx.h"
#include "FbxDeformer.h"
#include "FbxError.h"



{
	namespace FbxSDK
	{	
		void FbxDeformer::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}

		bool FbxDeformer::MultiLayer::get()				
		{
			return _Ref()->GetMultiLayer();
		}
		void FbxDeformer::MultiLayer::set(bool value)
		{
			_Ref()->SetMultiLayer(value);
		}			
		FbxDeformer::DeformerType FbxDeformer::Deformer_Type::get()
		{
			return (DeformerType)_Ref()->GetDeformerType();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDeformer,GetError(),FbxErrorManaged,KError);		

		FbxDeformer::Error FbxDeformer::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}				
		String^ FbxDeformer::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}				
	}
}