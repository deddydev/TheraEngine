#ifndef _FBXSDK_SCENE_CONSTRAINT_CHARACTER_H_
#define _FBXSDK_SCENE_CONSTRAINT_CHARACTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/math/fbxtransforms.h>
#include <fbxsdk/scene/constraint/fbxconstraint.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxControlSet
class FBXSDK_DLL FbxCharacterLink
public:
	FbxCharacterLink()
	FbxCharacterLink& operator=(const FbxCharacterLink& pCharacterLink)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	FbxProperty mPropertyLink
	FbxProperty mPropertyOffsetT
	FbxProperty mPropertyOffsetR
	FbxProperty mPropertyOffsetS
	FbxProperty mPropertyParentOffsetR
	FbxProperty mPropertyTemplateName
#endif 
class FBXSDK_DLL FbxCharacter : public FbxConstraint
	FBXSDK_OBJECT_DECLARE(FbxCharacter, FbxConstraint)
public:
	enum EInputType
		eInputActor, 
		eInputCharacter,
		eInputMarkerSet,
		eOutputMarkerSet,
		eInputStancePose
	enum ENodeId
		eHips,
		eLeftHip,
		eLeftKnee,
		eLeftAnkle,
		eLeftFoot,
		eRightHip,
		eRightKnee,
		eRightAnkle,
		eRightFoot,
		eWaist,
		eChest,
		eLeftCollar,
		eLeftShoulder,
		eLeftElbow,
		eLeftWrist,
		eRightCollar,
		eRightShoulder,
		eRightElbow,
		eRightWrist,
		eNeck,
		eHead,
		eLeftHipRoll,
		eLeftKneeRoll,
		eRightHipRoll,
		eRightKneeRoll,
		eLeftShoulderRoll,
		eLeftElbowRoll,
		eRightShoulderRoll,
		eRightElbowRoll,
		eSpine2,
		eSpine3,
		eSpine4,
		eSpine5,
		eSpine6,
		eSpine7,
		eSpine8,
		eSpine9,
		eLeftThumbA,
		eLeftThumbB,
		eLeftThumbC,
		eLeftIndexA,
		eLeftIndexB,
		eLeftIndexC,
		eLeftMiddleA,
		eLeftMiddleB,
		eLeftMiddleC,
		eLeftRingA,
		eLeftRingB,
		eLeftRingC,
		eLeftPinkyA,
		eLeftPinkyB,
		eLeftPinkyC,
		eRightThumbA,
		eRightThumbB,
		eRightThumbC,
		eRightIndexA,
		eRightIndexB,
		eRightIndexC,
		eRightMiddleA,
		eRightMiddleB,
		eRightMiddleC,
		eRightRingA,
		eRightRingB,
		eRightRingC,
		eRightPinkyA,
		eRightPinkyB,
		eRightPinkyC,
		eReference,
		eLeftFloor,
		eRightFloor,
		eHipsTranslation,
		eProps0,
		eProps1,
		eProps2,
		eProps3,
		eProps4,
		eGameModeParentLeftHipRoll,
		eGameModeParentLeftKnee,
		eGameModeParentLeftKneeRoll,
		eGameModeParentRightHipRoll,
		eGameModeParentRightKnee,
		eGameModeParentRightKneeRoll,
		eGameModeParentLeftShoulderRoll,       
		eGameModeParentLeftElbow,      
		eGameModeParentLeftElbowRoll,  
		eGameModeParentRightShoulderRoll,
		eGameModeParentRightElbow,             
		eGameModeParentRightElbowRoll, 
		eLeftUpLegRoll,
		eLeftLegRoll,
		eRightUpLegRoll,
		eRightLegRoll,
		eLeftArmRoll,
		eLeftForeArmRoll,
		eRightArmRoll,
		eRightForeArmRoll,
		eLeftHandFloor,
		eRightHandFloor,
		eLeftHand,
		eRightHand,
		eNeck1,
		eNeck2,
		eNeck3,
		eNeck4,
		eNeck5,
		eNeck6,
		eNeck7,
		eNeck8,
		eNeck9,
		eLeftInHandThumb,
		eLeftThumbD,
		eLeftInHandIndex,
		eLeftIndexD,
		eLeftInHandMiddle,
		eLeftMiddleD,
		eLeftInHandRing,
		eLeftRingD,
		eLeftInHandPinky,
		eLeftPinkyD,
		eLeftInHandExtraFinger,
		eLeftExtraFingerA,
		eLeftExtraFingerB,
		eLeftExtraFingerC,
		eLeftExtraFingerD,
		eRightInHandThumb,
		eRightThumbD,
		eRightInHandIndex,
		eRightIndexD,
		eRightInHandMiddle,
		eRightMiddleD,
		eRightInHandRing,
		eRightRingD,
		eRightInHandPinky,
		eRightPinkyD,
		eRightInHandExtraFinger,
		eRightExtraFingerA,
		eRightExtraFingerB,
		eRightExtraFingerC,
		eRightExtraFingerD,
		eLeftInFootThumb,
		eLeftFootThumbA,
		eLeftFootThumbB,
		eLeftFootThumbC,
		eLeftFootThumbD,
		eLeftInFootIndex,
		eLeftFootIndexA,
		eLeftFootIndexB,
		eLeftFootIndexC,
		eLeftFootIndexD,
		eLeftInFootMiddle,
		eLeftFootMiddleA,
		eLeftFootMiddleB,
		eLeftFootMiddleC,
		eLeftFootMiddleD,
		eLeftInFootRing,
		eLeftFootRingA,
		eLeftFootRingB,
		eLeftFootRingC,
		eLeftFootRingD,
		eLeftInFootPinky,
		eLeftFootPinkyA,
		eLeftFootPinkyB,
		eLeftFootPinkyC,
		eLeftFootPinkyD,
		eLeftInFootExtraFinger,
		eLeftFootExtraFingerA,
		eLeftFootExtraFingerB,
		eLeftFootExtraFingerC,
		eLeftFootExtraFingerD,
		eRightInFootThumb,
		eRightFootThumbA,
		eRightFootThumbB,
		eRightFootThumbC,
		eRightFootThumbD,
		eRightInFootIndex,
		eRightFootIndexA,
		eRightFootIndexB,
		eRightFootIndexC,
		eRightFootIndexD,
		eRightInFootMiddle,
		eRightFootMiddleA,
		eRightFootMiddleB,
		eRightFootMiddleC,
		eRightFootMiddleD,
		eRightInFootRing,
		eRightFootRingA,
		eRightFootRingB,
		eRightFootRingC,
		eRightFootRingD,
		eRightInFootPinky,
		eRightFootPinkyA,
		eRightFootPinkyB,
		eRightFootPinkyC,
		eRightFootPinkyD,
		eRightInFootExtraFinger,
		eRightFootExtraFingerA,
		eRightFootExtraFingerB,
		eRightFootExtraFingerC,
		eRightFootExtraFingerD,
		eLeftCollarExtra,
		eRightCollarExtra,
        eLeafLeftHipRoll1,
        eLeafLeftKneeRoll1,
        eLeafRightHipRoll1,
        eLeafRightKneeRoll1,
        eLeafLeftShoulderRoll1,
        eLeafLeftElbowRoll1,
        eLeafRightShoulderRoll1,
        eLeafRightElbowRoll1,
        eLeafLeftHipRoll2,
        eLeafLeftKneeRoll2,
        eLeafRightHipRoll2,
        eLeafRightKneeRoll2,
        eLeafLeftShoulderRoll2,
        eLeafLeftElbowRoll2,
        eLeafRightShoulderRoll2,
        eLeafRightElbowRoll2,
        eLeafLeftHipRoll3,
        eLeafLeftKneeRoll3,
        eLeafRightHipRoll3,
        eLeafRightKneeRoll3,
        eLeafLeftShoulderRoll3,
        eLeafLeftElbowRoll3,
        eLeafRightShoulderRoll3,
        eLeafRightElbowRoll3,
        eLeafLeftHipRoll4,
        eLeafLeftKneeRoll4,
        eLeafRightHipRoll4,
        eLeafRightKneeRoll4,
        eLeafLeftShoulderRoll4,
        eLeafLeftElbowRoll4,
        eLeafRightShoulderRoll4,
        eLeafRightElbowRoll4,
        eLeafLeftHipRoll5,
        eLeafLeftKneeRoll5,
        eLeafRightHipRoll5,
        eLeafRightKneeRoll5,
        eLeafLeftShoulderRoll5,
        eLeafLeftElbowRoll5,
        eLeafRightShoulderRoll5,
        eLeafRightElbowRoll5,
		eNodeIdCount,
		eNodeIdInvalid=-1
	enum EOffAutoUser
		eParamModeOff,
		eParamModeAuto,
		eParamModeUser
	enum EAutoUser
		eParamModeAuto2,
		eParamModeUser2
	enum EPostureMode
		ePostureBiped,
		ePostureQuadriped,
		ePostureCount
	enum EFloorPivot
		eFloorPivotAuto,
		eFloorPivotAnkle,
		eFloorPivotToes,
		eFloorPivotCount
	enum ERollExtractionMode
		eRelativeRollExtraction,
		eAbsoluteRollExtraction,
		eRollExtractionTypeCount
	enum EHipsTranslationMode
		eHipsTranslationWorldRigid,
		eHipsTranslationBodyRigid,
		eHipsTranslationTypeCount
	enum EFootContactType
		eFootTypeNormal,
		eFootTypeAnkle,
		eFootTypeToeBase,
		eFootTypeHoof,
		eFootContactModeCount
	enum EHandContactType
		eHandTypeNormal,
		eHandTypeWrist,
		eHandTypeFingerBase,
		eHandTypeHoof,
		eHandContactModeCount
	enum EFingerContactMode
		eFingerContactModeSticky,
		eFingerContactModeSpread,
		eFingerContactModeStickySpread,
		eFingerContactModeCount
	enum EContactBehaviour
		eContactNeverSync,
		eContactSyncOnKey,
		eContactAlwaysSync,
		eContactBehaviorCount
	enum EPropertyUnit
		ePropertyNoUnit,
		ePropertyPercent,
		ePropertySecond,
		ePropertyCentimeter,
		ePropertyDegree,
		ePropertyEnum,
		ePropertyReal
	enum EErrorCode
		eInternalError,
		eErrorCount
	void Reset()
	void SetInput(EInputType pInputType, FbxObject* pInputObject = NULL)
	EInputType GetInputType() const
	FbxObject* GetInputObject() const
	bool SetCharacterLink(ENodeId pCharacterNodeId, const FbxCharacterLink& pCharacterLink, bool pUpdateObjectList = true)
	bool GetCharacterLink(ENodeId pCharacterNodeId, FbxCharacterLink* pCharacterLink = NULL) const
	FbxControlSet& GetControlSet() const
	static int GetCharacterGroupCount(EGroupId pCharacterGroupId)
	static ENodeId GetCharacterGroupElementByIndex(EGroupId pCharacterGroupId, int pIndex)
	static char* GetCharacterGroupNameByIndex(EGroupId pCharacterGroupId, int pIndex)
	static int GetCharacterGroupVersionByIndex(EGroupId pCharacterGroupId, int pIndex)
	static bool FindCharacterGroupIndexByName(const char* pName, bool pForceGroupId, EGroupId& pCharacterGroupId, int& pIndex)
	static bool GetCharacterGroupIndexByElement(ENodeId pCharacterNodeId, EGroupId& pCharacterGroupId, int& pIndex)
	static bool GetCharacterGroupVersionByElement(ENodeId pCharacterNodeId, int& pVersion)
	static bool GetCharacterNodeNameFromNodeId(ENodeId pCharacterNodeId, char*& pName)
	static bool GetCharacterNodeIdFromNodeName(const char* pName, ENodeId& pCharacterNodeId)
	FbxPropertyT<FbxInt>				PullIterationCount
	FbxPropertyT<EPostureMode>			Posture
	FbxPropertyT<FbxBool>				ForceActorSpace
	FbxPropertyT<FbxDouble>				ScaleCompensation
	FbxPropertyT<EOffAutoUser>			ScaleCompensationMode
	FbxPropertyT<FbxDouble>				HipsHeightCompensation
	FbxPropertyT<EOffAutoUser>			HipsHeightCompensationMode
	FbxPropertyT<FbxDouble>				AnkleHeightCompensation
	FbxPropertyT<EOffAutoUser>			AnkleHeightCompensationMode
	FbxPropertyT<FbxDouble>				AnkleProximityCompensation
	FbxPropertyT<EOffAutoUser>			AnkleProximityCompensationMode
	FbxPropertyT<FbxDouble>				MassCenterCompensation
	FbxPropertyT<FbxBool>				ApplyLimits
	FbxPropertyT<FbxDouble>				ChestReduction
	FbxPropertyT<FbxDouble>				CollarReduction
	FbxPropertyT<FbxDouble>				NeckReduction
	FbxPropertyT<FbxDouble>				HeadReduction
	FbxPropertyT<FbxDouble>				ReachActorLeftAnkle
	FbxPropertyT<FbxDouble>				ReachActorRightAnkle
	FbxPropertyT<FbxDouble>				ReachActorLeftKnee
	FbxPropertyT<FbxDouble>				ReachActorRightKnee
	FbxPropertyT<FbxDouble>				ReachActorChest
	FbxPropertyT<FbxDouble>				ReachActorHead
	FbxPropertyT<FbxDouble>				ReachActorLeftWrist
	FbxPropertyT<FbxDouble>				ReachActorRightWrist
	FbxPropertyT<FbxDouble>				ReachActorLeftElbow
	FbxPropertyT<FbxDouble>				ReachActorRightElbow
	FbxPropertyT<FbxDouble>				ReachActorLeftFingerBase
	FbxPropertyT<FbxDouble>				ReachActorRightFingerBase
	FbxPropertyT<FbxDouble>				ReachActorLeftToesBase
	FbxPropertyT<FbxDouble>				ReachActorRightToesBase
	FbxPropertyT<FbxDouble>				ReachActorLeftFingerBaseRotation
	FbxPropertyT<FbxDouble>				ReachActorRightFingerBaseRotation
	FbxPropertyT<FbxDouble>				ReachActorLeftToesBaseRotation
	FbxPropertyT<FbxDouble>				ReachActorRightToesBaseRotation
	FbxPropertyT<FbxDouble>				ReachActorLeftAnkleRotation
	FbxPropertyT<FbxDouble>				ReachActorRightAnkleRotation
	FbxPropertyT<FbxDouble>				ReachActorHeadRotation
	FbxPropertyT<FbxDouble>				ReachActorLeftWristRotation
	FbxPropertyT<FbxDouble>				ReachActorRightWristRotation
	FbxPropertyT<FbxDouble>				ReachActorChestRotation
	FbxPropertyT<FbxDouble>				ReachActorLowerChestRotation
	FbxPropertyT<FbxDouble3>			HipsTOffset
	FbxPropertyT<FbxDouble3>			ChestTOffset
	FbxPropertyT<ERollExtractionMode>	RollExtractionMode
	FbxPropertyT<FbxDouble>				LeftUpLegRoll
	FbxPropertyT<FbxBool>				LeftUpLegRollMode
	FbxPropertyT<FbxDouble>				LeftLegRoll
	FbxPropertyT<FbxBool>				LeftLegRollMode
	FbxPropertyT<FbxDouble>				RightUpLegRoll
	FbxPropertyT<FbxBool>				RightUpLegRollMode
	FbxPropertyT<FbxDouble>				RightLegRoll
	FbxPropertyT<FbxBool>				RightLegRollMode
	FbxPropertyT<FbxDouble>				LeftArmRoll
	FbxPropertyT<FbxBool>				LeftArmRollMode
	FbxPropertyT<FbxDouble>				LeftForeArmRoll
	FbxPropertyT<FbxBool>				LeftForeArmRollMode
	FbxPropertyT<FbxDouble>				RightArmRoll
	FbxPropertyT<FbxBool>				RightArmRollMode
	FbxPropertyT<FbxDouble>				RightForeArmRoll
	FbxPropertyT<FbxBool>				RightForeArmRollMode
	FbxPropertyT<FbxDouble>				LeftUpLegRollEx
	FbxPropertyT<FbxBool>				LeftUpLegRollExMode
	FbxPropertyT<FbxDouble>				LeftLegRollEx
	FbxPropertyT<FbxBool>				LeftLegRollExMode
	FbxPropertyT<FbxDouble>				RightUpLegRollEx
	FbxPropertyT<FbxBool>				RightUpLegRollExMode
	FbxPropertyT<FbxDouble>				RightLegRollEx
	FbxPropertyT<FbxBool>				RightLegRollExMode
	FbxPropertyT<FbxDouble>				LeftArmRollEx
	FbxPropertyT<FbxBool>				LeftArmRollExMode
	FbxPropertyT<FbxDouble>				LeftForeArmRollEx
	FbxPropertyT<FbxBool>				LeftForeArmRollExMode
	FbxPropertyT<FbxDouble>				RightArmRollEx
	FbxPropertyT<FbxBool>				RightArmRollExMode
	FbxPropertyT<FbxDouble>				RightForeArmExRoll
	FbxPropertyT<FbxBool>				RightForeArmRollExMode
	FbxPropertyT<EContactBehaviour>		ContactBehaviour
	FbxPropertyT<FbxBool>				FootFloorContact
	FbxPropertyT<FbxBool>				FootAutomaticToes
	FbxPropertyT<EFloorPivot>			FootFloorPivot
	FbxPropertyT<FbxDouble>				FootBottomToAnkle
	FbxPropertyT<FbxDouble>				FootBackToAnkle
	FbxPropertyT<FbxDouble>				FootMiddleToAnkle
	FbxPropertyT<FbxDouble>				FootFrontToMiddle
	FbxPropertyT<FbxDouble>				FootInToAnkle
	FbxPropertyT<FbxDouble>				FootOutToAnkle
	FbxPropertyT<FbxDouble>				FootContactSize
	FbxPropertyT<FbxBool>				FootFingerContact
	FbxPropertyT<EFootContactType>		FootContactType
	FbxPropertyT<EFingerContactMode>	FootFingerContactMode
	FbxPropertyT<FbxDouble>				FootContactStiffness
	FbxPropertyT<FbxDouble>				FootFingerContactRollStiffness
	FbxPropertyT<FbxBool>				HandFloorContact
	FbxPropertyT<FbxBool>				HandAutomaticFingers
	FbxPropertyT<EFloorPivot>			HandFloorPivot
	FbxPropertyT<FbxDouble>				HandBottomToWrist
	FbxPropertyT<FbxDouble>				HandBackToWrist
	FbxPropertyT<FbxDouble>				HandMiddleToWrist
	FbxPropertyT<FbxDouble>				HandFrontToMiddle
	FbxPropertyT<FbxDouble>				HandInToWrist
	FbxPropertyT<FbxDouble>				HandOutToWrist
	FbxPropertyT<FbxDouble>				HandContactSize
	FbxPropertyT<FbxBool>				HandFingerContact
	FbxPropertyT<EHandContactType>		HandContactType
	FbxPropertyT<EFingerContactMode>	HandFingerContactMode
	FbxPropertyT<FbxDouble>				HandContactStiffness
	FbxPropertyT<FbxDouble>				HandFingerContactRollStiffness
	FbxPropertyT<FbxDouble>				LeftHandThumbTip
	FbxPropertyT<FbxDouble>				LeftHandIndexTip
	FbxPropertyT<FbxDouble>				LeftHandMiddleTip
	FbxPropertyT<FbxDouble>				LeftHandRingTip
	FbxPropertyT<FbxDouble>				LeftHandPinkyTip
	FbxPropertyT<FbxDouble>				LeftHandExtraFingerTip
	FbxPropertyT<FbxDouble>				RightHandThumbTip
	FbxPropertyT<FbxDouble>				RightHandIndexTip
	FbxPropertyT<FbxDouble>				RightHandMiddleTip
	FbxPropertyT<FbxDouble>				RightHandRingTip
	FbxPropertyT<FbxDouble>				RightHandPinkyTip
	FbxPropertyT<FbxDouble>				RightHandExtraFingerTip
	FbxPropertyT<FbxDouble>				LeftFootThumbTip
	FbxPropertyT<FbxDouble>				LeftFootIndexTip
	FbxPropertyT<FbxDouble>				LeftFootMiddleTip
	FbxPropertyT<FbxDouble>				LeftFootRingTip
	FbxPropertyT<FbxDouble>				LeftFootPinkyTip
	FbxPropertyT<FbxDouble>				LeftFootExtraFingerTip
	FbxPropertyT<FbxDouble>				RightFootThumbTip
	FbxPropertyT<FbxDouble>				RightFootIndexTip
	FbxPropertyT<FbxDouble>				RightFootMiddleTip
	FbxPropertyT<FbxDouble>				RightFootRingTip
	FbxPropertyT<FbxDouble>				RightFootPinkyTip
	FbxPropertyT<FbxDouble>				RightFootExtraFingerTip
	FbxPropertyT<FbxBool>				FingerSolving
	FbxPropertyT<FbxDouble>				CtrlPullLeftToeBase
	FbxPropertyT<FbxDouble>				CtrlPullLeftFoot
	FbxPropertyT<FbxDouble>				CtrlPullLeftKnee
	FbxPropertyT<FbxDouble>				CtrlPullRightToeBase
	FbxPropertyT<FbxDouble>				CtrlPullRightFoot
	FbxPropertyT<FbxDouble>				CtrlPullRightKnee
	FbxPropertyT<FbxDouble>				CtrlPullLeftFingerBase
	FbxPropertyT<FbxDouble>				CtrlPullLeftHand
	FbxPropertyT<FbxDouble>				CtrlPullLeftElbow
	FbxPropertyT<FbxDouble>				CtrlPullRightFingerBase
	FbxPropertyT<FbxDouble>				CtrlPullRightHand
	FbxPropertyT<FbxDouble>				CtrlPullRightElbow
	FbxPropertyT<FbxDouble>				CtrlChestPullLeftHand
	FbxPropertyT<FbxDouble>				CtrlChestPullRightHand
	FbxPropertyT<FbxDouble>				CtrlPullHead
	FbxPropertyT<FbxDouble>				CtrlResistHipsPosition
	FbxPropertyT<FbxDouble>				CtrlEnforceGravity
	FbxPropertyT<FbxDouble>				CtrlResistHipsOrientation
	FbxPropertyT<FbxDouble>				CtrlResistChestPosition
	FbxPropertyT<FbxDouble>				CtrlResistChestOrientation
	FbxPropertyT<FbxDouble>				CtrlResistLeftCollar
	FbxPropertyT<FbxDouble>				CtrlResistRightCollar
	FbxPropertyT<FbxDouble>				CtrlResistLeftKnee
	FbxPropertyT<FbxDouble>				CtrlResistMaximumExtensionLeftKnee
	FbxPropertyT<FbxDouble>				CtrlResistCompressionFactorLeftKnee
	FbxPropertyT<FbxDouble>				CtrlResistRightKnee
	FbxPropertyT<FbxDouble>				CtrlResistMaximumExtensionRightKnee
	FbxPropertyT<FbxDouble>				CtrlResistCompressionFactorRightKnee
	FbxPropertyT<FbxDouble>				CtrlResistLeftElbow
	FbxPropertyT<FbxDouble>				CtrlResistMaximumExtensionLeftElbow
	FbxPropertyT<FbxDouble>				CtrlResistCompressionFactorLeftElbow
	FbxPropertyT<FbxDouble>				CtrlResistRightElbow
	FbxPropertyT<FbxDouble>				CtrlResistMaximumExtensionRightElbow
	FbxPropertyT<FbxDouble>				CtrlResistCompressionFactorRightElbow
	FbxPropertyT<FbxDouble>				CtrlSpineStiffness
	FbxPropertyT<FbxDouble>				CtrlNeckStiffness
	FbxPropertyT<FbxBool>				MirrorMode
	FbxPropertyT<FbxDouble>				ShoulderCorrection
	FbxPropertyT<FbxBool>				LeftKneeKillPitch
	FbxPropertyT<FbxBool>				RightKneeKillPitch
	FbxPropertyT<FbxBool>				LeftElbowKillPitch
	FbxPropertyT<FbxBool>				RightElbowKillPitch
	FbxPropertyT<EHipsTranslationMode>	HipsTranslationMode
	FbxPropertyT<FbxBool>				WriteReference
	FbxPropertyT<FbxBool>				SyncMode
	FbxPropertyT<FbxDouble>				Damping
	FbxPropertyT<FbxDouble>				OrientationDamping
	FbxPropertyT<EOffAutoUser>			OrientationDampingMode
	FbxPropertyT<FbxDouble>				DisplacementDamping
	FbxPropertyT<EOffAutoUser>			DisplacementDampingMode
	FbxPropertyT<FbxDouble>				DisplacementMemory
	FbxPropertyT<EAutoUser>				DisplacementMemoryMode
	FbxPropertyT<FbxDouble>				HipsDisplacementDamping
	FbxPropertyT<EAutoUser>				HipsDisplacementDampingMode
	FbxPropertyT<FbxDouble>				AnkleDisplacementDamping
	FbxPropertyT<EAutoUser>				AnkleDisplacementDampingMode
	FbxPropertyT<FbxDouble>				WristDisplacementDamping
	FbxPropertyT<EAutoUser>				WristDisplacementDampingMode
	FbxPropertyT<FbxDouble>				Stabilization
	FbxPropertyT<FbxDouble>				AnkleStabilizationTime
	FbxPropertyT<EAutoUser>				AnkleStabilizationTimeMode
	FbxPropertyT<FbxDouble>				AnkleStabilizationPerimeter
	FbxPropertyT<EAutoUser>				AnkleStabilizationPerimeterMode
	FbxPropertyT<FbxDouble>				AnkleStabilizationAngularPerimeter
	FbxPropertyT<EOffAutoUser>			AnkleStabilizationAngularPerimeterMode
	FbxPropertyT<FbxDouble>				AnkleStabilizationFloorProximity
	FbxPropertyT<EOffAutoUser>			AnkleStabilizationFloorProximityMode
	FbxPropertyT<FbxDouble>				AnkleStabilizationDamping
	FbxPropertyT<EOffAutoUser>			AnkleStabilizationDampingMode
	FbxPropertyT<FbxDouble>				AnkleStabilizationRecoveryTime
	FbxPropertyT<EOffAutoUser>			AnkleStabilizationRecoveryTimeMode
	FbxPropertyT<FbxReference>			SourceObject
	FbxPropertyT<FbxReference>			DestinationObject
	FbxPropertyT<FbxReference>			Actor
	FbxPropertyT<FbxReference>			Character
	FbxPropertyT<FbxReference>			ControlSet
	FbxPropertyT<FbxDouble>				HikVersion
	FbxPropertyT<FbxBool>				Characterize
	FbxPropertyT<FbxBool>				LockXForm
	FbxPropertyT<FbxBool>				LockPick
    FbxPropertyT<FbxDouble>             RealisticShoulder
    FbxPropertyT<FbxDouble>             CollarStiffnessX
    FbxPropertyT<FbxDouble>             CollarStiffnessY
    FbxPropertyT<FbxDouble>             CollarStiffnessZ
    FbxPropertyT<FbxDouble>             ExtraCollarRatio
    FbxPropertyT<FbxDouble>             LeftLegMaxExtensionAngle
    FbxPropertyT<FbxDouble>             RightLegMaxExtensionAngle
    FbxPropertyT<FbxDouble>             LeftArmMaxExtensionAngle
    FbxPropertyT<FbxDouble>             RightArmMaxExtensionAngle
    FbxPropertyT<FbxDouble>             StretchStartArmsAndLegs
    FbxPropertyT<FbxDouble>             StretchStopArmsAndLegs
    FbxPropertyT<FbxDouble>             SnSScaleArmsAndLegs
    FbxPropertyT<FbxDouble>             SnSReachLeftWrist
    FbxPropertyT<FbxDouble>             SnSReachRightWrist
    FbxPropertyT<FbxDouble>             SnSReachLeftAnkle
    FbxPropertyT<FbxDouble>             SnSReachRightAnkle
    FbxPropertyT<FbxDouble>             SnSScaleSpine
    FbxPropertyT<FbxDouble>             SnSScaleSpineChildren
    FbxPropertyT<FbxDouble>             SnSSpineFreedom
    FbxPropertyT<FbxDouble>             SnSReachChestEnd
    FbxPropertyT<FbxDouble>             SnSScaleNeck
    FbxPropertyT<FbxDouble>             SnSNeckFreedom
    FbxPropertyT<FbxDouble>             SnSReachHead
    FbxPropertyT<FbxDouble>				LeafLeftUpLegRoll1
    FbxPropertyT<FbxBool>				LeafLeftUpLegRoll1Mode
    FbxPropertyT<FbxDouble>				LeafLeftLegRoll1
    FbxPropertyT<FbxBool>				LeafLeftLegRoll1Mode
    FbxPropertyT<FbxDouble>				LeafRightUpLegRoll1
    FbxPropertyT<FbxBool>				LeafRightUpLegRoll1Mode
    FbxPropertyT<FbxDouble>				LeafRightLegRoll1
    FbxPropertyT<FbxBool>				LeafRightLegRoll1Mode
    FbxPropertyT<FbxDouble>				LeafLeftArmRoll1
    FbxPropertyT<FbxBool>				LeafLeftArmRoll1Mode
    FbxPropertyT<FbxDouble>				LeafLeftForeArmRoll1
    FbxPropertyT<FbxBool>				LeafLeftForeArmRoll1Mode
    FbxPropertyT<FbxDouble>				LeafRightArmRoll1
    FbxPropertyT<FbxBool>				LeafRightArmRoll1Mode
    FbxPropertyT<FbxDouble>				LeafRightForeArmRoll1
    FbxPropertyT<FbxBool>				LeafRightForeArmRoll1Mode
    FbxPropertyT<FbxDouble>				LeafLeftUpLegRoll2
    FbxPropertyT<FbxBool>				LeafLeftUpLegRoll2Mode
    FbxPropertyT<FbxDouble>				LeafLeftLegRoll2
    FbxPropertyT<FbxBool>				LeafLeftLegRoll2Mode
    FbxPropertyT<FbxDouble>				LeafRightUpLegRoll2
    FbxPropertyT<FbxBool>				LeafRightUpLegRoll2Mode
    FbxPropertyT<FbxDouble>				LeafRightLegRoll2
    FbxPropertyT<FbxBool>				LeafRightLegRoll2Mode
    FbxPropertyT<FbxDouble>				LeafLeftArmRoll2
    FbxPropertyT<FbxBool>				LeafLeftArmRoll2Mode
    FbxPropertyT<FbxDouble>				LeafLeftForeArmRoll2
    FbxPropertyT<FbxBool>				LeafLeftForeArmRoll2Mode
    FbxPropertyT<FbxDouble>				LeafRightArmRoll2
    FbxPropertyT<FbxBool>				LeafRightArmRoll2Mode
    FbxPropertyT<FbxDouble>				LeafRightForeArmRoll2
    FbxPropertyT<FbxBool>				LeafRightForeArmRoll2Mode
    FbxPropertyT<FbxDouble>				LeafLeftUpLegRoll3
	FbxPropertyT<FbxBool>				LeafLeftUpLegRoll3Mode
	FbxPropertyT<FbxDouble>				LeafLeftLegRoll3
	FbxPropertyT<FbxBool>				LeafLeftLegRoll3Mode
	FbxPropertyT<FbxDouble>				LeafRightUpLegRoll3
	FbxPropertyT<FbxBool>				LeafRightUpLegRoll3Mode
	FbxPropertyT<FbxDouble>				LeafRightLegRoll3
	FbxPropertyT<FbxBool>				LeafRightLegRoll3Mode
	FbxPropertyT<FbxDouble>				LeafLeftArmRoll3
	FbxPropertyT<FbxBool>				LeafLeftArmRoll3Mode
	FbxPropertyT<FbxDouble>				LeafLeftForeArmRoll3
	FbxPropertyT<FbxBool>				LeafLeftForeArmRoll3Mode
	FbxPropertyT<FbxDouble>				LeafRightArmRoll3
	FbxPropertyT<FbxBool>				LeafRightArmRoll3Mode
	FbxPropertyT<FbxDouble>				LeafRightForeArmRoll3
	FbxPropertyT<FbxBool>				LeafRightForeArmRoll3Mode
    FbxPropertyT<FbxDouble>				LeafLeftUpLegRoll4
    FbxPropertyT<FbxBool>				LeafLeftUpLegRoll4Mode
    FbxPropertyT<FbxDouble>				LeafLeftLegRoll4
    FbxPropertyT<FbxBool>				LeafLeftLegRoll4Mode
    FbxPropertyT<FbxDouble>				LeafRightUpLegRoll4
    FbxPropertyT<FbxBool>				LeafRightUpLegRoll4Mode
    FbxPropertyT<FbxDouble>				LeafRightLegRoll4
    FbxPropertyT<FbxBool>				LeafRightLegRoll4Mode
    FbxPropertyT<FbxDouble>				LeafLeftArmRoll4
    FbxPropertyT<FbxBool>				LeafLeftArmRoll4Mode
    FbxPropertyT<FbxDouble>				LeafLeftForeArmRoll4
    FbxPropertyT<FbxBool>				LeafLeftForeArmRoll4Mode
    FbxPropertyT<FbxDouble>				LeafRightArmRoll4
    FbxPropertyT<FbxBool>				LeafRightArmRoll4Mode
    FbxPropertyT<FbxDouble>				LeafRightForeArmRoll4
    FbxPropertyT<FbxBool>				LeafRightForeArmRoll4Mode
    FbxPropertyT<FbxDouble>				LeafLeftUpLegRoll5
    FbxPropertyT<FbxBool>				LeafLeftUpLegRoll5Mode
    FbxPropertyT<FbxDouble>				LeafLeftLegRoll5
    FbxPropertyT<FbxBool>				LeafLeftLegRoll5Mode
    FbxPropertyT<FbxDouble>				LeafRightUpLegRoll5
    FbxPropertyT<FbxBool>				LeafRightUpLegRoll5Mode
    FbxPropertyT<FbxDouble>				LeafRightLegRoll5
    FbxPropertyT<FbxBool>				LeafRightLegRoll5Mode
    FbxPropertyT<FbxDouble>				LeafLeftArmRoll5
    FbxPropertyT<FbxBool>				LeafLeftArmRoll5Mode
    FbxPropertyT<FbxDouble>				LeafLeftForeArmRoll5
    FbxPropertyT<FbxBool>				LeafLeftForeArmRoll5Mode
    FbxPropertyT<FbxDouble>				LeafRightArmRoll5
    FbxPropertyT<FbxBool>				LeafRightArmRoll5Mode
    FbxPropertyT<FbxDouble>				LeafRightForeArmRoll5
    FbxPropertyT<FbxBool>				LeafRightForeArmRoll5Mode
    FbxPropertyT<FbxDouble>				LeftLegFullRollExtraction
    FbxPropertyT<FbxDouble>				RightLegFullRollExtraction
    FbxPropertyT<FbxDouble>				LeftArmFullRollExtraction
    FbxPropertyT<FbxDouble>				RightArmFullRollExtraction
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	void	SetVersion(int pVersion)
 mCharacterVersion = pVersion
	int		Version()
 return mCharacterVersion
	void	SetValuesFromLegacyLoad()
	void	SetValuesForLegacySave(int pVersion)
	void	RestoreValuesFromLegacySave()
	bool	IsLegacy()
	int		GetPropertyInfoCount()
	void	GetPropertyInfo(char* &pCharacterPropertyName, char* &pCharacterPropertyModeName, EPropertyUnit &pUnit, int &pPropertyIndex, char* &pHIKPropertyName, char* &pHIKPropertyModeName, int pIndex) const
	void	GetFbxCharacterPropertyFromHIKProperty(char* &pCharacterPropertyName, char* &pCharacterPropertyModeName, EPropertyUnit &pUnit, int &pPropertyIndex, const char* pHIKPropertyName) const
    FbxCharacterLink*	GetCharacterLinkPtr(ENodeId pCharacterNodeId)
    virtual FbxObject*	Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
	virtual FbxObject&		Copy(const FbxObject& pObject)
	virtual EType			GetConstraintType() const
	virtual FbxStringList	GetTypeFlags() const
	virtual bool			ConnectNotify (FbxConnectEvent const &pEvent)
private:
	bool					InverseProperty(FbxProperty& pProp)
	int						mCharacterVersion
	FbxCharacterLink		mCharacterLink[eNodeIdCount]
	FbxControlSet*			mControlSet
	friend class FbxNode
#endif 