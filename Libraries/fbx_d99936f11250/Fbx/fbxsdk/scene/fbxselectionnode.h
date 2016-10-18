#ifndef _FBXSDK_SCENE_SELECTION_NODE_H_
#define _FBXSDK_SCENE_SELECTION_NODE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSelectionNode : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxSelectionNode, FbxObject)
public:
    enum ESelectType
        eVertexLevel,
        eEdgeLevel,
        eFaceLevel,
        eObjectLevel,
        eSelectTypeCount
    bool SetSelectionObject(FbxObject* pObject)
    FbxObject* GetSelectionObject() const
    bool IsValid() const
    bool mIsTheNodeInSet
    FbxArray<int> mVertexIndexArray
    FbxArray<int> mEdgeIndexArray
    FbxArray<int> mPolygonIndexArray
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxArray<FbxArray<int>*> mSubTypeSelectArray
    static const char* SELECT_TYPE_NAMES[(int)eSelectTypeCount]
protected:
	virtual void Construct(const FbxObject* pFrom)
    bool ConnectNotify (FbxConnectEvent const &pEvent)
#endif 