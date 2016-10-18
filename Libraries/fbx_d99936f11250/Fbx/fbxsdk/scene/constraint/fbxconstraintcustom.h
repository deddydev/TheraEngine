#ifndef _FBXSDK_SCENE_CONSTRAINT_CUSTOM_H_
#define _FBXSDK_SCENE_CONSTRAINT_CUSTOM_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraintCustom : public FbxConstraint
    FBXSDK_OBJECT_DECLARE(FbxConstraintCustom, FbxConstraint)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual EType GetConstraintType() const
#endif 