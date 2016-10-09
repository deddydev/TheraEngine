#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateBool.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;
		public ref class FbxLayerElementVisibility : FbxLayerElementTemplateBool
		{		
			REF_DECLARE(FbxLayerElement,KFbxLayerElementVisibility);						
		internal:
			FbxLayerElementVisibility(KFbxLayerElementVisibility* instance) : FbxLayerElementTemplateBool(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementVisibility^ Create(FbxLayerContainer^ owner,String^ name);			

		};	
	}
}