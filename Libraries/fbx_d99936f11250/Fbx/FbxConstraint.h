#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxTakeNodeContainer.h"


namespace Skill
{
	namespace FbxSDK
	{		
		ref class FbxErrorManaged;
		ref class FbxVector4;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** Base class for constraints
		* \nosubgrouping
		*/
		public ref class FbxConstraint : FbxTakeNodeContainer
		{
			REF_DECLARE(FbxEmitter,KFbxConstraint);
		internal:
			FbxConstraint(KFbxConstraint* instance) : FbxTakeNodeContainer(instance)
			{
				_Free = false;
			}
		protected:
			virtual void CollectManagedMemory()override;
			FBXOBJECT_DECLARE(FbxConstraint);
		public:			


			/** \enum EConstraintType Constraint attribute types.
			*	 - \e eUNIDENTIFIED
			*	 - \e ePOSITION
			*	 - \e eROTATION
			*	 - \e eSCALE
			*	 - \e ePARENT
			*	 - \e eSINGLECHAIN_IK
			*	 - \e eAIM
			*	 - \e eCHARACTER_CONSTRAINT
			*	 - \e eCONSTRAINT_COUNT
			*/
			enum class ConstraintType
			{
				Unidentified = KFbxConstraint::eUNIDENTIFIED,
				Position = KFbxConstraint::ePOSITION ,
				Rotation = KFbxConstraint::eROTATION ,
				Scale = KFbxConstraint::eSCALE,
				Parent = KFbxConstraint::ePARENT,
				SinglechainIK = KFbxConstraint::eSINGLECHAIN_IK,
				Aim = KFbxConstraint::eAIM,
				CharacterConstraint = KFbxConstraint::eCHARACTER_CONSTRAINT,
				ConstraintCount = KFbxConstraint::eCONSTRAINT_COUNT
			};

			/** Return the type of node attribute.
			* \remarks     This class is pure virtual.
			*/
			virtual property ConstraintType Constraint_Type
			{
				ConstraintType get();
			}

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			* \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identifiers.
			* - \e eERROR
			* - \e eERROR_COUNT
			*/
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


			/** Set the offset.
			* \param pOffset     Offset vector value.
			*/
			virtual void SetOffset(FbxVector4^ offset);
			//
			//			///////////////////////////////////////////////////////////////////////////////
			//			//
			//			//  WARNING!
			//			//
			//			//  Anything beyond these lines may not be documented accurately and is
			//			//  subject to change without notice.
			//			//
			//			///////////////////////////////////////////////////////////////////////////////
		};

	}
}