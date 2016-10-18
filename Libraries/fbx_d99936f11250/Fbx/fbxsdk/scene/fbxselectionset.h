#ifndef _FBXSDK_SCENE_SELECTION_SET_H_
#define _FBXSDK_SCENE_SELECTION_SET_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxcollection.h>
#include <fbxsdk/scene/fbxselectionnode.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSelectionSet : public FbxCollection
    FBXSDK_OBJECT_DECLARE(FbxSelectionSet, FbxCollection)
public:
    FbxPropertyT<FbxString>        SelectionSetAnnotation
    void GetFaceSelection( FbxObject* pObj,FbxArray<int>& pPolygonIndexArray ) const
    void GetEdgeSelection( FbxObject* pObj,FbxArray<int>& pEdgeIndexArray ) const
    void GetVertexSelection( FbxObject* pObj,FbxArray<int>& pVertexIndexArray ) const
    void GetSelectionNodesAndDirectObjects(FbxArray<FbxSelectionNode*> &pSelectionNodeList, FbxArray<FbxObject*> &pDirectObjectList) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
#endif 