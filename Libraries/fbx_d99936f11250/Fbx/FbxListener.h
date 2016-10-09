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
			/**FBX SDK listener class
			* \nosubgrouping
			*/
			public ref class FbxListener : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxListener,KFbxListener);
				INATIVEPOINTER_DECLARE(FbxListener,KFbxListener);
			public:

			//	////////////////////////////////////////////////////////////////////////////////////////
			//	/**Bind an  Event handler.
			//	* \param pEmitter          Event emitter.
			//	* \param pFunc             The call back function.
			//	* \return                  the event handler binded.
			//	*/

			//	template <typename EventType,typename ListenerType>
			//	KFbxEventHandler* Bind(KFbxEmitter& pEmitter, void (ListenerType::*pFunc)(const EventType*))

			//	{
			//		KFbxMemberFuncEventHandler<EventType,ListenerType>* eventHandler = 
			//			new KFbxMemberFuncEventHandler<EventType,ListenerType>(static_cast<ListenerType*>(this),pFunc);
			//		pEmitter.AddListener(*eventHandler);
			//		mEventHandler.PushBack(*eventHandler);
			//		return eventHandler;
			//	}

			//	////////////////////////////////////////////////////////////////////////////////////////
			//	/**Bind an  Event handler.
			//	* \param pEmitter          Event emitter.
			//	* \param pFunc             The call back function.
			//	* \return                  the event handler binded.
			//	*/
			//	template <typename EventType,typename ListenerType>
			//	KFbxEventHandler* Bind(KFbxEmitter& pEmitter, void (ListenerType::*pFunc)(const EventType*)const)
			//	{
			//		KFbxConstMemberFuncEventHandler<EventType,ListenerType>* eventHandler = 
			//			new KFbxConstMemberFuncEventHandler<EventType,ListenerType>(static_cast<ListenerType*>(this),pFunc);
			//		pEmitter.AddListener(*eventHandler);
			//		mEventHandler.PushBack(*eventHandler);
			//		return eventHandler;
			//	}

			//	////////////////////////////////////////////////////////////////////////////////////////
			//	/**Bind an  Event handler.
			//	* \param pEmitter          Event emitter.
			//	* \param pFunc             The call back function.
			//	* \return                  the event handler binded.
			//	*/
			//	template <typename EventType>
			//	KFbxEventHandler* Bind(KFbxEmitter& pEmitter, void (*pFunc)(const EventType*,KFbxListener*))
			//	{
			//		KFbxFuncEventHandler<EventType>* eventHandler = 
			//			new KFbxFuncEventHandler<EventType>(this, pFunc);
			//		pEmitter.AddListener(*eventHandler);
			//		mEventHandler.PushBack(*eventHandler);
			//		return eventHandler;
			//	}

			//	/**Unbind an event handler.
			//	* \param aBindId                 The event handler to unbind.
			//	*/
			//	void Unbind(const KFbxEventHandler* aBindId);

			//private:
			//	typedef KIntrusiveList<KFbxEventHandler, KFbxEventHandler::eNODE_LISTENER> EventHandlerList;
			//	EventHandlerList mEventHandler;
			};
		}
	}
}