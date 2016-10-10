#pragma once
#include "stdafx.h"
#include "FbxStreamOptions.h"
#include "KFbxIO/kfbxstreamoptionsfbx.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;	
		namespace IO
		{	
			public ref class FbxStreamOptionsFbx abstract sealed
			{
			public:
				static String^ TAKE_NAME  = "CURRENT TAKE NAME";
				static String^ PASSWORD  = "PASSWORD";
				static String^ PASSWORD_ENABLE  = "PASSWORD ENABLE";
				static String^ MODEL = "MODEL";
				static String^ TEXTURE = "TEXTURE";
				static String^ MATERIAL = "MATERIAL";
				static String^ MEDIA = "MEDIA";
				static String^ LINK = "LINK";
				static String^ SHAPE = "SHAPE";
				static String^ GOBO = "GOBO";
				static String^ ANIMATION = "ANIMATION";
				static String^ CHARACTER = "CHARACTER";
				static String^ GLOBAL_SETTINGS = "GLOBAL SETTINGS";
				static String^ PIVOT = "PIVOT";
				static String^ MERGE_LAYER_AND_TIMEWARP = "MERGE LAYER AND TIMEWARP";
				static String^ CONSTRAINT = "CONSTRAINT";
				static String^ EMBEDDED = "EMBEDDED";
				static String^ MODEL_COUNT = "MODEL COUNT";
				static String^ DEVICE_COUNT = "DEVICE COUNT";
				static String^ CHARACTER_COUNT = "CHARACTER COUNT";
				static String^ ACTOR_COUNT  = "ACTOR COUNT";
				static String^ CONSTRAINT_COUNT = "CONSTRAINT_COUNT";
				static String^ MEDIA_COUNT = "MEDIA COUNT";
				static String^ TEMPLATE = "TEMPLATE";
				// Clone every external objects into the document when exporting?    (default: ON)
				static String^ COLLAPSE_EXTERNALS = "COLLAPSE EXTERNALS";
				// Can we compress arrays of sufficient size in files?               (default: ON)
				static String^ COMPRESS_ARRAYS = "COMPRESS ARRAYS";

				// ADVANCED OPTIONS -- SHOULD PROBABLY NOT BE IN ANY UI

				// Property to skip when looking for things to embed.
				// If you have more than one property to ignore (as is often the case) then you must
				// create sub-properties.
				// Property names must be the full hiearchical property name (ie: parent|child|child)
				static String^ EMBEDDED_PROPERTIES_SKIP = "EMBEDDED SKIP";

				// Compression level, from 0 (no compression) to 9 (eat your CPU)    (default: speed)
				static String^ COMPRESS_LEVEL = "COMPRESS LEVEL";

				// Minimum size before compression is even attempted, in bytes.     
				static String^ COMPRESS_MINSIZE = "COMPRESS MINSIZE";
			};

			public ref class FbxStreamOptionsFbxReader : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsFbxReader(KFbxStreamOptionsFbxReader* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
			public:			
				FBXOBJECT_DECLARE(FbxStreamOptionsFbxReader);				
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsFbxReader);

				/** Reset all options to default values
				*The default values is :
				* KFBXSTREAMOPT_FBX_CURRENT_TAKE_NAME    :Null
				* KFBXSTREAMOPT_FBX_PASSWORD             :Null
				* KFBXSTREAMOPT_FBX_PASSWORD_ENABLE      :false
				* KFBXSTREAMOPT_FBX_MODEL                :true
				* KFBXSTREAMOPT_FBX_TEXTURE              :true
				* KFBXSTREAMOPT_FBX_MATERIAL             :true
				* KFBXSTREAMOPT_FBX_MEDIA                :true
				* KFBXSTREAMOPT_FBX_LINK                 :true
				* KFBXSTREAMOPT_FBX_SHAPE                :true
				* KFBXSTREAMOPT_FBX_GOBO                 :true
				* KFBXSTREAMOPT_FBX_ANIMATION            :true
				* KFBXSTREAMOPT_FBX_CHARACTER            :true
				* KFBXSTREAMOPT_FBX_GLOBAL_SETTINGS      :true
				* KFBXSTREAMOPT_FBX_PIVOT                :true
				* KFBXSTREAMOPT_FBX_MERGE_LAYER_AND_TIMEWARP  :false
				* KFBXSTREAMOPT_FBX_CONSTRAINT           :true
				* KFBXSTREAMOPT_FBX_MODEL_COUNT          :0
				* KFBXSTREAMOPT_FBX_DEVICE_COUNT         :0
				* KFBXSTREAMOPT_FBX_CHARACTER_COUNT      :0
				* KFBXSTREAMOPT_FBX_ACTOR_COUNT          :0
				* KFBXSTREAMOPT_FBX_CONSTRAINT_COUNT     :0
				* KFBXSTREAMOPT_FBX_MEDIA_COUNT          :0
				* KFBXSTREAMOPT_FBX_TEMPLATE             :false
				*/

				//void Reset();			
												
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:
				CLONE_DECLARE();

				//KArrayTemplate<HKFbxTakeInfo> mTakeInfo;
				//HKFbxDocumentInfo mDocumentInfo;			
#endif
			};




			/**	\brief This class is used for accessing the Export options of Fbx files.
			* The content of KfbxStreamOptionsFbx is stored in the inherited Property of its parent (KFbxStreamOptions).
			* The export options include that:
			* KFBXSTREAMOPT_FBX_CURRENT_TAKE_NAME    :Current take name 
			* KFBXSTREAMOPT_FBX_PASSWORD             :The password
			* KFBXSTREAMOPT_FBX_PASSWORD_ENABLE      :If password enable
			* KFBXSTREAMOPT_FBX_MODEL                :If model export
			* KFBXSTREAMOPT_FBX_TEXTURE              :If texture export
			* KFBXSTREAMOPT_FBX_MATERIAL             :If material export
			* KFBXSTREAMOPT_FBX_MEDIA                :If media export
			* KFBXSTREAMOPT_FBX_LINK                 :If link export
			* KFBXSTREAMOPT_FBX_SHAPE                :If shape export
			* KFBXSTREAMOPT_FBX_GOBO                 :If gobo export
			* KFBXSTREAMOPT_FBX_ANIMATION            :If animation export
			* KFBXSTREAMOPT_FBX_CHARACTER            :If character export
			* KFBXSTREAMOPT_FBX_GLOBAL_SETTINGS      :If global settings export
			* KFBXSTREAMOPT_FBX_PIVOT                :If pivot export
			* KFBXSTREAMOPT_FBX_EMBEDDED             :If embedded
			* KFBXSTREAMOPT_FBX_CONSTRAINT           :If constrain export
			* KFBXSTREAMOPT_FBX_TEMPLATE             :If template export
			* KFBXSTREAMOPT_FBX_MODEL_COUNT          :The count of model
			* KFBXSTREAMOPT_FBX_DEVICE_COUNT         :The count of device
			* KFBXSTREAMOPT_FBX_CHARACTER_COUNT      :The count of character
			* KFBXSTREAMOPT_FBX_ACTOR_COUNT          :The count of actor
			* KFBXSTREAMOPT_FBX_CONSTRAINT_COUNT     :The count of constrain
			* KFBXSTREAMOPT_FBX_MEDIA_COUNT          :The count of media
			* KFBXSTREAMOPT_FBX_COLLAPSE_EXTERNALS   :Clone every external objects into the document when exporting
			* KFBXSTREAMOPT_FBX_COMPRESS_ARRAYS      :If compress arrays of sufficient size in files
			* KFBXSTREAMOPT_FBX_EMBEDDED_PROPERTIES_SKIP   :Property to skip when looking for things to embed.
			* KFBXSTREAMOPT_FBX_COMPRESS_LEVEL       :Compression level, from 0 (no compression) to 9
			* KFBXSTREAMOPT_FBX_COMPRESS_MINSIZE     :Minimum size before compression
			* 
			*/
			public ref class FbxStreamOptionsFbxWriter : FbxStreamOptionsManaged
			{
			internal:
				FbxStreamOptionsFbxWriter(KFbxStreamOptionsFbxWriter* instance) : FbxStreamOptionsManaged(instance)
				{
					_Free = false;
				}
			public:
				FBXOBJECT_DECLARE(FbxStreamOptionsFbxWriter);				
				REF_DECLARE(FbxEmitter,KFbxStreamOptionsFbxWriter);
				/** Reset all options to default values
				*The default values is :
				* KFBXSTREAMOPT_FBX_CURRENT_TAKE_NAME    :Null
				* KFBXSTREAMOPT_FBX_PASSWORD             :Null
				* KFBXSTREAMOPT_FBX_PASSWORD_ENABLE      :false
				* KFBXSTREAMOPT_FBX_MODEL                :true
				* KFBXSTREAMOPT_FBX_TEXTURE              :true
				* KFBXSTREAMOPT_FBX_MATERIAL             :true
				* KFBXSTREAMOPT_FBX_MEDIA                :true
				* KFBXSTREAMOPT_FBX_LINK                 :true
				* KFBXSTREAMOPT_FBX_SHAPE                :true
				* KFBXSTREAMOPT_FBX_GOBO                 :true
				* KFBXSTREAMOPT_FBX_ANIMATION            :true
				* KFBXSTREAMOPT_FBX_CHARACTER            :true
				* KFBXSTREAMOPT_FBX_GLOBAL_SETTINGS      :true
				* KFBXSTREAMOPT_FBX_PIVOT                :true
				* KFBXSTREAMOPT_FBX_EMBEDDED             :false
				* KFBXSTREAMOPT_FBX_CONSTRAINT           :true
				* KFBXSTREAMOPT_FBX_MODEL_COUNT          :0
				* KFBXSTREAMOPT_FBX_DEVICE_COUNT         :0
				* KFBXSTREAMOPT_FBX_CHARACTER_COUNT      :0
				* KFBXSTREAMOPT_FBX_ACTOR_COUNT          :0
				* KFBXSTREAMOPT_FBX_CONSTRAINT_COUNT     :0
				* KFBXSTREAMOPT_FBX_MEDIA_COUNT          :0
				* KFBXSTREAMOPT_FBX_TEMPLATE             :false
				* KFBXSTREAMOPT_FBX_COLLAPSE_EXTERNALS   :true
				* KFBXSTREAMOPT_FBX_COMPRESS_ARRAYS      :true
				* KFBXSTREAMOPT_FBX_EMBEDDED_PROPERTIES_SKIP  :Null
				* KFBXSTREAMOPT_FBX_COMPRESS_LEVEL       :1
				* KFBXSTREAMOPT_FBX_COMPRESS_MINSIZE     :1024
				*/
				//virtual void Reset();				
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			public:

				CLONE_DECLARE();

				//KArrayTemplate<HKFbxTakeInfo> mTakeInfo;
				//HKFbxDocumentInfo mDocumentInfo;			
#endif
			};




		}
	}
}