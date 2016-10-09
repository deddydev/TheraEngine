#pragma once
#include "stdafx.h"
#include "FbxLayerElementSmoothing.h"
#include "FbxLayerContainer.h"

namespace Skill
{
	namespace FbxSDK
	{		
			
		FbxLayerElementSmoothing^ FbxLayerElementSmoothing::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementSmoothing* t = KFbxLayerElementSmoothing::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementSmoothing(t);
			return nullptr;
		}
	}
}