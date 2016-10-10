#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		/**	\brief This node attribute contains the properties of an optical reference.
		* \nosubgrouping
		*/
		public ref class FbxOpticalReference : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxOpticalReference);
		internal:
			FbxOpticalReference(KFbxOpticalReference * instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}

			
			FBXOBJECT_DECLARE(FbxOpticalReference);

		public:
			//! Return the type of node attribute which is EAttributeType::eOPTICAL_REFERENCE.
			//virtual EAttributeType GetAttributeType() const;

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

			// Clone
			CLONE_DECLARE(); 

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}