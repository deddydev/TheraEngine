#ifndef _FBXSDK_FILEIO_WRITER_H_
#define _FBXSDK_FILEIO_WRITER_H_
class FbxStatus;
class FbxManager;
class FbxFile;
class FbxStream;
class FbxObject;
class FbxDocument;
class FbxScene;
class FbxExporter;
class FbxIO;
class FbxIOSettings;
class FbxProgress;
#define IOSP GetIOSettings()
class FBXSDK_DLL FbxWriter
{
public:
FbxWriter(FbxManager& pManager, int pID, FbxStatus& pStatus);
virtual ~FbxWriter();
enum EInfoRequest
{
eInfoExtension,
eInfoDescriptions,
eInfoVersions,
eInfoCompatibleDesc,
eInfoUILabel,
eReserved1 = 0xFBFB,
};
typedef FbxWriter*    (*CreateFuncType)(FbxManager& pManager, FbxExporter& pExporter, int pSubID, int pPluginID);
typedef void     (*IOSettingsFillerFuncType)(FbxIOSettings& pIOS);
typedef void*     (*GetInfoFuncType)(EInfoRequest pRequest, int pWriterTypeId);
virtual bool     FileCreate(char* pFileName) = 0;
virtual bool     FileCreate(FbxStream* pStream, void* pStreamData);
virtual bool     FileClose() = 0;
virtual bool     IsFileOpen() = 0;
virtual void              GetWriteOptions() = 0;
virtual bool     Write(FbxDocument* pDocument) = 0;
virtual bool     PreprocessScene(FbxScene &pScene) = 0;
virtual bool     PostprocessScene(FbxScene &pScene) = 0;
#ifndef FBXSDK_ENV_WINSTORE
virtual void     PluginWriteParameters(FbxObject& pParams);
#endif
virtual FbxNode*    FindRootNode(FbxScene& pScene);
virtual bool     CheckSpaceInNodeNameRecursive(FbxNode* pNode, FbxString& pNodeNameList);
bool       SetFileExportVersion(FbxString pVersion);
void       SetRenamingMode(FbxSceneRenamer::ERenamingMode pRenamingMode){mRenamingMode = pRenamingMode;}
inline void      SetResamplingRate(double pResamplingRate){mResamplingRate = pResamplingRate;}
bool       IsGenuine();
virtual FbxIOSettings * GetIOSettings();
virtual void SetIOSettings(FbxIOSettings * pIOSettings);
virtual void SetProgressHandler(FbxProgress*  ){}
virtual bool SupportsStreams() const;
protected:
#ifndef FBXSDK_ENV_WINSTORE
void       PluginsWriteBegin(FbxScene& pScene);
void       PluginsWrite(FbxIO& pFbx, bool pWriteObjectId);
void       PluginsWriteEnd(FbxScene& pScene);
#endif
public:
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxStatus& GetStatus()              { return mStatus; }
protected:
FbxWriter&      operator=(FbxWriter const&) { return *this; }
FbxStatus&                          mStatus;
FbxManager&             mManager;
FbxString       mFileVersion;
double           mResamplingRate;
FbxSceneRenamer::ERenamingMode     mRenamingMode;
private:
int            mInternalID;
FbxIOSettings *                     mIOSettings;
friend struct FbxWriterFbx7_Impl;
#endif
};
#define IOS_REF (*GetIOSettings())
#endif