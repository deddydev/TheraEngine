#ifndef _FBXSDK_SCENE_GEOMETRY_NODE_ATTRIBUTE_H_
#define _FBXSDK_SCENE_GEOMETRY_NODE_ATTRIBUTE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxNode
class FBXSDK_DLL FbxNodeAttribute : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxNodeAttribute, FbxObject)
public:
    static const char* sColor
    static const FbxDouble3 sDefaultColor
    FbxPropertyT<FbxDouble3> Color
    enum EType
        eUnknown,
        eNull,
        eMarker,
        eSkeleton, 
        eMesh, 
        eNurbs, 
        ePatch,
        eCamera, 
        eCameraStereo,
        eCameraSwitcher,
        eLight,
        eOpticalReference,
        eOpticalMarker,
        eNurbsCurve,
        eTrimNurbsSurface,
        eBoundary,
        eNurbsSurface,
        eShape,
        eLODGroup,
        eSubDiv,
        eCachedEffect,
        eLine
    virtual FbxNodeAttribute::EType GetAttributeType() const
	int GetNodeCount() const
    FbxNode* GetNode(int pIndex=0) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
#endif 