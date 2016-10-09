#pragma once
#include "stdafx.h"
#include "FbxConstraintRotation.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxVector4.h"

#define PROPERTY_GETSET_DEFINITION(PropType,PropName)\
	VALUE_PROPERTY_GET_DEFINATION(FbxConstraintRotation,Get##PropName(),PropType,PropName);\
	void FbxConstraintRotation::PropName::set(PropType value){ _Ref()->Set##PropName(value);}

namespace Skill
{
	namespace FbxSDK
	{	
		void FbxConstraintRotation::CollectManagedMemory()
		{
			_ConstrainedObject = nullptr;
			_Offset = nullptr;
			FbxConstraint::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxConstraintRotation,KFbxConstraintRotation);

		PROPERTY_GETSET_DEFINITION(bool,Lock);
		PROPERTY_GETSET_DEFINITION(bool,Active);
		PROPERTY_GETSET_DEFINITION(bool,AffectX);
		PROPERTY_GETSET_DEFINITION(bool,AffectY);
		PROPERTY_GETSET_DEFINITION(bool,AffectZ);

		void FbxConstraintRotation::SetWeight(double weight)
		{
			_Ref()->SetWeight(weight);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintRotation,GetOffset(),FbxVector4,Offset);
		void FbxConstraintRotation::Offset::set(FbxVector4^ value)
		{
			_Ref()->SetOffset(*value->_Ref());
		}

		double FbxConstraintRotation::GetSourceWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetSourceWeight(obj->_Ref());
		}
		void FbxConstraintRotation::AddConstraintSource(FbxObjectManaged^ obj, double weight)
		{
			_Ref()->AddConstraintSource(obj->_Ref(), weight);
		}
		int FbxConstraintRotation::ConstraintSourceCount::get()
		{
			return _Ref()->GetConstraintSourceCount();
		}

		FbxObjectManaged^ FbxConstraintRotation::GetConstraintSource(int index)
		{
			KFbxObject* obj = _Ref()->GetConstraintSource(index);
			return FbxCreator::CreateFbxObject(obj);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintRotation,KFbxObject,GetConstrainedObject(),FbxObjectManaged,ConstrainedObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintRotation,SetConstrainedObject,FbxObjectManaged,ConstrainedObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		
		CLONE_DEFINITION(FbxConstraintRotation,KFbxConstraintRotation);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}