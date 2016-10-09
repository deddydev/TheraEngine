#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"

namespace Skill
{
	namespace FbxSDK
	{	

		/**	This node attribute contains the properties of a skeleton segment.
		* \nosubgrouping
		*/
		ref class FbxColor;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;

		public ref class FbxSkeleton : FbxNodeAttribute
		{
		internal:
			FbxSkeleton(KFbxSkeleton* instance):FbxNodeAttribute(instance)
			{
				_Free = false;
			}

			REF_DECLARE(FbxEmitter,KFbxSkeleton);
			FBXOBJECT_DECLARE(FbxSkeleton);

		public:
			//! Return the type of node attribute which is EAttributeType::eSKELETON.
			//virtual AttributeType GetAttributeType() ;

			//! Reset the skeleton to default values and type to \c eROOT.
			void Reset();

			/**
			* \name Skeleton Properties
			*/
			//@{

			/** \enum ESkeletonType Skeleton types.
			* - \e eROOT
			* - \e eLIMB
			* - \e eLIMB_NODE
			* - \e eEFFECTOR
			*
			* \remarks \e eEFFECTOR is synonymous to \e eROOT.
			* \remarks The \e eLIMB_NODE type is a bone defined uniquely by a transform and a size value while
			* \remarks the \e eLIMB type is a bone defined by a transform and a length.
			* 
			*/
			 enum class  SkeletonType
			{
				Root=KFbxSkeleton::eROOT,
				Limb=KFbxSkeleton::eLIMB , 
				LimbNode=KFbxSkeleton::eLIMB_NODE , 
				Effector=KFbxSkeleton::eEFFECTOR
			} ;    

			 /** Get the skeleton type.
			* \return Skeleton type identifier.
			*/
			/** Set the skeleton type.
			* \param pSkeletonType Skeleton type identifier.
			*/						
			VALUE_PROPERTY_GETSET_DECLARE(SkeletonType,Skeleton_Type);

			/** Get a flag to know if the skeleton type was set.
			* \return \c true if a call to SetSkeletonType() has been made.
			* \remarks When the attribute is not set, the application can choose to ignore the attribute or use the default value.
			* \remarks The flag is set back to \c false when Reset() is called.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,SkeletonTypeIsSet);

			/** Get the default value for the skeleton type.
			* \return \c eROOT
			*/
			VALUE_PROPERTY_GET_DECLARE(SkeletonType,SkeletonTypeDefaultValue);
//
//			/** Set limb length.
//			* \param pLength Length of the limb.
//			* \return \c true if skeleton type is \c eLIMB, \c false otherwise.
//			* \remarks Limb length is only set if skeleton type is \c eLIMB.
//			* \remarks This function is deprecated. Use property LimbLength.Set(pLength) instead.
//			*/
//			K_DEPRECATED bool SetLimbLength(double pLength);
//
//			/** Get limb length.
//			* \return limb length.
//			* \remarks Limb length is only valid if skeleton type is \c eLIMB.
//			* \remarks This function is deprecated. Use property LimbLength.Get() instead.
//			*/
//			K_DEPRECATED double GetLimbLength() const;
//
//			/** Get a flag to know if the limb length was set.
//			* \return \c true if a call to SetLimbLength() has been made.
//			* \remarks When the attribute is not set, the application can choose to ignore the attribute or use the default value.
//			* \remarks The flag is set back to \c false when Reset() is called.
//			*/
//			K_DEPRECATED bool GetLimbLengthIsSet() const;
//
			/** Get the default value for the limb length.
			* \return 1.0
			*/
			VALUE_PROPERTY_GET_DECLARE(double,LimbLengthDefaultValue);
//
//			/** Set skeleton limb node size.
//			* \param pSize Size of the limb node.
//			* \remarks This function is deprecated. Use property Size.Set(pSize) instead.
//			*/
//			K_DEPRECATED bool SetLimbNodeSize(double pSize);
//
//			/** Get skeleton limb node size.
//			* \return Limb node size value.
//			* \remarks This function is deprecated. Use property Size.Get() instead.
//			*/
//			K_DEPRECATED double GetLimbNodeSize() const;
//
//			/** Get a flag to know if the limb node size was set.
//			* \return \c true if a call to SetLimbNodeSize() has been made.
//			* \remarks When the attribute is not set, the application can choose to ignore the attribute or use the default value.
//			* \remarks The flag is set back to \c false when Reset() is called.
//			* \remarks     This function is OBSOLETE, DO NOT USE.  It will always return false.  It will be removed on in the next release.
//			* \remarks     This function is deprecated. Use property Size instead.
//			*/
//			K_DEPRECATED bool GetLimbNodeSizeIsSet() const;
//
			/** Get the default value for the limb node size.
			* \return 100.0
			*/
			VALUE_PROPERTY_GET_DECLARE(double,LimbNodeSizeDefaultValue);

			/** Set limb or limb node color.
			* \param pColor RGB values for the limb color.
			* \return \c true if skeleton type is \c eLIMB or \c eLIMB_NODE, \c false otherwise.
			* \remarks Limb or limb node color is only set if skeleton type is \c eLIMB or \c eLIMB_NODE.
			*/
			bool SetLimbNodeColor(FbxColor^ Color);

			/** Get limb or limb node color.
			* \return Currently set limb color.
			* \remarks Limb or limb node color is only valid if skeleton type is \c eLIMB or \c eLIMB_NODE.
			*/
			FbxColor^ GetLimbNodeColor();

			/** Get a flag to know if the limb node color was set.
			* \return \c true if a call to SetLimbNodeColor() has been made.
			* \remarks When the attribute is not set, the application can choose to ignore the attribute or use the default value.
			* \remarks The flag is set back to \c false when Reset() is called.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,LimbNodeColorIsSet);

			/** Get the default value for the limb node color.
			* \return R=0.8, G=0.8, B=0.8
			*/
			FbxColor^ GetLimbNodeColorDefaultValue() ;

			/**
			* \name Property Names
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(String^,Size);
			//static const char*			sSize;
			//VALUE_PROPERTY_GETSET_DECLARE(String^,LimbLength);
			//static const char*			sLimbLength;

			/**
			* \name Property Default Values
			*/
			//@{	
			//VALUE_PROPERTY_GETSET_DECLARE(fbxDouble1,DefaultSize);
			//static const fbxDouble1		sDefaultSize;
			//VALUE_PROPERTY_GETSET_DECLARE(fbxDouble1,DefaultLimbLength);
			//static const fbxDouble1		sDefaultLimbLength;
//
//
//			//////////////////////////////////////////////////////////////////////////
//			//
//			// Properties
//			//
//			//////////////////////////////////////////////////////////////////////////
//
//			/** This property handles the limb node size.
//			*
//			* To access this property do: Size.Get().
//			* To set this property do: Size.Set(fbxDouble1).
//			*
//			* Default value is 100.0
//			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,Size);
//
//			/** This property handles the skeleton limb length.
//			*
//			* To access this property do: LimbLength.Get().
//			* To set this property do: LimbLength.Set(fbxDouble1).
//			*
//			* Default value is 1.0
//			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,LimbLength);
//
//			///////////////////////////////////////////////////////////////////////////////
//			//
//			//  WARNING!
//			//
//			//	Anything beyond these lines may not be documented accurately and is 
//			// 	subject to change without notice.
//			//
//			///////////////////////////////////////////////////////////////////////////////
//
#ifndef DOXYGEN_SHOULD_SKIP_THIS

		public:

			CLONE_DECLARE();			
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}