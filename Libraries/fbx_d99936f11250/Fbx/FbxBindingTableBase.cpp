#pragma once
#include "stdafx.h"
#include "FbxObject.h"
#include "FbxBindingTableBase.h"
#include "FbxBindingTableEntry.h"
#include "FbxString.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"

namespace Skill
{
	namespace FbxSDK
	{				
		//FBXOBJECT_DEFINITION(FbxBindingTableBase,KFbxBindingTableBase); 

		FbxBindingTableEntry^ FbxBindingTableBase::AddNewEntry()
		{
			return gcnew FbxBindingTableEntry(&_Ref()->AddNewEntry());
		}
		size_t FbxBindingTableBase::EntryCount::get()
		{
			return _Ref()->GetEntryCount();
		}			
		FbxBindingTableEntry^ FbxBindingTableBase::GetEntry( size_t index )
		{
			return gcnew FbxBindingTableEntry(&_Ref()->GetEntry(index));
		}
		void FbxBindingTableBase::CopyFrom(FbxBindingTableBase^ table)
		{
			*_Ref() = *table->_Ref();
		}			
		FbxBindingTableEntry^ FbxBindingTableBase::GetEntryForSource(System::String^ srcName)
		{
			STRINGTO_CONSTCHAR_ANSI(s,srcName);			
			FbxBindingTableEntry^ entry = gcnew FbxBindingTableEntry(_Ref()->GetEntryForSource(s));
			FREECHARPOINTER(s);
			return entry;
		}
		FbxBindingTableEntry^ FbxBindingTableBase::GetEntryForDestination(System::String^ destName)
		{
			STRINGTO_CONSTCHAR_ANSI(d,destName);			
			FbxBindingTableEntry^ entry = gcnew FbxBindingTableEntry(_Ref()->GetEntryForDestination(d));
			FREECHARPOINTER(d);
			return entry;
		}		

	}
}