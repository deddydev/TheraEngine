#ifndef _FBXSDK_SCENE_CONTAINER_TEMPLATE_H_
#define _FBXSDK_SCENE_CONTAINER_TEMPLATE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
#define FBXSDK_CONTAINER_TEMPLATE_STR  "templates"
#define FBXSDK_TEMPLATE_STR            "template"
#define FBXSDK_EXTENDS_TEMPLATE_STR    "extends"
struct FbxContainerTemplate_internal
class FBXSDK_DLL FbxContainerTemplate : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxContainerTemplate, FbxObject)
public:
    void ParseTemplateFile(const char* pTemplateFilePath, FbxArray<FbxString*>& pExtendTemplateNames)
    void AddExtendTemplatePath(const char* pExtendTemplatePath)
    char* GetExtendTemplatePathAt(FbxUInt pIndex) const
    FbxUInt GetExtendTemplateCount() const
    void ClearExtendTemplatePath()
    FbxPropertyT<FbxString> ContainerTemplateName
    FbxPropertyT<FbxString> ContainerTemplatePath
    FbxPropertyT<FbxString> ContainerTemplatePackageName
    FbxPropertyT<FbxString> ContainerTemplateVersion
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
    virtual void Destruct(bool pRecursive)
private:
	FbxContainerTemplate_internal*	mData
    FbxArray<FbxString*>		mExtendTemplatePaths
#endif 