#ifndef _FBXSDK_CORE_CONNECTION_POINT_H_
#define _FBXSDK_CORE_CONNECTION_POINT_H_
class FBXSDK_DLL FbxConnection
{
public:
enum EType
{
eNone = 0,
eSystem = 1 << 0,
eUser = 1 << 1,
eSystemOrUser = eUser | eSystem,
eReference = 1 << 2,
eContains = 1 << 3,
eData = 1 << 4,
eLinkType = eReference | eContains | eData,
eDefault = eUser | eReference,
eUnidirectional = 1 << 7
};
};
class FbxConnectionPointFilter;
class FBXSDK_DLL FbxConnectionPoint
{
public:
enum EDirection
{
eDirSrc = 1 << 0,
eDirDst = 1 << 1,
eDirUni = 1 << 2,
eDirBoth = eDirSrc | eDirDst,
eDirMask = eDirSrc | eDirDst | eDirUni
};
enum EType
{
eStandard = 0,
eSubConnection = 1 << 3,
eTypeMask = eSubConnection
};
enum EAttribute
{
eDefault = 0,
eCache = 1 << 4,
eAttributeMask = eCache
};
enum EAllocFlag
{
eNotAllocated = 0,
eAllocated = 1 << 5,
eAllocFlagMask = eAllocated
};
enum ECleanedFlag
{
eNotCleaned = 0,
eCleaned = 1 << 6,
eCleanedFlagMask = eCleaned
};
enum EEvent
{
eSrcConnectRequest,
eDstConnectRequest,
eSrcConnect,
eDstConnect,
eSrcConnected,
eDstConnected,
eSrcDisconnect,
eDstDisconnect,
eSrcDisconnected,
eDstDisconnected,
eSrcReplaceBegin,
eSrcReplaceEnd,
eDstReplaceBegin,
eDstReplaceEnd,
eSrcReorder,
eSrcReordered
};
FbxConnectionPoint(void* pData=0);
virtual ~FbxConnectionPoint();
void SetFilter(FbxConnectionPointFilter* pConnectFilter, EType pType=eStandard);
void InternalClear();
void WipeConnectionList();
void Destroy();
void SubConnectRemoveAll();
inline FbxConnectionPoint*   GetSubOwnerConnect(){ return GetConnectType() == eSubConnection ? mOwner : NULL; }
inline FbxConnectionPointFilter* GetFilter(){ return mFilter; }
virtual bool  IsInReplace(FbxConnectionPoint* p1, FbxConnectionPoint* p2);
inline void   SetConnectType(EType pType){ mFlags = (mFlags & ~eTypeMask) | pType; }
inline EType  GetConnectType(){ return EType(mFlags & eTypeMask); }
inline void   SetDirection(int pDirections){ mFlags = (mFlags & ~eDirMask) | pDirections; }
inline EDirection GetDirection(){ return EDirection(mFlags & eDirMask); }
inline void   SetAttribute(int pAttributes){ mFlags = (mFlags & ~eAttributeMask) | pAttributes; }
inline EAttribute GetAttribute(){ return EAttribute(mFlags & eAttributeMask); }
inline void   SetAllocatedFlag(bool pBool){ mFlags = ( pBool ) ? mFlags | eAllocated : mFlags & ~eAllocFlagMask; }
inline bool   GetAllocatedFlag(){ return ( mFlags & eAllocFlagMask ) ? true : false; }
inline void   SetCleanedFlag(bool pBool){ mFlags = ( pBool ) ? mFlags | eCleaned : mFlags & ~eCleanedFlagMask; }
inline bool   GetCleanedFlag(){ return ( mFlags & eCleanedFlagMask ) ? true : false; }
bool    IsValidSrc(FbxConnectionPoint* pConnect);
bool    IsValidDst(FbxConnectionPoint* pConnect);
bool    IsValidSrcConnection(FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType);
bool    IsValidDstConnection(FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType);
bool    RequestValidSrcConnection(FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType );
bool    RequestValidDstConnection(FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType );
bool    ConnectSrc(FbxConnectionPoint* pSrc,FbxConnection::EType pConnectionType=FbxConnection::eNone);
bool    ConnectDst(FbxConnectionPoint* pDst,FbxConnection::EType pConnectionType=FbxConnection::eNone);
bool    ConnectSrcAt(int pDst_SrcIndex, FbxConnectionPoint* pSrc, FbxConnection::EType pConnectionType=FbxConnection::eNone);
bool    ConnectDstAt(int pSrc_DstIndex, FbxConnectionPoint* pDst, FbxConnection::EType pConnectionType=FbxConnection::eNone);
static bool   ConnectConnect(FbxConnectionPoint* pSrc,FbxConnectionPoint* pDst,FbxConnection::EType pConnectionType);
static bool   ConnectAt(FbxConnectionPoint* pSrc, int pSrc_DstIndex, FbxConnectionPoint* pDst, int pDst_SrcIndex, FbxConnection::EType pConnectionType);
bool    DisconnectDst(FbxConnectionPoint* pSrc);
bool    DisconnectSrc(FbxConnectionPoint* pSrc);
void    DisconnectAllSrc();
void    DisconnectAllDst();
static bool   DisconnectConnect(FbxConnectionPoint* pSrc,FbxConnectionPoint* pDst);
bool    DisconnectDstAt(int pIndex);
bool    DisconnectSrcAt(int pIndex);
bool    ReplaceInDst(FbxConnectionPoint* pDstOld, FbxConnectionPoint* pDstNew, int pIndexInNew);
bool    ReplaceInSrc(FbxConnectionPoint* pSrcOld, FbxConnectionPoint* pSrcNew, int pIndexInNew);
bool    ReplaceDstAt(int pIndex, FbxConnectionPoint* pDst);
bool    ReplaceSrcAt(int pIndex, FbxConnectionPoint* pSrc);
bool    SwapSrc(int pIndexA, int pIndexB);
bool MoveSrcAt(int pIndex, int pAtIndex);
bool MoveSrcAt(FbxConnectionPoint* pSrc, FbxConnectionPoint* pAtSrc);
bool IsConnectedSrc(FbxConnectionPoint*);
bool IsConnectedDst(FbxConnectionPoint*);
inline bool IsConnected(FbxConnectionPoint* pConnect) { return IsConnectedSrc(pConnect) || IsConnectedDst(pConnect); }
inline int     GetSrcCount() const { return mConnectionList.GetSrcCount(); }
inline FbxConnectionPoint* GetSrc(int pIndex) const { return mConnectionList.GetSrc(pIndex);}
inline FbxConnection::EType GetSrcType(int pIndex) const { return mConnectionList.GetSrcType(pIndex);}
inline int     GetDstCount() const { return mConnectionList.GetDstCount(); }
inline FbxConnectionPoint* GetDst(int pIndex) const { return mConnectionList.GetDst(pIndex);}
inline FbxConnection::EType GetDstType(int pIndex) const { return mConnectionList.GetDstType(pIndex);}
inline int     FindSrc(FbxConnectionPoint* pConnect){ return mConnectionList.FindSrc(pConnect); }
inline int     FindDst(FbxConnectionPoint* pConnect){ return mConnectionList.FindDst(pConnect); }
inline int     GetSrcCount(FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetSrcCount() : GetSrcCount(); }
inline FbxConnectionPoint* GetSrc(int pIndex,FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetSrc(pIndex) : GetSrc(pIndex); }
inline FbxConnection::EType GetSrcType(int pIndex,FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetSrcType(pIndex) : GetSrcType(pIndex); }
inline int     GetDstCount(FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetDstCount() : GetDstCount(); }
inline FbxConnectionPoint* GetDst(int pIndex,FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetDst(pIndex): GetDst(pIndex); }
inline FbxConnection::EType GetDstType(int pIndex,FbxConnectionPointFilter* pFilter){ return (pFilter) ? SubConnectGetOrCreate(pFilter)->GetDstType(pIndex) : GetDstType(pIndex); }
void* GetData(){ return mData; }
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
class ConnectionList
{
public:
ConnectionList();
~ConnectionList();
void     Clear();
void     InsertSrcAt(int pIndex, FbxConnectionPoint* pConnect, FbxConnection::EType pType);
void     AddSrc(FbxConnectionPoint* pConnect, FbxConnection::EType pType);
void     RemoveSrcAt(int pIndex);
int      FindSrc(FbxConnectionPoint* pConnect) const;
int      GetSrcCount() const;
FbxConnectionPoint*  GetSrc(int pIndex) const;
FbxConnection::EType GetSrcType(int pIndex) const;
void     InsertDstAt(int pIndex, FbxConnectionPoint* pConnect, FbxConnection::EType pType);
void     AddDst(FbxConnectionPoint* pConnect, FbxConnection::EType pType);
void     RemoveDstAt(int pIndex);
int      FindDst(FbxConnectionPoint* pConnect) const;
int      GetDstCount() const;
FbxConnectionPoint*  GetDst(int pIndex) const;
FbxConnection::EType GetDstType(int pIndex) const;
Connection(FbxConnectionPoint* pPoint, FbxConnection::EType pType) : mPoint(pPoint), mType(pType){}
};
};
inline void   InsertSrcAt(int pIndex, FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType){ mConnectionList.InsertSrcAt(pIndex, pConnect, pConnectionType); }
inline void   InsertDstAt(int pIndex, FbxConnectionPoint* pConnect, FbxConnection::EType pConnectionType){ mConnectionList.InsertDstAt(pIndex, pConnect, pConnectionType); }
inline void   RemoveSrcAt(int pIndex){ mConnectionList.RemoveSrcAt(pIndex); }
inline void   RemoveDstAt(int pIndex){ mConnectionList.RemoveDstAt(pIndex); }
inline FbxConnectionPoint* GetOwnerConnect(){ return mOwner;  }
#endif
};
virtual ~FbxConnectionPointFilter() {};
virtual FbxInt GetUniqueId() const { return 0; }
};
#endif