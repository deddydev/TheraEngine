#ifndef _FBXSDK_SCENE_ANIMATION_EVALUATOR_H_
#define _FBXSDK_SCENE_ANIMATION_EVALUATOR_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/animation/fbxanimevalstate.h>
#include <fbxsdk/scene/animation/fbxanimstack.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxAnimEvaluator : public FbxObject
    FBXSDK_ABSTRACT_OBJECT_DECLARE(FbxAnimEvaluator, FbxObject)
public:
	FbxAMatrix& GetNodeGlobalTransform(FbxNode* pNode, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
    FbxAMatrix& GetNodeLocalTransform(FbxNode* pNode, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
	FbxVector4& GetNodeLocalTranslation(FbxNode* pNode, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
	FbxVector4& GetNodeLocalRotation(FbxNode* pNode, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
	FbxVector4& GetNodeLocalScaling(FbxNode* pNode, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
#if defined(__GNUC__) && (__GNUC__ < 4)
	template <class T> inline T GetPropertyValue(FbxProperty& pProperty, const FbxTime& pTime, bool pForceEval=false)
 FbxPropertyEvalState* s = GetPropertyEvalState(pProperty, pTime, pForceEval)
 return s->Get<T>()
#else
    template <class T> inline T GetPropertyValue(FbxProperty& pProperty, const FbxTime& pTime, bool pForceEval=false)
 return GetPropertyEvalState(pProperty, pTime, pForceEval)->Get<T>()
#endif
	FbxPropertyValue& GetPropertyValue(FbxProperty& pProperty, const FbxTime& pTime, bool pForceEval=false)
	FbxAnimCurveNode* GetPropertyCurveNode(FbxProperty& pProperty, FbxAnimLayer* pAnimLayer)
    FbxTime ValidateTime(const FbxTime& pTime)
	void Flush(FbxNode* pNode)
	void Flush(FbxProperty& pProperty)
	void ComputeLocalTRSFromGlobal(FbxVector4& pRetLT, FbxVector4& pRetLR, FbxVector4& pRetLS, FbxNode* pNode, FbxAMatrix& pGX, const FbxTime& pTime=FBXSDK_TIME_INFINITE, FbxNode::EPivotSet pPivotSet=FbxNode::eSourcePivot, bool pApplyTarget=false, bool pForceEval=false)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    virtual void EvaluateNodeTransform(FbxNodeEvalState* pResult, FbxNode* pNode, const FbxTime& pTime, FbxNode::EPivotSet pPivotSet, bool pApplyTarget) = 0
	virtual void EvaluatePropertyValue(FbxPropertyEvalState* pResult, FbxProperty& pProperty, const FbxTime& pTime) = 0
	FbxAnimEvalState*		GetDefaultEvalState()
	FbxAnimEvalState*		GetEvalState(const FbxTime& pTime)
	FbxNodeEvalState*		GetNodeEvalState(FbxNode* pNode, const FbxTime& pTime, FbxNode::EPivotSet pPivotSet, bool pApplyTarget, bool pForceEval)
	FbxPropertyEvalState*	GetPropertyEvalState(FbxProperty& pProperty, const FbxTime& pTime, bool pForceEval)
private:
	FbxAnimEvalState*		mEvalState
#endif 