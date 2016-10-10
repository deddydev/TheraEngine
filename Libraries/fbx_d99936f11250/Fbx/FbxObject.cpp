#pragma once
#include "stdafx.h"
#include "FbxObject.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"
#include "FbxString.h"
#include "FbxStream.h"
#include "FbxEvaluationInfo.h"
#include "FbxProperty.h"
#include "FbxDataType.h"
#include "FbxDocument.h"
#include "FbxScene.h"
#include "FbxLibrary.h"
#include "FbxQuery.h"
#include "FbxObjectMetaData.h"


{
	namespace FbxSDK
	{
		FbxObjectManaged::FbxObjectManaged(FbxObject* obj) : FbxPlug(obj)
		{			
			_Free = false;
		}
		void FbxObjectManaged::CollectManagedMemory()
		{
			refTo = nullptr;
			this->_Document = nullptr;
			this->_RootDocument = nullptr;
			this->_Scene = nullptr;
			this->_ParentLibrary = nullptr;
			FbxPlug::CollectManagedMemory();
		}

		FbxObjectManaged::~FbxObjectManaged()
		{			
			CollectManagedMemory();
			this->!FbxObjectManaged();
		}
		FbxObjectManaged::!FbxObjectManaged()
		{			
		}
		CLONE_DEFINITION(FbxObjectManaged,FbxObject);
		FBXOBJECT_DEFINITION(FbxObjectManaged,FbxObject);		

		VALUE_PROPERTY_GET_DEFINATION(FbxObjectManaged,IsAReferenceTo(),bool,IsAReferenceTo);		
		FbxObjectManaged^ FbxObjectManaged::ReferenceTo::get()
		{
			return refTo;
		}
		VALUE_PROPERTY_GET_DEFINATION(FbxObjectManaged,IsReferencedBy(),bool,IsReferencedBy);
		VALUE_PROPERTY_GET_DEFINATION(FbxObjectManaged,GetReferencedByCount(),int,ReferencedByCount);		
		FbxObjectManaged^ FbxObjectManaged::GetReferencedBy(int index)
		{
			FbxObject* obj = _Ref()->GetReferencedBy(index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}

		String^ FbxObjectManaged::Name::get()
		{
			FbxString kstr = _Ref()->GetName();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		void FbxObjectManaged::Name::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			FbxString kstr(v);
			_Ref()->SetName(kstr);
		}

		String^ FbxObjectManaged::NameWithNameSpacePrefix::get()
		{
			FbxString kstr = _Ref()->GetNameWithNameSpacePrefix();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		String^ FbxObjectManaged::NameWithoutNameSpacePrefix::get()
		{
			FbxString kstr = _Ref()->GetNameWithoutNameSpacePrefix();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		String^ FbxObjectManaged::InitialName::get()
		{
			FbxString kstr = _Ref()->GetInitialName();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		void FbxObjectManaged::InitialName::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			FbxString kstr(v);
			_Ref()->SetInitialName(kstr);
		}

		String^ FbxObjectManaged::NameSpaceOnly::get()
		{
			FbxString kstr = _Ref()->GetNameSpaceOnly();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		void FbxObjectManaged::NameSpaceOnly::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			FbxString kstr(v);
			_Ref()->SetNameSpace(kstr);
		}

		String^ FbxObjectManaged::NameOnly::get()
		{
			FbxString kstr = _Ref()->GetNameOnly();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		String^ FbxObjectManaged::NameSpacePrefix::get()
		{
			FbxString kstr = _Ref()->GetNameSpacePrefix();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		String^ FbxObjectManaged::RemovePrefix(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			FbxString kstr = KFbxObject::RemovePrefix(n);
			//FREECHARPOINTER(n);
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;

		}
		String^ FbxObjectManaged::StripPrefix(FbxStringManaged^ name)
		{			
			FbxString kstr = KFbxObject::StripPrefix(*name->_Ref());
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		String^ FbxObjectManaged::StripPrefix(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxString kstr = KFbxObject::StripPrefix(n);
			FREECHARPOINTER(n);
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		KFbxObjectID FbxObjectManaged::UniqueID::get()
		{
			return _Ref()->GetUniqueID();
		}

		kFbxUpdateId FbxObjectManaged::GetUpdateId(FbxUpdateIdType updateId)
		{
			return _Ref()->GetUpdateId((KFbxObject::eFbxUpdateIdType)updateId);
		}
		int FbxObjectManaged::ContentUnload()
		{
			return _Ref()->ContentUnload();
		}

		int FbxObjectManaged::ContentLoad()
		{
			return _Ref()->ContentLoad();
		}

		bool FbxObjectManaged::ContentIsLoaded::get()
		{
			return _Ref()->ContentIsLoaded();
		}

		void FbxObjectManaged::ContentDecrementLockCount()
		{
			_Ref()->ContentDecrementLockCount();
		}		
		void FbxObjectManaged::ContentIncrementLockCount()
		{
			_Ref()->ContentIncrementLockCount();
		}
		bool FbxObjectManaged::ContentIsLocked::get()
		{
			return _Ref()->ContentIsLocked();
		}

		bool FbxObjectManaged::ContentWriteTo(FbxStream^ stream)
		{
			return _Ref()->ContentWriteTo(*stream->_Ref());
		}
		bool FbxObjectManaged::ContentReadFrom(FbxStream^ stream)
		{
			return _Ref()->ContentReadFrom(*stream->_Ref());
		}
		bool FbxObjectManaged::Selected::get()
		{
			return _Ref()->GetSelected();
		}
		void FbxObjectManaged::Selected::set(bool value)
		{
			_Ref()->SetSelected(value);
		}

		bool FbxObjectManaged::Evaluate(FbxPropertyManaged^ prop,FbxEvaluationInfo^ evaluationInfo)
		{
			return _Ref()->Evaluate(*prop->_Ref(),evaluationInfo->_Ref());
		}

		FbxPropertyManaged^ FbxObjectManaged::GetFirstProperty()
		{
			KFbxProperty p = _Ref()->GetFirstProperty();
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			return pro;
		}
		FbxPropertyManaged^ FbxObjectManaged::GetNextProperty(FbxPropertyManaged^ prop)
		{
			KFbxProperty p = _Ref()->GetNextProperty(*prop->_Ref());
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			return pro;
		}
		FbxPropertyManaged^ FbxObjectManaged::FindProperty(String^ name, bool caseSensitive)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty p = _Ref()->FindProperty(n,caseSensitive);
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			FREECHARPOINTER(n);
			return pro;
		}
		FbxPropertyManaged^ FbxObjectManaged::FindProperty(String^ name, FbxDataType^ dataType, bool caseSensitive)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty p = _Ref()->FindProperty(n,*dataType->_Ref(),caseSensitive);
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			FREECHARPOINTER(n);
			return pro;
		}
		FbxPropertyManaged^ FbxObjectManaged::FindPropertyHierarchical(String^ name, bool caseSensitive)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty p = _Ref()->FindPropertyHierarchical(n,caseSensitive);
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			FREECHARPOINTER(n);
			return pro;
		}

		/*FbxProperty^ FbxObject::FindPropertyHierarchical(String^ name, FbxDataType^ dataType, bool caseSensitive)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty p = _Ref()->FindPropertyHierarchical(n,*dataType->type,caseSensitive);
			FbxProperty^ pro = gcnew FbxProperty();
			*pro->pro = p;
			FREECHARPOINTER(n);
			return pro;
		}*/
		FbxPropertyManaged^ FbxObjectManaged::GetRootProperty()
		{
			KFbxProperty p = _Ref()->GetRootProperty();
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			return pro;
		}
		FbxPropertyManaged^ FbxObjectManaged::GetClassRootProperty()
		{
			KFbxProperty p = _Ref()->GetClassRootProperty();
			FbxPropertyManaged^ pro = gcnew FbxPropertyManaged();
			*pro->_FbxProperty = p;
			return pro;
		}

		String^ FbxObjectManaged::Url::get()
		{
			return gcnew String(_Ref()->GetUrl());
		}
		void FbxObjectManaged::Url::set(String^ value)
		{
			STRINGTOCHAR_ANSI(n,value);
			_Ref()->SetUrl(n);
			FREECHARPOINTER(n);
		}
		bool FbxObjectManaged::PopulateLoadSettings(FbxObjectManaged^ settings,String^ fileName)
		{
			STRINGTOCHAR_ANSI(f,fileName);
			bool b = _Ref()->PopulateLoadSettings(settings->_Ref(),f);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxObjectManaged::Load(String^ fileName)
		{
			STRINGTOCHAR_ANSI(f,fileName);
			bool b = _Ref()->Load(f);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxObjectManaged::PopulateSaveSettings(FbxObjectManaged^ settings,String^ fileName)
		{
			STRINGTOCHAR_ANSI(f,fileName);
			bool b = _Ref()->PopulateSaveSettings(settings->_Ref(),f);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxObjectManaged::Save(String^ fileName)
		{		
			STRINGTOCHAR_ANSI(f,fileName);
			bool b = _Ref()->Save(f);
			FREECHARPOINTER(f);
			return b;		
		}		

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxObjectManaged,KFbxDocument,GetDocument(),FbxDocumentManaged,Document);		

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxObjectManaged,KFbxDocument,GetRootDocument(),FbxDocumentManaged,RootDocument);		

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxObjectManaged,KFbxScene,GetScene(),FbxSceneManaged,Scene);

		String^ FbxObjectManaged::Localize(String^ ID,String^ Default)
		{
			STRINGTO_CONSTCHAR_ANSI(id,ID);
			const char* s;
			if(Default )
			{
				STRINGTO_CONSTCHAR_ANSI(def,Default);
				s = _Ref()->Localize(id,def);				
				FREECHARPOINTER(def);
			}
			else
				s = _Ref()->Localize(id);
			FREECHARPOINTER(id);
			return gcnew String(s);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxObjectManaged,KFbxLibrary,GetParentLibrary(),FbxLibrary,ParentLibrary);		


		bool FbxObjectManaged::ConnectSrcObject(FbxObjectManaged^ obj,FbxConnectionType type)
		{
			return _Ref()->ConnectSrcObject(obj->_Ref(),(kFbxConnectionType)type);
		}
		bool FbxObjectManaged::IsConnectedSrcObject(FbxObjectManaged^ obj)
		{
			return _Ref()->IsConnectedSrcObject(obj->_Ref());
		}
		bool FbxObjectManaged::DisconnectSrcObject(FbxObjectManaged^ obj)
		{
			return _Ref()->DisconnectSrcObject(obj->_Ref());
		}

		/*bool FbxObject::DisconnectAllSrcObject()
		{
			return _Ref()->DisconnectAllSrcObject();
		}
		bool FbxObject::DisconnectAllSrcObject(FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllSrcObject(*criteria->_Ref());
		}
		bool FbxObject::DisconnectAllSrcObject(FbxClassId^ classId)
		{
			return _Ref()->DisconnectAllSrcObject(*classId->_Ref());
		}
		bool FbxObject::DisconnectAllSrcObject(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllSrcObject(*classId->_Ref(),*criteria->_Ref());
		}*/
		
		int FbxObjectManaged::GetSrcObjectCount()
		{
			return _Ref()->GetSrcObjectCount();
		}
		int FbxObjectManaged::GetSrcObjectCount(FbxCriteria^ criteria)
		{
			return _Ref()->GetSrcObjectCount(*criteria->_Ref());
		}
		int FbxObjectManaged::GetSrcObjectCount(FbxClassId^ classId)
		{
			return _Ref()->GetSrcObjectCount(*classId->_Ref());
		}
		/*int FbxObject::GetSrcObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->GetSrcObjectCount(*classId->_Ref(),*criteria->_Ref());
		}*/


		FbxObjectManaged^ FbxObjectManaged::GetSrcObject(int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}
		/*FbxObject^ FbxObject::GetSrcObject(FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*criteria->criteria,index);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}*/
		FbxObjectManaged^ FbxObjectManaged::GetSrcObject(FbxClassId^ classId,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*classId->_Ref(),index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}
		/*FbxObject^ FbxObject::GetSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*classId->id,*criteria->criteria,index);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}

		FbxObject^ FbxObject::FindSrcObject(String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindSrcObject(n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindSrcObject(FbxCriteria^ criteria,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindSrcObject(*criteria->criteria,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindSrcObject(FbxClassId^ classId,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindSrcObject(*classId->id,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindSrcObject(*classId->id,*criteria->criteria,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
*/

		bool FbxObjectManaged::ConnectDstObject(FbxObjectManaged^ obj,FbxConnectionType type)
		{
			return _Ref()->ConnectDstObject(obj->_Ref(),(kFbxConnectionType)type);
		}
		bool FbxObjectManaged::IsConnectedDstObject(FbxObjectManaged^ obj)
		{
			return _Ref()->IsConnectedDstObject (obj->_Ref());
		}
		bool FbxObjectManaged::DisconnectDstObject(FbxObjectManaged^ obj)
		{
			return _Ref()->DisconnectDstObject(obj->_Ref());
		}



		/*bool FbxObject::DisconnectAllDstObject()
		{
			return _Ref()->DisconnectAllDstObject();
		}
		bool FbxObject::DisconnectAllDstObject(FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllDstObject(*criteria->criteria);
		}
		bool FbxObject::DisconnectAllDstObject(FbxClassId^ classId)
		{
			return _Ref()->DisconnectAllDstObject(*classId->id);
		}
		bool FbxObject::DisconnectAllDstObject(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllDstObject(*classId->id,*criteria->criteria);
		}*/
		
		int FbxObjectManaged::GetDstObjectCount()
		{
			return _Ref()->GetDstObjectCount();
		}
		/*int FbxObject::GetDstObjectCount(FbxCriteria^ criteria)
		{
			return _Ref()->GetDstObjectCount(*criteria->_Ref());
		}*/
		int FbxObjectManaged::GetDstObjectCount(FbxClassId^ classId)
		{
			return _Ref()->GetDstObjectCount(*classId->_Ref());
		}
		/*int FbxObject::GetDstObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->GetDstObjectCount(*classId->_Ref(),*criteria->_Ref());
		}*/



		/*FbxObject^ FbxObject::GetDstObject(int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(index);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}*/
		/*FbxObject^ FbxObject::GetDstObject(FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*criteria->criteria,index);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}*/
		FbxObjectManaged^ FbxObjectManaged::GetDstObject(FbxClassId^ classId,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*classId->_Ref(),index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}
		/*FbxObject^ FbxObject::GetDstObject(FbxClassId^ classId,FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*classId->id,*criteria->criteria,index);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}*/

		/*FbxObject^ FbxObject::FindDstObject(String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindDstObject(n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindDstObject(FbxCriteria^ criteria,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindDstObject(*criteria->criteria,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindDstObject(FbxClassId^ classId,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindDstObject(*classId->id,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
		FbxObject^ FbxObject::FindDstObject(FbxClassId^ classId,FbxCriteria^ criteria,String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* obj = _Ref()->FindDstObject(*classId->id,*criteria->criteria,n,startIndex);
			FREECHARPOINTER(n);
			if(obj)
				return gcnew FbxObject(obj);
			return nullptr;
		}
*/

#ifndef DOXYGEN_SHOULD_SKIP_THIS					
		FbxClassId^ FbxObjectManaged::GetRuntimeClassId()
		{
			FbxClassId^ id = gcnew FbxClassId();
			*id->_Ref() = _Ref()->GetRuntimeClassId();
			return id;
		}
		void FbxObjectManaged::SetObjectFlags(ObjectFlag flags, bool value)
		{				
			_Ref()->SetObjectFlags((KFbxObject::EObjectFlag)flags,value);
		}
		bool FbxObjectManaged::GetObjectFlags(ObjectFlag flags)
		{
			return _Ref()->GetObjectFlags((KFbxObject::EObjectFlag)flags);
		}

		// All flags replaced at once. This includes overriding the runtime flags, so
		// most likely you'd want to do something like this:
		//						
		// All flags at once, as a bitmask		
		kUInt FbxObjectManaged::ObjectFlags::get()
		{
			return _Ref()->GetObjectFlags();
		}
		void FbxObjectManaged::ObjectFlags::set(kUInt value)
		{
			_Ref()->SetObjectFlags(value);
		}
		//FbxObjectHandle &GetPropertyHandle() { return RootProperty.mPropertyHandle; }

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		
		
	}
}