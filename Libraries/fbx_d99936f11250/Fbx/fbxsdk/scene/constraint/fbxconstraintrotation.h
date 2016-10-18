#ifndef _FBXSDK_SCENE_CONSTRAINT_ROTATION_H_
#define _FBXSDK_SCENE_CONSTRAINT_ROTATION_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraintRotation : public FbxConstraint
	FBXSDK_OBJECT_DECLARE(FbxConstraintRotation, FbxConstraint)
public:
			FbxPropertyT<FbxBool>		AffectX
			FbxPropertyT<FbxBool>		AffectY
			FbxPropertyT<FbxBool>		AffectZ
			FbxPropertyT<FbxDouble3>	Rotation
			FbxPropertyT<FbxReference> ConstraintSources
			FbxPropertyT<FbxReference> ConstrainedObject
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