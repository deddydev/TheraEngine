#pragma once
#include "stdafx.h"
#include "FbxCharacter.h"
#include "FbxNode.h"
#include "FbxString.h"
#include "FbxVector4.h"
#include "FbxProperty.h"
#include "FbxLimits.h"
#include "FbxString.h"
#include "FbxTypedProperty.h"

#define GETSET_FROM_TYPED_PROPERTY(PropType,PropName)\
	PropType FbxCharacter::PropName::get(){	return _Ref()->PropName.Get();}\
	void FbxCharacter::PropName::set(PropType value){_Ref()->PropName.Set(value);}

#define GETSET_FROM_ENUM_TYPED_PROPERTY(PropType,PropName)\
	Fbx##PropType FbxCharacter::PropName::get(){return (Fbx##PropType)_Ref()->PropName.Get();}\
	void FbxCharacter::PropName::set(Fbx##PropType value){_Ref()->PropName.Set((KCHARACTERDEF_NAMESPACE::k##PropType)value);}

namespace Skill
{
	namespace FbxSDK
	{					
		void FbxCharacterLink::CollectManagedMemory()
		{
			_Node = nullptr;
			_TemplateName = nullptr;
			_OffsetT = nullptr;
			_OffsetR = nullptr;
			_OffsetS = nullptr;
			_ParentROffset = nullptr;
			_RLimits = nullptr;
			_PostRotation = nullptr;
			_PreRotation = nullptr;

			_PropertyLink = nullptr;
			_PropertyOffsetT = nullptr;
			_PropertyOffsetR = nullptr;
			_PropertyOffsetS = nullptr;
			_PropertyParentOffsetR = nullptr;
			_PropertyTemplateName = nullptr;
		}

		FbxCharacterLink::FbxCharacterLink(FbxCharacterLink^ characterLink)
		{
			_SetPointer(new KFbxCharacterLink(*characterLink->_Ref()),true);			
		}			
		void FbxCharacterLink::Reset()
		{
			_Ref()->Reset();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCharacterLink,KFbxNode,mNode,FbxNode,Node);		
		void FbxCharacterLink::Node::set(FbxNode^ value)				
		{
			_Node = value;
			if(value)							
				_Ref()->mNode = _Node->_Ref();
			else
				_Ref()->mNode = NULL;
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mTemplateName,FbxStringManaged,TemplateName);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mOffsetT,FbxVector4,OffsetT);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mOffsetR,FbxVector4,OffsetR);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mOffsetS,FbxVector4,OffsetS);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mParentROffset,FbxVector4,ParentROffset);		

		VALUE_PROPERTY_GETSET_DEFINATION(FbxCharacterLink,mHasRotSpace,bool,HasRotSpace);		

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mRLimits,FbxLimits,RLimits);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPreRotation,FbxVector4,PreRotation);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPostRotation,FbxVector4,PostRotation);

		VALUE_PROPERTY_GETSET_DEFINATION(FbxCharacterLink,mRotOrder,int,RotOrder);		
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCharacterLink,mAxisLen,double,AxisLen);

#ifndef DOXYGEN_SHOULD_SKIP_THIS	

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyLink,FbxProperty,PropertyLink);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyOffsetT,FbxProperty,PropertyOffsetT);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyOffsetR,FbxProperty,PropertyOffsetR);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyOffsetS,FbxProperty,PropertyOffsetS);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyParentOffsetR,FbxProperty,PropertyParentOffsetR);

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxCharacterLink,mPropertyTemplateName,FbxProperty,PropertyTemplateName);					

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		



		void FbxCharacter::CollectManagedMemory()
		{
			_InputObject = nullptr;
			_ChestTOffset = nullptr;
			_HipsTOffset = nullptr;
			FbxConstraint::CollectManagedMemory();
		}

		void FbxCharacter::Reset()
		{
			_Ref()->Reset();
		}
		void FbxCharacter::SetInput(FbxCharacterInputType inputType, FbxObjectManaged^ inputObject)
		{
			_Ref()->SetInput((ECharacterInputType)inputType,inputObject->_Ref());
			_InputObject = inputObject;
		}			
		FbxCharacterInputType FbxCharacter::InputType::get()
		{
			return (FbxCharacterInputType)_Ref()->GetInputType();
		}			
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCharacter,KFbxObject,GetInputObject(),FbxObjectManaged,InputObject);

		bool FbxCharacter::SetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink, bool updateObjectList)
		{
			return _Ref()->SetCharacterLink((ECharacterNodeId)characterNodeId, *characterLink->_Ref(),updateObjectList);
		}
		bool FbxCharacter::SetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink)
		{
			return _Ref()->SetCharacterLink((ECharacterNodeId)characterNodeId, *characterLink->_Ref());
		}
		bool FbxCharacter::GetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink)
		{
			if(characterLink)
				return _Ref()->GetCharacterLink((ECharacterNodeId)characterNodeId,characterLink->_Ref());
			else
				return _Ref()->GetCharacterLink((ECharacterNodeId)characterNodeId);
		}
		int FbxCharacter::GetCharacterGroupCount(FbxCharacterGroupId characterGroupId)
		{
			return KFbxCharacter::GetCharacterGroupCount((ECharacterGroupId)characterGroupId);
		}
		FbxCharacterNodeId FbxCharacter::GetCharacterGroupElementByIndex(FbxCharacterGroupId characterGroupId, int index)
		{
			return (FbxCharacterNodeId)KFbxCharacter::GetCharacterGroupElementByIndex((ECharacterGroupId)characterGroupId,index);
		}			
		String^ FbxCharacter::GetCharacterGroupNameByIndex(FbxCharacterGroupId characterGroupId, int index)
		{
			return gcnew System::String(KFbxCharacter::GetCharacterGroupNameByIndex((ECharacterGroupId)characterGroupId,index));
		}
		int FbxCharacter::GetCharacterGroupVersionByIndex(FbxCharacterGroupId characterGroupId, int index)
		{
			return KFbxCharacter::GetCharacterGroupVersionByIndex((ECharacterGroupId)characterGroupId,index);
		}
		bool FbxCharacter::FindCharacterGroupIndexByName(System::String^ name, bool forceGroupId, [OutAttribute]FbxCharacterGroupId %CharacterGroupId, [OutAttribute]int %index)
		{
			STRINGTOCHAR_ANSI(n,name);

			ECharacterGroupId e;
			int i;
			bool b = KFbxCharacter::FindCharacterGroupIndexByName(n,forceGroupId,e,i);
			FREECHARPOINTER(n);
			if(b)
			{
				index = i;
				CharacterGroupId = (FbxCharacterGroupId)e;
			}
			return b;
		}
		bool FbxCharacter::GetCharacterGroupIndexByElement(FbxCharacterNodeId characterNodeId, [OutAttribute]FbxCharacterGroupId %characterGroupId, [OutAttribute]int %index)
		{
			ECharacterGroupId e;
			int i;
			bool b = KFbxCharacter::GetCharacterGroupIndexByElement((ECharacterNodeId)characterNodeId,e,i);
			if(b)
			{
				index = i;
				characterGroupId = (FbxCharacterGroupId)e;
			}
			return b;
		}
		bool FbxCharacter::GetCharacterGroupVersionByElement(FbxCharacterNodeId characterNodeId,[OutAttribute]int %index)
		{				
			int i;
			bool b = KFbxCharacter::GetCharacterGroupVersionByElement((ECharacterNodeId)characterNodeId,i);
			if(b)
			{
				index = i;					
			}
			return b;
		}
		bool FbxCharacter::GetCharacterNodeNameFromNodeId(FbxCharacterNodeId characterNodeId,[OutAttribute]System::String^ %name)
		{
			char* c;
			bool b = KFbxCharacter::GetCharacterNodeNameFromNodeId((ECharacterNodeId)characterNodeId,c);
			name = gcnew System::String(c);
			return b;
		}
		bool FbxCharacter::GetCharacterNodeIdFromNodeName(System::String^ name, [OutAttribute]FbxCharacterNodeId %characterNodeId)
		{
			STRINGTOCHAR_ANSI(n,name);			
			ECharacterNodeId e;
			bool b = KFbxCharacter::GetCharacterNodeIdFromNodeName(n,e);
			FREECHARPOINTER(n);
			if(b)
				characterNodeId = (FbxCharacterNodeId)e;
			return b;
		}

		GETSET_FROM_TYPED_PROPERTY(bool,Active);			
		GETSET_FROM_TYPED_PROPERTY(bool,Lock);
		GETSET_FROM_TYPED_PROPERTY(double,Weight);					
		GETSET_FROM_TYPED_PROPERTY(double,PullIterationCount);			
		GETSET_FROM_TYPED_PROPERTY(bool,ForceActorSpace);
		GETSET_FROM_TYPED_PROPERTY(double,ScaleCompensation);			
		GETSET_FROM_TYPED_PROPERTY(double,HipsHeightCompensation);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleHeightCompensation);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleProximityCompensation);			
		GETSET_FROM_TYPED_PROPERTY(double,MassCenterCompensation);
		GETSET_FROM_TYPED_PROPERTY(bool,ApplyLimits);
		GETSET_FROM_TYPED_PROPERTY(double,ChestReduction);
		GETSET_FROM_TYPED_PROPERTY(double,CollarReduction);
		GETSET_FROM_TYPED_PROPERTY(double,NeckReduction);
		GETSET_FROM_TYPED_PROPERTY(double,HeadReduction);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftKnee);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightKnee);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorChest);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorHead);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftWrist);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightWrist);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftElbow);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightElbow);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftFingerBase);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightFingerBase);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftToesBase);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightToesBase);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftFingerBaseRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightFingerBaseRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftToesBaseRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightToesBaseRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftAnkleRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightAnkleRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorHeadRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLeftWristRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorRightWristRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorChestRotation);
		GETSET_FROM_TYPED_PROPERTY(double,ReachActorLowerChestRotation);			
		GETSET_FROM_TYPED_PROPERTY(double,LeftUpLegRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftUpLegRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,LeftLegRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftLegRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,RightUpLegRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,RightUpLegRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,RightLegRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,RightLegRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,LeftArmRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftArmRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,LeftForeArmRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftForeArmRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,RightArmRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,RightArmRollMode);
		GETSET_FROM_TYPED_PROPERTY(double,RightForeArmRoll);
		GETSET_FROM_TYPED_PROPERTY(bool,RightForeArmRollMode);			
		GETSET_FROM_TYPED_PROPERTY(bool,FootFloorContact);
		GETSET_FROM_TYPED_PROPERTY(bool,FootAutomaticToes);			
		GETSET_FROM_TYPED_PROPERTY(double,FootBottomToAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,FootBackToAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,FootMiddleToAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,FootFrontToMiddle);
		GETSET_FROM_TYPED_PROPERTY(double,FootInToAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,FootOutToAnkle);
		GETSET_FROM_TYPED_PROPERTY(double,FootContactSize);
		GETSET_FROM_TYPED_PROPERTY(bool,FootFingerContact);			
		GETSET_FROM_TYPED_PROPERTY(double,FootContactStiffness);
		GETSET_FROM_TYPED_PROPERTY(double,FootFingerContactRollStiffness);
		GETSET_FROM_TYPED_PROPERTY(bool,HandFloorContact);
		GETSET_FROM_TYPED_PROPERTY(bool,HandAutomaticFingers);			
		GETSET_FROM_TYPED_PROPERTY(double,HandBottomToWrist);
		GETSET_FROM_TYPED_PROPERTY(double,HandBackToWrist);
		GETSET_FROM_TYPED_PROPERTY(double,HandMiddleToWrist);
		GETSET_FROM_TYPED_PROPERTY(double,HandFrontToMiddle);
		GETSET_FROM_TYPED_PROPERTY(double,HandInToWrist);
		GETSET_FROM_TYPED_PROPERTY(double,HandOutToWrist);
		GETSET_FROM_TYPED_PROPERTY(double,HandContactSize);
		GETSET_FROM_TYPED_PROPERTY(bool,HandFingerContact);			
		GETSET_FROM_TYPED_PROPERTY(double,HandContactStiffness);
		GETSET_FROM_TYPED_PROPERTY(double,HandFingerContactRollStiffness);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandThumbTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandIndexTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandMiddleTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandRingTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandPinkyTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftHandExtraFingerTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandThumbTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandIndexTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandMiddleTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandRingTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandPinkyTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightHandExtraFingerTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootThumbTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootIndexTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootMiddleTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootRingTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootPinkyTip);
		GETSET_FROM_TYPED_PROPERTY(double,LeftFootExtraFingerTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootThumbTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootIndexTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootMiddleTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootRingTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootPinkyTip);
		GETSET_FROM_TYPED_PROPERTY(double,RightFootExtraFingerTip);
		GETSET_FROM_TYPED_PROPERTY(bool,FingerSolving);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftToeBase);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftFoot);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightToeBase);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightFoot);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftFingerBase);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftHand);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullLeftElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightFingerBase);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightHand);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullRightElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlChestPullLeftHand);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlChestPullRightHand);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlPullHead);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistHipsPosition);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlEnforceGravity);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistHipsOrientation);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistChestPosition);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistChestOrientation);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistLeftCollar);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistRightCollar);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistLeftKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistMaximumExtensionLeftKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistCompressionFactorLeftKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistRightKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistMaximumExtensionRightKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistCompressionFactorRightKnee);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistLeftElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistMaximumExtensionLeftElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistCompressionFactorLeftElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistRightElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistMaximumExtensionRightElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlResistCompressionFactorRightElbow);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlSpineStiffness);
		GETSET_FROM_TYPED_PROPERTY(double,CtrlNeckStiffness);
		GETSET_FROM_TYPED_PROPERTY(bool,MirrorMode);
		GETSET_FROM_TYPED_PROPERTY(double,ShoulderCorrection);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftKneeKillPitch);
		GETSET_FROM_TYPED_PROPERTY(bool,RightKneeKillPitch);
		GETSET_FROM_TYPED_PROPERTY(bool,LeftElbowKillPitch);
		GETSET_FROM_TYPED_PROPERTY(bool,RightElbowKillPitch);			
		GETSET_FROM_TYPED_PROPERTY(bool,WriteReference);
		GETSET_FROM_TYPED_PROPERTY(bool,SyncMode);
		GETSET_FROM_TYPED_PROPERTY(double,Damping);
		GETSET_FROM_TYPED_PROPERTY(double,OrientationDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,DisplacementDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,DisplacementMemory);			
		GETSET_FROM_TYPED_PROPERTY(double,HipsDisplacementDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleDisplacementDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,WristDisplacementDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,Stabilization);
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationTime);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationPerimeter);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationAngularPerimeter);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationFloorProximity);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationDamping);			
		GETSET_FROM_TYPED_PROPERTY(double,AnkleStabilizationRecoveryTime);	


		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,OrientationDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,DisplacementDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,DisplacementMemoryMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,HipsDisplacementDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,AnkleDisplacementDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,WristDisplacementDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,AnkleStabilizationTimeMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterAutoUser,AnkleStabilizationPerimeterMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleStabilizationAngularPerimeterMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleStabilizationFloorProximityMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleStabilizationDampingMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleStabilizationRecoveryTimeMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterHipsTranslationMode,HipsTranslationMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterPosture,Posture);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,ScaleCompensationMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,HipsHeightCompensationMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleHeightCompensationMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterOffAutoUser,AnkleProximityCompensationMode);			
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterRollExtractionMode,RollExtractionMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterContactBehaviour,ContactBehaviour);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterFloorPivot,FootFloorPivot);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterFootContactType,FootContactType);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterFingerContactMode,FootFingerContactMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterFloorPivot,HandFloorPivot);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterHandContactType,HandContactType);
		GETSET_FROM_ENUM_TYPED_PROPERTY(CharacterFingerContactMode,HandFingerContactMode);


		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCharacter,HipsTOffset,FbxDouble3TypedProperty,HipsTOffset);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCharacter,ChestTOffset,FbxDouble3TypedProperty,ChestTOffset);		



#ifndef DOXYGEN_SHOULD_SKIP_THIS	

		// Clone
		CLONE_DEFINITION(FbxCharacter,KFbxCharacter);
#endif
	}
}