#pragma once
#include "stdafx.h"
#include "FbxEventBase.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Events
		{			
			void FbxEventBase::CollectManagedMemory()
			{				
			}			
			int FbxEventBase::TypeId::get()
			{
				return _Ref()->GetTypeId();
			}				
			String^ FbxEventBase::EventName::get()
			{
				return gcnew System::String(_Ref()->GetEventName());
			}

		}
	}
}