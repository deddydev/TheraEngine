#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_BASE_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_BASE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxscene.h>
#include <fbxsdk/utils/fbxnamehandler.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxRenamingStrategyBase
public:
    FbxRenamingStrategyBase()
    FbxRenamingStrategyBase(char pNameSpaceSymbol)
    virtual ~FbxRenamingStrategyBase()
    virtual bool DecodeScene(FbxScene* pScene)=0
    virtual bool EncodeScene(FbxScene* pScene)=0
    virtual bool DecodeString(FbxNameHandler& pString)=0
    virtual bool EncodeString(FbxNameHandler& pString, bool pIsPropertyName=false)=0
    virtual void CleanUp()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    struct NameCell
        NameCell(const char* pName) :
    mName(pName),
        mInstanceCount(0)
    FbxString mName
    int mInstanceCount
    char                     mNamespaceSymbol
    FbxCharPtrSet					mStringNameArray
#endif 