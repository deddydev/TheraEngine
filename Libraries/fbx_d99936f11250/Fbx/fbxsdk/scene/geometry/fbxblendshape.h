#ifndef _FBXSDK_SCENE_GEOMETRY_BLEND_SHAPE_H_
#define _FBXSDK_SCENE_GEOMETRY_BLEND_SHAPE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxdeformer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxGeometry
class FbxBlendShapeChannel
class FBXSDK_DLL FbxBlendShape : public FbxDeformer
    FBXSDK_OBJECT_DECLARE(FbxBlendShape, FbxDeformer)
public:
    bool SetGeometry(FbxGeometry* pGeometry)
    FbxGeometry* GetGeometry()
    bool AddBlendShapeChannel(FbxBlendShapeChannel* pBlendShapeChannel)
    FbxBlendShapeChannel* RemoveBlendShapeChannel(FbxBlendShapeChannel* pBlendShapeChannel)
    int GetBlendShapeChannelCount() const
    FbxBlendShapeChannel* GetBlendShapeChannel(int pIndex)
    const FbxBlendShapeChannel* GetBlendShapeChannel(int pIndex) const
    EDeformerType GetDeformerType()  const 
return eBlendShape
	void Reset()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
protected:
    virtual FbxStringList GetTypeFlags() const
#endif 