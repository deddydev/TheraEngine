#pragma once
#include "stdafx.h"
#include "FbxLayerElementNormal.h"
#include "FbxLayerContainer.h"

namespace Skill
{
	namespace FbxSDK
	{
		FbxLayerElementNormal^ FbxLayerElementNormal::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementNormal* t = KFbxLayerElementNormal::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementNormal(t);
			return nullptr;
		}

	}
}