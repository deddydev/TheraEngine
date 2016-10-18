#ifndef _FBXSDK_FILEIO_FBX_READER_FBX7_H_
#define _FBXSDK_FILEIO_FBX_READER_FBX7_H_
#include <fbxsdk.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
struct FbxReaderFbx7_Impl
class FbxReaderFbx7 : public FbxReader
public:
    typedef enum
        eASCII,     
        eENCRYPTED  
    FbxReaderFbx7(FbxManager& pManager, FbxImporter& pImporter, int pID, FbxStatus& pStatus)
    virtual ~FbxReaderFbx7()
    virtual bool FileOpen(char* pFileName, EFileOpenSpecialFlags pFlags)
    virtual bool FileOpen(char* pFileName)
    virtual bool FileOpen(FbxFile* pFile)
	virtual bool FileOpen(FbxStream * pStream, void* pStreamData)
    virtual bool FileClose()
    virtual bool IsFileOpen()
    EImportMode GetImportMode()
    virtual void GetVersion(int& pMajor, int& pMinor, int& pRevision)
    virtual bool GetAxisInfo(FbxAxisSystem* pAxisSystem, FbxSystemUnit* pSystemUnits)
	virtual bool GetFrameRate(FbxTime::EMode &pTimeMode)
    virtual bool GetStatistics(FbxStatistics* pStats)
    virtual bool GetReadOptions(bool pParseFileAsNeeded = true)
    virtual bool Read(FbxDocument *pDocument)
	virtual void PluginReadParameters(FbxObject& pParams)
    virtual bool GetReadOptions(FbxIO* pFbx, bool pParseFileAsNeeded = true)
    virtual bool Read(FbxDocument *pDocument, FbxIO* pFbx)
    virtual FbxDocumentInfo*  GetSceneInfo()
    virtual FbxArray<FbxTakeInfo*>* GetTakeInfo()
    virtual void SetProgressHandler(FbxProgress *pProgress)
	virtual void SetEmbeddingExtractionFolder(const char* pExtractFolder)
	virtual bool SupportsStreams() const 
 return true
private:
    FbxReaderFbx7(const FbxReaderFbx7&)
    FbxReaderFbx7& operator=(FbxReaderFbx7 const&)
private:
    FbxReaderFbx7_Impl* mImpl
#include <fbxsdk/fbxsdk_nsend.h>
#endif 