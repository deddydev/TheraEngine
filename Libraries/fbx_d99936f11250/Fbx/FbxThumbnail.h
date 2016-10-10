#pragma once
#include "stdafx.h"
#include "FbxObject.h"


{
	namespace FbxSDK
	{	
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** Simple class to hold RGBA values of a thumbnail image.
		* \nosubgrouping
		*/
		public ref class FbxThumbnail : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxThumbnail);
		internal:
			FbxThumbnail(KFbxThumbnail* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
			
			FBXOBJECT_DECLARE(FbxThumbnail);

		public:

			
			/**
			* \name Thumbnail properties
			*/
			//@{

			//! Pixel height of the thumbnail image
			VALUE_PROPERTY_GETSET_DECLARE(int,CustomHeight);

			//! Pixel width of the thumbnail image
			VALUE_PROPERTY_GETSET_DECLARE(int,CustomWidth);
			

			/** \enum EDataFormat Data format.
			* - \e eRGB_24
			* - \e eRGBA_32
			*/
			enum class DataFormat
			{
				RGB_24 = KFbxThumbnail::eRGB_24,  // 3 components
				RGBA_32 = KFbxThumbnail::eRGBA_32 // 4 components
			};				

			/** Get the data format.
			* \return Data format identifier for the thumbnail.
			*/
			/** Set the data format.
			* \param pDataFormat Data format identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(DataFormat,Data_Format);


			/** \enum EImageSize Image size.
			* - \e eNOT_SET
			* - \e e64x64
			* - \e e128x128
			* - \e eCUSTOM_SIZE
			*/
			enum class ImageSize
			{
				NotSet = 0,
				E64x64   = 64,
				E128x128 = 128,
				ECustomSize = -1
			};			

			/** Get the thumbnail dimensions.
			* \return Image size identifier.
			*/
			/** Set the thumbnail dimensions.
			* \param pImageSize Image size identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(ImageSize,Size);

			/** Get the thumbnail dimensions in bytes.
			* \return Thumbnail size in bytes.
			*/
			VALUE_PROPERTY_GET_DECLARE(unsigned long,SizeInBytes);


			//@}

			/**
			* \name Thumbnail data access
			*/
			//@{

			/** Fill the thumbnail image.
			* \param pImage Pointer to the image data. A copy
			* of the image data will be made.
			*	\remarks This pointer must point to a buffer region
			* that is at least Width * Height * Component count
			* bytes long. This pointer points to the upper left
			* corner of the image.
			* \remarks You must set the data format and the dimensions
			* before calling this function. If the image size is set to eCUSTOM_SIZE
			* the CustomHeight and CustomWidth properties must be set before calling
			* this function.
			* \return \c true if the thumbnail properties were set
			* before calling this funtion. \c false otherwise.
			*/
			bool SetThumbnailImage(array<kUByte>^ image);

			/** Get the thumbnail image.
			* \return Pointer to the image data, or \c NULL if the
			* thumbnail is empty.
			*/
			array<kUByte>^ GetThumbnailImage();

			//@}			 
		};

	}
}