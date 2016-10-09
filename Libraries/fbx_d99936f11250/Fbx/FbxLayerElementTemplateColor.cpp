#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateColor.h"
#include "FbxLayerElementArrayTemplateInt32.h"
#include "FbxLayerElementArrayTemplateColor.h"


namespace Skill
{
	namespace FbxSDK
	{
		void FbxLayerElementTemplateColor::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateColor::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateColor,GetDirectArray(),FbxLayerElementArrayTemplateColor,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateColor,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}