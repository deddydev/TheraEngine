#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateTexture.h"
#include "FbxLayerElementArrayTemplateTexture.h"
#include "FbxLayerElementArrayTemplateInt32.h"



{
	namespace FbxSDK
	{			
		void FbxLayerElementTemplateTexture::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateTexture::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateTexture,GetDirectArray(),FbxLayerElementArrayTemplateTexture,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateTexture,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}