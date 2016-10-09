#pragma once
#include "stdafx.h"
#include "FbxClassId.h"
#include "FbxPlug.h"
#include "FbxSdkManager.h"
#include "FbxString.h"
#include "FbxPropertyHandle.h"

namespace Skill
{
	namespace FbxSDK
	{							
		/*FbxPlugConstructorController::FbxPlugConstructorController()
		{
			collection = gcnew System::Collections::Generic::Dictionary<int ,FbxPlugConstructor^>();
		}					
		int FbxPlugConstructorController::Add(FbxPlugConstructor^ func)
		{
			++numFunc;
			collection->Add(numFunc , func);
			return numFunc;
		}
		KFbxPlug* FbxPlugConstructorController::CallFunction(int index,KFbxSdkManager& pManager, const char* pName, const KFbxPlug* pFrom, const char* pFBXType, const char* pFBXSubType)
		{
			Skill::FbxSDK::FbxSdkManager^ manager = gcnew Skill::FbxSDK::FbxSdkManager(&pManager);
			String^ name = nullptr;
			if(pName)
				name = gcnew String(pName);
			FbxPlugInfo^ from = nullptr;
			if(pFrom)
				from = gcnew FbxPlugInfo(pFrom);
			String^ fbxType = nullptr;
			if(pFBXType)
				fbxType = gcnew String(pFBXType);				
			String^ fbxSubType = nullptr;
			if(pFBXSubType)
				fbxSubType = gcnew String(pFBXSubType);

			FbxPlug^ p = collection[index](manager,name,from,fbxType,fbxSubType);
			if(p)
				return (KFbxPlug*)p->emitter;
			return nullptr;

		}*/		
					
		/*KFbxPlug* FbxClassId::ConstractorFunction(KFbxSdkManager& pManager, const char* pName, const KFbxPlug* pFrom, const char* pFBXType, const char* pFBXSubType)
		{
			return FbxPlugConstructorController::CallFunction(constractorIndex,pManager,pName,pFrom,pFBXType,pFBXSubType);
		}*/					

		void FbxClassId::CollectManagedMemory()
		{
			this->_Parent = nullptr;
		}
		FbxClassId::FbxClassId()
		{
			_SetPointer(new kFbxClassId(),true);			
		}
		void FbxClassId::Destroy()
		{
			_Ref()->Destroy();
		}				
		String^ FbxClassId::Name::get()
		{
			return gcnew String(_Ref()->GetName()); 
		}			
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxClassId,GetParent(),FbxClassId,Parent);
					
		FbxPlug^ FbxClassId::Create(FbxSdkManagerManaged^ manager, String^ name, FbxPlug^ from)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxPlug^ p = FbxCreator::CreateFbxPlug(_Ref()->Create(*manager->_Ref(),n,from->_Ref()));
			FREECHARPOINTER(n);
			return p;
		}			
		bool FbxClassId::Is(FbxClassId^ id)
		{
			return _Ref()->Is(*id->_Ref());
		}
		String^ FbxClassId::GetFbxFileTypeName(bool askParent)
		{
			return gcnew String(_Ref()->GetFbxFileTypeName(askParent));
		}
		String^ FbxClassId::GetFbxFileTypeName()
		{
			return gcnew String(_Ref()->GetFbxFileTypeName());
		}					    
		String^ FbxClassId::GetFbxFileSubTypeName()
		{
			return gcnew String(_Ref()->GetFbxFileSubTypeName());
		}			
		bool FbxClassId::IsValid::get()
		{
			if(!_Free && _FbxClassId )
				return _Ref()->IsValid();
			else
				return false;
		}				
		String^ FbxClassId::ObjectTypePrefix::get()
		{
			return gcnew String(_Ref()->GetObjectTypePrefix());
		}
		void FbxClassId::ObjectTypePrefix::set(String^ objectTypePrefix)
		{	
			STRINGTO_CONSTCHAR_ANSI(n,objectTypePrefix);
			_Ref()->SetObjectTypePrefix(n);
			FREECHARPOINTER(n);
		}				

		FbxPropertyHandle^ FbxClassId::GetRootClassDefaultPropertyHandle()
		{
			return gcnew FbxPropertyHandle(_Ref()->GetRootClassDefaultPropertyHandle());
		}							
		int FbxClassId::ClassInstanceIncRef::get()
		{
			return _Ref()->ClassInstanceIncRef();
		}				
		int FbxClassId::ClassInstanceDecRef::get()
		{
			return _Ref()->ClassInstanceDecRef();
		}				
		int FbxClassId::GetInstanceRef::get()
		{
			return _Ref()->GetInstanceRef();
		}				
	}
}