#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"
#include "FbxTransformation.h"



{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxDouble3;
		ref class FbxDouble3TypedProperty;
		ref class FbxClassId;
		ref class FbxVector4;
		ref class FbxNodeAttribute;
		ref class FbxNull;
		ref class FbxMarker;
		ref class FbxSkeleton;
		ref class FbxGeometry;
		ref class FbxCharacter;
		ref class FbxMesh;
		ref class FbxNurb;
		ref class FbxNurbsSurface;
		ref class FbxNurbsCurve;
		ref class FbxTrimNurbsSurface;
		ref class FbxPatch;
		ref class FbxCamera;
		ref class FbxCameraSwitcher;
		ref class FbxLight;
		ref class FbxOpticalReference;		
		ref class FbxXMatrix;
		ref class FbxTime;
		ref class FbxSurfaceMaterial;
		ref class FbxErrorManaged;
		ref class FbxNodeLimits;


		/// <summary>
		/// This class provides the structure to build a node hierarchy.
		/// \nosubgrouping
		/// It is a composite class that contains node tree management services in itself.
		/// Cyclic graphs are forbidden in a node hierarchy.
		///
		/// The content of a node is in its node attribute, which is an instance of a
		/// class derived from KFbxNodeAttribute. A node attribute can be shared among nodes.
		/// By default, the node attribute pointer is \c NULL meaning it is a simple reference point.
		///
		/// A node also contains an array of take nodes to hold animation data. See
		/// FbxTakeNodeContainer for more details.
		/// </summary>
		public ref class FbxNode : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxNode);
		internal:
			FbxNode(KFbxNode* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxNode);

		protected:
			virtual void CollectManagedMemory() override;
		public:			
			/**
			* \name Node Tree Management
			* This class holds the node tree structure in itself.
			*/
			//@{



			/// <summary>
			/// Get the parent node.			
			/// </summary>
			/// <returns>return Pointer to parent node or \c NULL if the current node has no parent.</returns>
			REF_PROPERTY_GET_DECLARE(FbxNode,Parent);


			/// <summary>
			/// Add a child node and its underlying node tree.		
			/// In the last case, KFbxNode::GetLastErrorID() can return one of the following:
			///     - eCYCLIC_GRAPH: The child node is already in the current node tree, the operation fails to avoid a cyclic graph.
			///     - eNODE_NAME_CLASH: The child node has a name already owned by another node in the destination scene.
			///     - eTEXTURE_NAME_CLASH: A texture in the child node has a name already owned by another texture in the destination scene.
			///     - eVIDEO_NAME_CLASH: A video in the child node has a name already owned by another video in the destination scene.
			///     - eMATERIAL_NAME_CLASH: A material in the child node has a name already owned by another material in the destination scene.
			///
			/// </summary>
			/// <param name="node">Child node</param>
			/// <returns>return \c true on success, \c false otherwise.</returns>
			/// <remarks>
			/// remarks If the added node already has a parent, it is first removed from it.
			/// </remarks>
			bool AddChild(FbxNode^ node);

			/// <summary>
			/// Remove a child node.	
			///In the last case, KFbxNode::GetLastErrorID() returns eNOT_A_CHILD.
			/// </summary>
			/// <param name="Node"> The child node to remove.</param>
			/// <returns>return \c true on success, \c false otherwise</returns>
			bool RemoveChild(FbxNode^ node);



			/// <summary>
			///Get the number of children nodes.			
			/// </summary>
			/// <param name="recursive"> If  true the method will also count all the descendant children.</param>
			/// <returns>return Total number of children for this node.</returns>
			int GetChildCount(bool recursive);

			/// <summary>
			///Get the number of children nodes.			
			/// </summary>
			/// <returns>return Total number of children for this node.</returns>
			int GetChildCount()
			{
				return GetChildCount(false);
			}


			/// <summary>
			///Get child by index.	
			/// In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			/// </summary>
			/// <returns>return Child node or null if index is out of range.</returns>
			FbxNode^ GetChild(int index);

			/** Get child by index.
			* \return Child node or \c NULL if index is out of range.
			* In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			//KFbxNode const* GetChild(int pIndex) const;


			/// <summary>
			///Finds a child node by name.	
			/// </summary>
			/// <param name="name"> Name of the searched child node.</param>
			/// <param name="recursive"> Flag to request recursive calls.(default is false)</param>
			/// <param name="initial"> Flag to a search in initial names.(default is false)</param>
			/// <returns>return Found child node or NULL if no child node with this name exists.</returns>

			FbxNode^ FindChild(String^ name, bool recursive, bool initial);


			/// <summary>
			///Finds a child node by name.	
			/// </summary>
			/// <param name="name"> Name of the searched child node.</param>		
			/// <returns>return Found child node or NULL if no child node with this name exists.</returns>
			FbxNode^ FindChild(String^ name)
			{
				return FindChild(name,false,false);
			}

			//@}

			/**
			* \name Node Target Management
			* When set, the target defines the orientation of the node.
			*
			* By default, the node's X axis points towards the target. A rotation
			* offset can be added to change this behavior. While the default
			* relative orientation to target is right for cameras, this feature is
			* useful for lights because they require a 90-degree offset on the Z
			* axis.
			*
			* By default, the node's up vector points towards the Up node.
			* If an Up node is not specified, the node's Up vector points towards the Y axis. A
			* rotation offset can be added to change this behavior. While the default
			* relative orientation to target is right for cameras, this feature is
			* useful for lights because they require a 90-degree offset on the Z
			* axis.
			*/
			//@{

			

			/// <summary>
			///Get or Set the target for this node.
			/// </summary>		
			/// <returns>returns  NULL if target isn't set.</returns>
			/// <remarks>
			/// The target must be part of the same scene and it cannot be itself.
			/// </remarks>
			REF_PROPERTY_GETSET_DECLARE(FbxNode,Target);									

			/** Get rotation offset from default relative orientation to target.
			* \return The rotation offset.
			*/
			/** Set rotation offset from default relative orientation to target.
			* \param pVector The rotation offset.
			*/

			/// <summary>
			///Get or Set rotation offset from default relative orientation to target.
			/// </summary>					
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,PostTargetRotation);			

			/** Get the target up node.
			* \return \c NULL if the target up model isn't set.
			*/
			/** The target up node must be part of the same scene and it cannot be itself.
			* \param pNode The target.
			*/
			/// <summary>
			///Get or Set the target up node.
			/// </summary>		
			REF_PROPERTY_GETSET_DECLARE(FbxNode,TargetUp);

			/** Get up vector offset from default relative target up vector.
			* \return The up vector offset.
			*/
			/** Set up vector offset from default relative target up vector.
			* \param pVector The rotation offset.
			*/
			/// <summary>
			///Get or Set up vector offset from default relative target up vector.
			/// </summary>	
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,TargetUpVector);			

			//@}


			/**
			* \name UpdateId Management
			*/
			//@{		
			//virtual kFbxUpdateId GetUpdateId(eFbxUpdateIdType pUpdateId=eUpdateId_Object) const;
			//@}

			/**
			* \name Node Display Parameters
			*/
			//@{			
			/** Get visibility.
			* \return \c true if node is visible in the scene.
			*/
			/** Set visibility.
			* \param pIsVisible Node is visible in the scene if set to \c true.
			*/
				/// <summary>
			///Get or Set visibility.
			/// </summary>				
			property bool Visible
			{
				bool get();
				void set(bool value);
			}

			/** \enum EShadingMode Shading modes.
			* - \e eHARD_SHADING
			* - \e eWIRE_FRAME
			* - \e eFLAT_SHADING
			* - \e eLIGHT_SHADING
			* - \e eTEXTURE_SHADING
			* - \e eLIGHT_TEXTURE_SHADING
			*/
			enum class ShadingMode
			{
				HardShading = KFbxNode::eHARD_SHADING,
				WireFrame = KFbxNode::eWIRE_FRAME,
				FlatShading = KFbxNode::eFLAT_SHADING,
				LightShading = KFbxNode::eLIGHT_SHADING,
				TextureShading = KFbxNode::eTEXTURE_SHADING,
				LightTextureShading = KFbxNode::eLIGHT_TEXTURE_SHADING
			};

			/** Get the shading mode.
			* \return The currently set shading mode.
			*/
			/** Set the shading mode.
			* \param pShadingMode The shading mode.
			*/	

			/// <summary>
			///Get or Set shading mode..
			/// </summary>			
			property ShadingMode Shading_Mode
			{
				ShadingMode get();
				void set(ShadingMode value);
			}			
			/** Get multilayer state.
			* \return The current state of the multi-layer flag.
			*/
			/** Enable or disable the multilayer state.
			* \param pMultiLayer The new state of the multi-layer flag.
			*/


			/// <summary>
			///Get or Set multilayer state.
			/// </summary>	
			property bool MultiLayer
			{
				bool get();
				void set(bool value);
			}

			/** \enum EMultiTakeMode MultiTake states.
			* - \e eOLD_MULTI_TAKE
			* - \e eMULTI_TAKE
			* - \e eMONO_TAKE
			*/
			enum class MultiTakeMode
			{
				OldMultiTake = KFbxNode::eOLD_MULTI_TAKE,
				MultiTake = KFbxNode::eMULTI_TAKE,
				MonoTake = KFbxNode::eMONO_TAKE
			};


			/** Get multitake mode.
			* \return The currently set multitake mode.
			*/
			/** Set the multitake mode.
			* \param pMultiTakeMode The multitake mode to set.
			*/



			
			/// <summary>
			///Get or Set multitake mode.
			/// </summary>	
			VALUE_PROPERTY_GETSET_DECLARE(MultiTakeMode ,MultiTake_Mode);			
			//@}

			/**
			* \name Node Attribute Management
			*/
			//@{

			/** Get the default node attribute.
			* \return Pointer to the default node attribute or \c NULL if the node doesn't
			* have a node attribute.
			*/
			/** Set the node attribute.
			* \param pNodeAttribute Node attribute object
			* \return Pointer to previous node attribute object.
			* \c NULL if the node didn't have a node attribute or if
			* the new node attribute is equal to the previous node attribute.
			* \remarks A node attribute can be shared between nodes.
			* \remarks If this node has more than one attribute, the deletion
			* of other attributes is done.
			*/			

			REF_PROPERTY_GETSET_DECLARE(FbxNodeAttribute,NodeAttribute);			

			/** Get the default node attribute.
			* \return Pointer to the default node attribute or \c NULL if the node doesn't
			* have a node attribute.
			*/
			//KFbxNodeAttribute const* GetNodeAttribute() const;

			/** Get the count of node attribute(s).
			* \return Number of node attribute(s) connected to this node.
			*/

			VALUE_PROPERTY_GET_DECLARE(int,Attribute_Count);			

			/** Get index of the default node attribute.
			* \return index of the default node attribute or
			* \c -1 if there is no default node attribute
			*/			
			VALUE_PROPERTY_GET_DECLARE(int,DefaultNodeAttributeIndex);			


			/** Set index of the default node attribute.
			* \return true if the operation succeeds or
			* \c false in other case.
			* In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
				
			/// <summary>
			///Set index of the default node attribute.
			/// </summary>	
			/// <returns>return true if the operation succeeds or false in other case.</returns>
			bool SetDefaultNodeAttributeIndex(int index);

			/** Get node attribute by index.
			* \return Pointer to corresponding node attribure or
			* \c NULL if index is out of range.
			* In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			/// <summary>
			///Get node attribute by index.
			/// </summary>	
			/// <returns>return Pointer to corresponding node attribure or NULL if index is out of range.</returns>
			/// <remarks>
			/// In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			/// </remarks>
			FbxNodeAttribute^ GetNodeAttributeByIndex(int index);

			/** Get node attribute by index.
			* \return Pointer to corresponding node attribure or
			* \c NULL if index is out of range.
			* In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			*/
			//KFbxNodeAttribute const* GetNodeAttributeByIndex(int pIndex) const;

			

			/// <summary>
			///Get index corresponding to a given node attribute Pointer.
			/// </summary>	
			/// <param name="nodeAttribute">The pointer to a node attribute.</param>
			/// <returns>Index of the node attribute or -1 if pNodeAttribute is NULL or not connected to this node.</returns>
			/// <remarks>
			/// In the last case, KFbxNode::GetLastErrorID() returns eATTRIBUTE_NOT_CONNECTED.
			/// </remarks>
			int GetNodeAttributeIndex(FbxNodeAttribute^ nodeAttribute);

			

			/// <summary>
			///Add a connection to a given node attribute Pointer.
			/// </summary>	
			/// <param name="nodeAttribute">The pointer to a node attribute.</param>
			/// <returns>true if the operation succeeded or false if the operation failed.</returns>
			/// <remarks>
			/// If the parameter node attribute is already connected to this node, false is returned
			/// </remarks>
			bool AddNodeAttribute(FbxNodeAttribute^ nodeAttribute);

			
			
			/// <summary>
			/// Remove a connection from a given node attribute.
			/// </summary>	
			/// <param name="nodeAttribute">The pointer to a node attribute.</param>
			/// <returns>true if the operation succeeded or false if the operation failed.</returns>
			/// <remarks>
			/// In the last case, KFbxNode::GetLastErrorID() returns eATTRIBUTE_NOT_CONNECTED.
			/// </remarks>
			bool RemoveNodeAttribute(FbxNodeAttribute^ nodeAttribute);

			
			/// <summary>
			/// Remove a connection from a given node attribute.
			/// </summary>	
			/// <param name="nodeAttribute">The pointer to a node attribute.</param>
			/// <returns>Pointer to the removed node attribute or NULL if the operation failed.</returns>
			/// <remarks>
			/// In the last case, KFbxNode::GetLastErrorID() returns eINDEX_OUT_OF_RANGE.
			/// </remarks>
			FbxNodeAttribute^ RemoveNodeAttributeByIndex(int index);

			/** Get the node attribute casted to a KFbxNull pointer.
			* \return Pointer to the null. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eNULL.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNull,Null);			

			/** Get the node attribute casted to a KFbxMarker pointer.
			* \return Pointer to the marker. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eMARKER.
			*/
			REF_PROPERTY_GET_DECLARE(FbxMarker,Marker);

			/** Get the node attribute casted to a KFbxSkeleton pointer.
			* \return Pointer to the skeleton. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eSKELETON.
			*/
			REF_PROPERTY_GET_DECLARE(FbxSkeleton,Skeleton);

			/** Get the node attribute casted to a KFbxGeometry pointer.
			* \return Pointer to the geometry. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eMESH,
			* KFbxNodeAttribute::eNURB or KFbxNodeAttribute::ePATCH.
			*/
			REF_PROPERTY_GET_DECLARE(FbxGeometry,Geometry);

			/** Get the node attribute casted to a KFbxMesh pointer.
			* \return Pointer to the mesh. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eMESH.
			*/
			REF_PROPERTY_GET_DECLARE(FbxMesh,Mesh);

			/** Get the node attribute casted to a KFbxNurb pointer.
			* \return Pointer to the nurb. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eNURB.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNurb,Nurb);

			/** Get the node attribute casted to a KFbxNurbsSurface pointer.
			* \return Pointer to the nurbs surface. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eNURBS_SURFACE.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNurbsSurface,NurbsSurface);

			/** Get the node attribute casted to a KFbxNurbsCurve pointer.
			* \return Pointer to the nurbs curve. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eNURBS_CURVE.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNurbsCurve,NurbsCurve);

			/** Get the node attribute casted to a KFbxNurbsSurface pointer.
			* \return Pointer to the nurbs surface. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eNURBS_SURFACE.
			*/
			REF_PROPERTY_GET_DECLARE(FbxTrimNurbsSurface,TrimNurbsSurface);

			/** Get the node attribute casted to a KFbxPatch pointer.
			* \return Pointer to the patch. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::ePATCH.
			*/
			REF_PROPERTY_GET_DECLARE(FbxPatch,Patch);

			/** Get the node attribute casted to a KFbxCamera pointer.
			* \return Pointer to the camera. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eCAMERA.
			*/
			REF_PROPERTY_GET_DECLARE(FbxCamera,Camera);

			/** Get the node attribute casted to a KFbxCameraSwitcher pointer.
			* \return Pointer to the camera switcher. \c NULL if the node doesn't have
			* a node attribute or if the node attribute type is not
			* KFbxNodeAttribute::eCAMERA_SWITCHER.
			*/
			REF_PROPERTY_GET_DECLARE(FbxCameraSwitcher,CameraSwitcher);

			/** Get the node attribute casted to a KFbxLight pointer.
			* \return Pointer to the light. \c NULL if the node doesn't have a node
			* attribute or if the node attribute type is not KFbxNodeAttribute::eLIGHT.
			*/
			REF_PROPERTY_GET_DECLARE(FbxLight,Light);

			/** Get the node attribute casted to a KFbxOpticalReference pointer.
			* \return Pointer to the optical reference. \c NULL if the node doesn't
			* have a node attribute or if the node attribute type is not
			* KFbxNodeAttribute::eOPTICAL_REFERENCE.
			*/
			REF_PROPERTY_GET_DECLARE(FbxOpticalReference,OpticalReference);
			//@}

			/**
			* \name Default Animation Values
			* This set of functions provides direct access to default
			* animation values in the default take node.
			*/
			//@{

			/** Set default translation vector (in local space).
			* \param pT The translation vector.
			*/
			void SetDefaultT(FbxVector4^ t);

			/** Get default translation vector (in local space).
			* \param pT The vector that will receive the default translation value.
			* \return Input parameter filled with appropriate data.
			*/
			void GetDefaultT(FbxVector4^ t);

			/** Set default rotation vector (in local space).
			* \param pR The rotation vector.
			*/
			void SetDefaultR(FbxVector4^ r);

			/** Get default rotation vector (in local space).
			* \param pR The vector that will receive the default rotation value.
			* \return Input parameter filled with appropriate data.
			*/
			void GetDefaultR(FbxVector4^ r);

			/** Set default scale vector (in local space).
			* \param pS The rotation vector.
			*/
			void SetDefaultS(FbxVector4^ s);

			/** Get default scale vector (in local space).
			* \param pS The vector that will receive the default translation value.
			* \return Input parameter filled with appropriate data.
			*/
			void GetDefaultS(FbxVector4^ s);						

			/** Get default visibility.
			* \return A value on a scale from 0 to 1.
			* 0 means hidden and any higher value means visible.
			* \remarks This parameter is only effective if node visibility
			* is enabled. Function KFbxNode::SetVisibility() enables
			* node visibility.
			*/
			/** Set default visibility.
			* \param pVisibility A value on a scale from 0 to 1.
			* 0 means hidden and any higher value means visible.
			* \remarks This parameter is only effective if node visibility
			* is enabled. Function KFbxNode::SetVisibility() enables
			* node visibility.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DefaultVisibility);


			//@}

			/**
			* \name Transformation propagation
			* This set of functions provides direct access to
			* the transformation propagations settings of the KFbxNode.
			* Those settings determine how transformations must be applied
			* when evaluating a node's transformation matrix.
			*/
			//@{			
			/** Get transformation inherit type.
			* \param pInheritType The returned value.
			*/
			/** Set transformation inherit type.
			* Set how the Translation/Rotation/Scaling transformations of a parent
			* node affect his childs.
			* \param pInheritType One of the following values eINHERIT_RrSs, eINHERIT_RSrs or eINHERIT_Rrs
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTransformInheritType,TransformationInheritType);
			//@}


			/**
			* \name Pivot Management
			* Pivots are used to specify translation, rotation and scaling centers
			* in coordinates relative to a node's origin. A node has two pivot
			* contexts defined by the EPivotSet enumeration. The node's animation
			* data can be converted from one pivot context to the other.
			*/
			//@{

			/** \enum EPivotSet  Pivot sets.
			* - \e eSOURCE_SET
			* - \e eDESTINATION_SET
			*/
			enum class PivotSet
			{
				SourceSet = KFbxNode::eSOURCE_SET,
				DestinationSet = KFbxNode::eDESTINATION_SET
			};

			/** \enum EPivotState  Pivot state.
			* - \e ePIVOT_STATE_ACTIVE
			* - \e ePIVOT_STATE_REFERENCE
			*/
			enum class PivotState
			{
				Active = KFbxNode::ePIVOT_STATE_ACTIVE,
				Reference = KFbxNode::ePIVOT_STATE_REFERENCE
			};

			/** Set the pivot state.
			* Tell FBX to use the pivot for TRS computation (ACTIVE), or
			* just keep it as a reference.
			* \param pPivotSet Specify which pivot set to modify its state.
			* \param pPivotState The new state of the pivot set.
			*/
			void SetPivotState(PivotSet pivotSet,PivotState pivotState);

			/** Get the pivot state.
			* Return the state of the pivot. If ACTIVE, we must take the pivot
			* TRS into account when computing the final transformation of a node.
			* \param pPivotSet Specify which pivot set to retrieve its state.
			* \param pPivotState The current state of the pivot set.
			*/
			void GetPivotState(PivotSet pivotSet, PivotState %pivotState);

			/** Set rotation space
			* Determine the rotation space (Euler or Spheric) and the rotation order.
			* \param pPivotSet Specify which pivot set to modify its rotation order.
			* \param pRotationOrder The new state of the pivot rotation order.
			*/
			void SetRotationOrder(PivotSet pivotSet, FbxRotationOrder rotationOrder);

			/** Get rotation order
			* \param pPivotSet Specify which pivot set to retrieve its rotation order.
			* \param pRotationOrder The current rotation order of the pivot set.
			*/
			void GetRotationOrder(PivotSet pivotSet, FbxRotationOrder %rotationOrder);

			/** Set rotation space for limit only.
			* \param pPivotSet Specify which pivot set to set the rotation space limit flag.
			* \param pUseForLimitOnly
			* When set to \c true, the current rotation space (set with SetRotationOrder)
			* define the rotation space for the limit only; leaving the rotation animation
			* in Euler XYZ space. When set to \c false, the current rotation space defines
			* the rotation space for both the limits and the rotation animation data.
			*/
			void SetUseRotationSpaceForLimitOnly(PivotSet pivotSet, bool useForLimitOnly);

			/** Get rotation space for limit only.
			* \param pPivotSet Specify which pivot set to query.
			* \return The rotation space limit flag current value.
			*/
			bool GetUseRotationSpaceForLimitOnly(PivotSet pivotSet);						

			/** Get the RotationActive state.
			* \return The value of the RotationActive flag.
			*/
			/** Set the RotationActive state.
			* \param pVal The new state of the property.
			* \remark When this flag is set to false, the RotationOrder, the Pre/Post rotation values
			* and the rotation limits should be ignored.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationActive);

			/** Set the Quaternion interpolation mode
			* \param pPivotSet Specify which pivot set to query.
			* \param pUseQuaternion The new value for the flag.
			*/
			void SetUseQuaternionForInterpolation(PivotSet pPivotSet, bool useQuaternion);

			/** Get the Quaternion interpolation mode
			* \param pPivotSet Specify which pivot set to query.
			* \return The currently state of the flag.
			*/
			bool GetUseQuaternionForInterpolation(PivotSet pivotSet);


			/** Get the rotation stiffness
			* \return The currently set rotation stiffness values.
			*/
			/** Set the rotation stiffness.
			* The stiffness attribute is used by IK solvers to generate a resistance
			* to a joint motion. The higher the stiffness the less it will rotate.
			* Stiffness works in a relative sense: it determines the willingness of
			* this joint to rotate with respect to the other joint in the IK chain.
			* \param pRotationStiffness The rotation stiffness values are limited to
			* the range [0, 100].
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,RotationStiffness);

			/** Get the minimum damp range angles
			* \return The currently set minimum damp range angles.
			*/			
			/** Set the minimum damp range angles.
			* This attributes apply resistance to a joint rotation as it approaches the
			* lower boundary of its rotation limits. This functionality allows joint
			* motion to slow down smoothly until the joint reaches its rotation limits
			* instead of stopping abruptly. The MinDampRange specifies when the
			* deceleration should start.
			* \param pMinDampRange : Angle in degrees where deceleration should start
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,MinDampRange);					

			/** Get the maximum damp range angles
			* \return The currently set maximum damp range angles.
			*/
			/** Set the maximum damp range angles.
			* This attributes apply resistance to a joint rotation as it approaches the
			* upper boundary of its rotation limits. This functionality allows joint
			* motion to slow down smoothly until the joint reaches its rotation limits
			* instead of stopping abruptly. The MaxDampRange specifies when the
			* deceleration should start.
			* \param pMaxDampRange : Angle in degrees where deceleration should start
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,MaxDampRange);


			/** Get the miminum damp strength
			* \return The currently set minimum damp strength values.
			*/
			/** Set the minimum damp strength.
			* This attributes apply resistance to a joint rotation as it approaches the
			* lower boundary of its rotation limits. This functionality allows joint
			* motion to slow down smoothly until the joint reaches its rotation limits
			* instead of stopping abruptly. The MinDampStrength defines the
			* rate of deceleration
			* \param pMinDampStrength Values are limited to the range [0, 100].
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,MinDampStrength);					

			/** Get the maximum damp strength
			* \return The currently set maximum damp strength values.
			*/
			/** Set the maximum damp strength.
			* This attributes apply resistance to a joint rotation as it approaches the
			* upper boundary of its rotation limits. This functionality allows joint
			* motion to slow down smoothly until the joint reaches its rotation limits
			* instead of stopping abruptly. The MaxDampStrength defines the
			* rate of deceleration
			* \param pMaxDampStrength Values are limited to the range [0, 100].
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,MaxDampStrength);					



			/** Get the prefered angle
			* \return The currently set prefered angle.
			*/
			/** Set the prefered angle.
			* The preferredAngle attribute defines the initial joint configuration used
			* by a single chain ik solver to calculate the inverse kinematic solution.
			* \param pPreferedAngle Angle in degrees
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,PreferedAngle);

			/** Set a translation offset for the rotation pivot.
			* The translation offset is in coordinates relative to the node's origin.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The translation offset.
			*/
			void SetRotationOffset(PivotSet pivotSet, FbxVector4^ vector);

			/** Get the translation offset for the rotation pivot.
			* The translation offset is in coordinates relative to the node's origin.
			* \param pPivotSet Specify which pivot set to to query the value.
			* \return The currently set vector.
			*/
			FbxVector4^ GetRotationOffset(PivotSet pivotSet);

			/** Set rotation pivot.
			* The rotation pivot is the center of rotation in coordinates relative to
			* the node's origin.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The new position of the rotation pivot.
			*/
			void SetRotationPivot(PivotSet pivotSet, FbxVector4^ vector);

			/** Get rotation pivot.
			* The rotation pivot is the center of rotation in coordinates relative to
			* the node's origin.
			* \param pPivotSet Specify which pivot set to query.
			* \return The current position of the rotation pivot.
			*/
			FbxVector4^ GetRotationPivot(PivotSet pivotSet);

			/** Set pre-rotation in Euler angles.
			* The pre-rotation is the rotation applied to the node before
			* rotation animation data.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The X,Y,Z rotation values to set.
			*/
			void SetPreRotation(PivotSet pivotSet, FbxVector4^ vector);

			/** Get pre-rotation in Euler angles.
			* The pre-rotation is the rotation applied to the node before
			* rotation animation data.
			* \param pPivotSet Specify which pivot set to query.
			* \return The X,Y and Z rotation values.
			*/
			FbxVector4^ GetPreRotation(PivotSet pivotSet);

			/** Set post-rotation in Euler angles.
			* The post-rotation is the rotation applied to the node after the
			* rotation animation data.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The X,Y,Z rotation values to set.
			*/
			void SetPostRotation(PivotSet pivotSet, FbxVector4^ vector);

			/** Get post-rotation in Euler angles.
			* The post-rotation is the rotation applied to the node after the
			* rotation animation data.
			* \param pPivotSet Specify which pivot set to query.
			* \return The X,Y and Z rotation values.
			*/
			FbxVector4^ GetPostRotation(PivotSet pivotSet);

			/** Set a translation offset for the scaling pivot.
			* The translation offset is in coordinates relative to the node's origin.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The translation offset.
			*/
			void SetScalingOffset(PivotSet pivotSet, FbxVector4^ vector);

			/** Get the translation offset for the scaling pivot.
			* The translation offset is in coordinates relative to the node's origin.
			* \param pPivotSet Specify which pivot set to query the value.
			* \return The currently set vector.
			*/
			FbxVector4^ GetScalingOffset(PivotSet pivotSet);

			/** Set scaling pivot.
			* The scaling pivot is the center of scaling in coordinates relative to
			* the node's origin.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector
			* \return The new position of the scaling pivot.
			*/
			void SetScalingPivot(PivotSet pivotSet, FbxVector4^ vector);

			/** Get scaling pivot.
			* The scaling pivot is the center of scaling in coordinates relative to
			* the node's origin.
			* \param pPivotSet Specify which pivot set to query.
			* \return The current position of the scaling pivot.
			*/
			FbxVector4^ GetScalingPivot(PivotSet pivotSet);

			/** Set geometric translation
			* The geometric translation is a local translation that is applied
			* to a node attribute only. This translation is applied to the node attribute
			* after the node transformations. This translation is not inherited across the
			* node hierarchy.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The translation vector.
			*/
			void SetGeometricTranslation(PivotSet pivotSet, FbxVector4^ vector);

			/** Get geometric translation
			* \param pPivotSet Specify which pivot set to query.
			* \return The current geometric translation.
			*/
			FbxVector4^ GetGeometricTranslation(PivotSet pivotSet);

			/** Set geometric rotation
			* The geometric rotation is a local rotation that is applied
			* to a node attribute only. This rotation is applied to the node attribute
			* after the node transformations. This rotation is not inherited across the
			* node hierarchy.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The X,Y and Z rotation values.
			*/
			void SetGeometricRotation(PivotSet pivotSet, FbxVector4^ vector);

			/** Get geometric rotation
			* \param pPivotSet Specify which pivot set to query.
			* \return The current geometric rotation.
			*/
			FbxVector4^ GetGeometricRotation(PivotSet pivotSet);

			/** Set geometric scaling
			* The geometric scaling is a local scaling that is applied
			* to a node attribute only. This scaling is applied to the node attribute
			* after the node transformations. This scaling is not inherited across the
			* node hierarchy.
			* \param pPivotSet Specify which pivot set to modify.
			* \param pVector The X,Y and Z scale values.
			*/
			void SetGeometricScaling(PivotSet pivotSet, FbxVector4^ vector);

			/** Get geometric scaling
			* \return The current geometric scaling.
			*/
			FbxVector4^ GetGeometricScaling(PivotSet pivotSet);

			/** Recursively convert the animation data according to pivot settings.
			* \param pConversionTarget If set to EPivotSet::eDESTINATION_SET,
			* convert animation data from the EPivotSet::eSOURCE_SET pivot context
			* to the EPivotSet::eDESTINATION_SET pivot context. Otherwise, the
			* conversion is computed the other way around.
			* \param pFrameRate Resampling frame rate in frames per second.
			* \param pKeyReduce Apply or skip key reducing filter.
			*/
			void ConvertPivotAnimation(PivotSet conversionTarget, double frameRate, bool keyReduce);
			void ConvertPivotAnimation(PivotSet conversionTarget, double frameRate)
			{
				ConvertPivotAnimation(conversionTarget,frameRate,true);
			}

			/** Second version of ConvertPivotAnimation.  This version now takes into account the new pivot set
			* \param pConversionTarget If set to EPivotSet::eDESTINATION_SET,
			* convert animation data from the EPivotSet::eSOURCE_SET pivot context
			* to the EPivotSet::eDESTINATION_SET pivot context. Otherwise, the
			* conversion is computed the other way around.
			* \param pFrameRate Resampling frame rate in frames per second.
			* \param pKeyReduce Apply or skip key reducing filter.
			*/
			void ConvertPivotAnimationRecursive(PivotSet conversionTarget, double frameRate, bool keyReduce);
			void ConvertPivotAnimationRecursive(PivotSet conversionTarget, double frameRate)
			{
				ConvertPivotAnimationRecursive(conversionTarget,frameRate,true);
			}

			/** Reset a pivot set to the default pivot context.
			* \param pPivotSet Pivot set to reset.
			* \remarks The default pivot context is with all the pivots disabled.
			*/
			void ResetPivotSet(PivotSet pivotSet);

			/** Reset all the pivot sets to the default pivot context and convert the animation.
			* \param pFrameRate Resampling frame rate in frames per second.
			* \param pKeyReduce Apply or skip key reducing filter.
			* \remarks The resulting animation will be visually equivalent and all the pivots will be cleared.
			* \remarks Will recursively convert the animation of all the children nodes.
			*/
			void ResetPivotSetAndConvertAnimation( double frameRate, bool keyReduce);
			void ResetPivotSetAndConvertAnimation( double frameRate)
			{
				ResetPivotSetAndConvertAnimation(frameRate,false);
			}
			void ResetPivotSetAndConvertAnimation()
			{
				ResetPivotSetAndConvertAnimation(30,false);
			}

			//@}

			/**
			* \name Access to TRS Local and Global Position
			*/
			//@{

			/** Gets the Local Translation from the default take
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Translation.
			*/
			FbxVector4^ GetLocalTFromDefaultTake(bool applyLimits);
			FbxVector4^ GetLocalTFromDefaultTake()
			{
				return GetLocalTFromDefaultTake(false);
			}

			/** Gets the Local Rotation from the default take
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Rotation.
			*/
			FbxVector4^ GetLocalRFromDefaultTake(bool applyLimits);
			FbxVector4^ GetLocalRFromDefaultTake()
			{
				return GetLocalRFromDefaultTake(false);
			}

			/** Gets the Local Scale from the default take
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Scale.
			*/
			FbxVector4^ GetLocalSFromDefaultTake(bool applyLimits);
			FbxVector4^ GetLocalSFromDefaultTake()
			{
				return GetLocalSFromDefaultTake(false);
			}

			/** Get the Global Transformation Matrix from the default take
			* \param  pPivotSet   The pivot set to take into account
			* \param pApplyTarget Applies the necessary transform to align into the target node
			* \return             The Global Transformation Matrix
			*/
			FbxXMatrix^ GetGlobalFromDefaultTake(PivotSet pivotSet, bool applyTarget);
			FbxXMatrix^ GetGlobalFromDefaultTake(PivotSet pivotSet)
			{
				return GetGlobalFromDefaultTake(pivotSet,false);
			}

			/** Gets the Local Translation from the current take at a given time
			* \param  pTime       The time at which we want to evaluate
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Translation.
			*/
			FbxVector4^ GetLocalTFromCurrentTake(FbxTime^ time, bool applyLimits);
			FbxVector4^ GetLocalTFromCurrentTake(FbxTime^ time)
			{
				return GetLocalTFromCurrentTake(time, false);
			}

			/** Gets the Local Rotation from the current take at a given time
			* \param  pTime       The time at which we want to evaluate
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Rotation.
			*/
			FbxVector4^ GetLocalRFromCurrentTake(FbxTime^ time, bool applyLimits);
			FbxVector4^ GetLocalRFromCurrentTake(FbxTime^ time)
			{
				return GetLocalRFromCurrentTake(time,false);
			}

			/** Gets the Local Scale from the current take at a given time
			* \param  pTime       The time at which we want to evaluate
			* \param pApplyLimits true if node limits are to be applied on result
			* \return             The Local Scale.
			*/			
			FbxVector4^ GetLocalSFromCurrentTake(FbxTime^ time, bool applyLimits);
			FbxVector4^ GetLocalSFromCurrentTake(FbxTime^ time)
			{
				return GetLocalSFromCurrentTake(time,false);
			}

			/** Get the Global Transformation Matrix from the current take at a given time
			* \param  pTime       The time at which we want to evaluate
			* \param  pPivotSet   The pivot set to take into accounr
			* \param pApplyTarget Applies the necessary transform to align into the target node
			* \return             The Global Transformation Matrix
			*/
			FbxXMatrix^ GetGlobalFromCurrentTake(FbxTime^ time, PivotSet pivotSet, bool applyTarget);
			FbxXMatrix^ GetGlobalFromCurrentTake(FbxTime^ time, PivotSet pivotSet)
			{
				return GetGlobalFromCurrentTake(time,pivotSet,false);
			}
			FbxXMatrix^ GetGlobalFromCurrentTake(FbxTime^ time)
			{
				return GetGlobalFromCurrentTake(time,PivotSet::SourceSet,false);
			}
			//@}

			/**
			* \name Character Link
			*/
			//@{

			/** Get number of character links.
			* \return The number of character links.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CharacterLinkCount);

			/** Get character link at given index.
			* \param pIndex Index of character link.
			* \param pCharacter Pointer to receive linked character if function succeeds.
			* \param pCharacterLinkType Pointer to receive character link type if function succeeds,
			* cast to \c ECharacterLinkType.
			* \param pNodeId Pointer to receive node ID if function succeeds. Cast to \c ECharacterNodeId
			* if returned character link type is \c eCharacterLink or \c eControlSetLink. Cast to
			* \c EEffectorNodeId if returned character link type is \c eControlSetEffector or
			* \c eControlSetEffectorAux.
			* \param pNodeSubId
			* \return \c true if function succeeds, \c false otherwise.
			*/
			//bool GetCharacterLink(int pIndex, KFbxCharacter** pCharacter, int* pCharacterLinkType, int* pNodeId, int *pNodeSubId);

			/** Find if a given character link exists.
			* \param pCharacter Character searched.
			* \param pCharacterLinkType Character link type searched, cast to \c ECharacterLinkType.
			* \param pNodeId Node ID searched. Cast from to \c ECharacterNodeId if searched
			* character link type is \c eCharacterLink or \c eControlSetLink. Cast from
			* \c EEffectorNodeId if searched character link type is \c eControlSetEffector or
			* \c eControlSetEffectorAux.
			* \param pNodeSubId
			* \return Index of found character link if it exists, -1 otherwise.
			*/
			int FindCharacterLink(FbxCharacter^ character, int characterLinkType, int nodeId, int nodeSubId);
			//@}

			/** Find out start and end time of the current take.
			* Query a node and all its children recursively for the current take node
			* start and end time.
			* \param pStart Reference to store start time.
			* \c pStart is overwritten only if start time found is lower than \c pStart value.
			* Initialize to KTIME_INFINITE to make sure the start time is overwritten in any case.
			* \param pStop Reference to store end time.
			* \c pStop is overwritten only if stop time found is higher than \c pStop value.
			* Initialize to KTIME_MINUS_INFINITE to make sure the stop time is overwritten in any case.
			* \return \c true on success, \c false otherwise.
			*/
			//virtual bool GetAnimationInterval(FbxTime^ start, FbxTime^ stop);


			/**
			* \name Material Management
			*/
			//@{

			/** Add a material to this node.
			* \param pMaterial The material to add.
			* \return non-negative index of added material, or -1 on error.
			*/
			int AddMaterial(FbxSurfaceMaterial^ material);

			/** Remove a material from this node.
			* \param pMaterial The material to remove.
			* \return true on success, false otherwise
			*/
			bool RemoveMaterial( FbxSurfaceMaterial^ material );

			/**
			* \return The number of materials applied to this node
			*/
			VALUE_PROPERTY_GET_DECLARE(int,MaterialCount);

			/** Access a material on this node.
			* \param pIndex Valid range is [0, GetMaterialCount() - 1]
			* \return The pIndex-th material, or NULL if pIndex is invalid.
			*/
			FbxSurfaceMaterial^ GetMaterial( int index );

			/** Remove all materials applied to this node.
			*/
			void RemoveAllMaterials();

			/** Find an applied material with the given name.
			* \param pName The requested name
			* \return an index to a material, or -1 if no applied material
			* has the requested name.
			*/
			int GetMaterialIndex(String^ name);

			//@}

			/**
			* \name Error Management
			* The same error object is shared among instances of this class.
			*/
			//@{

			/** Retrieve error object.
			* \return Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError  Error identifiers.
			* Some of these are only used internally.
			* - \e eTAKE_NODE_ERROR
			* - \e eNODE_NAME_CLASH
			* - \e eMATERIAL_NAME_CLASH
			* - \e eTEXTURE_NAME_CLASH
			* - \e eVIDEO_NAME_CLASH
			* - \e eNOT_A_CHILD
			* - \e eCYCLIC_GRAPH
			* - \e eINDEX_OUT_OF_RANGE
			* - \e eATTRIBUTE_NOT_CONNECTED
			* - \e eERROR_COUNT
			*/
			enum class Error
			{
				TakeNodeError = KFbxNode::eTAKE_NODE_ERROR,
				NodeNameClash = KFbxNode::eNODE_NAME_CLASH,
				MaterialNameClash = KFbxNode::eMATERIAL_NAME_CLASH,
				TextureNameClash = KFbxNode::eTEXTURE_NAME_CLASH,
				VideoNameClash = KFbxNode::eVIDEO_NAME_CLASH,
				NotAChild = KFbxNode::eNOT_A_CHILD,
				CyclicGraph = KFbxNode::eCYCLIC_GRAPH,
				IndexOutOfRange = KFbxNode::eINDEX_OUT_OF_RANGE,
				AttributeNotConnected = KFbxNode::eATTRIBUTE_NOT_CONNECTED,
				ErrorCount = KFbxNode::eERROR_COUNT
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


			/**
			* \name Public and fast access Properties
			*/
			//@{

			/** This property contains the translation information of the node
			*
			* To access this property do: LclTranslation.Get().
			* To set this property do: LclTranslation.Set(fbxDouble3).
			*
			* Default value is 0.,0.,0.
			*/			

		public:
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,LclTranslation);

			/** This property contains the rotation information of the node
			*
			* To access this property do: LclRotation.Get().
			* To set this property do: LclRotation.Set(fbxDouble3).
			*
			* Default value is 0.,0.,0.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,LclRotation);

			/** This property contains the scaling information of the node
			*
			* To access this property do: LclScaling.Get().
			* To set this property do: LclScaling.Set(fbxDouble3).
			*
			* Default value is 1.,1.,1.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,LclScaling);

			/** This property contains the global transform information of the node
			*
			* To access this property do: GlobalTransform.Get().
			* To set this property do: GlobalTransform.Set(KFbxXMatrix).
			*
			* Default value is identity matrix
			*/			
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,GlobalTransform);

			/** This property contains the visibility information of the node
			*
			* To access this property do: Visibility.Get().
			* To set this property do: Visibility.Set(fbxDouble1).
			*
			* Default value is 1.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,Visibility);

			VALUE_PROPERTY_GETSET_DECLARE(double,Weight);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,PoleVector);
			VALUE_PROPERTY_GETSET_DECLARE(double,Twist);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,WorldUpVector);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,UpVector);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,AimVector);
			VALUE_PROPERTY_GETSET_DECLARE(bool,QuaternionInterpolate);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,RotationOffset);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,RotationPivot);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ScalingOffset);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ScalingPivot);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationActive);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Translation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,TranslationMin);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,TranslationMax);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMinX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMinY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMinZ);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMaxX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMaxY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationMaxZ);

			VALUE_PROPERTY_GETSET_DECLARE(FbxRotationOrder,RotationOrder);			
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationSpaceForLimitOnly);
			VALUE_PROPERTY_GETSET_DECLARE(double,RotationStiffnessX);
			VALUE_PROPERTY_GETSET_DECLARE(double,RotationStiffnessY);
			VALUE_PROPERTY_GETSET_DECLARE(double,RotationStiffnessZ);
			VALUE_PROPERTY_GETSET_DECLARE(double,AxisLen);

			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,PreRotation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,PostRotation);			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,RotationMin);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,RotationMax);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMinX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMinY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMinZ);			
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMaxX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMaxY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationMaxZ);

			VALUE_PROPERTY_GETSET_DECLARE(FbxTransformInheritType,InheritType);

			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingActive);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Scaling);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ScalingMin);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ScalingMax);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMinX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMinY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMinZ);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMaxX);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMaxY);
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingMaxZ);

			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,GeometricTranslation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,GeometricRotation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,GeometricScaling);

			//			// Ik Settings
			//			//////////////////////////////////////////////////////////
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampRangeX);
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampRangeY);
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampRangeZ);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampRangeX);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampRangeY);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampRangeZ);
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampStrengthX);
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampStrengthY);
			VALUE_PROPERTY_GETSET_DECLARE(double,MinDampStrengthZ);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampStrengthX);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampStrengthY);
			VALUE_PROPERTY_GETSET_DECLARE(double,MaxDampStrengthZ);
			VALUE_PROPERTY_GETSET_DECLARE(double,PreferedAngleX);
			VALUE_PROPERTY_GETSET_DECLARE(double,PreferedAngleY);
			VALUE_PROPERTY_GETSET_DECLARE(double,PreferedAngleZ);
			//			///////////////////////////////////////////////////////
			//
			//			KFbxTypedProperty<fbxReference*>            LookAtProperty;
			//			KFbxTypedProperty<fbxReference*>            UpVectorProperty;
			////
			VALUE_PROPERTY_GETSET_DECLARE(bool,Show);
			VALUE_PROPERTY_GETSET_DECLARE(bool,NegativePercentShapeSupport);
			//
			VALUE_PROPERTY_GETSET_DECLARE(int,DefaultAttributeIndex);
			//			//@}
			//
			//
#ifndef DOXYGEN_SHOULD_SKIP_THIS
			///////////////////////////////////////////////////////////////////////////////
			//  WARNING!
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			///////////////////////////////////////////////////////////////////////////////
		public:
			/**
			* \name Local and Global States Management
			*/
			//@{

			/** Load in local state the TRS position relative to parent at a given time.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \param pApplyLimits true if node limits are to be applied on result
			* \remarks TRS position relative to parent is read from default take.
			* \remarks Has to be the DoF values
			*/
			void SetLocalStateFromDefaultTake(bool recursive, bool applyLimits);
			void SetLocalStateFromDefaultTake(bool recursive)
			{
				SetLocalStateFromDefaultTake(recursive,false);
			}

			/** Store local state as a TRS position relative to parent at a given time.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \remarks TRS position relative to parent is written in default take.
			*/
			void SetDefaultTakeFromLocalState(bool recursive);

			/** Load in local state the TRS position relative to parent at a given time.
			* \param pTime Given time to evaluate TRS position.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \param pApplyLimits true if node limits are to be applied on result
			* \remarks TRS position relative to parent is read from current take.
			*/
			void SetLocalStateFromCurrentTake(FbxTime^ time, bool recursive,bool applyLimits);
			void SetLocalStateFromCurrentTake(FbxTime^ time, bool recursive)
			{
				SetLocalStateFromCurrentTake(time,recursive,false);
			}

			/** Store local state as a TRS position relative to parent at a given time.
			* \param pTime Given time to store TRS position.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \remarks TRS position relative to parent is written in current take.
			*/
			void SetCurrentTakeFromLocalState(FbxTime^ time, bool recursive);

			/** Compute global state from local state.
			* \param pUpdateId Update ID to avoid useless recomputing.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \param pApplyTarget Applies the necessary transform to align into the target node
			* \remarks Local states of current node and all upward nodes are assumed to be valid.
			*/
			void ComputeGlobalState(kUInt updateId, bool recursive, PivotSet pivotSet, bool applyTarget);
			void ComputeGlobalState(kUInt updateId, bool recursive, PivotSet pivotSet)
			{
				ComputeGlobalState(updateId,recursive,pivotSet,false);
			}
			void ComputeGlobalState(kUInt updateId, bool recursive)
			{
				ComputeGlobalState(updateId,recursive,PivotSet::SourceSet,false);
			}

			/** Compute local state from global state.
			* \param pUpdateId Update ID to avoid useless recomputing.
			* \param pRecursive Flag to call the function recursively to children nodes.
			* \remarks Global states of current node and all upward nodes are assumed to be valid.
			*/
			void ComputeLocalState(kUInt updateId, bool recursive);


			//! Get global state.
			/** Set global state.
			* \param pGX TRS global position.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxXMatrix,GlobalState);

			/** Set local state.
			* \param pLX TRS position relative to parent.
			*/
			void SetLocalState(FbxVector4^ LT,FbxVector4^ LR,FbxVector4^ LS);

			//! Get local state.
			void GetLocalState(FbxVector4^ LT, FbxVector4^ LR, FbxVector4^ LS);

			/** Set global state ID.
			* \param pUpdateId Update ID to avoid useless recomputing.
			* \param pRecursive Flag to call the function recursively to children nodes.
			*/
			void SetGlobalStateId(kUInt updateId, bool recursive);

			//! Get global state ID.
			VALUE_PROPERTY_GET_DECLARE(kUInt, GlobalStateId);

			/** Set local state ID.
			* \param pUpdateId Update ID to avoid useless recomputing.
			* \param pRecursive Flag to call the function recursively to children nodes.
			*/
			void SetLocalStateId(kUInt updateId, bool recursive);

			//! Get local state ID.			
			VALUE_PROPERTY_GET_DECLARE(kUInt,LocalStateId);
			//@}

			REF_PROPERTY_GET_DECLARE(FbxNodeLimits,Limits);			


			// Clone,
			// Note this does not clone the node's attribute.
			CLONE_DECLARE();			

			void UpdatePivotsAndLimitsFromProperties();
			void UpdatePropertiesFromPivotsAndLimits();
			void SetRotationActiveProperty(bool val);

			void PivotSetToMBTransform(PivotSet pivotSet);
			FbxXMatrix^ GetLXFromLocalState( bool T, bool R, bool S, bool soff );

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}