#pragma once
#include "stdafx.h"
#include "FbxLibrary.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxObject.h"
#include "FbxQuery.h"



{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxLibrary,KFbxLibrary);

		void FbxLibrary::CollectManagedMemory()
		{
			_ParentLibrary = nullptr;			
			FbxDocumentManaged::CollectManagedMemory();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLibrary,KFbxLibrary,GetParentLibrary(),FbxLibrary,ParentLibrary);

		bool FbxLibrary::IsSystemLibrary::get()
		{
			return _Ref()->IsSystemLibrary();
		}
		void FbxLibrary::IsSystemLibrary::set(bool value)
		{
			_Ref()->SystemLibrary(value);
		}

		String^ FbxLibrary::LocalizationBaseNamePrefix::get()
		{
			FbxString kstr = _Ref()->LocalizationBaseNamePrefix();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;

		}
		void FbxLibrary::LocalizationBaseNamePrefix::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);			
			_Ref()->LocalizationBaseNamePrefix(v);
			FREECHARPOINTER(v);			
		}
		bool FbxLibrary::AddSubLibrary(FbxLibrary^ subLibrary)
		{
			return _Ref()->AddSubLibrary(subLibrary->_Ref());
		}
		bool FbxLibrary::RemoveSubLibrary(FbxLibrary^ subLibrary)
		{
			return _Ref()->RemoveSubLibrary(subLibrary->_Ref());
		}

		int FbxLibrary::SubLibraryCount::get()
		{
			return _Ref()->GetSubLibraryCount();
		}

		FbxLibrary^ FbxLibrary::GetSubLibrary(int index)
		{
			return gcnew FbxLibrary(_Ref()->GetSubLibrary(index));
		}
		FbxObjectManaged^ FbxLibrary::CloneAsset(FbxObjectManaged^ toClone, FbxObjectManaged^ optionalDestinationContainer)
		{
			KFbxObject* obj = nullptr;
			if(optionalDestinationContainer)
				obj = _Ref()->CloneAsset(toClone->_Ref(),optionalDestinationContainer->_Ref());
			else
				obj = _Ref()->CloneAsset(toClone->_Ref());
			return FbxCreator::CreateFbxObject(obj);
		}
		FbxCriteria^ FbxLibrary::GetAssetCriteriaFilter()
		{
			return gcnew FbxCriteria(&KFbxLibrary::GetAssetCriteriaFilter());
		}
		FbxCriteria^ FbxLibrary::GetAssetDependentsFilter()
		{
			return gcnew FbxCriteria(&KFbxLibrary::GetAssetDependentsFilter());
		}

		bool FbxLibrary::ImportAssets(FbxLibrary^ srcLibrary)		
		{
			return _Ref()->ImportAssets(srcLibrary->_Ref());
		}
		bool FbxLibrary::ImportAssets(FbxLibrary^ srcLibrary, FbxCriteria^ criteria)
		{
			return _Ref()->ImportAssets(srcLibrary->_Ref(),*criteria->_Ref());
		}
		/*String^ FbxLibrary::Localize(String^ ID, String^ Default)
		{
			STRINGTO_CONSTCHAR_ANSI(id,ID);
			STRINGTO_CONSTCHAR_ANSI(d,Default);
			String^ str =  gcnew String(_Ref()->Localize(id,d));
			FREECHARPOINTER(id);
			FREECHARPOINTER(d);
			return str;
		}*/
		bool FbxLibrary::AddShadingObject(FbxObjectManaged^ shadingObject)
		{
			return _Ref()->AddShadingObject(shadingObject->_Ref());
		}
		bool FbxLibrary::RemoveShadingObject(FbxObjectManaged^ shadingObject)
		{
			return _Ref()->RemoveShadingObject(shadingObject->_Ref());
		}
		int FbxLibrary::ShadingObjectCount::get()
		{
			return _Ref()->GetShadingObjectCount();
		}
		FbxObjectManaged^ FbxLibrary::GetShadingObject(int index)		
		{
			KFbxObject* obj = _Ref()->GetShadingObject(index);			
			return FbxCreator::CreateFbxObject(obj);			
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxLibrary,KFbxLibrary);		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}