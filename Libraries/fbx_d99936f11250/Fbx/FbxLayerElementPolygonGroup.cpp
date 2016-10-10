#pragma once
#include "stdafx.h"
#include "FbxLayerElementPolygonGroup.h"
#include "FbxLayerContainer.h"



{
	namespace FbxSDK
	{		
		FbxLayerElementPolygonGroup^ FbxLayerElementPolygonGroup::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementPolygonGroup* t = KFbxLayerElementPolygonGroup::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementPolygonGroup(t);
			return nullptr;
		}
	}
}