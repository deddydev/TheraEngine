#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxObject.h"


{
	namespace FbxSDK
	{		
		ref class FbxBindingTableEntry;		
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		/** \brief A binding table represents a collection of bindings
		* from source types such as KFbxObjects, or KFbxLayerElements
		* to destinations which can be of similar types. See KFbxBindingTableEntry.
		* \nosubgrouping
		*/
		public ref class FbxBindingTableBase : FbxObjectManaged
		{
			REF_DECLARE(FbxEmitter,KFbxBindingTableBase);
		internal:
			FbxBindingTableBase(KFbxBindingTableBase* instance) : FbxObjectManaged(instance)
			{
				_Free = false;
			}
			//FBXOBJECT_DECLARE(FbxBindingTableBase);
		public:

			/** Adds a new entry to the binding table.
			* \return The new entry
			*/
			FbxBindingTableEntry^ AddNewEntry();

			/** Query the number of table entries.
			* \return The number of entries
			*/
			property size_t EntryCount
			{
				size_t get();
			}
			/** Access a table entry. 
			* \param pIndex Valid range is [0, GetEntryCount()-1]
			* \return A valid table entry if pIndex is valid. Otherwise the value is undefined.
			*/
			FbxBindingTableEntry^ GetEntry( size_t index );

			/** Access a table entry. 
			* \param pIndex Valid range is [0, GetEntryCount()-1]
			* \return A valid table entry if pIndex is valid. Otherwise the value is undefined.
			*/	
			//FbxBindingTableEntry& GetEntry( size_t pIndex );

			void CopyFrom(FbxBindingTableBase^ table);

			/** Retrieve the table entry  for the given source value.
			* \param pSrcName The source value to query
			* \return The corresponding entry, or NULL if no entry in 
			* the table has a source equal in value to pSrcName.
			*/
			FbxBindingTableEntry^ GetEntryForSource(System::String^ srcName);

			/** Retrieve the table entry for the given destination value.
			* \param pDestName The destination value to query
			* \return The corresponding entry, or NULL if no entry in 
			* the table has a destination equal in value to pDestName.
			*/			
			FbxBindingTableEntry^ GetEntryForDestination(System::String^ destName);
		};

	}
}