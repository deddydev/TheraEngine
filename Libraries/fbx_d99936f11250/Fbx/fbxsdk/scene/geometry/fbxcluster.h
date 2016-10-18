#ifndef _FBXSDK_SCENE_GEOMETRY_CLUSTER_H_
#define _FBXSDK_SCENE_GEOMETRY_CLUSTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxsubdeformer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCluster : public FbxSubDeformer
    FBXSDK_OBJECT_DECLARE(FbxCluster,FbxSubDeformer)
public:
    EType GetSubDeformerType() const 
return eCluster
    void Reset()
    enum ELinkMode
		eNormalize,       
        eAdditive,
        eTotalOne   
    void SetLinkMode(ELinkMode pMode)
    ELinkMode GetLinkMode() const
    void SetLink(const FbxNode* pNode)
    FbxNode* GetLink()
    const FbxNode* GetLink() const
    void SetAssociateModel(FbxNode* pNode)
    FbxNode* GetAssociateModel() const
    void AddControlPointIndex(int pIndex, double pWeight)
    int GetControlPointIndicesCount() const
    int* GetControlPointIndices() const
    double* GetControlPointWeights() const
	void SetControlPointIWCount(int pCount)
    void SetTransformMatrix(const FbxAMatrix& pMatrix)
    FbxAMatrix& GetTransformMatrix(FbxAMatrix& pMatrix) const
    void SetTransformLinkMatrix(const FbxAMatrix& pMatrix)
    FbxAMatrix& GetTransformLinkMatrix(FbxAMatrix& pMatrix) const
    void SetTransformAssociateModelMatrix(const FbxAMatrix& pMatrix)
    FbxAMatrix& GetTransformAssociateModelMatrix(FbxAMatrix& pMatrix) const
    void SetTransformParentMatrix(const FbxAMatrix& pMatrix)
    FbxAMatrix& GetTransformParentMatrix(FbxAMatrix& pMatrix) const
    bool IsTransformParentSet() const 
 return mIsTransformParentSet
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    void SetUserData(const char* pUserDataID, const char* pUserData)
    const char* GetUserDataID () const
    const char* GetUserData () const
    const char* GetUserData (const char* pUserDataID) const
    FbxString	mBeforeVersion6LinkName
    FbxString	mBeforeVersion6AssociateModelName
protected:
    virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
    virtual FbxStringList GetTypeFlags() const
    ELinkMode               mLinkMode
    FbxString               mUserDataID
    FbxString               mUserData
    FbxArray<int>           mControlPointIndices
    FbxArray<double>        mControlPointWeights
    FbxMatrix              mTransform
    FbxMatrix              mTransformLink
    FbxMatrix              mTransformAssociate
    FbxMatrix              mTransformParent
    bool                    mIsTransformParentSet
    FbxPropertyT<FbxReference> SrcModelReference
#endif 