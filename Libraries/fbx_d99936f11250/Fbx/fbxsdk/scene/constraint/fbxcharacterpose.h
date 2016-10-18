#ifndef _FBXSDK_SCENE_CONSTRAINT_CHARACTER_POSE_H_
#define _FBXSDK_SCENE_CONSTRAINT_CHARACTER_POSE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/constraint/fbxcharacter.h>
#include <fbxsdk/scene/geometry/fbxnode.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCharacterPose : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxCharacterPose,FbxObject)
public:
    void Reset()
    FbxNode* GetRootNode() const
    FbxCharacter* GetCharacter() const
    bool GetOffset(FbxCharacter::ENodeId pCharacterNodeId, FbxAMatrix& pOffset) const
    bool GetLocalPosition(FbxCharacter::ENodeId pCharacterNodeId, FbxVector4& pLocalT, FbxVector4& pLocalR, FbxVector4& pLocalS) const
    bool GetGlobalPosition(FbxCharacter::ENodeId pCharacterNodeId, FbxAMatrix& pGlobalPosition) const
	FbxScene* GetPoseScene() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(                          FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
            void       Clone(FbxScene* pPoseScene,     FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
private:
    FbxScene* mPoseScene
#endif 