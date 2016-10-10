#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "kfbxio/kfbxstreamoptionsobj.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		namespace IO
		{
			public ref class FbxStreamOptionsObj abstract sealed
			{
			public:
				static String^ REFERENCENODE = "REFERENCENODE";
				static String^ TRIANGULATE = "TRIANGULATE";
				static String^ DEFORMATION = "DEFORMATION";
			};

			/**	\brief This class is used for accessing the Import options of Obj files.
			* The content of a KfbxStreamOptionsObj object is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsObjReader : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsObjReader(KFbxStreamOptionsObjReader* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
			public:

				FBXOBJECT_DECLARE(FbxStreamOptionsObjReader);				
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsObjReader);
				/** Reset all options to default values
				*/
				//void Reset();

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();
#endif
			};


			/**	\brief This class is used for accessing the Export options of Obj files.
			* The content of a KfbxStreamOptionsObj object is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsObjWriter : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsObjWriter(KFbxStreamOptionsObjWriter* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}

			public:
				FBXOBJECT_DECLARE(FbxStreamOptionsObjWriter);				
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsObjWriter);

				/** Reset all options to default values
				*/
				//void Reset();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();
#endif
			};

		}
	}
}