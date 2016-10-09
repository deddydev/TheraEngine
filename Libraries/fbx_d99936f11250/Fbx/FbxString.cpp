#pragma once
#include "stdafx.h"
#include "FbxString.h"
#include "FbxMemoryAllocator.h"

namespace Skill
{
	namespace FbxSDK
	{		
		void FbxStringManaged::CollectManagedMemory()
		{
		}		

		//void FbxString::CharToWchar_t(char* orig , wchar_t* wcstring)
		//{
		//	size_t origsize = strlen(orig) + 1;				
		//	size_t convertedChars = 0;				
		//	mbstowcs_s(&convertedChars, wcstring, origsize, orig, _TRUNCATE);				
		//}

		//System::String^ FbxString::CharToString(char* orig)
		//{
		//	return gcnew String(orig);
		//}
		//void FbxString::Wchar_tToChar(wchar_t* orig , char* nstring)
		//{
		//	size_t origsize = wcslen(orig) + 1;				
		//	size_t convertedChars = 0;				
		//	wcstombs_s(&convertedChars, nstring, origsize, orig, _TRUNCATE);				
		//}

		//System::String^ FbxString::Wchar_tToString(wchar_t* orig)
		//{
		//	return gcnew String(orig);
		//}

		//void FbxString::StringToChar(System::String^ orig , char* nstring)
		//{
		//	pin_ptr<const wchar_t> wch = PtrToStringChars(orig);

		//	// Convert to a char*
		//	size_t origsize = wcslen(wch) + 1;				
		//	size_t convertedChars = 0;				
		//	wcstombs_s(&convertedChars, nstring, origsize, wch, _TRUNCATE);				
		//}
		//void FbxString::StringToWchar_t(System::String^ orig,wchar_t* wcstring)
		//{
		//	pin_ptr<const wchar_t> wch = PtrToStringChars(orig);
		//	size_t origsize = wcslen(wch) + 1;
		//	wcscpy_s(wcstring,origsize,wch);
		//}
		FbxStringManaged^ FbxStringManaged::Create()		
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(FbxString::Create());			
			return s;				
		}
		FbxStringManaged^ FbxStringManaged::Create(FbxStringManaged^ str)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(FbxString::Create(str->_Ref()));			
			return s;
		}
		void FbxStringManaged::Destroy()
		{
			_Ref()->Destroy();				
		}			
		FbxStringManaged^ FbxStringManaged::DestroyIfEmpty(FbxStringManaged^ str)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(FbxString::DestroyIfEmpty(str->_Ref()));			
			return s;
		}			
		FbxStringManaged^ FbxStringManaged::StringOrEmpty(FbxStringManaged^ str)
		{
			FbxStringManaged^ s =  gcnew FbxStringManaged(FbxString::StringOrEmpty(str->_Ref()));			
			return s;
		}					
		FbxStringManaged::FbxStringManaged(FbxStringManaged^ str)
		{
			this->_SetPointer(new FbxString(*str->_Ref()),true);			
		}
		FbxStringManaged::FbxStringManaged(char* pStr)
		{
			this->_SetPointer(new FbxString(pStr),true);			
		}
		FbxStringManaged::FbxStringManaged(System::String^ str)
		{							
			STRINGTO_CONSTCHAR_ANSI(pStr,str);
			this->_SetPointer(new FbxString(pStr),true);
			FREECHARPOINTER(pStr);			
		}
		FbxStringManaged::FbxStringManaged(char c, size_t pNbRepeat)
		{
			this->_SetPointer(new FbxString(c,pNbRepeat),true);			
		}
		FbxStringManaged::FbxStringManaged(char c)
		{
			this->_SetPointer(new FbxString(c),true);			
		}
		FbxStringManaged::FbxStringManaged(char* pCharPtr, size_t pLength)
		{
			this->_SetPointer(new FbxString(pCharPtr,pLength),true);			
		}
		FbxStringManaged::FbxStringManaged(int value)
		{
			this->_SetPointer(new FbxString(value),true);			
		}
		FbxStringManaged::FbxStringManaged(float value)
		{
			this->_SetPointer(new FbxString(value),true);			
		}
		FbxStringManaged::FbxStringManaged(double value)
		{
			this->_SetPointer(new FbxString(value),true);			
		}		
		bool FbxStringManaged::IsOK::get()
		{
			return _Ref()->IsOK();
		}			
		FbxStringManaged^ FbxStringManaged::Invalidate()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Invalidate());			
			return s;				
		}			
		size_t FbxStringManaged::Length::get()
		{
			return _Ref()->GetLen();
		}			
		bool FbxStringManaged::IsEmpty::get()
		{
			return _Ref()->IsEmpty(); 
		}			
		FbxStringManaged^ FbxStringManaged::Empty()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Empty());			
			return s;
		}
		char FbxStringManaged::GetChar(int index)
		{
			if((size_t)index < _Ref()->GetLen() && index >= 0)
				return (*_Ref())[index];
			else
				return '!';
		}

		char FbxStringManaged::default::get(int index)
		{
			return this->GetChar(index);
		}

		void FbxStringManaged::CopyFrom(FbxStringManaged^ str)
		{
			*this->_FbxString = *str->_Ref();
		}
		void FbxStringManaged::CopyFrom(char c)
		{
			*this->_FbxString = c;
		}
		void FbxStringManaged::CopyFrom(char* c)
		{
			*this->_FbxString = c;
		}
		void FbxStringManaged::CopyFrom(int value)
		{
			*this->_FbxString = value;
		}
		void FbxStringManaged::CopyFrom(float value)
		{
			*this->_FbxString = value;
		}
		void FbxStringManaged::CopyFrom(double value)
		{
			*this->_FbxString = value;
		}
		void FbxStringManaged::Swap(FbxStringManaged^ str)
		{
			_Ref()->Swap(*str->_Ref());
		}
		void FbxStringManaged::Append(char* pStr)
		{				
			_Ref()->Append(pStr);
		}
		void FbxStringManaged::Append(String^ str)
		{			
			STRINGTO_CONSTCHAR_ANSI(s,str);
			this->_Ref()->Append(s);
			FREECHARPOINTER(s);			
		}
		String^ FbxStringManaged::ToString()
		{				
			return gcnew String(_Ref()->Buffer());
		}
		int FbxStringManaged::Compare(const char* pStr)
		{
			return _Ref()->Compare(pStr);
		}
		int FbxStringManaged::Compare(String^ s)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,s);
			int r = Compare(ps);
			FREECHARPOINTER(ps);
			return r;
		}
		int FbxStringManaged::CompareNoCase( const char * pStr )
		{
			return _Ref()->CompareNoCase(pStr);
		}
		int FbxStringManaged::CompareNoCase(String^ s)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,s);
			int r = _Ref()->CompareNoCase(ps);
			FREECHARPOINTER(ps);
			return r;
		}
		FbxStringManaged^ FbxStringManaged::Mid(size_t first, size_t count)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Mid(first,count));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Mid(size_t first)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Mid(first));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Left(size_t count)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Left(count));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Right(size_t count)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Right(count));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Pad(PaddingType padding, size_t length, char car)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Pad((FbxString::PaddingType)padding,length,car));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::UnPad(PaddingType padding)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(&_Ref()->UnPad((FbxString::PaddingType)padding));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Upper()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Upper());			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Lower()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Lower());			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::Reverse()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->Reverse());			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::ConvertToUnix()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->ConvertToUnix());			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::ConvertToWindows()
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->ConvertToWindows());			
			return s;
		}
		int FbxStringManaged::Find(char c, size_t startPosition)
		{
			return _Ref()->Find(c,startPosition);
		}
		int FbxStringManaged::Find(char c)
		{
			return _Ref()->Find(c);
		}
		int FbxStringManaged::Find(const char* strSub, size_t startPosition)
		{
			return _Ref()->Find(strSub,startPosition);
		}
		int FbxStringManaged::Find(String^ s, size_t startPosition)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,s);
			int r = Find(ps,startPosition);
			FREECHARPOINTER(ps);
			return r;
		}
		int FbxStringManaged::Find(const char* strSub)
		{
			return _Ref()->Find(strSub);
		}
		int FbxStringManaged::Find(String^ s)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,s);
			int r = Find(ps);
			FREECHARPOINTER(ps);
			return r;
		}
		int FbxStringManaged::ReverseFind(char c)
		{
			return _Ref()->ReverseFind(c);
		}
		int FbxStringManaged::FindOneOf(const char * strCharSet, size_t startPosition)
		{
			return _Ref()->FindOneOf(strCharSet,startPosition);
		}
		int FbxStringManaged::FindOneOf(String^ str, size_t startPosition)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,str);
			int r = FindOneOf(ps,startPosition);
			FREECHARPOINTER(ps);
			return r;
		}

		int FbxStringManaged::FindOneOf(const char * strCharSet)
		{
			return _Ref()->FindOneOf(strCharSet);
		}
		int FbxStringManaged::FindOneOf(String^ str)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,str);
			int r = Find(ps);
			FREECHARPOINTER(ps);
			return r;
		}
		bool FbxStringManaged::FindAndReplace(const char* find, const char* replaceBy, size_t startPosition)
		{
			return _Ref()->FindAndReplace(find,replaceBy,startPosition);
		}
		bool FbxStringManaged::FindAndReplace(String^ find, String^ replaceBy, size_t startPosition)
		{			
			STRINGTO_CONSTCHAR_ANSI(f,find);
			STRINGTO_CONSTCHAR_ANSI(r,replaceBy);
			bool b = FindAndReplace(f,r,startPosition);
			FREECHARPOINTER(r);
			FREECHARPOINTER(f);
			return b;
		}

		bool FbxStringManaged::FindAndReplace(const char* find, const char* replaceBy)
		{
			return _Ref()->FindAndReplace(find,replaceBy);
		}
		bool FbxStringManaged::FindAndReplace(String^ find, String^ replaceBy)
		{
			STRINGTO_CONSTCHAR_ANSI(f,find);
			STRINGTO_CONSTCHAR_ANSI(r,replaceBy);
			bool b = FindAndReplace(f,r);
			FREECHARPOINTER(r);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxStringManaged::ReplaceAll( char find, char replaceBy )
		{
			return _Ref()->ReplaceAll(find,replaceBy);
		}
		int FbxStringManaged::GetTokenCount(const char* spans)
		{
			return _Ref()->GetTokenCount(spans);
		}
		int FbxStringManaged::GetTokenCount(String^ s)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,s);
			int r = _Ref()->GetTokenCount(ps);
			FREECHARPOINTER(ps);
			return r;
		}
		FbxStringManaged^ FbxStringManaged::GetToken(int tokenIndex, const char* spans)
		{
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->GetToken(tokenIndex,spans));			
			return s;
		}
		FbxStringManaged^ FbxStringManaged::GetToken(int tokenIndex, String^ spans)
		{			
			STRINGTO_CONSTCHAR_ANSI(ps,spans);
			FbxStringManaged^ s = gcnew FbxStringManaged(_Ref()->GetToken(tokenIndex,ps));			
			FREECHARPOINTER(ps);
			return s;
		}
		void FbxStringManaged::AllocatorPurge()
		{
			FbxString::AllocatorPurge();
		}
		void  FbxStringManaged::AllocatorRelease()
		{
			FbxString::AllocatorRelease();				
		}

	}
}