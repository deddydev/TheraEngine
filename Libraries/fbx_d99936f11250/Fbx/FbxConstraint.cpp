#pragma once
#include "stdafx.h"
#include "FbxConstraint.h"
#include "FbxError.h"
#include "FbxVector4.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"



{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxConstraint,KFbxConstraint);
		void FbxConstraint::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}							
		FbxConstraint::ConstraintType FbxConstraint::Constraint_Type::get()
		{
			return (ConstraintType)_Ref()->GetConstraintType();
		}		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxConstraint,GetError(),FbxErrorManaged,KError);

		FbxConstraint::Error FbxConstraint::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}			
		String^ FbxConstraint::LastErrorString::get()
		{
			return gcnew System::String(_Ref()->GetLastErrorString());
		}			
		void FbxConstraint::SetOffset(FbxVector4^ offset)
		{
			_Ref()->SetOffset(*offset->_Ref());
		}

	}
}