#ifndef _FBXSDK_CORE_BASE_TIMECODE_H_
#define _FBXSDK_CORE_BASE_TIMECODE_H_
#define FBXSDK_TC_ZERO     FBXSDK_LONGLONG(0)
#define FBXSDK_TC_EPSILON    FBXSDK_LONGLONG(1)
#define FBXSDK_TC_MINFINITY    FBXSDK_LONGLONG(-0x7fffffffffffffff)
#define FBXSDK_TC_INFINITY    FBXSDK_LONGLONG(0x7fffffffffffffff)
#define FBXSDK_TC_FIX_DEN    FBXSDK_LONGLONG(100000000)
#define FBXSDK_TC_MILLISECOND   FBXSDK_LONGLONG(46186158)
#define FBXSDK_TC_SECOND    FbxLongLong(FBXSDK_TC_MILLISECOND*1000)
#define FBXSDK_TC_MINUTE    FbxLongLong(FBXSDK_TC_SECOND*60)
#define FBXSDK_TC_HOUR     FbxLongLong(FBXSDK_TC_MINUTE*60)
#define FBXSDK_TC_DAY     FbxLongLong(FBXSDK_TC_HOUR*24)
#define FBXSDK_TC_NTSC_FIELD   FbxLongLong(FBXSDK_TC_SECOND/30/2)
#define FBXSDK_TC_NTSC_FRAME   FbxLongLong(FBXSDK_TC_SECOND/30)
#define FBXSDK_TC_MNTSC_FIELD   FbxLongLong(FBXSDK_TC_MNTSC_FRAME/2)
#define FBXSDK_TC_MNTSC_FRAME   FbxLongLong(FBXSDK_TC_SECOND/30*1001/1000)
#define FBXSDK_TC_MNTSC_2_FRAMES  FbxLongLong(FBXSDK_TC_MNTSC_FRAME*2)
#define FBXSDK_TC_MNTSC_30_FRAMES  FbxLongLong(FBXSDK_TC_MNTSC_FRAME*30)
#define FBXSDK_TC_MNTSC_1798_FRAMES  FbxLongLong(FBXSDK_TC_MNTSC_FRAME*1798)
#define FBXSDK_TC_MNTSC_1800_FRAMES  FbxLongLong(FBXSDK_TC_MNTSC_FRAME*1800)
#define FBXSDK_TC_MNTSC_17982_FRAMES FbxLongLong(FBXSDK_TC_MNTSC_FRAME*17982)
#define FBXSDK_TC_MNTSC_107892_FRAMES FbxLongLong(FBXSDK_TC_MNTSC_FRAME*107892)
#define FBXSDK_TC_MNTSC_108000_FRAMES FbxLongLong(FBXSDK_TC_MNTSC_FRAME*108000)
#define FBXSDK_TC_MNTSC_1_SECOND  FbxLongLong(FBXSDK_TC_MNTSC_FRAME*30)
#define FBXSDK_TC_MNTSC_1_MINUTE  FbxLongLong(FBXSDK_TC_MNTSC_1_SECOND*60)
#define FBXSDK_TC_MNTSC_1_HOUR   FbxLongLong(FBXSDK_TC_MNTSC_1_SECOND*3600)
#define FBXSDK_TC_MNTSC_NUM    FbxULong(FBXSDK_TC_FIX_DEN*1000*30/1001)
#define FBXSDK_TC_MNTSC_DEN    FBXSDK_TC_FIX_DEN
#define FBXSDK_TC_PAL_FIELD    FbxLongLong(FBXSDK_TC_SECOND/25/2)
#define FBXSDK_TC_PAL_FRAME    FbxLongLong(FBXSDK_TC_SECOND/25)
#define FBXSDK_TC_FILM_FRAME   FbxLongLong(FBXSDK_TC_SECOND/24)
#define FBXSDK_TC_MFILM_FIELD   FbxLongLong(FBXSDK_TC_MFILM_FRAME/2)
#define FBXSDK_TC_MFILM_FRAME   FbxLongLong(FBXSDK_TC_SECOND/24*1001/1000)
#define FBXSDK_TC_MFILM_1_SECOND  FbxLongLong(FBXSDK_TC_MFILM_FRAME*24)
#define FBXSDK_TC_MFILM_1_MINUTE  FbxLongLong(FBXSDK_TC_MFILM_1_SECOND*60)
#define FBXSDK_TC_MFILM_1_HOUR   FbxLongLong(FBXSDK_TC_MFILM_1_SECOND*3600)
#define FBXSDK_TC_MFILM_NUM    FbxULong(FBXSDK_TC_FIX_DEN*1000*24/1001)
#define FBXSDK_TC_MFILM_DEN    FBXSDK_TC_FIX_DEN
#define FBXSDK_TC_REM(quot, num, den)  ((quot) = (num) / (den), (quot) * (den))
#define FBXSDK_TC_HOUR_REM(quot, num, den) ((quot) = ((num - (-FbxLongLong(num < 0) & (den - 1))) / (den)), (quot) * (den))
FBXSDK_DLL FbxLongLong FbxTCSeconds(FbxLongLong pTime);
FBXSDK_DLL FbxLongLong FbxTCMinutes(FbxLongLong pTime);
FBXSDK_DLL FbxLongLong FbxTCHours(FbxLongLong pTime);
FBXSDK_DLL FbxLongLong FbxTCSetRate(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, FbxLongLong pPeriod);
FBXSDK_DLL FbxLongLong FbxTCGetRate(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, FbxLongLong pPeriod);
FBXSDK_DLL FbxLongLong FbxTCSetNTSC(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetNTSC(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
FBXSDK_DLL FbxLongLong FbxTCSetMNTSCnd(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetMNTSCnd(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
FBXSDK_DLL FbxLongLong FbxTCSetMNTSC_2Xnd(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetMNTSC_2Xnd(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
FBXSDK_DLL FbxLongLong FbxTCSetMNTSC(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetMNTSC(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
FBXSDK_DLL FbxLongLong FbxTCSetPAL(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetPAL(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
FBXSDK_DLL FbxLongLong FbxTCSetFILM(int pHour, int pMinute, int pSecond, FbxLongLong pFrame);
FBXSDK_DLL FbxLongLong FbxTCGetFILM(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame);
FBXSDK_DLL FbxLongLong FbxTCSetFILMND(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField);
FBXSDK_DLL FbxLongLong FbxTCGetFILMND(FbxLongLong pTime, int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField);
#endif