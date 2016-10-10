#pragma once
#include "stdafx.h"
#include "FbxDataType.h"
#include "FbxType.h"
#include "FbxString.h"


{
	namespace FbxSDK
	{				

		void FbxDataTypeManaged::CollectManagedMemory()
		{
		}
		
		FbxDataTypeManaged^ FbxDataTypeManaged::Create(String^ name,FbxType type)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			KFbxDataType d = KFbxDataType::Create(n,(EFbxType)type);
			FREECHARPOINTER(n);
			return gcnew FbxDataTypeManaged(d);
		}
		FbxDataTypeManaged^ FbxDataTypeManaged::Create(String^ name,FbxDataTypeManaged^ dataType)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			KFbxDataType d = KFbxDataType::Create(n,*dataType->_Ref());
			FREECHARPOINTER(n);
			return gcnew FbxDataTypeManaged(d);
		}			

		FbxDataTypeManaged::FbxDataTypeManaged( FbxDataTypeManaged^ dataType )
		{
			_SetPointer(new KFbxDataType(*dataType->_Ref()),true);
		}			
		void FbxDataTypeManaged::Destroy()
		{
			_Ref()->Destroy();
		}
		void FbxDataTypeManaged::CopyFrom(FbxDataTypeManaged^ dataType)
		{
			*this->_Ref() = *dataType->_Ref();
		}			
		bool FbxDataTypeManaged::IsValid::get()
		{			
			return _Ref()->Valid();
		}
		bool FbxDataTypeManaged::Is(FbxDataTypeManaged^ dataType)
		{
			return _Ref()->Is(*dataType->_Ref());
		}
		FbxType FbxDataTypeManaged::Type::get()
		{
			return (FbxType)_Ref()->GetType();
		}			
		String^ FbxDataTypeManaged::Name::get()
		{
			return gcnew String(_Ref()->GetName());
		}			
	}
}