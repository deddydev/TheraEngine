#pragma once
#include "stdafx.h"
#include "FbxSdkManager.h"
#include "FbxIOPluginRegistry.h"
#include "FbxMemoryAllocator.h"
#include "FbxError.h"
#include "FbxDataType.h"
#include "FbxXRefManager.h"
#include "FbxString.h"
#include "FbxLibrary.h"
#include "FbxScene.h"
#include "FbxIOSettings.h"

namespace Skill
{
	namespace FbxSDK
	{	
		void FbxSdkManagerManaged::CollectManagedMemory()
		{
			this->_IOPluginRegistry = nullptr;
			this->_SystemLibraries = nullptr;
			this->_UserLibraries = nullptr;
			this->_XRefManager = nullptr;
			this->_RootLibrary = nullptr;
			this->_KError = nullptr;			
		}		
		bool FbxSdkManagerManaged::SetMemoryAllocator(FbxMemoryAllocator^ memoryAllocator)
		{
			return KFbxSdkManager::SetMemoryAllocator(memoryAllocator->_Ref());
		}
		FbxSdkManagerManaged^ FbxSdkManagerManaged::Create()
		{
			KFbxSdkManager* km = KFbxSdkManager::Create();
			FbxSdkManagerManaged^ m = nullptr;
			if(km)
			{
				m = gcnew FbxSdkManagerManaged(km);
				m->_Free = true;
			}
			return m;
		}
		void FbxSdkManagerManaged::Destroy()
		{
			CollectManagedMemory();
			_Ref()->Destroy();
			_Free = false;
			_disposed = true;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSdkManagerManaged,GetError(),FbxErrorManaged,KError);

		FbxSdkManagerManaged::Error FbxSdkManagerManaged::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}		
		String^ FbxSdkManagerManaged::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}			
		FbxDataType^ FbxSdkManagerManaged::CreateFbxDataType(String^ name,FbxType type)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,name);
			FbxDataType^ b = gcnew FbxDataType(&_Ref()->CreateFbxDataType(n,(EFbxType)type));
			FREECHARPOINTER(n);
			return b;
		}
		VALUE_PROPERTY_GET_DEFINATION(FbxSdkManagerManaged,GetFbxDataTypeCount(),int,FbxDataTypeCount);				
		FbxDataType^ FbxSdkManagerManaged::GetFbxDataType(int index)
		{
			return gcnew FbxDataType(&_Ref()->GetFbxDataType(index));
		}


		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSdkManagerManaged,KFbxLibrary,GetRootLibrary(),FbxLibrary,RootLibrary);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSdkManagerManaged,KFbxLibrary,GetSystemLibraries(),FbxLibrary,SystemLibraries);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSdkManagerManaged,KFbxLibrary,GetUserLibraries(),FbxLibrary,UserLibraries);		

		bool FbxSdkManagerManaged::SetLocale(String^ locale)
		{			
			STRINGTO_CONSTCHAR_ANSI(l,locale);
			bool b = _Ref()->SetLocale(l);
			FREECHARPOINTER(l);
			return b;
		}
		String^ FbxSdkManagerManaged::Localize(String^ ID, String^ Default)
		{			
			STRINGTO_CONSTCHAR_ANSI(id,ID);			
			STRINGTO_CONSTCHAR_ANSI(d,Default);

			String^ result = gcnew String(_Ref()->Localize(id,d));
			FREECHARPOINTER(id);
			FREECHARPOINTER(d);
			return result;

		}
		String^ FbxSdkManagerManaged::Localize(String^ ID)
		{			
			STRINGTO_CONSTCHAR_ANSI(id,ID);	
			String^  result = gcnew String(_Ref()->Localize(id));
			FREECHARPOINTER(id);
			return result;			
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSdkManagerManaged,GetXRefManager(),FbxXRefManager,XRefManager);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSdkManagerManaged,KFbxIOPluginRegistry,GetIOPluginRegistry(),FbxIOPluginRegistry,IOPluginRegistry);
		bool FbxSdkManagerManaged::LoadPluginsDirectory (String^ filename,String^ extensions)
		{			
			STRINGTOCHAR_ANSI(f,filename);			
			STRINGTOCHAR_ANSI(e,extensions);
			bool b = _Ref()->LoadPluginsDirectory (f,e);
			FREECHARPOINTER(e);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxSdkManagerManaged::LoadPlugin (String^ filename)
		{			
			STRINGTOCHAR_ANSI(f,filename);
			bool b = _Ref()->LoadPlugin(f);
			FREECHARPOINTER(f);
			return b;
		}
		bool FbxSdkManagerManaged::UnloadPlugins()
		{
			return _Ref()->UnloadPlugins();
		}

		void FbxSdkManagerManaged::RegisterObject(FbxPlug^ plug)
		{
			_Ref()->RegisterObject(plug->_Ref());
		}
		void FbxSdkManagerManaged::UnregisterObject(FbxPlug^ plug)
		{
			_Ref()->UnregisterObject(plug->_Ref());
		}

		void FbxSdkManagerManaged::FillIOSettingsForReadersRegistered(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->FillIOSettingsForReadersRegistered(*ios->_Ref());
		}

		void FbxSdkManagerManaged::FillIOSettingsForWritersRegistered(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->FillIOSettingsForWritersRegistered(*ios->_Ref());
		}

		void FbxSdkManagerManaged::FillCommonIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios, bool import)
		{
			_Ref()->FillCommonIOSettings(*ios->_Ref(),import);
		}

		void FbxSdkManagerManaged::Create_Common_Import_IOSettings_Groups(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Create_Common_Import_IOSettings_Groups(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Create_Common_Export_IOSettings_Groups(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Create_Common_Export_IOSettings_Groups(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Add_Common_Import_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Add_Common_Import_IOSettings(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Add_Common_Export_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Add_Common_Export_IOSettings(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Add_Common_RW_Import_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Add_Common_RW_Import_IOSettings(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Add_Common_RW_Export_IOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Add_Common_RW_Export_IOSettings(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Fill_Motion_Base_ReaderIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Fill_Motion_Base_ReaderIOSettings(*ios->_Ref());
		}

		void FbxSdkManagerManaged::Fill_Motion_Base_WriterIOSettings(Skill::FbxSDK::IO::FbxIOSettingsManaged^ ios)
		{
			_Ref()->Fill_Motion_Base_WriterIOSettings(*ios->_Ref());
		}

		String^ FbxSdkManagerManaged::PrefixName(String^ prefix,String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(pr,prefix);
			STRINGTO_CONSTCHAR_ANSI(n,name);

			FbxString kstr = KFbxSdkManager::PrefixName(pr,n);

			FREECHARPOINTER(pr);
			FREECHARPOINTER(n);

			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

			#ifndef DOXYGEN_SHOULD_SKIP_THIS
			  
				  bool FbxSdkManagerManaged::CanDestroyFbxSrcObject(FbxObjectManaged^ obj, FbxObjectManaged^ srcObj, bool recursive, bool dependents)
				  {
					  return _Ref()->CanDestroyFbxSrcObject(obj->_Ref(), srcObj->_Ref(),recursive,dependents);
				  }

				  void FbxSdkManagerManaged::CreateMissingBindPoses(FbxSceneManaged^ scene)
				  {
					  _Ref()->CreateMissingBindPoses(scene->_Ref());
				  }
				  int  FbxSdkManagerManaged::GetBindPoseCount(FbxSceneManaged^ scene)
				  {
					  return _Ref()->GetBindPoseCount(scene->_Ref());
				  }

				  //FbxPlug^ CreateClassFrom(FbxClassId^ , const char *pName, const KFbxObject* pFrom, const char* pFBXType=0, const char* pFBXSubType=0);  

			  #endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}