#pragma once
#include "stdafx.h"
#include "FbxLayerElementVisibility.h"
#include "FbxLayerContainer.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxLayerElementVisibility^ FbxLayerElementVisibility::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementVisibility* t = KFbxLayerElementVisibility::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementVisibility(t);
			return nullptr;
		}
	}
}