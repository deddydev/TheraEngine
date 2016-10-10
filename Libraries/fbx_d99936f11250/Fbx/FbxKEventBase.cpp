#pragma once
#include "stdafx.h"
#include "FbxKEventBase.h"


{
	namespace FbxSDK
	{					
		void FbxKEventBase::CollectManagedMemory()
		{
		}
		int FbxKEventBase::Type::get()
		{
			return _Ref()->GetType();
		}				
	}
}