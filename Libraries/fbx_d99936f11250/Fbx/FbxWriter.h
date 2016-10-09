#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxRenamingStrategy.h"
#include "kfbxio/kfbxwriter.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxStringManaged;
		ref class FbxDocumentManaged;		
		ref class FbxErrorManaged;
		ref class FbxAxisSystem;
		ref class FbxSceneManaged;
		ref class FbxNode;
		ref class FbxSystemUnit;
		namespace IO
		{
			ref class FbxStreamOptionsManaged;
			public ref class FbxWriter : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxWriter,KFbxWriter);
				INATIVEPOINTER_DECLARE(FbxWriter,KFbxWriter);			
			public:				

				virtual bool FileCreate(String^ fileName);
				virtual bool FileClose();
				virtual property bool IsFileOpen
				{
					bool get();
				}

				virtual REF_PROPERTY_GET_DECLARE(FbxStreamOptionsManaged,WriteOptions);
				virtual bool Write(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions);

				virtual bool PreprocessScene(FbxSceneManaged^ scene);
				virtual bool PostprocessScene(FbxSceneManaged^ scene);

				bool SetFileExportVersion(FbxStringManaged^ version);

				void SetRenamingMode(FbxSceneRenamer::RenamingMode renamingMode);
				void SetResamplingRate(double resamplingRate);

				//! Error codes
				enum class Error
				{
					FileCorrupted,
					FileNotOpened,
					FileNotCreated,
					OutOfDiskSpace,
					StreamOptionsNotSet,
					InvalidDocumentHandle,
					DocumentNotSupported,
					UnidentifiedError,
					EmbeddedOutOfSpace,
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


				//	KFbxWriter& operator=(KFbxWriter const&) { return *this; }
				virtual FbxNode^ FindRootNode(FbxSceneManaged^ scene);
				virtual bool CheckSpaceInNodeNameRecursive(FbxNode^ node, FbxStringManaged^ nodeNameList);

				//	typedef KFbxWriter* (*CreateFuncType)(KFbxSdkManager& pManager,
				//		KFbxExporter& pExporter,
				//		int pID);

				//	typedef void (*IOSettingsFillerFuncType)(KFbxIOSettings& pIOS);

				enum class InfoRequest {
					InfoExtension, // return a null terminated char const* const*
					InfoDescriptions, // return a null terminated char const* const*
					InfoVersions, // return a null terminated char const* const*
					Reserved1 = 0xFBFB,
				};

				//	typedef void* (*GetInfoFuncType)(KInfoRequest pRequest, int pWriterTypeId);						
			};

		}
	}
}