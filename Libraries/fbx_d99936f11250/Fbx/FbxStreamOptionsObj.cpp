#pragma once
#include "stdafx.h"
#include "FbxStreamOptionsObj.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{	
			FBXOBJECT_DEFINITION(FbxStreamOptionsObjReader,KFbxStreamOptionsObjReader);			

#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			CLONE_DEFINITION(FbxStreamOptionsObjReader,KFbxStreamOptionsObjReader);
#endif			

			FBXOBJECT_DEFINITION(FbxStreamOptionsObjWriter,KFbxStreamOptionsObjWriter);			
#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			CLONE_DEFINITION(FbxStreamOptionsObjWriter,KFbxStreamOptionsObjWriter);			
#endif			

		}
	}
}