#ifndef _FBXSDK_CORE_SYNC_THREAD_H_
#define _FBXSDK_CORE_SYNC_THREAD_H_
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
class FbxThreadImpl;
typedef void (*FbxThreadProc)(void*);
class FBXSDK_DLL FbxThread
{
public:
enum EState {eUnknown, eRunning, eDead};
enum EPriority {eNone, eIdle, eLowest, eLow, eNormal, eHigh, eHighest, eRealTime};
FbxThread(FbxThreadProc pProc, void* pArg, bool pSuspend=false);
FbxThread(FbxThreadProc pProc, void* pArg, EPriority pPriority, bool pSuspend=false);
virtual ~FbxThread();
bool Suspend();
bool Resume();
bool Join();
bool Kill();
EPriority GetPriority();
bool SetPriority(EPriority pPriority);
EState GetState();
private:
FbxThreadImpl* mImpl;
};
#endif
#endif