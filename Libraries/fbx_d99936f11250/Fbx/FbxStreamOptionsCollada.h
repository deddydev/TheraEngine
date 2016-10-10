#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "FbxTime.h"
#include "KFbxIO/kfbxstreamoptionsCollada.h"


{
	namespace FbxSDK
	{		
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		namespace IO
		{

			public ref class FbxStreamOptionsCollada abstract sealed
			{
			public:
				static String^ TRIANGULATE = "TRIANGULATE";
				static String^ SINGLEMATRIX = "SINGLE MATRIX";
				static String^ FRAME_COUNT = "FRAME COUNT";
				static String^ FRAME_RATE = "FRAME RATE";
				static String^ START = "START";
				static String^ TAKE_NAME = "TAKE NAME";
			};
			
			/**	\brief This class is used for accessing the Import options of Collada files.
			* The content of KfbxStreamOptionsCollada is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsColladaReader : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsColladaReader(KFbxStreamOptionsColladaReader* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
				
				FBXOBJECT_DECLARE(FbxStreamOptionsColladaReader);
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsColladaReader);				
			public:				
				/** Reset all options to default values
				*/
				//void Reset();

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();

#endif
			};

			/**	\brief This class is used for accessing the Export options of Collada files.
			* The content of KfbxStreamOptionsCollada is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptionsColladaWriter : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsColladaWriter(KFbxStreamOptionsColladaWriter* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}

				REF_DECLARE(FbxEmitter,KFbxStreamOptionsColladaWriter);
				FBXOBJECT_DECLARE(FbxStreamOptionsColladaWriter);				
			public:

				/** Reset all the options to default value
				*/
				//void Reset();

				/** Sets the Time Mode
				* \param pTimeMode            The time mode to be used.
				* \param pCustomFrameRate     The value of the frame rate. 
				*/
				void SetTimeMode(FbxTime::TimeMode timeMode, double customFrameRate);
				void SetTimeMode(FbxTime::TimeMode timeMode);


				/** Get the Frame Period
				* \return     KTime of the Frame Period
				*/
				property FbxTime^ FramePeriod
				{
					FbxTime^ get();
				}

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();
#endif
			};

		}
	}
}