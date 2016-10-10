#pragma once
#include "stdafx.h"
#include "FbxLimits.h"
#include "FbxNode.h"
#include "FbxVector4.h"


{
	namespace FbxSDK
	{		
		void FbxLimits::CollectManagedMemory()
		{
		}

		void FbxLimits::SetLimitMinActive(bool xActive,bool yActive, bool zActive)
		{
			_Ref()->SetLimitMinActive(xActive,yActive,zActive);
		}
		void FbxLimits::GetLimitMinActive([OutAttribute]bool %xActive, [OutAttribute]bool %yActive, [OutAttribute]bool %zActive)
		{
			bool x,y,z;
			_Ref()->GetLimitMinActive(x,y,z);
			xActive = x;
			yActive = y;
			zActive = z;
		}
		void FbxLimits::SetLimitMaxActive(bool xActive, bool yActive, bool zActive)
		{
			_Ref()->SetLimitMaxActive(xActive,yActive,zActive);
		}
		void FbxLimits::GetLimitMaxActive([OutAttribute]bool %xActive, [OutAttribute]bool %yActive, [OutAttribute]bool %zActive)
		{
			bool x,y,z;
			_Ref()->GetLimitMaxActive(x,y,z);
			xActive = x;
			yActive = y;
			zActive = z;
		}
		bool FbxLimits::LimitSomethingActive::get()
		{
			return _Ref()->GetLimitSomethingActive();
		}
		FbxVector4^ FbxLimits::LimitMin::get()
		{
			return gcnew FbxVector4(_Ref()->GetLimitMin());
		}
		void FbxLimits::LimitMin::set(FbxVector4^ value)
		{
			_Ref()->SetLimitMin(*value->_Ref());
		}

		FbxVector4^ FbxLimits::LimitMax::get()
		{
			return gcnew FbxVector4(_Ref()->GetLimitMax());
		}
		void FbxLimits::LimitMax::set(FbxVector4^ value)
		{
			_Ref()->SetLimitMax(*value->_Ref());
		}

		/*LimitedProperty FbxLimits::Limited_Property::get()
		{
		return (LimitedProperty)_Ref()->GetLimitedProperty();
		}
		void FbxLimits::Limited_Property::set(LimitedProperty value)
		{
		_Ref()->SetLimitedProperty((ELimitedProperty)value);
		}*/
		void FbxLimits::CopyFrom(FbxLimits^ limits)
		{
			*this->_FbxLimits = *limits->_Ref();
		}





		void FbxNodeLimits::CollectManagedMemory()
		{
			_LimitedNode = nullptr;
			_RotationLimits = nullptr;
			_ScalingLimits = nullptr;
			_TranslationLimits = nullptr;
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNodeLimits,KFbxNode,GetLimitedNode(),FbxNode,LimitedNode);

		bool FbxNodeLimits::TranslationLimitActive::get()
		{
			return _Ref()->GetTranslationLimitActive();
		}
		void FbxNodeLimits::TranslationLimitActive::set(bool value)
		{
			_Ref()->SetTranslationLimitActive(value);
		}
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxNodeLimits,mTranslationLimits,FbxLimits,TranslationLimits);			


		bool FbxNodeLimits::RotationLimitActive::get()
		{
			return _Ref()->GetRotationLimitActive();
		}
		void FbxNodeLimits::RotationLimitActive::set(bool value)
		{
			_Ref()->SetRotationLimitActive(value);
		}
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxNodeLimits,mRotationLimits,FbxLimits,RotationLimits);			


		bool FbxNodeLimits::ScalingLimitActive::get()
		{
			return _Ref()->GetScalingLimitActive();
		}
		void FbxNodeLimits::ScalingLimitActive::set(bool value)
		{
			_Ref()->SetScalingLimitActive(value);
		}
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxNodeLimits,mScalingLimits,FbxLimits,ScalingLimits);			

	}
}