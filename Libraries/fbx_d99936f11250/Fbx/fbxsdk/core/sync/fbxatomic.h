#ifndef _FBXSDK_CORE_SYNC_ATOMIC_H_
#define _FBXSDK_CORE_SYNC_ATOMIC_H_
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
class FBXSDK_DLL FbxAtomOp
{
public:
static void   Inc(volatile FbxAtomic* pPtr);
static void   Dec(volatile FbxAtomic* pPtr);
static bool   Add(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   Sub(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   And(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   Or(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   Nand(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   Xor(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static bool   CompareAndSwap(volatile FbxAtomic* pPtr, FbxAtomic pOld, FbxAtomic pSwap);
static FbxAtomic TestAndSet(volatile FbxAtomic* pPtr);
static FbxAtomic FetchAndSwap(volatile FbxAtomic* pPtr, FbxAtomic pSwap);
static FbxAtomic FetchAndInc(volatile FbxAtomic* pPtr);
static FbxAtomic FetchAndDec(volatile FbxAtomic* pPtr);
static FbxAtomic FetchAndAdd(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic FetchAndSub(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic FetchAndOr(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic FetchAndAnd(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic FetchAndXor(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic FetchAndNand(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic IncAndFetch(volatile FbxAtomic* pPtr);
static FbxAtomic DecAndFetch(volatile FbxAtomic* pPtr);
static FbxAtomic AddAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic SubAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic OrAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic AndAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic XorAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
static FbxAtomic NandAndFetch(volatile FbxAtomic* pPtr, FbxAtomic pVal);
};
#endif
#endif