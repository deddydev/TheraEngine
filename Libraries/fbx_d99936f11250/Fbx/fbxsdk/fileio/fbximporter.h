#ifndef _FBXSDK_FILEIO_IMPORTER_H_
#define _FBXSDK_FILEIO_IMPORTER_H_
class FbxIO;
class FbxIOFileHeaderInfo;
class FbxDocumentInfo;
class FbxTakeInfo;
class FbxReader;
class FbxThread;
struct FbxImportThreadArg;
class FBXSDK_DLL FbxImporter : public FbxIOBase
{
FBXSDK_OBJECT_DECLARE(FbxImporter, FbxIOBase);
public:
virtual bool Initialize(const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL);
virtual bool Initialize(FbxStream* pStream, void* pStreamData=NULL, const int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
void GetFileVersion(int& pMajor, int& pMinor, int& pRevision);
bool GetDefaultRenderResolution(FbxString& pCamName, FbxString& pResolutionMode, double& pW, double& pH);
FbxIOFileHeaderInfo* GetFileHeaderInfo();
enum EStreamOptionsGeneration
{
eParseFile,
eDoNotParseFile
};
bool GetImportOptions(EStreamOptionsGeneration pStreamOptionsGeneration = eParseFile);
bool GetImportOptions(FbxIO* pFbxObject);
bool Import(FbxDocument* pDocument, bool pNonBlocking=false);
#if  !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
bool IsImporting(bool& pImportResult);
#endif
float GetProgress(FbxString* pStatus=NULL);
void SetProgressCallback(FbxProgressCallback pCallback, void* pArgs=NULL);
void SetEmbeddingExtractionFolder(const char* pExtractFolder);
const char* GetEmbeddingExtractionFolder();
FbxIOSettings* GetIOSettings();
void SetIOSettings(FbxIOSettings* pIOSettings);
void SetPassword(char* pPassword);
int GetAnimStackCount();
FbxTakeInfo* GetTakeInfo(int pIndex);
FbxString GetActiveAnimStackName();
FbxDocumentInfo* GetSceneInfo();
int GetFileFormat ();
bool IsFBX();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxFile* GetFile();
FbxStream* GetStream();
void*      GetStreamData();
void ParseForGlobalSettings(bool pState);
void ParseForStatistics(bool pState);
bool GetAxisInfo(FbxAxisSystem* pAxisSystem, FbxSystemUnit* pSystemUnits);
bool GetStatistics(FbxStatistics* pStatistics);
bool GetFrameRate(FbxTime::EMode &pTimeMode);
#if  !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
#endif
#endif
};
inline FbxEventPreImport( FbxDocument* pDocument ) : mDocument(pDocument) {};
};
inline FbxEventPostImport( FbxDocument* pDocument ) : mDocument(pDocument) {};
};
#endif