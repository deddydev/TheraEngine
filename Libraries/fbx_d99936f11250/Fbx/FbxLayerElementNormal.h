#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateVector4.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;
		/** \brief Layer to map Normals on a geometry.
		* \nosubgrouping
		*/
		public ref class FbxLayerElementNormal : FbxLayerElementTemplateVector4
		{
			REF_DECLARE(FbxLayerElement,KFbxLayerElementNormal);						
		internal:
			FbxLayerElementNormal(KFbxLayerElementNormal* instance) : FbxLayerElementTemplateVector4(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementNormal^ Create(FbxLayerContainer^ owner,String^ name);			
		};
	}
}