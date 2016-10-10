#pragma once
#include "stdafx.h"
#include "FbxMarker.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxColor.h"
#include "FbxTypedProperty.h"
#include "FbxProperty.h"



{
	namespace FbxSDK
	{
		void FbxMarker::CollectManagedMemory()
		{
			_DefaultColor = nullptr;
			_IKPivot = nullptr;
			_Occlusion = nullptr;
			_IKReachRotation = nullptr;
			_IKReachTranslation = nullptr;
			FbxNodeAttribute::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxMarker,KFbxMarker);
		FbxMarker::MarkerType FbxMarker::Type::get()
		{
			return (FbxMarker::MarkerType)_Ref()->GetType();
		}
		void FbxMarker::Type::set(FbxMarker::MarkerType value)
		{
			_Ref()->SetType((KFbxMarker::EType)value);
		}
		double FbxMarker::DefaultOcclusion::get()		
		{
			return _Ref()->GetDefaultOcclusion();
		}
		void FbxMarker::DefaultOcclusion::set(double value)		
		{
			return _Ref()->SetDefaultOcclusion(value);
		}


		double FbxMarker::DefaultIKReachTranslation::get()		
		{
			return _Ref()->GetDefaultIKReachTranslation();
		}
		void FbxMarker::DefaultIKReachTranslation::set(double value)		
		{
			return _Ref()->SetDefaultIKReachTranslation(value);
		}		
		
		double FbxMarker::DefaultIKReachRotation::get()		
		{
			return _Ref()->GetDefaultIKReachRotation();
		}
		void FbxMarker::DefaultIKReachRotation::set(double value)		
		{
			return _Ref()->SetDefaultIKReachRotation(value);
		}

		FbxColor^ FbxMarker::DefaultColor::get()
		{			
			if(!_DefaultColor)
				_DefaultColor = gcnew FbxColor(0,0,0,0);
			_Ref()->GetDefaultColor(*_DefaultColor->_Ref());
			return _DefaultColor;
		}
		
		void FbxMarker::DefaultColor::set(FbxColor^ value)
		{
			if(value )
			{				
				_Ref()->SetDefaultColor(*value->_Ref());
			}
		}
		FbxMarker::Look FbxMarker::MarkerLook::get()
		{
			return (FbxMarker::Look)_Ref()->Look.Get();
		}
		void FbxMarker::MarkerLook::set(FbxMarker::Look value)
		{
			_Ref()->Look.Set((KFbxMarker::ELook)value);
		}


		double FbxMarker::Size::get()
		{
			return _Ref()->Size.Get();
		}
		void FbxMarker::Size::set(double value)
		{
			_Ref()->Size.Set(value);
		}
		
		bool FbxMarker::ShowLabel::get()
		{
			return _Ref()->ShowLabel.Get();
		}
		void FbxMarker::ShowLabel::set(bool value)
		{
			_Ref()->ShowLabel.Set(value);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxMarker,IKPivot,FbxDouble3TypedProperty,IKPivot);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxMarker,KFbxProperty,GetOcclusion(),FbxPropertyManaged,Occlusion);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxMarker,KFbxProperty,GetIKReachTranslation(),FbxPropertyManaged,IKReachTranslation);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxMarker,KFbxProperty,GetIKReachRotation(),FbxPropertyManaged,IKReachRotation);
	
#ifndef DOXYGEN_SHOULD_SKIP_THIS	

		CLONE_DEFINITION(FbxMarker,KFbxMarker);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		
	}
}