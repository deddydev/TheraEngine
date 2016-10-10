#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "kfbxevents/includes.h"


{
	namespace FbxSDK
	{
		namespace Events
		{
			ref class FbxEventBase;
			ref class FbxListener;
			//-----------------------------------------------------------------
			public ref class FbxEventHandler : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxEventHandler,KFbxEventHandler);
				INATIVEPOINTER_DECLARE(FbxEventHandler,KFbxEventHandler);
			
			public:
			
				enum class NodeType
				{
					Listener = 0,
					Emitter,
					Count
				};			

				// Handler handles a certain type of event
				virtual int GetHandlerEventType();
				virtual void FunctionCall(Skill::FbxSDK::Events::FbxEventBase^ e);
				virtual FbxListener^ GetListener();

				//LISTNODE(KFbxEventHandler, eNODE_COUNT);
			};

		}
	}
}