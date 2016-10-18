#ifndef _FBXSDK_UTILS_DEFORMATIONS_EVALUATOR_H_
#define _FBXSDK_UTILS_DEFORMATIONS_EVALUATOR_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxNode
class FbxMesh
class FbxTime
class FbxAnimLayer
class FbxPose
class FbxCluster
class FbxVector4
class FbxAMatrix
class FbxDualQuaternion
class FBXSDK_DLL FbxDeformationsEvaluator
public:
    bool Init(const FbxNode* pNode, const FbxMesh* pMesh)
    bool ComputeShapeDeformation(FbxVector4* pVertexArray, const FbxTime& pTime)
    bool ComputeSkinDeformation(FbxVector4* pVertexArray, const FbxTime& pTime, FbxAMatrix* pGX=NULL, const FbxPose* pPose=NULL)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxDeformationsEvaluator()
    virtual ~FbxDeformationsEvaluator()
private:
	void				ComputeClusterDeformation(FbxVector4* pVertexArray, const FbxTime& pTime, const FbxAMatrix& pGX, FbxCluster* pCluster, FbxAMatrix& pVertexTransformMatrix, const FbxPose* pPose)
	void				ComputeLinearDeformation(FbxVector4* pVertexArray, const FbxTime& pTime, const FbxAMatrix& pGX, const FbxPose* pPose)
	void				ComputeDualQuaternionDeformation(FbxVector4* pVertexArray, const FbxTime& pTime, const FbxAMatrix& pGX, const FbxPose* pPose)
	void				Cleanup()
	bool				mIsConfigured
	FbxNode*			mNode
	FbxMesh*			mMesh
	FbxAnimLayer*		mAnimLayer
	int					mVertexCount
	FbxVector4*			mDstVertexArray
	FbxVector4*			mVertexArrayLinear
	FbxVector4*			mVertexArrayDQ
	FbxAMatrix*			mClusterDeformation
	double*				mClusterWeight
	FbxDualQuaternion*	mDQClusterDeformation
#endif 