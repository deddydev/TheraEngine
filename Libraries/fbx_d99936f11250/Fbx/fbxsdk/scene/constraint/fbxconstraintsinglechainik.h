#ifndef _FBXSDK_SCENE_CONSTRAINT_SINGLE_CHAIN_IK_H_
#define _FBXSDK_SCENE_CONSTRAINT_SINGLE_CHAIN_IK_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraintSingleChainIK : public FbxConstraint
    FBXSDK_OBJECT_DECLARE(FbxConstraintSingleChainIK, FbxConstraint)
public:
    enum ESolverMode
        eRotatePlane,	
        eSingleChain	
    enum EPoleVectorMode
        eVector,	
        eObject		
	enum EEvaluationMode
		eNeverTS,		
		eAutoDetect,	
		eAlwaysTS		
		FbxPropertyT<FbxEnum>        PoleVectorType
		FbxPropertyT<FbxEnum>        SolverType
		FbxPropertyT<FbxEnum>        EvaluateTSAnim
		FbxPropertyT<FbxReference>    PoleVectorObjects
		FbxPropertyT<FbxDouble3>    PoleVector
		FbxPropertyT<FbxDouble>    Twist
		FbxPropertyT<FbxReference> FirstJointObject
		FbxPropertyT<FbxReference> EndJointObject
		FbxPropertyT<FbxReference> EffectorObject
    double GetPoleVectorObjectWeight(const FbxObject* pObject) const
    void AddPoleVectorObject(FbxObject* pObject, double pWeight = 100)
    int GetConstraintPoleVectorCount() const
    FbxObject* GetPoleVectorObject(int pIndex) const
    void SetFirstJointObject(FbxObject* pObject)
    FbxObject* GetFirstJointObject() const
    void SetEndJointObject(FbxObject* pObject)
    FbxObject* GetEndJointObject() const
    void SetEffectorObject(FbxObject* pObject)
    FbxObject* GetEffectorObject() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual EType GetConstraintType() const
#endif 