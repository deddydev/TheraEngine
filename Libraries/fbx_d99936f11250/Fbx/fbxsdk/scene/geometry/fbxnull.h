#ifndef _FBXSDK_SCENE_GEOMETRY_NULL_H_
#define _FBXSDK_SCENE_GEOMETRY_NULL_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxNull : public FbxNodeAttribute
    FBXSDK_OBJECT_DECLARE(FbxNull, FbxNodeAttribute)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
    enum ELook
        eNone,
        eCross,
    double GetSizeDefaultValue() const
    static const char*          sSize
    static const char*          sLook
    static const FbxDouble     sDefaultSize
    static const ELook      sDefaultLook
    FbxPropertyT<FbxDouble>       Size
    FbxPropertyT<ELook>            Look
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
    virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
public:
    virtual FbxStringList GetTypeFlags() const
#endif 