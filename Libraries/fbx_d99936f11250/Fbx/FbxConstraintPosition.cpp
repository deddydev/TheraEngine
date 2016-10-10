#pragma once
#include "stdafx.h"
#include "FbxConstraintPosition.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxVector4.h"



{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxConstraintPosition,KFbxConstraintPosition);

		void FbxConstraintPosition::CollectManagedMemory()
		{
			_ConstrainedObject = nullptr;
			_Offset = nullptr;
			FbxConstraint::CollectManagedMemory();
		}

		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintPosition,GetLock(),bool,Lock);
		void FbxConstraintPosition::Lock::set(bool value)
		{
			_Ref()->SetLock(value);
		}

		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintPosition,GetActive(),bool,Active);
		void FbxConstraintPosition::Active::set(bool value)
		{
			_Ref()->SetActive(value);
		}

		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintPosition,GetAffectX(),bool,AffectX);
		void FbxConstraintPosition::AffectX::set(bool value)
		{
			_Ref()->SetAffectX(value);
		}

		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintPosition,GetAffectY(),bool,AffectY);
		void FbxConstraintPosition::AffectY::set(bool value)
		{
			_Ref()->SetAffectY(value);
		}

		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintPosition,GetAffectZ(),bool,AffectZ);
		void FbxConstraintPosition::AffectZ::set(bool value)
		{
			_Ref()->SetAffectZ(value);			
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintPosition,GetOffset(),FbxVector4,Offset);
		void FbxConstraintPosition::Offset::set(FbxVector4^ value)
		{
			_Ref()->SetOffset(*value->_Ref());
		}


		void FbxConstraintPosition::SetWeight(double weight)
		{
			_Ref()->SetWeight(weight);
		}
		double FbxConstraintPosition::GetSourceWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetSourceWeight(obj->_Ref());
		}
		void FbxConstraintPosition::AddConstraintSource(FbxObjectManaged^ obj, double weight)
		{
			_Ref()->AddConstraintSource(obj->_Ref(),weight);
		}
		bool FbxConstraintPosition::RemoveConstraintSource(FbxObjectManaged^ obj)
		{
			return _Ref()->RemoveConstraintSource(obj->_Ref());
		}
		int FbxConstraintPosition::ConstraintSourceCount::get()
		{
			return _Ref()->GetConstraintSourceCount();
		}
		FbxObjectManaged^ FbxConstraintPosition::GetConstraintSource(int index)
		{
			KFbxObject* obj = _Ref()->GetConstraintSource(index);
			return FbxCreator::CreateFbxObject(obj);
		}		
		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintPosition,KFbxObject,GetConstrainedObject(),FbxObjectManaged,ConstrainedObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintPosition,SetConstrainedObject,FbxObjectManaged,ConstrainedObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			CLONE_DEFINITION(FbxConstraintPosition,KFbxConstraintPosition);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}