#pragma once
#include "stdafx.h"
#include "FbxLayerElementTexture.h"
#include "FbxStream.h"
#include "FbxLayerContainer.h"


{
	namespace FbxSDK
	{						

		FbxLayerElementTexture^ FbxLayerElementTexture::Create(FbxLayerContainer^ owner,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxLayerElementTexture* t = KFbxLayerElementTexture::Create(owner->_Ref(),n);
			FREECHARPOINTER(n);
			if(t)
				return gcnew FbxLayerElementTexture(t);
			return nullptr;
		}		

		//REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayerElementTexture,KFbxLayerContainer,GetOwner(),FbxLayerContainer,Owner);		
		FbxLayerElementTexture::BlendMode FbxLayerElementTexture::Blend_Mode::get()
		{
			return (FbxLayerElementTexture::BlendMode)_Ref()->GetBlendMode();
		}
		void FbxLayerElementTexture::Blend_Mode::set(FbxLayerElementTexture::BlendMode value)
		{
			_Ref()->SetBlendMode((KFbxLayerElementTexture::EBlendMode)value);
		}
		VALUE_PROPERTY_GET_DEFINATION(FbxLayerElementTexture,GetAlpha(),double,Alpha);

		void FbxLayerElementTexture::Alpha::set(double value)
		{
			_Ref()->SetAlpha(value);
		}
		
	}
}