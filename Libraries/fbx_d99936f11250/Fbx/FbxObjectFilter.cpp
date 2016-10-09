#pragma once
#include "stdafx.h"
#include "FbxObjectFilter.h"
#include "FbxObject.h"

namespace Skill
{
	namespace FbxSDK
	{	
		void FbxObjectFilter::CollectManagedMemory()
		{
		}
		bool FbxObjectFilter::Match(FbxObjectManaged^ objectPtr)
		{
			return _Ref()->Match(objectPtr->_Ref());
		}
		bool FbxObjectFilter::NotMatch(FbxObjectManaged^ objectPtr)
		{
			return _Ref()->NotMatch(objectPtr->_Ref());
		}

		/*FbxNameFilter::FbxNameFilter(String^ targetName) 
			: FbxObjectFilter(nullptr)
		{
			STRINGTO_CONSTCHAR_ANSI(t,targetName);
			_SetPointer(new KFbxNameFilter(t),true);
			FREECHARPOINTER(t);
		}*/
	}
}