#pragma once
#include "stdafx.h"
#include "FbxDataType.h"
#include "FbxType.h"
#include "FbxString.h"

namespace Skill
{
	namespace FbxSDK
	{				

		void FbxDataType::CollectManagedMemory()
		{
		}
		
		FbxDataType^ FbxDataType::Create(String^ name,FbxType type)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			KFbxDataType d = KFbxDataType::Create(n,(EFbxType)type);
			FREECHARPOINTER(n);
			return gcnew FbxDataType(d);
		}
		FbxDataType^ FbxDataType::Create(String^ name,FbxDataType^ dataType)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			KFbxDataType d = KFbxDataType::Create(n,*dataType->_Ref());
			FREECHARPOINTER(n);
			return gcnew FbxDataType(d);
		}			

		FbxDataType::FbxDataType( FbxDataType^ dataType )
		{
			_SetPointer(new KFbxDataType(*dataType->_Ref()),true);
		}			
		void FbxDataType::Destroy()
		{
			_Ref()->Destroy();
		}
		void FbxDataType::CopyFrom(FbxDataType^ dataType)
		{
			*this->_Ref() = *dataType->_Ref();
		}			
		bool FbxDataType::IsValid::get()
		{			
			return _Ref()->Valid();
		}
		bool FbxDataType::Is(FbxDataType^ dataType)
		{
			return _Ref()->Is(*dataType->_Ref());
		}
		FbxType FbxDataType::Type::get()
		{
			return (FbxType)_Ref()->GetType();
		}			
		String^ FbxDataType::Name::get()
		{
			return gcnew String(_Ref()->GetName());
		}			
	}
}