#ifndef _FBXSDK_SCENE_GEOMETRY_SHAPE_H_
#define _FBXSDK_SCENE_GEOMETRY_SHAPE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometrybase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxBlendShapeChannel
class FbxGeometry
class FBXSDK_DLL FbxShape : public FbxGeometryBase
    FBXSDK_OBJECT_DECLARE(FbxShape, FbxGeometryBase)
public:
	bool SetBlendShapeChannel(FbxBlendShapeChannel* pBlendShapeChannel)
	FbxBlendShapeChannel* GetBlendShapeChannel() const
	FbxGeometry* GetBaseGeometry()
	int GetControlPointIndicesCount() const
	int* GetControlPointIndices() const
	void SetControlPointIndicesCount(int pCount)
	void AddControlPointIndex(int pIndex)
	void Reset()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual void Compact()
	virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
protected:
    virtual FbxNodeAttribute::EType GetAttributeType() const
	virtual FbxStringList GetTypeFlags() const
	FbxArray<int> mControlPointIndices
#endif 