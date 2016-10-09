#pragma once
#include "stdafx.h"
#include "FbxDocument.h"
#include "FbxPeripheral.h"
#include "FbxDocumentInfo.h"
#include "FbxTakeInfo.h"
#include "FbxError.h"
#include "FbxString.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxStringRefArray.h"

namespace Skill
{
	namespace FbxSDK
	{

		FBXOBJECT_DEFINITION(FbxDocumentManaged,KFbxDocument);

		void FbxDocumentManaged::CollectManagedMemory()
		{
			_DocumentInfo = nullptr;
			_Peripheral = nullptr;
			_KError = nullptr;
			_PathToRootDocument = nullptr;
			FbxCollectionManaged::CollectManagedMemory();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxDocumentManaged,KFbxDocumentInfo,GetDocumentInfo(),FbxDocumentInfo,DocumentInfo);		
		void FbxDocumentManaged::DocumentInfo::set(FbxDocumentInfo^ value)
		{
			_DocumentInfo = value;
			_Ref()->SetDocumentInfo(value->_Ref());
		}			

		void FbxDocumentManaged::Peripheral::set(FbxPeripheral^ value)
		{
			_Peripheral = value;
			_Ref()->SetPeripheral(_Peripheral->_Ref());
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxDocumentManaged,KFbxPeripheral,GetPeripheral(),FbxPeripheral,Peripheral);
		
		int FbxDocumentManaged::UnloadContent()
		{
			return _Ref()->UnloadContent();
		}
		int FbxDocumentManaged::LoadContent()
		{
			return _Ref()->LoadContent();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentManaged,GetPathToRootDocument(),FbxStringManaged,PathToRootDocument);		
		bool FbxDocumentManaged::CreateTake(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);			
			bool b = _Ref()->CreateTake(n);
			FREECHARPOINTER(n);
			return b;
		}
		bool FbxDocumentManaged::RemoveTake(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);			
			bool b = _Ref()->RemoveTake(n);
			FREECHARPOINTER(n);
			return b;
		}
		bool FbxDocumentManaged::SetCurrentTake(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);			
			bool b = _Ref()->SetCurrentTake(n);
			FREECHARPOINTER(n);
			return b;
		}
		String^ FbxDocumentManaged::GetCurrentTakeName()
		{
			return gcnew String(_Ref()->GetCurrentTakeName());
		}

		void FbxDocumentManaged::FillTakeNameArray(FbxStringRefArray^ nameArray)
		{
			_Ref()->FillTakeNameArray(*nameArray->_Ref());		
			nameArray->UpdateInternalList();
		}

		bool FbxDocumentManaged::SetTakeInfo(FbxTakeInfo^ takeInfo)
		{
			return _Ref()->SetTakeInfo(*takeInfo->_Ref());
		}
		FbxTakeInfo^ FbxDocumentManaged::GetTakeInfo(FbxStringManaged^ takeName)
		{
			return gcnew FbxTakeInfo(_Ref()->GetTakeInfo(*takeName->_Ref()));
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentManaged,GetError(),FbxErrorManaged,KError);
		
		FbxDocumentManaged::Error FbxDocumentManaged::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}			
		String^ FbxDocumentManaged::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}			
#ifndef DOXYGEN_SHOULD_SKIP_THIS		

		CLONE_DEFINITION(FbxDocumentManaged,KFbxDocument);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}