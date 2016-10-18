#ifndef _FBXSDK_CORE_SYNC_H_
#define _FBXSDK_CORE_SYNC_H_
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
class FbxMutexImpl;
class FbxSemaphoreImpl;
class FbxGateImpl;
class FBXSDK_DLL FbxSpinLock
{
public:
FbxSpinLock();
void Acquire();
void Release();
};
};
};
};
inline Item(){ mNext = NULL; }
inline Item* Set(Item* pNext){ return mNext = pNext; }
inline Item* Next(){ return mNext; }
};
};
#endif
#endif