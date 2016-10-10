#pragma once
#include "stdafx.h"
#include "FbxConstraintAim.h"
#include "FbxVector4.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"

#define PROPERTY_GETSET_DEFINITION(PropType,PropName)\
	PropType FbxConstraintAim::PropName::get(){return _Ref()->PropName.Get();}\
	void FbxConstraintAim::PropName::set(PropType value){ _Ref()->PropName.Set(value);}


{
	namespace FbxSDK
	{
		void FbxConstraintAim::CollectManagedMemory()
		{
			_ConstrainedObject = nullptr;
			_Offset = nullptr;
			_WorldUpObject = nullptr;
			_AimVector = nullptr;
			_UpVector = nullptr;
			_WorldUpVector = nullptr;
			FbxConstraint::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxConstraintAim,KFbxConstraintAim);

		PROPERTY_GETSET_DEFINITION(bool,Lock);
		PROPERTY_GETSET_DEFINITION(bool,Active);
		void FbxConstraintAim::SetWeight(double weight)
		{
			_Ref()->SetWeight(weight);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintAim,GetOffset(),FbxVector4,Offset)		
			void FbxConstraintAim::Offset::set(FbxVector4^ value)
		{
			_Ref()->SetOffset(*value->_Ref());
		}

		double FbxConstraintAim::GetSourceWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetSourceWeight(obj->_Ref());
		}
		void FbxConstraintAim::AddConstraintSource(FbxObjectManaged^ obj, double weight)		
		{
			_Ref()->AddConstraintSource(obj->_Ref(),weight);
		}
		int FbxConstraintAim::ConstraintSourceCount::get()
		{
			return _Ref()->GetConstraintSourceCount();
		}
		FbxObjectManaged^ FbxConstraintAim::GetConstraintSource(int index)
		{
			KFbxObject* obj = _Ref()->GetConstraintSource(index);
			return FbxCreator::CreateFbxObject(obj);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintAim,KFbxObject,GetConstrainedObject(),FbxObjectManaged,ConstrainedObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintAim,SetConstrainedObject,FbxObjectManaged,ConstrainedObject);

		FbxConstraintAim::AimConstraintWoldUpType FbxConstraintAim::WorldUpType::get()
		{
			return (FbxConstraintAim::AimConstraintWoldUpType)_Ref()->GetWorldUpType();
		}
		void FbxConstraintAim::WorldUpType::set(FbxConstraintAim::AimConstraintWoldUpType value)
		{
			return _Ref()->SetWorldUpType((KFbxConstraintAim::EAimConstraintWoldUpType)value);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintAim,KFbxObject,GetWorldUpObject(),FbxObjectManaged,WorldUpObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintAim,SetWorldUpObject,FbxObjectManaged,WorldUpObject);


		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintAim,GetWorldUpVector(),FbxVector4,WorldUpVector);
		void FbxConstraintAim::WorldUpVector::set(FbxVector4^ value)
		{
			_Ref()->SetWorldUpVector(*value->_Ref());
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintAim,GetUpVector(),FbxVector4,UpVector);
		void FbxConstraintAim::UpVector::set(FbxVector4^ value)
		{
			_Ref()->SetUpVector(*value->_Ref());
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintAim,GetAimVector(),FbxVector4,AimVector);
		void FbxConstraintAim::AimVector::set(FbxVector4^ value)
		{
			_Ref()->SetAimVector(*value->_Ref());
		}

		PROPERTY_GETSET_DEFINITION(bool,AffectX);							
		PROPERTY_GETSET_DEFINITION(bool,AffectY);
		PROPERTY_GETSET_DEFINITION(bool,AffectZ);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxConstraintAim,KFbxConstraintAim);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}