#pragma once
#include "stdafx.h"
namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxStringManaged;		
		ref class FbxDocumentManaged;
		ref class FbxErrorManaged;
		ref class FbxAxisSystem;
		ref class FbxSystemUnit;

		namespace IO
		{
			ref class FbxStreamOptionsManaged;
			ref class FbxImporterManaged;
			public ref class FbxReaderManaged : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxReaderManaged, FbxReader);
				INATIVEPOINTER_DECLARE(FbxReaderManaged, FbxReader);
			protected:
				FbxStreamOptionsManaged^ readOptions;
			public:								
				virtual Version^ GetVersion();

				virtual bool FileOpen(String^ fileName);
				//virtual bool FileOpen(KFile * pFile);
				virtual bool FileClose();
				virtual property bool IsFileOpen { bool get(); }

				virtual FbxStreamOptionsManaged^ GetReadOptions(bool parseFileAsNeeded);
				virtual FbxStreamOptionsManaged^ GetReadOptions();
				virtual bool Read(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions);

				enum class Error
				{
					FileCorrupted,
					FileVersionNotSupportedYet,
					FileVersionNotSupportedAnymore,
					FileNotOpened,
					FileNotCreated,
					WrongPassword,
					StreamOptionsNotSet,
					InvalidDocumentHandle,
					DocumentNotSupported,
					UnresolvedExternalReferences, // this is a warning
					UnidentifiedError,
					ErrorCount
				};

				REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);
				property Error LastErrorID
				{
					Error get();
				}
				property String^ LastErrorString
				{
					String^ get();
				}

				void GetMessage(FbxStringManaged^ message);
				void ClearMessage();

				//	typedef KFbxReader* (*CreateFuncType)(KFbxSdkManager& pManager,
				//		KFbxImporter& pImporter,
				//		int pID);

				//	typedef void (*IOSettingsFillerFuncType)(KFbxIOSettings& pIOS);

				enum class InfoRequest {
					InfoExtension, // return a null terminated char const* const*
					InfoDescriptions, // return a null terminated char const* const*
					Reserved1 = 0xFBFB,
				};

				//	typedef void* (*GetInfoFuncType)(KInfoRequest pRequest, int pReaderTypeId);			

			public:
				enum class FileOpenSpecialFlags
				{
					ParseForGlobalSettings = 1,
					ParseForStatistics = 2
				};

				virtual bool FileOpen(String^ fileName, FileOpenSpecialFlags flags);
				virtual bool GetAxisInfo(FbxAxisSystem^ axisSystem, FbxSystemUnit^ systemUnits);
				//virtual bool GetStatistics(FbxStatistics* pStats) {return false; };
				
			};

		}
	}
}