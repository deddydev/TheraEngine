#pragma once
#include "stdafx.h"
#include "FbxLimitsUtilities.h"
#include "FbxLimits.h"
#include "FbxNode.h"
#include "FbxVector4.h"				



{
	namespace FbxSDK
	{
		void FbxLimitsUtilities::CollectManagedMemory()
		{
			_Limits = nullptr;			
		}

		FbxLimitsUtilities::FbxLimitsUtilities(FbxNodeLimits^ limits)
		{
			_Limits = limits;
			_SetPointer(new KFbxLimitsUtilities(limits->_Ref()),true);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLimitsUtilities,KFbxNodeLimits,mLimits,FbxNodeLimits,Limits);
		void FbxLimitsUtilities::Limits::set(FbxNodeLimits^ value)
		{
			if(value)
			{
				_Ref()->mLimits = value->_Ref();
				_Limits = value;
			}
		}


		void FbxLimitsUtilities::SetAuto(FbxLimitsUtilities::LimitType type, bool Auto)
		{
			_Ref()->SetAuto((KFbxLimitsUtilities::ELimitType)type,Auto);
		}
		bool FbxLimitsUtilities::GetAuto(FbxLimitsUtilities::LimitType type)
		{
			return _Ref()->GetAuto((KFbxLimitsUtilities::ELimitType)type);
		}

		void FbxLimitsUtilities::SetEnable(FbxLimitsUtilities::LimitType type, bool enable)
		{
			_Ref()->SetEnable((KFbxLimitsUtilities::ELimitType)type,enable);
		}
		bool FbxLimitsUtilities::GetEnable(FbxLimitsUtilities::LimitType type)
		{
			return _Ref()->GetEnable((KFbxLimitsUtilities::ELimitType)type);
		}

		void FbxLimitsUtilities::SetDefault(FbxLimitsUtilities::LimitType type, FbxVector4^ Default)
		{
			_Ref()->SetDefault((KFbxLimitsUtilities::ELimitType)type,*Default->_Ref());
		}
		FbxVector4^ FbxLimitsUtilities::GetDefault(FbxLimitsUtilities::LimitType type)
		{
			return gcnew FbxVector4(_Ref()->GetDefault((KFbxLimitsUtilities::ELimitType)type));
		}

		void FbxLimitsUtilities::SetMin(FbxLimitsUtilities::LimitType type, FbxVector4^ min)
		{
			_Ref()->SetMin((KFbxLimitsUtilities::ELimitType)type,*min->_Ref());
		}
		FbxVector4^ FbxLimitsUtilities::GetMin(FbxLimitsUtilities::LimitType type)
		{
			return gcnew FbxVector4(_Ref()->GetMin((KFbxLimitsUtilities::ELimitType)type));
		}


		void FbxLimitsUtilities::SetMax(FbxLimitsUtilities::LimitType type, FbxVector4^ max)
		{
			_Ref()->SetMax((KFbxLimitsUtilities::ELimitType)type,*max->_Ref());
		}
		FbxVector4^ FbxLimitsUtilities::GetMax(FbxLimitsUtilities::LimitType type)
		{
			return gcnew FbxVector4(_Ref()->GetMax((KFbxLimitsUtilities::ELimitType)type));
		}


		FbxLimitsUtilities::RotationType FbxLimitsUtilities::Rotation_Type::get()
		{
			return (FbxLimitsUtilities::RotationType)_Ref()->GetRotationType();
		}
		void FbxLimitsUtilities::Rotation_Type::set(FbxLimitsUtilities::RotationType value)
		{
			_Ref()->SetRotationType((KFbxLimitsUtilities::ERotationType)value);
		}

		FbxLimitsUtilities::RotationClampType FbxLimitsUtilities::RotationClamp_Type::get()
		{
			return (FbxLimitsUtilities::RotationClampType)_Ref()->GetRotationClampType();
		}
		void FbxLimitsUtilities::RotationClamp_Type::set(FbxLimitsUtilities::RotationClampType value)
		{
			_Ref()->SetRotationClampType((KFbxLimitsUtilities::ERotationClampType)value);
		}

		FbxVector4^ FbxLimitsUtilities::RotationAxis::get()
		{
			return gcnew FbxVector4(_Ref()->GetRotationAxis());
		}
		void FbxLimitsUtilities::RotationAxis::set(FbxVector4^ value)
		{
			_Ref()->SetRotationAxis(*value->_Ref());
		}

		double FbxLimitsUtilities::AxisLength::get()
		{
			return _Ref()->GetAxisLength();
		}
		void FbxLimitsUtilities::AxisLength::set(double value)
		{
			_Ref()->SetAxisLength(value);
		}

		void FbxLimitsUtilities::UpdateAutomatic(FbxNode^ node)
		{
			_Ref()->UpdateAutomatic(node->_Ref());
		}
		FbxVector4^ FbxLimitsUtilities::GetEndPointTranslation(FbxNode^ node)
		{
			return gcnew FbxVector4(_Ref()->GetEndPointTranslation(node->_Ref()));
		}
		FbxVector4^ FbxLimitsUtilities::GetEndSite(FbxNode^ node)
		{
			return gcnew FbxVector4(_Ref()->GetEndSite(node->_Ref()));
		}
	}
}