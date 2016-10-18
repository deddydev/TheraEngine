#ifndef _FBXSDK_CORE_PROPERTY_H_
#define _FBXSDK_CORE_PROPERTY_H_
class FbxObject;
class FbxAnimStack;
class FbxAnimLayer;
class FbxAnimCurveNode;
class FbxAnimCurve;
class FbxAnimEvaluator;
class FBXSDK_DLL FbxProperty
{
public:
static FbxProperty Create(const FbxProperty& pCompoundProperty, const FbxDataType& pDataType, const char* pName, const char* pLabel="", bool pCheckForDup=true, bool* pWasFound=NULL);
static FbxProperty Create(FbxObject* pObject, const FbxDataType& pDataType, const char* pName, const char* pLabel="", bool pCheckForDup=true, bool* pWasFound=NULL);
static FbxProperty CreateFrom(const FbxProperty& pCompoundProperty, FbxProperty& pFromProperty, bool pCheckForDup=true);
static FbxProperty CreateFrom(FbxObject* pObject, FbxProperty& pFromProperty, bool pCheckForDup=true);
void Destroy();
void DestroyRecursively();
void DestroyChildren();
FbxProperty();
FbxProperty(const FbxProperty& pProperty);
FbxProperty(const FbxPropertyHandle& pPropertyHandle);
~FbxProperty();
FbxDataType GetPropertyDataType() const;
FbxString GetName() const;
const char* GetNameAsCStr() const;
FbxString GetHierarchicalName() const;
FbxString GetLabel(bool pReturnNameIfEmpty=true) const;
void SetLabel(const FbxString& pLabel);
FbxObject* GetFbxObject() const;
void SetUserTag(int pTag);
int GetUserTag();
void SetUserDataPtr(void* pUserData);
void* GetUserDataPtr();
void ModifyFlag(FbxPropertyFlags::EFlags pFlag, bool pValue);
bool GetFlag(FbxPropertyFlags::EFlags pFlag) const;
FbxPropertyFlags::EFlags GetFlags() const;
FbxPropertyFlags::EInheritType GetFlagInheritType( FbxPropertyFlags::EFlags pFlag ) const;
bool SetFlagInheritType( FbxPropertyFlags::EFlags pFlag, FbxPropertyFlags::EInheritType pType );
bool ModifiedFlag( FbxPropertyFlags::EFlags pFlag ) const;
FbxProperty& operator= (const FbxProperty& pProperty);
bool operator== (const FbxProperty& pProperty) const;
bool operator!= (const FbxProperty& pProperty) const;
bool operator< (const FbxProperty& pProperty) const;
bool operator> (const FbxProperty& pProperty) const;
inline bool operator== (int pValue) const { return (pValue == 0) ? !IsValid() : IsValid(); }
inline bool operator!= (int pValue) const { return (pValue != 0) ? !IsValid() : IsValid(); }
bool CompareValue(const FbxProperty& pProperty) const;
bool CopyValue(const FbxProperty& pProperty);
template <class T> inline T Get() const { T lValue; Get(&lValue, FbxTypeOf(lValue)); return lValue; }
template <class T> inline bool Set(const T& pValue){ return Set(&pValue, FbxTypeOf(pValue)); }
bool IsValid() const;
static bool HasDefaultValue(FbxProperty& pProperty);
FbxPropertyFlags::EInheritType GetValueInheritType() const;
bool SetValueInheritType( FbxPropertyFlags::EInheritType pType );
bool Modified() const;
bool SupportSetLimitAsDouble() const;
bool SetMinLimit(double pMin);
bool HasMinLimit() const;
double GetMinLimit() const;
bool HasMaxLimit() const;
bool SetMaxLimit(double pMax);
double GetMaxLimit() const;
bool SetLimits(double pMin, double pMax);
int AddEnumValue(const char* pStringValue);
void InsertEnumValue(int pIndex, const char* pStringValue);
int GetEnumCount() const;
void SetEnumValue(int pIndex, const char* pStringValue);
void RemoveEnumValue(int pIndex);
const char* GetEnumValue(int pIndex) const;
inline bool IsRoot() const { return mPropertyHandle.IsRoot(); }
inline bool IsChildOf(const FbxProperty& pParent) const { return mPropertyHandle.IsChildOf(pParent.mPropertyHandle); }
inline bool IsDescendentOf(const FbxProperty& pAncestor) const { return mPropertyHandle.IsDescendentOf(pAncestor.mPropertyHandle); }
inline FbxProperty GetParent() const { return FbxProperty(mPropertyHandle.GetParent());  }
inline FbxProperty GetChild() const { return FbxProperty(mPropertyHandle.GetChild());   }
inline FbxProperty GetSibling() const { return FbxProperty(mPropertyHandle.GetSibling()); }
inline FbxProperty GetFirstDescendent() const { return FbxProperty(mPropertyHandle.GetFirstDescendent());   }
inline FbxProperty GetNextDescendent(const FbxProperty& pProperty) const { return FbxProperty(mPropertyHandle.GetNextDescendent(pProperty.mPropertyHandle)); }
inline FbxProperty Find (const char* pName, bool pCaseSensitive = true) const { return FbxProperty(mPropertyHandle.Find(pName,pCaseSensitive));  }
inline FbxProperty Find (const char* pName, const FbxDataType& pDataType, bool pCaseSensitive = true) const { return FbxProperty(mPropertyHandle.Find(pName,pDataType.GetTypeInfoHandle(),pCaseSensitive));  }
inline FbxProperty FindHierarchical (const char* pName, bool pCaseSensitive = true) const { return FbxProperty(mPropertyHandle.Find(pName,sHierarchicalSeparator,pCaseSensitive));  }
inline FbxProperty FindHierarchical (const char* pName, const FbxDataType& pDataType, bool pCaseSensitive = true) const { return FbxProperty(mPropertyHandle.Find(pName,sHierarchicalSeparator,pDataType.GetTypeInfoHandle(),pCaseSensitive));  }
inline void BeginCreateOrFindProperty(){ mPropertyHandle.BeginCreateOrFindProperty(); }
inline void EndCreateOrFindProperty(){ mPropertyHandle.EndCreateOrFindProperty(); }
class FbxPropertyNameCache
{
public:
FbxPropertyNameCache(const FbxProperty& prop) : mProp(const_cast<FbxProperty&>(prop)){ mProp.BeginCreateOrFindProperty(); }
~FbxPropertyNameCache(){ mProp.EndCreateOrFindProperty(); }
private:
FbxProperty& mProp;
FbxPropertyNameCache& operator=(const FbxPropertyNameCache& pOther){ mProp = pOther.mProp; mProp.BeginCreateOrFindProperty(); return *this; }
};
FbxAnimEvaluator* GetAnimationEvaluator() const;
bool IsAnimated(FbxAnimLayer* pAnimLayer=NULL) const;
template <class T> T EvaluateValue(const FbxTime& pTime=FBXSDK_TIME_INFINITE, bool pForceEval=false);
FbxPropertyValue& EvaluateValue(const FbxTime& pTime=FBXSDK_TIME_INFINITE, bool pForceEval=false);
FbxAnimCurveNode* CreateCurveNode(FbxAnimLayer* pAnimLayer);
FbxAnimCurveNode* GetCurveNode(bool pCreate=false);
FbxAnimCurveNode* GetCurveNode(FbxAnimStack* pAnimStack, bool pCreate=false);
FbxAnimCurveNode* GetCurveNode(FbxAnimLayer* pAnimLayer, bool pCreate=false);
inline FbxAnimCurve* GetCurve(FbxAnimLayer* pAnimLayer, bool pCreate=false)
{
return GetCurve(pAnimLayer, GetName(), NULL, pCreate);
}
inline FbxAnimCurve* GetCurve(FbxAnimLayer* pAnimLayer, const char* pChannel, bool pCreate=false)
{
return GetCurve(pAnimLayer, GetName(), pChannel, pCreate);
}
FbxAnimCurve* GetCurve(FbxAnimLayer* pAnimLayer, const char* pName, const char* pChannel, bool pCreate);
bool ConnectSrcObject(FbxObject* pObject, FbxConnection::EType pType=FbxConnection::eNone);
bool IsConnectedSrcObject(const FbxObject* pObject) const;
bool DisconnectSrcObject(FbxObject* pObject);
bool DisconnectAllSrcObject();
bool DisconnectAllSrcObject(const FbxCriteria& pCriteria);
int  GetSrcObjectCount() const;
int GetSrcObjectCount(const FbxCriteria& pCriteria) const;
FbxObject* GetSrcObject(const int pIndex=0) const;
FbxObject* GetSrcObject(const FbxCriteria& pCriteria, const int pIndex=0) const;
FbxObject* FindSrcObject(const char* pName, const int pStartIndex=0) const;
FbxObject* FindSrcObject(const FbxCriteria& pCriteria, const char* pName, const int pStartIndex=0) const;
template <class T> inline bool DisconnectAllSrcObject(){ return DisconnectAllSrcObject(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline bool DisconnectAllSrcObject(const FbxCriteria& pCriteria){ return DisconnectAllSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline int GetSrcObjectCount() const { return GetSrcObjectCount(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline int GetSrcObjectCount(const FbxCriteria& pCriteria) const { return GetSrcObjectCount(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline T* GetSrcObject(const int pIndex=0) const { return (T*)GetSrcObject(FbxCriteria::ObjectType(T::ClassId), pIndex); }
template <class T> inline T* GetSrcObject(const FbxCriteria& pCriteria, const int pIndex=0) const { return (T*)GetSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pIndex); }
template <class T> inline T* FindSrcObject(const char* pName, const int pStartIndex=0) const { return (T*)FindSrcObject(FbxCriteria::ObjectType(T::ClassId), pName, pStartIndex); }
template <class T> inline T* FindSrcObject(const FbxCriteria& pCriteria, const char* pName, const int pStartIndex=0) const { return (T*)FindSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pName, pStartIndex); }
bool ConnectDstObject(FbxObject* pObject, FbxConnection::EType pType=FbxConnection::eNone);
bool IsConnectedDstObject(const FbxObject* pObject) const;
bool DisconnectDstObject(FbxObject* pObject);
bool DisconnectAllDstObject();
bool DisconnectAllDstObject(const FbxCriteria& pCriteria);
int GetDstObjectCount() const;
int GetDstObjectCount(const FbxCriteria& pCriteria) const;
FbxObject* GetDstObject(const int pIndex=0) const;
FbxObject* GetDstObject(const FbxCriteria& pCriteria, const int pIndex=0) const;
FbxObject* FindDstObject(const char* pName, const int pStartIndex=0) const;
FbxObject* FindDstObject(const FbxCriteria& pCriteria, const char* pName, const int pStartIndex=0) const;
template <class T> inline bool DisconnectAllDstObject(){ return DisconnectAllDstObject(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline bool DisconnectAllDstObject(const FbxCriteria& pCriteria){ return DisconnectAllDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline int GetDstObjectCount() const { return GetDstObjectCount(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline int GetDstObjectCount(const FbxCriteria& pCriteria) const { return GetDstObjectCount(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline T* GetDstObject(const int pIndex=0) const { return (T*)GetDstObject(FbxCriteria::ObjectType(T::ClassId), pIndex); }
template <class T> inline T* GetDstObject(const FbxCriteria& pCriteria, const int pIndex=0) const { return (T*)GetDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pIndex); }
template <class T> inline T* FindDstObject(const char* pName, const int pStartIndex=0) const { return (T*)FindDstObject(FbxCriteria::ObjectType(T::ClassId), pName, pStartIndex); }
template <class T> inline T* FindDstObject(const FbxCriteria& pCriteria, const char* pName, const int pStartIndex=0) const { return (T*)FindDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pName, pStartIndex); }
bool ConnectSrcProperty(const FbxProperty& pProperty);
bool IsConnectedSrcProperty(const FbxProperty& pProperty);
bool DisconnectSrcProperty(const FbxProperty& pProperty);
int GetSrcPropertyCount() const;
bool ConnectDstProperty(const FbxProperty&  pProperty);
bool IsConnectedDstProperty(const FbxProperty& pProperty);
bool DisconnectDstProperty(const FbxProperty& pProperty);
int GetDstPropertyCount() const;
void ClearConnectCache();
FbxProperty GetSrcProperty(const int pIndex=0) const;
FbxProperty FindSrcProperty(const char* pName, const int pStartIndex=0) const;
FbxProperty GetDstProperty(const int pIndex=0) const;
FbxProperty FindDstProperty(const char* pName, const int pStartIndex=0) const;
static const char* sHierarchicalSeparator;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
FbxProperty(FbxObject* pObject, const char* pName, const FbxDataType& pDataType=FbxDataType(), const char* pLabel="");
FbxProperty(const FbxProperty& pParent, const char* pName, const FbxDataType& pDataType, const char* pLabel);
bool Set(const void* pValue, const EFbxType& pValueType, bool pCheckForValueEquality=true);
bool Get(void* pValue, const EFbxType& pValueType) const;
bool NotifySetRequest();
bool NotifySet();
bool NotifyGet() const;
private:
inline void* Get() const { FBX_ASSERT_NOW("Cannot get property value as void!"); return NULL; }
inline bool  Set(const void* &){ FBX_ASSERT_NOW("Cannot set property value as void!"); return false; }
bool   ConnectSrc(const FbxProperty& pProperty, FbxConnection::EType pType=FbxConnection::eNone);
bool   DisconnectSrc(const FbxProperty& pProperty);
bool   DisconnectAllSrc();
bool   DisconnectAllSrc(const FbxCriteria& pCriteria);
bool   IsConnectedSrc(const FbxProperty& pProperty) const;
int    GetSrcCount() const;
int    GetSrcCount(const FbxCriteria& pCriteria) const;
FbxProperty  GetSrc(int pIndex=0) const;
FbxProperty  GetSrc(const FbxCriteria& pCriteria, int pIndex=0) const;
FbxProperty  FindSrc(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const;
bool   ConnectDst(const FbxProperty& pProperty, FbxConnection::EType pType=FbxConnection::eNone);
bool   DisconnectDst(const FbxProperty& pProperty);
bool   DisconnectAllDst();
bool   DisconnectAllDst(const FbxCriteria& pCriteria);
bool   IsConnectedDst(const FbxProperty& pProperty) const;
int    GetDstCount() const;
int    GetDstCount(const FbxCriteria& pCriteria) const;
FbxProperty  GetDst(int pIndex=0) const;
FbxProperty  GetDst(const FbxCriteria& pCriteria, int pIndex=0) const;
FbxProperty  FindDst(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const;
mutable FbxPropertyHandle mPropertyHandle;
friend class FbxObject;
friend class FbxIOSettings;
friend class FbxBindingOperator;
friend class FbxAnimEvalClassic;
friend void FbxMarkObject(FbxObject* pObject, FbxMap<FbxObject*, int>& pObjectDstDisconnectCount, FbxSet<FbxObject*>& pObjectsToDeleted, FbxArray<FbxObject*>& pObjectToDeletedInSequence);
friend void FbxCleanUpConnectionsAtDestructionBoundary(FbxScene* pObject, FbxArray<FbxObject*>& pObjectToDeletedInSequence);
#endif
};
template <class T> class FbxPropertyT : public FbxProperty
{
public:
FbxProperty& StaticInit(FbxObject* pObject, const char* pName, const T& pValue, bool pForceSet, FbxPropertyFlags::EFlags pFlags=FbxPropertyFlags::eNone)
{
return StaticInit(pObject, pName, FbxGetDataTypeFromEnum(FbxTypeOf(*((T*)0))), pValue, pForceSet, pFlags);
}
FbxProperty& StaticInit(FbxObject* pObject, const char* pName, const FbxDataType& pDataType, const T& pValue, bool pForceSet, FbxPropertyFlags::EFlags pFlags=FbxPropertyFlags::eNone)
{
bool lWasFound = false;
*this = Create(pObject, pDataType, pName, "", true, &lWasFound);
if( pForceSet || !lWasFound )
{
ModifyFlag(pFlags, true);
FbxProperty::Set(&pValue, FbxTypeOf(pValue), false);
}
ModifyFlag(FbxPropertyFlags::eStatic, true);
return *this;
}
FbxProperty& StaticInit(FbxProperty pCompound, const char* pName, const FbxDataType& pDataType, const T& pValue, bool pForceSet=true, FbxPropertyFlags::EFlags pFlags=FbxPropertyFlags::eNone)
{
bool lWasFound = false;
*this = Create(pCompound, pDataType, pName, "", true, &lWasFound);
if( pForceSet || !lWasFound )
{
ModifyFlag(pFlags, true);
FbxProperty::Set(&pValue, FbxTypeOf(pValue), false);
}
ModifyFlag(FbxPropertyFlags::eStatic, true);
return *this;
}
FbxPropertyT& Set(const T& pValue){ FbxProperty::Set(&pValue, FbxTypeOf(pValue)); return *this; }
T Get() const { T lValue; FbxProperty::Get(&lValue, FbxTypeOf(lValue)); return lValue; }
FbxPropertyT& operator=(const T& pValue){ return Set(pValue); }
operator T() const { return Get(); }
T EvaluateValue(const FbxTime& pTime=FBXSDK_TIME_INFINITE, bool pForceEval=false)
{
return GetAnimationEvaluator()-> template GetPropertyValue<T>(*this, pTime, pForceEval);
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxPropertyT() : FbxProperty(){}
FbxPropertyT(const FbxProperty& pProperty) : FbxProperty(pProperty){}
~FbxPropertyT(){}
#endif
};
#ifndef DOXYGEN_SHOULD_SKIP_THIS
template <> class FbxPropertyT<FbxReference> : public FbxProperty
{
public:
FbxPropertyT() : FbxProperty(){}
FbxPropertyT(const FbxProperty& pProperty) : FbxProperty(pProperty){}
~FbxPropertyT(){}
const FbxProperty& StaticInit(FbxObject* pObject, const char* pName, const FbxReference& pValue, bool pForceSet, FbxPropertyFlags::EFlags pFlags=FbxPropertyFlags::eNone)
{
return StaticInit(pObject, pName, FbxGetDataTypeFromEnum(FbxTypeOf(*((FbxReference*)0))), pValue, pForceSet, pFlags);
}
const FbxProperty& StaticInit(FbxObject* pObject, const char* pName, const FbxDataType& pDataType, const FbxReference& pValue, bool pForceSet, FbxPropertyFlags::EFlags pFlags=FbxPropertyFlags::eNone)
{
bool lWasFound = false;
*this = Create(pObject, pDataType, pName, "", true, &lWasFound);
if( pForceSet || !lWasFound )
{
ModifyFlag(pFlags, true);
Set(pValue);
}
ModifyFlag(FbxPropertyFlags::eStatic, true);
return *this;
}
FbxReference Get() const
{
FbxProperty::NotifyGet();
return GetSrcObject();
}
FbxPropertyT& Set(const FbxReference& pValue)
{
if( FbxProperty::NotifySetRequest() )
{
DisconnectAllSrcObject();
if( ConnectSrcObject(pValue) )
{
FbxProperty::SetValueInheritType(FbxPropertyFlags::eOverride);
FbxProperty::NotifySet();
}
}
return *this;
}
operator FbxReference() const
{
return Get();
}
FbxPropertyT& operator=(const FbxReference& pValue)
{
return Set(pValue);
}
};
#endif
#endif