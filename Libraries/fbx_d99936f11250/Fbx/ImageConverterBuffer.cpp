#pragma once
#include "stdafx.h"
#include "FbxImageConverterBuffer.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"


namespace Skill
{
	namespace FbxSDK
	{	
		void FbxImageConverterBuffer::CollectManagedMemory()
		{
		}

		bool FbxImageConverterBuffer::IsValid::get()
		{
			if(_FbxImageConverterBuffer)
				return _Ref()->IsValid();
			return false;
		}
		bool FbxImageConverterBuffer::UseDataBuffer::get()
		{
			return _Ref()->UseDataBuffer();
		}
		int FbxImageConverterBuffer::Width::get()
		{
			return _Ref()->GetWidth();
		}
		int FbxImageConverterBuffer::Height::get()
		{
			return _Ref()->GetHeight();
		}
		int FbxImageConverterBuffer::ColorSpace::get()
		{
			return _Ref()->GetColorSpace();
		}
		char FbxImageConverterBuffer::PixelSize::get()
		{
			return _Ref()->GetPixelSize();
		}
		IntPtr FbxImageConverterBuffer::Data::get()
		{
			return IntPtr(_Ref()->GetData());
		}
		bool FbxImageConverterBuffer::OriginalFormat::get()
		{
			return _Ref()->GetOriginalFormat();
		}
		String^ FbxImageConverterBuffer::OriginalFileName::get()
		{
			FbxString kstr = _Ref()->GetOriginalFileName();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		void FbxImageConverterBuffer::Initialize(int width, int height, bool useDataBuffer, int colorSpace, char pixelSize)
		{
			_Ref()->Init(width,height,useDataBuffer,colorSpace,pixelSize);
		}
		void FbxImageConverterBuffer::SetOriginalFormat(bool state)
		{
			_Ref()->SetOriginalFormat(state);
		}			
		void FbxImageConverterBuffer::SetOriginalFileName(String^ filename)
		{
			STRINGTO_CONSTCHAR_ANSI(f,filename);
			FbxString s(f);
			FREECHARPOINTER(f);
			_Ref()->SetOriginalFileName(s);
		}


		FBXOBJECT_DEFINITION(FbxImageConverter,KFbxImageConverter);

		bool FbxImageConverter::Convert(int direction,String^ fileName, FbxImageConverterBuffer^ buffer)
		{
			STRINGTO_CONSTCHAR_ANSI(f,fileName);
			FbxString s(f);
			FREECHARPOINTER(f);
			return _Ref()->Convert(direction,s,*buffer->_Ref());
		}
	}
}