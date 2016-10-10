#pragma once
#include "stdafx.h"
#include "FbxProperty.h"
#include "FbxDataType.h"
#include "FbxObject.h"
#include "FbxClassId.h"
#include "FbxString.h"
#include "FbxQuery.h"
#include "FbxPropertyHandle.h"
#include "FbxCurveNode.h"
#include "FbxCurve.h"
#include "FbxColor.h"
#include "FbxDouble2.h"
#include "FbxDouble3.h"
#include "FbxVector2.h"
#include "FbxVector4.h"

namespace FbxSDK
{		
	void FbxPropertyManaged::CollectManagedMemory()
	{
		this->_Object = nullptr;
	}		
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxPropertyManaged^ compoundProperty,
		System::String^ name, FbxDataType^ dataType,System::String^  label,bool checkForDuplicate,bool %wasFound)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);			
		STRINGTO_CONSTCHAR_ANSI(l,label);

		bool b;
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(*compoundProperty->_Ref(),n,*dataType->_Ref(),
			l,checkForDuplicate,&b));
		wasFound = b;			
		FREECHARPOINTER(n);
		FREECHARPOINTER(l);
		return p;

	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxPropertyManaged^ compoundProperty,
		System::String^ name, FbxDataType^ dataType,System::String^  label)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);			
		STRINGTO_CONSTCHAR_ANSI(l,label);			
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(*compoundProperty->_Ref(),n,*dataType->_Ref(),
			l));			
		FREECHARPOINTER(n);
		FREECHARPOINTER(l);
		return p;

	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label,bool checkForDuplicate,bool %wasFound)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		STRINGTO_CONSTCHAR_ANSI(l,label);

		bool b;
		FbxProperty kp = FbxProperty::Create(obj->_Ref(),n,*dataType->_Ref(),	l,checkForDuplicate,&b);
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(kp);
		wasFound = b;			
		FREECHARPOINTER(n);
		FREECHARPOINTER(l);
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		STRINGTO_CONSTCHAR_ANSI(l,label);			
		FbxProperty kp = FbxProperty::Create(obj->_Ref(),n,*dataType->_Ref(),	l);
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(kp);			
		FREECHARPOINTER(n);
		FREECHARPOINTER(l);
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxObjectManaged^ obj, FbxPropertyManaged^ fromProperty, bool checkForDuplicate)
	{
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(obj->_Ref(),*fromProperty->_Ref(),checkForDuplicate));						 			
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxObjectManaged^ obj, FbxPropertyManaged^ fromProperty)
	{
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(obj->_Ref(),*fromProperty->_Ref()));
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxPropertyManaged^ compoundProperty,
		FbxPropertyManaged^ fromProperty, bool checkForDuplicate)
	{
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(*compoundProperty->_Ref(),*fromProperty->_Ref(),checkForDuplicate));			
		return p;
	}

	FbxPropertyManaged^ FbxPropertyManaged::Create(FbxPropertyManaged^ compoundProperty,
		FbxPropertyManaged^ fromProperty)
	{
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(FbxProperty::Create(*compoundProperty->_Ref(),*fromProperty->_Ref()));			
		return p;
	}

	void FbxPropertyManaged::Destroy(bool recursive, bool dependents)
	{
		_Ref()->Destroy(recursive,dependents);
		_Free = false;
	}
	void FbxPropertyManaged::Destroy()
	{
		_Ref()->Destroy();
		_Free = false;
	}		
	FbxPropertyManaged::FbxPropertyManaged(FbxPropertyManaged^ p)
	{
		_SetPointer(new FbxProperty(*p->_Ref()),true);			
	}
	FbxPropertyManaged::FbxPropertyManaged(FbxPropertyHandle^ p)
	{
		_SetPointer(new FbxProperty(*p->_Ref()),true);			
	}

	FbxDataType^ FbxPropertyManaged::PropertyDataType::get()
	{			
		return gcnew FbxDataType(_Ref()->GetPropertyDataType());
	}
	FbxStringManaged^ FbxPropertyManaged::Name::get()
	{
		return gcnew FbxStringManaged(_Ref()->GetName());
	}		         
	FbxStringManaged^ FbxPropertyManaged::HierarchicalName::get()
	{
		return gcnew FbxStringManaged(_Ref()->GetHierarchicalName());
	}
	FbxStringManaged^ FbxPropertyManaged::GetLabel(bool returnNameIfEmpty)
	{
		return gcnew FbxStringManaged(_Ref()->GetLabel(returnNameIfEmpty));
	}
	void FbxPropertyManaged::SetLabel(FbxStringManaged^ label)
	{
		_Ref()->SetLabel(*label->_Ref());
	}
	REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxPropertyManaged,KFbxObject,GetFbxObject(),FbxObjectManaged,Object);		

	int FbxPropertyManaged::UserTag::get()
	{
		return _Ref()->GetUserTag();
	}
	void FbxPropertyManaged::UserTag::set(int value)
	{
		_Ref()->SetUserTag(value);
	}			
	void FbxPropertyManaged::ModifyFlag(FbxPropertyFlags::FbxPropertyFlagsType flag, bool value)
	{
		_Ref()->ModifyFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag , value);
	}
	bool FbxPropertyManaged::GetFlag(FbxPropertyFlags::FbxPropertyFlagsType flag)					 
	{
		return _Ref()->GetFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
	}
	FbxInheritType FbxPropertyManaged::GetFlagInheritType( FbxPropertyFlags::FbxPropertyFlagsType flag )
	{
		return (FbxInheritType)_Ref()->GetFlagInheritType((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
	}
	bool FbxPropertyManaged::SetFlagInheritType(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flag, FbxInheritType type )
	{
		return _Ref()->SetFlagInheritType(
			(fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag,
			(KFbxInheritType)type);
	}
	bool FbxPropertyManaged::ModifiedFlag( FbxPropertyFlags::FbxPropertyFlagsType flag )
	{
		return _Ref()->ModifiedFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
	}			
	void FbxPropertyManaged::CopyFrom(FbxPropertyManaged^ p)
	{
		*_Ref() = *p->_Ref();
	}

	bool FbxPropertyManaged::CompareValue(FbxPropertyManaged^ prop)					 
	{
		return _Ref()->CompareValue(*prop->_Ref());
	}
	bool FbxPropertyManaged::CopyValue(FbxPropertyManaged^ prop)
	{
		return _Ref()->CopyValue(*prop->_Ref());
	}
	bool FbxPropertyManaged::IsValid::get()
	{
		return _Ref()->IsValid();
	}

	FbxInheritType FbxPropertyManaged::ValueInheritType::get()
	{
		return (FbxInheritType)_Ref()->GetValueInheritType(); 
	} 			
	bool FbxPropertyManaged::SetValueInheritType( FbxInheritType type )
	{
		return _Ref()->SetValueInheritType((KFbxInheritType)type);
	}
	bool FbxPropertyManaged::Modified::get()
	{
		return _Ref()->Modified();
	}			
	double FbxPropertyManaged::MinLimit::get()
	{
		return _Ref()->GetMinLimit();
	}
	void FbxPropertyManaged::MinLimit::set(double value)
	{
		_Ref()->SetMinLimit(value);
	}
	bool  FbxPropertyManaged::HasMinLimit::get()
	{
		return _Ref()->HasMinLimit();
	}			
	bool  FbxPropertyManaged::HasMaxLimit::get()
	{
		return _Ref()->HasMaxLimit();
	}			
	double FbxPropertyManaged::MaxLimit::get()
	{
		return _Ref()->GetMaxLimit();
	}
	void FbxPropertyManaged::MaxLimit::set(double value)
	{
		_Ref()->SetMaxLimit(value);
	}			
	void FbxPropertyManaged::SetLimits(double min, double max)
	{
		_Ref()->SetLimits(min,max);
	}
	int FbxPropertyManaged::AddEnumValue(System::String^ value)
	{			
		STRINGTO_CONSTCHAR_ANSI(v,value);
		int r = _Ref()->AddEnumValue(v);
		FREECHARPOINTER(v);
		return r;
	}
	void FbxPropertyManaged::InsertEnumValue(int index, System::String^ value)					 
	{			
		STRINGTO_CONSTCHAR_ANSI(v,value);
		_Ref()->InsertEnumValue(index,v);
		FREECHARPOINTER(v);
	}
	int FbxPropertyManaged::EnumCount::get()
	{
		return _Ref()->GetEnumCount();
	}			
	void FbxPropertyManaged::SetEnumValue(int index,System::String^ value)
	{			
		STRINGTO_CONSTCHAR_ANSI(v,value);
		_Ref()->SetEnumValue(index,v);
		FREECHARPOINTER(v);
	}
	void FbxPropertyManaged::RemoveEnumValue(int index)
	{
		_Ref()->RemoveEnumValue(index);
	}
	System::String^ FbxPropertyManaged::GetEnumValue(int index)
	{
		return gcnew System::String(_Ref()->GetEnumValue(index));
	}
	bool FbxPropertyManaged::IsRoot::get()
	{
		return _Ref()->IsRoot();
	}			
	FbxPropertyManaged^ FbxPropertyManaged::GetParent()					 
	{
		return gcnew FbxPropertyManaged(_Ref()->GetParent());
	}
	bool FbxPropertyManaged::SetParent(FbxPropertyManaged^ other )
	{
		return _Ref()->SetParent(*other->_Ref());
	}
	FbxPropertyManaged^ FbxPropertyManaged::GetChild()
	{
		return gcnew FbxPropertyManaged(_Ref()->GetChild());
	}
	FbxPropertyManaged^ FbxPropertyManaged::GetSibling()
	{
		return gcnew FbxPropertyManaged (_Ref()->GetSibling());
	}
	FbxPropertyManaged^ FbxPropertyManaged::GetFirstDescendent()
	{
		return gcnew FbxPropertyManaged (_Ref()->GetFirstDescendent());
	}
	FbxPropertyManaged^ FbxPropertyManaged::GetNextDescendent(FbxPropertyManaged^ p)
	{
		return gcnew FbxPropertyManaged(_Ref()->GetNextDescendent(*p->_Ref()));
	}
	FbxPropertyManaged^ FbxPropertyManaged::Find (System::String^ name,bool caseSensitive)
	{						 			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(_Ref()->Find(n,caseSensitive));
		FREECHARPOINTER(n);
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::Find (System::String^ name,FbxDataType^ dataType, bool caseSensitive)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(_Ref()->Find(n,*dataType->_Ref(),caseSensitive));
		FREECHARPOINTER(n);
		return p;
	}
	FbxPropertyManaged^ FbxPropertyManaged::FindHierarchical (System::String^ name,bool caseSensitive)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		FbxPropertyManaged^ p = gcnew FbxPropertyManaged(_Ref()->FindHierarchical(n,caseSensitive));
		FREECHARPOINTER(n);
		return p;
	}
	void FbxPropertyManaged::BeginCreateOrFindProperty()
	{
		_Ref()->BeginCreateOrFindProperty();
	}
	void FbxPropertyManaged::EndCreateOrFindProperty()
	{
		_Ref()->EndCreateOrFindProperty();
	}		

	FbxPropertyManaged::FbxPropertyNameCache::FbxPropertyNameCache(FbxPropertyManaged^ prop)			            
	{
		_SetPointer(new KFbxProperty::KFbxPropertyNameCache(*prop->_Ref()),true);			
	}
	void FbxPropertyManaged::FbxPropertyNameCache::CollectManagedMemory()
	{
	}


	bool FbxPropertyManaged::SetArraySize( int size, bool variableArray )
	{
		return _Ref()->SetArraySize(size,variableArray );
	}
	int  FbxPropertyManaged::GetArraySize()
	{
		return _Ref()->GetArraySize();
	}
	FbxPropertyManaged^ FbxPropertyManaged::GetArrayItem(int index)
	{
		KFbxProperty p = _Ref()->GetArrayItem(index);
		if(p.IsValid())
			return gcnew FbxPropertyManaged(p);
		return nullptr;
	}
	FbxPropertyManaged^ FbxPropertyManaged::default::get(int index)
	{
		return GetArrayItem(index);
	}


	FbxCurveNode^ FbxPropertyManaged::CreateKFCurveNode(String^ takeName)
	{
		if(takeName)
		{
			STRINGTO_CONSTCHAR_ANSI(n,takeName);
			KFCurveNode* c = _Ref()->CreateKFCurveNode(n);
			FREECHARPOINTER(n);
			if(c)
				return gcnew FbxCurveNode(c);
			else
				return nullptr;
		}
		else
		{
			KFCurveNode* c = _Ref()->CreateKFCurveNode();				
			if(c)
				return gcnew FbxCurveNode(c);
			else
				return nullptr;
		}
		return nullptr;
	}
	FbxCurveNode^ FbxPropertyManaged::GetKFCurveNode(bool createAsNeeded, String^ takeName)
	{
		if(takeName)
		{
			STRINGTO_CONSTCHAR_ANSI(n,takeName);
			KFCurveNode* c = _Ref()->GetKFCurveNode(createAsNeeded,n);
			FREECHARPOINTER(n);
			if(c)
				return gcnew FbxCurveNode(c);
			else
				return nullptr;
		}
		else
		{
			KFCurveNode* c = _Ref()->GetKFCurveNode(createAsNeeded);				
			if(c)
				return gcnew FbxCurveNode(c);
			else
				return nullptr;
		}
		return nullptr;
	}

	FbxCurve^ FbxPropertyManaged::GetKFCurve(String^ channel)
	{
		const char* ch = NULL;			
		FbxCurve^ curve = nullptr;
		if(channel)
		{
			IntPtr _ch = Marshal::StringToHGlobalAnsi(channel);
			ch = static_cast<const char*>(_ch.ToPointer());
		}						
		KFCurve* c = _Ref()->GetKFCurve(ch);
		if(c)
			curve = gcnew FbxCurve(c);
		if(channel)
			FREECHARPOINTER(ch);			
		return curve;
	}
	FbxCurve^ FbxPropertyManaged::GetKFCurve(String^ channel, String^ takeName)
	{
		const char* ch = NULL;
		const char* tn = NULL;
		FbxCurve^ curve = nullptr;
		if(channel)
		{
			IntPtr _ch = Marshal::StringToHGlobalAnsi(channel);
			ch = static_cast<const char*>(_ch.ToPointer());
		}
		if(takeName)
		{
			IntPtr _tn = Marshal::StringToHGlobalAnsi(takeName);
			tn = static_cast<const char*>(_tn.ToPointer());
		}			
		KFCurve* c = _Ref()->GetKFCurve(ch,tn);
		if(c)
			curve = gcnew FbxCurve(c);
		if(channel)
			FREECHARPOINTER(ch);
		if(takeName)
			FREECHARPOINTER(tn);
		return curve;
	}

	bool FbxPropertyManaged::ConnectSrcObject (FbxObjectManaged^ obj,FbxConnectionType type)
	{
		return _Ref()->ConnectSrcObject(obj->_Ref(),(kFbxConnectionType)type);
	}
	bool FbxPropertyManaged::IsConnectedSrcObject (FbxObjectManaged^ obj)
	{
		return _Ref()->IsConnectedSrcObject(obj->_Ref());
	}
	bool FbxPropertyManaged::DisconnectSrcObject(FbxObjectManaged^ obj)
	{
		return _Ref()->DisconnectSrcObject(obj->_Ref());
	}

	bool FbxPropertyManaged::DisconnectAllSrcObject()
	{
		return _Ref()->DisconnectAllSrcObject();
	}
	bool FbxPropertyManaged::DisconnectAllSrcObject(FbxCriteria^ criteria)
	{
		return _Ref()->DisconnectAllSrcObject(*criteria->_Ref());
	}
	bool FbxPropertyManaged::DisconnectAllSrcObject(FbxClassId^ classId)
	{
		return _Ref()->DisconnectAllSrcObject(*classId->_Ref());
	}
	bool FbxPropertyManaged::DisconnectAllSrcObject(FbxClassId^ classId, FbxCriteria^ criteria)
	{
		return _Ref()->DisconnectAllSrcObject(*classId->_Ref(),*criteria->_Ref());
	}

	int FbxPropertyManaged::SrcObjectCount::get()
	{
		return _Ref()->GetSrcObjectCount();
	}

	int FbxPropertyManaged::GetSrcObjectCount(FbxCriteria^ criteria)
	{
		return _Ref()->GetSrcObjectCount(*criteria->_Ref());
	}
	int FbxPropertyManaged::GetSrcObjectCount(FbxClassId^ classId)					 
	{
		return _Ref()->GetSrcObjectCount(*classId->_Ref());
	}
	int FbxPropertyManaged::GetSrcObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
	{
		return _Ref()->GetSrcObjectCount(*classId->_Ref(),*criteria->_Ref());
	}

	FbxObjectManaged^ FbxPropertyManaged::GetSrcObject(int index)
	{
		KFbxObject* obj = _Ref()->GetSrcObject(index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::GetSrcObject(FbxCriteria^ criteria,int index)
	{
		KFbxObject* obj = _Ref()->GetSrcObject(*criteria->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;			
	}
	FbxObjectManaged^ FbxPropertyManaged::GetSrcObject(FbxClassId^ classId,int index)
	{
		KFbxObject* obj = _Ref()->GetSrcObject(*classId->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;			
	}
	FbxObjectManaged^ FbxPropertyManaged::GetSrcObject(FbxClassId^ classId, FbxCriteria^ criteria,int index)
	{
		KFbxObject* obj = _Ref()->GetSrcObject(*classId->_Ref(),*criteria->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;						
	}

	FbxObjectManaged^ FbxPropertyManaged::FindSrcObject(System::String^ name,int startIndex)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindSrcObject(n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindSrcObject(FbxCriteria^ criteria,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindSrcObject(*criteria->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindSrcObject(FbxClassId^ classId,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindSrcObject(*classId->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindSrcObject(*classId->_Ref(),*criteria->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	bool FbxPropertyManaged::ConnectDstObject(FbxObjectManaged^ obj,FbxConnectionType type)
	{
		return _Ref()->ConnectDstObject(obj->_Ref(), (kFbxConnectionType)type);
	}
	bool FbxPropertyManaged::IsConnectedDstObject(FbxObjectManaged^ obj)
	{
		return _Ref()->IsConnectedDstObject(obj->_Ref());
	}
	bool FbxPropertyManaged::DisconnectDstObject(FbxObjectManaged^ obj)
	{
		return _Ref()->DisconnectDstObject(obj->_Ref());
	}

	bool FbxPropertyManaged::DisconnectAllDstObject()
	{
		return _Ref()->DisconnectAllDstObject();
	}
	bool FbxPropertyManaged::DisconnectAllDstObject(FbxCriteria^ criteria)
	{
		return _Ref()->DisconnectAllDstObject(*criteria->_Ref());
	}
	bool FbxPropertyManaged::DisconnectAllDstObject(FbxClassId^ classId)
	{
		return _Ref()->DisconnectAllDstObject(*classId->_Ref());
	}
	bool FbxPropertyManaged::DisconnectAllDstObject(FbxClassId^ classId,FbxCriteria^ criteria)
	{
		return _Ref()->DisconnectAllDstObject(*classId->_Ref(),*criteria->_Ref());
	}

	int FbxPropertyManaged::DstObjectCount::get()
	{
		return _Ref()->GetDstObjectCount();
	}

	int FbxPropertyManaged::GetDstObjectCount(FbxCriteria^ criteria)
	{
		return _Ref()->GetDstObjectCount(*criteria->_Ref());
	}
	int FbxPropertyManaged::GetDstObjectCount(FbxClassId^ classId)					 
	{
		return _Ref()->GetDstObjectCount(*classId->_Ref());
	}
	int FbxPropertyManaged::GetDstObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
	{
		return _Ref()->GetDstObjectCount(*classId->_Ref(),*criteria->_Ref());
	}			

	FbxObjectManaged^ FbxPropertyManaged::GetDstObject(int index)
	{
		KFbxObject* obj = _Ref()->GetDstObject(index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::GetDstObject(FbxCriteria^ criteria,int index)
	{
		KFbxObject* obj = _Ref()->GetDstObject(*criteria->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;			
	}
	FbxObjectManaged^ FbxPropertyManaged::GetDstObject(FbxClassId^ classId,int index)
	{
		KFbxObject* obj = _Ref()->GetDstObject(*classId->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;						
	}
	FbxObjectManaged^ FbxPropertyManaged::GetDstObject(FbxClassId^ classId, FbxCriteria^ criteria,int index)
	{
		KFbxObject* obj = _Ref()->GetDstObject(*classId->_Ref(),*criteria->_Ref(), index);
		if(obj)
			return FbxCreator::CreateFbxObject(obj);
		return nullptr;			
	}

	FbxObjectManaged^ FbxPropertyManaged::FindDstObject(System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindDstObject(n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindDstObject(FbxCriteria^ criteria,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindDstObject(*criteria->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindDstObject(FbxClassId^ classId,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindDstObject(*classId->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	FbxObjectManaged^ FbxPropertyManaged::FindDstObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxObject* o = _Ref()->FindDstObject(*classId->_Ref(),*criteria->_Ref(),n,startIndex);
		FREECHARPOINTER(n);
		if(o)
			return FbxCreator::CreateFbxObject(o);
		return nullptr;
	}
	bool FbxPropertyManaged::ConnectSrcProperty(FbxPropertyManaged^ p)
	{
		return _Ref()->ConnectSrcProperty(*p->_Ref());
	}
	bool FbxPropertyManaged::IsConnectedSrcProperty (FbxPropertyManaged^ p)
	{
		return _Ref()->IsConnectedSrcProperty(*p->_Ref());
	}
	bool FbxPropertyManaged::DisconnectSrcProperty (FbxPropertyManaged^p)
	{
		return _Ref()->DisconnectSrcProperty(*p->_Ref());
	}
	int FbxPropertyManaged::SrcPropertyCount::get()
	{
		return _Ref()->GetSrcPropertyCount();
	}

	FbxPropertyManaged^ FbxPropertyManaged::GetSrcProperty (int index)
	{
		return gcnew FbxPropertyManaged(_Ref()->GetSrcProperty(index));
	}
	FbxPropertyManaged^ FbxPropertyManaged::FindSrcProperty(System::String^ name,int startIndex)
	{			
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxProperty o = _Ref()->FindSrcProperty(n,startIndex);
		FREECHARPOINTER(n);
		if(o.IsValid())
			return gcnew FbxPropertyManaged(o);
		return nullptr;
	}

	bool FbxPropertyManaged::ConnectDstProperty(FbxPropertyManaged^ p)
	{
		return _Ref()->ConnectDstProperty(*p->_Ref());
	}
	bool FbxPropertyManaged::IsConnectedDstProperty  (FbxPropertyManaged^ p)
	{
		return _Ref()->IsConnectedDstProperty(*p->_Ref());
	}
	bool FbxPropertyManaged::DisconnectDstProperty(FbxPropertyManaged^ p)
	{
		return _Ref()->DisconnectDstProperty(*p->_Ref());
	}
	int FbxPropertyManaged::DstPropertyCount::get()
	{
		return _Ref()->GetDstPropertyCount();
	}


	FbxPropertyManaged^ FbxPropertyManaged::GetDstProperty (int index)
	{
		return gcnew FbxPropertyManaged(_Ref()->GetDstProperty(index));
	}
	FbxPropertyManaged^ FbxPropertyManaged::FindDstProperty(System::String^ name,int startIndex)
	{
		STRINGTO_CONSTCHAR_ANSI(n,name);
		KFbxProperty o = _Ref()->FindDstProperty(n,startIndex);
		FREECHARPOINTER(n);
		if(o.IsValid())
			return gcnew FbxPropertyManaged(o);
		return nullptr;
	}

	void FbxPropertyManaged::ClearConnectCache()
	{
		_Ref()->ClearConnectCache();
	}

	FbxDataType^ FbxPropertyManaged::UserPropertyTypeToDataType(FbxPropertyManaged::UserPropertyType type)
	{
		KFbxDataType dt = EUserPropertyTypeToDataType((KFbxProperty::EUserPropertyType)type);
		return gcnew FbxDataType(dt);
	}
	FbxPropertyManaged::UserPropertyType FbxPropertyManaged::DataTypeToUserPropertyType(FbxDataType^ dataType)
	{
		return (FbxPropertyManaged::UserPropertyType)DataTypeToEUserPropertyType(*dataType->_Ref());
	}

	bool FbxPropertyManaged::GetValueAsBool()
	{
		return KFbxGet<bool>(*_Ref());
	}
	int FbxPropertyManaged::GetValueAsInt()
	{
		return KFbxGet<int>(*_Ref());
	}
	double FbxPropertyManaged::GetValueAsDouble()
	{
		return KFbxGet<double>(*_Ref());
	}
	FbxColor^ FbxPropertyManaged::GetValueAsColor()
	{
		KFbxColor c = KFbxGet<KFbxColor>(*_Ref());
		return gcnew FbxColor(c);
	}
	FbxDouble2^ FbxPropertyManaged::GetValueAsDouble2()
	{
		fbxDouble2 d2 = KFbxGet<fbxDouble2>(*_Ref());
		return gcnew FbxDouble2(d2);
	}
	FbxDouble3^ FbxPropertyManaged::GetValueAsDouble3()
	{
		fbxDouble3 d3 = KFbxGet<fbxDouble3>(*_Ref());
		return gcnew FbxDouble3(d3);
	}
	float FbxPropertyManaged::GetValueAsFloat()
	{
		return KFbxGet<float>(*_Ref());
	}
	String^ FbxPropertyManaged::GetValueAsString()
	{
		FbxString kstr = KFbxGet<FbxString>(*_Ref());
		CONVERT_FbxString_TO_STRING(kstr,str);
		return str;
	}

	void FbxPropertyManaged::Set(bool value)
	{
		_Ref()->Set<bool>(value);
	}
	void FbxPropertyManaged::Set(int value)
	{
		_Ref()->Set<int>(value);
	}
	void FbxPropertyManaged::Set(double value)
	{
		_Ref()->Set<double>(value);
	}
	void FbxPropertyManaged::Set(FbxColor^ value)
	{
		_Ref()->Set<KFbxColor>(*value->_Ref());
	}
	void FbxPropertyManaged::Set(FbxDouble2^ value)
	{
		_Ref()->Set<fbxDouble2>(*value->_Ref());
	}
	void FbxPropertyManaged::Set(FbxDouble3^ value)
	{
		_Ref()->Set<fbxDouble3>(*value->_Ref());
	}
	void FbxPropertyManaged::Set(FbxVector4^ value)
	{
		_Ref()->Set<KFbxVector4>(*value->_Ref());
	}
	void FbxPropertyManaged::Set(FbxVector2^ value)
	{
		_Ref()->Set<KFbxVector2>(*value->_Ref());
	}
	void FbxPropertyManaged::Set(float value)
	{
		_Ref()->Set<float>(value);
	}
	void FbxPropertyManaged::Set(String^ value)
	{
		STRINGTO_CONSTCHAR_ANSI(v,value);
		FbxString kstr(v); 
		FREECHARPOINTER(v);
		_Ref()->Set<FbxString>(kstr);
	}






	FbxConnectEventManaged::FbxConnectEventManaged(FbxConnectEventType type,
		FbxConnectEventDirection dir, FbxPropertyManaged^  src, FbxPropertyManaged^ dst)			        
	{
		_SetPointer(new KFbxConnectEvent((eFbxConnectEventType)type,(eFbxConnectEventDirection)dir,
			src->_Ref(),dst->_Ref()),true);			
		this->_Source = src;
		this->_Destination = dst;
	}
	void FbxConnectEventManaged::CollectManagedMemory()
	{
		this->_Source = nullptr;
		this->_Destination = nullptr;			
	}		
	FbxConnectEventType FbxConnectEventManaged::Type::get()
	{
		return (FbxConnectEventType)_Ref()->GetType();
	}

	FbxConnectEventDirection FbxConnectEventManaged::Direction::get()
	{
		return (FbxConnectEventDirection)_Ref()->GetDirection();
	}

	FbxPropertyManaged^ FbxConnectEventManaged::Source::get()
	{
		return _Source;
	}

	FbxPropertyManaged^ FbxConnectEventManaged::Destination::get()
	{
		return _Destination;
	}
}