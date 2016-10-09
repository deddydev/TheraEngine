#pragma once
#include "stdafx.h"
#include "FbxName.h"
#include "FbxString.h"

namespace Skill
{
	namespace FbxSDK
	{		

		void FbxName::CollectManagedMemory()
		{
		}				
		FbxName::FbxName(String^ initialName)
		{
			STRINGTO_CONSTCHAR_ANSI(n,initialName);			
			_SetPointer(new KName(n),true);
			FREECHARPOINTER(n);			
		}

		FbxName::FbxName(FbxName^ name)
		{
			_SetPointer(new KName(*name->_Ref()),true);			
		}

		String^ FbxName::InitialName::get()
		{
			return gcnew String(_Ref()->GetInitialName());
		}
		void FbxName::InitialName::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetInitialName(v);
			FREECHARPOINTER(v);
		}
		

		String^ FbxName::CurrentName::get()
		{
			return gcnew String(_Ref()->GetCurrentName());
		}
		void FbxName::CurrentName::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetCurrentName(v);
			FREECHARPOINTER(v);
		}

		String^ FbxName::NameSpace::get()
		{
			return gcnew String(_Ref()->GetNameSpace());
		}
		void FbxName::NameSpace::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetNameSpace(v);
			FREECHARPOINTER(v);
		}

		bool FbxName::IsRenamed::get()
		{
			return _Ref()->IsRenamed();
		}			
		void FbxName::CopyFrom(FbxName^ name)
		{
			*this->_Ref() = *name->_Ref();
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS


	}	
}