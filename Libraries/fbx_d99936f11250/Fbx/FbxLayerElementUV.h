#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateVector2.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;

		/** \brief Layer to map UVs on a geometry.
		* \nosubgrouping
		*/
		public ref class FbxLayerElementUV : FbxLayerElementTemplateVector2
		{		
			REF_DECLARE(FbxLayerElement,KFbxLayerElementUV);						
		internal:
			FbxLayerElementUV(KFbxLayerElementUV* instance) : FbxLayerElementTemplateVector2(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementUV^ Create(FbxLayerContainer^ owner,String^ name);			

		};	
	}
}