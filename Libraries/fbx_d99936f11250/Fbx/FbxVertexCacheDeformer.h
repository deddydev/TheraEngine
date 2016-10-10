#pragma once
#include "stdafx.h"
#include "FbxDeformer.h"



{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxCacheManaged;
		/** \brief This class deforms control points of a geometry using control point positions
		* stored in the associated cache object.
		* \nosubgrouping
		*/
		public ref class FbxVertexCacheDeformer : FbxDeformer
		{
			REF_DECLARE(FbxEmitter,KFbxVertexCacheDeformer);
		internal:
			FbxVertexCacheDeformer(KFbxVertexCacheDeformer* instance) : FbxDeformer(instance)
			{
				_Free = false;
			}

			FBXOBJECT_DECLARE(FbxVertexCacheDeformer);
		protected:
			virtual void CollectManagedMemory() override;
		public:

			/** Get the cache object used by this deformer.
			* \return     A pointer to the cache object used by this deformer, or \c NULL if no cache object is assigned.
			*/
			/** Assign a cache object to be used by this deformer.
			* \param pCache     The cache object.
			*/ 
			REF_PROPERTY_GETSET_DECLARE(FbxCacheManaged,Cache);

			/** Get the name of the selected channel.
			* \return     The name of the selected channel within the cache object.
			*/
			/** Select the cache channel by name.
			* \param pName     The name of channel to use within the cache object.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,CacheChannel);			

			/** Indicate if the deformer is active or not.
			* \return     The current state of the deformer.
			*/
			/** Activate the deformer.
			* \param pValue     Set to \c true to enable the deformer.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,IsActive);

			/** Get the deformer type.
			* \return     Deformer type identifier.
			*/
			//virtual EDeformerType GetDeformerType() const { return KFbxDeformer::eVERTEX_CACHE; }

			//! Assigment operator.
			void CopyFrom(FbxVertexCacheDeformer^ deformer)
			{
				*_Ref() = *deformer->_Ref();
			}		


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:
			//static const char* ChannelPropertyName;
			//static const char* ActivePropertyName;


			// Clone
			CLONE_DECLARE()
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}