#ifndef _FBXSDK_SCENE_LIBRARY_H_
#define _FBXSDK_SCENE_LIBRARY_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxdocument.h>
#include <fbxsdk/scene/fbxobjectfilter.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxLocalizationManager
class FbxCriteria
class FBXSDK_DLL FbxLibrary : public FbxDocument
    FBXSDK_OBJECT_DECLARE(FbxLibrary, FbxDocument)
public:
    FbxLibrary* GetParentLibrary(void) const
    void SystemLibrary(bool pSystemLibrary)
    bool IsSystemLibrary() const
    void LocalizationBaseNamePrefix(const char* pPrefix)
    FbxString LocalizationBaseNamePrefix() const
    bool AddSubLibrary(FbxLibrary* pSubLibrary)
    bool RemoveSubLibrary(FbxLibrary* pSubLibrary)
    int GetSubLibraryCount(void) const
    FbxLibrary* GetSubLibrary(int pIndex) const
    FbxObject* CloneAsset( FbxObject* pToClone, FbxObject* pOptionalDestinationContainer = NULL) const
    static FbxCriteria GetAssetCriteriaFilter()
    static FbxCriteria GetAssetDependentsFilter()
    bool ImportAssets(FbxLibrary* pSrcLibrary)
    bool ImportAssets(FbxLibrary* pSrcLibrary, const FbxCriteria& pAssetFilter)
    template < class T > T* InstantiateMember( const T* pFBX_TYPE, const FbxObjectFilter& pFilter, bool pRecurse = true, FbxObject* pOptContainer = NULL)
    FbxLocalizationManager& GetLocalizationManager() const
    virtual const char* Localize( const char* pID, const char* pDefault = NULL ) const
    bool AddShadingObject(FbxObject* pShadingObject)
    bool RemoveShadingObject(FbxObject* pShadingObject)
    int GetShadingObjectCount(void) const
    FbxObject* GetShadingObject(int pIndex) const
    int GetShadingObjectCount(const FbxImplementationFilter& pCriteria) const
    FbxObject* GetShadingObject(int pIndex, const FbxImplementationFilter& pCriteria) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	mutable FbxLocalizationManager* mLocalizationManager
#endif 
#ifndef DOXYGEN_SHOULD_SKIP_THIS
template <class T> T* FbxLibrary::InstantiateMember(const T* pFBX_TYPE, const FbxObjectFilter& pFilter, bool pRecurse, FbxObject* pOptContainer)
	for( int i = 0
 i < GetMemberCount<T>()
 ++i )
		T* lObject = GetMember<T>(i)
		if( pFilter.Match(lObject) )
			return FbxCast<T>(CloneAsset(lObject,pOptContainer))
	if( pRecurse )
		for( int i = 0
 i < GetMemberCount<FbxLibrary>()
 ++i )
			FbxLibrary* lLibrary = GetMember<FbxLibrary>(i)
			T* lClonedObject = lLibrary->InstantiateMember(pFBX_TYPE, pFilter, pRecurse, pOptContainer)
			if( lClonedObject )
				return lClonedObject
	return NULL
class FBXSDK_DLL FbxEventPopulateSystemLibrary : public FbxEvent<FbxEventPopulateSystemLibrary>
	FBXSDK_EVENT_DECLARE(FbxEventPopulateSystemLibrary)
public:			
	FbxEventPopulateSystemLibrary(FbxLibrary* pLibrary) 
 mLibrary = pLibrary
	inline FbxLibrary* GetLibrary() const 
 return mLibrary
private:
	FbxLibrary* mLibrary
class FBXSDK_DLL FbxEventUpdateSystemLibrary : public FbxEvent<FbxEventUpdateSystemLibrary>
	FBXSDK_EVENT_DECLARE(FbxEventUpdateSystemLibrary)
public:
	FbxEventUpdateSystemLibrary(FbxLibrary *pLibrary) 
 mLibrary = pLibrary
	inline FbxLibrary* GetLibrary() const 
 return mLibrary
private:
	FbxLibrary* mLibrary
class FBXSDK_DLL FbxEventWriteLocalization : public FbxEvent<FbxEventWriteLocalization>
	FBXSDK_EVENT_DECLARE(FbxEventWriteLocalization)
public:
	FbxEventWriteLocalization(FbxLibrary* pAssetLibrary) 
 mAssetLibrary = pAssetLibrary
	inline FbxLibrary* GetLibrary() const 
 return mAssetLibrary
private:
	FbxLibrary* mAssetLibrary
class FBXSDK_DLL FbxEventMapAssetFileToAssetObject : public FbxEvent<FbxEventMapAssetFileToAssetObject>
	FBXSDK_EVENT_DECLARE(FbxEventMapAssetFileToAssetObject)
public:
	FbxEventMapAssetFileToAssetObject(const char* pFile) :
		mAsset(NULL),
		mFilePath( pFile )
	inline const char* GetFilePath() const 
 return mFilePath
	mutable FbxObject* mAsset
private:
	FbxString mFilePath
#endif 