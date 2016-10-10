#pragma once
#include "stdafx.h"
#include "FbxTexture.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxTypedProperty.h"
#include "FbxString.h"
#include "FbxVector4.h"

#define GET_VALUE_FROM_TYPEDPROPERTY(Class,PropType,PropName,NativeType)\
	PropType Class::PropName::get(){return (PropType)_Ref()->PropName.Get();}\
	void Class::PropName::set(PropType value){_Ref()->PropName.Set((NativeType)value);}	


{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxTexture,KFbxTexture);
		void FbxTexture::CollectManagedMemory()
		{

			_Translation= nullptr;
			_Rotation = nullptr;
			_Scaling = nullptr;
			_RotationPivot = nullptr;
			_ScalingPivot = nullptr;
			_DefaultR = nullptr;
			_DefaultS = nullptr;
			_DefaultT = nullptr;
			FbxShadingNode::CollectManagedMemory();
		}

		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,FbxTexture::TextureUse6,TextureTypeUse,KFbxTexture::ETextureUse6);

		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,double,Alpha,double);		

		// Mapping information
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,FbxTexture::UnifiedMappingType,CurrentMappingType,KFbxTexture::EUnifiedMappingType);
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,FbxTexture::WrapMode,WrapModeU,KFbxTexture::EWrapMode);
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,FbxTexture::WrapMode,WrapModeV,KFbxTexture::EWrapMode);				
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,bool,UVSwap,bool);		

		// Texture positioning
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTexture,Translation,FbxDouble3TypedProperty,Translation);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTexture,Rotation,FbxDouble3TypedProperty,Rotation);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTexture,Scaling,FbxDouble3TypedProperty,Scaling);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTexture,RotationPivot,FbxDouble3TypedProperty,RotationPivot);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTexture,ScalingPivot,FbxDouble3TypedProperty,ScalingPivot);		

		// Material management
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,bool,UseMaterial,bool);

		// Blend mode
		GET_VALUE_FROM_TYPEDPROPERTY(FbxTexture,FbxTexture::BlendMode,CurrentTextureBlendMode,KFbxTexture::EBlendMode);		

		// UV set to use.

		FbxStringManaged^ FbxTexture::UVSet::get()
		{			
			return gcnew FbxStringManaged(_Ref()->UVSet.Get());
		}
		void FbxTexture::UVSet::set(FbxStringManaged^ value)
		{			
			_Ref()->UVSet.Set(*value->_Ref());
		}

		void FbxTexture::Reset()
		{
			_Ref()->Reset();
		}
		bool FbxTexture::SetFileName(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			bool b = _Ref()->SetFileName(n);
			FREECHARPOINTER(n);
			return b;
		}

		bool FbxTexture::SetRelativeFileName(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			bool b = _Ref()->SetRelativeFileName(n);
			FREECHARPOINTER(n);
			return b;
		}
		String^ FbxTexture::FileName::get()
		{
			return gcnew String(_Ref()->GetFileName());
		}
		String^ FbxTexture::RelativeFileName::get()
		{
			return gcnew String(_Ref()->GetRelativeFileName());
		}
		void FbxTexture::SwapUV::set(bool value)
		{
			_Ref()->SetSwapUV(value);
		}
		bool FbxTexture::SwapUV::get()
		{
			return _Ref()->GetSwapUV();
		}
		FbxTexture::AlphaSource FbxTexture::AlphaSrc::get()
		{
			return (FbxTexture::AlphaSource)_Ref()->GetAlphaSource();
		}
		void FbxTexture::AlphaSrc::set(FbxTexture::AlphaSource value)
		{
			return _Ref()->SetAlphaSource((KFbxTexture::EAlphaSource)value);
		}

		void FbxTexture::SetCropping(int left, int top, int right, int bottom)
		{
			_Ref()->SetCropping(left,top,right,bottom);
		}
		int FbxTexture::CroppingLeft::get()
		{
			return _Ref()->GetCroppingLeft();
		}
		int FbxTexture::CroppingTop::get()
		{
			return _Ref()->GetCroppingTop();
		}
		int FbxTexture::CroppingRight::get()
		{
			return _Ref()->GetCroppingRight();
		}
		int FbxTexture::CroppingBottom::get()
		{
			return _Ref()->GetCroppingBottom();
		}		
		FbxTexture::MappingType FbxTexture::Mapping::get()
		{
			return (FbxTexture::MappingType)_Ref()->GetMappingType();
		}
		void FbxTexture::Mapping::set(FbxTexture::MappingType value)
		{
			_Ref()->SetMappingType((KFbxTexture::EMappingType)value);
		}				

		FbxTexture::PlanarMappingNormal FbxTexture::PlanarMappingNormalType::get()
		{
			return (FbxTexture::PlanarMappingNormal)_Ref()->GetPlanarMappingNormal();
		}
		void FbxTexture::PlanarMappingNormalType::set(FbxTexture::PlanarMappingNormal value)
		{
			_Ref()->SetPlanarMappingNormal((KFbxTexture::EPlanarMappingNormal)value);
		}


		FbxTexture::MaterialUse FbxTexture::MaterialUseType::get()
		{
			return (FbxTexture::MaterialUse)_Ref()->GetMaterialUse();
		}
		void FbxTexture::MaterialUseType::set(FbxTexture::MaterialUse value)
		{
			_Ref()->SetMaterialUse((KFbxTexture::EMaterialUse)value);
		}
		FbxTexture::TextureUse FbxTexture::TextureUseType::get()
		{
			return (FbxTexture::TextureUse)_Ref()->GetTextureUse();
		}
		void FbxTexture::TextureUseType::set(FbxTexture::TextureUse value)
		{
			_Ref()->SetTextureUse((KFbxTexture::ETextureUse)value);
		}

		void FbxTexture::SetWrapMode(FbxTexture::WrapMode wrapU, FbxTexture::WrapMode wrapV)
		{
			_Ref()->SetWrapMode((KFbxTexture::EWrapMode)wrapU,(KFbxTexture::EWrapMode)wrapV);
		}
		/*FbxTexture::WrapMode FbxTexture::WrapModeU::get()
		{
			return (FbxTexture::WrapMode)_Ref()->GetWrapModeU();
		}*/
		/*FbxTexture::WrapMode FbxTexture::WrapModeV::get()
		{
			return (FbxTexture::WrapMode)_Ref()->GetWrapModeV();
		}*/
		/*void FbxTexture::BlendMode::set(FbxTexture::BlendMode value)
		{
			_Ref()->SetBlendMode((KFbxTexture::EBlendMode)value);
		}
		FbxTexture::BlendMode FbxTexture::BlendMode::get()
		{
			return (FbxTexture::BlendMode)_Ref()->GetBlendMode();
		}*/

		FbxVector4^ FbxTexture::DefaultT::get()
		{
			if(_DefaultT = nullptr)
				_DefaultT = gcnew FbxVector4(0,0,0,0);
			_Ref()->GetDefaultT(*_DefaultT->_Ref());
			return _DefaultT;
		}

		void FbxTexture::DefaultT::set(FbxVector4^ value)
		{
			if(value )
			{
				_Ref()->SetDefaultT(*value->_Ref());
			}
		}

		FbxVector4^ FbxTexture::DefaultR::get()
		{
			if(_DefaultR = nullptr)
				_DefaultR = gcnew FbxVector4(0,0,0,0);
			_Ref()->GetDefaultR(*_DefaultR->_Ref());
			return _DefaultR;
		}
		void FbxTexture::DefaultR::set(FbxVector4^ value)
		{
			if(value )
			{
				_Ref()->SetDefaultR(*value->_Ref());
			}
		}

		FbxVector4^ FbxTexture::DefaultS::get()
		{
			if(_DefaultS = nullptr)
				_DefaultS = gcnew FbxVector4(0,0,0,0);
			_Ref()->GetDefaultS(*_DefaultS->_Ref());
			return _DefaultS;
		}
		void FbxTexture::DefaultS::set(FbxVector4^ value)
		{
			if(value )
			{
				_Ref()->SetDefaultS(*value->_Ref());
			}
		}

		double FbxTexture::DefaultAlpha::get()
		{
			return _Ref()->GetDefaultAlpha();
		}
		void FbxTexture::DefaultAlpha::set(double value)
		{
			_Ref()->SetDefaultAlpha(value);
		}

		void FbxTexture::SetTranslation(double u,double v)
		{
			_Ref()->SetTranslation(u,v);
		}
		double FbxTexture::TranslationU::get()
		{
			return _Ref()->GetTranslationU();
		}
		double FbxTexture::TranslationV::get()
		{
			return _Ref()->GetTranslationV();
		}
		void FbxTexture::SetRotation(double u, double v, double w)
		{
			_Ref()->SetRotation(u,v,w);
		}

		double FbxTexture::RotationU::get()
		{
			return _Ref()->GetRotationU();
		}		
		double FbxTexture::RotationV::get()
		{
			return _Ref()->GetRotationV();
		}
		double FbxTexture::RotationW::get()
		{
			return _Ref()->GetRotationW();
		}		
		void FbxTexture::SetScale(double u,double v)
		{
			_Ref()->SetScale(u,v);
		}
		double FbxTexture::ScaleU::get()
		{
			return _Ref()->GetScaleU();
		}		
		double FbxTexture::ScaleV::get()
		{
			return _Ref()->GetScaleV();
		}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
		// Clone
		CLONE_DEFINITION(FbxTexture,KFbxTexture);		
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}