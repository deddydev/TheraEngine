#pragma once
#include "stdafx.h"
#include "Fbx.h"


namespace FbxSDK
{

	public enum class FbxPropertyId
	{
		ProperyIdNull = -1,
		ProperyIdRoot = 0
	};

	public enum class FbxInheritType
	{
		Override=0,
		Inherit=1,
		Deleted=2
	};

	public enum class FbxConnectionType
	{ 
		ConnectionNone				= 0,

		// System or user
		ConnectionSystem			= 1 << 0,
		ConnectionUser				= 1 << 1,
		ConnectionSystemOrUser		= ConnectionUser | ConnectionSystem,

		// Type of Link
		ConnectionReference			= 1 << 2,
		ConnectionContains			= 1 << 3,
		ConnectionData				= 1 << 4,
		ConnectionLinkType			= ConnectionReference | ConnectionContains | ConnectionData,

		ConnectionDefault			= ConnectionUser | ConnectionReference,


		ConnectionUnidirectional    = 1 << 7
	};


	public ref class FbxPropertyFlags abstract
	{		
		public:
			enum class FbxPropertyFlagsType
			{
				NoFlag    	= 0,
				AnimaTable 	= 1, 
				User       	= 1<<1,
				Temporary  	= 1<<2,  // System property
				Published		= 1<<3, 
				PStatic		= 1<<4, 

				NotSavable	= 1<<5,
				Hidden     	= 1<<6,

				UIDisabled	= 1<<7,  // for dynamic UI
				UIGroup       = 1<<8,  // for dynamic UI
				UIBoolGroup   = 1<<9,  // for dynamic UI
				UIExpanded    = 1<<10, // for dynamic UI
				UINnCaption   = 1<<11, // for dynamic UI
				UIPanel     = 1<<12  // for dynamic UI
			};

			// VC6 Does not like static variables that are initialized in the header
			// and there is no kfbxpropertydef.cxx file.
			static property int FlagCount
			{
				int get();
			}

			static property FbxPropertyFlagsType AllFlags
			{
				FbxPropertyFlagsType get();
			}
	};

	/**************************************************************************
	* Filter management
	**************************************************************************/
	/**	\brief Class to manage ConnectFilter.
	* \nosubgrouping
	*/
	public ref class FbxConnectionPointFilter : IFbxNativePointer
	{
		BASIC_CLASS_DECLARE(FbxConnectionPointFilter,KFbxConnectionPointFilter);
		INATIVEPOINTER_DECLARE(FbxConnectionPointFilter,KFbxConnectionPointFilter);

		/**
		* \name Constructor and Destructor
		*/			
		public: 
			//! Constructor
			DEFAULT_CONSTRUCTOR(FbxConnectionPointFilter,KFbxConnectionPointFilter);

			//@}
		public:
			/**
			* \name ConnectFilter management
			*/
			//@{

			//! Return reference ConnectionPoint filter.
			virtual REF_PROPERTY_GET_DECLARE(FbxConnectionPointFilter,Reference);

			//! Cancel reference
			virtual void Unref();

			//! Get unique filter ID
			virtual VALUE_PROPERTY_GET_DECLARE(kFbxFilterId,UniqueId);

			//! Judge if the given Connection Point is valid
			//virtual bool IsValid(FbxConnectionPoint^	connect)
			//{
			//}
			////! Judge if the given Connection Point is a valid connection
			//virtual bool IsValidConnection	(KFbxConnectionPoint*	pConnect,kFbxConnectionType pType) const;
			////! Judge if it is equal with the given  ConnectionPoint filter. 
			//virtual bool							IsEqual				(KFbxConnectionPointFilter*	pConnectFilter)	const;

			//@}
	};
}