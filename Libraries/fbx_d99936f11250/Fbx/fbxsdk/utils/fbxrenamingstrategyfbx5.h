#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX5_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX5_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/utils/fbxrenamingstrategybase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxRenamingStrategyFbx5: public FbxRenamingStrategyBase
public:
    FbxRenamingStrategyFbx5()
    virtual ~FbxRenamingStrategyFbx5()
    virtual bool DecodeScene(FbxScene* pScene)
    virtual bool EncodeScene(FbxScene* pScene)
    virtual bool DecodeString(FbxNameHandler& pName)
    virtual bool EncodeString(FbxNameHandler& pName, bool pIsPropertyName=false)
    virtual void CleanUp()
#include <fbxsdk/fbxsdk_nsend.h>
#endif 