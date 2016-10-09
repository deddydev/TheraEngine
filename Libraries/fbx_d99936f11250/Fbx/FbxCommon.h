#pragma once
#include "stdafx.h"

using namespace System;
using namespace System::Runtime::InteropServices;

#define STRINGTO_CONSTCHAR_ANSI(ptrName,value) IntPtr _##ptrName## = Marshal::StringToHGlobalAnsi(value);\
const char* ptrName = static_cast<const char*>(_##ptrName##.ToPointer());

#define STRINGTOCHAR_ANSI(ptrName,value) IntPtr _##ptrName## = Marshal::StringToHGlobalAnsi(value);\
char* ptrName = static_cast<char*>(_##ptrName##.ToPointer());

#define FREECHARPOINTER(ptrName) System::Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr((void*)ptrName));

#define CONVERT_FbxString_TO_STRING(kstr,str) String^ str = gcnew String(kstr.Buffer());


#define FBXPLUG_DECLARE(Class)	private: static FbxClassId^ _##Class##classID;\
		public: virtual FbxClassId^ GetClassId()override;\
		static Class^ Create(FbxSdkManager^ manager , String^ name);\
		static property FbxClassId^ ClassId {FbxClassId^ get(); }\

#define FBXPLUG_DEFINITION(Class,NativeClass)FbxClassId^ Class::GetClassId(){return gcnew FbxClassId(_Ref()->GetClassId());}\
		Class^ Class::Create(FbxSdkManager^ manager , String^ name){\
			STRINGTO_CONSTCHAR_ANSI(n,name);\
			NativeClass* _##NativeClass = NativeClass::Create(manager->_Ref(),n);\
			Class^ _##Class  = nullptr;\
			if(_##NativeClass) _##Class  = gcnew Class(_##NativeClass);	FREECHARPOINTER(n);((FbxPlug^)_##Class)->sdkManager = manager; return _##Class;}\
			FbxClassId^ Class::ClassId::get(){if(_##Class##classID)_##Class##classID->_FbxClassId = &NativeClass::ClassId;\
		else _##Class##classID = gcnew FbxClassId(&NativeClass::ClassId);\
		return _##Class##classID;}

#define FBXOBJECT_DECLARE(Class) FBXPLUG_DECLARE(Class);\
				public:\
				static Class^ Create(FbxObjectManaged^ container , String^ name);\
				static Class^ CreateForClone(FbxSdkManagerManaged^ manager , String^ name,Class^ from);\
				Class^ TypedClone(FbxObjectManaged^ container , Skill::FbxSDK::FbxObjectManaged::CloneType cloneType);\
				Class^ TypedClone(FbxObjectManaged^ container);\
				virtual bool Compare(FbxObjectManaged^ otherObject,FbxCompare fbxCompare)override;\

#define FBXOBJECT_DEFINITION(Class,NativeClass) FBXPLUG_DEFINITION(Class,NativeClass);\
		Class^ Class::Create(FbxObject^ container , String^ name){STRINGTO_CONSTCHAR_ANSI(n,name);NativeClass* _##NativeClass = NativeClass::Create(container->_Ref(),n);\
		Class^ _##Class  = nullptr;\
		if(_##NativeClass){_##Class  = gcnew Class(_##NativeClass);}\
		FREECHARPOINTER(n);	return _##Class;} Class^ Class::CreateForClone(FbxSdkManager^ manager , String^ name,Class^ from){\
		STRINGTO_CONSTCHAR_ANSI(n,name); NativeClass* _##NativeClass = NativeClass::CreateForClone(manager->_Ref(),n,from->_Ref());\
		Class^ _##Class  = nullptr;\
		if(_##NativeClass)	{_##Class  = gcnew Class(_##NativeClass);}\
		FREECHARPOINTER(n);	return _##Class;}\
		Class^ Class::TypedClone(FbxObject^ container ,Skill::FbxSDK::FbxObject::CloneType cloneType){NativeClass* _##NativeClass;\
		if(container)_##NativeClass = _Ref()->TypedClone(container->_Ref(),(KFbxObject::ECloneType)cloneType);\
		else _##NativeClass = _Ref()->TypedClone(NULL,(KFbxObject::ECloneType)cloneType);\
		Class^ _##Class  = nullptr;	if(_##NativeClass){	_##Class = gcnew Class(_##NativeClass);}\
		return _##Class;}\
		Class^ Class::TypedClone(FbxObject^ container){	return TypedClone(container ,CloneType::Deep);}\
		bool Class::Compare(FbxObject^ otherObject,FbxCompare fbxCompare){return _Ref()->Compare(otherObject->_Ref(),(eFbxCompare)fbxCompare);}\


#define GET_NATIVEPOINTER_DECLARE(Class,NativeClass) public: virtual property IntPtr NativePointer { IntPtr get() override{return IntPtr(_##Class);} void set(IntPtr value) override{_##Class = (NativeClass*)value.ToPointer();} }

#define ISVALID_DECLARE(Class) public: virtual property bool IsValid {bool get() override{if(_##Class) return true; else return false;}}

#define GET_NATIVEPOINTER_DECLARE_BASE(Class,NativeClass) public: virtual property IntPtr NativePointer { IntPtr get(){return IntPtr(_##Class);} void set(IntPtr value) {_##Class = (NativeClass*)value.ToPointer();} }

#define ISVALID_DECLARE_BASE(Class) public: virtual property bool IsValid {bool get(){if(_##Class) return true; else return false;}}

#define IS_SAME_AS_DECLARE_BASE() public: virtual bool IsSameAs(IFbxNativePointer^ other){return this->NativePointer.ToPointer() == other->NativePointer.ToPointer();}

#define IS_SAME_AS_DECLARE() public: virtual bool IsSameAs(IFbxNativePointer^ other)override {return this->NativePointer.ToPointer() == other->NativePointer.ToPointer();}

#define INATIVEPOINTER_DECLARE(Class,NativeClass)\
	public: GET_NATIVEPOINTER_DECLARE_BASE(Class,NativeClass)\
	ISVALID_DECLARE_BASE(Class);\
	IS_SAME_AS_DECLARE_BASE();

#define CLONE_DECLARE() public: virtual FbxObject^ Clone(FbxObject^ container, FbxObject::CloneType cloneType) override;

#define CLONE_DEFINITION(Class,NativeClass) FbxObject^ Class::Clone(FbxObject^ container, FbxObject::CloneType cloneType){\
			Class^ obj = gcnew Class((NativeClass*)_Ref()->Clone(container->_Ref(),(KFbxObject::ECloneType)cloneType));\
			((Class^)obj)->refTo = this;return obj;}

#define REF_DECLARE(Class,NativeClass)\
	internal: inline NativeClass* _Ref() { return (NativeClass*)_##Class; }

#define DESTRUCTOR_DECLARE(Class)\
	public: ~Class() { this->CollectManagedMemory(); this->!Class(); }\
	!Class() { if (_Free && _##Class) delete _##Class; _Free = false; _##Class = nullptr; _disposed = true; }

#define DESTRUCTOR_DECLARE_2(Class)\
	public: ~Class() { this->CollectManagedMemory(); this->!Class(); }\
	!Class() { _Free = false; _##Class = nullptr; _disposed = true; }

#define INTERNAL_CLASS_DECLARE(Class,NativeClass)\
	private: bool _disposed;\
	internal: bool _Free;\
	NativeClass* _##Class;\
	Class(NativeClass* instance){this->_##Class = instance;_Free = false;}\
	void _SetPointer(NativeClass* instance , bool free){_##Class = instance;_Free = free;}\
	public: property bool Disposed {bool get(){return _disposed;}}\
	protected: virtual void CollectManagedMemory();

#define BASIC_CLASS_DECLARE(Class,NativeClass)\
	INTERNAL_CLASS_DECLARE(Class,NativeClass);\
	REF_DECLARE(Class,NativeClass);\
	DESTRUCTOR_DECLARE(Class);

#define DEFAULT_CONSTRUCTOR(Class,NativeClass)\
	public: Class(){_##Class = new NativeClass();_Free = true;}\

//// value properties ////
#define VALUE_PROPERTY_GET_DECLARE(PropType,PropName)\
	property PropType PropName { PropType get();}

#define VALUE_PROPERTY_GETSET_DECLARE(PropType,PropName)\
	property PropType PropName { PropType get(); void set(PropType value);}

#define VALUE_PROPERTY_GET_DEFINATION(Class,NativePtr,PropType,PropName)\
	PropType Class::PropName::get() {return _Ref()->NativePtr;}

#define VALUE_PROPERTY_GETSET_DEFINATION(Class,NativePtr,PropType,PropName)\
	VALUE_PROPERTY_GET_DEFINATION(Class,NativePtr,PropType,PropName);\
	void Class::PropName::set(PropType value) {_Ref()->NativePtr = value;}

//// ref properties ////
#define REF_PROPERTY_GET_DECLARE(PropType,PropName)\
	property PropType^ PropName {PropType^ get();}\
	private: PropType^ _##PropName;public:
	
#define REF_PROPERTY_GETSET_DECLARE(PropType,PropName)\
	property PropType^ PropName { PropType^ get(); void set(PropType^ value);}\
	private: PropType^ _##PropName;public:

#define REF_PROPERTY_GET_DEFINATION_FROM_VALUE(Class,NativePtr,PropType,PropName)\
	PropType^ Class::PropName::get() {\
	if(_##PropName) _##PropName->_SetPointer(&_Ref()->NativePtr,false);\
	else _##PropName = gcnew PropType(&_Ref()->NativePtr);return _##PropName;}

#define REF_PROPERTY_SET_DEFINATION_FROM_VALUE(Class,NativePtr,PropType,PropName)\
	void Class::PropName::set(PropType^ value){_Ref()->NativePtr = *value->_Ref();}

#define REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(Class,NativePtr,PropType,PropName)\
	REF_PROPERTY_GET_DEFINATION_FROM_VALUE(Class,NativePtr,PropType,PropName);\
	REF_PROPERTY_SET_DEFINATION_FROM_VALUE(Class,NativePtr,PropType,PropName);

#define REF_PROPERTY_GET_DEFINATION_FROM_REF(Class,NativeClass,NativePtr,PropType,PropName)\
	PropType^ Class::PropName::get() {\
	NativeClass* _##NativeClass = _Ref()->NativePtr;\
	if(_##NativeClass){\
	if(_##PropName) _##PropName->_SetPointer(_##NativeClass,false);\
	else _##PropName = gcnew PropType(_##NativeClass);return _##PropName;}return nullptr;}


#define REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(Class,NativeClass,NativePtr,PropType,PropName)\
	PropType^ Class::PropName::get() {\
	NativeClass* _##NativeClass = _Ref()->NativePtr;\
	if(_##NativeClass){\
	if(_##PropName) _##PropName->_SetPointer(_##NativeClass,false);\
	else _##PropName = FbxCreator::Create##PropType(_##NativeClass);return _##PropName;}return nullptr;}

#define REF_PROPERTY_SET_DEFINATION_FROM_REF(Class,NativePtr,PropType,PropName)\
	void Class::PropName::set(PropType^ value){_Ref()->NativePtr = value->_Ref();}

#define REF_PROPERTY_SET_DEFINATION_BY_FUNC(Class,Func,PropType,PropName)\
	void Class::PropName::set(PropType^ value){if(value){_Ref()->Func(value->_Ref());_##PropName = value;}else {_Ref()->Func(NULL);_##PropName = nullptr;}}

#define REF_PROPERTY_GETSET_DEFINATION_FROM_REF(Class,NativePtr,PropType,PropName)\
	REF_PROPERTY_GET_DEFINATION_FROM_REF(Class,NativePtr,PropType,PropName);\
	REF_PROPERTY_SET_DEFINATION_FROM_REF(Class,NativePtr,PropType,PropName);
