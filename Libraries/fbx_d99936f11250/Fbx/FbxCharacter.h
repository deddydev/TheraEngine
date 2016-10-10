#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxConstraint.h"
#include "FbxCharacterEnums.h"

using namespace System::Runtime::InteropServices;


{
	namespace FbxSDK
	{		
		ref class FbxConstraint;
		ref class FbxNode;
		ref class FbxStringManaged;
		ref class FbxVector4;
		ref class FbxPropertyManaged;
		ref class FbxLimits;
		ref class FbxStringManaged;
		ref class FbxDouble3TypedProperty;		

		/** \class KFbxCharacterLink
		*
		* \brief This class represents a link between a given character's node and the associated node in the hierarchy. It also contains
		*  the characterization matrix (offset) for the linked character's node.
		*/
		public ref class FbxCharacterLink : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxCharacterLink,KFbxCharacterLink);
			INATIVEPOINTER_DECLARE(FbxCharacterLink,KFbxCharacterLink);		
		public:

			void Reset();

			DEFAULT_CONSTRUCTOR(FbxCharacterLink,KFbxCharacterLink);

			FbxCharacterLink(FbxCharacterLink^ characterLink);

			REF_PROPERTY_GETSET_DECLARE(FbxNode,Node);			

			//! A template name is a naming convention that is used to automatically map
			//! the nodes of other skeletons that use the same naming convention.
			REF_PROPERTY_GETSET_DECLARE(FbxStringManaged,TemplateName);			

			//! Get offset position of this character link.
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,OffsetT);
			//! Get offset rotation of this character link.
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,OffsetR);
			//! Get offset scale of this character link.
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,OffsetS);
			//! Get the parent offset rotation of this character link			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,ParentROffset);			

			//! \c true if this character link has a defined rotation space
			property bool HasRotSpace
			{
				bool get();
				void set(bool value);
			}
			//! Get the rotation limits of this character link
			REF_PROPERTY_GETSET_DECLARE(FbxLimits,RLimits);

			//! Get the PreRotation of this character link			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,PreRotation);

			//! Get the PostRotation of this character link	
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,PostRotation);

			//! Get the rotation order of this character link			
			property int RotOrder
			{
				int get();
				void set(int value);
			}
			//! Get the axis length of this character link			
			property double AxisLen
			{
				double get();
				void set(double value);
			}




			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS	

			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyLink);
			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyOffsetT);
			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyOffsetR);
			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyOffsetS);
			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyParentOffsetR);
			REF_PROPERTY_GETSET_DECLARE(FbxPropertyManaged,PropertyTemplateName);			

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

		/** \class KFbxCharacter
		*
		* \brief This class contains all methods to setup an exported character or query information on an imported character.
		*
		* This class also contains some methods for manipulating the KFbxCharacterLink, KFbxControlSet
		* 
		* The most important part of a KFbxCharacter is the KFbxCharacterLink. There is one KFbxCharacterLink for each characterized node.
		* This class contains the associated KFbxNode to the node represented by this link and also the characterization offset of this node. 
		* For more information see KFbxCharacterLink class documentation. 
		*   
		*
		*/
		public ref class FbxCharacter : FbxConstraint
		{
			REF_DECLARE(FbxEmitter,KFbxCharacter);
		internal:
			FbxCharacter(KFbxCharacter* instance): FbxConstraint(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
		public:			

			/** Reset to default values. 
			*     - Input type will be set to eCharacterInputStance.
			*     - Input object will be set to NULL.
			*     - Each Character link will be reset.
			*     - The control set will be reset.
			*/
			void Reset();

			/** Set input type and index.
			* \param pInputType       Input type.
			* \param pInputObject     Pointer to input character if input type equals eCharacterInputCharacter, otherwise \c NULL.
			*/
			void SetInput(FbxCharacterInputType inputType, FbxObjectManaged^ inputObject);

			//! Get input type.
			property FbxCharacterInputType InputType
			{
				FbxCharacterInputType get();
			}

			/** Get input actor or character.
			* \return     Pointer or \c Null, depending on the input type.
			*                  - If the input type is set to eCharacterInputCharacter. The returned pointer can be casted to a pointer of type KFbxCharacter.
			*                  - \c Null pointer if the input object has not been set, or if the input type is not set to eCharacterInputCharacter.
			*/
			REF_PROPERTY_GET_DECLARE(FbxObjectManaged,InputObject);

			/** Associate a character link to a given character node ID. If a character link already exists for this character node ID,
			* the character link will be removed.
			* \param pCharacterNodeId      Character node ID.
			* \param pCharacterLink        Character link.
			* \param pUpdateObjectList     Set to \c true to update the object list (default value).
			* \return                      \c true if successful, \c false otherwise.
			*/
			bool SetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink, bool updateObjectList);
			bool SetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink);

			/** Get a character link associated with a given character node ID.
			* \param pCharacterNodeId     ID of character node requested.
			* \param pCharacterLink       Optional pointer to receive the character link if function succeeds.
			* \return                     \c true if successful, \c false otherwise.
			*/
			bool GetCharacterLink(FbxCharacterNodeId characterNodeId, FbxCharacterLink^ characterLink);



			/** Get number of elements in a given character group.
			* \param pCharacterGroupId     Character group ID.
			* \return                      The number of elements in the pCharacterGroupId character group.
			*/
			static int GetCharacterGroupCount(FbxCharacterGroupId characterGroupId);

			/** Get character node ID of an element in a given character group.
			* \param pCharacterGroupId     Character group ID.
			* \param pIndex                Character index ID.
			* \return                      Character node ID.
			*/
			static FbxCharacterNodeId GetCharacterGroupElementByIndex(FbxCharacterGroupId characterGroupId, int index);


			/** Get character node name of an element in a given character group.
			* \param pCharacterGroupId     Character group ID.
			* \param pIndex                Character index ID.
			* \return                      Character node name.
			*/
			static System::String^ GetCharacterGroupNameByIndex(FbxCharacterGroupId characterGroupId, int index);

			/** Get character node version of an element in a given character group.
			* \param pCharacterGroupId     Character group ID.
			* \param pIndex                Character index ID.
			* \return                      Character node version.
			*/
			static int GetCharacterGroupVersionByIndex(FbxCharacterGroupId characterGroupId, int index);

			/** Find the character group index associated with a given character node name.
			* \param pName                 Character node name.
			* \param pForceGroupId         Set to \c true to force the character group ID.
			* \param pCharacterGroupId     Receives character group ID.
			* \param pIndex                Receives character index ID.
			* \return                      \c true if successful, otherwise \c false.
			*/
			static bool FindCharacterGroupIndexByName(System::String^ name, bool forceGroupId, [OutAttribute]FbxCharacterGroupId %CharacterGroupId, [OutAttribute]int %index);

			/** Get character node group and index of a given character node ID.
			* \param pCharacterNodeId     Character node ID.
			* \param pCharacterGroupId    if the Character node ID is found, the method returns the group ID through this parameter
			* \param pIndex               if the Character node ID is found, the method returns the index through this parameter
			* \remarks                    Only works for a character node ID that is part of a group.
			* \return                     \c true if successful, \c false otherwise.
			*/
			static bool GetCharacterGroupIndexByElement(FbxCharacterNodeId characterNodeId, [OutAttribute]FbxCharacterGroupId %characterGroupId, [OutAttribute]int %index);

			/** Get character node version of a given character node ID.
			* \param pCharacterNodeId     Character node ID to get version.
			* \param pVersion             if the node ID is found, the method returns the version through this parameter
			* \remarks                    Only works for a character node ID is part of a group.
			* \return                     \c true if successful, \c false otherwise.
			*/
			static bool GetCharacterGroupVersionByElement(FbxCharacterNodeId characterNodeId,[OutAttribute]int %index);

			/** Get character node name associated with a given character node ID.
			* \param pCharacterNodeId     Character node ID to get name.
			* \param pName                if the node ID is found, the method returns the node name through this parameter
			*                             Since the Pointer points to internal data, it is not necessary to allocate a string buffer 
			*                             before calling this function.
			* \return                     \c true if a name exists for the given node ID.
			*/
			static bool GetCharacterNodeNameFromNodeId(FbxCharacterNodeId characterNodeId,[OutAttribute]System::String^ %name);

			/** Get the character node ID associated with a given character node name.
			* \param pName                Character node name to get node ID.
			* \param pCharacterNodeId     if the node name is found, this method returns the node ID through this parameter
			* \return                     \c true if a node ID exists for the given node name.
			*/
			static bool GetCharacterNodeIdFromNodeName(System::String^ name, [OutAttribute]FbxCharacterNodeId %characterNodeId);

			enum  class Error
			{
				InternalError,
				ErrorCount
			} ; 

			// KFbxConstraint Properties
			VALUE_PROPERTY_GETSET_DECLARE(bool,Active);
			VALUE_PROPERTY_GETSET_DECLARE(bool,Lock);
			VALUE_PROPERTY_GETSET_DECLARE(double,Weight);					
			VALUE_PROPERTY_GETSET_DECLARE(double,PullIterationCount);			
			VALUE_PROPERTY_GETSET_DECLARE(bool,ForceActorSpace);
			VALUE_PROPERTY_GETSET_DECLARE(double,ScaleCompensation);			
			VALUE_PROPERTY_GETSET_DECLARE(double,HipsHeightCompensation);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleHeightCompensation);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleProximityCompensation);			
			VALUE_PROPERTY_GETSET_DECLARE(double,MassCenterCompensation);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ApplyLimits);
			VALUE_PROPERTY_GETSET_DECLARE(double,ChestReduction);
			VALUE_PROPERTY_GETSET_DECLARE(double,CollarReduction);
			VALUE_PROPERTY_GETSET_DECLARE(double,NeckReduction);
			VALUE_PROPERTY_GETSET_DECLARE(double,HeadReduction);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorChest);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorHead);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftFingerBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightFingerBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftToesBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightToesBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftFingerBaseRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightFingerBaseRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftToesBaseRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightToesBaseRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftAnkleRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightAnkleRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorHeadRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLeftWristRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorRightWristRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorChestRotation);
			VALUE_PROPERTY_GETSET_DECLARE(double,ReachActorLowerChestRotation);			
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftUpLegRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftUpLegRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftLegRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftLegRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightUpLegRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightUpLegRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightLegRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightLegRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftArmRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftArmRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftForeArmRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftForeArmRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightArmRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightArmRollMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightForeArmRoll);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightForeArmRollMode);			
			VALUE_PROPERTY_GETSET_DECLARE(bool,FootFloorContact);
			VALUE_PROPERTY_GETSET_DECLARE(bool,FootAutomaticToes);			
			VALUE_PROPERTY_GETSET_DECLARE(double,FootBottomToAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootBackToAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootMiddleToAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootFrontToMiddle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootInToAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootOutToAnkle);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootContactSize);
			VALUE_PROPERTY_GETSET_DECLARE(bool,FootFingerContact);			
			VALUE_PROPERTY_GETSET_DECLARE(double,FootContactStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(double,FootFingerContactRollStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(bool,HandFloorContact);
			VALUE_PROPERTY_GETSET_DECLARE(bool,HandAutomaticFingers);			
			VALUE_PROPERTY_GETSET_DECLARE(double,HandBottomToWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandBackToWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandMiddleToWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandFrontToMiddle);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandInToWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandOutToWrist);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandContactSize);
			VALUE_PROPERTY_GETSET_DECLARE(bool,HandFingerContact);			
			VALUE_PROPERTY_GETSET_DECLARE(double,HandContactStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(double,HandFingerContactRollStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandThumbTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandIndexTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandMiddleTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandRingTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandPinkyTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftHandExtraFingerTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandThumbTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandIndexTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandMiddleTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandRingTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandPinkyTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightHandExtraFingerTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootThumbTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootIndexTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootMiddleTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootRingTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootPinkyTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,LeftFootExtraFingerTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootThumbTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootIndexTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootMiddleTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootRingTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootPinkyTip);
			VALUE_PROPERTY_GETSET_DECLARE(double,RightFootExtraFingerTip);
			VALUE_PROPERTY_GETSET_DECLARE(bool,FingerSolving);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftToeBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftFoot);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightToeBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightFoot);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftFingerBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftHand);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullLeftElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightFingerBase);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightHand);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullRightElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlChestPullLeftHand);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlChestPullRightHand);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlPullHead);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistHipsPosition);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlEnforceGravity);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistHipsOrientation);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistChestPosition);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistChestOrientation);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistLeftCollar);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistRightCollar);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistLeftKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistMaximumExtensionLeftKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistCompressionFactorLeftKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistRightKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistMaximumExtensionRightKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistCompressionFactorRightKnee);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistLeftElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistMaximumExtensionLeftElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistCompressionFactorLeftElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistRightElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistMaximumExtensionRightElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlResistCompressionFactorRightElbow);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlSpineStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(double,CtrlNeckStiffness);
			VALUE_PROPERTY_GETSET_DECLARE(bool,MirrorMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,ShoulderCorrection);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftKneeKillPitch);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightKneeKillPitch);
			VALUE_PROPERTY_GETSET_DECLARE(bool,LeftElbowKillPitch);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RightElbowKillPitch);			
			VALUE_PROPERTY_GETSET_DECLARE(bool,WriteReference);
			VALUE_PROPERTY_GETSET_DECLARE(bool,SyncMode);
			VALUE_PROPERTY_GETSET_DECLARE(double,Damping);
			VALUE_PROPERTY_GETSET_DECLARE(double,OrientationDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,DisplacementDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,DisplacementMemory);			
			VALUE_PROPERTY_GETSET_DECLARE(double,HipsDisplacementDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleDisplacementDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,WristDisplacementDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,Stabilization);
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationTime);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationPerimeter);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationAngularPerimeter);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationFloorProximity);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationDamping);			
			VALUE_PROPERTY_GETSET_DECLARE(double,AnkleStabilizationRecoveryTime);			

			/*KFbxTypedProperty<fbxReference*>					SourceObject;
			KFbxTypedProperty<fbxReference*>					DestinationObject;
			KFbxTypedProperty<fbxReference*>					Actor;
			KFbxTypedProperty<fbxReference*>					Character;
			KFbxTypedProperty<fbxReference*>					ControlSet;*/


			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,OrientationDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,DisplacementDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,DisplacementMemoryMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,HipsDisplacementDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,AnkleDisplacementDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,WristDisplacementDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,AnkleStabilizationTimeMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterAutoUser,AnkleStabilizationPerimeterMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleStabilizationAngularPerimeterMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleStabilizationFloorProximityMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleStabilizationDampingMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleStabilizationRecoveryTimeMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterHipsTranslationMode,HipsTranslationMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterPosture,Posture);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,ScaleCompensationMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,HipsHeightCompensationMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleHeightCompensationMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterOffAutoUser,AnkleProximityCompensationMode);			
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterRollExtractionMode,RollExtractionMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterContactBehaviour,ContactBehaviour);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterFloorPivot,FootFloorPivot);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterFootContactType,FootContactType);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterFingerContactMode,FootFingerContactMode);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterFloorPivot,HandFloorPivot);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterHandContactType,HandContactType);
			VALUE_PROPERTY_GETSET_DECLARE(FbxCharacterFingerContactMode,HandFingerContactMode);

			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,HipsTOffset);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ChestTOffset);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS	

			// Clone
			CLONE_DECLARE();

			//protected:
			//	KFbxCharacter& operator=(KFbxCharacter const& pCharacter); 
			//public:
			//	KFbxCharacter(KFbxSdkManager& pManager, char const* pName);
			//	~KFbxCharacter();
			//
			//	virtual void Construct(const KFbxCharacter* pFrom);
			//	virtual void Destruct(bool pRecursive, bool pDependents);
			//
			//	virtual EConstraintType GetConstraintType();
			//	virtual FbxString			GetTypeName() const;
			//
			//	virtual FbxStringList	GetTypeFlags() const;
			//
			//	// KFbxTakeNodeContainer virtual functions
			//	virtual void AddChannels(KFbxTakeNode *pTakeNode);
			//
			//	virtual	bool	ConnecNotify (KFbxConnectEvent const &pEvent);
			//
			//	// Used when bridging to HumanIK Middleware
			//	const void GetKFbxCharacterPropertyFromHIKProperty(	char* &pKFbxCharacterPropertyName, 
			//														char* &pKFbxCharacterPropertyModeName,	
			//														kCharacterPropertyUnit &pUnit,
			//														int &pPropertyIndex,
			//														const char* pHIKPropertyName) const;
			//	
			//	const void GetHIKPropertyFromKFbxCharacterProperty(	char* &pHIKPropertyName, 														
			//														kCharacterPropertyUnit &pUnit,
			//														const char* pKFbxCharacterPropertyName) const ;
			//
			//	const void GetPropertyInfo(	char* &pKFbxCharacterPropertyName, 
			//								char* &pKFbxCharacterPropertyModeName,	
			//								kCharacterPropertyUnit &pUnit,
			//								int &pPropertyIndex,
			//								char* &pHIKPropertyName,
			//								int pIndex) const;
			//	int GetPropertyInfoCount();
			//
			//	KError				mError;
			//
			//	FbxString				mName;	
			//	KFbxCharacterLink	mCharacterLink[eCharacterLastNodeId];
			//	KFbxControlSet*		mControlSet;
			//	
			//	friend class KFbxScene;
			//	friend class KFbxNode;
			//	friend class KFbxReaderFbx;	
			//
			//
#endif

		};		
	}
}