#ifndef _FBXSDK_SCENE_ANIMATION_EVALUATOR_CLASSIC_H_
#define _FBXSDK_SCENE_ANIMATION_EVALUATOR_CLASSIC_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/animation/fbxanimevaluator.h>
#include <fbxsdk/scene/animation/fbxanimlayer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxAnimEvalClassic : public FbxAnimEvaluator
    FBXSDK_OBJECT_DECLARE(FbxAnimEvalClassic, FbxAnimEvaluator)
	enum EBlendType 
eSimple, eRotation, eScaling
	void ComputeTRSLocal(FbxNodeEvalState* pResult, FbxNode* pNode, const FbxTime& pTime, FbxAnimStack* pStack)
	void ComputeGlobalTransform(FbxNodeEvalState* pResult, FbxNode* pNode, const FbxTime& pTime, FbxAnimStack* pStack, FbxNode::EPivotSet pPivotSet, bool pApplyTarget)
	void ComputeLocalTransform(FbxNodeEvalState* pResult, FbxNode* pNode, const FbxTime& pTime, FbxAnimStack* pStack, FbxNode::EPivotSet pPivotSet, bool pApplyTarget)
	bool HasAnimationCurveNode(FbxProperty& pProperty, FbxAnimLayer* pAnimLayer)
	void ComputeTRSAnimationLayer(FbxNodeEvalState* pResult, FbxNode* pNode, FbxVector4& pLT, FbxVector4& pLR, FbxVector4& pLS, const FbxTime& pTime, FbxAnimLayer* pLayer, bool pBlend)
	void BlendPropertyEvalWithLayer(double* pResult, int pResultSize, FbxProperty& pProperty, FbxNodeEvalState* pEvalState, const FbxTime& pTime, FbxAnimLayer* pLayer, EBlendType pType)
	void BlendSimple(double* pResult, int pResultSize, double* pApply, int pApplySize, double pWeight, FbxAnimLayer::EBlendMode pBlendMode)
	void BlendRotation(double* pResult, int pResultSize, double* pApply, int pApplySize, double pWeight, FbxAnimLayer::EBlendMode pBlendMode, FbxAnimLayer::ERotationAccumulationMode pRotAccuMode, int pRotationOrder)
	void BlendScaling(double* pResult, int pResultSize, double* pApply, int pApplySize, double pWeight, FbxAnimLayer::EBlendMode pBlendMode, FbxAnimLayer::EScaleAccumulationMode pScaleAccuMode)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	virtual void EvaluateNodeTransform(FbxNodeEvalState* pResult, FbxNode* pNode, const FbxTime& pTime, FbxNode::EPivotSet pPivotSet, bool pApplyTarget)
	virtual void EvaluatePropertyValue(FbxPropertyEvalState* pResult, FbxProperty& pProperty, const FbxTime& pTime)
private:
	double* mPropertyValues
	int		mPropertySize
	double*	mCurveNodeEvalValues
	int		mCurveNodeEvalSize
#endif 