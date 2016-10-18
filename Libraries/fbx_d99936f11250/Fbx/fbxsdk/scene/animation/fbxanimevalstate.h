#ifndef _FBXSDK_SCENE_ANIMATION_EVALUATION_STATE_H_
#define _FBXSDK_SCENE_ANIMATION_EVALUATION_STATE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxtime.h>
#include <fbxsdk/core/fbxpropertydef.h>
#include <fbxsdk/scene/geometry/fbxnode.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxTransform
class FbxNodeEvalState
class FbxPropertyEvalState
typedef FbxMap<FbxNode*, FbxNodeEvalState*> FbxNodeEvalStateMap
typedef FbxMap<FbxProperty, FbxPropertyEvalState*> FbxPropertyEvalStateMap
typedef FbxMap<FbxAnimLayer*, FbxAnimCurveNode*> FbxAnimLayerCurveNodeMap
typedef FbxMap<FbxProperty, FbxAnimLayerCurveNodeMap*> FbxPropertyCurveNodeMap
class FBXSDK_DLL FbxAnimEvalState
public:
    FbxTime GetTime() const
    void Begin(const FbxTime& pTime)
	void Flush(FbxNode* pNode)
	void Flush(FbxProperty& pProperty)
	FbxNodeEvalState* GetNodeEvalState(FbxNode* pNode)
    FbxPropertyEvalState* GetPropertyEvalState(FbxProperty& pProperty)
	FbxAnimCurveNode* GetPropertyCurveNode(FbxProperty& pProperty, FbxAnimLayer* pAnimLayer)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxAnimEvalState()
    virtual ~FbxAnimEvalState()
private:
	FbxTime					mTime
	FbxNodeEvalStateMap		mNodeMap
	FbxPropertyEvalStateMap	mPropertyMap
	FbxPropertyCurveNodeMap	mPropertyCurveNodeMap
#endif 
	FbxTransform* mTransform
class FBXSDK_DLL FbxPropertyEvalState : public FbxEvalState
public:
	FbxPropertyEvalState(FbxProperty& pProperty)
	virtual ~FbxPropertyEvalState()
	template <class T> inline T Get() const 
 T lValue
 mValue->Get(&lValue, FbxTypeOf(lValue))
 return lValue
	template <class T> inline bool Set(const T& pValue)
 return mValue->Set(&pValue, FbxTypeOf(pValue))
	FbxPropertyValue* mValue
#include <fbxsdk/fbxsdk_nsend.h>
#endif 