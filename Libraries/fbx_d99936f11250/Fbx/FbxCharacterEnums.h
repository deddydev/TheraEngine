#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{			
		public enum class FbxCharacterOffAutoUser{ ParamModeOff, ParamModeAuto, ParamModeUser };
		public enum class FbxCharacterAutoUser	{ ParamModeAuto2, ParamModeUser2 };

		public enum class FbxCharacterPosture { ParamPostureBiped = 0, ParamPostureQuadriped, LastParamPosture };
		public enum class FbxCharacterFloorPivot { ParamFloorPivotAuto = 0, ParamFloorPivotAnkle, ParamFloorPivotToes, LastParamFloorPivot };
		public enum class FbxCharacterRollExtractionMode { ParamRelativeRollExtraction = 0, ParamAbsoluteRollExtraction, LastRollExtractionMode };
		public enum class FbxCharacterHipsTranslationMode { ParamHipsTranslationWorldRigid = 0, ParamHipsTranslationBodyRigid, LastHipsTranslationMode };
		public enum class FbxCharacterFootContactType	{ ParamFootTypeNormal = 0, ParamFootTypeAnkle, ParamFootTypeToeBase, ParamFootTypeHoof, LastFootContactType };
		public enum class FbxCharacterHandContactType	{ ParamHandTypeNormal = 0, ParamHandTypeWrist, ParamHandTypeFingerBase, ParamHandTypeHoof, LastHandContactType };
		public enum class FbxCharacterFingerContactMode { ParamFingerContactModeSticky, ParamFingerContactModeSpread, ParamFingerContactModeStickySpread, LastFingerContactMode };
		public enum class FbxCharacterContactBehaviour { ParamContactNeverSync = 0, ParamContactSyncOnKey, ParamContactAlwaysSync, LastContactBehaviour };

		public enum class FbxCharacterPropertyUnit{PropertyNoUnit = 0, PropertyPercent,PropertySecond, PropertyCentimeter, PropertyDegree, PropertyEnum,	PropertyReal }; 



		public enum class FbxCharacterInputType
		{ 
			CharacterInputActor = 0, 
			CharacterInputCharacter = 1, 
			CharacterInputMarkerSet = 2, 
			CharacterOutputMarkerSet = 3, 
			CharacterInputStance = 4 
		}; 


		public enum  class FbxCharacterNodeId
		{	
			CharacterHips = 0,             // Required
			CharacterLeftHip,              // Required
			CharacterLeftKnee,             // Required
			CharacterLeftAnkle,            // Required
			CharacterLeftFoot,
			CharacterRightHip,             // Required
			CharacterRightKnee,            // Required
			CharacterRightAnkle,           // Required
			CharacterRightFoot,
			CharacterWaist,				// Spine0, Required
			CharacterChest,		        // Spine1
			CharacterLeftCollar,
			CharacterLeftShoulder,         // Required
			CharacterLeftElbow,            // Required
			CharacterLeftWrist,            // Required
			CharacterRightCollar,
			CharacterRightShoulder,        // Required
			CharacterRightElbow,           // Required
			CharacterRightWrist,           // Required
			CharacterNeck,
			CharacterHead,                 // Required
			CharacterLeftHipRoll,
			CharacterLeftKneeRoll,
			CharacterRightHipRoll,
			CharacterRightKneeRoll,
			CharacterLeftShoulderRoll,
			CharacterLeftElbowRoll,
			CharacterRightShoulderRoll,
			CharacterRightElbowRoll,
			CharacterSpine2,
			CharacterSpine3,
			CharacterSpine4,
			CharacterSpine5,
			CharacterSpine6,
			CharacterSpine7,
			CharacterSpine8,
			CharacterSpine9,
			CharacterLeftThumbA,
			CharacterLeftThumbB,
			CharacterLeftThumbC,
			CharacterLeftIndexA,
			CharacterLeftIndexB,
			CharacterLeftIndexC,
			CharacterLeftMiddleA,
			CharacterLeftMiddleB,
			CharacterLeftMiddleC,
			CharacterLeftRingA,
			CharacterLeftRingB,
			CharacterLeftRingC,
			CharacterLeftPinkyA,
			CharacterLeftPinkyB,
			CharacterLeftPinkyC,
			CharacterRightThumbA,
			CharacterRightThumbB,
			CharacterRightThumbC,
			CharacterRightIndexA,
			CharacterRightIndexB,
			CharacterRightIndexC,
			CharacterRightMiddleA,
			CharacterRightMiddleB,
			CharacterRightMiddleC,
			CharacterRightRingA,
			CharacterRightRingB,
			CharacterRightRingC,
			CharacterRightPinkyA,
			CharacterRightPinkyB,
			CharacterRightPinkyC,
			CharacterReference,
			CharacterLeftFloor,
			CharacterRightFloor,
			CharacterHipsTranslation,
			CharacterProps0,
			CharacterProps1,
			CharacterProps2,
			CharacterProps3,
			CharacterProps4,

			// Added in 4.0x as patch.

			CharacterGameModeParentLeftHipRoll,
			CharacterGameModeParentLeftKnee,
			CharacterGameModeParentLeftKneeRoll,
			CharacterGameModeParentRightHipRoll,
			CharacterGameModeParentRightKnee,
			CharacterGameModeParentRightKneeRoll,
			CharacterGameModeParentLeftShoulderRoll,	
			CharacterGameModeParentLeftElbow,	
			CharacterGameModeParentLeftElbowRoll,	
			CharacterGameModeParentRightShoulderRoll,
			CharacterGameModeParentRightElbow,		
			CharacterGameModeParentRightElbowRoll,	


			// Elements added for Dominus.
			// From this point, the enum doesn't have to be synchronized with the HumanIK library.

			CharacterLeftHandFloor,
			CharacterRightHandFloor,

			CharacterLeftHand,
			CharacterRightHand,

			CharacterNeck1,
			CharacterNeck2,
			CharacterNeck3,
			CharacterNeck4,
			CharacterNeck5,
			CharacterNeck6,
			CharacterNeck7,
			CharacterNeck8,
			CharacterNeck9,

			CharacterLeftInHandThumb,
			CharacterLeftThumbD,

			CharacterLeftInHandIndex,
			CharacterLeftIndexD,

			CharacterLeftInHandMiddle,
			CharacterLeftMiddleD,

			CharacterLeftInHandRing,
			CharacterLeftRingD,

			CharacterLeftInHandPinky,
			CharacterLeftPinkyD,

			CharacterLeftInHandExtraFinger,
			CharacterLeftExtraFingerA,
			CharacterLeftExtraFingerB,
			CharacterLeftExtraFingerC,
			CharacterLeftExtraFingerD,

			CharacterRightInHandThumb,
			CharacterRightThumbD,

			CharacterRightInHandIndex,
			CharacterRightIndexD,

			CharacterRightInHandMiddle,
			CharacterRightMiddleD,

			CharacterRightInHandRing,
			CharacterRightRingD,

			CharacterRightInHandPinky,
			CharacterRightPinkyD,

			CharacterRightInHandExtraFinger,
			CharacterRightExtraFingerA,
			CharacterRightExtraFingerB,
			CharacterRightExtraFingerC,
			CharacterRightExtraFingerD,

			CharacterLeftInFootThumb,
			CharacterLeftFootThumbA,
			CharacterLeftFootThumbB,
			CharacterLeftFootThumbC,
			CharacterLeftFootThumbD,
			CharacterLeftInFootIndex,
			CharacterLeftFootIndexA,
			CharacterLeftFootIndexB,
			CharacterLeftFootIndexC,
			CharacterLeftFootIndexD,
			CharacterLeftInFootMiddle,
			CharacterLeftFootMiddleA,
			CharacterLeftFootMiddleB,
			CharacterLeftFootMiddleC,
			CharacterLeftFootMiddleD,
			CharacterLeftInFootRing,
			CharacterLeftFootRingA,
			CharacterLeftFootRingB,
			CharacterLeftFootRingC,
			CharacterLeftFootRingD,
			CharacterLeftInFootPinky,
			CharacterLeftFootPinkyA,
			CharacterLeftFootPinkyB,
			CharacterLeftFootPinkyC,
			CharacterLeftFootPinkyD,
			CharacterLeftInFootExtraFinger,
			CharacterLeftFootExtraFingerA,
			CharacterLeftFootExtraFingerB,
			CharacterLeftFootExtraFingerC,
			CharacterLeftFootExtraFingerD,

			CharacterRightInFootThumb,
			CharacterRightFootThumbA,
			CharacterRightFootThumbB,
			CharacterRightFootThumbC,
			CharacterRightFootThumbD,
			CharacterRightInFootIndex,
			CharacterRightFootIndexA,
			CharacterRightFootIndexB,
			CharacterRightFootIndexC,
			CharacterRightFootIndexD,
			CharacterRightInFootMiddle,
			CharacterRightFootMiddleA,
			CharacterRightFootMiddleB,
			CharacterRightFootMiddleC,
			CharacterRightFootMiddleD,
			CharacterRightInFootRing,
			CharacterRightFootRingA,
			CharacterRightFootRingB,
			CharacterRightFootRingC,
			CharacterRightFootRingD,
			CharacterRightInFootPinky,
			CharacterRightFootPinkyA,
			CharacterRightFootPinkyB,
			CharacterRightFootPinkyC,
			CharacterRightFootPinkyD,
			CharacterRightInFootExtraFinger,
			CharacterRightFootExtraFingerA,
			CharacterRightFootExtraFingerB,
			CharacterRightFootExtraFingerC,
			CharacterRightFootExtraFingerD,

			CharacterLastNodeId
		};


		public enum class FbxCharacterGroupId
		{
			Base = eCharacterGroup_Base,
			Auxiliary = eCharacterGroup_Auxiliary,
			Spine = eCharacterGroup_Spine,
			Roll = eCharacterGroup_Roll,
			Special = eCharacterGroup_Special,
			LeftHand = eCharacterGroup_LeftHand,
			RightHand = eCharacterGroup_RightHand,
			Props = eCharacterGroup_Props,
			GameModeParent = eCharacterGroup_GameModeParent,

			// Added for 4.5 

			Neck = eCharacterGroup_Neck,
			LeftFoot = eCharacterGroup_LeftFoot,
			RightFoot = eCharacterGroup_RightFoot,

			LastCharacterGroupId = eLastCharacterGroupId
		};


		public enum class FbxCharacterLinkType
		{
			CharacterLink,
			ControlSetLink,
			ControlSetEffector,
			ControlSetEffectorAux
		};
	}
}