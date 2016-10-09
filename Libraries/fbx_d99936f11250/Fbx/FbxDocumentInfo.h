#pragma once
#include "stdafx.h"
#include "FbxObject.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxProperty;
		ref class FbxStringManaged;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxStringTypedProperty;
		ref class FbxThumbnail;
		/** Contains scene thumbnail and user-defined summary data.
		*/
		public ref class FbxDocumentInfo : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxDocumentInfo);
		internal:
			FbxDocumentInfo(KFbxDocumentInfo* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}

			//	/**
			//	* \name Public properties
			//	*/
			//	//@{
			FBXOBJECT_DECLARE(FbxDocumentInfo);
		protected:
			virtual void CollectManagedMemory()override;
		public:			

			/*REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,LastSavedUrl);
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,Url);	*/		

			//! Parent property for all 'creation-related' properties; these properties
			// should be set once, when the file is created, and should be left alone
			// on subsequent save/reload operations.
			// 
			// Below are the default properties, but application vendors can add new
			// properties under this parent property.
			REF_PROPERTY_GETSET_DECLARE(FbxProperty,Original);


			// "CompanyName"
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,Original_ApplicationVendor);
			// "UberGizmo"
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,Original_ApplicationName);
			// "2009.10"
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,Original_ApplicationVersion);
			// "foo.bar"
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,Original_FileName);
			//
			//				//! Date/time should be stored in GMT.
			//				KFbxTypedProperty<fbxDateTime>  Original_DateTime_GMT;
			//
			//				//! Parent property for all 'lastmod-related' properties; these properties
			//				// should be updated everytime a file is saved.
			//				// 
			//				// Below are the default properties, but application vendors can add new
			//				// properties under this parent property.
			//				// 
			//				// It is up to the file creator to set both the 'Original' and
			//				// 'Last Saved' properties.	

			REF_PROPERTY_GETSET_DECLARE(FbxProperty,LastSaved);



			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,LastSaved_ApplicationVendor);

			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,LastSaved_ApplicationName);

			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,LastSaved_ApplicationVersion);

			//! Date/time should be stored in GMT.
			//KFbxTypedProperty<fbxDateTime>  LastSaved_DateTime_GMT;

			/**
			* This property is set to point to the .fbm folder created when 
			* reading a .fbx file with embedded data.  It is not saved in 
			* the .fbx file. 
			*/
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,EmbeddedUrl);			
			//@}


			/** User-defined summary data.
			* These fields are filled by the user to identify/classify
			* the files.
			*/
			//@{
		public:
			//! Title.
			VALUE_PROPERTY_GETSET_DECLARE(String^,Title)

				//
				//			//! Subject.		
				VALUE_PROPERTY_GETSET_DECLARE(String^,Subject)
				//			property FbxString^ Subject
				//			{
				//				FbxString^ get();
				//				void set(FbxString^ value);
				//			}
				//
				//			//! Author				
				VALUE_PROPERTY_GETSET_DECLARE(String^,Author)
				//			property FbxString^ Author
				//			{
				//				FbxString^ get();
				//				void set(FbxString^ value);
				//			}
				//
				//			//! Keywords.	
				VALUE_PROPERTY_GETSET_DECLARE(String^ ,Keywords)
				//			property FbxString^ Keywords
				//			{
				//				FbxString^ get();
				//				void set(FbxString^ value);
				//			}
				//
				//			//! Revision.	
				VALUE_PROPERTY_GETSET_DECLARE(String^,Revision)
				//			property FbxString^ Revision
				//			{
				//				FbxString^ get();
				//				void set(FbxString^ value);
				//			}
				//
				//			//! Comment.	
				VALUE_PROPERTY_GETSET_DECLARE(String^,Comment)
				//			property FbxString^ Comment
				//			{
				//				FbxString^ get();
				//				void set(FbxString^ value);
				//			}
				//
				//			/** Scene Thumbnail.
				//			*/
				//			//@{
				//
				REF_PROPERTY_GETSET_DECLARE(FbxThumbnail,SceneThumbnail)

				/** Clear the content.
				* Reset all the strings to the empty string and clears 
				* the pointer to the thumbnail.
				*/
				void Clear();

			//! assignment operator.
			//KFbxDocumentInfo& operator=(const KFbxDocumentInfo& pDocumentInfo);
			void CopyFrom(FbxDocumentInfo^ info);
			//
			//			///////////////////////////////////////////////////////////////////////////////
			//			//
			//			//  WARNING!
			//			//
			//			//	Anything beyond these lines may not be documented accurately and is 
			//			// 	subject to change without notice.
			//			//
			//			///////////////////////////////////////////////////////////////////////////////
#ifndef DOXYGEN_SHOULD_SKIP_THIS

			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
		};

	}
}