#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxErrorManaged;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** Empty node containing properties.
		* \nosubgrouping
		*/
		public ref class FbxGenericNode : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxGenericNode);
		internal:
			FbxGenericNode(KFbxGenericNode* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxGenericNode);
		protected:
			virtual void CollectManagedMemory()override;			
		public:			
			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				Error,
				ErrorCount
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

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			// Clone
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}