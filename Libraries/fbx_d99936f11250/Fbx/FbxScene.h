#pragma once
#include "stdafx.h"
#include "FbxDocument.h"



{
	namespace FbxSDK
	{			
		ref class FbxSdkManagerManaged;
		ref class FbxNode;
		ref class FbxClassId;
		ref class FbxGenericNode;
		ref class FbxCharacter;
		ref class FbxCharacterPose;
		ref class FbxControlSet;
		ref class FbxControlSetPlug;
		ref class FbxPose;
		ref class FbxStringManaged;
		ref class FbxSurfaceMaterial;
		ref class FbxGlobalLightSettings;
		ref class FbxGlobalCameraSettings;
		ref class FbxGlobalTimeSettings;
		ref class FbxGlobalSettings;
		ref class FbxTexture;
		ref class FbxGeometry;		
		ref class FbxVideo;
		ref class FbxDocumentInfo;		
		ref class FbxTakeNodeContainer;
		///<summary>
		///This class contains the description of a complete 3D scene.
		///nosubgrouping
		/// The FBX SDK requires unique names for nodes, textures, materials, generic nodes, videos and gobos			///	contained
		/// in a given scene. To build a valid node tree, new nodes must be inserted under the
		/// scene's root node.		
		/// This class also provides access to global settings and take information.	
		///</summary>
		public ref class FbxSceneManaged : FbxDocumentManaged
		{
		internal:
			REF_DECLARE(FbxEmitter,FbxScene);
			FbxSceneManaged(FbxScene* instance) : FbxDocumentManaged(instance)
			{
				_Free = false;
			}	
		protected:
			virtual void CollectManagedMemory() override;
		public:

			FBXOBJECT_DECLARE(FbxSceneManaged);

			///<summary>
			/// Delete the node tree below the root node and restore default settings.
			///</summary>
			virtual void Clear() override;			

			///<summary>
			/// Get the root node.			
			///</summary>
			/// <returns>return Pointer to the root node.</returns>
			///<remarks>
			/// This node is not saved. Do not use it to apply a global transformation
			/// to the node hierarchy. If a global transformation must be applied, insert a
			/// new node below this one.						
			///</remarks>
			REF_PROPERTY_GET_DECLARE(FbxNode,RootNode);
			
			//
			//			/** Fill a node array will all existing node with a given node attribute type.
			//			*  \param pNodeArray An array of pointers to KFbxNode objects.
			//			*  \param pAttributeType Node attribute type searched.
			//			*  \param pDepthFirst If \c true, the node tree is scanned depth first.
			//			*  \remarks Scans the node hierarchy to find all existing node with a given node attribute type.
			//			*  The array of pointers is cleared before scanning the node hierarchy.
			//			*/
			//			void FillNodeArray(KArrayTemplate<KFbxNode*>& pNodeArray,
			//				KFbxNodeAttribute::EAttributeType pAttributeType,
			//				bool pDepthFirst = false);
			//
			//			//@}
			//
			//			/**
			//			* \name Texture Material and Video Access
			//			*/
			//			//@{
			//
			//			/** Clear then fill a texture array with all existing textures included in the scene.
			//			* \param pTextureArray An array of texture pointers.
			//			*/
			//			void FillTextureArray(KArrayTemplate<KFbxTexture*>& pTextureArray);
			//
			//			/** Clear then fill a material array with all existing materials included in the scene.
			//			* \param pMaterialArray An array of material pointers.
			//			*/
			//			void FillMaterialArray(KArrayTemplate<KFbxSurfaceMaterial*>& pMaterialArray);
			//
			//			//@}
			//
			//			/**
			//			* \name Generic Node Access
			//			*/
			//			//@{
			//


			///<summary>
			/// Get number of generic nodes.
			/// return Number of Generic Nodes in this scene.
			///</summary>
			property int GenericNodeCount
			{
				int get();
			}

			///<summary>
			/// Get generic node at given index.
			///</summary> 
			/// <param name='index'>Position in the list of the generic nodes.</param>			
			///<returns> return Pointer to the generic node or Null if the index is out of bounds.</returns>	
			FbxGenericNode^ GetGenericNode(int index);

			///<summary>
			/// Access a generic node from its name.
			///</summary>			
			/// <param name='name'>Name</param>			
			///<returns>return found generic node.</returns>
			FbxGenericNode^ GetGenericNode(String^ name);

			/** Add the generic node to this scene.
			* \param pGenericNode Pointer to the generic node to be added.
			* \return If the passed parameter is \c NULL, this method will return \c false otherwise \c true.
			*/
			bool AddGenericNode(FbxGenericNode^ genericNode);

			/** Remove the generic node from this scene.
			* \param pGenericNode Pointer to the generic node to be removed.
			* \return If the passed parameter is \c NULL, this method will return \c false otherwise \c true.
			* \remarks The pointed object is not referenced by the scene anymore but is not deleted from the system.
			*/
			bool RemoveGenericNode(FbxGenericNode^ genericNode);

			//@}


			/**
			* \name Character Management
			*/
			//@{

			/** Get number of characters.
			* \return Number of characters in this scene.
			*/			
			property int CharacterCount
			{
				int get();
			}

			/** Get character at given index.
			* \param pIndex Position in the list of the characters.
			* \return Pointer to the character or \c NULL if index is out of bounds.
			*/
			FbxCharacter^ GetCharacter(int index);

			/** Create a new character.
			* \param pName Name given to character.
			* \return Index of the created character.
			*/			
			int GetCharacter(String^ name);

			/** Destroy character.
			* \param pIndex Specify which character to destroy.
			* \remarks After the destruction  of the requested element the list is resized.
			*/
			void DestroyCharacter(int index);

			//@}

			/**
			* \name ControlSetPlug Management
			*/
			//@{

			/** Get number of ControlSetPlugs.
			* \return Number of ControlSet plugs in this scene.
			*/
			property int ControlSetPlugCount
			{
				int get();
			}

			/** Get ControlSetPlug at given index.
			* \param pIndex Position in the list of the ControlSetPlug
			* \return Pointer to ControlSetPlug or \c NULL if index is out of bounds.
			*/
			FbxControlSetPlug^ GetControlSetPlug(int index);

			/** Create a new ControlSetPlug.
			* \param pName Name given to ControlSetPlug.
			* \return Index of created ControlSetPlug.
			*/
			int CreateControlSetPlug(String^ name);

			/** Destroy ControlSetPlug.
			* \param pIndex Specify which ControlSetPlug to destroy.
			* \remarks After the destruction of the requested element the list is resized.
			*/
			void DestroyControlSetPlug(int index);

			//@}

			/**
			* \name Character Pose Management
			*/
			//@{

			/** Get number of character poses.
			* \return Number of character poses in this scene.
			* \remarks Character Poses and Poses are two distinct entities having their own lists.
			*/			
			property int CharacterPoseCount
			{
				int get();
			}

			/** Get character pose at given index.
			* \param pIndex Position in the list of character poses.
			* \return Pointer to the character pose or \c NULL if index is out of bounds.
			*/
			FbxCharacterPose^ GetCharacterPose(int index);

			/** Create a new character pose.
			* \param pName Name given to character pose.
			* \return Index of created character pose.
			*/			
			int CreateCharacterPose(String^ name);

			/** Destroy character pose.
			* \param pIndex Specify which character pose to destroy.
			* \remarks After the destruction of the requested element the list is resized.
			*/
			void DestroyCharacterPose(int index);



			/**
			* \name Pose Management
			*/
			//@{

			/** Get number of poses.
			* \return Number of poses in the scene.
			* \remarks Poses and Character Poses are two distinct entities having their own lists.
			*/			
			property int PoseCount
			{
				int get();
			}

			/** Get pose at given index.
			* \param pIndex Position in the list of poses.
			* \return Pointer to the pose or \c NULL if index is out of bounds.
			*/
			FbxPose^ GetPose(int index);

			/** Add a pose to this scene.
			* \param pPose The pose to be added to the scene.
			* \return If the pose is correctly added to the scene, return \c true otherwise, if the pose is
			* already in the scene, returns \c false.
			*/
			bool AddPose(FbxPose^ pose);

			/** Remove the specified pose from the scene.
			* \param pPose The pose to be removed from the scene.
			* \return If the pose was successfully removed from the scene, return \c true otherwise, if the
			* pose could not be found returns \c false.
			*/
			bool RemovePose(FbxPose^ pose);

			/** Remove the pose at the given index from the scene.
			* \param pIndex The zero-based index of the pose to be removed.
			* \return If the pose was successfully removed from the scene, return \c true otherwise, if the
			* pose could not be found returns \c false.
			*/
			bool RemovePose(int index);


			//@}
			/**
			* \name Scene information
			*/
			//@{

			/** Get the scene information.
			* \return Pointer to the scene information object.
			*/
			/** Set the scene information.
			* \param pSceneInfo Pointer to the scene information object.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxDocumentInfo,SceneInfo);						

			//@}

			/**
			* \name Global Settings
			*/
			//@{

			/** Access global settings.
			* \return Reference to the Global Settings.
			*/			
			REF_PROPERTY_GET_DECLARE(FbxGlobalSettings,GlobalSettings);

			/** Access global light settings.
			* \return Reference to the Global Light Settings.
			*/
			REF_PROPERTY_GET_DECLARE(FbxGlobalLightSettings,GlobalLightSettings);			
			/** Access global camera settings.
			* \return Reference to the Global Camera Settings.
			*/
			REF_PROPERTY_GET_DECLARE(FbxGlobalCameraSettings,GlobalCameraSettings);			

			/** Access global time settings.
			* \return Reference to the Global Time Settings.
			*/
			REF_PROPERTY_GET_DECLARE(FbxGlobalTimeSettings,GlobalTimeSettings);			

			/** Const access to global settings.
			* \return Const reference to the Global Settings.
			*/
			//const KFbxGlobalSettings& GetGlobalSettings() const;

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

			void ConnectMaterials();

			void BuildMaterialLayersDirectArray();
			// called to make sure that eINDEX is remapped to eINDEX_TO_DIRECT
			void ReindexMaterialConnections();

			/** Clear then fill a node array with all existing nodes included in the scene.
			* \param pNodeArray An array of node pointers.
			*/
			//void FillNodeArray(KArrayTemplate<KFbxNode*>& pNodeArray);

			/** Clear then fill a pose array with all existing pose included in the scene.
			* \param pPoseArray An array of video pointers.
			*/
			//void FillPoseArray(KArrayTemplate<KFbxPose*>& pPoseArray);

			// Clone
			CLONE_DECLARE();


			/**
			* \name Material Access
			*/
			//@{

			/** Get number of materials.
			* \return Number of materials in this scene.
			*/
			property int MaterialCount
			{
				int get();
			}

			/** Get the material at the given index.
			* \param pIndex Position in the list of materials.
			* \return Pointer to the material or \c NULL if the index is out of bounds.
			* \remarks pIndex must be between 0 and GetMaterialCount().
			*/
			FbxSurfaceMaterial^ GetMaterial (int index);

			/** Get the material by its name.
			* \param pName Name of the material.
			* \return Pointer to the material or \c NULL if not found.
			*/
			FbxSurfaceMaterial^ GetMaterial (String^ name);

			/** Add the material to this scene.
			* \param pMaterial Pointer to the material to be added.
			* \return true on successful addition.
			*/
			bool AddMaterial (FbxSurfaceMaterial^ material);

			/** Remove the material from this scene.
			* \param pMaterial Pointer to the material to be removed.
			* \return true on successful removal.
			*/
			bool RemoveMaterial (FbxSurfaceMaterial^ material);

			//@}

			/**
			* \name Texture Access
			*/
			//@{

			/** Get number of textures.
			* \return Number of textures in this scene.
			*/			
			property int TextureCount
			{
				int get();
			}

			/** Get the texture at the given index.
			* \param pIndex Position in the list of textures.
			* \return Pointer to the texture or \c NULL if the index is out of bounds.
			* \remarks pIndex must be between 0 and GetTextureCount().
			*/			
			FbxTexture^ GetTexture(int index);

			/** Get the texture by its name.
			* \param pName Name of the texture.
			* \return Pointer to the texture or \c NULL if not found.
			*/
			FbxTexture^ GetTexture(String^ name);

			/** Add the texture to this scene.
			* \param pTexture Pointer to the texture to be added.
			* \return true on successful addition.
			*/
			bool AddTexture (FbxTexture^ texture);

			/** Remove the texture from this scene.
			* \param pTexture Pointer to the texture to be removed.
			* \return true on successful removal.
			*/
			bool RemoveTexture(FbxTexture^ texture);

			//@}

			/**
			* \name Node Access
			*/
			//@{

			/** Get number of nodes.
			* \return Number of nodes in this scene.
			*/			
			property int NodeCount
			{
				int get();
			}

			/** Get the node at the given index.
			* \param pIndex Position in the list of nodes.
			* \return Pointer to the node or \c NULL if the index is out of bounds.
			* \remarks pIndex must be between 0 and GetNodeCount().
			*/
			FbxNode^ GetNode (int index);

			/** Add the node to this scene.
			* \param pNode Pointer to the node to be added.
			* \return true on successful addition.
			*/			
			bool AddNode (FbxNode^ node);

			/** Remove the node from this scene.
			* \param pNode Pointer to the node to be removed.
			* \return true on successful removal.
			*/
			bool RemoveNode (FbxNode^ node);

			/** Helper method for determining the number of nodes that have
			* curves on surface attributes in the scene. Since the curve-on-surface
			* nodes are connected to nurbs geometry and not any KFbxNodes in the
			* scene, they won't normally be picked up in a graph traversal.
			* \return The number of curve-on-surface nodes in the scene
			*/			
			property int CurveOnSurfaceCount
			{
				int get();
			}

			FbxNode^ FindNodeByName (FbxStringManaged^ name);

			//@}

			/**
			* \name Geometry Access
			*/
			//@{

			/** Get number of geometries.
			* \return Number of geometries in this scene.
			*/			
			property int GeometryCount
			{
				int get();
			}

			/** Get the geometry at the given index.
			* \param pIndex Position in the list of geometries.
			* \return Pointer to the geometry or \c NULL if the index is out of bounds.
			* \remarks pIndex must be between 0 and GetGeometryCount().
			*/
			FbxGeometry^ GetGeometry(int index);

			/** Add the geometry to this scene.
			* \param pGeometry Pointer to the geometry to be added.
			* \return true on successful addition.
			*/
			bool AddGeometry(FbxGeometry^ geometry);

			/** Remove the geometry from this scene.
			* \param pGeometry Pointer to the geometry to be removed.
			* \return true on successful removal.
			*/
			bool RemoveGeometry(FbxGeometry^ geometry);

			//@}

			/**
			* \name Video Access
			*/
			//@{

			/** Get number of videos.
			* \return Number of videos in this scene.
			*/			
			property int VideoCount
			{
				int get();
			}

			/** Get the video at the given index.
			* \param pIndex Position in the list of videos.
			* \return Pointer to the video or \c NULL if the index is out of bounds.
			* \remarks pIndex must be between 0 and GetVideoCount().
			*/
			FbxVideo^ GetVideo (int index);

			/** Add the video to this scene.
			* \param pVideo Pointer to the video to be added.
			* \return true on successful addition.
			*/
			bool AddVideo (FbxVideo^ video);

			/** Remove the video from this scene.
			* \param pVideo Pointer to the video to be removed.
			* \return true on successful removal.
			*/
			bool RemoveVideo(FbxVideo^ video);

			//@}

			property int TakeNodeContainerCount
			{
				int get();
			}
			FbxTakeNodeContainer^  GetTakeNodeContainer(int index);
			bool AddTakeNodeContainer(FbxTakeNodeContainer^ takeNodeContainer);
			bool RemoveTakeNodeContainer(FbxTakeNodeContainer^ takeNodeContainer);

			//KSet* AddTakeTimeWarpSet(char *pTakeName);
			//KSet* GetTakeTimeWarpSet(char *pTakeName);*/		

		public:
			void ConvertNurbsSurfaceToNurb();
			void ConvertMeshNormals();
			void ConvertNurbCurvesToNulls();

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}