#pragma once
#include "stdafx.h"
#include "FbxError.h"
#include "FbxString.h"

namespace Skill
{
	namespace FbxSDK
	{

		void FbxErrorManaged::CollectManagedMemory()
		{
		}

//		FbxError::FbxError(KError* e)
//		{
//			er = e;
//			isNew = false;
//		}		
//		FbxError::FbxError()
//		{
//			er = new KError();
//			isNew = true;
//		}
//		FbxError::~FbxError()
//		{
//			this->!FbxError();
//		}
//		FbxError::!FbxError()
//		{
//			if(isNew && er)
//				delete er;
//			isNew = false;
//			er = nullptr;
//		}
//		FbxError::FbxError(array<String^>^ stringArray)
//		{								
//			int errorCount = stringArray->Length;
//			char** arr = new char*[errorCount];
//
//			for(int i = 0;i<errorCount;i++)
//			{
//				arr[i] = new char(FbxString::NumCharToCreateString);
//				FbxString::StringToChar(stringArray[i],arr[i]);
//			}
//			er = new KError(arr,errorCount);
//			isNew = true;
//		}
//		int FbxError::ErrorCount::get()
//		{
//			return er->GetErrorCount();
//		}			
//		String^ FbxError::GetErrorString(int index)
//		{
//			return gcnew String(er->GetErrorString(index));
//		}
//		void FbxError::SetLastError(int index, String^ str)
//		{
//			char* s = new char(FbxString::NumCharToCreateString);
//			FbxString::StringToChar(str,s);
//			er->SetLastError(index, s);
//		}
//		int FbxError::LastErrorID::get()
//		{
//			return er->GetLastErrorID();
//		}
//		void FbxError::LastErrorID::set(int value)
//		{
//			er->SetLastErrorID(value);
//		}								
//		String^ FbxError::LastErrorString::get()
//		{
//			return gcnew String(er->GetLastErrorString());
//		}
//		void FbxError::LastErrorString::set(String^ value)
//		{
//			char* s = new char(FbxString::NumCharToCreateString);
//			FbxString::StringToChar(value,s);
//			er->SetLastErrorString(s);
//		}			
//		void FbxError::ClearLastError()
//		{
//			er->ClearLastError();
//		}
//#ifndef DOXYGEN_SHOULD_SKIP_THIS								
//
//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		

	}
}