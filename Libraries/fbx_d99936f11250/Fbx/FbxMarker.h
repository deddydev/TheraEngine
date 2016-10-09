#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"
#include "FbxDouble3.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxColor;
		ref class FbxDouble3TypedProperty;
		ref class FbxProperty;
		/**	This node attribute contains the properties of a marker.
		* \nosubgrouping
		*/
		public ref class FbxMarker : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxMarker);
		internal:
			FbxMarker(KFbxMarker* instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxMarker);
		protected:
			virtual void CollectManagedMemory() override;
		public:
			//! Return the type of node attribute which is EAttributeType::eMARKER.
			//virtual EAttributeType GetAttributeType() const;

			//! Reset the marker to default values.
			//virtual void Reset() override;

			/** \enum EType Marker types.
			* - \e eSTANDARD
			* - \e eOPTICAL
			* - \e eFK_EFFECTOR
			* - \e eIK_EFFECTOR
			*/
			enum class MarkerType{ 
				Standard = KFbxMarker::eSTANDARD, 
				Optical = KFbxMarker::eOPTICAL, 
				FkEffector = KFbxMarker::eFK_EFFECTOR,
				IkEffector = KFbxMarker::eIK_EFFECTOR
			};

			/** Get marker type.
			* \return The type of the marker.
			*/
			/** Set marker type.
			* \param pType The type of marker.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(MarkerType,Type);				

			/** \enum ELook Marker look.
			* - \e eCUBE
			* - \e eHARD_CROSS
			* - \e eLIGHT_CROSS
			* - \e eSPHERE
			*/
			enum class Look{ 
				Cube = KFbxMarker::eCUBE, 
				HardCross = KFbxMarker::eHARD_CROSS, 
				LightCross = KFbxMarker::eLIGHT_CROSS, 
				Sphere = KFbxMarker::eSPHERE
			};

//			/** Set marker look.
//			* \param pLook The look of the marker.
//			* \remarks This function is deprecated. Use property SetLook.Set(pLook) instead.
//			*/
//			K_DEPRECATED void SetLook(ELook pLook);
//
//			/** Get marker look.
//			* \return The look of the marker.
//			* \remarks This function is deprecated. Use property SetLook.Get() instead.
//			*/
//			K_DEPRECATED ELook GetLook() const;
//
//			/** Set marker size.
//			* \param pSize The size of the marker.
//			* \remarks This function is deprecated. Use property Size.Set(pSize) instead.
//			*/
//			K_DEPRECATED void SetSize(double pSize);
//
//			/** Get marker size.
//			* \return The currently set marker size.
//			* \remarks This function is deprecated. Use property Size.Get() instead.
//			*/
//			K_DEPRECATED double GetSize() const;
//
//			/** Set whether a marker label is shown.
//			* \param pShowLabel If set to \c true the marker label is visible.
//			* \remarks This function is deprecated. Use property ShowLabel.Set(pShowLabel) instead.
//			*/
//			K_DEPRECATED void SetShowLabel(bool pShowLabel);
//
//			/** Get whether a marker label is shown.
//			* \return \c true if the marker label is visible.
//			* \remarks This function is deprecated. Use property ShowLabel.Get() instead.
//			*/
//			K_DEPRECATED bool GetShowLabel() const;
//
//			/** Set the IK pivot position.
//			* \param pIKPivot The translation in local coordinates.
//			* \remarks This function is deprecated. Use property IKPivot.Set(pIKPivot) instead.
//			*/
//			K_DEPRECATED void SetIKPivot(KFbxVector4& pIKPivot);
//
//			/**  Get the IK pivot position.
//			* \return The pivot position vector.
//			* \remarks This function is deprecated. Use property IKPivot.Get() instead.
//			*/
//			K_DEPRECATED KFbxVector4 GetIKPivot() const;

			/**
			* \name Default Animation Values
			* This set of functions provides direct access to default
			* animation values specific to a marker. The default animation 
			* values are found in the default take node of the associated node.
			* Hence, these functions only work if the marker has been associated
			* with a node.
			*/
			//@{

			/** Get default occlusion.
			* \return 0.0 if optical marker animation is valid by default, 1.0 if it is occluded by default.
			* \remarks This function only works if marker type is set to KFbxMarker::eOPTICAL.
			*/
			/** Set default occlusion.
			* \param pOcclusion 0.0 if optical marker animation is valid by default, 1.0 if it is occluded by default.
			* \remarks This function only works if marker type is set to KFbxMarker::eOPTICAL.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DefaultOcclusion);

			/** Get default IK reach translation.
			* \return A value between 0.0 and 100.0, 100.0 means complete IK reach.
			* \remarks This function only works if marker type is set to KFbxMarker::eIK_EFFECTOR.
			*/
			/** Set default IK reach translation.
			* \param pIKReachTranslation A value between 0.0 and 100.0, 100.0 means complete IK reach.
			* \remarks This function only works if marker type is set to KFbxMarker::eIK_EFFECTOR.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DefaultIKReachTranslation);									

			/** Get default IK reach rotation.
			* \return A value between 0.0 and 100.0, 100.0 means complete IK reach.
			* \remarks This function only works if marker type is set to KFbxMarker::eIK_EFFECTOR.
			*/
			/** Set default IK reach rotation.
			* \param pIKReachRotation A value between 0.0 and 100.0, 100.0 means complete IK reach.
			* \remarks This function only works if marker type is set to KFbxMarker::eIK_EFFECTOR.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DefaultIKReachRotation);					

			//@}

			/**
			* \name Obsolete functions
			*/
			//@{

			/** Get default color.
			* \return Input parameter filled with appropriate data.
			* \remarks Marker color can not be animated anymore.
			*/
			/** Set default color.
			* \remarks Marker color can not be animated anymore.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxColor,DefaultColor);			

			//@}

			/**
			* \name Property Names
			*/
			//static property String^ SLook {String^ get() {return gcnew String(KFbxMarker::sLook);}}
			//static property String^ SSize {String^ get() {return gcnew String(KFbxMarker::sSize);}}
			//static property String^ SShowLabel {String^ get() {return gcnew String(KFbxMarker::sShowLabel);}}
			//static property String^ SIKPivot {String^ get() {return gcnew String(KFbxMarker::sIKPivot);}}			

			/**
			* \name Property Default Values
			*/

			//static const Look	SDefaultLook = (Look)KFbxMarker::sDefaultLook;
			//static const double	SDefaultSize = KFbxMarker::sDefaultSize;
			//static const bool   SDefaultShowLabel = KFbxMarker::sDefaultShowLabel;
			//static const FbxDouble3^ SDefaultIKPivot = gcnew FbxDouble3(KFbxMarker::sDefaultIKPivot.mData[0],
			//	KFbxMarker::sDefaultIKPivot.mData[1],KFbxMarker::sDefaultIKPivot.mData[2]);

			//////////////////////////////////////////////////////////////////////////
			//
			// Properties
			//
			//////////////////////////////////////////////////////////////////////////

			/** This property handles the marker's look.
			*
			* To access this property do: Look.Get().
			* To set this property do: Look.Set(ELook).
			*
			* Default value is eCUBE
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxMarker::Look,MarkerLook);			

			/** This property handles the marker's size.
			*
			* To access this property do: Size.Get().
			* To set this property do: Size.Set(fbxDouble1).
			*
			* Default value is 100
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,Size);			

			/** This property handles the marker's label visibility.
			*
			* To access this property do: ShowLabel.Get().
			* To set this property do: ShowLabel.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowLabel);			

			/** This property handles the marker's pivot position.
			*
			* To access this property do: IKPivot.Get().
			* To set this property do: IKPivot.Set(fbxDouble3).
			*
			* Default value is (0., 0., 0.)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,IKPivot);

			// Dynamic properties

			/** This method grants access to the occlusion property.
			* \remark If the marker is not of type Optical or the property
			* is invalid, return NULL
			*/
			REF_PROPERTY_GET_DECLARE(FbxProperty,Occlusion);

			/** This method grants access to the IKReachTranslation property.
			* \remark If the marker is not of type IK Effector or the property
			* is invalid, return NULL
			*/			
			REF_PROPERTY_GET_DECLARE(FbxProperty,IKReachTranslation);
			/** This method grants access to the IKReachRotation property.
			* \remark If the marker is not of type IK Effector or the property
			* is invalid, return NULL
			*/
			REF_PROPERTY_GET_DECLARE(FbxProperty,IKReachRotation);			

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
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