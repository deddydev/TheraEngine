#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"



{
	namespace FbxSDK
	{
		/** FBX SDK subdeformer class
		* \nosubgrouping
		*/

		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxErrorManaged;


		public ref class FbxSubDeformer : FbxTakeNodeContainer
		{

		internal:
			FbxSubDeformer(KFbxSubDeformer* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}
			REF_DECLARE(FbxEmitter,KFbxSubDeformer);
			FBXOBJECT_DECLARE(FbxSubDeformer);
		protected:
			virtual void CollectManagedMemory() override;
		public:

			/** Get multilayer state.
			* \return                         The state of the multi-layer flag.
			*/
			/** Set multi layer state flag.
			* \param pMultiLayer               If \c true, multi-layering is enabled.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,MultiLayer);

			//	/** \enum ESubDeformerType SubDeformer types.
			//	* - \e eCLUSTER
			//	*/
			enum class SubDeformerType
			{
				Unidentified,
				Cluster,
				Subdeformer_count
			};

			/** Get the type of the sub deformer.
			* \return                         SubDeformer type identifier.
			*/
			virtual  VALUE_PROPERTY_GET_DECLARE(SubDeformerType,SubdeformerType);

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return                        Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			*/
			enum class Error
			{
				Eerror,
				Eerror_count
			} ;

			/** Get last error code.
			* \return                        Last error code.
			*/
			VALUE_PROPERTY_GET_DECLARE(Error,LastErrorID);

			/** Get last error string.
			* \return                        Textual description of the last error.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,LastErrorString);			
		};

	}
}