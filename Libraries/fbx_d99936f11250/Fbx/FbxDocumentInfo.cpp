#pragma once
#include "stdafx.h"
#include "FbxDocumentInfo.h"
#include "FbxProperty.h"
#include "FbxString.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxThumbnail.h"
#include "FbxTypedProperty.h"

namespace Skill
{
	namespace FbxSDK
	{					
		void FbxDocumentInfo::CollectManagedMemory()
		{
			_Original = nullptr;
			_Original_ApplicationVendor = nullptr;
			_Original_ApplicationName = nullptr;
			_Original_ApplicationVersion = nullptr;
			_Original_FileName = nullptr;
			_LastSaved = nullptr;
			_LastSaved_ApplicationVendor = nullptr;
			_LastSaved_ApplicationName = nullptr;
			_LastSaved_ApplicationVersion = nullptr;
			_EmbeddedUrl = nullptr;
			_SceneThumbnail = nullptr;
			
			FbxObjectManaged::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxDocumentInfo,KFbxDocumentInfo);

		/*REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,LastSavedUrl,FbxStringTypedProperty,LastSavedUrl);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Url,FbxStringTypedProperty,Url);	*/	


		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Original,FbxProperty,Original);

		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Original_ApplicationVendor,FbxStringTypedProperty,Original_ApplicationVendor);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Original_ApplicationName,FbxStringTypedProperty,Original_ApplicationName);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Original_ApplicationVersion,FbxStringTypedProperty,Original_ApplicationVersion);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,Original_FileName,FbxStringTypedProperty,Original_FileName);		
		

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxDocumentInfo,LastSaved,FbxProperty,LastSaved);		
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,LastSaved_ApplicationVendor,FbxStringTypedProperty,LastSaved_ApplicationVendor);	
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,LastSaved_ApplicationName,FbxStringTypedProperty,LastSaved_ApplicationName);	
		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,LastSaved_ApplicationVersion,FbxStringTypedProperty,LastSaved_ApplicationVersion);			

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxDocumentInfo,EmbeddedUrl,FbxStringTypedProperty,EmbeddedUrl);		
		
		String^ FbxDocumentInfo::Title::get()
		{
			return gcnew String(_Ref()->mTitle.Buffer());
		}
		void FbxDocumentInfo::Title::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mTitle = FbxString(n);
			FREECHARPOINTER(n);
		}

		String^ FbxDocumentInfo::Subject::get()
		{
			return gcnew String(_Ref()->mSubject.Buffer());
		}
		void FbxDocumentInfo::Subject::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mSubject = FbxString(n);
			FREECHARPOINTER(n);
		}							

		String^ FbxDocumentInfo::Author::get()
		{
			return gcnew String(_Ref()->mAuthor.Buffer());
		}
		void FbxDocumentInfo::Author::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mAuthor = FbxString(n);
			FREECHARPOINTER(n);
		}		
		
		String^ FbxDocumentInfo::Keywords::get()
		{
			return gcnew String(_Ref()->mKeywords.Buffer());
		}
		void FbxDocumentInfo::Keywords::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mKeywords = FbxString(n);
			FREECHARPOINTER(n);
		}		
		
		String^ FbxDocumentInfo::Revision::get()
		{
			return gcnew String(_Ref()->mRevision.Buffer());
		}
		void FbxDocumentInfo::Revision::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mRevision = FbxString(n);
			FREECHARPOINTER(n);
		}			

		String^ FbxDocumentInfo::Comment::get()
		{
			return gcnew String(_Ref()->mComment.Buffer());
		}
		void FbxDocumentInfo::Comment::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(n,value);						
			_Ref()->mComment = FbxString(n);
			FREECHARPOINTER(n);
		}


		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxDocumentInfo,KFbxThumbnail,GetSceneThumbnail(),FbxThumbnail,SceneThumbnail);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxDocumentInfo,SetSceneThumbnail,FbxThumbnail,SceneThumbnail);



		void FbxDocumentInfo::Clear()
		{
			_Ref()->Clear();
		}
		void FbxDocumentInfo::CopyFrom(FbxDocumentInfo^ info)
		{
			*this->_Ref() = *info->_Ref();
		}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
		CLONE_DEFINITION(FbxDocumentInfo,KFbxDocumentInfo);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 			

	}
}