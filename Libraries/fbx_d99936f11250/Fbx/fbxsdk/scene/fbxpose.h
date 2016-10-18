#ifndef _FBXSDK_SCENE_POSE_H_
#define _FBXSDK_SCENE_POSE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/math/fbxmatrix.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxStatus
class FbxPose
class FbxNode
class FbxUserNotification
struct FbxPoseInfo
    FbxMatrix	mMatrix
    bool		mMatrixIsLocal
    FbxNode*	mNode
typedef FbxArray<FbxNode*> NodeList
typedef FbxArray<FbxPose*> PoseList
typedef FbxArray<FbxPoseInfo*> PoseInfoList
class FBXSDK_DLL FbxPose : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxPose,FbxObject)
public:
	void SetIsBindPose(bool pIsBindPose)
	bool IsBindPose() const 
 return mType == 'b'
	bool IsRestPose() const 
 return mType == 'r'
	int GetCount() const 
 return mPoseInfo.GetCount()
	int Add(FbxNode* pNode, const FbxMatrix& pMatrix, bool pLocalMatrix = false, bool pMultipleBindPose = true)
	void Remove(int pIndex)
	FbxNameHandler GetNodeName(int pIndex) const
	FbxNode* GetNode(int pIndex) const
	const FbxMatrix& GetMatrix(int pIndex)       const
	bool IsLocalMatrix(int pIndex) const
		enum ENameComponent
			eInitialNameComponent = 1,	
			eCurrentNameComponent = 2,	
			eAllNameComponents = 3		
		int Find(const FbxNameHandler& pNodeName, char pCompareWhat = eAllNameComponents) const
		int Find(const FbxNode* pNode) const
		static bool GetPosesContaining(FbxManager& pManager, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		static bool GetPosesContaining(FbxScene* pScene, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		static bool GetBindPoseContaining(FbxManager& pManager, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		static bool GetBindPoseContaining(FbxScene* pScene, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		static bool GetRestPoseContaining(FbxManager& pManager, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		static bool GetRestPoseContaining(FbxScene* pScene, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
		bool IsValidBindPose(FbxNode* pRoot, double pMatrixCmpTolerance=0.0001, FbxStatus* pStatus = NULL)
		bool IsValidBindPoseVerbose(FbxNode* pRoot, NodeList& pMissingAncestors, NodeList& pMissingDeformers, NodeList& pMissingDeformersAncestors, NodeList& pWrongMatrices, double pMatrixCmpTolerance=0.0001, FbxStatus* pStatus = NULL)
		bool IsValidBindPoseVerbose(FbxNode* pRoot, FbxUserNotification* pUserNotification, double pMatrixCmpTolerance=0.0001, FbxStatus* pStatus = NULL)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    virtual void ConstructProperties(bool pForceSet)
	virtual FbxObject&	Copy(const FbxObject& pObject)
    virtual const char*	GetTypeName() const
    bool				ValidateParams(const FbxNode* pNode, const FbxMatrix& pMatrix, int& pPos)
    bool				LocalValidateParams(const FbxNode* pNode, const FbxMatrix& pMatrix, int& pPos)
    static bool			GetSpecificPoseContaining(int poseType, FbxScene* pScene, FbxNode* pNode, PoseList& pPoseList, FbxArray<int>& pIndex)
private:
    FbxPoseInfo*		GetItem(int pIndex) const
    void                UpdatePosInfoList()
    bool				IsValidBindPoseCommon(FbxNode* pRoot, NodeList* pMissingAncestors, NodeList* pMissingDeformers, NodeList* pMissingDeformersAncestors, NodeList* pWrongMatrices, FbxStatus* pStatus, double pMatrixCmpTolerance=0.0001)
    char				        mType
    PoseInfoList		        mPoseInfo
    bool                        mPoseInfoIsDirty
    FbxPropertyT<FbxReference>  Nodes
#endif 