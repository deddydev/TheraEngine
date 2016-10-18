#ifndef _FBXSDK_SCENE_GEOMETRY_WEIGHTED_MAP_H_
#define _FBXSDK_SCENE_GEOMETRY_WEIGHTED_MAP_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/geometry/fbxweightedmapping.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxGeometry
class FBXSDK_DLL FbxGeometryWeightedMap : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxGeometryWeightedMap, FbxObject)
public:
    void SetValues(const FbxWeightedMapping* pWeightedMappingTable)
    FbxWeightedMapping* GetValues() const
    FbxGeometry* GetSourceGeometry()
    FbxGeometry* GetDestinationGeometry()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    FbxWeightedMapping* mWeightedMapping
#endif 