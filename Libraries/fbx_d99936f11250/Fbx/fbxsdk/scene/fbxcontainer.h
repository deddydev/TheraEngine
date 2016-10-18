#ifndef _FBXSDK_SCENE_CONTAINER_H_
#define _FBXSDK_SCENE_CONTAINER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/fbxcontainertemplate.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxContainer : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxContainer, FbxObject)
public:
		FbxProperty CreateProperty(FbxString pName, FbxDataType & pType, FbxString pLabel)
		FbxPropertyT<FbxString> TemplateName
		FbxPropertyT<FbxString> TemplatePath
		FbxPropertyT<FbxString> TemplateVersion
		FbxPropertyT<FbxString> ViewName
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxContainerTemplate* mContainerTemplate
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
#endif 