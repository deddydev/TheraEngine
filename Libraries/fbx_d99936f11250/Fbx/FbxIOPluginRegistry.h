#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNativePointer.h"



{
	namespace FbxSDK
	{
		namespace IO
		{
			ref class FbxReaderManaged;
			ref class FbxWriter;
			ref class FbxImporterManaged;
			ref class FbxExporterManaged;
		}
	}
}

using ::FbxSDK::IO;


{
	namespace FbxSDK
	{
		ref class FbxStringManaged;		
		ref class FbxSdkManagerManaged;
		/**	\brief This class serves as the registrar for file formats.
		* A file format must be registered when it is used by the FBX SDK.
		*
		* This class also lets you create and read formats other than FBX SDK native formats
		*/
		public ref class FbxIOPluginRegistry : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxIOPluginRegistry,KFbxIOPluginRegistry);
			INATIVEPOINTER_DECLARE(FbxIOPluginRegistry,KFbxIOPluginRegistry);			
			/** Constructor
			*/
			DEFAULT_CONSTRUCTOR(FbxIOPluginRegistry,KFbxIOPluginRegistry);			

			/** Registers Reader from a plug-in path
			*	\param pPluginPath          path of the plug-in
			* \param pFirstPluginID       contains the ID of the first plug-in found
			* \param pRegisteredCount     contains the number of registered Readers
			*/
			void RegisterReader(String^ pluginPath,
				int %firstPluginID,
				int %registeredCount);

			/** Registers Readers
			*	\param pCreateF             Provide function information on file format
			* \param pInfoF               Provide information about the file format
			* \param pFirstPluginID       Contains the ID of the first plug-in found
			* \param pRegisteredCount     Contains the number of registered Readers
			* \param pIOSettingsFillerF
			*/
			/*void RegisterReader(KFbxReader::CreateFuncType pCreateF, 
			KFbxReader::GetInfoFuncType pInfoF,
			int& pFirstPluginID,
			int& pRegisteredCount,
			KFbxReader::IOSettingsFillerFuncType pIOSettingsFillerF = NULL
			);*/


			/** Registers Writers from a plug-in path
			*	\param pPluginPath          Path of the plug-in
			* \param pFirstPluginID       Contains the ID of the first plug-in found
			* \param pRegisteredCount     Contains the number of registered Writers
			*/						
			void RegisterWriter(String^ pluginPath,
				int %firstPluginID,
				int %registeredCount);

			/** Creates a Reader in the Sdk Manager
				*	\param pManager      The Sdk Manager where the reader will be created
				*	\param pImporter     Importer that will hold the created Reader
				* \param pPluginID     Plug-in ID to create a Reader from
				*/
				FbxReader^ CreateReader(FbxSdkManagerManaged^ manager, 
				FbxImporter^ importer, 
				int pluginID);

			/** Registers Writers
			*	\param pCreateF             Provide function information on file format
			* \param pInfoF               Provide information about the file format 
			* \param pFirstPluginID       Contains the ID of the first plug-in found
			* \param pRegisteredCount     Contains the number of registered writers
			* \param pIOSettingsFillerF
			*/
			/*void RegisterWriter(KFbxWriter::CreateFuncType pCreateF, 
			KFbxWriter::GetInfoFuncType pInfoF,
			int& pFirstPluginID,
			int& pRegisteredCount,
			KFbxWriter::IOSettingsFillerFuncType pIOSettingsFillerF = NULL);*/


			/** Creates a Writer in the Sdk Manager
				* \param pManager      The Sdk Manager where the writer will be created
				*	\param pExporter     Exporter that will hold the created Writer
				* \param pPluginID     Plug-in ID to create a Writer from
				*/
				FbxWriter^ CreateWriter(FbxSdkManagerManaged^ manager, 
					FbxExporter^ exporter,
				int pluginID);

			/** Search for the Reader ID by the extension of the file.
			*	\return     The Reader ID if found, else returns -1
			*/
			int FindReaderIDByExtension(String^ ext);

			/** Search for the Writer ID by the extension of the file.
			*	\return     The Writer ID if found, else returns -1
			*/						
			int FindWriterIDByExtension(String^ ext);

			/** Search for the Reader ID by the description of the file format.
			*	\return     The Reader ID if found, else returns -1
			*/						
			int FindReaderIDByDescription(String^ desc);

			/** Search for the Writer ID by the description of the file format.
			*	\return     The Writer ID if found, else returns -1
			*/						
			int FindWriterIDByDescription(String^ desc);

			/** Verifies if the file format of the Reader is FBX.
			*	\return     \c true if the file format of the Reader is FBX.
			*/
			bool ReaderIsFBX(int fileFormat);

			/** Verifies if the file format of the Writer is FBX.
			*	\return     \c true if the file format of the Writer is FBX.
			*/
			bool WriterIsFBX(int fileFormat);

			/** Get the number of importable file formats.
			*	\return     Number of importable formats.
			*/
			property int ReaderFormatCount
			{
				int get();
			}

			/** Get the number of exportable file formats.
			*	\return      Number of exportable formats.
			* \remarks     Multiple identifiers for the same format are counted as 
			*              file formats. For example, eFBX_BINARY, eFBX_ASCII and eFBX_ENCRYPTED
			*              count as three file formats.
			*/						
			property int WriterFormatCount
			{
				int get();
			}

			/** Get the description of an importable file format.
			*	\param pFileFormat     File format identifier.
			*	\return                Pointer to the character representation of the description.
			*/
			String^ GetReaderFormatDescription(int fileFormat);

			/** Get the description of an exportable file format.
			*	\param pFileFormat     File format identifier.
			*	\return                Pointer to the character representation of the description.
			*/						
			String^ GetWriterFormatDescription(int fileFormat);

			/** Get the file extension of an importable file format.
			*	\param pFileFormat     File format identifier.
			*	\return                Pointer to the character representation of the file extension.
			*/						
			String^ GetReaderFormatExtension(int fileFormat);

			/** Get the file extension of an exportable file format.
			*	\param pFileFormat     File format identifier.
			*	\return                Pointer to the character representation of the file extension.
			*/						
			String^ GetWriterFormatExtension(int fileFormat);

			/** Get a list of the writable file format versions.
			*	\param pFileFormat     File format identifier.
			*	\return                Pointer to a list of user-readable strings representing the versions.
			*/
			//char const* const* GetWritableVersions(int pFileFormat) const;


			/** Detect the file format of the specified file.
			* \param pFileName       The file to determine his file format.
			* \param pFileFormat     The file format identifier if the function returns \c true. if the function returns \c false, unmodified otherwise.
			* \return                Return \c true if the file has been determined successfully, 
			*                        \c false otherwise.
			* \remarks               This function attempts to detect the file format of pFileName based on the file extension and, 
			*                        in some cases, its content. This function may not be able to determine all file formats.
			*                        Use this function as a helper before calling \c SetFileFormat().
			* \note                  The file must not be locked (already opened) for this function to succeed.
			*/
			bool DetectFileFormat(String^ fileName, int %fileFormat);

			/** Gets the native reader file format.
			*	\return     The native reader file format ID.
			*/						
			property int NativeReaderFormat
			{
				int get();
			}

			/** Gets the native writer file format.
			*	\return     The native writer file format ID.
			*/						
			property int NativeWriterFormat
			{
				int get();
			}

			/** Fills the IO Settings from all readers registered
			*	\param pIOS			   The properties hierarchies to fill
			*/
			/*void FillIOSettingsForReadersRegistered(Skill::FbxSDK::IO::FbxIOSettings^ IOS)
			{
				reg->FillIOSettingsForReadersRegistered(&(KFbxIOSettings*)IOS->emitter);
			}*/

			/** Fills the IO Settings from all writers registered
			*	\param pIOS			   The properties hierarchies to fill
			*/			
			/*void FillIOSettingsForWritersRegistered(Skill::FbxSDK::IO::FbxIOSettings^ IOS)
			{
				reg->FillIOSettingsForWritersRegistered(&(KFbxIOSettings*)IOS->emitter);
			}*/


			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			void RegisterInternalIOPlugins();
#endif //DOXYGEN
		};


	}
}