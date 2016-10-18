#ifndef _FBXSDK_UTILS_EMBEDDED_FILES_ACCUMULATOR_H_
#define _FBXSDK_UTILS_EMBEDDED_FILES_ACCUMULATOR_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/utils/fbxprocessor.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxEmbeddedFilesAccumulator : public FbxProcessor
public:
    struct PropertyUrlIndex
        FbxString mPropName
        int     mIndex
        PropertyUrlIndex() : mIndex(0)
        PropertyUrlIndex(const FbxString& pUrl, int pIndex)
            : mPropName(pUrl)
            , mIndex(pIndex)
    struct FbxPropertyUrlIndexCompare
        inline int operator()(const PropertyUrlIndex& pKeyA, const PropertyUrlIndex& pKeyB) const
            if( pKeyA.mPropName < pKeyB.mPropName ) return -1
            if( pKeyB.mPropName < pKeyA.mPropName ) return 1
            if( pKeyA.mIndex < pKeyB.mIndex ) return -1
            if( pKeyB.mIndex < pKeyA.mIndex ) return 1
            return 0
    typedef FbxSet<PropertyUrlIndex, FbxPropertyUrlIndexCompare> PropertyUrlIndexSet
    typedef FbxMap<FbxObject*, PropertyUrlIndexSet> ObjectPropertyMap
    struct EmbeddedFileInfo
        FbxString                 mOriginalPropertyUrl
        ObjectPropertyMap   mConsumers
    typedef FbxMap<FbxString, EmbeddedFileInfo>     EmbeddedFilesMap
    EmbeddedFilesMap   mEmbeddedFiles
public:
    FbxEmbeddedFilesAccumulator(FbxManager& pManager, const char* pName, FbxSet<FbxString>& pPropertyFilter)
    virtual ~FbxEmbeddedFilesAccumulator()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    virtual bool internal_ProcessObject(FbxObject* pObject)
    FbxSet<FbxString>   mPropertyFilter
#endif 