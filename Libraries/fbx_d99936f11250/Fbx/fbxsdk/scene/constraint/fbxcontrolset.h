#ifndef _FBXSDK_SCENE_CONSTRAINT_CONTROL_SET_H_
#define _FBXSDK_SCENE_CONSTRAINT_CONTROL_SET_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/constraint/fbxcharacter.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxControlSetPlug
class FBXSDK_DLL FbxControlSetLink
public:
    FbxControlSetLink()
    FbxControlSetLink(const FbxControlSetLink& pControlSetLink)
    FbxControlSetLink& operator=(const FbxControlSetLink& pControlSetLink)
    void Reset()
    FbxNode* mNode
    FbxString mTemplateName
class FBXSDK_DLL FbxEffector
public:
	enum ESetId
		eDefaultSet,
		eAux1Set,
		eAux2Set,
		eAux3Set,
		eAux4Set,
		eAux5Set,
		eAux6Set,
		eAux7Set,
		eAux8Set,
		eAux9Set,
		eAux10Set,
		eAux11Set,
		eAux12Set,
		eAux13Set,
		eAux14Set,
		eSetIdCount
	enum ENodeId
		eHips,
		eLeftAnkle,
		eRightAnkle,
		eLeftWrist,
		eRightWrist,
		eLeftKnee,
		eRightKnee,
		eLeftElbow,
		eRightElbow,
		eChestOrigin,
		eChestEnd,
		eLeftFoot,
		eRightFoot,
		eLeftShoulder,
		eRightShoulder,
		eHead,
		eLeftHip,
		eRightHip,
		eLeftHand,
		eRightHand,
		eLeftHandThumb,
		eLeftHandIndex,
		eLeftHandMiddle,
		eLeftHandRing,
		eLeftHandPinky,
		eLeftHandExtraFinger,
		eRightHandThumb,
		eRightHandIndex,
		eRightHandMiddle,
		eRightHandRing,
		eRightHandPinky,
		eRightHandExtraFinger,
		eLeftFootThumb,
		eLeftFootIndex,
		eLeftFootMiddle,
		eLeftFootRing,
		eLeftFootPinky,
		eLeftFootExtraFinger,
		eRightFootThumb,
		eRightFootIndex,
		eRightFootMiddle,
		eRightFootRing,
		eRightFootPinky,
		eRightFootExtraFinger,
		eNodeIdCount,
		eNodeIdInvalid=-1
    FbxEffector()
    FbxEffector& operator=(const FbxEffector& pEffector)
    void Reset()
    FbxNode* mNode
    bool mShow
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    bool mTActive
    bool mRActive
    bool mCandidateTActive
    bool mCandidateRActive
#endif 
class FBXSDK_DLL FbxControlSet
public:
    void Reset()
    enum EType
        eNone,
        eFkIk,
        eIkOnly
    void SetType(EType pType)
    EType GetType() const
    void SetUseAxis(bool pUseAxis)
    bool GetUseAxis() const
    void SetLockTransform(bool pLockTransform)
    bool GetLockTransform()const
    void SetLock3DPick(bool pLock3DPick)
    bool GetLock3DPick() const
    bool SetControlSetLink(FbxCharacter::ENodeId pCharacterNodeId, const FbxControlSetLink& pControlSetLink)
    bool GetControlSetLink(FbxCharacter::ENodeId pCharacterNodeId, FbxControlSetLink* pControlSetLink = NULL) const
    bool SetEffector(FbxEffector::ENodeId pEffectorNodeId, FbxEffector pEffector)
    bool GetEffector(FbxEffector::ENodeId pEffectorNodeId, FbxEffector* pEffector = NULL)
    bool SetEffectorAux(FbxEffector::ENodeId pEffectorNodeId, FbxNode* pNode, FbxEffector::ESetId pEffectorSetId=FbxEffector::eAux1Set)
    bool GetEffectorAux(FbxEffector::ENodeId pEffectorNodeId, FbxNode** pNode=NULL, FbxEffector::ESetId pEffectorSetId=FbxEffector::eAux1Set) const
    static char* GetEffectorNodeName(FbxEffector::ENodeId pEffectorNodeId)
    static FbxEffector::ENodeId GetEffectorNodeId(char* pEffectorNodeName)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    void FromPlug(FbxControlSetPlug *pPlug)
    void ToPlug(FbxControlSetPlug *pPlug)
private:
    FbxControlSet()
    ~FbxControlSet()
    FbxCharacter*		mCharacter
    EType				mType
    bool				mUseAxis
    bool				mLockTransform
    bool				mLock3DPick
    FbxControlSetLink	mControlSetLink[FbxCharacter::eNodeIdCount]
    FbxEffector			mEffector[FbxEffector::eNodeIdCount]
    FbxNode*			mEffectorAux[FbxEffector::eNodeIdCount][FbxEffector::eSetIdCount-1]
    FBXSDK_FRIEND_NEW()
    friend class FbxCharacter
    friend class FbxNode
#endif 
class FBXSDK_DLL FbxControlSetPlug : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxControlSetPlug, FbxObject)
public:
	FbxPropertyT<FbxControlSet::EType> ControlSetType
	FbxPropertyT<FbxBool> UseAxis
	FbxPropertyT<FbxReference> Character
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	virtual FbxStringList GetTypeFlags() const
private:
	FbxArray<FbxProperty>    mFKBuf
	FbxArray<FbxProperty>    mIKBuf
	friend class FbxScene
	friend class FbxControlSet
inline EFbxType FbxTypeOf(const FbxControlSet::EType&)
 return eFbxEnum
#include <fbxsdk/fbxsdk_nsend.h>
#endif 