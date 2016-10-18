#ifndef _FBXSDK_SCENE_GEOMETRY_VERTEX_CACHE_DEFORMER_H_
#define _FBXSDK_SCENE_GEOMETRY_VERTEX_CACHE_DEFORMER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxdeformer.h>
#include <fbxsdk/scene/geometry/fbxcache.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxVertexCacheDeformer : public FbxDeformer
	FBXSDK_OBJECT_DECLARE(FbxVertexCacheDeformer, FbxDeformer)
public:
	enum ECacheChannelType
		ePositions,		
		eNormals,		
		eUVs,			
		eTangents,		
		eBinormals,		
		eUserDefined	
	void SetCache(FbxCache* pCache)
	FbxCache* GetCache() const
	FbxPropertyT<FbxBool> Active
	FbxPropertyT<FbxString> Channel
	FbxPropertyT<FbxString> CacheSet
	FbxPropertyT<ECacheChannelType> Type
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	virtual EDeformerType GetDeformerType() const 
 return FbxDeformer::eVertexCache
protected:
	virtual void ConstructProperties(bool pForceSet)
	virtual FbxStringList GetTypeFlags() const
#endif 