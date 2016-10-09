#pragma once
#include "stdafx.h"
#include "FbxObject.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** This class is used to hold meta-data information on nodes.
		* \nosubgrouping
		*
		* This class does not offer any new functionality over a regular KFbxObject;
		* all meta-data information should be stored in properties.
		* 
		*/
		public ref class FbxObjectMetaData : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxObjectMetaData);
		internal:
			FbxObjectMetaData(KFbxObjectMetaData* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
			
			FBXOBJECT_DECLARE(FbxObjectMetaData);
		public:

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

	}
}