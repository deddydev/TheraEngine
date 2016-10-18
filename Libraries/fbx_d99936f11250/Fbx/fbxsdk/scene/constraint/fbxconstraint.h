#ifndef _FBXSDK_SCENE_CONSTRAINT_H_
#define _FBXSDK_SCENE_CONSTRAINT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxConstraint : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxConstraint, FbxObject)
public:
        FbxPropertyT<FbxDouble> Weight
        FbxPropertyT<FbxBool> Active
        FbxPropertyT<FbxBool> Lock
    enum EType
        eUnknown,			
        ePosition,			
        eRotation,			
        eScale,				
        eParent,			
        eSingleChainIK,		
        eAim,				
        eCharacter,			
        eCustom				
    virtual EType GetConstraintType() const 
 return eUnknown
    virtual FbxObject* GetConstrainedObject() const 
 return NULL
    virtual int GetConstraintSourceCount() const 
 return 0
    virtual FbxObject* GetConstraintSource(int 
    double GetSourceWeight(const FbxObject* pObject) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
#endif 
#ifndef DOXYGEN_SHOULD_SKIP_THIS
const FbxString GetWeightPropertyName(const FbxObject * pObject)
void CreateWeightPropertyForSourceObject(FbxObject * pConstraint, const FbxObject * pSourceObject, double pWeightValue)
#endif 