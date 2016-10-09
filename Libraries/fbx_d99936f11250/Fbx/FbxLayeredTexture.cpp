#pragma once
#include "stdafx.h"
#include "FbxLayeredTexture.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxLayeredTexture,KFbxLayeredTexture);

		FbxLayeredTexture::BlendMode FbxLayeredTexture::default::get(int index)
		{	
			KFbxLayeredTexture::EBlendMode m;
			_Ref()->GetTextureBlendMode(index,m);
			return (FbxLayeredTexture::BlendMode)m;
		}
		void FbxLayeredTexture::default::set(int index,FbxLayeredTexture::BlendMode value)
		{				
			_Ref()->SetTextureBlendMode(index,(KFbxLayeredTexture::EBlendMode)value);			
		}
	}
}