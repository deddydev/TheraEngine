#ifndef _FBXSDK_SCENE_ANIMATION_STACK_H_
#define _FBXSDK_SCENE_ANIMATION_STACK_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxtime.h>
#include <fbxsdk/scene/fbxcollection.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
#define FBXSDK_TAKENODE_DEFAULT_NAME	"Default"
#define FBXSDK_ROOTCURVE_DEFAULT_NAME	"Defaults"
class FbxTakeInfo
class FbxThumbnail
class FbxAnimEvaluator
class FBXSDK_DLL FbxAnimStack : public FbxCollection
    FBXSDK_OBJECT_DECLARE(FbxAnimStack, FbxCollection)
public:
    FbxPropertyT<FbxString> Description
    FbxPropertyT<FbxTime> LocalStart
    FbxPropertyT<FbxTime> LocalStop
    FbxPropertyT<FbxTime> ReferenceStart
    FbxPropertyT<FbxTime> ReferenceStop
    void Reset(const FbxTakeInfo* pTakeInfo = NULL)
        FbxTimeSpan GetLocalTimeSpan() const
        void SetLocalTimeSpan(FbxTimeSpan& pTimeSpan)
        FbxTimeSpan GetReferenceTimeSpan() const
        void SetReferenceTimeSpan(FbxTimeSpan& pTimeSpan)
        bool BakeLayers(FbxAnimEvaluator* pEvaluator, FbxTime pStart, FbxTime pStop, FbxTime pPeriod)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
#endif 