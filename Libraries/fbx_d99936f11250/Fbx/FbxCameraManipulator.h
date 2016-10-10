#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"



{
	namespace FbxSDK
	{
		ref class FbxCamera;
		ref class FbxSceneManaged;		
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** This class can be used to provide basic camera manipulation in any program using this library.
		* \nosubgrouping
		*/
		public ref class FbxCameraManipulator : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxCameraManipulator);
		internal:
			FbxCameraManipulator(KFbxCameraManipulator* instance):FbxObjectManaged(instance)
			{
				_Free = false;
			}
		protected:			
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxCameraManipulator);
		public:			
			/** Set the camera used for the manipulation.
			*	\param	pCamera				Camera that will be used for the manipulation.
			*	\param	pValidateLookAtPos	If TRUE, LookAt position will be aligned with the camera orientation. */
			void SetCamera(FbxCamera^ camera, bool validateLookAtPos);

			REF_PROPERTY_GET_DECLARE(FbxCamera,Camera);

			/** Set the manipulator up vector relative to the scene.
			*	\param	pUpVector	Vector defining the up direction of the scene. */
			void SetUpVector(FbxVector4^ upVector);

			/** Change camera position and look at to frame all objects.
			/*	\param	pScene	The scene containing the elements to frame. */
			void FrameAll(FbxSceneManaged^ scene);

			//** Change camera position and look at to frame all selected objects.
			/*	\param	pScene	The scene containing the elements to frame. */			
			void FrameSelected(FbxSceneManaged^ scene);

			//** Begin orbit manipulation around camera's look at.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			If TRUE, orbit manipulation successfully initialized. */
			bool OrbitBegin(int mouseX, int mouseY);

			//** Notify orbit manipulation of latest input.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			TRUE if orbit manipulation was previously initialized successfully. */			
			bool OrbitNotify(int mouseX, int mouseY);

			/** End orbit manipulation. */
			void OrbitEnd();

			/** Begin dolly manipulation.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			If TRUE, dolly manipulation successfully initialized. */			
			bool DollyBegin(int mouseX, int mouseY);

			/** Notify dolly manipulation of latest input.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			TRUE if dolly manipulation was previously initialized successfully. */			
			bool DollyNotify(int mouseX, int mouseY);

			/** End dolly manipulation. */
			void DollyEnd();

			/** Begin pan manipulation.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			If TRUE, pan manipulation successfully initialized. */			
			bool PanBegin(int mouseX, int mouseY);

			/** Notify pan manipulation of latest input.
			/*	\param	pMouseX	Horizontal position of the mouse cursor.
			/*	\param	pMouseY	Vertical position of the mouse cursor.
			/*	\return			TRUE if pan manipulation was previously initialized successfully. */
			bool PanNotify(int mouseX, int mouseY);

			/** End pan manipulation. */
			void PanEnd();

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			CLONE_DECLARE();

			//! Assignment operator.
			//KFbxCameraManipulator& operator=(KFbxCameraManipulator const& pCamManip);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
		};

	}
}