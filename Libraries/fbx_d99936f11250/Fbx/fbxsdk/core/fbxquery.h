#ifndef _FBXSDK_CORE_QUERY_H_
#define _FBXSDK_CORE_QUERY_H_
#define FBXSDK_QUERY_UNIQUE_ID 0x14000000
class FbxProperty;
class FBXSDK_DLL FbxQuery
{
public:
virtual FbxInt GetUniqueId() const { return FBXSDK_QUERY_UNIQUE_ID; }
virtual bool IsValid(const FbxProperty& pProperty) const;
virtual bool IsEqual(FbxQuery* pOtherQuery) const;
void Ref();
void Unref();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
FbxQuery();
virtual ~FbxQuery();
private:
class InternalFilter : public FbxConnectionPointFilter
{
public:
InternalFilter(FbxQuery* pQuery);
~InternalFilter();
public:
FbxConnectionPointFilter* Ref();
void      Unref();
FbxInt      GetUniqueId() const { return mQuery->GetUniqueId(); }
bool      IsValid(FbxConnectionPoint* pConnect) const;
bool      IsEqual(FbxConnectionPointFilter* pConnectFilter) const;
FbxQuery*     mQuery;
};
InternalFilter mFilter;
int    mRefCount;
FBXSDK_FRIEND_NEW();
friend class FbxProperty;
#endif
};
class FBXSDK_DLL FbxCriteria
{
public:
static FbxCriteria ObjectType(const FbxClassId& pClassId);
static FbxCriteria ObjectTypeStrict(const FbxClassId& pClassId);
static FbxCriteria IsProperty();
FbxCriteria operator&&(const FbxCriteria& pCriteria) const;
FbxCriteria operator||(const FbxCriteria& pCriteria) const;
FbxCriteria operator!() const;
FbxQuery* GetQuery() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxCriteria();
FbxCriteria(const FbxCriteria& pCriteria);
FbxCriteria(FbxQuery* pQuery);
~FbxCriteria();
FbxCriteria& operator=(const FbxCriteria& pCriteria);
#endif
};
}
};
#ifndef DOXYGEN_SHOULD_SKIP_THIS
enum EType {eAND, eOR};
virtual FbxInt GetUniqueId() const { return FBXSDK_QUERY_UNIQUE_ID+1; }
};
virtual FbxInt GetUniqueId() const{ return FBXSDK_QUERY_UNIQUE_ID+2; }
};
virtual FbxInt GetUniqueId() const{ return FBXSDK_QUERY_UNIQUE_ID+3; }
};
virtual FbxInt GetUniqueId() const{ return FBXSDK_QUERY_UNIQUE_ID+4; }
};
virtual FbxInt GetUniqueId() const{ return FBXSDK_QUERY_UNIQUE_ID+5; }
};
#endif
#endif