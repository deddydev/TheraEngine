#pragma once
#include "stdafx.h"
#include "FbxEventHandler.h"
#include "FbxEventBase.h"
#include "FbxListener.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Events
		{

			void FbxEventHandler::CollectManagedMemory()
			{
			}			
			int FbxEventHandler::GetHandlerEventType()
			{
				return _Ref()->GetHandlerEventType();
			}
			void FbxEventHandler::FunctionCall(Skill::FbxSDK::Events::FbxEventBase^ e)
			{
				_Ref()->FunctionCall(*e->_Ref());
			}
			FbxListener^ FbxEventHandler::GetListener()
			{
				return gcnew FbxListener(_Ref()->GetListener());					
			}
		}
	}
}