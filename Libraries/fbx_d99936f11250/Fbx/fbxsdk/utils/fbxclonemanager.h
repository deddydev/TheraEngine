#ifndef _FBXSDK_UTILS_CLONE_MANAGER_H_
#define _FBXSDK_UTILS_CLONE_MANAGER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/core/fbxquery.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCloneManager
public:
    static const int sMaximumCloneDepth
    static const int sConnectToOriginal
    static const int sConnectToClone
    struct FBXSDK_DLL CloneSetElement
    public:
        CloneSetElement( int pSrcPolicy = 0,
                         int pExternalDstPolicy = 0,
                         FbxObject::ECloneType pCloneType = FbxObject::eReferenceClone )
        FbxObject::ECloneType mType
        int mSrcPolicy
        int mExternalDstPolicy
        FbxObject* mObjectClone
		bool mLayerElementProcessed
		bool mConnectionsProcessed
    typedef FbxMap<FbxObject*,CloneSetElement> CloneSet
    FbxCloneManager()
    virtual ~FbxCloneManager()
    static FbxObject* Clone(const FbxObject* pObject, FbxObject* pContainer = NULL)
    virtual bool Clone( CloneSet& pSet, FbxObject* pContainer = NULL ) const
    virtual void AddDependents( CloneSet& pSet,
                        const FbxObject* pObject,
                        const CloneSetElement& pCloneOptions = CloneSetElement(),
                        FbxCriteria pTypes = FbxCriteria::ObjectType(FbxObject::ClassId),
                        int pDepth = sMaximumCloneDepth ) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    static FbxObject* Clone(const FbxObject* pObject, CloneSet* pSet, FbxObject* pContainer = NULL)
private:
    friend class FbxScene
	bool ReAssignLayerElements( FbxCloneManager::CloneSet::RecordType* pIterator, const FbxCloneManager::CloneSet& pSet) const
	bool CloneConnections( CloneSet::RecordType* pIterator, const CloneSet& pSet) const
    bool CheckIfCloneOnSameScene(const FbxObject* pObject, FbxObject* pContainer) const
    virtual void LookForIndirectDependent(const FbxObject* pObject, CloneSet& pSet, FbxArray<FbxObject*>& lIndirectDepend)
    virtual bool NeedToBeExcluded(FbxObject* lObj) const
    bool      mCloneOnSameScene
#endif 