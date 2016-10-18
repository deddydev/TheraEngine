#ifndef _FBXSDK_CORE_EMITTER_H_
#define _FBXSDK_CORE_EMITTER_H_
class FbxListener;
class FBXSDK_DLL FbxEmitter
{
public:
void AddListener(FbxEventHandler& pHandler);
void RemoveListener(FbxEventHandler& pHandler);
template <typename EventType> void Emit(const EventType& pEvent) const
{
if( !mData ) return;
EventHandlerList::iterator itBegin = mData->mEventHandlerList.Begin();
EventHandlerList::iterator itEnd = mData->mEventHandlerList.End();
for( EventHandlerList::iterator it = itBegin; it != itEnd; ++it )
{
if ((*it).GetHandlerEventType() == pEvent.GetTypeId())
{
(*it).FunctionCall(pEvent);
}
}
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxEmitter();
~FbxEmitter();
typedef FbxIntrusiveList<FbxEventHandler, FbxEventHandler::eEmitter> EventHandlerList;
struct EventData { EventHandlerList mEventHandlerList; };
#endif
};
#endif