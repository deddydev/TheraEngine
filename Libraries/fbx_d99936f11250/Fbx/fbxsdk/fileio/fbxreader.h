#ifndef _FBXSDK_FILEIO_READER_H_
#define _FBXSDK_FILEIO_READER_H_
class FbxManager;
class FbxFile;
class FbxStream;
class FbxObject;
class FbxDocument;
class FbxDocumentInfo;
class FbxScene;
class FbxImporter;
class FbxIOSettings;
class FbxAxisSystem;
class FbxStatistics;
class FbxSystemUnit;
class FbxNode;
class FbxProgress;
class FbxTakeInfo;
class FBXSDK_DLL FbxReader
{
public:
FbxReader(FbxManager& pManager, int pID, FbxStatus& pStatus);
virtual ~FbxReader();
enum EInfoRequest
{
eInfoExtension,
eInfoDescriptions,
eReserved1 = 0xFBFB,
};
enum EFileOpenSpecialFlags
{
eParseForGlobalSettings = 1,
eParseForStatistics = 2
};
typedef FbxReader*   (*CreateFuncType)(FbxManager& pManager, FbxImporter& pImporter, int pSubID, int pPluginID);
typedef void    (*IOSettingsFillerFuncType)(FbxIOSettings& pIOS);
typedef void*    (*GetInfoFuncType)(EInfoRequest pRequest, int pReaderTypeId);
virtual void    GetVersion(int& pMajor, int& pMinor, int& pRevision){ pMajor = pMinor = pRevision = 0; }
virtual bool    FileOpen(char* pFileName) = 0;
virtual bool    FileOpen(FbxStream* pStream, void* pStreamData);
virtual bool    FileClose() = 0;
virtual bool    IsFileOpen() = 0;
virtual bool                GetReadOptions(bool pParseFileAsNeeded = true) = 0;
virtual bool    Read(FbxDocument* pDocument) = 0;
#ifndef FBXSDK_ENV_WINSTORE
virtual void    PluginReadParameters(FbxObject& pParams);
#endif
virtual bool    FileOpen(char* pFileName, EFileOpenSpecialFlags  ){ return FileOpen(pFileName); }
virtual bool    GetAxisInfo(FbxAxisSystem*  , FbxSystemUnit*  ){ return false; }
virtual bool    GetStatistics(FbxStatistics*  ){ return false; }
virtual bool                GetFrameRate(FbxTime::EMode& pTimeMode) { pTimeMode = FbxTime::eDefaultMode; return false; }
virtual FbxDocumentInfo*   GetSceneInfo(){return NULL;}
virtual FbxArray<FbxTakeInfo*>* GetTakeInfo(){return NULL;}
virtual bool         GetDefaultRenderResolution(FbxString& pCamName, FbxString& pResolutionMode, double& pW, double& pH);
bool      IsGenuine();
virtual FbxIOSettings * GetIOSettings();
virtual void SetIOSettings(FbxIOSettings * pIOSettings);
virtual void SetProgressHandler(FbxProgress*  ){}
virtual void SetEmbeddingExtractionFolder(const char*  ){}
virtual bool SupportsStreams() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
virtual bool    FileOpen(FbxFile * pFile);
FbxStatus& GetStatus()              { return mStatus; }
protected:
void      SetDefaultRenderResolution(const char* pCamName, const char* pResolutionMode, double pW, double pH);
#ifndef FBXSDK_ENV_WINSTORE
void      PluginsReadBegin(FbxScene& pScene);
void      PluginsRead(const char* pName, const char* pVersion);
void      PluginsReadEnd(FbxScene& pScene);
#endif
FbxReader&     operator=(FbxReader const&) { return *this; }
virtual bool    CheckDuplicateNodeNames(FbxNode* pRootNode, FbxString& pDuplicateNodeNameList);
FbxStatus&                      mStatus;
FbxManager&      mManager;
FbxIODefaultRenderResolution* mData;
private:
int    mInternalID;
FbxIOSettings* mIOSettings;
friend struct FbxReaderFbx7_Impl;
#endif
};
#define IOS_REF (*GetIOSettings())
#endif