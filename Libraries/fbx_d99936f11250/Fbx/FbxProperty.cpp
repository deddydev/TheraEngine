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


namespace Skill
{
	namespace FbxSDK
	{		

		void FbxProperty::CollectManagedMemory()
		{
			this->_Object = nullptr;
		}		
		FbxProperty^ FbxProperty::Create(FbxProperty^ compoundProperty,
			System::String^ name, FbxDataType^ dataType,System::String^  label,bool checkForDuplicate,bool %wasFound)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			STRINGTO_CONSTCHAR_ANSI(l,label);

			bool b;
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(*compoundProperty->_Ref(),n,*dataType->_Ref(),
				l,checkForDuplicate,&b));
			wasFound = b;			
			FREECHARPOINTER(n);
			FREECHARPOINTER(l);
			return p;

		}
		FbxProperty^ FbxProperty::Create(FbxProperty^ compoundProperty,
			System::String^ name, FbxDataType^ dataType,System::String^  label)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);			
			STRINGTO_CONSTCHAR_ANSI(l,label);			
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(*compoundProperty->_Ref(),n,*dataType->_Ref(),
				l));			
			FREECHARPOINTER(n);
			FREECHARPOINTER(l);
			return p;

		}
		FbxProperty^ FbxProperty::Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label,bool checkForDuplicate,bool %wasFound)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(l,label);

			bool b;
			KFbxProperty kp = KFbxProperty::Create(obj->_Ref(),n,*dataType->_Ref(),	l,checkForDuplicate,&b);
			FbxProperty^ p = gcnew FbxProperty(kp);
			wasFound = b;			
			FREECHARPOINTER(n);
			FREECHARPOINTER(l);
			return p;
		}
		FbxProperty^ FbxProperty::Create(FbxObjectManaged^ obj,System::String^ name, FbxDataType^ dataType, System::String^ label)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			STRINGTO_CONSTCHAR_ANSI(l,label);			
			KFbxProperty kp = KFbxProperty::Create(obj->_Ref(),n,*dataType->_Ref(),	l);
			FbxProperty^ p = gcnew FbxProperty(kp);			
			FREECHARPOINTER(n);
			FREECHARPOINTER(l);
			return p;
		}
		FbxProperty^ FbxProperty::Create(FbxObjectManaged^ obj, FbxProperty^ fromProperty, bool checkForDuplicate)
		{
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(obj->_Ref(),*fromProperty->_Ref(),checkForDuplicate));						 			
			return p;
		}
		FbxProperty^ FbxProperty::Create(FbxObjectManaged^ obj, FbxProperty^ fromProperty)
		{
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(obj->_Ref(),*fromProperty->_Ref()));
			return p;
		}
		FbxProperty^ FbxProperty::Create(FbxProperty^ compoundProperty,
			FbxProperty^ fromProperty, bool checkForDuplicate)
		{
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(*compoundProperty->_Ref(),*fromProperty->_Ref(),checkForDuplicate));			
			return p;
		}

		FbxProperty^ FbxProperty::Create(FbxProperty^ compoundProperty,
			FbxProperty^ fromProperty)
		{
			FbxProperty^ p = gcnew FbxProperty(KFbxProperty::Create(*compoundProperty->_Ref(),*fromProperty->_Ref()));			
			return p;
		}

		void FbxProperty::Destroy(bool recursive, bool dependents)
		{
			_Ref()->Destroy(recursive,dependents);
			_Free = false;
		}
		void FbxProperty::Destroy()
		{
			_Ref()->Destroy();
			_Free = false;
		}		
		FbxProperty::FbxProperty(FbxProperty^ p)
		{
			_SetPointer(new KFbxProperty(*p->_Ref()),true);			
		}
		FbxProperty::FbxProperty(FbxPropertyHandle^ p)
		{
			_SetPointer(new KFbxProperty(*p->_Ref()),true);			
		}

		FbxDataType^ FbxProperty::PropertyDataType::get()
		{			
			return gcnew FbxDataType(_Ref()->GetPropertyDataType());
		}
		FbxStringManaged^ FbxProperty::Name::get()
		{
			return gcnew FbxStringManaged(_Ref()->GetName());
		}		         
		FbxStringManaged^ FbxProperty::HierarchicalName::get()
		{
			return gcnew FbxStringManaged(_Ref()->GetHierarchicalName());
		}
		FbxStringManaged^ FbxProperty::GetLabel(bool returnNameIfEmpty)
		{
			return gcnew FbxStringManaged(_Ref()->GetLabel(returnNameIfEmpty));
		}
		void FbxProperty::SetLabel(FbxStringManaged^ label)
		{
			_Ref()->SetLabel(*label->_Ref());
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxProperty,KFbxObject,GetFbxObject(),FbxObjectManaged,Object);		

		int FbxProperty::UserTag::get()
		{
			return _Ref()->GetUserTag();
		}
		void FbxProperty::UserTag::set(int value)
		{
			_Ref()->SetUserTag(value);
		}			
		void FbxProperty::ModifyFlag(FbxPropertyFlags::FbxPropertyFlagsType flag, bool value)
		{
			_Ref()->ModifyFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag , value);
		}
		bool FbxProperty::GetFlag(FbxPropertyFlags::FbxPropertyFlagsType flag)					 
		{
			return _Ref()->GetFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
		}
		FbxInheritType FbxProperty::GetFlagInheritType( FbxPropertyFlags::FbxPropertyFlagsType flag )
		{
			return (FbxInheritType)_Ref()->GetFlagInheritType((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
		}
		bool FbxProperty::SetFlagInheritType(Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType flag, FbxInheritType type )
		{
			return _Ref()->SetFlagInheritType(
				(fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag,
				(KFbxInheritType)type);
		}
		bool FbxProperty::ModifiedFlag( FbxPropertyFlags::FbxPropertyFlagsType flag )
		{
			return _Ref()->ModifiedFlag((fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)flag);
		}			
		void FbxProperty::CopyFrom(FbxProperty^ p)
		{
			*_Ref() = *p->_Ref();
		}

		bool FbxProperty::CompareValue(FbxProperty^ prop)					 
		{
			return _Ref()->CompareValue(*prop->_Ref());
		}
		bool FbxProperty::CopyValue(FbxProperty^ prop)
		{
			return _Ref()->CopyValue(*prop->_Ref());
		}
		bool FbxProperty::IsValid::get()
		{
			return _Ref()->IsValid();
		}

		FbxInheritType FbxProperty::ValueInheritType::get()
		{
			return (FbxInheritType)_Ref()->GetValueInheritType(); 
		} 			
		bool FbxProperty::SetValueInheritType( FbxInheritType type )
		{
			return _Ref()->SetValueInheritType((KFbxInheritType)type);
		}
		bool FbxProperty::Modified::get()
		{
			return _Ref()->Modified();
		}			
		double FbxProperty::MinLimit::get()
		{
			return _Ref()->GetMinLimit();
		}
		void FbxProperty::MinLimit::set(double value)
		{
			_Ref()->SetMinLimit(value);
		}
		bool  FbxProperty::HasMinLimit::get()
		{
			return _Ref()->HasMinLimit();
		}			
		bool  FbxProperty::HasMaxLimit::get()
		{
			return _Ref()->HasMaxLimit();
		}			
		double FbxProperty::MaxLimit::get()
		{
			return _Ref()->GetMaxLimit();
		}
		void FbxProperty::MaxLimit::set(double value)
		{
			_Ref()->SetMaxLimit(value);
		}			
		void FbxProperty::SetLimits(double min, double max)
		{
			_Ref()->SetLimits(min,max);
		}
		int FbxProperty::AddEnumValue(System::String^ value)
		{			
			STRINGTO_CONSTCHAR_ANSI(v,value);
			int r = _Ref()->AddEnumValue(v);
			FREECHARPOINTER(v);
			return r;
		}
		void FbxProperty::InsertEnumValue(int index, System::String^ value)					 
		{			
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->InsertEnumValue(index,v);
			FREECHARPOINTER(v);
		}
		int FbxProperty::EnumCount::get()
		{
			return _Ref()->GetEnumCount();
		}			
		void FbxProperty::SetEnumValue(int index,System::String^ value)
		{			
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetEnumValue(index,v);
			FREECHARPOINTER(v);
		}
		void FbxProperty::RemoveEnumValue(int index)
		{
			_Ref()->RemoveEnumValue(index);
		}
		System::String^ FbxProperty::GetEnumValue(int index)
		{
			return gcnew System::String(_Ref()->GetEnumValue(index));
		}
		bool FbxProperty::IsRoot::get()
		{
			return _Ref()->IsRoot();
		}			
		FbxProperty^ FbxProperty::GetParent()					 
		{
			return gcnew FbxProperty(_Ref()->GetParent());
		}
		bool FbxProperty::SetParent(FbxProperty^ other )
		{
			return _Ref()->SetParent(*other->_Ref());
		}
		FbxProperty^ FbxProperty::GetChild()
		{
			return gcnew FbxProperty(_Ref()->GetChild());
		}
		FbxProperty^ FbxProperty::GetSibling()
		{
			return gcnew FbxProperty (_Ref()->GetSibling());
		}
		FbxProperty^ FbxProperty::GetFirstDescendent()
		{
			return gcnew FbxProperty (_Ref()->GetFirstDescendent());
		}
		FbxProperty^ FbxProperty::GetNextDescendent(FbxProperty^ p)
		{
			return gcnew FbxProperty(_Ref()->GetNextDescendent(*p->_Ref()));
		}
		FbxProperty^ FbxProperty::Find (System::String^ name,bool caseSensitive)
		{						 			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxProperty^ p = gcnew FbxProperty(_Ref()->Find(n,caseSensitive));
			FREECHARPOINTER(n);
			return p;
		}
		FbxProperty^ FbxProperty::Find (System::String^ name,FbxDataType^ dataType, bool caseSensitive)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxProperty^ p = gcnew FbxProperty(_Ref()->Find(n,*dataType->_Ref(),caseSensitive));
			FREECHARPOINTER(n);
			return p;
		}
		FbxProperty^ FbxProperty::FindHierarchical (System::String^ name,bool caseSensitive)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxProperty^ p = gcnew FbxProperty(_Ref()->FindHierarchical(n,caseSensitive));
			FREECHARPOINTER(n);
			return p;
		}
		void FbxProperty::BeginCreateOrFindProperty()
		{
			_Ref()->BeginCreateOrFindProperty();
		}
		void FbxProperty::EndCreateOrFindProperty()
		{
			_Ref()->EndCreateOrFindProperty();
		}		

		FbxProperty::FbxPropertyNameCache::FbxPropertyNameCache(FbxProperty^ prop)			            
		{
			_SetPointer(new KFbxProperty::KFbxPropertyNameCache(*prop->_Ref()),true);			
		}
		void FbxProperty::FbxPropertyNameCache::CollectManagedMemory()
		{
		}


		bool FbxProperty::SetArraySize( int size, bool variableArray )
		{
			return _Ref()->SetArraySize(size,variableArray );
		}
		int  FbxProperty::GetArraySize()
		{
			return _Ref()->GetArraySize();
		}
		FbxProperty^ FbxProperty::GetArrayItem(int index)
		{
			KFbxProperty p = _Ref()->GetArrayItem(index);
			if(p.IsValid())
				return gcnew FbxProperty(p);
			return nullptr;
		}
		FbxProperty^ FbxProperty::default::get(int index)
		{
			return GetArrayItem(index);
		}


		FbxCurveNode^ FbxProperty::CreateKFCurveNode(String^ takeName)
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
		FbxCurveNode^ FbxProperty::GetKFCurveNode(bool createAsNeeded, String^ takeName)
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

		FbxCurve^ FbxProperty::GetKFCurve(String^ channel)
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
		FbxCurve^ FbxProperty::GetKFCurve(String^ channel, String^ takeName)
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

		bool FbxProperty::ConnectSrcObject (FbxObjectManaged^ obj,FbxConnectionType type)
		{
			return _Ref()->ConnectSrcObject(obj->_Ref(),(kFbxConnectionType)type);
		}
		bool FbxProperty::IsConnectedSrcObject (FbxObjectManaged^ obj)
		{
			return _Ref()->IsConnectedSrcObject(obj->_Ref());
		}
		bool FbxProperty::DisconnectSrcObject(FbxObjectManaged^ obj)
		{
			return _Ref()->DisconnectSrcObject(obj->_Ref());
		}

		bool FbxProperty::DisconnectAllSrcObject()
		{
			return _Ref()->DisconnectAllSrcObject();
		}
		bool FbxProperty::DisconnectAllSrcObject(FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllSrcObject(*criteria->_Ref());
		}
		bool FbxProperty::DisconnectAllSrcObject(FbxClassId^ classId)
		{
			return _Ref()->DisconnectAllSrcObject(*classId->_Ref());
		}
		bool FbxProperty::DisconnectAllSrcObject(FbxClassId^ classId, FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllSrcObject(*classId->_Ref(),*criteria->_Ref());
		}

		int FbxProperty::SrcObjectCount::get()
		{
			return _Ref()->GetSrcObjectCount();
		}

		int FbxProperty::GetSrcObjectCount(FbxCriteria^ criteria)
		{
			return _Ref()->GetSrcObjectCount(*criteria->_Ref());
		}
		int FbxProperty::GetSrcObjectCount(FbxClassId^ classId)					 
		{
			return _Ref()->GetSrcObjectCount(*classId->_Ref());
		}
		int FbxProperty::GetSrcObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->GetSrcObjectCount(*classId->_Ref(),*criteria->_Ref());
		}

		FbxObjectManaged^ FbxProperty::GetSrcObject(int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::GetSrcObject(FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*criteria->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;			
		}
		FbxObjectManaged^ FbxProperty::GetSrcObject(FbxClassId^ classId,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*classId->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;			
		}
		FbxObjectManaged^ FbxProperty::GetSrcObject(FbxClassId^ classId, FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetSrcObject(*classId->_Ref(),*criteria->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;						
		}

		FbxObjectManaged^ FbxProperty::FindSrcObject(System::String^ name,int startIndex)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindSrcObject(n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindSrcObject(FbxCriteria^ criteria,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindSrcObject(*criteria->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindSrcObject(FbxClassId^ classId,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindSrcObject(*classId->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindSrcObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindSrcObject(*classId->_Ref(),*criteria->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		bool FbxProperty::ConnectDstObject(FbxObjectManaged^ obj,FbxConnectionType type)
		{
			return _Ref()->ConnectDstObject(obj->_Ref(), (kFbxConnectionType)type);
		}
		bool FbxProperty::IsConnectedDstObject(FbxObjectManaged^ obj)
		{
			return _Ref()->IsConnectedDstObject(obj->_Ref());
		}
		bool FbxProperty::DisconnectDstObject(FbxObjectManaged^ obj)
		{
			return _Ref()->DisconnectDstObject(obj->_Ref());
		}

		bool FbxProperty::DisconnectAllDstObject()
		{
			return _Ref()->DisconnectAllDstObject();
		}
		bool FbxProperty::DisconnectAllDstObject(FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllDstObject(*criteria->_Ref());
		}
		bool FbxProperty::DisconnectAllDstObject(FbxClassId^ classId)
		{
			return _Ref()->DisconnectAllDstObject(*classId->_Ref());
		}
		bool FbxProperty::DisconnectAllDstObject(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->DisconnectAllDstObject(*classId->_Ref(),*criteria->_Ref());
		}

		int FbxProperty::DstObjectCount::get()
		{
			return _Ref()->GetDstObjectCount();
		}

		int FbxProperty::GetDstObjectCount(FbxCriteria^ criteria)
		{
			return _Ref()->GetDstObjectCount(*criteria->_Ref());
		}
		int FbxProperty::GetDstObjectCount(FbxClassId^ classId)					 
		{
			return _Ref()->GetDstObjectCount(*classId->_Ref());
		}
		int FbxProperty::GetDstObjectCount(FbxClassId^ classId,FbxCriteria^ criteria)
		{
			return _Ref()->GetDstObjectCount(*classId->_Ref(),*criteria->_Ref());
		}			

		FbxObjectManaged^ FbxProperty::GetDstObject(int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::GetDstObject(FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*criteria->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;			
		}
		FbxObjectManaged^ FbxProperty::GetDstObject(FbxClassId^ classId,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*classId->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;						
		}
		FbxObjectManaged^ FbxProperty::GetDstObject(FbxClassId^ classId, FbxCriteria^ criteria,int index)
		{
			KFbxObject* obj = _Ref()->GetDstObject(*classId->_Ref(),*criteria->_Ref(), index);
			if(obj)
				return FbxCreator::CreateFbxObject(obj);
			return nullptr;			
		}

		FbxObjectManaged^ FbxProperty::FindDstObject(System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindDstObject(n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindDstObject(FbxCriteria^ criteria,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindDstObject(*criteria->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindDstObject(FbxClassId^ classId,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindDstObject(*classId->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		FbxObjectManaged^ FbxProperty::FindDstObject(FbxClassId^ classId,FbxCriteria^ criteria,System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxObject* o = _Ref()->FindDstObject(*classId->_Ref(),*criteria->_Ref(),n,startIndex);
			FREECHARPOINTER(n);
			if(o)
				return FbxCreator::CreateFbxObject(o);
			return nullptr;
		}
		bool FbxProperty::ConnectSrcProperty(FbxProperty^ p)
		{
			return _Ref()->ConnectSrcProperty(*p->_Ref());
		}
		bool FbxProperty::IsConnectedSrcProperty (FbxProperty^ p)
		{
			return _Ref()->IsConnectedSrcProperty(*p->_Ref());
		}
		bool FbxProperty::DisconnectSrcProperty (FbxProperty^p)
		{
			return _Ref()->DisconnectSrcProperty(*p->_Ref());
		}
		int FbxProperty::SrcPropertyCount::get()
		{
			return _Ref()->GetSrcPropertyCount();
		}

		FbxProperty^ FbxProperty::GetSrcProperty (int index)
		{
			return gcnew FbxProperty(_Ref()->GetSrcProperty(index));
		}
		FbxProperty^ FbxProperty::FindSrcProperty(System::String^ name,int startIndex)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty o = _Ref()->FindSrcProperty(n,startIndex);
			FREECHARPOINTER(n);
			if(o.IsValid())
				return gcnew FbxProperty(o);
			return nullptr;
		}

		bool FbxProperty::ConnectDstProperty(FbxProperty^ p)
		{
			return _Ref()->ConnectDstProperty(*p->_Ref());
		}
		bool FbxProperty::IsConnectedDstProperty  (FbxProperty^ p)
		{
			return _Ref()->IsConnectedDstProperty(*p->_Ref());
		}
		bool FbxProperty::DisconnectDstProperty(FbxProperty^ p)
		{
			return _Ref()->DisconnectDstProperty(*p->_Ref());
		}
		int FbxProperty::DstPropertyCount::get()
		{
			return _Ref()->GetDstPropertyCount();
		}


		FbxProperty^ FbxProperty::GetDstProperty (int index)
		{
			return gcnew FbxProperty(_Ref()->GetDstProperty(index));
		}
		FbxProperty^ FbxProperty::FindDstProperty(System::String^ name,int startIndex)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxProperty o = _Ref()->FindDstProperty(n,startIndex);
			FREECHARPOINTER(n);
			if(o.IsValid())
				return gcnew FbxProperty(o);
			return nullptr;
		}

		void FbxProperty::ClearConnectCache()
		{
			_Ref()->ClearConnectCache();
		}

		FbxDataType^ FbxProperty::UserPropertyTypeToDataType(FbxProperty::UserPropertyType type)
		{
			KFbxDataType dt = EUserPropertyTypeToDataType((KFbxProperty::EUserPropertyType)type);
			return gcnew FbxDataType(dt);
		}
		FbxProperty::UserPropertyType FbxProperty::DataTypeToUserPropertyType(FbxDataType^ dataType)
		{
			return (FbxProperty::UserPropertyType)DataTypeToEUserPropertyType(*dataType->_Ref());
		}

		bool FbxProperty::GetValueAsBool()
		{
			return KFbxGet<bool>(*_Ref());
		}
		int FbxProperty::GetValueAsInt()
		{
			return KFbxGet<int>(*_Ref());
		}
		double FbxProperty::GetValueAsDouble()
		{
			return KFbxGet<double>(*_Ref());
		}
		FbxColor^ FbxProperty::GetValueAsColor()
		{
			KFbxColor c = KFbxGet<KFbxColor>(*_Ref());
			return gcnew FbxColor(c);
		}
		FbxDouble2^ FbxProperty::GetValueAsDouble2()
		{
			fbxDouble2 d2 = KFbxGet<fbxDouble2>(*_Ref());
			return gcnew FbxDouble2(d2);
		}
		FbxDouble3^ FbxProperty::GetValueAsDouble3()
		{
			fbxDouble3 d3 = KFbxGet<fbxDouble3>(*_Ref());
			return gcnew FbxDouble3(d3);
		}
		float FbxProperty::GetValueAsFloat()
		{
			return KFbxGet<float>(*_Ref());
		}
		String^ FbxProperty::GetValueAsString()
		{
			FbxString kstr = KFbxGet<FbxString>(*_Ref());
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		void FbxProperty::Set(bool value)
		{
			_Ref()->Set<bool>(value);
		}
		void FbxProperty::Set(int value)
		{
			_Ref()->Set<int>(value);
		}
		void FbxProperty::Set(double value)
		{
			_Ref()->Set<double>(value);
		}
		void FbxProperty::Set(FbxColor^ value)
		{
			_Ref()->Set<KFbxColor>(*value->_Ref());
		}
		void FbxProperty::Set(FbxDouble2^ value)
		{
			_Ref()->Set<fbxDouble2>(*value->_Ref());
		}
		void FbxProperty::Set(FbxDouble3^ value)
		{
			_Ref()->Set<fbxDouble3>(*value->_Ref());
		}
		void FbxProperty::Set(FbxVector4^ value)
		{
			_Ref()->Set<KFbxVector4>(*value->_Ref());
		}
		void FbxProperty::Set(FbxVector2^ value)
		{
			_Ref()->Set<KFbxVector2>(*value->_Ref());
		}
		void FbxProperty::Set(float value)
		{
			_Ref()->Set<float>(value);
		}
		void FbxProperty::Set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			FbxString kstr(v); 
			FREECHARPOINTER(v);
			_Ref()->Set<FbxString>(kstr);
		}






		FbxConnectEvent::FbxConnectEvent(FbxConnectEventType type,
			FbxConnectEventDirection dir, FbxProperty^  src, FbxProperty^ dst)			        
		{
			_SetPointer(new KFbxConnectEvent((eFbxConnectEventType)type,(eFbxConnectEventDirection)dir,
				src->_Ref(),dst->_Ref()),true);			
			this->_Source = src;
			this->_Destination = dst;
		}
		void FbxConnectEvent::CollectManagedMemory()
		{
			this->_Source = nullptr;
			this->_Destination = nullptr;			
		}		
		FbxConnectEventType FbxConnectEvent::Type::get()
		{
			return (FbxConnectEventType)_Ref()->GetType();
		}

		FbxConnectEventDirection FbxConnectEvent::Direction::get()
		{
			return (FbxConnectEventDirection)_Ref()->GetDirection();
		}

		FbxProperty^ FbxConnectEvent::Source::get()
		{
			return _Source;
		}

		FbxProperty^ FbxConnectEvent::Destination::get()
		{
			return _Destination;
		}
	}
}