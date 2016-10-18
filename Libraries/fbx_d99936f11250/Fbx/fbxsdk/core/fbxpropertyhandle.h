#ifndef _FBXSDK_CORE_PROPERTY_HANDLE_H_
#define _FBXSDK_CORE_PROPERTY_HANDLE_H_
class FbxPropertyPage;
class FbxPropertyHandle;
class FbxConnectionPointFilter;
class FBXSDK_DLL FbxPropertyHandle
{
public:
static FbxPropertyHandle Create();
static FbxPropertyHandle Create(const FbxPropertyHandle& pInstanceOf);
static FbxPropertyHandle Create(const char* pName, EFbxType pType=eFbxUndefined);
static FbxPropertyHandle Create(const char* pName, FbxPropertyHandle pTypeInfo);
bool Destroy();
FbxPropertyHandle();
FbxPropertyHandle(const FbxPropertyHandle& pAddress);
~FbxPropertyHandle();
FbxPropertyHandle(FbxPropertyPage* pPage, FbxInt pId=FBXSDK_PROPERTY_ID_ROOT);
FbxPropertyHandle& operator=(const FbxPropertyHandle& pHandle);
bool operator==(const FbxPropertyHandle& pHandle) const;
bool operator!=(const FbxPropertyHandle& pHandle) const;
bool operator< (const FbxPropertyHandle& pHandle) const;
bool operator> (const FbxPropertyHandle& pHandle) const;
bool Is(const FbxPropertyHandle& pHandle) const;
bool Valid() const;
const char* GetName() const;
const char* GetLabel() const;
bool SetLabel(const char* pLabel);
EFbxType GetType() const;
FbxPropertyHandle GetTypeInfo() const;
FbxPropertyFlags::EFlags GetFlags() const;
FbxPropertyFlags::EInheritType GetFlagsInheritType(FbxPropertyFlags::EFlags pFlags, bool pCheckReferences) const;
bool ModifyFlags(FbxPropertyFlags::EFlags pFlags, bool pValue);
bool SetFlagsInheritType(FbxPropertyFlags::EFlags pFlags, FbxPropertyFlags::EInheritType pType);
void* GetUserData() const;
bool SetUserData(const void* pUserData);
int GetUserTag() const;
bool SetUserTag(int pUserData);
int AddEnumValue(const char* pStringValue);
void InsertEnumValue(int pIndex, const char* pStringValue);
int GetEnumCount();
void SetEnumValue(int pIndex, const char* pStringValue);
void RemoveEnumValue(int pIndex);
char* GetEnumValue(int pIndex);
void BeginCreateOrFindProperty();
void EndCreateOrFindProperty();
inline bool IsRoot() const { return ( mPage && mId == 0 ) ? true : false; }
bool IsChildOf(const FbxPropertyHandle& pParent) const;
bool IsDescendentOf(const FbxPropertyHandle& pParent) const;
bool SetParent(const FbxPropertyHandle& pOther );
FbxPropertyHandle Add(const char* pName, const FbxPropertyHandle& pTypeInfo);
FbxPropertyHandle GetParent() const;
FbxPropertyHandle GetChild() const;
FbxPropertyHandle GetSibling() const;
FbxPropertyHandle GetFirstDescendent() const;
FbxPropertyHandle GetNextDescendent(const FbxPropertyHandle& pHandle) const;
FbxPropertyHandle Find(const char* pName, bool pCaseSensitive) const;
FbxPropertyHandle Find(const char* pName, const FbxPropertyHandle& pTypeInfo, bool pCaseSensitive) const;
FbxPropertyHandle Find(const char* pName, const char* pChildrenSeparator, bool pCaseSensitive) const;
FbxPropertyHandle Find(const char* pName, const char* pChildrenSeparator, const FbxPropertyHandle& pTypeInfo, bool pCaseSensitive) const;
bool ConnectSrc(const FbxPropertyHandle& pSrc, const FbxConnection::EType pType=FbxConnection::eDefault);
int GetSrcCount(FbxConnectionPointFilter* pFilter=0) const;
FbxPropertyHandle GetSrc(FbxConnectionPointFilter* pFilter=0, int pIndex=0) const;
bool DisconnectSrc(const FbxPropertyHandle& pSrc);
bool IsConnectedSrc(const FbxPropertyHandle& pSrc);
bool ConnectDst(const FbxPropertyHandle& pDst, const FbxConnection::EType pType=FbxConnection::eDefault);
int GetDstCount(FbxConnectionPointFilter* pFilter=0) const;
FbxPropertyHandle GetDst(FbxConnectionPointFilter* pFilter=0, int pIndex=0) const;
bool DisconnectDst(const FbxPropertyHandle& pDst);
bool IsConnectedDst(const FbxPropertyHandle& pDst);
void ClearConnectCache();
void WipeAllConnections();
bool HasMin() const;
bool GetMin(void* pValue, EFbxType pValueType) const;
bool SetMin(const void* pValue, EFbxType pValueType);
template <class T> inline bool SetMin(const T& pValue){ return SetMin(&pValue, FbxTypeOf(pValue)); }
template <class T> inline T GetMin(const T* pFBX_TYPE) const { T lValue; GetMin(&lValue, FbxTypeOf(lValue)); return lValue; }
bool HasSoftMin() const;
bool GetSoftMin(void* pValue, EFbxType pValueType) const;
bool SetSoftMin(const void* pValue, EFbxType pValueType);
template <class T> inline bool SetSoftMin(const T& pValue){ return SetSoftMin(&pValue, FbxTypeOf(pValue)); }
template <class T> inline T GetSoftMin(const T* pFBX_TYPE) const { T lValue; GetSoftMin(&lValue, FbxTypeOf(lValue)); return lValue; }
bool HasMax() const;
bool GetMax(void* pValue, EFbxType pValueType) const;
bool SetMax(const void* pValue, EFbxType pValueType);
template <class T> inline bool SetMax(const T& pValue){ return SetMax(&pValue, FbxTypeOf(pValue)); }
template <class T> inline T GetMax(const T* pFBX_TYPE) const { T lValue; GetMax(&lValue, FbxTypeOf(lValue)); return lValue; }
bool HasSoftMax() const;
bool GetSoftMax(void* pValue, EFbxType pValueType) const;
bool SetSoftMax(const void* pValue, EFbxType pValueType);
template <class T> inline bool SetSoftMax(const T& pValue){ return SetSoftMax(&pValue, FbxTypeOf(pValue)); }
template <class T> inline T GetSoftMax(const T* pFBX_TYPE) const { T lValue; GetSoftMax(&lValue, FbxTypeOf(lValue)); return lValue; }
FbxPropertyFlags::EInheritType GetValueInheritType(bool pCheckReferences) const;
bool SetValueInheritType(FbxPropertyFlags::EInheritType pType);
bool GetDefaultValue(void* pValue, EFbxType pValueType) const;
bool Get(void* pValue, EFbxType pValueType) const;
bool Set(const void* pValue, EFbxType pValueType, bool pCheckValueEquality);
template <class T> inline bool Set(const T& pValue){ return Set(&pValue, FbxTypeOf(pValue)); }
template <class T> inline T Get(const T* pFBX_TYPE) const { T lValue; Get(&lValue, FbxTypeOf(lValue)); return lValue; }
void SetPageDataPtr(void* pData);
void* GetPageDataPtr() const;
bool PushPropertiesToParentInstance();
bool IsAReferenceTo(void) const;
void* GetReferenceTo(void) const;
bool IsReferencedBy(void) const;
int GetReferencedByCount(void) const;
void* GetReferencedBy(int pIndex) const;
private:
FbxPropertyPage* mPage;
FbxInt    mId;
};
#endif