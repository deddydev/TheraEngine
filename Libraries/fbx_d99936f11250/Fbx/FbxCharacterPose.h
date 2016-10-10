#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"
#include "FbxCharacterEnums.h"



{
	namespace FbxSDK
	{
		ref class FbxObjectManaged;
		ref class FbxCharacter;
		ref class FbxXMatrix;
		ref class FbxNode;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;		

		/** \class KFbxCharacterPose
		* \nosubgrouping
		* \brief A character pose is a character and an associated hierarchy of nodes.
		*
		* Only the default position of the nodes is considered, the animation data is ignored.
		*
		* You can access the content of a character pose, using the functions KFbxCharacterPose::GetOffset(),
		* KFbxCharacterPose::GetLocalPosition(), and KFbxCharacterPose::GetGlobalPosition().
		* Their source code is provided inline as examples on how to access the character pose data.
		*
		* To create a character pose, You must first create a hierarchy of nodes under the root
		* node provided by function KFbxCharacterPose::GetRootNode(). Then, feed this hierarchy
		* of nodes into the character returned by function KFbxCharacterPose::GetCharacter().
		* Offsets are set in the character links. Local positions are set using
		* KFbxNode::SetDefaultT(), KFbxNode::SetDefaultR(), and KFbxNode::SetDefaultS().
		*
		* To set local positions from global positions:
		*     -# Declare lCharacterPose as a valid pointer to a KFbxCharacterPose;
		*     -# Call lCharacterPose->GetRootNode()->SetLocalStateId(0, true);
		*     -# Call lCharacterPose->GetRootNode()->SetGlobalStateId(1, true);
		*     -# Call KFbxNode::SetGlobalState() for all nodes found in the hierarchy under lCharacterPose->GetRootNode();
		*     -# Call lCharacterPose->GetRootNode()->ComputeLocalState(1, true);
		*     -# Call lCharacterPose->GetRootNode()->SetCurrentTakeFromLocalState(KTIME_ZERO, true).
		*/

		public ref class FbxCharacterPose : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxCharacterPose);
		internal:
			FbxCharacterPose(KFbxCharacterPose* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}

		protected:
			virtual void CollectManagedMemory() override;
			FBXOBJECT_DECLARE(FbxCharacterPose);
		public:
			
			//! Reset to an empty character pose.
			void Reset();

			/** Get the root node.
			* \return     Pointer to the root node.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,RootNode);			

			/** Get the character.
			* \return     Pointer to the character.
			*/
			REF_PROPERTY_GET_DECLARE(FbxCharacter,Character);

			/** Get offset matrix for a given character node.
			* \param pCharacterNodeId     Character Node ID.
			* \param pOffset              Receives offset matrix.
			* \return                     \c true if successful, \c false otherwise.
			*/
			//bool GetOffset(FbxCharacterNodeId characterNodeId, FbxXMatrix^ offset);

			/** Get local position for a given character node.
			* \param pCharacterNodeId     Character Node ID.
			* \param pLocalT              Receives local translation vector.
			* \param pLocalR              Receives local rotation vector.
			* \param pLocalS              Receives local scaling vector.
			* \return                     \c true if successful, \c false otherwise.
			*/
			//bool GetLocalPosition(FbxCharacterNodeId characterNodeId, FbxVector4^ localT, FbxVector4^ localR, FbxVector4^ localS);

			/** Get global position for a given character node.
			* \param pCharacterNodeId     Character Node ID.
			* \param pGlobalPosition      Receives global position.
			* \return                     \c true if successful, \c false otherwise.
			*/
			//bool GetGlobalPosition(FbxCharacterNodeId characterNodeId, FbxXMatrix^ globalPosition);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:
			// Clone
			CLONE_DECLARE();
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}