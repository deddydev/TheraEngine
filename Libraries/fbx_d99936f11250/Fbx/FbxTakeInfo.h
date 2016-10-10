#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{	
		namespace Arrays
		{
			ref class FbxLayerInfoRefArray;
		}
	}
}
using ::FbxSDK::Arrays;


{
	namespace FbxSDK
	{	
		ref class FbxStringManaged;
		ref class FbxTimeSpan;
		ref class FbxTime;
		ref class FbxThumbnail;
		
		public ref struct FbxLayerInfo : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxLayerInfo,KLayerInfo);
			INATIVEPOINTER_DECLARE(FbxLayerInfo,KLayerInfo);		
		protected:
			FbxStringManaged^ name;
		public:
			DEFAULT_CONSTRUCTOR(FbxLayerInfo,KLayerInfo);
			FbxLayerInfo(FbxLayerInfo^ layerInfo);

			REF_PROPERTY_GETSET_DECLARE(FbxStringManaged,Name);

			VALUE_PROPERTY_GETSET_DECLARE(int,ID);			
		};


		/** Contains take information prefetched from an imported file
		* and exported to an output file. 
		*/
		public ref class FbxTakeInfo : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxTakeInfo,KFbxTakeInfo);
			INATIVEPOINTER_DECLARE(FbxTakeInfo,KFbxTakeInfo);		
		/*protected:
			FbxString^ name;
			FbxString^ importName;
			FbxString^ description;
			FbxTimeSpan^ localTimeSpan;
			FbxTimeSpan^ referenceTimeSpan;
			FbxTime^ importOffset;
			FbxThumbnail^ takeThumbnail;
			Skill::FbxSDK::Arrays::FbxLayerInfoRefArray^ layerInfoList;*/					
		public:
			//! Constructor.
			DEFAULT_CONSTRUCTOR(FbxTakeInfo,KFbxTakeInfo);			
			FbxTakeInfo(FbxTakeInfo^ takeInfo);
			void CopyFrom(FbxTakeInfo^ takeInfo);

			//! Take name.
			REF_PROPERTY_GETSET_DECLARE(FbxStringManaged,Name);			

			/** Take name once imported in a scene.
			* Modify it if it has to be different than the take name in the imported file.
			* \remarks This field is only used when importing a scene.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxStringManaged,ImportName);			

			//! Take description.			
			REF_PROPERTY_GETSET_DECLARE(FbxStringManaged,Description);			

			/** Import/export flag.
			* Set to \c true by default. Set to \c false if the take must not be imported or exported.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Select);			

			//! Local time span, set to animation interval if left to default value.
			REF_PROPERTY_GET_DECLARE(FbxTimeSpan,LocalTimeSpan);			

			//! Reference time span, set to animation interval if left to default value.			
			REF_PROPERTY_GET_DECLARE(FbxTimeSpan,ReferenceTimeSpan);			

			/** Time value to offset the animation keys once imported in a scene.
			* Modify it if the animation of a take must be offset.
			* Its effect depends on the state of \c mImportOffsetType.
			* \remarks This field is only used when importing a scene.
			*/
			REF_PROPERTY_GET_DECLARE(FbxTime,ImportOffset);			

			/** EImportOffsetType Import offset types.
			* - \e eABSOLUTE
			* - \e eRELATIVE
			*/
			enum class TakeInfoImportOffsetType
			{
				Absolute = KFbxTakeInfo::eABSOLUTE,
				Relative= KFbxTakeInfo::eRELATIVE
			};

			/** Import offset type.
			* If set to \c eABSOLUTE, \c mImportOffset gives the absolute time of 
			* the first animation key and the appropriate time shift is applied 
			* to all of the other animation keys.
			* If set to \c eRELATIVE, \c mImportOffset gives the relative time 
			* shift applied to all the animation keys.
			*/
			property TakeInfoImportOffsetType ImportOffsetType
			{
				TakeInfoImportOffsetType get();
				void set(TakeInfoImportOffsetType value);
			}

			/**	Get the take thumbnail.
			* \return Pointer to the thumbnail.
			*/
			/** Set the take thumbnail.
			* \param pTakeThumbnail The referenced thumbnail object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxThumbnail,TakeThumbnail);					

			void CopyLayers(FbxTakeInfo^ takeInfo);

			REF_PROPERTY_GET_DECLARE(Skill::FbxSDK::Arrays::FbxLayerInfoRefArray,LayerInfoList);

			VALUE_PROPERTY_GETSET_DECLARE(int,CurrentLayer);			
		};

	}
}