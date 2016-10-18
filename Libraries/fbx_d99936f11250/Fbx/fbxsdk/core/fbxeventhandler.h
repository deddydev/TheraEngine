#ifndef _FBXSDK_CORE_EVENT_HANDLER_H_
#define _FBXSDK_CORE_EVENT_HANDLER_H_
class FbxListener;
class FbxEventHandler
{
public:
enum EType
{
eListener,
eEmitter,
eCount
};
virtual int GetHandlerEventType()=0;
virtual void FunctionCall(const FbxEventBase& pEvent)=0;
virtual FbxListener* GetListener()=0;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxEventHandler(){}
virtual ~FbxEventHandler(){}
FBXSDK_INTRUSIVE_LIST_NODE(FbxEventHandler, eCount);
#endif
};
#ifndef DOXYGEN_SHOULD_SKIP_THIS
template <typename EventType, typename ListenerType> class FbxMemberFuncEventHandler : public FbxEventHandler
{
typedef void (ListenerType::*CallbackFnc)(const EventType*);
public:
FbxMemberFuncEventHandler(ListenerType* pListenerInstance, CallbackFnc pFunction) : mListener(pListenerInstance), mFunction(pFunction){}
virtual int GetHandlerEventType(){ return EventType::GetStaticTypeId(); }
virtual void FunctionCall(const FbxEventBase& pEvent){ (*mListener.*mFunction)(reinterpret_cast<const EventType*>(&pEvent)); }
virtual FbxListener* GetListener(){ return mListener; }
private:
ListenerType* mListener;
CallbackFnc  mFunction;
};
template <typename EventType, typename ListenerType> class FbxConstMemberFuncEventHandler : public FbxEventHandler
{
typedef void (ListenerType::*CallbackFnc)(const EventType*) const;
public:
FbxConstMemberFuncEventHandler(ListenerType* pListenerInstance, CallbackFnc pFunction) : mListener(pListenerInstance), mFunction(pFunction){}
virtual int GetHandlerEventType(){ return EventType::GetStaticTypeId(); }
virtual void FunctionCall(const FbxEventBase& pEvent){ (*mListener.*mFunction)(reinterpret_cast<const EventType*>(&pEvent)); }
virtual FbxListener* GetListener(){ return mListener; }
private:
ListenerType* mListener;
CallbackFnc  mFunction;
};
template <typename EventType> class FbxFuncEventHandler : public FbxEventHandler
{
typedef void (*CallbackFnc)(const EventType*, FbxListener*);
public:
FbxFuncEventHandler(FbxListener* pListener, CallbackFnc pFunction) : mListener(pListener), mFunction(pFunction){}
virtual int GetHandlerEventType(){ return EventType::GetStaticTypeId(); }
virtual void FunctionCall(const FbxEventBase& pEvent){ (*mFunction)(reinterpret_cast<const EventType*>(&pEvent), mListener); }
virtual FbxListener* GetListener(){ return mListener; }
private:
FbxListener* mListener;
CallbackFnc  mFunction;
};
#endif
#endif