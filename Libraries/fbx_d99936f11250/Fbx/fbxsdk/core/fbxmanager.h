#ifndef _FBXSDK_CORE_MANAGER_H_
#define _FBXSDK_CORE_MANAGER_H_
class FbxIOSettings;
class FbxIOPluginRegistry;
class FbxAnimEvaluator;
class FbxSceneReference;
class FbxUserNotification;
class FbxMessageEmitter;
class FbxLocalizationManager;
class FbxXRefManager;
class FbxManager_internal;
#ifndef FBXSDK_ENV_WINSTORE
class FbxPlugin;
#endif
class FBXSDK_DLL FbxManager
{
public:
static FbxManager* Create();
virtual void Destroy();
static const char* GetVersion(bool pFull=true);
static void GetFileFormatVersion(int& pMajor, int& pMinor, int& pRevision);
template <typename T1, typename T2> inline FbxClassId RegisterFbxClass(const char* pName, const T1*  , const T2*  , const char* pFbxFileTypeName=0, const char* pFbxFileSubTypeName=0)
{
T1::ClassId = Internal_RegisterFbxClass(pName, T2::ClassId, (FbxObjectCreateProc)T1::Allocate, pFbxFileTypeName, pFbxFileSubTypeName);
return T1::ClassId;
}
template <typename T> inline FbxClassId RegisterRuntimeFbxClass(const char* pName, const T*  , const char* pFbxFileTypeName=0,const char* pFbxFileSubTypeName=0)
{
return Internal_RegisterFbxClass(pName, T::ClassId, (FbxObjectCreateProc)T::Allocate, pFbxFileTypeName, pFbxFileSubTypeName);
}
inline void UnregisterRuntimeFbxClass(const char* pName)
{
FbxClassId lClassId = FindClass(pName);
if( !(lClassId == FbxClassId()) )
{
Internal_UnregisterFbxClass(lClassId);
}
}
template <typename T1,typename T2> inline FbxClassId OverrideFbxClass(const T1* pFBX_TYPE_Class, const T2* pFBX_TYPE_OverridenClass)
{
T1::ClassId  = Internal_OverrideFbxClass(T2::ClassId,(FbxObjectCreateProc)T1::Allocate );
return T1::ClassId;
}
FbxObject* CreateNewObjectFromClassId(FbxClassId pClassId, const char* pName, FbxObject* pContainer=NULL, const FbxObject* pCloneFrom=NULL);
FbxClassId FindClass(const char* pClassName) const;
FbxClassId FindFbxFileClass(const char* pFbxFileTypeName, const char* pFbxFileSubTypeName) const;
template <typename T> inline void UnregisterFbxClass(const T* pFBX_TYPE_Class)
{
Internal_UnregisterFbxClass(T::ClassId);
T::ClassId = FbxClassId();
}
FbxDataType CreateDataType(const char* pName, const EFbxType pType);
int GetDataTypeCount() const;
FbxDataType& GetDataType(const int pIndex) const;
FbxDataType& GetDataTypeFromName(const char* pDataType) const;
FbxUserNotification* GetUserNotification() const;
void SetUserNotification(FbxUserNotification* pUN);
virtual FbxIOSettings* GetIOSettings() const;
virtual void SetIOSettings(FbxIOSettings* pIOSettings);
FbxMessageEmitter& GetMessageEmitter();
bool SetMessageEmitter(FbxMessageEmitter* pMessageEmitter);
void AddLocalization(FbxLocalizationManager* pLocManager);
void RemoveLocalization(FbxLocalizationManager* pLocManager);
bool SetLocale(const char* pLocale);
const char* Localize(const char* pID, const char* pDefault=NULL) const;
FbxXRefManager& GetXRefManager();
FbxLibrary* GetRootLibrary() const;
FbxLibrary* GetSystemLibraries() const;
FbxLibrary* GetUserLibraries() const;
FbxIOPluginRegistry* GetIOPluginRegistry() const;
#ifndef FBXSDK_ENV_WINSTORE
bool LoadPluginsDirectory(const char* pFilename, const char* pExtensions=NULL);
bool LoadPlugin(const char* pFilename);
bool UnloadPlugins();
bool EmitPluginsEvent(const FbxEventBase& pEvent);
FbxArray<const FbxPlugin*> GetPlugins() const;
int GetPluginCount() const;
FbxPlugin* FindPlugin(const char* pName, const char* pVersion) const;
#endif
void FillIOSettingsForReadersRegistered(FbxIOSettings& pIOS);
void FillIOSettingsForWritersRegistered(FbxIOSettings& pIOS);
void FillCommonIOSettings(FbxIOSettings& pIOS, bool pImport);
void RegisterObject(FbxObject* pObject);
void UnregisterObject(FbxObject* pObject);
void RegisterObjects(const FbxArray<FbxObject*>& pArray);
void UnregisterObjects(const FbxArray<FbxObject*>& pArray);
void IncreaseDestroyingSceneFlag();
void DecreaseDestroyingSceneFlag();
int GetReferenceCount() const;
FbxSceneReference* GetReference(int pIndex) const;
int AddReference(FbxSceneReference* pReference);
bool RemoveReference(FbxSceneReference* pReference);
bool ClearReference(FbxSceneReference* pReference);
static FbxString PrefixName(const char* pPrefix, const char* pName);
int GetDocumentCount();
FbxDocument* GetDocument(int pIndex);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
static FbxManager* GetDefaultManager();
void    CreateMissingBindPoses(FbxScene* pScene);
int     GetBindPoseCount(FbxScene *pScene) const;
int     GetFbxClassCount() const;
FbxClassId   GetNextFbxClass(FbxClassId pClassId  ) const;
#endif
};
#endif