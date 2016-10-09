#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNodeAttribute.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxInteger1TypedProperty;
		/** \brief This node attribute contains methods for accessing the properties of a camera switcher.
		* \nosubgrouping
		*/
		public ref class FbxCameraSwitcher : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxCameraSwitcher);			
		internal:
			FbxCameraSwitcher(KFbxCameraSwitcher* instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxCameraSwitcher);
		protected:
			virtual void CollectManagedMemory() override;
		public:			
			/**
			* \name Properties
			*/
			//@{
			REF_PROPERTY_GET_DECLARE(FbxInteger1TypedProperty,CameraIndex);
			//@}

			//! Return the type of node attribute which is EAttributeType::eCAMERA_SWITCHER.
			//virtual AttributeType GetAttributeType() const;

			/**
			* \name Default Animation Values
			* These functions provides direct access to default animation values specific to a camera switcher. The default animation
			* values are found in the default take node of the associated node. These functions only work if the camera switcher has been
			* associated with a node.
			*
			* Camera indices start at 1. Out of range indices are clamped between 1 and the number of cameras in the scene. The index of a
			* camera refers to its order of appearance when searching the node tree depth first.
			*/
			//@{

			/** Get default camera index.
			* \return Camera index. The return value is an integer between 1 and the number
			* of cameras in the scene, or 0 if there are no default camera set in the camera switcher.
			*/
			/** Set default camera index.
			* \param pIndex Id of the camera to set as default. This parameter has an integer
			* scale from 1 to the number of cameras in the scene. Its default value is 1 if
			* there is at least one camera in the camera switcher, 0 if there are none.
			* No validation checks are made.
			*/
			property int DefaultCameraIndex
			{
				int get();
				void set(int value);
			}				

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

		public:

			// Clone
			CLONE_DECLARE();
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}