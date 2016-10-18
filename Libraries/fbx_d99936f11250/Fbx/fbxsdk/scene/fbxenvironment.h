#ifndef _FBXSDK_SCENE_ENVIRONMENT_H_
#define _FBXSDK_SCENE_ENVIRONMENT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxEnvironment : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxEnvironment, FbxObject)
public:
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	bool ProvidesLighting() const
#endif 