#pragma once
#include "stdafx.h"
#include "FbxGeometry.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** FBX SDK procedural geometry class
		* \nosubgrouping
		*/
		public ref class FbxProceduralGeometry : FbxGeometry
		{
			REF_DECLARE(FbxEmitter,KFbxProceduralGeometry);
		internal:
			FbxProceduralGeometry(KFbxProceduralGeometry* instance) : FbxGeometry(instance)
			{
				_Free = false;
			}
			
			FBXOBJECT_DECLARE(FbxProceduralGeometry);
		public:			
		};

	}
}