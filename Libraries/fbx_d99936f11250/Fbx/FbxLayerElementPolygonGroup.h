#pragma once
#include "stdafx.h"
#include "FbxLayerElementTemplateInt32.h"

namespace Skill
{
	namespace FbxSDK
	{	
		ref class FbxLayerContainer;

		public ref class FbxLayerElementPolygonGroup :FbxLayerElementTemplateInt32
		{		
			REF_DECLARE(FbxLayerElement,KFbxLayerElementPolygonGroup);						
		internal:
			FbxLayerElementPolygonGroup(KFbxLayerElementPolygonGroup* instance) : FbxLayerElementTemplateInt32(instance)
			{
				_Free = false;
			}		
		public :				
			static FbxLayerElementPolygonGroup^ Create(FbxLayerContainer^ owner,String^ name);	


		};
	}
}