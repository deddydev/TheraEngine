#ifndef _FBXSDK_UTILS_PROCESSOR_XREF_H_
#define _FBXSDK_UTILS_PROCESSOR_XREF_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxmap.h>
#include <fbxsdk/utils/fbxprocessor.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxProcessorXRefCopy : public FbxProcessor
    FBXSDK_OBJECT_DECLARE(FbxProcessorXRefCopy, FbxProcessor)
public:
    class FBXSDK_DLL MissingUrlHandler
    public:
        virtual ~MissingUrlHandler()
        virtual void MissingUrl(const FbxString& pUrl, const FbxProperty&) = 0
        FbxPropertyT<FbxString>    OutputDirectory
        FbxPropertyT<FbxBool>     UpdateProperties
        FbxPropertyT<FbxBool>     TrackUpdatedProperties
        FbxPropertyT<FbxBool>     ForceCopy
        FbxPropertyT<FbxBool>     CopyFileTimes
        MissingUrlHandler*          MissingUrlHandler
    struct PropertyUpdate
        FbxProperty mProperty
        FbxString    mOriginalValue
        inline PropertyUpdate() 
        inline PropertyUpdate(const FbxProperty& pProp, const FbxString& pVal) :
            mProperty(pProp), mOriginalValue(pVal) 
        inline bool operator <(const PropertyUpdate& pOther) const
            return strcmp(mProperty.GetName(), pOther.mProperty.GetName()) < 0
    typedef FbxSet<PropertyUpdate>           UpdateSet
    typedef FbxMap<FbxObject*, UpdateSet>    PropertyUpdateMap
    PropertyUpdateMap& GetUpdatedProperties()
    void RevertPropertyChanges()
    struct FBXSDK_DLL AutoRevertPropertyChanges
        AutoRevertPropertyChanges(FbxProcessorXRefCopy* pCopy) : mXRefCopy(pCopy) 
        ~AutoRevertPropertyChanges()
            if( mXRefCopy )
                mXRefCopy->RevertPropertyChanges()
        FbxProcessorXRefCopy* mXRefCopy
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	PropertyUpdateMap mUpdatedProperties
	bool ShouldCopyFile(const FbxString& pTarget, const FbxString& pSource) const
	virtual bool	internal_ProcessCollectionBegin (FbxCollection*     pObject)
	virtual bool	internal_ProcessCollectionEnd   (FbxCollection*     pObject)
	virtual bool	internal_ProcessObject          (FbxObject*     pObject)
	bool			ProcessPathProperty(FbxProperty &pProperty)
	virtual bool	ValidPropertyForXRefCopy(FbxObject* pObject, FbxProperty& lProperty) const
#endif 