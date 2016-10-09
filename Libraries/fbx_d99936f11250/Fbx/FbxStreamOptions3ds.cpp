#pragma once
#include "stdafx.h"
#include "FbxStreamOptions3ds.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "KFbxIO/kfbxstreamoptions3ds.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{
			FBXOBJECT_DEFINITION(FbxStreamOptions3dsReader,KFbxStreamOptions3dsReader);					

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			CLONE_DEFINITION(FbxStreamOptions3dsReader,KFbxStreamOptions3dsReader);
#endif

			FBXOBJECT_DEFINITION(FbxStreamOptions3dsWriter,KFbxStreamOptions3dsWriter);						
			
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			CLONE_DEFINITION(FbxStreamOptions3dsWriter,KFbxStreamOptions3dsWriter);
#endif			

		}
	}
}
