#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateTexture.h"


{
	namespace FbxSDK
	{			
		ref class FbxLayerContainer;		

		/** Layer to map Textures on a geometry.
		* \nosubgrouping
		*/
		public ref class FbxLayerElementTexture : FbxLayerElementTemplateTexture
		{					
			REF_DECLARE(FbxLayerElement,KFbxLayerElementTexture);						
		internal:
			FbxLayerElementTexture(KFbxLayerElementTexture* instance) : FbxLayerElementTemplateTexture(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementTexture^ Create(FbxLayerContainer^ owner,String^ name);
			enum class BlendMode
			{
				Translucent = KFbxLayerElementTexture::eTRANSLUCENT,
				Add = KFbxLayerElementTexture::eADD,
				Modulate = KFbxLayerElementTexture::eMODULATE,
				Modulate2 = KFbxLayerElementTexture::eMODULATE2,
				Maxblend = KFbxLayerElementTexture::eMAXBLEND
			};

			//REF_PROPERTY_GET_DECLARE(FbxLayerContainer,Owner);			
			VALUE_PROPERTY_GETSET_DECLARE(BlendMode,Blend_Mode);
			VALUE_PROPERTY_GETSET_DECLARE(double,Alpha);			

		};	
	}
}