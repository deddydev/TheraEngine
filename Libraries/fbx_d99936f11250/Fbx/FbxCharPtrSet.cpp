#pragma once
#include "stdafx.h"
#include "FbxCharPtrSet.h"
#include "FbxString.h"


{
	namespace FbxSDK
	{				
		void FbxCharPtrSet::CollectManagedMemory()
		{
		}		
		FbxCharPtrSet::FbxCharPtrSet( int itemPerBlock)
		{
			_SetPointer(new KCharPtrSet(itemPerBlock),true);
		}

		bool FbxCharPtrSet::Remove (String^ reference)
		{
			STRINGTO_CONSTCHAR_ANSI(r,reference);			
			bool b = _Ref()->Remove(r);
			FREECHARPOINTER(r);
			return b;
		}
		void FbxCharPtrSet::RemoveFromIndex(int index)
		{
			_Ref()->RemoveFromIndex(index);
		}
		int	FbxCharPtrSet::Count::get()
		{
			return _Ref()->GetCount();
		}
		void FbxCharPtrSet::Sort()
		{
			_Ref()->Sort();
		}
		void FbxCharPtrSet::Clear()
		{
			_Ref()->Clear();
		}		

	}
}