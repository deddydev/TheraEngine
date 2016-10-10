#pragma once
#include "stdafx.h"
#include "FbxSkeleton.h"
#include "FbxColor.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"


{
	namespace FbxSDK
	{	


		FBXOBJECT_DEFINITION(FbxSkeleton,KFbxSkeleton);		

		void FbxSkeleton::Reset()
		{
			_Ref()->Reset();
		}


		void FbxSkeleton::Skeleton_Type::set(FbxSkeleton::SkeletonType value)
		{
			_Ref()->SetSkeletonType((KFbxSkeleton::ESkeletonType)value);
		}

		FbxSkeleton::SkeletonType FbxSkeleton::Skeleton_Type::get()
		{
			return (FbxSkeleton::SkeletonType)_Ref()->GetSkeletonType();
		}
		bool  FbxSkeleton::SkeletonTypeIsSet::get() 
		{
			return _Ref()->GetSkeletonTypeIsSet();
		}



		FbxSkeleton::SkeletonType FbxSkeleton::SkeletonTypeDefaultValue::get() 
		{
			return (FbxSkeleton::SkeletonType)_Ref()->GetSkeletonTypeDefaultValue();
		}

		double FbxSkeleton::LimbLengthDefaultValue::get()
		{
			return _Ref()->GetLimbLengthDefaultValue();
		}

		double FbxSkeleton::LimbNodeSizeDefaultValue::get() 
		{
			return _Ref()->GetLimbNodeSizeDefaultValue();
		}

		bool FbxSkeleton::SetLimbNodeColor(FbxColor^ color)
		{
			return _Ref()->SetLimbNodeColor(*color->_Ref());
		}

		FbxColor^ FbxSkeleton::GetLimbNodeColor()
		{
			return gcnew FbxColor(_Ref()->GetLimbNodeColor());
		}

		bool FbxSkeleton::LimbNodeColorIsSet::get() 
		{
			return _Ref()->GetLimbNodeColorIsSet();
		}


		FbxColor^ FbxSkeleton::GetLimbNodeColorDefaultValue() 
		{
			return gcnew FbxColor(_Ref()->GetLimbNodeColorDefaultValue());
		}		

		double FbxSkeleton::Size::get()
		{
			return _Ref()->Size.Get();
		}
		void FbxSkeleton::Size::set(double value)
		{
			_Ref()->Size.Set(value);
		}

		double FbxSkeleton::LimbLength::get()
		{
			return _Ref()->LimbLength.Get();
		}
		void FbxSkeleton::LimbLength::set(double value)
		{
			_Ref()->LimbLength.Set(value);
		}		

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxSkeleton,KFbxSkeleton);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS


	}
}