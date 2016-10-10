#pragma once
#include "stdafx.h"
#include "FbxBindingTableEntry.h"
#include "FbxString.h"



{
	namespace FbxSDK
	{

		void FbxBindingTableEntry::CollectManagedMemory()
		{
		}

		FbxBindingTableEntry::FbxBindingTableEntry(FbxBindingTableEntry^ entry)
		{
			_SetPointer(new KFbxBindingTableEntry(*entry->_Ref()),true);
		}			

		String^ FbxBindingTableEntry::Source::get()
		{
			return gcnew System::String(_Ref()->GetSource());
		}
		void FbxBindingTableEntry::Source::set(System::String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetSource(v);
			FREECHARPOINTER(v);
		}			
		String^ FbxBindingTableEntry::Destination::get()
		{
			return gcnew System::String(_Ref()->GetDestination());
		}
		void FbxBindingTableEntry::Destination::set(System::String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetDestination(v);
			FREECHARPOINTER(v);
		}			
		void FbxBindingTableEntry::SetEntryType( System::String^ type, bool asSource )
		{
			STRINGTO_CONSTCHAR_ANSI(t,type);
			_Ref()->SetEntryType(t,asSource);
			FREECHARPOINTER(t);
		}
		System::String^ FbxBindingTableEntry::GetEntryType( bool asSource )
		{
			return gcnew System::String(_Ref()->GetEntryType(asSource));
		}

		IntPtr FbxBindingTableEntry::UserDataPtr::get()
		{
			return IntPtr(_Ref()->GetUserDataPtr());
		}
		void FbxBindingTableEntry::UserDataPtr::set(IntPtr value)
		{
			_Ref()->SetUserDataPtr(value.ToPointer());
		}

		void FbxBindingTableEntry::CopyFrom(FbxBindingTableEntry^ entry)
		{
			*this->_Ref() = *entry->_Ref();
		}		

	}
}