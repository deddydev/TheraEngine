#ifndef _FBXSDK_SCENE_GEOMETRY_CACHED_EFFECT_H_
#define _FBXSDK_SCENE_GEOMETRY_CACHED_EFFECT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/scene/geometry/fbxcache.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCachedEffect : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxCachedEffect, FbxNodeAttribute)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
    enum ECategory
        eParticles,	
        eFluids,	
        eHair,		
        eGeneric	
    ECategory GetCategory() const
	void SetCache( FbxCache* pCache, ECategory pCategory = eGeneric)
	FbxCache* GetCache() const
protected:
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject&	Copy(const FbxObject& pObject)
protected:
	virtual void ConstructProperties(bool pForceSet)
public:
	virtual const char* GetTypeName() const
	virtual FbxStringList GetTypeFlags() const
private:
	void ClearCacheConnections()
    FbxPropertyT<ECategory> Category
#endif 