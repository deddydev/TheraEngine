#pragma once
#include "stdafx.h"
#include "FbxLight.h"
#include "FbxTexture.h"
#include "FbxTypedProperty.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"

#define GETSET_FROM_TYPED_PROPERTY(PropType,PropName)\
	PropType FbxLight::PropName::get(){	return _Ref()->PropName.Get();}\
	void FbxLight::PropName::set(PropType value){_Ref()->PropName.Set(value);}

#define GETSET_FROM_Double_PROPERTY(PropType,PropName)\
	REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxLight,PropName,PropType,PropName);



{
	namespace FbxSDK
	{
		void FbxLight::CollectManagedMemory()
		{
			_Color = nullptr;
			_ConeAngle = nullptr;
			_DecayStart = nullptr;
			_FarAttenuationEnd = nullptr;
			_FarAttenuationStart = nullptr;
			_FileName = nullptr;
			_Fog = nullptr;
			_HotSpot = nullptr;
			_Intensity = nullptr;
			_NearAttenuationEnd = nullptr;
			_NearAttenuationStart = nullptr;
			_ShadowColor = nullptr;
			_ShadowTexture = nullptr;			

			FbxNodeAttribute::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxLight,KFbxLight);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLight,KFbxTexture,GetShadowTexture(),FbxTexture,ShadowTexture);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLight,SetShadowTexture,FbxTexture,ShadowTexture);

		FbxLight::LightType FbxLight::Light_Type::get()
		{
			return (FbxLight::LightType)_Ref()->LightType.Get();
		}
		void FbxLight::Light_Type::set(FbxLight::LightType value)
		{
			_Ref()->LightType.Set((KFbxLight::ELightType)value);
		}



		GETSET_FROM_TYPED_PROPERTY(bool,CastLight);
		GETSET_FROM_TYPED_PROPERTY(bool,DrawVolumetricLight);
		GETSET_FROM_TYPED_PROPERTY(bool,DrawGroundProjection);
		GETSET_FROM_TYPED_PROPERTY(bool,DrawFrontFacingVolumetricLight);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,Color);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,Intensity);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,HotSpot);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,ConeAngle);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,Fog);
		
		FbxLight::DecayType FbxLight::Decay_Type::get()
		{
			return (FbxLight::DecayType)_Ref()->DecayType.Get();
		}
		void FbxLight::Decay_Type::set(FbxLight::DecayType value)
		{
			_Ref()->DecayType.Set((KFbxLight::EDecayType)value);
		}


		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,DecayStart);
		GETSET_FROM_Double_PROPERTY(FbxStringTypedProperty,FileName);
		GETSET_FROM_TYPED_PROPERTY(bool,EnableNearAttenuation);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,NearAttenuationStart);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,NearAttenuationEnd);
		GETSET_FROM_TYPED_PROPERTY(bool,EnableFarAttenuation);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FarAttenuationStart);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FarAttenuationEnd);
		GETSET_FROM_TYPED_PROPERTY(bool,CastShadows);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,ShadowColor);

		CLONE_DEFINITION(FbxLight,KFbxLight);
	}
}