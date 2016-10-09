#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateInt32.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;
		public ref class FbxLayerElementSmoothing :FbxLayerElementTemplateInt32  
		{	
			REF_DECLARE(FbxLayerElement,KFbxLayerElementSmoothing);						
		internal:
			FbxLayerElementSmoothing(KFbxLayerElementSmoothing* instance) : FbxLayerElementTemplateInt32(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementSmoothing^ Create(FbxLayerContainer^ owner,String^ name);	




		};
	}
}