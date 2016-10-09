#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateInt32.h"
#include "FbxLayerElementArrayTemplateInt32.h"



namespace Skill
{
	namespace FbxSDK
	{
		void FbxLayerElementTemplateInt32::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateInt32::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateInt32,GetDirectArray(),FbxLayerElementArrayTemplateInt32,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateInt32,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}