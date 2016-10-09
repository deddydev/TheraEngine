#pragma once
#include "stdafx.h"
#include "FbxLayerElementUV.h"
#include "FbxLayerContainer.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxLayerElementUV^ FbxLayerElementUV::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementUV* t = KFbxLayerElementUV::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementUV(t);
			return nullptr;
		}		
	}
}