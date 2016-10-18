#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX7_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_FBX7_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/utils/fbxrenamingstrategybase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxRenamingStrategyFbx7 : public FbxRenamingStrategyBase
public:
    FbxRenamingStrategyFbx7()
    virtual ~FbxRenamingStrategyFbx7()
    virtual void CleanUp()
    virtual bool DecodeScene(FbxScene* pScene)
    virtual bool EncodeScene(FbxScene* pScene)
    virtual bool DecodeString(FbxNameHandler& pName)
    virtual bool EncodeString(FbxNameHandler& pName, bool pIsPropertyName=false)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 