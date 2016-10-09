#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "KFbxIO/kfbxstreamoptions3ds.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		namespace IO
		{	
			public ref class FbxStreamOptions3ds abstract sealed
			{
			public:
				static String^ AMBIENT_LIGHT = "AMBIENT LIGHT";
				static String^ REFERENCENODE = "REFERENCENODE";
				static String^ TEXTURE = "TEXTURE";
				static String^ MATERIAL = "MATERIAL";
				static String^ ANIMATION = "ANIMATION";
				static String^ MESH  = "MESH";
				static String^ LIGHT = "LIGHT";
				static String^ CAMERA = "CAMERA";				
				static String^ RESCALING = "RESCALING";
				static String^ FILTER = "FILTER";
				static String^ SMOOTHGROUP = "SMOOTHGROUP";
				static String^ TEXUVBYPOLY = "TEXUVBYPOLY";
				static String^ TAKE_NAME = "TAKE NAME";
				static String^ MESH_COUNT = "MESH COUNT";
				static String^ LIGHT_COUNT = "LIGHT COUNT";
				static String^ CAMERA_COUNT = "CAMERA COUNT";

			};

			/** \brief This class is used for accessing the Import options of 3ds files.
			* The content of KfbxStreamOptions3ds is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/
			public ref class FbxStreamOptions3dsReader : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptions3dsReader(KFbxStreamOptions3dsReader* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
				REF_DECLARE(FbxEmitter,KFbxStreamOptions3dsReader);

				FBXOBJECT_DECLARE(FbxStreamOptions3dsReader);				

			public:
				/** Reset all options to default values
				*/
				//void Reset();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
				CLONE_DECLARE();
#endif
			};


			/**	\brief This class is used for accessing the Export options of 3ds files.
			* The content of KfbxStreamOptions3ds is stored in the inherited Property of its parent (KFbxStreamOptions).
			*/

			public ref class FbxStreamOptions3dsWriter : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptions3dsWriter(KFbxStreamOptions3dsWriter* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
				REF_DECLARE(FbxEmitter,KFbxStreamOptions3dsWriter);

			public:
				FBXOBJECT_DECLARE(FbxStreamOptions3dsWriter);				

				/** Reset all options to default values
				*/
				//void Reset();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
				CLONE_DECLARE();
#endif
			};

		}
	}
}
