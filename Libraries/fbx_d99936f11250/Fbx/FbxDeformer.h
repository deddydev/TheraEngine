#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxErrorManaged;
		/**FBX SDK deformer class
		*\nosubgrouping
		*/
		public ref class FbxDeformer : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxDeformer);
		internal:
			FbxDeformer(KFbxDeformer* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}

		protected:
			virtual void CollectManagedMemory() override;
		public:
			/** Get multi-layer state.
			* \return     The current state of the multi-layer flag.
			*/
			/** Set multi-layer state flag.
			* \param pMultiLayer     Set to \c true to enable multi-layering.
			*/
			property bool MultiLayer
			{
				bool get();
				void set(bool value);
			}				

			/** \enum EDeformerType Deformer types.
			* - \e UNIDENTIFIED
			* - \e eSKIN
			* - \e eVERTEX_CACHE
			* - \e eDEFORMER_COUNT
			*/
			enum class DeformerType
			{
				Unidentified = KFbxDeformer::eUNIDENTIFIED,
				Skin = KFbxDeformer::eSKIN,
				VertexCache = KFbxDeformer::eVERTEX_CACHE,
				DeformerCount = KFbxDeformer::eDEFORMER_COUNT
			};

			/** Get the deformer type.
			* \return     Deformer type identifier.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(DeformerType,Deformer_Type);

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				ERROR,
				ERRORCOUNT
			};

			/** Get last error code.
			* \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}
			/** Get last error string.
			* \return     Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}					

			//@}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////		
		};

	}
}