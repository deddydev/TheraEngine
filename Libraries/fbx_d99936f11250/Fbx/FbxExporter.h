#pragma once
#include "stdafx.h"
#include "FbxIO.h"
#include "FbxRenamingStrategy.h"

namespace FbxSDK
{
	ref class FbxDocumentManaged;	
	ref class FbxStringManaged;
	ref class FbxSdkManagerManaged;
	ref class FbxClassId;
	namespace IO
	{	
		ref class FbxStreamOptionsManaged;
		public enum class FileFormat : int
		{
			FbxBinary = 0,
			FbxAscii = 1,
			FbxEncrypted = 2,
			Fbx5Binary = 3,
			Fbx5Ascii = 4,
			AutocadDXF = 5,
			StudioMax3DS = 6 ,
			AliasOBJ = 7,
			ColladaDAE = 8
		};			

		/** Class to export SDK objects into an FBX file.
		* Normally this class is used as is. But for very special needs
		* a user can override Initialize() for special purpose.
		*
		* An exporter will select the appropriate writer to a particular file.
		* Ex: When an exporter must export an FBX 7 file,
		* the exporter will ask for all registered writers if an FBX 7 file writer is available,
		* then if a writer is found, the exporter will create
		* the specialized FBX 7 writer and write the file.
		* This way, an exporter can "write" many different type of files like FBX 5/6/7, 3DS, Obj, Dxf, Collada, etc.
		* \see FbxWriter
		*
		* Typical workflow for using the FbxExporter class:
		* -# create a SDKManager
		* -# create an IOSettings object
		* -# create an empty scene
		* -# create an exporter
		* -# initialize it with a file name
		* -# set numerous options to control how the exporter will behave.\n
		*    ex: set IOSettings values to export Materials or Textures.
		* -# call FbxExporter::Export() with the entity to export.
		*
		* \code
		* // ex:
		* // create a SdkManager
		* FbxManager* lSdkManager = FbxManager::Create();
		*
		* // create an IOSettings object
		* FbxIOSettings* ios = FbxIOSettings::Create(lSdkManager, IOSROOT);
		*
		* // set some IOSettings options
		* ios->SetBoolProp(EXP_FBX_MATERIAL, true);
		* ios->SetBoolProp(EXP_FBX_TEXTURE,  true);
		*
		* // create an empty scene
		* FbxScene* lScene = FbxScene::Create(lSdkManager, "");
		*
		* // create an exporter.
		* FbxExporter* lExporter = FbxExporter::Create(lSdkManager, "");
		*
		* // initialize the exporter by providing a filename and the IOSettings to use
		* lExporter->Initialize("C:\\myfile.fbx", -1, ios);
		*
		* // export the scene.
		* lExporter->Export(lScene);
		*
		* // destroy the exporter
		* lExporter->Destroy();
		* \endcode
		*
		* \remarks According to the file suffix, a specialized writer will be created internally.\n
		* 		 Ex: for .fbx files a FBX Writer, for .3ds files, a 3ds writer, etc.\n
		*          Supported files formats: FBX 5/6/7 Binary & ASCII, Collada, DXF, OBJ, 3DS
		* \nosubgrouping
		*/
		public ref class FbxExporterManaged : FbxIOManaged
		{
			REF_DECLARE(FbxEmitter, FbxExporter);
		internal:
			FbxExporterManaged(FbxExporter* instance) : FbxIOManaged(instance)
			{
				_Free = false;
			}

			FBXOBJECT_DECLARE(FbxExporterManaged);								
				
		public:

			virtual void CollectManagedMemory() override;

			/** Initialize object.
			*	\param pFileName     Name of file to access.
			*	\return              \c true on success, \c false otherwise.
			* \remarks             To identify the error that occured, call KFbxIO::GetLastErrorID().
			*/
			virtual bool Initialize(String^ fileName) override;


			/** Get file export options settings.
			*	\return     Pointer to file export options or NULL on failure.
			* \remarks    Caller gets ownership of the returned structure.
			*/				
			REF_PROPERTY_GET_DECLARE(FbxStreamOptionsManaged,ExportOptions);				

			/** Export the document to the currently created file.
			* \param pDocument          Document to export.
			* \param pStreamOptions     Pointer to file export options.
			*	\return                   \c true on success, \c false otherwise.
			* \remarks                  To identify the error, call KFbxIO::GetLastErrorID().
			*/
			bool Export(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions);
			bool Export(FbxDocumentManaged^ document);

			/** Release the file export options. 
			* \param pStreamOptions     Pointer to file export options.
			*/
			void ReleaseExportOptions(FbxStreamOptionsManaged^ streamOptions);

			//@}

			/** 
			* \name File Format
			*/
			//@{

				
			/** Get the format of the exported file.
			*	\return     File format identifier.
			*/
			/** Set the exported file format.
			*	\param pFileFormat     File format identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(int,FileFormat);				

			/** Return     \c true if the file format is a recognized FBX format.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsFBX);				

			/** Get writable version for the current file format.
			* \return     \c char**
			*/				
			//array<String^>^ GetCurrentWritableVersions();				

			/** Set file version for a given file format.
			* \param pVersion        String description of the file format.
			* \param pRenamingMode   Renaming mode.
			* \return                \c true if mode is set correctly
			*/
			bool SetFileExportVersion(FbxStringManaged^ version, FbxSceneRenamerManaged::RenamingMode renamingMode);

			/** Set the resampling rate (only used when exporting to FBX5.3 and lower)
			* \param     pResamplingRate resampling rate
			*/
			//void SetResamplingRate(double resamplingRate);								


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			/** Get file export option settings.
			* \param pFbxObject     Target FBX file.
			*	\return               Pointer to stream export options or NULL on failure.
			* \remarks              Caller gets ownership of the returned structure.
			*/	
			//FbxStreamOptions^ GetExportOptions(KFbx* pFbxObject);

			/** Export the document to a FBX file.
			* \param pDocument          Document to export.
			* \param pStreamOptions     Pointer to stream export options, not publicly available yet.
			* \param pFbxObject         Target FBX file.
			*	\return                   \c true on success, \c false otherwise.
			* \remarks                  To identify the error, call KFbxIO::GetLastErrorID().
			*/
			//bool Export(FbxDocument^ %document, FbxStreamOptions^ %streamOptions, KFbx* pFbxObject);			

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}