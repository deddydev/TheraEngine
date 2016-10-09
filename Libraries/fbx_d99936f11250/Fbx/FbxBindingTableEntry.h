#pragma once
#include "stdafx.h"
#include "Fbx.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStringManaged;
		/** A binding table entry stores a binding between a source and a 
		* destination. Users should not instiantiate this class directly!
		* \nosubgrouping
		*/
		public ref class FbxBindingTableEntry : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxBindingTableEntry,KFbxBindingTableEntry);
			INATIVEPOINTER_DECLARE(FbxBindingTableEntry,KFbxBindingTableEntry);

		internal:
			FbxBindingTableEntry(KFbxBindingTableEntry e)
			{
				_SetPointer(new KFbxBindingTableEntry(e),true);
			}
			FbxBindingTableEntry(const KFbxBindingTableEntry* e)
			{
				_SetPointer(new KFbxBindingTableEntry(),true);
				*_FbxBindingTableEntry = *e;
			}

		public:
			/** 
			*\name Constructor and Destructor
			*/
			//@{
			//!Constructor.
			DEFAULT_CONSTRUCTOR(FbxBindingTableEntry,KFbxBindingTableEntry);
			//!Copy constructor.
			FbxBindingTableEntry(FbxBindingTableEntry^ entry);			

			/**
			* \name Access
			*/
			//@{

			//!Retrieve the source.
			/** Set the source. 
			* \param pSource             The source to set.
			*/
			property  System::String^ Source
			{
				System::String^ get();
				void set(System::String^ value);
			}			
			//
			/** Set the destination. 
			* \param pDestination             The destination to set.
			*/			
			//!Retrieve the destination.			
			property  System::String^ Destination
			{
				System::String^ get();
				void set(System::String^ value);
			}		

			/** Set the source type or destination type. 
			* \param pType             The source type or destination type to set.
			* \param pAsSource         Flag indicates soure type or destination type to set.
			*/
			void SetEntryType( System::String^ type, bool asSource );

			/** Get the source type or destination type. 
			* \param pAsSource         Flag indicates soure type or destination type to get.
			* \return                  Source type or destination type.
			*/
			System::String^ GetEntryType( bool asSource );


			//!Retrieve user data pointer.	  
			//!Set user data pointed.
			VALUE_PROPERTY_GETSET_DECLARE(IntPtr,UserDataPtr);
			//!Retrieve user data pointer.
			//const void* GetUserDataPtr() const;					

			//!Assignment operator.
			void CopyFrom(FbxBindingTableEntry^ entry);

		};		
	}
}