#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateSurfaceMaterial.h"
#include "FbxLayerElementArrayTemplateSurfaceMaterial.h"
#include "FbxLayerElementArrayTemplateInt32.h"



{
	namespace FbxSDK
	{			
		void FbxLayerElementTemplateSurfaceMaterial::CollectManagedMemory()
		{
			_DirectArray = nullptr;
			_IndexArray = nullptr;
			FbxLayerElement::CollectManagedMemory();
		}
		bool FbxLayerElementTemplateSurfaceMaterial::Clear()
		{
			return _Ref()->Clear();			
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateSurfaceMaterial,GetDirectArray(),FbxLayerElementArrayTemplateSurfaceMaterial,DirectArray);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLayerElementTemplateSurfaceMaterial,GetIndexArray(),FbxLayerElementArrayTemplateInt32,IndexArray);				
	}
}