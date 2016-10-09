#pragma once
#include "stdafx.h"
#include "FbxIO.h"
#include "FbxError.h"
#include "FbxString.h"

namespace FbxSDK
{
	namespace IO
	{
		void FbxIOManaged::CollectManagedMemory()
		{
			_KError = nullptr;
		}			
		Version^ FbxIOManaged::CurrentVersion::get()
		{
			int minor,major,revision,build = 0;
			FbxIO::GetCurrentVersion(major,minor,revision);
			return gcnew Version(major,minor,build,revision);
		}
		bool FbxIOManaged::Initialize(String^ fileName)
		{				
			STRINGTO_CONSTCHAR_ANSI(f,fileName);				
			bool b = _Ref()->Initialize(f);
			FREECHARPOINTER(f);
			return b;
		}
		String^ FbxIOManaged::FileName::get()
		{
			FbxString s = _Ref()->GetFileName();
			CONVERT_FbxString_TO_STRING(s,str);
			return str;
		}				

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxIOManaged,GetError(),FbxErrorManaged,Error);

		FbxIOManaged::Error FbxIOManaged::LastErrorID::get()					
		{
			return (Error)_Ref()->GetLastErrorID();
		}				
		String^ FbxIOManaged::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}				
		void FbxIOManaged::GetMessage(FbxStringManaged^ message)
		{
			_Ref()->GetMessage(*message->_Ref());
		}
	}
}