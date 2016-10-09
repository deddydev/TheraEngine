#pragma once
#include "stdafx.h"
#include "FbxObject.h"

namespace FbxSDK
{
	ref class FbxErrorManaged;
	ref class FbxStringManaged;
	namespace IO
	{
		/** \brief Base class for FBX file import and export.
		* \nosubgrouping
		*/
		public ref class FbxIOManaged : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,FbxIO);
			FbxIOManaged(FbxIO* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
		protected: 
			virtual void CollectManagedMemory() override;
		public:
			/** Get the FBX version number for this version of the FBX SDK.
			* FBX version numbers start at 5.0.0.
			* \param pMajor        Version major number.
			* \param pMinor        Version minor number.
			* \param pRevision     Version revision number.
			*/
			static property Version^ CurrentVersion
			{
				Version^ get();
			}

			/** Initialize object.
			* \param pFileName     Name of file to access.
			* \return              \c true if successful, \c false otherwise.
			* \remarks             To identify the error, call KFbxIO::GetLastErrorID().
			*/
			virtual bool Initialize(String^ fileName);

//#ifdef KARCH_DEV_MACOSX_CFM
//				virtual bool Initialize(const FSSpec &pMacFileSpec);
//				virtual bool Initialize(const FSRef &pMacFileRef);
//				virtual bool Initialize(const CFURLRef &pMacURL);
//#endif

			/** Get the file name.
			* \return     Filename or an empty string if the filename has not been set.
			*/
			virtual property String^ FileName
			{
				String^ get();
			}

			/** Progress update function.
			* \param pTitle           Title of status box.
			* \param pMessage         Description of current file read/write step.
			* \param pDetail          Additional string appended to previous parameter.
			* \param pPercentDone     Finished percent of current file read/write.
			* \remarks                Overload this function to receive an update of current file read/write.
			*/
			//virtual void ProgressUpdate(String^ %title, String^ %message,String^ %detail, float percentDone);

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eFILE_CORRUPTED
			* - \e eFILE_VERSION_NOT_SUPPORTED_YET
			* - \e eFILE_VERSION_NOT_SUPPORTED_ANYMORE
			* - \e eFILE_NOT_OPENED
			* - \e eFILE_NOT_CREATED
			* - \e eOUT_OF_DISK_SPACE
			* - \e eUNINITIALIZED_FILENAME
			* - \e eUNIDENTIFIED_ERROR
			* - \e eINDEX_OUT_OF_RANGE
			* - \e ePASSWORD_ERROR
			* - \e eSTREAM_OPTIONS_NOT_SET
			* - \e eEMBEDDED_OUT_OF_SPACE
			*/
			enum class Error
			{
				FileCorrupted = FbxIO::eFILE_CORRUPTED,
				FileVersionNotSupportedYet= FbxIO::eFILE_VERSION_NOT_SUPPORTED_YET,
				FileVersionNotSupportedAnymore= FbxIO::eFILE_VERSION_NOT_SUPPORTED_ANYMORE,
				FileNotOpened = FbxIO::eFILE_NOT_OPENED,
				FileNotCreated= FbxIO::eFILE_NOT_CREATED,
				OutOfDiskSpace= FbxIO::eOUT_OF_DISK_SPACE,
				UninitializedFilename= FbxIO::eUNINITIALIZED_FILENAME,
				UnidentifiedError= FbxIO::eUNIDENTIFIED_ERROR,
				IndexOutOfRange= FbxIO::eINDEX_OUT_OF_RANGE,
				PasswordError= FbxIO::ePASSWORD_ERROR,
				StreamOptionsNotSet= FbxIO::eSTREAM_OPTIONS_NOT_SET,
				EmbeddedOutOfSpace= FbxIO::eEMBEDDED_OUT_OF_SPACE,
				ErrorCount = FbxIO::eERROR_COUNT
			};
			property Error LastErrorID
			{
				Error get();
			}
			property String^ LastErrorString
			{
				String^ get();
			}
			void GetMessage(FbxStringManaged^ message);
		};
	}
}