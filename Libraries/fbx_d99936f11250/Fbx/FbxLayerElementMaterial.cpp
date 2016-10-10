#pragma once
#include "stdafx.h"
#include "FbxLayerElementMaterial.h"
#include "FbxLayerContainer.h"


{
	namespace FbxSDK
	{		
		FbxLayerElementMaterial^ FbxLayerElementMaterial::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementMaterial* t = KFbxLayerElementMaterial::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementMaterial(t);
			return nullptr;
		}
	}
}