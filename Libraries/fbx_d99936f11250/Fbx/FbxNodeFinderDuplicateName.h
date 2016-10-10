#pragma once
#include "stdafx.h"
#include "FbxNodeFinder.h"
#include <kfbxplugins/kfbxnodefinderduplicatename.h>


{
	namespace FbxSDK
	{
		ref class FbxNode;
		ref class FbxSceneManaged;
		ref class FbxSurfaceMaterial;
		ref class FbxTexture;
		ref class FbxVideo;
		ref class FbxGenericNode;

		//! KFbxNodeFinderDuplicateName 
		public ref class FbxNodeFinderDuplicateName : FbxNodeFinder
		{
			REF_DECLARE(FbxNodeFinder,KFbxNodeFinderDuplicateName);
		internal:
			FbxNodeFinderDuplicateName(KFbxNodeFinderDuplicateName* instance) : FbxNodeFinder(instance)
			{
				_Free =false;
			}
		public:

			/** \enum EState
			* - \e eCHECK_NODE_NAME
			* - \e eCHECK_MATERIAL_NAME
			* - \e eCHECK_TEXTURE_NAME
			* - \e eCHECK_VIDEO_NAME
			* - \e eCHECK_GENERIC_NODE_NAME
			* - \e eSTATE_COUNT
			*/
			enum class State
			{
				CheckNodeName = KFbxNodeFinderDuplicateName::eCHECK_NODE_NAME,
				CheckMaterialName = KFbxNodeFinderDuplicateName::eCHECK_MATERIAL_NAME,
				CheckTextureName = KFbxNodeFinderDuplicateName::eCHECK_TEXTURE_NAME,
				CheckVideoName = KFbxNodeFinderDuplicateName::eCHECK_VIDEO_NAME,
				CheckGenericNode_name = KFbxNodeFinderDuplicateName::eCHECK_GENERIC_NODE_NAME,
				StateCount = KFbxNodeFinderDuplicateName::eSTATE_COUNT
			};

			/** Constructor. 
			*  When the destination scene is specified, duplicates are searched in both the destination scene and in the processed node tree.
			*  \param pDestinationScene     Destination scene to search. \c NULL by default.
			*/
			FbxNodeFinderDuplicateName(FbxSceneManaged^ destinationScene);
			FbxNodeFinderDuplicateName();

			/** GetState.
			*	\param pStateIndex     State index.
			*	\return                State of pStateIndex.
			*/
			bool GetState(int stateIndex);

			/** SetState.
			*	\param pStateIndex     State index.
			*	\param pValue          
			*/
			void SetState(int stateIndex, bool value);

			/** GetNodeArray.
			*	\return
			*/
			array<FbxNode^>^ GetNodeArray();

			/** GetNodeArray.
			*	\return
			*/
			array<FbxNode^>^  GetDuplicateNodeArray();

			/** GetMaterialArray.
			*	\return
			*/
			array<FbxSurfaceMaterial^>^ GetMaterialArray();

			/** GetMaterialArray.
			*	\return
			*/
			array<FbxSurfaceMaterial^>^ GetDuplicateMaterialArray();

			/** GetTextureArray.
			*	\return
			*/
			array<FbxTexture^>^ GetTextureArray();

			/** GetTextureArray.
			*	\return
			*/
			array<FbxTexture^>^ GetDuplicateTextureArray();

			/** GetVideoArray.
			*	\return
			*/			
			array<FbxVideo^>^ GetVideoArray();

			/** GetVideoArray.
			*	\return
			*/
			array<FbxVideo^>^ GetDuplicateVideoArray();


			array<FbxGenericNode^>^ GetGenericNodeArray();

			array<FbxGenericNode^>^ GetDuplicateGenericNodeArray();		
		};

	}
}