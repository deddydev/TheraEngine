#ifndef _FBXSDK_CORE_ARCH_ALLOC_H_
#define _FBXSDK_CORE_ARCH_ALLOC_H_
#if defined(_DEBUG) && defined(FBXSDK_ENV_WIN)
#endif
#if defined(FBXSDK_ENV_MAC)
#else
#endif
#if defined(FBXSDK_CPU_32) && !defined(FBXSDK_ENV_IOS)
#define FBXSDK_MEMORY_ALIGNMENT ((size_t)8U)
#else
#define FBXSDK_MEMORY_ALIGNMENT ((size_t)16U)
#endif
#define FBXSDK_MEMORY_COPY(dst, src, size) {memcpy(dst,src,size);}
typedef void* (*FbxMallocProc)(size_t);
typedef void* (*FbxCallocProc)(size_t, size_t);
typedef void* (*FbxReallocProc)(void*, size_t);
typedef void (*FbxFreeProc)(void*);
FBXSDK_DLL void FbxSetMallocHandler(FbxMallocProc pHandler);
FBXSDK_DLL void FbxSetCallocHandler(FbxCallocProc pHandler);
FBXSDK_DLL void FbxSetReallocHandler(FbxReallocProc pHandler);
FBXSDK_DLL void FbxSetFreeHandler(FbxFreeProc pHandler);
FBXSDK_DLL FbxMallocProc FbxGetMallocHandler();
FBXSDK_DLL FbxCallocProc FbxGetCallocHandler();
FBXSDK_DLL FbxReallocProc FbxGetReallocHandler();
FBXSDK_DLL FbxFreeProc FbxGetFreeHandler();
FBXSDK_DLL FbxMallocProc FbxGetDefaultMallocHandler();
FBXSDK_DLL FbxCallocProc FbxGetDefaultCallocHandler();
FBXSDK_DLL FbxReallocProc FbxGetDefaultReallocHandler();
FBXSDK_DLL FbxFreeProc FbxGetDefaultFreeHandler();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FBXSDK_DLL void* FbxMalloc(size_t pSize);
FBXSDK_DLL void* FbxCalloc(size_t pCount, size_t pSize);
FBXSDK_DLL void* FbxRealloc(void* pData, size_t pSize);
FBXSDK_DLL void FbxFree(void* pData);
FBXSDK_DLL char* FbxStrDup(const char* pString);
FBXSDK_DLL wchar_t* FbxStrDupWC(const wchar_t* pString);
FBXSDK_DLL void* FbxMallocDebug(size_t pSize, int pBlock, const char* pFile, int pLine);
FBXSDK_DLL void* FbxCallocDebug(size_t pCount, size_t pSize, int pBlock, const char* pFile, int pLine);
FBXSDK_DLL void* FbxReallocDebug(void* pData, size_t pSize, int pBlock, const char* pFile, int pLine);
FBXSDK_DLL void FbxFreeDebug(void* pData, int pBlock);
#if defined(FBXSDK_ALLOC_DEBUG)
#define FbxMalloc(s) FbxMallocDebug(s, _NORMAL_BLOCK, __FILE__, __LINE__)
#define FbxCalloc(c, s) FbxCallocDebug(c, s, _NORMAL_BLOCK, __FILE__, __LINE__)
#define FbxRealloc(p, s) FbxReallocDebug(p, s, _NORMAL_BLOCK, __FILE__, __LINE__)
#define FbxFree(p) FbxFreeDebug(p, _NORMAL_BLOCK)
#endif
#endif
template <class Type> class FbxDeletionPolicyDefault
{
public:
static inline void DeleteIt(Type** pPtr)
{
if( *pPtr )
{
delete *pPtr;
*pPtr = NULL;
}
}
};
template<typename T> void FbxDelete(T* p);
template<typename T> void FbxDelete(const T* p);
template <class Type> class FbxDeletionPolicyDelete
{
public:
static inline void DeleteIt(Type** mPtr)
{
if( *mPtr )
{
FbxDelete(*mPtr);
*mPtr = NULL;
}
}
};
template <class Type> class FbxDeletionPolicyFree
{
public:
static inline void DeleteIt(Type** pPtr)
{
if( *pPtr )
{
FbxFree(*pPtr);
*pPtr = NULL;
}
}
};
template <class Type> class FbxDeletionPolicyObject
{
public:
static inline void DeleteIt(Type** pPtr)
{
if( *pPtr )
{
(*pPtr)->Destroy();
*pPtr = NULL;
}
}
};
template<class Type, class Policy=FbxDeletionPolicyDefault<Type> > class FbxAutoPtr
{
public:
explicit FbxAutoPtr(Type* pPtr=0) : mPtr(pPtr){}
~FbxAutoPtr() { Policy::DeleteIt(&mPtr); }
inline Type* Get() const { return mPtr; }
inline Type* operator->() const { return mPtr; }
inline operator Type* () const { return mPtr; }
inline Type& operator*() const { return *mPtr; }
inline bool operator!() const { return mPtr == 0; }
inline operator bool () const { return mPtr != 0; }
inline void Reset(Type* pPtr=0)
{
FBX_ASSERT(pPtr == 0 || pPtr != mPtr);
FbxAutoPtr<Type, Policy>(pPtr).Swap(*this);
}
inline void Swap(FbxAutoPtr& pOther)
{
Type* TmpPtr = pOther.mPtr;
pOther.mPtr = mPtr;
mPtr = TmpPtr;
}
inline Type* Release()
{
Type* TmpPtr = mPtr;
mPtr = NULL;
return TmpPtr;
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
FbxAutoPtr(const FbxAutoPtr&);
FbxAutoPtr& operator=(const FbxAutoPtr&);
Type* mPtr;
#endif
};
template <class Type> class FbxAutoFreePtr : public FbxAutoPtr<Type, FbxDeletionPolicyFree<Type> >
{
public:
explicit FbxAutoFreePtr(Type* pPtr=0) : FbxAutoPtr<Type, FbxDeletionPolicyFree<Type> >(pPtr){}
};
template <class Type> class FbxAutoDeletePtr : public FbxAutoPtr<Type, FbxDeletionPolicyDelete<Type> >
{
public:
explicit FbxAutoDeletePtr(Type* pPtr=0) : FbxAutoPtr<Type, FbxDeletionPolicyDelete<Type> >(pPtr){}
};
template <class Type> class FbxAutoDestroyPtr : public FbxAutoPtr<Type, FbxDeletionPolicyObject<Type> >
{
public:
explicit FbxAutoDestroyPtr(Type* pPtr=0) : FbxAutoPtr<Type, FbxDeletionPolicyObject<Type> >(pPtr){}
};
class RefCount
{
public:
RefCount() { Init(); };
~RefCount() { Init(); };
void    Init()   { count = 0; }
void IncRef() { count++; }
int     DecRef() { count--; if (count < 0) count = 0; return count; }
private:
int  count;
};
template<class Type, class Policy=FbxDeletionPolicyDefault<Type> > class FbxSharedPtr
{
public:
FbxSharedPtr() :
mPtr(0),
mRef(0)
{}
explicit FbxSharedPtr(Type* pPtr) :
mPtr(pPtr),
mRef(0)
{
if (pPtr != 0)
{
mRef = (RefCount*)FbxMalloc(sizeof(RefCount));
mRef->Init();
mRef->IncRef();
}
}
FbxSharedPtr(const FbxSharedPtr& pSPtr) :
mPtr(pSPtr.mPtr),
mRef(pSPtr.mRef)
{
if (pSPtr.mPtr != 0 && mRef != 0)
mRef->IncRef();
}
FbxSharedPtr& operator=(const FbxSharedPtr& pSPtr)
{
if (this != &pSPtr)
{
Reset();
if (pSPtr.mPtr)
{
mPtr = pSPtr.mPtr;
mRef = pSPtr.mRef;
FBX_ASSERT(mRef != NULL);
mRef->IncRef();
}
}
return *this;
}
~FbxSharedPtr() { Destroy(); }
void Destroy() { Reset(); }
inline Type* Get() const { return mPtr; }
inline Type* operator->() const { return mPtr; }
inline operator Type* () const { return mPtr; }
inline Type& operator*() const { return *mPtr; }
inline bool operator!() const { return mPtr == 0; }
inline operator bool () const { return mPtr != 0; }
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
void Reset()
{
if (mRef)
{
FBX_ASSERT(mPtr != 0);
if (mRef->DecRef() == 0)
{
Policy::DeleteIt(&mPtr);
FbxFree(mRef);
mRef = NULL;
}
}
}
Type* mPtr;
RefCount* mRef;
#endif
};
template <class Type> class FbxSharedFreePtr : public FbxSharedPtr<Type, FbxDeletionPolicyFree<Type> >
{
public:
explicit FbxSharedFreePtr(Type* pPtr=0) : FbxSharedPtr<Type, FbxDeletionPolicyFree<Type> >(pPtr){}
};
template <class Type> class FbxSharedDeletePtr : public FbxSharedPtr<Type, FbxDeletionPolicyDelete<Type> >
{
public:
explicit FbxSharedDeletePtr(Type* pPtr=0) : FbxSharedPtr<Type, FbxDeletionPolicyDelete<Type> >(pPtr){}
};
template <class Type> class FbxSharedDestroyPtr : public FbxSharedPtr<Type, FbxDeletionPolicyObject<Type> >
{
public:
explicit FbxSharedDestroyPtr(Type* pPtr=0) : FbxSharedPtr<Type, FbxDeletionPolicyObject<Type> >(pPtr){}
};
#endif