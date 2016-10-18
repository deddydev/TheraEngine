#ifndef _FBXSDK_SCENE_SHADING_BINDING_TABLE_H_
#define _FBXSDK_SCENE_SHADING_BINDING_TABLE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxbindingtablebase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBindingTable : public FbxBindingTableBase
    FBXSDK_OBJECT_DECLARE(FbxBindingTable, FbxBindingTableBase)
public:
    FbxPropertyT<FbxString>            TargetName
    FbxPropertyT<FbxString>            TargetType
    FbxPropertyT<FbxString>            DescRelativeURL
    FbxPropertyT<FbxString>            DescAbsoluteURL
    FbxPropertyT<FbxString>            DescTAG
    FbxPropertyT<FbxString>            CodeRelativeURL
    FbxPropertyT<FbxString>            CodeAbsoluteURL
    FbxPropertyT<FbxString>            CodeTAG
    static const char* sTargetName
    static const char* sTargetType
    static const char* sDescRelativeURL
    static const char* sDescAbsoluteURL
    static const char* sDescTAG
    static const char* sCodeRelativeURL
    static const char* sCodeAbsoluteURL
    static const char* sCodeTAG
    static const char* sDefaultTargetName
    static const char* sDefaultTargetType
    static const char* sDefaultDescRelativeURL
    static const char* sDefaultDescAbsoluteURL
    static const char* sDefaultDescTAG
    static const char* sDefaultCodeRelativeURL
    static const char* sDefaultCodeAbsoluteURL
    static const char* sDefaultCodeTAG
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    virtual void ConstructProperties(bool pForceSet)
#endif 