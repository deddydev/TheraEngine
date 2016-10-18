#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX6_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX6_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/utils/fbxrenamingstrategybase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxRenamingStrategyFbx6: public FbxRenamingStrategyBase
public:
    FbxRenamingStrategyFbx6()
    virtual ~FbxRenamingStrategyFbx6()
    virtual bool DecodeScene(FbxScene* pScene)
    virtual bool EncodeScene(FbxScene* pScene)
    virtual bool DecodeString(FbxNameHandler& pName)
    virtual bool EncodeString(FbxNameHandler& pName, bool pIsPropertyName=false)
    virtual void CleanUp()
#include <fbxsdk/fbxsdk_nsend.h>
#endif 