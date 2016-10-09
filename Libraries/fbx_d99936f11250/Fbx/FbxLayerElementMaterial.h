#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateSurfaceMaterial.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;

		/** \brief Layer to map Materials on a geometry.
		* \nosubgrouping
		*/
		public ref class FbxLayerElementMaterial : FbxLayerElementTemplateSurfaceMaterial
		{		
			REF_DECLARE(FbxLayerElement,KFbxLayerElementMaterial);						
		internal:
			FbxLayerElementMaterial(KFbxLayerElementMaterial* instance) : FbxLayerElementTemplateSurfaceMaterial(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementMaterial^ Create(FbxLayerContainer^ owner,String^ name);

		};
	}
}