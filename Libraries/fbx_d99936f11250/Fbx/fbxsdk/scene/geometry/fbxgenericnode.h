#ifndef _FBXSDK_SCENE_GEOMETRY_GENERIC_NODE_H_
#define _FBXSDK_SCENE_GEOMETRY_GENERIC_NODE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxGenericNode : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxGenericNode, FbxObject)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual FbxStringList GetTypeFlags() const
#endif 