#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{		
		ref class FbxPropertyManaged;
		ref class FbxStringManaged;
		ref class FbxDocumentManaged;
		/** This class manages external references to files.
		*/
		public ref class FbxXRefManager : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxXRefManager,KFbxXRefManager);
			INATIVEPOINTER_DECLARE(FbxXRefManager,KFbxXRefManager);

			public:
				DEFAULT_CONSTRUCTOR(FbxXRefManager,KFbxXRefManager);



				//! This project represents a Url for storing temporary files.
				//static const char* sTemporaryFileProject;

				//! This project represents a Url for configuration files.
				//static const char* sConfigurationProject;

				//! This project represents a url for storing localization files (not part of the asset lib)
				//static const char* sLocalizationProject;

				/** This project is used for creating the .fbm folders used by
				*   embedded ressources in .fbx files.
				*  
				*   When not set, or if the folder is not writable, the .fbm
				*   folder is created alongside the .fbx file.
				*  
				*   If we cannot write in that folder, we look at the
				*   sTemporaryFileProject location.  If that's not set, or not
				*   writable, we use the operating system's temp folder
				*   location.
				*/
				//static const char* sEmbeddedFileProject;
				//@}

			public:

				/**
				* \name XRef Url properties
				*/
				//@{
				/** Get the number of Urls stored in a property
				* \return The Url Count
				*/
				static int GetUrlCount(FbxPropertyManaged^ p);
				static int GetUrlCount(FbxStringManaged^ url);
				static int GetUrlCount(String^ url);

				/** Return The nth Relative Url stored in the property
				* upon return the pXRefProject will return the name of the XRef Project closest to the Url of the property
				* \return The Url if it is valid relative
				*/
				static bool IsRelativeUrl(FbxPropertyManaged^ p,int index);

				/** Return The nth Url stored in the property
				* \return The Url
				*/
				static String^ GetUrl(FbxPropertyManaged^ p,int index);
				static void GetUrl(FbxPropertyManaged^ p,int index,FbxStringManaged^ urlOut);

				/** Return The nth Relative Url stored in the property
				* upon return the pXRefProject will return the name of the XRef Project closest to the Url of the property
				* \return The Url if it is valid relative
				*/
				bool GetResolvedUrl(FbxPropertyManaged^ p ,int index,FbxStringManaged^ resolvedPath);
				bool GetResolvedUrl(FbxPropertyManaged^ p ,int index,String^ resolvedPath);
				//@}


				bool GetResolvedUrl(String^ url,FbxDocumentManaged^ doc, String^ resolvedPath);

				/**
				* Looks for the first file matching a specified "pattern",
				* which is built as:
				*
				* if pOptExt is given:         prefix*.ext
				* If pOptExt is NULL:          prefix*
				* if pOptExt is "" or ".":     prefix*.
				*
				* Returns the URL of the first matching pattern.  Cannot be
				* used to resolve folders, only files.
				*
				* If a document is given, we start by looking at the document's
				* fbm folder.
				*/
				bool GetFirstMatchingUrl(String^ prefix, String^ optExt,FbxDocumentManaged^ doc,String^ resolvedPath);

				/**
				* \name XRef Resolve Url and Projects
				*/
				//@{

				/** Add XRef Projects.
				* Note that only one Url is associated with a project. Calling 
				* this on an existing project will replace the project's existing Url.
				* \param pName The name of the project
				* \param pUrl The Url to associate with the project
				* \return true on success, false otherwise.
				*/
				bool AddXRefProject   (String^ name,String^ url);
				bool AddXRefProject   (String^ name,String^ extension,String^ url);
				/** 
				* Add an XRef project based on a document's EmbeddedUrl 
				* property, if set, otherwise based on its current Url. 
				* \param Document to use to name the project, and to specify 
				*        the Url.
				* \return true on success, false otherwise.
				*/
				bool AddXRefProject(FbxDocumentManaged^ doc);
				bool RemoveXRefProject(String^ name);
				bool RemoveAllXRefProjects();

				VALUE_PROPERTY_GET_DECLARE(int,XRefProjectCount);
				String^ GetXRefProjectName(int index);

				/** Retrieve the base Url for the given project.
				* \param pName The name of the project
				* \return The base Url of the project or NULL if the Project is not found.
				*/
				String^ GetXRefProjectUrl(String^ name);   // FIXME: Should be const, will break AV.				
				String^ GetXRefProjectUrl(int index);

				//! Check if a project with the given name is defined in this manager.
				//bool HasXRefProject(String^ name);

				/** Return Try to resolve an Relative Url
				* \return true if the Url is resolved
				*/
				bool GetResolvedUrl(String^ url,String^ resolvePath);				
		};

	}	
}