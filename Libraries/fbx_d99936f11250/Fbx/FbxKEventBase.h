#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include <object/e/keventbase.h>


{
	namespace FbxSDK
	{		
		public ref class FbxKEventBase : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxKEventBase,KEventBase);
			INATIVEPOINTER_DECLARE(FbxKEventBase,KEventBase);		
		public:
			static const int EVENT_BASE = 0x200;
			static const int EVENT_NONE	= 0x000;
			static const int EVENT_VALUE_CHANGED = 0x206;

			DEFAULT_CONSTRUCTOR(FbxKEventBase,KEventBase);
			property int Type
			{
				int get();
			}

			//True if Event are not going to be treated next...
			/*property bool BreakEvent
			{
				bool get()
				{
					return e->GetBreakEvent();
				}
				void set(bool value)
				{
					if(value)
						e->SetBreakEvent();
				}
			}	*/		

		};		
	}
}