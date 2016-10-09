#pragma once
#include "stdafx.h"
#include "FbxGlobalLightSettings.h"
#include "FbxColor.h"
#include "FbxVector4.h"
#include "FbxError.h"


namespace Skill
{
	namespace FbxSDK
	{
		void FbxGlobalLightSettings::CollectManagedMemory()
		{			
			_KError = nullptr;	
			if(_list)
				_list->Clear();
			_list = nullptr;
		}		

		FbxGlobalLightSettings::FbxGlobalLightSettings(KFbxGlobalLightSettings* instance)
		{
			this->_FbxGlobalLightSettings = instance;
			_Free = false;
			_list = gcnew System::Collections::Generic::List<FbxShadowPlane^>();
			for(int i =0;i< _Ref()->GetShadowPlaneCount();i++)
			{
				KFbxGlobalLightSettings::KFbxShadowPlane* p = _Ref()->GetShadowPlane(i);
				if(p)
					_list->Add(gcnew FbxGlobalLightSettings::FbxShadowPlane(p));
				else
					_list->Add(nullptr);
			}
		}

		FbxColor^ FbxGlobalLightSettings::AmbientColor::get()
		{
			return gcnew FbxColor(_Ref()->GetAmbientColor());
		}
		void FbxGlobalLightSettings::AmbientColor::set(FbxColor^ value)
		{
			_Ref()->SetAmbientColor(*value->_Ref());
		}
		bool FbxGlobalLightSettings::FogEnable::get()
		{
			return _Ref()->GetFogEnable();
		}
		void FbxGlobalLightSettings::FogEnable::set(bool value)
		{
			_Ref()->SetFogEnable(value);
		}
		FbxColor^ FbxGlobalLightSettings::FogColor::get()
		{
			return gcnew FbxColor(_Ref()->GetFogColor());
		}
		void FbxGlobalLightSettings::FogColor::set(FbxColor^ value)
		{
			_Ref()->SetFogColor(*value->_Ref());
		}
		FbxGlobalLightSettings::LightFogMode FbxGlobalLightSettings::FogMode::get()
		{
			return (LightFogMode)_Ref()->GetFogMode(); 
		}
		void FbxGlobalLightSettings::FogMode::set(LightFogMode value)
		{
			_Ref()->SetFogMode((KFbxGlobalLightSettings::EFogMode)value);
		}			
		double FbxGlobalLightSettings::FogDensity::get()
		{
			return _Ref()->GetFogDensity();
		}
		void FbxGlobalLightSettings::FogDensity::set(double value)
		{
			_Ref()->SetFogDensity(value);
		}
		double FbxGlobalLightSettings::FogStart::get()
		{
			return _Ref()->GetFogStart();
		}
		void FbxGlobalLightSettings::FogStart::set(double value)
		{
			_Ref()->SetFogStart(value);
		}			
		double FbxGlobalLightSettings::FogEnd::get()
		{
			return _Ref()->GetFogEnd();
		}
		void FbxGlobalLightSettings::FogEnd::set(double value)
		{
			_Ref()->SetFogEnd(value);
		}		

		void FbxGlobalLightSettings::FbxShadowPlane::CollectManagedMemory()
		{
			_Normal = nullptr;
			_Origin = nullptr;
		}		

		bool FbxGlobalLightSettings::FbxShadowPlane::Enable::get()
		{
			return _Ref()->mEnable;
		}
		void FbxGlobalLightSettings::FbxShadowPlane::Enable::set(bool value)
		{
			_Ref()->mEnable = value;
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGlobalLightSettings::FbxShadowPlane,mOrigin,FbxVector4,Origin);		
		void FbxGlobalLightSettings::FbxShadowPlane::Origin::set(FbxVector4^ value)
		{
			_Ref()->mOrigin = *value->_Ref();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGlobalLightSettings::FbxShadowPlane,mNormal,FbxVector4,Normal);
		void FbxGlobalLightSettings::FbxShadowPlane::Normal::set(FbxVector4^ value)
		{
			_Ref()->mNormal = *value->_Ref();
		}

		bool FbxGlobalLightSettings::ShadowEnable::get()
		{
			return _Ref()->GetShadowEnable();
		}
		void FbxGlobalLightSettings::ShadowEnable::set(bool value)
		{
			_Ref()->SetShadowEnable(value);
		}
		double FbxGlobalLightSettings::ShadowIntensity::get()
		{
			return _Ref()->GetShadowIntensity();
		}
		void FbxGlobalLightSettings::ShadowIntensity::set(double value)
		{
			_Ref()->SetShadowIntensity(value);
		}
		int FbxGlobalLightSettings::ShadowPlaneCount::get()
		{
			return _Ref()->GetShadowPlaneCount();
		}


		FbxGlobalLightSettings::FbxShadowPlane^ FbxGlobalLightSettings::GetShadowPlane(int index)
		{
			return _list[index];
		}
		void FbxGlobalLightSettings::AddShadowPlane(FbxGlobalLightSettings::FbxShadowPlane^ shadowPlane)
		{
			_Ref()->AddShadowPlane(*shadowPlane->_Ref());
			_list->Add(shadowPlane);
		}
		void FbxGlobalLightSettings::RemoveAllShadowPlanes()
		{
			_Ref()->RemoveAllShadowPlanes();
			_list->Clear();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGlobalLightSettings,GetError(),FbxErrorManaged,KError);

		FbxGlobalLightSettings::Error FbxGlobalLightSettings::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}			
		String^ FbxGlobalLightSettings::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}			
		void FbxGlobalLightSettings::RestoreDefaultSettings()
		{
			_Ref()->RestoreDefaultSettings();
		}
		void FbxGlobalLightSettings::CopyFrom(FbxGlobalLightSettings^ settings)
		{
			*this->_FbxGlobalLightSettings = *settings->_FbxGlobalLightSettings;
		}

	}
}