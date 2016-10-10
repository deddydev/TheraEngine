#pragma once
#include "stdafx.h"
#include "Fbx.h"

using namespace System::Runtime::InteropServices;


{
	namespace FbxSDK
	{
		ref class FbxVector4;
		/** \enum ELimitedProperty
		* - \e eTRANSLATION
		* - \e eROTATION
		* - \e eSCALE
		*/
		public enum class LimitedProperty
		{
			Translation = eTRANSLATION,
			Rotation = eROTATION,
			Scale = eSCALE
		};

		/** \brief KFbxLimits defines a 3 component min, max limit. 
		* KFbxLimits uses KFbxVector4 objects to store the values. Although the members are identified as
		* X, Y and Z (the W component is ignored) at this level, they are unitless values and will only 
		* have meaning within the context they are queried.
		* \nosubgrouping
		*/
		public ref class FbxLimits : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxLimits,KFbxLimits);
			INATIVEPOINTER_DECLARE(FbxLimits,KFbxLimits);

		public:
			//! Constructor.
			//KFbxLimits(KMBTransform *pMBTransform = NULL);
			//DEFAULT_CONSTRUCTOR(FbxLimits,KFbxLimits);			


			/** Set the active state of min limit.
			* \param pXActive     Set to \c true, to activate the X component min limit.
			* \param pYActive     Set to \c true, to activate the Y component min limit.
			* \param pZActive     Set to \c true, to activate the Z component min limit.
			*/
			void SetLimitMinActive(bool xActive,bool yActive, bool zActive);

			/** Get the active states of the three components of the min limit.
			* \param pXActive     \c true if the X component of the min limit is active.
			* \param pYActive     \c true if the Y component of the min limit is active.
			* \param pZActive     \c true if the Z component of the min limit is active.
			*/
			void GetLimitMinActive([OutAttribute]bool %xActive, [OutAttribute]bool %yActive, [OutAttribute]bool %zActive);

			/** Set the active state of max limit.
			* \param pXActive     Set to \c true, to activate the X component max limit.
			* \param pYActive     Set to \c true, to activate the Y component max limit.
			* \param pZActive     Set to \c true, to activate the Z component max limit.
			*/
			void SetLimitMaxActive(bool xActive, bool yActive, bool zActive);

			/** Get the active states of the three components of the max limit.
			* \param pXActive     \c true if the X component of the max limit is active.
			* \param pYActive     \c true if the Y component of the max limit is active.
			* \param pZActive     \c true if the Z component of the max limit is active.
			*/
			void GetLimitMaxActive([OutAttribute]bool %xActive, [OutAttribute]bool %yActive, [OutAttribute]bool %zActive);

			/** Check if at least one of the active flags is set.
			* \return     \c true if one of the six active flags is set.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,LimitSomethingActive);

			/** Get the min limit.
			* \return     The current X, Y and Z values for the min limit.
			*/
			/** Set the min limit.
			* \param pMin     The X, Y and Z values to be set for the min limit.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxVector4^,LimitMin);					
			
			/** Get the max limit.
			* \return     The current X, Y and Z values for the max limit.
			*/
			/** Set the max limit.
			* \param pMax    The X, Y and Z values to be set for the max limit.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxVector4^,LimitMax);						

			/** Get the property that is limited
			* \return     The current limited property
			*/
			/** Set the property that is limited
			* \param pProperty     The limited property
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(LimitedProperty,Limited_Property);

			void CopyFrom(FbxLimits^ limits);		
		};

		ref class FbxNode;

		/** The KFbxNodeLimits defines limits for transforms.
		* \nosubgrouping
		*/
		public ref class FbxNodeLimits : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxNodeLimits,KFbxNodeLimits);
			INATIVEPOINTER_DECLARE(FbxNodeLimits,KFbxNodeLimits);		

		public:
			/** Constructor.
			* \param pLimitedNode     Pointer to the node to which these limits apply.
			* \param pMBTransform
			*/
			//KFbxNodeLimits(KFbxNode *pLimitedNode,KMBTransform *pMBTransform);

			/** Get the limited node.
			* \return     Pointer to the node to which these limits apply. This node is the same pointer as 
			*             the one passed to the constructor.
			*/
			REF_PROPERTY_GET_DECLARE(FbxNode,LimitedNode);

			/**
			* \name Node Translation Limits
			*/
			//@{			

			/** Get the translation limit active flag.
			* \return     Translation limit active flag state.
			* \remarks    If this flag is \c false, the values in the mTranslationLimits are ignored.
			*/
			/** Change the translation limit active flag.
			* \param pActive     State of the translation limits active flag.
			* \remarks           If this flag is set to \c false, the values in the mTranslationLimits are ignored.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,TranslationLimitActive);

			//! The translation limits.
			REF_PROPERTY_GETSET_DECLARE(FbxLimits,TranslationLimits);

			//@}

			/**
			* \name Node Rotation Limits
			*/
			//@				

			/** Get the rotation limit active flag.
			* \return     Rotation limit active flag state. 
			* \remarks    If this flag is \c false, the values in the mRotationLimits are ignored.
			*/			
			/** Change the rotation limit active flag.
			* \param pActive     State of the rotation limits active flag.
			* \remarks           If this flag is set to \c false, the values in the mRotationLimits are ignored.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,RotationLimitActive);

			//! The rotation limits.
			REF_PROPERTY_GETSET_DECLARE(FbxLimits,RotationLimits);

			//@}

			/**
			* \name Node Scale Limits
			*/
			//@{

			/** Get the scaling limit active flag.
			* \return      Scaling limit active flag state.
			* \remarks     If this flag is \c false, the values in the mScalingLimits are ignored.
			*/			
			/** Change the scaling limit active flag.
			* \param pActive     State of the scaling limits active flag.
			* \remarks           If this flag is set to \c false, the values in the mScalingLimits are ignored.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ScalingLimitActive);

			//! The scaling limits.
			REF_PROPERTY_GETSET_DECLARE(FbxLimits,ScalingLimits);			
		};

	}
}