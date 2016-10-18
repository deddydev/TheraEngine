#ifndef _FBXSDK_CORE_CLASSID_H_
#define _FBXSDK_CORE_CLASSID_H_
class FbxClassIdInfo;
class FbxObject;
class FbxPropertyHandle;
class FbxManager;
typedef FbxObject* (*FbxObjectCreateProc)(FbxManager& pManager, const char* pName, const FbxObject* pFrom);
class FBXSDK_DLL FbxClassId
{
public:
FbxClassId();
FbxClassId(const char* pClassName, const FbxClassId& pParentClassId, FbxObjectCreateProc pConstructor=0, const char* pFBXType=NULL, const char* pFBXSubType=NULL);
void Destroy();
const char* GetName() const;
FbxClassId GetParent() const;
FbxObject* Create(FbxManager& pManager, const char* pName, const FbxObject* pFrom);
bool Override(FbxObjectCreateProc pConstructor);
bool Is(const FbxClassId& pId) const;
bool operator==(const FbxClassId& pClassId) const;
bool operator!=(const FbxClassId& pClassId) const;
const char* GetFbxFileTypeName(bool pAskParent=false) const;
const char* GetFbxFileSubTypeName() const;
inline bool IsValid() const { return mClassInfo ? true : false; }
void SetObjectTypePrefix(const char* pObjectTypePrefix);
const char* GetObjectTypePrefix();
FbxPropertyHandle* GetRootClassDefaultPropertyHandle();
int ClassInstanceIncRef();
int ClassInstanceDecRef();
int GetInstanceRef();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline FbxClassIdInfo* GetClassIdInfo() { return mClassInfo; }
inline const FbxClassIdInfo* GetClassIdInfo() const { return mClassInfo; }
private:
FbxClassId(FbxClassIdInfo* mClassInfo);
bool SetFbxFileTypeName(const char* pName);
bool SetFbxFileSubTypeName(const char* pName);
FbxClassIdInfo* mClassInfo;
friend class FbxManager;
#endif
};
struct FbxClassIdCompare
{
inline int operator()(const FbxClassId& pKeyA, const FbxClassId& pKeyB) const
{
const FbxClassIdInfo* lKeyA = pKeyA.GetClassIdInfo();
const FbxClassIdInfo* lKeyB = pKeyB.GetClassIdInfo();
return lKeyA < lKeyB ? -1 : (lKeyA > lKeyB ? 1 : 0);
}
};
#endif