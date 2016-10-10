#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateVector4.h"
#include "FbxLayerElementArrayTemplateInt32.h"
#include "FbxLayerElementArrayTemplateVector4.h"



{
	namespace FbxSDK
	{
		void FbxLayerElementTemplateVector4::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateVector4::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateVector4,GetDirectArray(),FbxLayerElementArrayTemplateVector4,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateVector4,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}