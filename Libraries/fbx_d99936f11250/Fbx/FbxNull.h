#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"


{
	namespace FbxSDK
	{
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxDouble1TypedProperty;
		/** \brief This node attribute contains the properties of a null node.
		* \nosubgrouping
		*/
		public ref class FbxNull : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxNull);
		internal:
			FbxNull(KFbxNull* instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxNull);

		protected:
			virtual void CollectManagedMemory()override;
		public:
			//! Return the type of node attribute which is EAttributeType::eNULL.
			//virtual EAttributeType GetAttributeType() const;

			//! Reset to default values.
			void Reset();

			/**
			* \name Null Properties
			*/
			//@{

			/** \enum ELook Null look types.
			* - \e eNONE
			* - \e eCROSS
			*/
			enum class Look {
				None = KFbxNull::eNONE,
				Cross = KFbxNull::eCROSS,
			};

			/** Get a flag used to verify that the size has been set.
			* \return      \c true if SetSize() was called for this object.
			* \remarks     When an attribute is not set, the application can choose to ignore the attribute or use the default value.
			*              When Reset() is called, this flag is set to \c false.
			* \remarks     This function is OBSOLETE, DO NOT USE.  It will always return false.  It will be removed on in the next release.
			* \remarks     This function is deprecated. Use property Size instead.
			*/
			//K_DEPRECATED bool GetSizeIsSet() const;

			/** Get the default value for the size.
			* \return     Default size of this object (100).
			*/
			VALUE_PROPERTY_GET_DECLARE(double,SizeDefaultValue);

			//@}

			/**
			* \name Property Names
			*/
			//@{
			//static const char*          sSize;
			//static const char*          sLook;
			//@}

			/**
			* \name Property Default Values
			*/
			//@{
			//static const fbxDouble1     sDefaultSize;
			//static const ELook      sDefaultLook;
			//@}


			//////////////////////////////////////////////////////////////////////////
			//
			// Properties
			//
			//////////////////////////////////////////////////////////////////////////

			/** This property handles size.
			*
			* To access this property do: Size.Get().
			* To set this property do: Size.Set(fbxDouble1).
			*
			* Default value is 100.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,Size);

			/** This property handles the look of the null.
			*
			* To access this property do: Look.Get().
			* To set this property do: Look.Set(ELook).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(Look,LookType);			


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