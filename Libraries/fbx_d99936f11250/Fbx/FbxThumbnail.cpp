#pragma once
#include "stdafx.h"
#include "FbxThumbnail.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


{
	namespace FbxSDK
	{			
		FBXOBJECT_DEFINITION(FbxThumbnail,KFbxThumbnail);

		int FbxThumbnail::CustomHeight::get()
		{
			return _Ref()->CustomHeight.Get();
		}
		void FbxThumbnail::CustomHeight::set(int value)
		{
			_Ref()->CustomHeight.Set(value);
		}

		int FbxThumbnail::CustomWidth::get()
		{
			return _Ref()->CustomWidth.Get();
		}
		void FbxThumbnail::CustomWidth::set(int value)
		{
			_Ref()->CustomWidth.Set(value);
		}			

		FbxThumbnail::DataFormat FbxThumbnail::Data_Format::get()
		{
			return (FbxThumbnail::DataFormat)_Ref()->GetDataFormat();
		}
		void FbxThumbnail::Data_Format::set(FbxThumbnail::DataFormat value)
		{
			_Ref()->SetDataFormat((KFbxThumbnail::EDataFormat)value);
		}


		FbxThumbnail::ImageSize FbxThumbnail::Size::get()
		{
			return (FbxThumbnail::ImageSize)_Ref()->GetSize();
		}
		void FbxThumbnail::Size::set(FbxThumbnail::ImageSize value)
		{
			_Ref()->SetSize((KFbxThumbnail::EImageSize)value);
		}

		unsigned long FbxThumbnail::SizeInBytes::get()
		{
			return _Ref()->GetSizeInBytes();
		}

		bool FbxThumbnail::SetThumbnailImage(array<kUByte>^ image)
		{
			if(image->Length != _Ref()->GetSizeInBytes())
				return false;

			kUByte* buffer = new kUByte[_Ref()->GetSizeInBytes()];
			for(int i = 0 ; i< image->Length;i++)
				buffer[i] = image[i];
			bool b = _Ref()->SetThumbnailImage(buffer);
			delete[] buffer;
			return b;
		}

		array<kUByte>^ FbxThumbnail::GetThumbnailImage()
		{
			kUByte* buffer = _Ref()->GetThumbnailImage();
			if(buffer)
			{
				array<kUByte>^ arr = gcnew array<kUByte>(_Ref()->GetSizeInBytes());
				for(int i = 0 ; i< arr->Length;i++)
					arr[i] = buffer[i];						
				return arr;
			}
			return nullptr;
		}
	}
}