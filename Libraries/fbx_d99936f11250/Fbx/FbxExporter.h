#pragma once
#include "stdafx.h"
#include "FbxIO.h"
#include "FbxRenamingStrategy.h"

namespace FbxSDK
{
	//ref class FbxDocumentManaged;	
	//ref class FbxStringManaged;
	//ref class FbxSdkManagerManaged;
	//ref class FbxClassId;

	ref class FbxIOManaged;
	ref class FbxIOFileHeaderInfoManaged;
	ref class FbxThreadManaged;
	ref class FbxWriterManaged;

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
		public ref class FbxExporterManaged : FbxIOManaged
		{
			//REF_DECLARE(FbxEmitter, FbxExporter);
		private:
			FbxExporter* _FbxExporter;
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