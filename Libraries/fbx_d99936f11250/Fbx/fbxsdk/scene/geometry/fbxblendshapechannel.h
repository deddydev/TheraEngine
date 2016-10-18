#ifndef _FBXSDK_SCENE_GEOMETRY_BLEND_SHAPE_CHANNEL_H_
#define _FBXSDK_SCENE_GEOMETRY_BLEND_SHAPE_CHANNEL_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxsubdeformer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxShape
class FbxBlendShape
class FBXSDK_DLL FbxBlendShapeChannel : public FbxSubDeformer
    FBXSDK_OBJECT_DECLARE(FbxBlendShapeChannel, FbxSubDeformer)
public:
	FbxPropertyT<FbxDouble>        DeformPercent
	bool SetBlendShapeDeformer(FbxBlendShape* pBlendShape)
	FbxBlendShape* GetBlendShapeDeformer()
	bool AddTargetShape(FbxShape* pShape, double pFullDeformPercent = 100)
	FbxShape* RemoveTargetShape(FbxShape* pShape)
	int GetTargetShapeCount() const
	FbxShape* GetTargetShape(int pIndex)
	const FbxShape* GetTargetShape(int pIndex) const
	int GetTargetShapeIndex( FbxShape* pShape)
	double* GetTargetShapeFullWeights()
	void SetFullWeightsCount(int pCount)
    EType GetSubDeformerType() const 
return eBlendShapeChannel
    void Reset()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
protected:
    virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
    virtual FbxStringList GetTypeFlags() const
	FbxArray<double> mShapeFullWeightArray
#endif 