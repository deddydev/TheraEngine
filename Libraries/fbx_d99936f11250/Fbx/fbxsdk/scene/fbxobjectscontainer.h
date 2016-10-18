#ifndef _FBXSDK_SCENE_OBJECTS_CONTAINER_H_
#define _FBXSDK_SCENE_OBJECTS_CONTAINER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/scene/fbxscene.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
typedef FbxArray<FbxNodeAttribute::EType> FbxAttributeFilters
class FbxObjectsContainer
public:
    enum EDepth
        eChildOnly,
        eChildAndSubChild,
		eSubChildWithNoScaleInherit
	FbxObjectsContainer() : mStartNode(NULL) 
    virtual ~FbxObjectsContainer()
 Clear()
    FbxArray<FbxAnimCurveNode*> mFCurvesT
    FbxArray<FbxAnimCurveNode*> mFCurvesR
	FbxArray<FbxAnimCurveNode*> mFCurvesS
    FbxArray<FbxNode*> mNodes
public:
    void ExtractSceneObjects(FbxScene* pScene, EDepth pDepth, const FbxAttributeFilters& pFilters)
	void ExtractSceneObjects(FbxNode* pRootNode, EDepth pDepth, const FbxAttributeFilters& pFilters)
    void Clear() 
 mFCurvesT.Clear()
 mFCurvesR.Clear()
 mFCurvesS.Clear()
 mNodes.Clear()
 mStartNode = NULL
protected:
    void ExtractNodesAnimCurveNodes(FbxNode* pNode, EDepth pDepth, const FbxAttributeFilters& pFilters)
    void ExtractAnimCurveNodes(FbxNode* pNode)
	bool InheritsScale( FbxNode* pNode ) const
	FbxNode* mStartNode
#include <fbxsdk/fbxsdk_nsend.h>
#endif 