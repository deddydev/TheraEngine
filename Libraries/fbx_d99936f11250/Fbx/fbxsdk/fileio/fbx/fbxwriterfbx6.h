#ifndef _FBXSDK_FILEIO_FBX_WRITER_FBX6_H_
#define _FBXSDK_FILEIO_FBX_WRITER_FBX6_H_
#include <fbxsdk.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class Fbx6TypeDefinition
class Fbx6TypeWriteReferences
class Fbx6TypeObjectHierarchy
typedef FbxArray<FbxTakeInfo*> TakeInfoArray
class FbxWriterFbx6 : public FbxWriter
public:
    FbxWriterFbx6(FbxManager& pManager, FbxExporter& pExporter, int pID, FbxStatus& pStatus)
    virtual ~FbxWriterFbx6()
    virtual bool    FileCreate(char* pFileName)
    virtual bool	FileCreate(FbxStream* pStream, void* pStreamData)
    virtual bool    FileClose()
    virtual bool    IsFileOpen()
    typedef enum 
eASCII, eBINARY, eENCRYPTED
 EExportMode
    void            SetExportMode(EExportMode pMode)
    virtual void    GetWriteOptions()
    virtual bool    Write(FbxDocument* pDocument)
    virtual bool    PreprocessScene(FbxScene& pScene)
    virtual bool    PostprocessScene(FbxScene& pScene)
    virtual void    PluginWriteParameters(FbxObject& pParams)
    virtual bool    Write(FbxDocument* pDocument, FbxIO* pFbx)
    virtual void    SetProgressHandler(FbxProgress *pProgress)
	virtual bool SupportsStreams() const		
 return true
private:
    bool            WriteNodes(FbxScene& pScene, bool pIncludeRoot)
    bool            WriteNodes(const FbxDocument& pDocument)
    void            WriteGlobalLightSettings(FbxScene& pScene)
    void            WriteShadowPlane(FbxScene& pScene)
    void            WriteShadowPlaneSection(FbxScene& pScene)
    void            WriteAmbientColor(FbxScene& pScene)
    void            WriteFogOption(FbxScene& pScene)
    void            WriteGlobalCameraSettings(FbxScene& pScene)
    void            WriteGlobalTimeSettings(FbxScene& pScene)