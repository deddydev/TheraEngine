#pragma once
#include "stdafx.h"
#include "FbxLayerElement.h"


namespace Skill
{
	namespace FbxSDK
	{				
		ref class FbxLayerElementArrayTemplateSurfaceMaterial;		
		ref class FbxLayerElementArrayTemplateInt32;

		/** This class complements the KFbxLayerElement class.
		* \nosubgrouping
		*/

		public ref class FbxLayerElementTemplateSurfaceMaterial : FbxLayerElement
		{
			REF_DECLARE(FbxLayerElement,KFbxLayerElementTemplate<KFbxSurfaceMaterial*>);
			protected:
			virtual void CollectManagedMemory() override;
		internal:
			FbxLayerElementTemplateSurfaceMaterial(KFbxLayerElementTemplate<KFbxSurfaceMaterial*>* instance) : FbxLayerElement(instance)
			{
				_Free = false;
			}
		public:
			bool Clear();
			/** Access the array of Layer Elements.
			* \return      A reference to the Layer Elements array.
			* \remarks     You cannot put elements in the direct array when the mapping mode is set to eINDEX.
			*/
			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateSurfaceMaterial,DirectArray);				

			/** Access the array of index.
			* \return      A reference to the index array.
			* \remarks     You cannot put elements in the index array when the mapping mode is set to eDIRECT.
			*/
			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateInt32,IndexArray);			
		};


	}
}