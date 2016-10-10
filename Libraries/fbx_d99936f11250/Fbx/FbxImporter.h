#pragma once
#include "stdafx.h"
#include "FbxIO.h"


{
	namespace FbxSDK
	{
		ref class FbxDocumentManaged;
		ref class FbxDocumentInfo;
		ref class FbxAxisSystem;
		ref class FbxSystemUnit;
		ref class FbxSystemUnit;
		ref class FbxSdkManagerManaged;
		ref class FbxTakeInfo;
		ref class fbxString;
		ref class FbxClassId;

		namespace IO
		{			

			ref class FbxStreamOptionsManaged;
			/** \brief Class to import an FBX file into SDK objects.
			* \nosubgrouping
			*	Typical workflow for using the KFbxImporter class:
			*		-# create an importer
			*		-# initialize the importer with a file name
			*	    -# set numerous states, take information, defining how the importer will behave
			*		-# call KFbxImporter::Import() with an empty scene
			*/
			public ref class FbxImporterManaged : FbxIOManaged
			{
			internal:
				FbxImporterManaged(FbxImporter* instance) : FbxIOManaged(instance)
				{
					_Free = false;
				}			
				REF_DECLARE(FbxEmitter,FbxImporter);
				FBXOBJECT_DECLARE(FbxImporterManaged);
			protected:
				FbxStreamOptionsManaged^ _ImportOptions;
				virtual void CollectManagedMemory() override;
			public:				

				
				/** 
				* \name Import Functions
				*/
				//@{

				/** Initialize object.
				*	\param pFileName     Name of file to access.
				*	\return              \c true on success, \c false otherwise.
				* \remarks             To identify the error that occurred, call KFbxIO::GetLastErrorID().	  
				*/
				virtual bool Initialize(String^ fileName) override;

				/** Initialize object.
				*	\param pFile        file to access. Ownership is transfered to this object.
				*	\return              \c true on success, \c false otherwise.
				* \remarks             To identify the error that occurred, call KFbxIO::GetLastErrorID().	  
				*/
				//virtual bool Initialize(KFile * pFile);
				//
				//#ifdef KARCH_DEV_MACOSX_CFM
				//				virtual bool Initialize(const FSSpec &pMacFileSpec);
				//				virtual bool Initialize(const FSRef &pMacFileRef);
				//				virtual bool Initialize(const CFURLRef &pMacURL);
				//#endif

				/** Get the file, if any.
				* \return     File or an null if the file has not been set.
				*/
				//virtual KFile * GetFile();

				/** Get the FBX version number of the FBX file.
				* FBX version numbers start at 5.0.0.
				* \param pMajor        Version major number.
				* \param pMinor        Version minor number.
				* \param pRevision     Version revision number.
				*	\remarks             This function must be called after KFbxImporter::Initialize().	  
				*/
				property Version^ FileVersion
				{
					Version^ get();
				}

				/**	Get the default rendering resolution if present in the file header.
				* \param pCamName            Returned name of the camera.
				* \param pResolutionMode     Returned resolution mode.
				* \param pW                  Returned width.
				* \param pH                  Returned height.
				* \return                    \c true if the default rendering settings are defined in the file, otherwise
				*                            returns \c false with empty parameters.
				*/
				bool GetDefaultRenderResolution(FbxStringManaged^ camName, FbxStringManaged^ resolutionMode, double^ %w, double^ %h);

				/**	Get the complete file header information.
				* \return		valid pointer to the complete header information
				*/
				//KFbxFileHeaderInfo* GetFileHeaderInfo();

				/** \enum EStreamOptionsGeneration Stream options identifiers.
				* - \e eSTREAMOPTIONS_PARSE_FILE
				* - \e eSTREAMOPTIONS_FAST		Do not parse the file.
				* - \e eSTREAMOPTIONS_COUNT
				*/
				enum class StreamOptionsGeneration
				{
					StreamoptionsParseFile,
					StreamoptionsFast,        // Do not parse the file
					StreamoptionsCount
				};

				/** Read the currently opened file header to retrieve information related to takes.
				* \param pStreamOptionsGeneration     Stream options identifier.
				*	\return                             Pointer to file import options or \c NULL on failure.
				* \remarks                            Caller gets ownership of the returned structure.
				*/			

				FbxStreamOptionsManaged^ GetImportOptions(StreamOptionsGeneration streamOptionsGeneration);
				FbxStreamOptionsManaged^ GetImportOptions();

				/** Read the currently opened file header to retrieve information related to takes.
				* \param pFbxObject     Target FBX file.
				*	\return               Pointer to stream import options or \c NULL on failure.
				* \remarks              Caller gets ownership of the returned structure.
				*/
				//KFbxStreamOptions* GetImportOptions(KFbx* pFbxObject);

				/** Import the currently opened file into a scene. 
				* \param pDocument       Document to fill with file content.
				* \param pStreamOptions  Pointer to file import options.
				*	\return                \c true on success, \c false otherwise.
				* \remarks               To identify the error that occurred, call KFbxIO::GetLastErrorID().
				*                        If the imported file is password protected and the password is not
				*                        set or wrong, function KFbxIO::GetLastErrorID() returns 
				*                        KFbxIO::ePASSWORD_ERROR.
				*/
				bool Import(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions);
				bool Import(FbxDocumentManaged^ document);

				/** Import the FBX file into a scene. 
				* \param pDocument	       Document to fill with file content.
				* \param pStreamOptions      Pointer to file import options.
				* \param pFbxObject          Source FBX file.
				*	\return                    \c true on success, \c false otherwise.
				* \remarks                   To identify the error that occurred, call KFbxIO::GetLastErrorID().
				*                            If the imported file is password protected and the password is not
				*                            set or wrong, function KFbxIO::GetLastErrorID() returns 
				*                            KFbxIO::ePASSWORD_ERROR.
				*/
				//bool Import(KFbxDocument* pDocument, KFbxStreamOptions* pStreamOptions, KFbx* pFbxObject);

				/** Release the file import options. 
				* \param pStreamOptions     Pointer to file import options.
				*/
				void ReleaseImportOptions(FbxStreamOptionsManaged^ streamOptions);

				//@}

				/** Set the password.
				* All subsequently imported files are opened with the given password.
				* \param pPassword     Password string.
				*/
				/*void SetPassword(String^ password)
				{
				char* p = new char(FbxString::NumCharToCreateString);
				FbxString::StringToChar(password,p);
				((KFbxImporter*)emitter)->SetPassword(p);
				}*/

				/** 
				* \name Take Description Access
				*/
				//@{

				/** Get the number of available takes in the file.
				* \return      Number of takes.
				*	\remarks     This function must be called after KFbxImporter::Initialize().
				*/
				property int TakeCount
				{
					int get();
				}

				/** Get the take information about an available take.
				* Use the returned reference to a KFbxTakeInfo object to set whether the indexed take is imported.
				*	\param pIndex     Index of the requested take.
				*	\return           Take information or \c NULL if function failed.
				*	\remarks          This function must be called after KFbxImporter::Initialize().
				*/
				FbxTakeInfo^ GetTakeInfo(int index);

				/** Return the current take name.
				*	\return     Current take name if there is one, otherwise returns an empty string.
				*	\remarks    This function must be called after KFbxImporter::Initialize().
				*/
				property String^ CurrentTakeName
				{
					String^ get();
				}

				//@}

				/** 
				* \name Scene Description Access
				*/
				//@{

				/** Get the scene info.
				* \return     Pointer to the scene info or \c NULL if no scene information
				*             is available in the file.
				*/
				REF_PROPERTY_GET_DECLARE(FbxDocumentInfo,SceneInfo);				

				//@}
				/** 
				* \name File Format
				*/
				//@{

				/** Get the format of the imported file.
				*	\return     File format identifier.
				*/
				/** Set the imported file format.
				*	\param pFileFormat     File format identifier.
				*/
				VALUE_PROPERTY_GETSET_DECLARE(int,FileFormat);								

				/** \return     \c true if the file format is a recognized FBX format.
				*/
				VALUE_PROPERTY_GET_DECLARE(bool,IsFBX);				
				//@}

				///////////////////////////////////////////////////////////////////////////////
				//
				//  WARNING!
				//
				//	Anything beyond these lines may not be documented accurately and is 
				// 	subject to change without notice.
				//
				///////////////////////////////////////////////////////////////////////////////

				void ParseForGlobalSettings(bool state);
				bool GetAxisInfo(FbxAxisSystem^ axisSystem, FbxSystemUnit^ systemUnits);

				void ParseForStatistics(bool state);
				//bool GetStatistics(FbxStatistics* pStatistics);

#ifndef DOXYGEN_SHOULD_SKIP_THIS
				bool UpdateImportOptions(FbxStreamOptionsManaged^ streamOptions);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
			};

		}
	}
}