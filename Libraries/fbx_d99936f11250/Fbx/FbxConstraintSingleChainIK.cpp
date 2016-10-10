#pragma once
#include "stdafx.h"
#include "FbxConstraintSingleChainIK.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxVector4.h"


#define PROPERTY_GETSET_DEFINITION(PropType,PropName)\
	VALUE_PROPERTY_GET_DEFINATION(FbxConstraintSingleChainIK,Get##PropName(),PropType,PropName);\
	void FbxConstraintSingleChainIK::PropName::set(PropType value){ _Ref()->Set##PropName(value);}


{
	namespace FbxSDK
	{		
		void FbxConstraintSingleChainIK::CollectManagedMemory()
		{
			_EffectorObject = nullptr;
			_EndJointObject = nullptr;
			_FirstJointObject = nullptr;
			_PoleVector = nullptr;			
			FbxConstraint::CollectManagedMemory();			
		}
		FBXOBJECT_DEFINITION(FbxConstraintSingleChainIK,KFbxConstraintSingleChainIK);


		PROPERTY_GETSET_DEFINITION(bool,Lock);
		PROPERTY_GETSET_DEFINITION(bool,Active);

		FbxConstraintSingleChainIK::PoleVectorType FbxConstraintSingleChainIK::PoleVector_Type::get()
		{
			return (FbxConstraintSingleChainIK::PoleVectorType)_Ref()->GetPoleVectorType();
		}
		void FbxConstraintSingleChainIK::PoleVector_Type::set(FbxConstraintSingleChainIK::PoleVectorType value)
		{
			_Ref()->SetPoleVectorType((KFbxConstraintSingleChainIK::EPoleVectorType)value);
		}


		FbxConstraintSingleChainIK::SolverType FbxConstraintSingleChainIK::Solver_Type::get()
		{
			return (FbxConstraintSingleChainIK::SolverType)_Ref()->GetSolverType();
		}
		void FbxConstraintSingleChainIK::Solver_Type::set(FbxConstraintSingleChainIK::SolverType value)
		{
			_Ref()->SetSolverType((KFbxConstraintSingleChainIK::ESolverType)value);
		}

		FbxConstraintSingleChainIK::EvalTS FbxConstraintSingleChainIK::Eval_TS::get()
		{
			return (FbxConstraintSingleChainIK::EvalTS)_Ref()->GetEvalTS();
		}
		void FbxConstraintSingleChainIK::Eval_TS::set(FbxConstraintSingleChainIK::EvalTS value)
		{
			_Ref()->SetEvalTS((KFbxConstraintSingleChainIK::EEvalTS)value);
		}

		double FbxConstraintSingleChainIK::Weight::get()
		{
			return _Ref()->GetWeight();
		}
		void FbxConstraintSingleChainIK::Weight::set(double value)
		{
			_Ref()->SetWeight(value);
		}

		double FbxConstraintSingleChainIK::GetPoleVectorObjectWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetPoleVectorObjectWeight(obj->_Ref());
		}
		void FbxConstraintSingleChainIK::AddPoleVectorObject(FbxObjectManaged^ obj, double weight)
		{
			_Ref()->AddPoleVectorObject(obj->_Ref(),weight);
		}
		int FbxConstraintSingleChainIK::ConstraintPoleVectorCount::get()
		{
			return _Ref()->GetConstraintPoleVectorCount();
		}
		FbxObjectManaged^ FbxConstraintSingleChainIK::GetPoleVectorObject(int index)
		{
			KFbxObject* obj = _Ref()->GetPoleVectorObject(index);
			return FbxCreator::CreateFbxObject(obj);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraintSingleChainIK,GetPoleVector(),FbxVector4,PoleVector);
		void FbxConstraintSingleChainIK::PoleVector::set(FbxVector4^ value)
		{
			_Ref()->SetPoleVector(*value->_Ref());
		}

		double FbxConstraintSingleChainIK::Twist::get()
		{
			return _Ref()->GetTwist();
		}
		void FbxConstraintSingleChainIK::Twist::set(double value)
		{
			_Ref()->SetTwist(value);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintSingleChainIK,KFbxObject,GetFirstJointObject(),FbxObjectManaged,FirstJointObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintSingleChainIK,SetFirstJointObject,FbxObjectManaged,FirstJointObject);

		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintSingleChainIK,KFbxObject,GetEndJointObject(),FbxObjectManaged,EndJointObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintSingleChainIK,SetEndJointObject,FbxObjectManaged,EndJointObject);

		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintSingleChainIK,KFbxObject,GetEffectorObject(),FbxObjectManaged,EffectorObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintSingleChainIK,SetEffectorObject,FbxObjectManaged,EffectorObject);		

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		// Clone
		CLONE_DEFINITION(FbxConstraintSingleChainIK,KFbxConstraintSingleChainIK);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}