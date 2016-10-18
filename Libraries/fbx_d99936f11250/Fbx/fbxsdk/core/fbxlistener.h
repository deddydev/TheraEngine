#ifndef _FBXSDK_CORE_LISTENER_H_
#define _FBXSDK_CORE_LISTENER_H_
class FBXSDK_DLL FbxListener
{
public:
~FbxListener();
FbxListener(){}
template <typename EventType,typename ListenerType> FbxEventHandler* Bind(FbxEmitter& pEmitter, void (ListenerType::*pFunc)(const EventType*))
{
FbxMemberFuncEventHandler<EventType,ListenerType>* eventHandler =
FbxNew< FbxMemberFuncEventHandler<EventType,ListenerType> >(static_cast<ListenerType*>(this),pFunc);
pEmitter.AddListener(*eventHandler);
mEventHandler.PushBack(*eventHandler);
return eventHandler;
}
template <typename EventType,typename ListenerType> FbxEventHandler* Bind(FbxEmitter& pEmitter, void (ListenerType::*pFunc)(const EventType*)const)
{
FbxConstMemberFuncEventHandler<EventType,ListenerType>* eventHandler =
FbxNew< FbxConstMemberFuncEventHandler<EventType,ListenerType> >(static_cast<ListenerType*>(this),pFunc);
pEmitter.AddListener(*eventHandler);
mEventHandler.PushBack(*eventHandler);
return eventHandler;
}
template <typename EventType> FbxEventHandler* Bind(FbxEmitter& pEmitter, void (*pFunc)(const EventType*,FbxListener*))
{
FbxFuncEventHandler<EventType>* eventHandler =
FbxNew< FbxFuncEventHandler<EventType> >(this, pFunc);
pEmitter.AddListener(*eventHandler);
mEventHandler.PushBack(*eventHandler);
return eventHandler;
}
void Unbind(const FbxEventHandler* aBindId);
private:
typedef FbxIntrusiveList<FbxEventHandler, FbxEventHandler::eListener> EventHandlerList;
EventHandlerList mEventHandler;
};
#endif