#ifndef _FBXSDK_CORE_SYNC_CLOCK_H_
#define _FBXSDK_CORE_SYNC_CLOCK_H_
#ifndef FBXSDK_ENV_WINSTORE
FBXSDK_DLL void FbxSleep(int pMilliseconds);
FBXSDK_DLL FbxLongLong FbxGetHighResCounter();
FBXSDK_DLL FbxLongLong FbxGetHighResFrequency();
#endif
#endif