#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateColor.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;
		public ref class FbxLayerElementVertexColor :FbxLayerElementTemplateColor
		{
			REF_DECLARE(FbxLayerElement,KFbxLayerElementVertexColor);					
		internal:
			FbxLayerElementVertexColor(KFbxLayerElementVertexColor* instance) : FbxLayerElementTemplateColor(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementVertexColor^ Create(FbxLayerContainer^ owner,String^ name);				
		};		
	}
}