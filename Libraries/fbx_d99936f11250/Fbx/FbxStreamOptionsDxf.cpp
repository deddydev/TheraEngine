#pragma once
#include "stdafx.h"
#include "FbxStreamOptionsDxf.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"


{
	namespace FbxSDK
	{
		namespace IO
		{
			FBXOBJECT_DEFINITION(FbxStreamOptionsDxfReader,KFbxStreamOptionsDxfReader);
			FbxStreamOptionsDxfReader::ObjectDerivation FbxStreamOptionsDxfReader::ObjDerivation::get()
			{
				return (ObjectDerivation)_Ref()->GetObjectDerivation();
			}
			void FbxStreamOptionsDxfReader::ObjDerivation::set(ObjectDerivation value)
			{
				_Ref()->SetObjectDerivation((KFbxStreamOptionsDxfReader::EObjectDerivation)value);
			}				

#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			CLONE_DEFINITION(FbxStreamOptionsDxfReader,KFbxStreamOptionsDxfReader);
#endif

			FBXOBJECT_DEFINITION(FbxStreamOptionsDxfWriter,KFbxStreamOptionsDxfWriter);			
#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			CLONE_DEFINITION(FbxStreamOptionsDxfWriter,KFbxStreamOptionsDxfWriter);
#endif			

		}
	}
}