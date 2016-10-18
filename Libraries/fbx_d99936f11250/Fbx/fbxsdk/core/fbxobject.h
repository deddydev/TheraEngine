#ifndef _FBXSDK_CORE_OBJECT_H_
#define _FBXSDK_CORE_OBJECT_H_
class FbxManager;
class FbxDocument;
class FbxImplementation;
class FbxImplementationFilter;
class FbxLibrary;
class FbxMessage;
class FbxPeripheral;
class FbxUserDataRecord;
class FbxConnectEvent;
#define FBXSDK_CLASS_DECLARE(Class, Parent)\
private:\
Class(const Class&);\
Class& operator=(const Class&);\
protected:\
virtual ~Class(){};\
public:\
static FbxClassId ClassId;\
virtual FbxClassId GetClassId() const { return ClassId; }\
friend class FBXSDK_NAMESPACE::FbxManager;\
typedef Parent ParentClass;\
static Class* Create(FbxManager* pManager, const char* pName);\
#define FBXSDK_FBXOBJECT_DECLARE(Class, Parent)\
FBXSDK_CLASS_DECLARE(Class, Parent)\
FBXSDK_FRIEND_NEW()\
static Class* Create(FbxObject* pContainer, const char* pName);\
protected:\
static Class* Allocate(FbxManager* pManager, const char* pName, const Class* pFrom);\
#define FBXSDK_OBJECT_DECLARE(Class, Parent)\
FBXSDK_FBXOBJECT_DECLARE(Class, Parent)\
protected:\
Class(FbxManager& pManager, const char* pName) : Parent(pManager, pName){};\
private:  \
#define FBXSDK_ABSTRACT_OBJECT_DECLARE(Class, Parent)\
FBXSDK_CLASS_DECLARE(Class, Parent)\
protected:\
static FbxObjectCreateProc Allocate;\
Class(FbxManager& pManager, const char* pName) : Parent(pManager, pName){};\
private:  \
#define FBXSDK_OBJECT_IMPLEMENT(Class)\
FbxClassId Class::ClassId;\
Class* Class::Create(FbxManager* pManager, const char* pName)\
{\
return (Class*)pManager->CreateNewObjectFromClassId(Class::ClassId, pName);\
}\
Class* Class::Create(FbxObject* pContainer, const char* pName)\
{\
FBX_ASSERT_RETURN_VALUE(pContainer && pContainer->GetFbxManager(), NULL);\
return (Class*)pContainer->GetFbxManager()->CreateNewObjectFromClassId(Class::ClassId, pName, pContainer);\
}\
Class* Class::Allocate(FbxManager* pManager, const char* pName, const Class* pFrom)\
{\
Class* lNewObject = FbxNew<Class>(*pManager, pName);\
lNewObject->Construct(pFrom);\
lNewObject->SetObjectFlags(FbxObject::eInitialized, true);\
return lNewObject;\
}\
#define FBXSDK_ABSTRACT_OBJECT_IMPLEMENT(Class)\
FbxClassId Class::ClassId;\
FbxObjectCreateProc Class::Allocate = 0;\
Class* Class::Create(FbxManager* pManager, const char* pName)\
{\
return (Class*)pManager->CreateNewObjectFromClassId(Class::ClassId, pName);\
}\
class FBXSDK_DLL FbxObject : public FbxEmitter
{
FBXSDK_FBXOBJECT_DECLARE(FbxObject, FbxEmitter);
public:
template <class T> inline bool Is() const { return GetClassId().Is(T::ClassId); }
FbxManager* GetFbxManager() const;
FbxDocument* GetDocument() const;
FbxDocument* GetRootDocument() const;
FbxScene* GetScene() const;
void Destroy(bool pRecursive=false);
void ResetProperties();
enum EObjectFlag
{
eNone = 0,
eInitialized = 1 << 0,
eSystem = 1 << 1,
eSavable = 1 << 2,
eSelected = 1 << 3,
eHidden = 1 << 4,
eContentLoaded = 1 << 5,
eDontLocalize = 1 << 6,
eCopyCalledByClone = 1 << 16
};
void SetObjectFlags(EObjectFlag pFlags, bool pValue);
bool GetObjectFlags(EObjectFlag pFlags) const;
void SetAllObjectFlags(FbxUInt pFlags);
FbxUInt GetAllObjectFlags() const;
virtual FbxObject& Copy(const FbxObject& pObject);
enum ECloneType
{
eDeepClone,
eReferenceClone
};
virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const;
bool IsAReferenceTo() const;
FbxObject* GetReferenceTo() const;
bool IsReferencedBy() const;
int GetReferencedByCount() const;
FbxObject* GetReferencedBy(int pIndex) const;
void SetName(const char* pName);
const char* GetName() const;
FbxString GetNameWithoutNameSpacePrefix() const;
FbxString GetNameWithNameSpacePrefix() const;
void SetInitialName(const char* pName);
const char* GetInitialName() const;
FbxString GetNameSpaceOnly();
void SetNameSpace(FbxString pNameSpace);
FbxArray<FbxString*> GetNameSpaceArray(char identifier);
FbxString GetNameOnly() const;
FbxString GetNameSpacePrefix() const;
static FbxString RemovePrefix(char* pName);
static FbxString StripPrefix(FbxString& lName);
static FbxString StripPrefix(const char* pName);
const FbxUInt64& GetUniqueID() const;
virtual bool GetSelected();
virtual void SetSelected(bool pSelected);
void SetUserDataPtr(const FbxUInt64& pUserID, void* pUserData);
void* GetUserDataPtr(const FbxUInt64& pUserID) const;
inline void SetUserDataPtr(void* pUserData){ SetUserDataPtr(GetUniqueID(), pUserData); }
inline void* GetUserDataPtr() const { return GetUserDataPtr(GetUniqueID()); }
inline bool ConnectSrcObject(FbxObject* pObject, FbxConnection::EType pType=FbxConnection::eNone) { return RootProperty.ConnectSrcObject(pObject,pType); }
inline bool IsConnectedSrcObject(const FbxObject* pObject) const { return RootProperty.IsConnectedSrcObject(pObject); }
inline bool DisconnectSrcObject(FbxObject* pObject){ return RootProperty.DisconnectSrcObject(pObject); }
inline bool DisconnectAllSrcObject() { return RootProperty.DisconnectAllSrcObject(); }
inline bool DisconnectAllSrcObject(const FbxCriteria& pCriteria) { return RootProperty.DisconnectAllSrcObject(pCriteria); }
inline int GetSrcObjectCount() const { return RootProperty.GetSrcObjectCount(); }
inline int GetSrcObjectCount(const FbxCriteria& pCriteria) const { return RootProperty.GetSrcObjectCount(pCriteria); }
inline FbxObject* GetSrcObject(int pIndex=0) const { return RootProperty.GetSrcObject(pIndex); }
inline FbxObject* GetSrcObject(const FbxCriteria& pCriteria, int pIndex=0) const { return RootProperty.GetSrcObject(pCriteria,pIndex); }
inline FbxObject* FindSrcObject(const char* pName, int pStartIndex=0) const { return RootProperty.FindSrcObject(pName,pStartIndex); }
inline FbxObject* FindSrcObject(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const { return RootProperty.FindSrcObject(pCriteria,pName,pStartIndex); }
template <class T> inline bool DisconnectAllSrcObject() { return RootProperty.DisconnectAllSrcObject(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline bool DisconnectAllSrcObject(const FbxCriteria& pCriteria) { return RootProperty.DisconnectAllSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline int GetSrcObjectCount() const { return RootProperty.GetSrcObjectCount(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline int GetSrcObjectCount(const FbxCriteria& pCriteria) const { return RootProperty.GetSrcObjectCount(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline T* GetSrcObject(int pIndex=0) const { return (T*)RootProperty.GetSrcObject(FbxCriteria::ObjectType(T::ClassId), pIndex); }
template <class T> inline T* GetSrcObject(const FbxCriteria& pCriteria, int pIndex=0) const { return (T*)RootProperty.GetSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pIndex); }
template <class T> inline T* FindSrcObject(const char* pName, int pStartIndex=0) const { return (T*)RootProperty.FindSrcObject(FbxCriteria::ObjectType(T::ClassId), pName, pStartIndex); }
template <class T> inline T* FindSrcObject(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const { return (T*)RootProperty.FindSrcObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pName, pStartIndex); }
inline bool ConnectDstObject(FbxObject* pObject, FbxConnection::EType pType=FbxConnection::eNone) { return RootProperty.ConnectDstObject(pObject,pType); }
inline bool IsConnectedDstObject(const FbxObject* pObject) const { return RootProperty.IsConnectedDstObject(pObject); }
inline bool DisconnectDstObject(FbxObject* pObject) { return RootProperty.DisconnectDstObject(pObject); }
inline bool DisconnectAllDstObject() { return RootProperty.DisconnectAllDstObject(); }
inline bool DisconnectAllDstObject(const FbxCriteria& pCriteria) { return RootProperty.DisconnectAllDstObject(pCriteria); }
inline int GetDstObjectCount() const { return RootProperty.GetDstObjectCount(); }
inline int GetDstObjectCount(const FbxCriteria& pCriteria) const { return RootProperty.GetDstObjectCount(pCriteria); }
inline FbxObject* GetDstObject(int pIndex=0) const { return RootProperty.GetDstObject(pIndex); }
inline FbxObject* GetDstObject(const FbxCriteria& pCriteria, int pIndex=0) const { return RootProperty.GetDstObject(pCriteria,pIndex); }
inline FbxObject* FindDstObject(const char* pName, int pStartIndex=0) const { return RootProperty.FindDstObject(pName,pStartIndex); }
inline FbxObject* FindDstObject(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const { return RootProperty.FindDstObject(pCriteria,pName,pStartIndex); }
template <class T> inline bool DisconnectAllDstObject() { return RootProperty.DisconnectAllDstObject(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline bool DisconnectAllDstObject(const FbxCriteria& pCriteria) { return RootProperty.DisconnectAllDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline int GetDstObjectCount() const { return RootProperty.GetDstObjectCount(FbxCriteria::ObjectType(T::ClassId)); }
template <class T> inline int GetDstObjectCount(const FbxCriteria& pCriteria) const { return RootProperty.GetDstObjectCount(FbxCriteria::ObjectType(T::ClassId) && pCriteria); }
template <class T> inline T* GetDstObject(int pIndex=0) const { return (T*)RootProperty.GetDstObject(FbxCriteria::ObjectType(T::ClassId), pIndex); }
template <class T> inline T* GetDstObject(const FbxCriteria& pCriteria, int pIndex=0) const { return (T*)RootProperty.GetDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pIndex); }
template <class T> inline T* FindDstObject(const char* pName, int pStartIndex=0) const { return (T*)RootProperty.FindDstObject(FbxCriteria::ObjectType(T::ClassId), pName, pStartIndex); }
template <class T> inline T* FindDstObject(const FbxCriteria& pCriteria, const char* pName, int pStartIndex=0) const { return (T*)RootProperty.FindDstObject(FbxCriteria::ObjectType(T::ClassId) && pCriteria, pName, pStartIndex); }
inline FbxProperty GetFirstProperty() const
{
return RootProperty.GetFirstDescendent();
}
inline FbxProperty GetNextProperty(const FbxProperty& pProperty) const
{
return RootProperty.GetNextDescendent(pProperty);
}
inline FbxProperty FindProperty(const char* pName, bool pCaseSensitive = true) const
{
return RootProperty.Find(pName, pCaseSensitive );
}
inline FbxProperty FindProperty(const char* pName, const FbxDataType& pDataType, bool pCaseSensitive = true) const
{
return RootProperty.Find(pName, pDataType, pCaseSensitive );
}
inline FbxProperty FindPropertyHierarchical(const char* pName, bool pCaseSensitive = true) const
{
return RootProperty.FindHierarchical(pName, pCaseSensitive );
}
inline FbxProperty FindPropertyHierarchical(const char* pName, const FbxDataType& pDataType, bool pCaseSensitive = true) const
{
return RootProperty.FindHierarchical(pName, pDataType, pCaseSensitive );
}
FbxProperty GetClassRootProperty();
inline bool ConnectSrcProperty(const FbxProperty& pProperty) { return RootProperty.ConnectSrcProperty(pProperty); }
inline bool IsConnectedSrcProperty(const FbxProperty& pProperty) { return RootProperty.IsConnectedSrcProperty(pProperty); }
inline bool DisconnectSrcProperty(const FbxProperty& pProperty) { return RootProperty.DisconnectSrcProperty(pProperty); }
inline int GetSrcPropertyCount() const { return RootProperty.GetSrcPropertyCount(); }
inline FbxProperty GetSrcProperty(int pIndex=0) const { return RootProperty.GetSrcProperty(pIndex); }
inline FbxProperty FindSrcProperty(const char* pName,int pStartIndex=0) const { return RootProperty.FindSrcProperty(pName,pStartIndex); }
inline bool ConnectDstProperty(const FbxProperty& pProperty) { return RootProperty.ConnectDstProperty(pProperty); }
inline bool IsConnectedDstProperty(const FbxProperty& pProperty) { return RootProperty.IsConnectedDstProperty(pProperty); }
inline bool DisconnectDstProperty(const FbxProperty& pProperty) { return RootProperty.DisconnectDstProperty(pProperty); }
inline int GetDstPropertyCount() const { return RootProperty.GetDstPropertyCount(); }
inline FbxProperty GetDstProperty(int pIndex=0) const { return RootProperty.GetDstProperty(pIndex); }
inline FbxProperty FindDstProperty(const char* pName, int pStartIndex=0) const { return RootProperty.FindDstProperty(pName,pStartIndex); }
int ContentUnload();
int ContentLoad();
bool ContentIsLoaded() const;
void ContentDecrementLockCount();
void ContentIncrementLockCount();
bool ContentIsLocked() const;
virtual bool ContentWriteTo(FbxStream& pStream) const;
virtual bool ContentReadFrom(const FbxStream& pStream);
void EmitMessage(FbxMessage* pMessage) const;
virtual const char* Localize(const char* pID, const char* pDefault=NULL) const;
FbxLibrary* GetParentLibrary() const;
bool AddImplementation(FbxImplementation* pImplementation);
bool RemoveImplementation(FbxImplementation* pImplementation);
bool HasDefaultImplementation(void) const;
FbxImplementation* GetDefaultImplementation(void) const;
bool SetDefaultImplementation(FbxImplementation* pImplementation);
int GetImplementationCount(const FbxImplementationFilter* pCriteria=NULL) const;
FbxImplementation* GetImplementation(int pIndex, const FbxImplementationFilter* pCriteria=NULL) const;
virtual FbxString GetUrl() const;
virtual bool SetUrl(char* pUrl);
void SetRuntimeClassId(const FbxClassId& pClassId);
FbxClassId GetRuntimeClassId() const;
bool IsRuntime(const FbxClassId& pClassId) const;
bool IsRuntimePlug() const;
virtual void Compact();
FbxProperty RootProperty;
protected:
virtual void Construct(const FbxObject* pFrom);
virtual void ConstructProperties(bool pForceSet);
virtual void Destruct(bool pRecursive);
virtual void ContentClear();
virtual FbxPeripheral* GetPeripheral();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
public:
virtual bool Compare(FbxObject* pOtherObject);
bool operator==(const FbxObject& pObject);
bool operator!=(const FbxObject& pObject);
virtual void    SetDocument(FbxDocument* pDocument);
inline FbxPropertyHandle& GetPropertyHandle() { return RootProperty.mPropertyHandle; }
virtual const char*   GetTypeName() const;
virtual FbxStringList  GetTypeFlags() const;
void WipeAllConnections();
static void SetWipeMode(bool pState);
static bool GetWipeMode();
protected:
FbxObject(FbxManager& pManager, const char* pName);
enum EPropertyNotifyType
{
ePropertySetRequest,
ePropertySet,
ePropertyGet
};
virtual bool  ConnectNotify(const FbxConnectEvent& pEvent);
virtual bool  PropertyNotify(EPropertyNotifyType pType, FbxProperty& pProperty);
bool    Copyable(const FbxObject& pObject);
private:
void    CopyPropertiesFrom(const FbxObject& pFrom);
void    SetClassRootProperty(FbxProperty& lProperty);
int     GetFlatPropertyCount() const;
FbxNameHandler  mName;
FbxClassId   mRuntimeClassId;
FbxUserDataRecord* mUserData;
FbxManager*   mManager;
FbxImplementation* mDefaultImplementation;
FbxUInt64   mUniqueID;
FbxInt32   mObjectFlags;
FbxInt32   mContentLockCount;
FbxInt32   mUserDataCount;
static bool   mWipeMode;
friend class FbxProperty;
#endif
};
template<typename FbxProperty> class FbxIterator
{
public:
FbxIterator(const FbxObject* pObject) : mObject(pObject) {}
inline const FbxProperty& GetFirst() { mProperty = mObject->GetFirstProperty(); return mProperty; }
inline const FbxProperty& GetNext() { mProperty = mObject->GetNextProperty(mProperty); return mProperty; }
private:
FbxProperty        mProperty;
const FbxObject*   mObject;
};
class FbxIteratorSrcBase
{
public:
inline FbxIteratorSrcBase(FbxProperty& pProperty,FbxClassId pClassId) :
mProperty(pProperty),
mClassId(pClassId),
mSize(0),
mIndex(-1)
{
ResetToBegin();
}
inline FbxIteratorSrcBase(FbxObject* pObject,FbxClassId pClassId) :
mProperty(pObject->RootProperty),
mClassId(pClassId),
mSize(0),
mIndex(-1)
{
ResetToBegin();
}
inline FbxObject* GetFirst()
{
ResetToBegin();
return GetNext();
}
inline FbxObject* GetNext()
{
mIndex++;
return ((mIndex>=0) && (mIndex<mSize)) ? mProperty.GetSrcObject(FbxCriteria::ObjectType(mClassId), mIndex) : NULL;
}
inline FbxObject* GetSafeNext()
{
mSize = mProperty.GetSrcObjectCount(FbxCriteria::ObjectType(mClassId));
return GetNext();
}
inline FbxObject* GetLast()
{
ResetToEnd();
return GetPrevious();
}
inline FbxObject* GetPrevious()
{
mIndex--;
return ((mIndex>=0) && (mIndex<mSize)) ? mProperty.GetSrcObject(FbxCriteria::ObjectType(mClassId), mIndex) : NULL;
}
inline FbxObject* GetSafePrevious()
{
mSize = mProperty.GetSrcObjectCount(FbxCriteria::ObjectType(mClassId));
while (mIndex>mSize) mIndex--;
return GetPrevious();
}
}
}
};
inline FbxIteratorSrc(FbxObject* pObject) : FbxIteratorSrcBase(pObject,Type::ClassId) {}
inline FbxIteratorSrc(FbxProperty& pProperty) : FbxIteratorSrcBase(pProperty,Type::ClassId) {}
inline Type* GetFirst()         { return (Type*)FbxIteratorSrcBase::GetFirst(); }
inline Type* GetNext()          { return (Type*)FbxIteratorSrcBase::GetNext(); }
inline Type* GetSafeNext()      { return (Type*)FbxIteratorSrcBase::GetSafeNext(); }
inline Type* GetLast()          { return (Type*)FbxIteratorSrcBase::GetLast(); }
inline Type* GetPrevious()      { return (Type*)FbxIteratorSrcBase::GetPrevious(); }
inline Type* GetSafePrevious()  { return (Type*)FbxIteratorSrcBase::GetSafePrevious(); }
};
}
}
}
}
}
}
}
}
}
}
};
inline FbxIteratorDst(FbxObject* pObject) : FbxIteratorDstBase(pObject,Type::ClassId) {}
inline FbxIteratorDst(FbxProperty& pProperty) : FbxIteratorDstBase(pProperty,Type::ClassId) {}
inline Type* GetFirst()         { return (Type*)FbxIteratorDstBase::GetFirst(); }
inline Type* GetNext()          { return (Type*)FbxIteratorDstBase::GetNext(); }
inline Type* GetSafeNext()      { return (Type*)FbxIteratorDstBase::GetSafeNext(); }
inline Type* GetLast()          { return (Type*)FbxIteratorDstBase::GetLast(); }
inline Type* GetPrevious()      { return (Type*)FbxIteratorDstBase::GetPrevious(); }
inline Type* GetSafePrevious()  { return (Type*)FbxIteratorDstBase::GetSafePrevious(); }
};
#define FBX_TYPE(class) ((const class*)0)
}
}
#define FbxForEach(Iterator, Object) for((Object)=(Iterator).GetFirst();(Object)!=0;(Object)=(Iterator).GetNext())
#define FbxForEachReverse(Iterator, Object) for(Object=(Iterator).GetLast();(Object)!=0;Object=(Iterator).GetPrevious())
#ifndef DOXYGEN_SHOULD_SKIP_THIS
};
};
}
inline EType GetType() const { return mType; }
inline EDirection GetDirection() const { return mDirection; }
inline FbxProperty& GetSrc() const { return *mSrc;  }
inline FbxProperty& GetDst() const { return *mDst;  }
template <class T> inline T* GetSrcIfObject() const { return mSrc->IsRoot() ? FbxCast<T>(mSrc->GetFbxObject()) : (T*)0; }
template <class T> inline T* GetDstIfObject() const { return mDst->IsRoot() ? FbxCast<T>(mDst->GetFbxObject()) : (T*)0; }
};
FbxObjectPropertyChanged(FbxProperty pProp) : mProp(pProp) {}
};
#endif
#endif