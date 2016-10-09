#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNativePointer.h"
#include "kfbxevents/kfbxemitter.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Events
		{
			ref class FbxEventHandler;
			/** Base class for types that can emit events.
			* Note that only Emit() is thread-safe.
			*/
			public ref class FbxEmitter : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxEmitter,KFbxEmitter);			
				DEFAULT_CONSTRUCTOR(FbxEmitter,KFbxEmitter);			
				INATIVEPOINTER_DECLARE(FbxEmitter,KFbxEmitter);
			public:
				void AddListener(FbxEventHandler^ Handler);
				void RemoveListener(FbxEventHandler^ Handler);

				//	////////////////////////////////////////////////////////
				//	/*template <typename EventType>
				//	void Emit(const EventType& pEvent) const
				//	{
				//		if ( mData )
				//		{
				//			kfbxmp::KFbxMutexHelper lLock( mData->mMutex );

				//			EventHandlerList::iterator itBegin = mData->mEventHandler.Begin();
				//			EventHandlerList::iterator itEnd = mData->mEventHandler.End();
				//			for (EventHandlerList::iterator it = itBegin; it!=itEnd; ++it)
				//			{
				//				if ((*it).GetHandlerEventType() == pEvent.GetTypeId())
				//					(*it).FunctionCall(pEvent);
				//			}
				//		}
				//	}*/			
			};
		}
	}
}