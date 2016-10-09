#pragma once
#include "stdafx.h"
#include "FbxStreamOptionsFbx.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxString.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{			
			FBXOBJECT_DEFINITION(FbxStreamOptionsFbxReader,KFbxStreamOptionsFbxReader);											
			
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			CLONE_DEFINITION(FbxStreamOptionsFbxReader,KFbxStreamOptionsFbxReader);	
#endif

			FBXOBJECT_DEFINITION(FbxStreamOptionsFbxWriter,KFbxStreamOptionsFbxWriter);			

#ifndef DOXYGEN_SHOULD_SKIP_THIS		
			CLONE_DEFINITION(FbxStreamOptionsFbxWriter,KFbxStreamOptionsFbxWriter);	
#endif			
		}
	}
}