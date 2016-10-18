#ifndef _FBXSDK_SCENE_CONSTRAINT_PARENT_H_
#define _FBXSDK_SCENE_CONSTRAINT_PARENT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraintParent : public FbxConstraint
    FBXSDK_OBJECT_DECLARE(FbxConstraintParent, FbxConstraint)
public:
        FbxPropertyT<FbxBool> AffectTranslationX
        FbxPropertyT<FbxBool> AffectTranslationY
        FbxPropertyT<FbxBool> AffectTranslationZ
        FbxPropertyT<FbxBool> AffectRotationX
        FbxPropertyT<FbxBool> AffectRotationY
        FbxPropertyT<FbxBool> AffectRotationZ
        FbxPropertyT<FbxBool> AffectScalingX
        FbxPropertyT<FbxBool> AffectScalingY
        FbxPropertyT<FbxBool> AffectScalingZ
        FbxPropertyT<FbxReference> ConstraintSources
        FbxPropertyT<FbxReference> ConstrainedObject
    void SetTranslationOffset(FbxObject* pObject, FbxVector4 pTranslation)
    FbxVector4 GetTranslationOffset(const FbxObject* pObject) const
    virtual void SetRotationOffset(const FbxObject* pObject, FbxVector4 pRotation)
    FbxVector4 GetRotationOffset(const FbxObject* pObject) const
    void AddConstraintSource(FbxObject* pObject, double pWeight = 100)
    int GetConstraintSourceCount() const
    FbxObject* GetConstraintSource(int pIndex) const
    void SetConstrainedObject(FbxObject* pObject)
    FbxObject* GetConstrainedObject() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual EType GetConstraintType() const
#endif 