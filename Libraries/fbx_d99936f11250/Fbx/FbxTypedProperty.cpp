#pragma once
#include "stdafx.h"
#include "FbxTypedProperty.h"
#include "FbxString.h"
#include "FbxEvaluationInfo.h"
#include "FbxObject.h"
#include "FbxDataType.h"
#include "FbxDouble3.h"
#include "FbxDouble2.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FBXTYPEDPEROPERTY_DEFINITION(String,FbxString,FbxStringManaged^,*value->_Ref());
		FBXTYPEDPEROPERTY_GET_DEFINITION_Class(String,FbxString,FbxStringManaged^,FbxStringManaged);

		FBXTYPEDPEROPERTY_DEFINITION(Double3,fbxDouble3,FbxDouble3^,*value->_Ref());
		FBXTYPEDPEROPERTY_GET_DEFINITION_Class(Double3,fbxDouble3,FbxDouble3^,FbxDouble3);

		FBXTYPEDPEROPERTY_DEFINITION(Double2,fbxDouble2,FbxDouble2^,*value->_Ref());
		FBXTYPEDPEROPERTY_GET_DEFINITION_Class(Double2,fbxDouble2,FbxDouble2^,FbxDouble2);




		FbxDouble1TypedProperty::FbxDouble1TypedProperty()
			: FbxProperty(new KFbxTypedProperty<fbxDouble1>())
		{
			_Free = true;
		}
		void FbxDouble1TypedProperty::CopyFrom(FbxDouble1TypedProperty^ other)
		{
			*_Ref() = *other->_Ref();
		}
		bool FbxDouble1TypedProperty::Set(double value, bool checkValueEquality )
		{
			return _Ref()->Set(value,checkValueEquality);
		}
		bool FbxDouble1TypedProperty::Set(double value )
		{
			return _Ref()->Set(value);
		}
		double FbxDouble1TypedProperty::Get()
		{
			return _Ref()->Get();
		}
		double FbxDouble1TypedProperty::Get(FbxEvaluationInfo^ info )
		{
			return _Ref()->Get(info->_Ref());
		}







		FbxInteger1TypedProperty::FbxInteger1TypedProperty()
			: FbxProperty(new KFbxTypedProperty<fbxInteger1>())
		{
			_Free = true;
		}
		void FbxInteger1TypedProperty::CopyFrom(FbxInteger1TypedProperty^ other)
		{
			*_Ref() = *other->_Ref();
		}
		bool FbxInteger1TypedProperty::Set(int value, bool checkValueEquality )
		{
			return _Ref()->Set(value,checkValueEquality);
		}
		bool FbxInteger1TypedProperty::Set(int value )
		{
			return _Ref()->Set(value);
		}
		int FbxInteger1TypedProperty::Get()
		{
			return _Ref()->Get();
		}
		int FbxInteger1TypedProperty::Get(FbxEvaluationInfo^ info )
		{
			return _Ref()->Get(info->_Ref());
		}
	}
}