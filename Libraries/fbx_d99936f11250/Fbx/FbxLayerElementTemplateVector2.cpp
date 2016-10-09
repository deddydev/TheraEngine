#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateVector2.h"
#include "FbxLayerElementArrayTemplateInt32.h"
#include "FbxLayerElementArrayTemplateVector2.h"


namespace Skill
{
	namespace FbxSDK
	{

		void FbxLayerElementTemplateVector2::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateVector2::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateVector2,GetDirectArray(),FbxLayerElementArrayTemplateVector2,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateVector2,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}