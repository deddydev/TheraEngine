#ifndef _FBXSDK_CORE_QUERY_EVENT_H_
#define _FBXSDK_CORE_QUERY_EVENT_H_
template <typename QueryT> class FbxQueryEvent : public FbxEvent<FbxQueryEvent<QueryT> >
{
public:
explicit FbxQueryEvent(QueryT* pData):mData(pData){}
QueryT& GetData()const { return *mData; }
private:
mutable QueryT* mData;
private:
virtual const char* GetEventName() const { FBX_ASSERT(false); return ""; }
static const char* FbxEventName() { FBX_ASSERT(false); return ""; }
friend class FbxEvent< FbxQueryEvent<QueryT> >;
};
#endif