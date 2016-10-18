#ifndef _FBXSDK_FILEIO_EXPORTER_H_
#define _FBXSDK_FILEIO_EXPORTER_H_
class FbxIO;
class FbxIOFileHeaderInfo;
class FbxThread;
class FbxWriter;
struct FbxExportThreadArg;
class FBXSDK_DLL FbxExporter : public FbxIOBase
{
FBXSDK_OBJECT_DECLARE(FbxExporter, FbxIOBase);
public:
virtual bool Initialize(const char* pFileName, int pFileFormat = -1, FbxIOSettings* pIOSettings = NULL);
virtual bool Initialize(FbxStream* pStream, void* pStreamData=NULL, int pFileFormat = -1, FbxIOSettings * pIOSettings = NULL);
bool GetExportOptions();
FbxIOSettings* GetIOSettings();
void SetIOSettings(FbxIOSettings* pIOSettings);
bool Export(FbxDocument* pDocument, bool pNonBlocking=false);
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
bool IsExporting(bool& pExportResult);
#endif
float GetProgress(FbxString* pStatus=NULL);
void SetProgressCallback(FbxProgressCallback pCallback, void* pArgs=NULL);
int GetFileFormat();
bool IsFBX();
char const* const* GetCurrentWritableVersions();
bool SetFileExportVersion(FbxString pVersion, FbxSceneRenamer::ERenamingMode pRenamingMode=FbxSceneRenamer::eNone);
inline void SetResamplingRate(double pResamplingRate){ mResamplingRate = pResamplingRate; }
void SetDefaultRenderResolution(FbxString pCamName, FbxString pResolutionMode, double pW, double pH);
FbxIOFileHeaderInfo* GetFileHeaderInfo();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
bool GetExportOptions(FbxIO* pFbxObject);
bool Export(FbxDocument* pDocument, FbxIO* pFbxObject);
protected:
virtual void Construct(const FbxObject* pFrom);
virtual void Destruct(bool pRecursive);
virtual void SetOrCreateIOSettings(FbxIOSettings* pIOSettings, bool pAllowNULL);
void Reset();
bool FileCreate();
void FileClose();
private:
bool ExportProcess(FbxDocument* pDocument);
int        mFileFormat;
FbxWriter*      mWriter;
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
FbxThread*      mExportThread;
FbxExportThreadArg*    mExportThreadArg;
bool       mExportThreadResult;
bool       mIsThreadExporting;
#endif
FbxProgress      mProgress;
FbxStream*                      mStream;
void*                           mStreamData;
FbxString      mStrFileVersion;
double       mResamplingRate;
FbxSceneRenamer::ERenamingMode mRenamingMode;
FbxIOFileHeaderInfo*   mHeaderInfo;
FbxIOSettings*     mIOSettings;
bool       mClientIOSettings;
friend void ExportThread(void*);
#endif
};
class FBXSDK_DLL FbxEventPreExport : public FbxEvent<FbxEventPreExport>
{
FBXSDK_EVENT_DECLARE(FbxEventPreExport);
public:
FbxEventPreExport(FbxDocument* pDocument) : mDocument(pDocument) {};
FbxDocument* mDocument;
};
class FBXSDK_DLL FbxEventPostExport : public FbxEvent<FbxEventPostExport>
{
FBXSDK_EVENT_DECLARE(FbxEventPostExport);
public:
FbxEventPostExport(FbxDocument* pDocument) : mDocument(pDocument) {};
FbxDocument* mDocument;
};
#endif