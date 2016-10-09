#pragma once
#include "stdafx.h"
#include "FbxConstraintScale.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxVector4.h"

#define PROPERTY_GETSET_DEFINITION(PropType,PropName)\
	VALUE_PROPERTY_GET_DEFINATION(FbxConstraintScale,Get##PropName(),PropType,PropName);\
	void FbxConstraintScale::PropName::set(PropType value){ _Ref()->Set##PropName(value);}

namespace Skill
{
	namespace FbxSDK
	{		
		void FbxConstraintScale::CollectManagedMemory()
		{
			_ConstrainedObject = nullptr;
			_Offset = nullptr;
			FbxConstraint::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxConstraintScale,KFbxConstraintScale);
		PROPERTY_GETSET_DEFINITION(bool,Lock);
		PROPERTY_GETSET_DEFINITION(bool,Active);
		PROPERTY_GETSET_DEFINITION(bool,AffectX);
		PROPERTY_GETSET_DEFINITION(bool,AffectY);
		PROPERTY_GETSET_DEFINITION(bool,AffectZ);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintScale,GetOffset(),FbxVector4,Offset)
		void FbxConstraintScale::Offset::set(FbxVector4^ value)
		{
			_Ref()->SetOffset(*value->_Ref());
		}

		void FbxConstraintScale::SetWeight(double weight)
		{
			_Ref()->SetWeight(weight);
		}
		double FbxConstraintScale::GetSourceWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetSourceWeight(obj->_Ref());
		}
		void FbxConstraintScale::AddConstraintSource(FbxObjectManaged^ obj, double weight)
		{
			_Ref()->AddConstraintSource(obj->_Ref(),weight);
		}
		int FbxConstraintScale::ConstraintSourceCount::get()
		{
			return _Ref()->GetConstraintSourceCount();
		}

		FbxObjectManaged^ FbxConstraintScale::GetConstraintSource(int index)
		{
			KFbxObject* obj = _Ref()->GetConstraintSource(index);
			return FbxCreator::CreateFbxObject(obj);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintScale,KFbxObject,GetConstrainedObject(),FbxObjectManaged,ConstrainedObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintScale,SetConstrainedObject,FbxObjectManaged,ConstrainedObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		
		CLONE_DEFINITION(FbxConstraintScale,KFbxConstraintScale);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}