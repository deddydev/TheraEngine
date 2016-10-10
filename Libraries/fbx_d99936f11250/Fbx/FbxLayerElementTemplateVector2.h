#pragma once
#include "stdafx.h"
#include "FbxLayerElement.h"



{
	namespace FbxSDK
	{				
		ref class FbxLayerElementArrayTemplateVector2;
		ref class FbxLayerElementArrayTemplateInt32;
		
		public ref class FbxLayerElementTemplateVector2: FbxLayerElement
		{
			REF_DECLARE(FbxLayerElement,KFbxLayerElementTemplate<KFbxVector2>);
			protected:
			virtual void CollectManagedMemory() override;
		internal:
			FbxLayerElementTemplateVector2(KFbxLayerElementTemplate<KFbxVector2>* instance) : FbxLayerElement(instance)
			{
				_Free = false;
			}
		public:
			bool Clear();
			/** Access the array of Layer Elements.
			* \return      A reference to the Layer Elements array.
			* \remarks     You cannot put elements in the direct array when the mapping mode is set to eINDEX.
			*/
			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateVector2,DirectArray);				

			/** Access the array of index.
			* \return      A reference to the index array.
			* \remarks     You cannot put elements in the index array when the mapping mode is set to eDIRECT.
			*/
			REF_PROPERTY_GET_DECLARE(FbxLayerElementArrayTemplateInt32,IndexArray);

			/** Remove all elements from the direct and the index arrays.
			* \remark This function will fail if there is a lock on the arrays.
			* \return True if successfull or False if a lock is present
			*/
			//bool Clear();
		
			/*static bool operator ==(FbxLayerElementArrayTemplateTexture^ l1 , FbxLayerElementArrayTemplateTexture^ l2)
			{
				return *l1->_Ref() == *l2->_Ref();
			}*/				

			/** Change the Mapping mode to the new one and re-compute the index
			* array accordingly.
			* \param pNewMapping New mapping mode.
			* \return If the remapping is successfull, return 1 else
			* if an error occurred return 0. In case the function cannot
			* remap to the desired mode due to incompatible modes or not
			* supported mode, returns -1.
			*/
			//int RemapIndexTo(FbxLayerElement::MappingMode newMapping);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS			
		//public:
			//virtual int MemorySize();

			/**
			* \name Serialization section
			*/
			//@{
			//virtual bool ContentWriteTo(KFbxStream& pStream);

			//virtual bool ContentReadFrom(const KFbxStream& pStream);				

			//KFbxLayerElementArrayTemplate<Type>* mDirectArray;
			//KFbxLayerElementArrayTemplate<int>*  mIndexArray;
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}