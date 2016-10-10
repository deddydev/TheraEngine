#pragma once
#include "stdafx.h"
#include "FbxProperty.h"

#define FBXTYPEDPEROPERTY_DECLARE(Class,NativeClass,SetClass)public ref class Fbx##Class##TypedProperty : FbxProperty {\
		internal: Fbx##Class##TypedProperty(KFbxTypedProperty<NativeClass>* _property);\
		REF_DECLARE(FbxProperty,KFbxTypedProperty<NativeClass>);\
		public:	Fbx##Class##TypedProperty();\
		void CopyFrom(Fbx##Class##TypedProperty^ other);\
		bool Set(SetClass value, bool checkValueEquality );\
		bool Set(SetClass value );\
		SetClass Get();\
		SetClass Get(FbxEvaluationInfo^ info );};
		


#define FBXTYPEDPEROPERTY_DEFINITION(Class,NativeClass,SetClass,SetNativeClass)\
		Fbx##Class##TypedProperty::Fbx##Class##TypedProperty(KFbxTypedProperty<NativeClass>* a):FbxProperty(a){_Free = false;}\
		Fbx##Class##TypedProperty::Fbx##Class##TypedProperty():FbxProperty(new KFbxTypedProperty<NativeClass>()){_Free = true;}\
		void Fbx##Class##TypedProperty::CopyFrom(Fbx##Class##TypedProperty^ other){*_Ref() = *other->_Ref();}\
		bool Fbx##Class##TypedProperty::Set(SetClass value, bool checkValueEquality ){return _Ref()->Set(SetNativeClass,checkValueEquality);}\
		bool Fbx##Class##TypedProperty::Set(SetClass value){return _Ref()->Set(SetNativeClass);}\

#define FBXTYPEDPEROPERTY_GET_DEFINITION_Class(Class,NativeClass,SetClass,NewClass)\
		SetClass Fbx##Class##TypedProperty::Get(){return gcnew NewClass(&(_Ref()->Get()));}\
		SetClass Fbx##Class##TypedProperty::Get(FbxEvaluationInfo^ info){return gcnew NewClass(&(_Ref()->Get(info->_Ref())));}\

#define FBXTYPEDPEROPERTY_GET_DEFINITION_Value(Class,NativeClass,SetClass)\
		SetClass Fbx##Class##TypedProperty::Get(){return _Ref()->Get();}\
		SetClass Fbx##Class##TypedProperty::Get(FbxEvaluationInfo^ info){return _Ref()->Get(info->_Ref());}\


{
	namespace FbxSDK
	{		
		ref class FbxStringManaged;
		ref class FbxEvaluationInfo;
		ref class FbxObjectManaged;
		ref class FbxDataType;
		ref class FbxDouble3;
		ref class FbxDouble2;

		FBXTYPEDPEROPERTY_DECLARE(String,FbxString,FbxStringManaged^);
		FBXTYPEDPEROPERTY_DECLARE(Double3,fbxDouble3,FbxDouble3^);
		FBXTYPEDPEROPERTY_DECLARE(Double2,fbxDouble2,FbxDouble2^);
		

		public ref class FbxDouble1TypedProperty : FbxPropertyManaged
		{
			REF_DECLARE(FbxPropertyManaged,KFbxTypedProperty<fbxDouble1>);
		internal:
			FbxDouble1TypedProperty(KFbxTypedProperty<fbxDouble1>* instance)
				:FbxPropertyManaged(instance)
			{
				_Free = false;
			}
		public:
			FbxDouble1TypedProperty();
			void CopyFrom(FbxDouble1TypedProperty^ other);
			bool Set(double value, bool checkValueEquality );
			bool Set(double value );
			double Get();
			double Get(FbxEvaluationInfo^ info );
		};


		public ref class FbxInteger1TypedProperty : FbxPropertyManaged
		{
			REF_DECLARE(FbxPropertyManaged,KFbxTypedProperty<fbxInteger1>);
		internal:
			FbxInteger1TypedProperty(KFbxTypedProperty<fbxInteger1>* instance)
				:FbxPropertyManaged(instance)
			{
				_Free = false;
			}
		public:
			FbxInteger1TypedProperty();
			void CopyFrom(FbxInteger1TypedProperty^ other);
			bool Set(int value, bool checkValueEquality );
			bool Set(int value );
			int Get();
			int Get(FbxEvaluationInfo^ info );
		};
	}
}