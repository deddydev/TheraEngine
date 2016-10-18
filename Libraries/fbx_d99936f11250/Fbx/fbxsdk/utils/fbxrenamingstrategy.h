#ifndef _FBXSDK_UTILS_RENAMINGSTRATEGY_H_
#define _FBXSDK_UTILS_RENAMINGSTRATEGY_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxcharptrset.h>
#include <fbxsdk/utils/fbxnamehandler.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxScene
class FbxNode
class FBXSDK_DLL FbxRenamingStrategyInterface
public:
    FbxRenamingStrategyInterface()
    virtual ~FbxRenamingStrategyInterface ()
    virtual void Clear() = 0
    virtual bool Rename(FbxNameHandler& pName) = 0
    virtual FbxRenamingStrategyInterface* Clone() = 0
class FBXSDK_DLL FbxRenamingStrategyNumber : public FbxRenamingStrategyInterface
public:
    FbxRenamingStrategyNumber()
    virtual ~FbxRenamingStrategyNumber ()
    virtual void Clear()
    virtual bool Rename(FbxNameHandler& pName)
    virtual FbxRenamingStrategyInterface* Clone()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    struct NameCell
        NameCell(const char* pName) :
            mName(pName),
            mInstanceCount(0)
        FbxString mName
        int mInstanceCount
    FbxArray<NameCell*> mNameArray
#endif 
class FBXSDK_DLL FbxRenamingStrategy : public FbxRenamingStrategyInterface
public:
    enum EDirection
        eToFBX,
        eFromFBX
    FbxRenamingStrategy(EDirection pMod, bool pOnCreationRun = false)
    virtual ~FbxRenamingStrategy()
    virtual bool Rename(FbxNameHandler& pName)
    virtual void Clear()
    virtual FbxRenamingStrategyInterface* Clone()
    enum EClashType
        eNameClashAuto,
        eNameClashType1,
        eNameClashType2
    void SetClashSoverType(EClashType pType)
    static char* NoPrefixName (const char* pName)
    static char* NoPrefixName (FbxString& pName)
    virtual char* GetNameSpace() 
 return mNameSpace.Buffer()
    virtual void SetInNameSpaceSymbol(FbxString pNameSpaceSymbol)
mInNameSpaceSymbol = pNameSpaceSymbol
    virtual void SetOutNameSpaceSymbol(FbxString pNameSpaceSymbol)
mOutNameSpaceSymbol = pNameSpaceSymbol
    virtual void SetCaseSensibility(bool pIsCaseSensitive)
mCaseSensitive = pIsCaseSensitive 
    virtual void SetReplaceNonAlphaNum(bool pReplaceNonAlphaNum)
mReplaceNonAlphaNum = pReplaceNonAlphaNum
    virtual void SetFirstNotNum(bool pFirstNotNum)
mFirstNotNum = pFirstNotNum
    virtual bool RenameUnparentNameSpace(FbxNode* pNode, bool pIsRoot = false)
    virtual bool RemoveImportNameSpaceClash(FbxNode* pNode)
    virtual void GetParentsNameSpaceList(FbxNode* pNode, FbxArray<FbxString*> &pNameSpaceList)
    virtual bool PropagateNameSpaceChange(FbxNode* pNode, FbxString OldNS, FbxString NewNS)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual bool RenameToFBX(FbxNameHandler& pName)
    virtual bool RenameFromFBX(FbxNameHandler& pName)
    virtual FbxString& ReplaceNonAlphaNum(FbxString& pName,  const char* pReplace, bool pIgnoreNameSpace)
    EDirection mMode
    EClashType mType
    struct NameCell
        NameCell(const char* pName) :
            mName(pName),
            mInstanceCount(0)
        FbxString mName
        int mInstanceCount
    FbxCharPtrSet		mStringNameArray
    FbxArray<NameCell*>	mExistingNsList
    bool				mOnCreationRun
    bool				mCaseSensitive
    bool				mReplaceNonAlphaNum
    bool				mFirstNotNum
    FbxString			mNameSpace
    FbxString			mInNameSpaceSymbol
    FbxString			mOutNameSpaceSymbol
#endif 
class FBXSDK_DLL FbxSceneRenamer
public:
    FbxSceneRenamer(FbxScene* pScene) 
mScene = pScene
    virtual ~FbxSceneRenamer()
    enum ERenamingMode
        eNone,
        eMAYA_TO_FBX5,
        eMAYA_TO_FBX_MB75,
        eMAYA_TO_FBX_MB70,
        eFBXMB75_TO_FBXMB70,
        eFBX_TO_FBX,
        eMAYA_TO_FBX,
        eFBX_TO_MAYA,
        eLW_TO_FBX,
        eFBX_TO_LW,
        eXSI_TO_FBX,
        eFBX_TO_XSI,
        eMAX_TO_FBX,
        eFBX_TO_MAX,
        eMB_TO_FBX,
        eFBX_TO_MB,
        eDAE_TO_FBX,
        eFBX_TO_DAE
    void RenameFor(ERenamingMode pMode)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    void ResolveNameClashing(    bool pFromFbx, bool pIgnoreNS, bool pIsCaseSensitive,
                                bool pReplaceNonAlphaNum, bool pFirstNotNum,
                                FbxString pInNameSpaceSymbol, FbxString pOutNameSpaceSymbol,
                                bool pNoUnparentNS
#include <fbxsdk/fbxsdk_nsend.h>
#endif 