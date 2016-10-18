#ifndef _FBXSDK_SCENE_GEOMETRY_SKIN_H_
#define _FBXSDK_SCENE_GEOMETRY_SKIN_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxdeformer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCluster
class FbxGeometry
class FBXSDK_DLL FbxSkin : public FbxDeformer
    FBXSDK_OBJECT_DECLARE(FbxSkin, FbxDeformer)
public:
    void SetDeformAccuracy(double pDeformAccuracy)
    double GetDeformAccuracy() const
    bool SetGeometry(FbxGeometry* pGeometry)
    FbxGeometry* GetGeometry()
    bool AddCluster(FbxCluster* pCluster)
    FbxCluster* RemoveCluster(FbxCluster* pCluster)
    int GetClusterCount() const
    FbxCluster* GetCluster(int pIndex)
    const FbxCluster* GetCluster(int pIndex) const
    EDeformerType GetDeformerType()  const 
return eSkin
	enum EType
		eRigid,
		eLinear,
		eDualQuaternion,
		eBlend
	void SetSkinningType(EType pType)
	EType GetSkinningType() const
	void AddControlPointIndex(int pIndex, double pBlendWeight = 0)
	int GetControlPointIndicesCount() const
	int* GetControlPointIndices() const
	double* GetControlPointBlendWeights() const
	void SetControlPointIWCount(int pCount)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual void Compact()
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual FbxStringList GetTypeFlags() const
    double mDeformAccuracy
	EType mSkinningType
	FbxArray<int>     mControlPointIndices
	FbxArray<double>  mControlPointBlendWeights
#endif 