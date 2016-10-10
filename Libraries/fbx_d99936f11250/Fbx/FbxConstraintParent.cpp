#pragma once
#include "stdafx.h"
#include "FbxConstraintParent.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxVector4.h"

#define PROPERTY_GETSET_DEFINITION(PropType,PropName)\
	VALUE_PROPERTY_GET_DEFINATION(FbxConstraintParent,Get##PropName(),PropType,PropName);\
	void FbxConstraintParent::PropName::set(PropType value){ _Ref()->Set##PropName(value);}



{
	namespace FbxSDK
	{		
		void FbxConstraintParent::CollectManagedMemory()
		{
			_ConstrainedObject = nullptr;			
			FbxConstraint::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxConstraintParent,KFbxConstraintParent);

		PROPERTY_GETSET_DEFINITION(bool,Lock);
		PROPERTY_GETSET_DEFINITION(bool,Active);
		PROPERTY_GETSET_DEFINITION(bool,AffectTranslationX);
		PROPERTY_GETSET_DEFINITION(bool,AffectTranslationY);
		PROPERTY_GETSET_DEFINITION(bool,AffectTranslationZ);
		PROPERTY_GETSET_DEFINITION(bool,AffectRotationX);
		PROPERTY_GETSET_DEFINITION(bool,AffectRotationY);
		PROPERTY_GETSET_DEFINITION(bool,AffectRotationZ);

		void FbxConstraintParent::SetTranslationOffset(FbxObjectManaged^ obj, FbxVector4^ translation)
		{
			_Ref()->SetTranslationOffset(obj->_Ref(),*translation->_Ref());
		}
		FbxVector4^ FbxConstraintParent::GetTranslationOffset(FbxObjectManaged^ obj)
		{
			return gcnew FbxVector4(_Ref()->GetTranslationOffset(obj->_Ref()));
		}
		void FbxConstraintParent::SetRotationOffset(FbxObjectManaged^ obj, FbxVector4^ rotation)
		{
			_Ref()->SetRotationOffset(obj->_Ref(),*rotation->_Ref());
		}
		FbxVector4^ FbxConstraintParent::GetRotationOffset(FbxObjectManaged^ obj)
		{
			return gcnew FbxVector4(_Ref()->GetRotationOffset(obj->_Ref()));
		}
		void FbxConstraintParent::SetWeight(double weight)
		{
			_Ref()->SetWeight(weight);
		}
		double FbxConstraintParent::GetSourceWeight(FbxObjectManaged^ obj)
		{
			return _Ref()->GetSourceWeight(obj->_Ref());
		}
		void FbxConstraintParent::AddConstraintSource(FbxObjectManaged^ obj, double weight)
		{
			_Ref()->AddConstraintSource(obj->_Ref(),weight);
		}
		VALUE_PROPERTY_GET_DEFINATION(FbxConstraintParent,GetConstraintSourceCount(),int,ConstraintSourceCount)

		FbxObjectManaged^ FbxConstraintParent::GetConstraintSource(int index)
		{
			KFbxObject* obj = _Ref()->GetConstraintSource(index);
			return FbxCreator::CreateFbxObject(obj);
		}
		
		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxConstraintParent,KFbxObject,GetConstrainedObject(),FbxObjectManaged,ConstrainedObject);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxConstraintParent,SetConstrainedObject,FbxObjectManaged,ConstrainedObject);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			
		CLONE_DEFINITION(FbxConstraintParent,KFbxConstraintParent);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}