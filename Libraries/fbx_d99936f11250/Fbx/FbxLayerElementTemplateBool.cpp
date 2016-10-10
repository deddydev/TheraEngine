#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateBool.h"
#include "FbxLayerElementArrayTemplateInt32.h"
#include "FbxLayerElementArrayTemplateBool.h"




{
	namespace FbxSDK
	{
		void FbxLayerElementTemplateBool::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateBool::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateBool,GetDirectArray(),FbxLayerElementArrayTemplateBool,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateBool,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}