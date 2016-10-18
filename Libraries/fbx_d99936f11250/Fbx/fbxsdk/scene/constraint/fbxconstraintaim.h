#ifndef _FBXSDK_SCENE_CONSTRAINT_AIM_H_
#define _FBXSDK_SCENE_CONSTRAINT_AIM_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraintAim : public FbxConstraint
    FBXSDK_OBJECT_DECLARE(FbxConstraintAim, FbxConstraint)
public:
    enum EWorldUp
        eAimAtSceneUp,			
        eAimAtObjectUp,			
        eAimAtObjectRotationUp,	
        eAimAtVector,			
        eAimAtNone,				
        eAimAtCount				
        FbxPropertyT<FbxDouble3> RotationOffset
        FbxPropertyT<FbxReference> AimAtObjects
        FbxPropertyT<FbxReference> ConstrainedObject
        FbxPropertyT<FbxEnum> WorldUpType
        FbxPropertyT<FbxReference> WorldUpObject
        FbxPropertyT<FbxDouble3> WorldUpVector
        FbxPropertyT<FbxDouble3> UpVector
        FbxPropertyT<FbxDouble3> AimVector
        FbxPropertyT<FbxBool> AffectX
        FbxPropertyT<FbxBool> AffectY
        FbxPropertyT<FbxBool> AffectZ
    void AddConstraintSource(FbxObject* pObject, double pWeight = 100)
    int GetConstraintSourceCount() const
    FbxObject* GetConstraintSource(int pIndex) const
    void SetConstrainedObject(FbxObject* pObject)
    FbxObject* GetConstrainedObject() const
    void SetWorldUpObject(FbxObject* pObject)
    FbxObject* GetWorldUpObject() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual EType GetConstraintType() const
#endif 