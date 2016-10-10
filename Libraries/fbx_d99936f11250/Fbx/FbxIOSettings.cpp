#pragma once
#include "stdafx.h"
#include "FbxIOSettings.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxProperty.h"
#include "FbxDataType.h"
#include "FbxString.h"


{
	namespace FbxSDK
	{
		namespace IO
		{	
			void FbxSoInfoManaged::CollectManagedMemory()
			{
				this->ASF = nullptr;
			}			
			void FbxSoInfoManaged::Reset(ImpExp impExp)
			{
				_Ref()->Reset((KsoInfo::EIMPEXP)impExp);
			}
			void FbxSoInfoManaged::SetTimeMode(FbxTime::TimeMode timeMode, double customFrameRate)
			{
				_Ref()->SetTimeMode((KTime::ETimeMode)timeMode,customFrameRate);
			}
			FbxTime::TimeMode FbxSoInfoManaged::TimeMode::get()
			{
				return (FbxTime::TimeMode)_Ref()->GetTimeMode();
			}					
			FbxTime^ FbxSoInfoManaged::FramePeriod::get()
			{						
				return gcnew FbxTime(_Ref()->GetFramePeriod());				
			}
			void FbxSoInfoManaged::SetASFScene(FbxObject^ ASFScene, bool ASFSceneOwned)
			{
				_Ref()->SetASFScene(ASFScene->_Ref(),ASFSceneOwned);
				this->ASF = ASFScene;
			}
			FbxObjectManaged^ FbxSoInfoManaged::GetASFScene()
			{
				if(ASF)
					return ASF;
				else
				{					
					ASF = FbxCreator::CreateFbxObject(_Ref()->GetASFScene());
					return ASF;
				}
			}													

			FBXOBJECT_DEFINITION(FbxIOSettingsManaged,KFbxIOSettings);

			void FbxIOSettingsManaged::CollectManagedMemory()
			{
				this->_ImpInfo = nullptr;
				this->_ExpInfo = nullptr;
				FbxObjectManaged::CollectManagedMemory();
			}

			void FbxIOSettingsManaged::AllocateIOSettings(FbxSdkManagerManaged^ manager)
			{
				KFbxIOSettings::AllocateIOSettings(*manager->_Ref());
			}

			bool FbxIOSettingsManaged::IsIOSettingsAllocated()
			{
				return KFbxIOSettings::IsIOSettingsAllocated();
			}					
			void FbxIOSettingsManaged::FreeIOSettings()
			{
				_Ref()->FreeIOSettings();
			}

			FbxIOSettingsManaged^ FbxIOSettingsManaged::IOSettingsRef::get()
			{
				if(refIO )
					refIO->_SetPointer(&KFbxIOSettings::IOSettingsRef(),false);
				else
					refIO = gcnew FbxIOSettingsManaged(&KFbxIOSettings::IOSettingsRef());
				return refIO;				
			}		

			FbxPropertyManaged^ FbxIOSettingsManaged::AddPropertyGroup(String^ name, FbxDataType^ dataType, String^ label)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				STRINGTO_CONSTCHAR_ANSI(l,label);

				KFbxProperty p = _Ref()->AddPropertyGroup(n,*dataType->_Ref(),l);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);				
				FREECHARPOINTER(n);
				FREECHARPOINTER(l);
				return p1;
			}

			FbxPropertyManaged^ FbxIOSettingsManaged::AddPropertyGroup(String^ name)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);				
				KFbxProperty p = _Ref()->AddPropertyGroup(n);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);
				FREECHARPOINTER(n);				
				return p1;
			}

			FbxPropertyManaged^ FbxIOSettingsManaged::AddPropertyGroup(FbxPropertyManaged^ parentProperty, 
				String^ name,
				FbxDataType^ dataType, 
				String^ label,
				bool visible,
				bool savable,
				bool enabled)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				STRINGTO_CONSTCHAR_ANSI(l,label);

				KFbxProperty p = _Ref()->AddPropertyGroup(*parentProperty->_Ref(),n,*dataType->_Ref(),l,visible,savable,enabled);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);
				FREECHARPOINTER(n);
				FREECHARPOINTER(l);
				return p1;
			}

			FbxPropertyManaged^ FbxIOSettingsManaged::AddPropertyGroup(FbxPropertyManaged^ parentProperty, 
				String^ name)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);				
				KFbxProperty p = _Ref()->AddPropertyGroup(*parentProperty->_Ref(),n);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);
				FREECHARPOINTER(n);				
				return p1;
			}

			FbxPropertyManaged^ FbxIOSettingsManaged::GetProperty(String^ name)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);				
				KFbxProperty p = _Ref()->GetProperty(n);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);
				FREECHARPOINTER(n);				
				return p1;
			}
			FbxPropertyManaged^ FbxIOSettingsManaged::GetProperty(FbxPropertyManaged^ parentProperty,String^ name)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);				
				KFbxProperty p = _Ref()->GetProperty(*parentProperty->_Ref(),n);
				FbxPropertyManaged^ p1 = gcnew FbxPropertyManaged(p);
				FREECHARPOINTER(n);				
				return p1;
			}
			bool FbxIOSettingsManaged::GetBoolProp(String^ name, bool defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				bool b = _Ref()->GetBoolProp(n,defValue);
				FREECHARPOINTER(n);	
				return b;
			}
			void FbxIOSettingsManaged::SetBoolProp(String^ name, bool value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetBoolProp(n,value);
				FREECHARPOINTER(n);					
			}

			double FbxIOSettingsManaged::GetDoubleProp(String^ name, double defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				double d = _Ref()->GetDoubleProp(n,defValue);
				FREECHARPOINTER(n);	
				return d;
			}
			void FbxIOSettingsManaged::SetDoubleProp(String^ name, double value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetDoubleProp(n,value);
				FREECHARPOINTER(n);					
			}
			int FbxIOSettingsManaged::GetIntProp(String^ name, int defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				int i = _Ref()->GetIntProp(n,defValue);
				FREECHARPOINTER(n);	
				return i;
			}
			void FbxIOSettingsManaged::SetIntProp(String^ name, int value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetIntProp(n,value);
				FREECHARPOINTER(n);					
			}

			FbxTime^ FbxIOSettingsManaged::GetFbxTimeProp(String^ name, FbxTime^ defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				KTime kt = _Ref()->GetKTimeProp(n,*defValue->_Ref());
				FREECHARPOINTER(n);	
				FbxTime^ time = gcnew FbxTime(kt);				
				return time;
			}
			void FbxIOSettingsManaged::SetFbxTimeProp(String^ name, FbxTime^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetKTimeProp(n,*value->_Ref());
				FREECHARPOINTER(n);					
			}

			FbxStringManaged^ FbxIOSettingsManaged::GetEnumProp(String^ name, FbxStringManaged^ defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				FbxString ks = _Ref()->GetEnumProp(n,*defValue->_Ref());
				FREECHARPOINTER(n);	
				FbxStringManaged^ st = gcnew FbxStringManaged(ks);				
				return st;
			}
			int FbxIOSettingsManaged::GetEnumProp(String^ name, int defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				int i = _Ref()->GetEnumProp(n,defValue);
				FREECHARPOINTER(n);	
				return i;
			}
			int FbxIOSettingsManaged::GetEnumIndex(String^ name, FbxStringManaged^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				int i = _Ref()->GetEnumIndex(n,*value->_Ref());
				FREECHARPOINTER(n);	
				return i;
			}
			void FbxIOSettingsManaged::SetEnumProp(String^ name, FbxStringManaged^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetEnumProp(n,*value->_Ref());
				FREECHARPOINTER(n);					
			}
			void FbxIOSettingsManaged::SetEnumProp(String^ name, int value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetEnumProp(n,value);
				FREECHARPOINTER(n);					
			}
			void FbxIOSettingsManaged::RemoveEnumPropValue(String^ name, FbxStringManaged^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->RemoveEnumPropValue(n,*value->_Ref());
				FREECHARPOINTER(n);
			}
			void FbxIOSettingsManaged::EmptyEnumProp(String^ name)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->EmptyEnumProp(n);
				FREECHARPOINTER(n);
			}
			bool FbxIOSettingsManaged::SetFlag(String^ name, Skill::FbxSDK::FbxPropertyFlags::FbxPropertyFlagsType propFlag, bool value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				bool b =_Ref()->SetFlag(n,(fbxsdk_200901::FbxPropertyFlags::eFbxPropertyFlags)propFlag,value);
				FREECHARPOINTER(n);
				return b;
			}
			FbxStringManaged^ FbxIOSettingsManaged::GetStringProp(String^ name, FbxStringManaged^ defValue)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				FbxString ks = _Ref()->GetStringProp(n,*defValue->_Ref());
				FREECHARPOINTER(n);	
				FbxStringManaged^ st = gcnew FbxStringManaged(ks);				
				return st;
			}
			void FbxIOSettingsManaged::SetStringProp(String^ name, FbxStringManaged^ value)
			{
				STRINGTO_CONSTCHAR_ANSI(n,name);
				_Ref()->SetStringProp(n,*value->_Ref());
				FREECHARPOINTER(n);
			}
			bool FbxIOSettingsManaged::ReadXMLFile(String^ path)
			{
				STRINGTO_CONSTCHAR_ANSI(p,path);
				FbxString s(p);
				bool b = _Ref()->ReadXMLFile(s);
				FREECHARPOINTER(p);
				return b;
			}			
			bool FbxIOSettingsManaged::WriteXMLFile(String^ path)
			{
				STRINGTO_CONSTCHAR_ANSI(p,path);
				FbxString s(p);
				bool b = _Ref()->WriteXMLFile(s);
				FREECHARPOINTER(p);
				return b;
			}
			bool FbxIOSettingsManaged::WriteXmlPropToFile(String^ fullPath, String^ propPath)
			{
				STRINGTO_CONSTCHAR_ANSI(f,fullPath);
				FbxString sfull(f);
				STRINGTO_CONSTCHAR_ANSI(p,propPath);
				FbxString sprop(p);
				bool b = _Ref()->WriteXmlPropToFile(sfull,sprop);
				FREECHARPOINTER(f);
				FREECHARPOINTER(p);
				return b;
			}
			bool FbxIOSettingsManaged::WriteXmlPropToRegistry(String^ regKeyName, String^ regValueName, String^ propPath, String^ regPath)
			{
				STRINGTO_CONSTCHAR_ANSI(rk,regKeyName);
				FbxString srk(rk);
				STRINGTO_CONSTCHAR_ANSI(rv,regValueName);
				FbxString srv(rv);
				STRINGTO_CONSTCHAR_ANSI(pp,propPath);
				FbxString spp(pp);
				STRINGTO_CONSTCHAR_ANSI(rp,regPath);
				FbxString srp(rp);
				bool b = _Ref()->WriteXmlPropToRegistry(srk,srv,spp,srp);
				FREECHARPOINTER(rk);
				FREECHARPOINTER(rv);
				FREECHARPOINTER(pp);
				FREECHARPOINTER(rp);
				return b;
			}
			bool FbxIOSettingsManaged::ReadXmlPropFromRegistry(String^ regKeyName, String^ regValueName, String^ regPath)
			{
				STRINGTO_CONSTCHAR_ANSI(rk,regKeyName);
				FbxString srk(rk);
				STRINGTO_CONSTCHAR_ANSI(rv,regValueName);
				FbxString srv(rv);				
				STRINGTO_CONSTCHAR_ANSI(rp,regPath);
				FbxString srp(rp);
				bool b = _Ref()->ReadXmlPropFromRegistry(srk,srv,srp);
				FREECHARPOINTER(rk);
				FREECHARPOINTER(rv);				
				FREECHARPOINTER(rp);
				return b;
			}
			bool FbxIOSettingsManaged::ReadXmlPropFromMyDocument(String^ subDir, String^ filename)
			{
				STRINGTO_CONSTCHAR_ANSI(sd,subDir);
				FbxString ssd(sd);
				STRINGTO_CONSTCHAR_ANSI(fn,filename);
				FbxString sfn(fn);								
				bool b = _Ref()->ReadXmlPropFromMyDocument(ssd,sfn);
				FREECHARPOINTER(sd);
				FREECHARPOINTER(fn);				
				return b;
			}
			bool FbxIOSettingsManaged::WriteXmlPropToMyDocument(String^ subDir, String^ filename, String^ propPath)
			{				
				STRINGTO_CONSTCHAR_ANSI(sd,subDir);
				FbxString ssd(sd);
				STRINGTO_CONSTCHAR_ANSI(fn,filename);
				FbxString sfn(fn);
				STRINGTO_CONSTCHAR_ANSI(pp,propPath);
				FbxString spp(pp);
				bool b = _Ref()->WriteXmlPropToMyDocument(ssd,sfn,spp);
				FREECHARPOINTER(sd);
				FREECHARPOINTER(fn);				
				FREECHARPOINTER(pp);				
				return b;
			}
			String^ FbxIOSettingsManaged::UserMyDocumentDir::get()
			{
				FbxString kstr = KFbxIOSettings::GetUserMyDocumentDir();
				CONVERT_FbxString_TO_STRING(kstr,str);
				return str;
			}

			REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxIOSettingsManaged,impInfo,FbxSoInfoManaged,ImpInfo);
			REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxIOSettingsManaged,expInfo,FbxSoInfoManaged,ExpInfo);
			
			FbxUILanguage FbxIOSettingsManaged::UILanguage::get()
			{
				return (FbxUILanguage)_Ref()->UILanguage;
			}
			void FbxIOSettingsManaged::UILanguage::set(FbxUILanguage value)
			{
				_Ref()->UILanguage = (FBXUILANGUAGE)value;
			}
			String^ FbxIOSettingsManaged::GetLanguageLabel(FbxPropertyManaged^ prop)
			{
				FbxString kstr = _Ref()->GetLanguageLabel(*prop->_Ref());
				CONVERT_FbxString_TO_STRING(kstr,str);
				return str;
			}
			void FbxIOSettingsManaged::SetLanguageLabel(FbxPropertyManaged^ prop, String^ label)
			{				
				STRINGTO_CONSTCHAR_ANSI(l,label);
				FbxString kstr(l);
				_Ref()->SetLanguageLabel(*prop->_Ref(),kstr);
			}
			FbxUILanguage FbxIOSettingsManaged::GetMaxRuntimeLanguage(String^ regLocation)
			{
				STRINGTO_CONSTCHAR_ANSI(rl,regLocation);
				FbxString kstr(rl);
				return (FbxUILanguage)_Ref()->Get_Max_Runtime_Language(kstr);
			}
			bool FbxIOSettingsManaged::IsEnumExist(FbxPropertyManaged^ prop, String^ enumString)
			{
				STRINGTO_CONSTCHAR_ANSI(es,enumString);
				FbxString kstr(es);
				return _Ref()->IsEnumExist(*prop->_Ref(),kstr);
			}
			int  FbxIOSettingsManaged::GetEnumIndex(FbxPropertyManaged^ prop, String^ enumString, bool noCase)
			{
				STRINGTO_CONSTCHAR_ANSI(es,enumString);
				FbxString kstr(es);
				return _Ref()->GetEnumIndex(*prop->_Ref(),kstr,noCase);
			}			
			CLONE_DEFINITION(FbxIOSettingsManaged,KFbxIOSettings);
		}
	}
}