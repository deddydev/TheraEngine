#pragma once
#include "stdafx.h"
#include "FbxObject.h"

namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxNode;
		ref class FbxMatrix;
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxName;
		ref class FbxErrorManaged;
		/** This structure contains the description of a named pose.
		*
		*/
		public ref struct FbxPoseInfo : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxPoseInfo,KFbxPoseInfo);
			INATIVEPOINTER_DECLARE(FbxPoseInfo,KFbxPoseInfo);
		public:
			REF_PROPERTY_GETSET_DECLARE(FbxMatrix,Matrix); //! Transform matrix
			VALUE_PROPERTY_GETSET_DECLARE(bool ,MatrixIsLocal); //! If true, the transform matrix above is defined in local coordinates.
			REF_PROPERTY_GETSET_DECLARE(FbxNode,Node);          //! Affected node (to replace the identifier).

		};

		/** This class contains the description of a Pose manager.
		* \nosubgrouping
		* The KFbxPose object can be setup to hold "Bind Pose" data or "Rest Pose" data.
		*
		* The Bind Pose holds the transformation (translation, rotation and scaling)
		* matrix of all the nodes implied in a link deformation. This includes the geometry
		* being deformed, the links deforming the geometry, and recursively all the
		* ancestors nodes of the link. The Bind Pose gives you the transformation of the nodes
		* at the moment of the binding operation when no deformation occurs.
		*
		* The Rest Pose is a snapshot of a node transformation. A Rest Pose can be used
		* to store the position of every node of a character at a certain point in
		* time. This pose can then be used as a reference position for animation tasks,
		* like editing walk cycles.
		*
		* One difference between the two modes is in the validation performed before
		* adding an item and the kind of matrix stored.
		*
		* In "Bind Pose" mode, the matrix is assumed to be defined in the global space,
		* while in "Rest Pose" the type of the matrix may be specified by the caller. So
		* local system matrices can be used. Actually, because there is one such flag for
		* each entry (KFbxPoseInfo), it is possible to have mixed types in a KFbxPose elements.
		* It is therefore the responsability of the caller to check for the type of the retrieved
		* matrix and to do the appropriate conversions if required.
		*
		* The validation of the data to be added consists of the following steps:
		*
		*     \li If this KFbxPose object stores "Bind Poses", then
		*        add a KFbxPoseInfo only if the node is not already
		*        associated to another "Bind Pose". This check is done
		*        by visiting ALL the KFbxPose objects in the system.
		*
		*        The above test is only performed for the "Bind Pose" type. While
		*        the next one is always performed, no matter what kind of poses this
		*        KFbxPose object is setup to hold.
		*
		*     \li If a node is already inserted in the KFbxPose internal list,
		*        then the passed matrix MUST be equal to the one already stored.
		*        If this is not the case, the Add method will return -1, indicating
		*        that no new KFbxPoseInfo has been created.
		*
		* If the Add method succeeds, it will return the index of the KFbxPoseInfo
		* structure that as been created and held by the KFbxPose object.
		*
		* To ensure data integrity, the stored information can only be
		* accessed using the provided methods (read-only). If an entry needs to be
		* modified, the caller has to remove the KFbxPoseInfo item by calling Remove(i)
		* and then Add a new one.
		*
		* The internal list is not ordered and the search inside this is list is linear
		* (from the first element to ... the first match or the end of the list).
		*
		*/
		public ref class FbxPose : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxPose);
		internal:
			FbxPose(KFbxPose* instance);
			FBXOBJECT_DECLARE(FbxPose);
		protected:
			virtual void CollectManagedMemory() override;
			System::Collections::Generic::List<FbxNode^>^ _List;
		public:			

			/** Pose identifier flag.
			* \return \c true if this object holds BindPose data.
			*/
			/** Set the type of pose.
			* \param pIsBindPose If true, type will be bind pose, else rest pose.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,IsBindPose);				

			/** Pose identifier flag.
			* \return \c true if this object holds RestPose data.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsRestPose);

			/** Get number of stored items.
			* \return The number of items stored.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,Count);

			/** Stores the pose transformation for the given node.
			* \param pNode pointer to the node for which the pose is stored.
			* \param pMatrix Pose transform of the node.
			* \param pLocalMatrix Flag to indicate if pMatrix is defined in Local or Global space.
			* \param pMultipleBindPose
			* \return -1 if the function failed or the index of the stored item.
			*/
			int Add(FbxNode^ node,FbxMatrix^ matrix, bool localMatrix, bool multipleBindPose);
			int Add(FbxNode^ node,FbxMatrix^ matrix);

			/** Remove the pIndexth item from the Pose object.
			* \param pIndex Index of the item to be removed.
			*/
			void Remove(int index);

			/** Get the node name.
			* \param pIndex Index of the queried item.
			* \return The node intial and current names.
			* \remarks If the index is invalid ann empty KName is returned.
			*/
			FbxName^ GetNodeName(int index);

			/** Get the node.
			* \param pIndex Index of the queried item.
			* \return A pointer to the node referenced.
			* \remarks If the index is invalid or no pointer to a node is set, returns NULL.
			*  The returned pointer will become undefined if the KFbxPose object is destroyed.
			*/
			FbxNode^ GetNode(int index);

			/** Get the transform matrix.
			* \param pIndex Index of the queried item.
			* \return A reference to the pose matrix.
			* \remarks If the index is invalid a reference to an identiy matrix is returned.
			*  The reference will become undefined if the KFbxPose object is destroyed.
			*/
			FbxMatrix^ GetMatrix(int index);

			/** Get the type of the matrix.
			* \param pIndex Index of the queried item.
			* \return \c true if the matrix is defined in the Local coordinate space and false otherwise.
			* \remarks If the KFbxPose object is configured to hold BindPose data, this method will always return \c false.
			*/
			bool IsLocalMatrix(int index);


			/**
			* \name Search Section
			*/
			//@{
			enum class NameComponent : char
			{
				Initialname = 1,
				Currentname = 2,
				All_Name    = 3
			};

			/** Look in the KFbxPose object for the given node name.
			* \param pNodeName Name of the node we are looking for.
			* \param pCompareWhat Bitwise or of the following flags: INTIALNAME_COMPONENT, CURRENTNAME_COMPONENT
			* \return -1 if the node is not in the list. Otherwise, the index of the
			* corresponding KFbxPoseInfo element.
			*/
			int Find(FbxName^ nodeName, char compareWhat);
			int Find(FbxName^ nodeName);

			/** Look in the KFbxPose object for the given node.
			* \param pNode the node we are looking for.
			* \return -1 if the node is not in the list. Otherwise, the index of the
			* corresponding KFbxPoseInfo element.
			*/
			int Find(FbxNode^ node);


			//@}
			/**
			* \name Utility Section
			*/
			//@{

			/** Get the list of Poses objects that contain the node with name pNodeName.
			* This method will look in all the poses of all the scenes.
			* \param pManager    The manager owning the poses and scenes.
			* \param pNode       The node being explored.
			* \param pPoseList   List of BindPoses/RestPoses that have the node.
			* \param pIndex      List of indices of the nodes in the corresponding poses lists.
			* \return \c true if the node belongs to at least one Pose (either a BindPose or a RestPose).
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetPosesContaining(FbxSdkManager^ manager, FbxNode& node,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Get the list of Poses objects that contain the node with name pNodeName.
			* \param pScene     Scene owning the poses.
			* \param pNode      The node being explored.
			* \param pPoseList  List of BindPoses/RestPoses that have the node.
			* \param pIndex     List of indices of the nodes in the corresponding poses lists.
			* \return \c true if the node belongs to at least one Pose (either a BindPose or a RestPose).
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetPosesContaining(KFbxScene* pScene, KFbxNode* pNode,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Get the list of BindPose objects that contain the node with name pNodeName.
			* This method will look in all the bind poses of all the scenes.
			* \param pManager     The manager owning the poses.
			* \param pNode        The node being explored.
			* \param pPoseList    List of BindPoses that have the node.
			* \param pIndex       List of indices of the nodes in the corresponding bind poses lists.
			* \return \c true if the node belongs to at least one BindPose.
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetBindPoseContaining(KFbxSdkManager& pManager, KFbxNode* pNode,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Get the list of BindPose objects that contain the node with name pNodeName.
			* \param pScene       The scene owning the poses.
			* \param pNode        The node being explored.
			* \param pPoseList    List of BindPoses that have the node.
			* \param pIndex       List of indices of the nodes in the corresponding bind poses lists.
			* \return \c true if the node belongs to at least one BindPose.
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetBindPoseContaining(KFbxScene* pScene, KFbxNode* pNode,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Get the list of RestPose objects that contain the node with name pNodeName.
			* This method will look in all the bind poses of all the scenes.
			* \param pManager     The manager owning the poses.
			* \param pNode        The node being explored.
			* \param pPoseList    List of RestPoses that have the node.
			* \param pIndex       List of indices of the nodes in the corresponding rest poses lists.
			* \return \c true if the node belongs to at least one RestPose.
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetRestPoseContaining(KFbxSdkManager& pManager, KFbxNode* pNode,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Get the list of RestPose objects that contain the node with name pNodeName.
			* \param pScene       The scene owning the poses.
			* \param pNode        The node being explored.
			* \param pPoseList    List of RestPoses that have the node.
			* \param pIndex       List of indices of the nodes in the corresponding rest poses lists.
			* \return \c true if the node belongs to at least one RestPose.
			* \remarks The pPoseList and pIndex are filled by this method.
			*  The elements of the returned list must not be deleted since they still belong to the scene.
			*/
			/*static bool GetRestPoseContaining(KFbxScene* pScene, KFbxNode* pNode,
			KArrayTemplate<KFbxPose*>& pPoseList,
			KArrayTemplate<int>& pIndex);*/

			/** Check this bindpose and report an error if all the conditions to a valid bind pose are not
			* met. The conditions are:
			*
			* a) We are a BindPose.
			* b) For every node in the bind pose, all their parent node are part of the bind pose.
			* c) All the deforming nodes are part of the bind pose.
			* d) All the parents of the deforming nodes are part of the bind pose.
			* e) Each deformer relative matrix correspond to the deformer Inv(bindMatrix) * deformed Geometry bindMatrix.
			*
			* \param pRoot This node is used as the stop point when visiting the parents (cannot be NULL).
			* \param pMatrixCmpTolerance Tolerance value when comparing the matrices.
			* \return true if all the above conditions are met and false otherwise.
			* \remarks If the returned value is false, querying for the error will return the reason of the failure.
			*  As soon as one of the above conditions is not met, this method return ignoring any subsequent errors.
			* Run the IsBindPoseVerbose if more details are needed.
			*/
			bool IsValidBindPose(FbxNode^ root, double matrixCmpTolerance);
			bool IsValidBindPose(FbxNode^ root)
			{
				return IsValidBindPose(root,0.0001);
			}

			/** Same as IsValidBindPose but slower because it will not stop as soon as a failure occurs. Instead,
			* keeps running to accumulate the faulty nodes (stored in the appropriate array). It is then up to the
			* caller to fill the UserNotification if desired.
			*
			* \param pRoot This node is used as the stop point when visiting the parents (cannot be NULL).
			* \param pMissingAncestors Each ancestor missing from the BindPose is added to this list.
			* \param pMissingDeformers Each deformer missing from the BindPose is added to this list.
			* \param pMissingDeformersAncestors Each deformer ancestors missing from the BindPose is added to this list.
			* \param pWrongMatrices Nodes that yeld to a wrong matric comparisons are added to this list.
			* \param pMatrixCmpTolerance Tolerance value when comparing the matrices.
			*/
			/*bool IsValidBindPoseVerbose(KFbxNode* pRoot,
			KArrayTemplate<KFbxNode*>& pMissingAncestors,
			KArrayTemplate<KFbxNode*>& pMissingDeformers,
			KArrayTemplate<KFbxNode*>& pMissingDeformersAncestors,
			KArrayTemplate<KFbxNode*>& pWrongMatrices,
			double pMatrixCmpTolerance=0.0001);*/

			/** Same as IsValidBindPose but slower because it will not stop as soon as a failure occurs. Instead,
			* keeps running to accumulate the faulty nodes and send them directly to the UserNotification.
			*
			* \param pRoot This node is used as the stop point when visiting the parents (cannot be NULL).
			* \param pUserNotification Pointer to the user notification where the messages will be accumulated.
			* \param pMatrixCmpTolerance Tolerance value when comparing the matrices.
			* \remarks If the pUserNotification parameter is NULL, this method will call IsValidBindPose.
			*/
			/*bool IsValidBindPoseVerbose(FbxNode^ root,
			FbxUserNotification* pUserNotification,
			double pMatrixCmpTolerance=0.0001);*/

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				eERROR = KFbxPose::eERROR,
				validbindpose_Failure_InvalidObject = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_INVALIDOBJECT,
				validbindpose_Failure_InvalidRoot = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_INVALIDROOT,
				validbindpose_Failure_NotAllAncestors_Nodes = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_NOTALLANCESTORS_NODES,
				validbindpose_Failure_NotAllDeforming_Nodes = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_NOTALLDEFORMING_NODES,
				validbindpose_Failure_NotAllAncestors_Defnodes = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_NOTALLANCESTORS_DEFNODES,
				Validbindpose_Failure_RelativeMatrix = KFbxPose::eERROR_VALIDBINDPOSE_FAILURE_RELATIVEMATRIX,
				Count = KFbxPose::eERROR_COUNT
			};

			/** Get last error code.
			* \return Last error code.
			*/
			VALUE_PROPERTY_GET_DECLARE(Error,LastErrorID);

			/** Get last error string.
			* \return Textual description of the last error.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,LastErrorString);

			//@}



			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}