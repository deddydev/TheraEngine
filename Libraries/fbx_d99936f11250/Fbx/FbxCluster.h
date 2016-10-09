#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxSubDeformer.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSubDeformer;
		ref class FbxNode;
		ref class FbxXMatrix;
		ref class FbxStringManaged;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		public ref class FbxCluster : FbxSubDeformer
		{
		internal:
			FbxCluster(KFbxCluster* instance) : FbxSubDeformer(instance)
			{
				_Free = false;
			}
			REF_DECLARE(FbxEmitter,KFbxCluster);
			FBXOBJECT_DECLARE(FbxCluster);
		protected:
			virtual void CollectManagedMemory()override;
		public:		
			/** Restore the link to its initial state.
			* Calling this function will clear the following:
			* - pointer to linked node
			* - pointer to associate model
			* - control point indices and weights
			* - transformation matrices
			*/
			void Reset();

			/** \enum ELinkMode Link modes.
			* The link mode sets how the link influences the position of a control
			* point and the relationship between the weights assigned to a control
			* point. The weights assigned to a control point are distributed among
			* the set of links associated with an instance of class KFbxGeometry.
			*      - \e eNORMALIZE     In mode eNORMALIZE, the sum of the weights assigned to a control point
			*                          is normalized to 1.0. Setting the associate model in this mode is not
			*                          relevant. The influence of the link is a function of the displacement of the
			*                          link node relative to the node containing the control points.
			*      - \e eADDITIVE      In mode eADDITIVE, the sum of the weights assigned to a control point
			*                          is kept as is. It is the only mode where setting the associate model is
			*                          relevant. The influence of the link is a function of the displacement of
			*                          the link node relative to the node containing the control points or,
			*                          if set, the associate model. The weight gives the proportional displacement
			*                          of a control point. For example, if the weight of a link over a control
			*                          point is set to 2.0, a displacement of the link node of 1 unit in the X
			*                          direction relative to the node containing the control points or, if set,
			*                          the associate model, triggers a displacement of the control point of 2
			*                          units in the same direction.
			*      - \e eTOTAL1        Mode eTOTAL1 is identical to mode eNORMALIZE except that the sum of the
			*                          weights assigned to a control point is not normalized and must equal 1.0.
			*/
			enum class LinkMode
			{
				Normalize,
				Additive,
				Total1
			};

			/** Get the link mode.
			* \return     The link mode.
			*/
			/** Set the link mode.
			* \param pMode     The link mode.
			* \remarks         All the links associated to an instance of class KFbxGeometry must have the same link mode.
			*/
			/** Set the link node.
			* \param pNode     The link node.
			* \remarks         The link node is the node which influences the displacement
			*                  of the control points. Typically, the link node is the bone a skin is
			*                  attached to.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(LinkMode,Mode)									

			/** Get the link node.
			* \return      The link node or \c NULL if KFbxCluster::SetLink() has not been called before.
			* \remarks     The link node is the node which influences the displacement
			*              of the control points. Typically, the link node is the bone a skin is
			*              attached to.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxNode,Link);
			//			KFbxNode const* GetLink() const;

			/** Set the associate model.
			* The associate model is optional. It is only relevant if the link mode
			* is of type eADDITIVE.
			* \param pNode     The associate model node.
			* \remarks         If set, the associate model is the node used as a reference to
			*                  measure the relative displacement of the link node. Otherwise, the
			*                  displacement of the link node is measured relative to the node
			*                  containing the control points. Typically, the associate model node is
			*                  the parent of the bone a skin is attached to.
			*/
			

			/** Get the associate model.
			* The associate model is optional. It is only relevant if the link mode is of type
			* eADDITIVE.
			* \return      The associate model node or \c NULL if KFbxCluster::SetAssociateModel() has not been called before.
			* \remarks     If set, the associate model is the node used as a reference to
			*              measure the relative displacement of the link node. Otherwise, the
			*              displacement of the link node is measured relative the the node
			*              containing the control points. Typically, the associate model node is
			*              the parent of the bone a skin is attached to.
			*/

			REF_PROPERTY_GETSET_DECLARE(FbxNode ,AssociateModel)
			

			/**
			* \name Control Points
			* A link has an array of indices to control points and associated weights.
			* The indices refer to the control points in the instance of class KFbxGeometry
			* owning the link. The weights are the influence of the link node over the
			* displacement of the indexed control points.
			*/
			//@{

			/** Add an element in both arrays of control point indices and weights.
			* \param pIndex     The index of the control point.
			* \param pWeight    The link weight.
			*/
			void AddControlPointIndex(int index, double weight);

			/** Get the length of the arrays of control point indices and weights.
			* \return     Length of the arrays of control point indices and weights.
			*             Returns 0 if no control point indices have been added or the arrays have been reset.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ControlPointIndicesCount)
			
			/** Get the array of control point indices.
			* \return     Pointer to the array of control point indices.
			*             \c NULL if no control point indices have been added or the array has been reset.
			*/
			IntPtr GetControlPointIndices();
			array<int>^ GetControlPointIndicesArray();

			/** Get the array of control point weights.
			* \return     Pointer to the array of control point weights.
			*             \c NULL if no control point indices have been added or the array has been reset.
			*/
			IntPtr GetControlPointWeights();
			array<double>^ GetControlPointWeightsArray();

			//@}


			/**
			* \name Transformation Matrices
			* A link has three transformation matrices:
			*      \li Transform refers to the global initial position of the node containing the link
			*      \li TransformLink refers to global initial position of the link node
			*      \li TransformAssociateModel refers to the global initial position of the associate model
			*
			* These matrices are used to set the positions where the
			* influences of the link node and associate model over the
			* control points are null.
			*/
			//@{

			/** Get matrix associated with the node containing the link.
			* \param pMatrix     Transformation matrix.
			* \return            Input parameter filled with appropriate data.
			*/
			/** Set matrix associated with the node containing the link.
			* \param pMatrix     Transformation matrix.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,TransformMatrix);				


			/** Get matrix associated with the link node.
			* \param pMatrix     Transformation matrix.
			* \return            Input parameter filled with appropriate data.
			*/			
			/** Set matrix associated with the link node.
			* \param pMatrix     Transformation matrix.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,TransformLinkMatrix);			
				

			/** Get matrix associated with the associate model.
			* \param pMatrix     Transformation matrix.
			* \return            Input parameter filled with appropriate data.
			*/
			/** Set matrix associated with the associate model.
			* \param pMatrix     Transformation matrix.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,TransformAssociateModelMatrix);			
							

			/** Get matrix associated with the parent node.
			* \param pMatrix     Transformation matrix.
			* \return            Input parameter filled with appropriate data.
			*/			
			/** Set matrix associated with the parent node.
			* \param pMatrix     Transformation matrix.
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,TransformParentMatrix);			

			/** Get the Transform Parent set flag value.
			* \return           \c true if transform matrix associated with parent node is set.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsTransformParentSet);
			

//			//@}
//
//			//!Assigment operator
//			//KFbxCluster& operator=(KFbxCluster const& pCluster);
//
//			///////////////////////////////////////////////////////////////////////////////
//			//
//			//  WARNING!
//			//
//			//  Anything beyond these lines may not be documented accurately and is
//			//  subject to change without notice.
//			//
//			///////////////////////////////////////////////////////////////////////////////
//
#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();

		public:
			/** Set user data.
			* \param pUserDataID Identifier of user data.
			* \param pUserData User data.
			*/
			void SetUserData(FbxStringManaged^ userDataID, FbxStringManaged^ userData);

			//! Get the user data identifier.
			VALUE_PROPERTY_GET_DECLARE(FbxStringManaged^,UserDataID);
			

			//! Get the user data.			
			VALUE_PROPERTY_GET_DECLARE(FbxStringManaged^,UserData);
			

			//! Get the user data by identifier.
			FbxStringManaged^ GetUserData (FbxStringManaged^ userDataID);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}