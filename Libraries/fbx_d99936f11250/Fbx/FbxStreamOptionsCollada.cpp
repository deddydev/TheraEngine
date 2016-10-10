#pragma once
#include "stdafx.h"
#include "FbxStreamOptionsCollada.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"


{
	namespace FbxSDK
	{
		namespace IO
		{

			FBXOBJECT_DEFINITION(FbxStreamOptionsColladaReader,KFbxStreamOptionsColladaReader);			
#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			CLONE_DEFINITION(FbxStreamOptionsColladaReader,KFbxStreamOptionsColladaReader);

#endif						
			void FbxStreamOptionsColladaWriter::SetTimeMode(FbxTime::TimeMode timeMode, double customFrameRate)
			{
				_Ref()->SetTimeMode((KTime::ETimeMode)timeMode,customFrameRate);
			}
			void FbxStreamOptionsColladaWriter::SetTimeMode(FbxTime::TimeMode timeMode)
			{
				_Ref()->SetTimeMode((KTime::ETimeMode)timeMode);
			}
			FbxTime^ FbxStreamOptionsColladaWriter::FramePeriod::get()
			{
				return gcnew FbxTime(&_Ref()->GetFramePeriod());
			}
			FBXOBJECT_DEFINITION(FbxStreamOptionsColladaWriter,KFbxStreamOptionsColladaWriter);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			CLONE_DEFINITION(FbxStreamOptionsColladaWriter,KFbxStreamOptionsColladaWriter);
#endif
		}
	}
}