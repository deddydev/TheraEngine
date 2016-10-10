#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxCharacter.h"
#include "FbxTakeNodeContainer.h"


{
	namespace FbxSDK
	{		
		ref class FbxTakeNodeContainer;
		ref class FbxStringManaged;
		ref class FbxNode;
		ref class FbxCharacter;		

		public enum class FbxEffectorNodeId
		{
			EffectorHips = 0,
			EffectorLeftAnkle,
			EffectorRightAnkle,
			EffectorLeftWrist,
			EffectorRightWrist,
			EffectorLeftKnee,
			EffectorRightKnee,
			EffectorLeftElbow,
			EffectorRightElbow,
			EffectorChestOrigin,
			EffectorChestEnd,
			EffectorLeftFoot,
			EffectorRightFoot,
			EffectorLeftShoulder,
			EffectorRightShoulder,
			EffectorHead,
			EffectorLeftHip,
			EffectorRightHip,

			// Added for 4.5

			EffectorLeftHand,
			EffectorRightHand,

			EffectorLeftHandThumb,
			EffectorLeftHandIndex,
			EffectorLeftHandMiddle,
			EffectorLeftHandRing,
			EffectorLeftHandPinky,
			EffectorLeftHandExtraFinger,
			EffectorRightHandThumb,
			EffectorRightHandIndex,
			EffectorRightHandMiddle,
			EffectorRightHandRing,
			EffectorRightHandPinky,
			EffectorRightHandExtraFinger,

			EffectorLeftFootThumb,
			EffectorLeftFootIndex,
			EffectorLeftFootMiddle,
			EffectorLeftFootRing,
			EffectorLeftFootPinky,
			EffectorLeftFootExtraFinger,

			EffectorRightFootThumb,
			EffectorRightFootIndex,
			EffectorRightFootMiddle,
			EffectorRightFootRing,
			EffectorRightFootPinky,
			EffectorRightFootExtraFinger,

			EffectorLastNodeId
		};

		public enum class FbxEffectorSetId
		{
			EffectorSetDefault = 0,
			EffectorSetAux1,
			EffectorSetAux2,
			EffectorSetAux3,
			EffectorSetAux4,
			EffectorSetAux5,
			EffectorSetAux6,
			EffectorSetAux7,
			EffectorSetAux8,
			EffectorSetAux9,
			EffectorSetAux10,
			EffectorSetAux11,
			EffectorSetAux12,
			EffectorSetAux13,
			EffectorSetAux14,
			LastEffectorSetId
		};


		/** \class KFbxControlSetLink
		*
		* \brief This class represents a link between a given character's FK node and the associated node in the hierarchy.
		*
		*/
		public ref class FbxControlSetLink : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxControlSetLink,KFbxControlSetLink);
			INATIVEPOINTER_DECLARE(FbxControlSetLink,KFbxControlSetLink);
		public:

			//! Default constructor.
			DEFAULT_CONSTRUCTOR(FbxControlSetLink,KFbxControlSetLink);			

			/** Copy constructor.
			* \param pControlSetLink Given object.
			*/
			FbxControlSetLink(FbxControlSetLink^ controlSetLink);
			//! Assignment operator.
			//KFbxControlSetLink& operator=(const KFbxControlSetLink& pControlSetLink);
			void CopyFrom(FbxControlSetLink^ controlSetLink);

			/** Reset to default values.
			*
			* Member mNode is set to \c NULL and member mTemplateName is cleared.
			*/
			void Reset();		
		public:

			//! The character's node in a hierarchy linked to this control set link.
			REF_PROPERTY_GETSET_DECLARE(FbxNode,Node);

			//! A template name is a naming convention that is used to automatically map
			//! the nodes of other skeletons that use the same naming convention for automatic characterization.
			REF_PROPERTY_GET_DECLARE(FbxStringManaged,TemplateName);
		};

		/** \class KFbxEffector
		*
		* \brief This class represents a link between a given character's effector and the associated node in the hierarchy.
		*
		*/
		public ref class FbxEffector : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxEffector,KFbxEffector);
			INATIVEPOINTER_DECLARE(FbxEffector,KFbxEffector);
		public:

			//! Default constructor.
			DEFAULT_CONSTRUCTOR(FbxEffector,KFbxEffector);

			//! Assignment operator.
			//KFbxEffector& operator=(KFbxEffector& pEffector);
			void CopyFrom(FbxEffector^ e);

			/** Reset to default values.
			*     - mNode is set to NULL.
			*     - mTemplateName is cleared.
			*     - mShow is set to true.
			*/
			void Reset();		
		public:
			//! The character's node in a hierarchy linked to this cotnrol set link.
			REF_PROPERTY_GETSET_DECLARE(FbxNode,Node);

			//! \c true if the effector is visible, \c false if hidden
			property bool Show
			{
				bool get();
				void set(bool value);
			}

			// These members are for backward compatibility and should not be used. These properties are now published through class KFbxControlSetPlug.
#ifndef DOXYGEN_SHOULD_SKIP_THIS			
			property bool TActive
			{
				bool get();
				void set(bool value);
			}			
			property bool RActive
			{
				bool get();
				void set(bool value);
			}			
			property bool CandidateTActive
			{
				bool get();
				void set(bool value);
			}			
			property bool CandidateRActive
			{
				bool get();
				void set(bool value);
			}
#endif
		};

		ref class FbxControlSetPlug;

		/** \class KFbxControlSet
		*
		* \brief This class contains all methods to either set-up an exported control rig or query information on an imported control rig.
		*
		* This class also contains some methods to manipulate the KFbxEffector and KFbxControlSetLink.
		*
		* The KFbxControlSet class contains FK rig (Forward Kinematics) and IK rig (Inverse Kinematics) animation. The FK rig is represented
		* by a list of nodes while the IK rig is represented by a list of effectors.
		*
		* You can access the FK rig with the KFbxControlSetLink class, using the functions KFbxControlSet::SetControlSetLink() and KFbxControlSet::GetControlSetLink().
		*
		* You can access the IK rig with the KFbxEffector class, using the functions KFbxControlSet::SetEffector() and KFbxControlSet::GetEffector().
		*
		*/
		public ref class FbxControlSet : IFbxNativePointer
		{
			INTERNAL_CLASS_DECLARE(FbxControlSet,KFbxControlSet);
			REF_DECLARE(FbxControlSet,KFbxControlSet);
			DESTRUCTOR_DECLARE_2(FbxControlSet);
			INATIVEPOINTER_DECLARE(FbxControlSet,KFbxControlSet);		

		public:	
			/** Reset to default values.
			* Reset all effector and control set links.
			*/
			void Reset();

			/** \enum EType      Control rig type.
			* - \e eNone       No Control rig.
			* - \e eFkIk       Both an FK rig and IK rig.
			* - \e eIkOnly     Only an IK rig.
			*/
			enum class ControlSetType
			{
				None = KFbxControlSet::eNone,
				FkIk = KFbxControlSet::eFkIk,
				IkOnl = KFbxControlSet::eIkOnly
			};															

			/** Get type.
			* \return The gotten type.
			*/
			/** Set type as given.
			* \param pType The given type.
			*/
			property ControlSetType Type
			{
				ControlSetType get();
				void set(ControlSetType value);
			}

			/** Get use axis flag.
			* \return The gotten use axis flag.
			*/
			/** Set use axis flag as given.
			* \param pUseAxis The given use axis flag.
			*/							
			property bool UseAxis
			{
				bool get();
				void set(bool value);
			}

			/** Get lock transform flag.
			* \return The gotten lock transform flag.
			*/							
			/** Set lock transform flag as given.
			* \param pLockTransform The given lock transform flag.
			*/										
			property bool LockTransform
			{
				bool get();
				void set(bool value);
			}

			/** Get lock 3D pick flag.
			* \return The gotten lock 3D pick flag.
			*/
			/** Set lock 3D pick flag as given.
			* \param pLock3DPick The given lock 3D pick flag.
			*/																							
			property bool Lock3DPick
			{
				bool get();
				void set(bool value);
			}

			/** Get control set associated with the character.
			* \return     Return the control set associated with the character.
			*/
			static FbxControlSet^ GetControlSet(FbxCharacter^ ch);

			/** Set a control set link for a character node ID.
			* \param pCharacterNodeId     Character node ID.
			* \param pControlSetLink      Control set link to be associated with the Character node ID.
			* \return                     \c true if successful, \c false otherwise.
			* \remarks                    You should avoid setting a control set link for
			*                             eCharacterLeftFloor, eCharacterRightFloor, eCharacterLeftHandFloor, eCharacterRightHandFloor,
			*                             eCharacterProps0, eCharacterProps1, eCharacterProps2, eCharacterProps3 or eCharacterProps4.
			*                             None of these nodes are part of a control set.
			*/
			bool SetControlSetLink(FbxCharacterNodeId characterNodeId, FbxControlSetLink^ controlSetLink);

			/** Get the control set link associated with a character node ID.
			* \param pCharacterNodeId     Requested character node ID.
			* \param pControlSetLink      Optional pointer that returns the control set link if the function succeeds.
			* \return                     \c true if successful, \c false otherwise.
			* \remarks                    You should avoid getting a control set link for
			*                             eCharacterLeftFloor, eCharacterRightFloor, eCharacterLeftHandFloor, eCharacterRightHandFloor,
			*                             eCharacterProps0, eCharacterProps1, eCharacterProps2, eCharacterProps3 or eCharacterProps4.
			*                             None of these nodes are part of a control set.
			*/							
			bool GetControlSetLink(FbxCharacterNodeId characterNodeId, FbxControlSetLink^ controlSetLink);

			/** Set an effector node for an effector node ID.
			* \param pEffectorNodeId     Effector node ID.
			* \param pEffector           Effector to be associated with the effector node ID.
			* \return                    \c true if successful, \c false otherwise.
			*/
			bool SetEffector(FbxEffectorNodeId effectorNodeId, FbxEffector^ effector);

			/** Get the effector associated with an effector node ID.
			* \param pEffectorNodeId     ID of requested effector node.
			* \param pEffector           Optional pointer that returns the effector if the function succeeds.
			* \return                    \c true if successful, \c false otherwise.
			*/
			bool GetEffector(FbxEffectorNodeId effectorNodeId, FbxEffector^ effector);

			/** Set an auxiliary effector node for an effector node ID.
			* \param pEffectorNodeId     Effector node ID.
			* \param pNode               Auxiliary effector node to be associated with the effector node ID.
			* \param pEffectorSetId      Effector set ID. Set to eEffectorSetAux1 by default.
			* \return                    \c true if successful, \c false otherwise.
			*/
			bool SetEffectorAux(FbxEffectorNodeId effectorNodeId, FbxNode^ node,FbxEffectorSetId effectorSetId);

			/** Get the auxiliary effector associated with an effector node ID.
			* \param pEffectorNodeId     ID of requested auxilliary effector node.
			* \param pNode               Optional pointer that returns the auxiliary effector node if the function succeeds.
			* \param pEffectorSetId      Effector set ID. Set to eEffectorSetAux1 by default.
			* \return                    \c true if successful, \c false otherwise.
			*/
			/*bool GetEffectorAux(FbxEffectorNodeId effectorNodeId, FbxNode^ %node,FbxEffectorSetId effectorSetId)
			{
			return control->GetEffectorAux((EEffectorNodeId)effectorNodeId,(KFbxNode*)node->emitter,(EEffectorSetId)effectorSetId);
			}*/

			/** Get the name associated with an effector node ID.
			* \param pEffectorNodeId     Effector node ID.
			* \return                    Name associated with the effector node ID.
			*/
			static System::String^ GetEffectorNodeName(FbxEffectorNodeId effectorNodeId);

			/** Get ID associated with an effector node name.
			* \param pEffectorNodeName     Effector node name.
			* \return                      Effector node ID associated with the given effector node name, or -1 if
			*                              no effector node with pEffectorNodeName exists.
			*/
			static FbxEffectorNodeId GetEffectorNodeId(System::String^ effectorNodeName);
			//
			//
			//				///////////////////////////////////////////////////////////////////////////////
			//				//
			//				//  WARNING!
			//				//
			//				//  Anything beyond these lines may not be documented accurately and is
			//				//  subject to change without notice.
			//				//
			//				///////////////////////////////////////////////////////////////////////////////
			//
#ifndef DOXYGEN_SHOULD_SKIP_THIS			
		public:			
			void FromPlug(FbxControlSetPlug^ plug);
			void ToPlug(FbxControlSetPlug^ plug);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		/** Plug class for control set.
		* \nosubgrouping
		*/
		public ref class FbxControlSetPlug : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxControlSetPlug);
		internal:
			FbxControlSetPlug(KFbxControlSetPlug* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}

			FBXOBJECT_DECLARE(FbxControlSetPlug);
		protected:
			virtual void CollectManagedMemory()override;

		public:

			//! Type property of control set.
			VALUE_PROPERTY_GETSET_DECLARE(FbxControlSet::ControlSetType,ControlSet_Type);
			//! Use axis flag.
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseAxis);
			//! Reference character.
			//KFbxTypedProperty<fbxReference*>            Character;

			/**
			* \name Error Management
			* The same error object is shared among instances of this class.
			*/
			//@{

			/** Retrieve error object.
			* \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			//! Error identifiers.
			enum class Error
			{
				Error,
				ErrorCount
			};

			/** Get last error code.
			* \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			* \return     Textual description of the last error.
			*/				
			property System::String^ LastErrorString
			{
				System::String^ get();
			}

			//	//@}

			//	//! Clone
			CLONE_DECLARE();
		};

	}
}