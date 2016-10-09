#pragma once
#include "stdafx.h"
#include "FbxLayerElementVertexColor.h"
#include "FbxLayerContainer.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxLayerElementVertexColor^ FbxLayerElementVertexColor::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementVertexColor* t = KFbxLayerElementVertexColor::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementVertexColor(t);
			return nullptr;
		}
	}
}