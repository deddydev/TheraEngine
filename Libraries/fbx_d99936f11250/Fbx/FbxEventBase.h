#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "kfbxevents/includes.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Events
		{
			/** FBX SDK event base class
			* \nosubgrouping
			*/
			public ref class FbxEventBase : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxEventBase,KFbxEventBase);
				INATIVEPOINTER_DECLARE(FbxEventBase,KFbxEventBase);			
			public:
				virtual VALUE_PROPERTY_GET_DECLARE(int,TypeId);

				/** Force events to give us a name
				* \return            event name 
				*/
				virtual VALUE_PROPERTY_GET_DECLARE(String^,EventName);			
			};

		}
	}
}