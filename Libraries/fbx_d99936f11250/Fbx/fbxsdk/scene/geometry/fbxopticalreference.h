#ifndef _FBXSDK_SCENE_GEOMETRY_OPTICAL_REFERENCE_H_
#define _FBXSDK_SCENE_GEOMETRY_OPTICAL_REFERENCE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxOpticalReference : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxOpticalReference,FbxNodeAttribute)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual FbxStringList	GetTypeFlags() const
#endif 