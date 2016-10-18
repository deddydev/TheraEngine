#ifndef _FBXSDK_CORE_BASE_TIME_H_
#define _FBXSDK_CORE_BASE_TIME_H_
#define FBXSDK_TIME_INFINITE  FbxTime(FBXSDK_TC_INFINITY)
#define FBXSDK_TIME_MINUS_INFINITE FbxTime(FBXSDK_TC_MINFINITY)
#define FBXSDK_TIME_ZERO   FbxTime(FBXSDK_TC_ZERO)
#define FBXSDK_TIME_EPSILON   FbxTime(FBXSDK_TC_EPSILON)
#define FBXSDK_TIME_ONE_SECOND  FbxTime(FBXSDK_TC_SECOND)
#define FBXSDK_TIME_ONE_MINUTE  FbxTime(FBXSDK_TC_MINUTE)
#define FBXSDK_TIME_ONE_HOUR  FbxTime(FBXSDK_TC_HOUR)
#define FBXSDK_TIME_ASSERT_EPSILON 0.5
#define FBXSDK_TIME_FORWARD   1
#define FBXSDK_TIME_BACKWARD  -1
class FbxTimeModeObject;
class FBXSDK_DLL FbxTime
{
public:
FbxTime(const FbxLongLong pTime=0){ mTime = pTime; }
enum EMode
{
eDefaultMode,
eFrames120,
eFrames100,
eFrames60,
eFrames50,
eFrames48,
eFrames30,
eFrames30Drop,
eNTSCDropFrame,
eNTSCFullFrame,
ePAL,
eFrames24,
eFrames1000,
eFilmFullFrame,
eCustom,
eFrames96,
eFrames72,
eFrames59dot94,
eModesCount
};
enum EProtocol {eSMPTE, eFrameCount, eDefaultProtocol};
static void SetGlobalTimeMode(EMode pTimeMode, double pFrameRate=0.0);
static EMode GetGlobalTimeMode();
static void SetGlobalTimeProtocol(EProtocol pTimeProtocol);
static EProtocol GetGlobalTimeProtocol();
static double GetFrameRate(EMode pTimeMode);
static EMode ConvertFrameRateToTimeMode(double pFrameRate, double pPrecision=0.00000001);
inline void Set(FbxLongLong pTime){ mTime = pTime; }
inline FbxLongLong Get() const { return mTime; }
inline void SetMilliSeconds(FbxLongLong pMilliSeconds){ mTime = pMilliSeconds * FBXSDK_TC_MILLISECOND; }
inline FbxLongLong GetMilliSeconds() const { return mTime / FBXSDK_TC_MILLISECOND; }
void SetSecondDouble(double pTime);
double GetSecondDouble() const;
void SetTime(int pHour, int pMinute, int pSecond, int pFrame=0, int pField=0, EMode pTimeMode=eDefaultMode);
void SetTime(int pHour, int pMinute, int pSecond, int pFrame, int pField, int pResidual, EMode pTimeMode);
bool GetTime(int& pHour, int& pMinute, int& pSecond, int& pFrame, int& pField, int& pResidual, EMode pTimeMode=eDefaultMode) const;
FbxTime GetFramedTime(bool pRound=true) const;
void SetFrame(FbxLongLong pFrames, EMode pTimeMode=eDefaultMode);
void SetFramePrecise(FbxDouble pFrames, EMode pTimeMode=eDefaultMode);
int GetHourCount() const;
int GetMinuteCount() const;
int GetSecondCount() const;
FbxLongLong GetFrameCount(EMode pTimeMode=eDefaultMode) const;
FbxDouble GetFrameCountPrecise(EMode pTimeMode=eDefaultMode) const;
FbxLongLong GetFieldCount(EMode pTimeMode=eDefaultMode) const;
int GetResidual(EMode pTimeMode=eDefaultMode) const;
static bool IsDropFrame(EMode pTimeMode=eDefaultMode);
char GetFrameSeparator(EMode pTimeMode=eDefaultMode) const;
char* GetTimeString(char* pTimeString, const FbxUShort& pTimeStringSize, int pInfo=5, EMode pTimeMode=eDefaultMode, EProtocol pTimeFormat=eDefaultProtocol) const;
enum EElement {eHours, eMinutes, eSeconds, eFrames, eField, eResidual};
FbxString GetTimeString(EElement pStart=eHours, EElement pEnd=eResidual, EMode pTimeMode=eDefaultMode, EProtocol pTimeFormat=eDefaultProtocol) const;
bool SetTimeString(const char* pTime, EMode pTimeMode=eDefaultMode, EProtocol pTimeFormat=eDefaultProtocol);
inline bool operator==(const FbxTime& pTime) const { return mTime == pTime.mTime; }
inline bool operator!=(const FbxTime& pTime) const { return mTime != pTime.mTime; }
inline bool operator>=(const FbxTime& pTime) const { return mTime >= pTime.mTime; }
inline bool operator<=(const FbxTime& pTime) const { return mTime <= pTime.mTime; }
inline bool operator>(const FbxTime& pTime) const { return mTime > pTime.mTime; }
inline bool operator<(const FbxTime& pTime) const { return mTime < pTime.mTime; }
inline FbxTime& operator=(const FbxTime& pTime) { mTime = pTime.mTime; return *this; }
inline FbxTime& operator+=(const FbxTime& pTime) { mTime += pTime.mTime; return *this; }
inline FbxTime& operator-=(const FbxTime& pTime) { mTime -= pTime.mTime; return *this; }
FbxTime operator+(const FbxTime& pTime) const;
FbxTime operator-(const FbxTime& pTime) const;
FbxTime operator*(const int Mult) const;
FbxTime operator/(const FbxTime& pTime) const;
FbxTime operator*(const FbxTime& pTime) const;
static FbxLongLong GetOneFrameValue(EMode pTimeMode=eDefaultMode);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
enum EOldMode
{
eOLD_DEFAULT_MODE,
eOLD_CINEMA,
eOLD_PAL,
eOLD_FRAMES30,
eOLD_NTSC_DROP_FRAME,
eOLD_FRAMES50,
eOLD_FRAMES60,
eOLD_FRAMES100,
eOLD_FRAMES120,
eOLD_NTSC_FULL_FRAME,
eOLD_FRAMES30_DROP,
eOLD_FRAMES1000
};
private:
FbxLongLong     mTime;
static EMode    gsGlobalTimeMode;
static EProtocol   gsGlobalTimeProtocol;
static FbxTimeModeObject* gsTimeObject;
void InternalSetTime(int pHour, int pMinute, int pSecond, FbxLongLong pFrame, int pField, EMode pTimeMode);
friend FBXSDK_DLL FbxTime::EMode  FbxGetGlobalTimeMode();
friend FBXSDK_DLL FbxTimeModeObject* FbxGetGlobalTimeModeObject();
friend FBXSDK_DLL FbxTime::EProtocol FbxGetGlobalTimeFormat();
friend FBXSDK_DLL void     FbxSetGlobalTimeMode(FbxTime::EMode pTimeMode, double pFrameRate);
friend FBXSDK_DLL void     FbxSetGlobalTimeFormat(FbxTime::EProtocol pTimeFormat);
#endif
};
FBXSDK_DLL inline FbxTime FbxTimeSeconds(const FbxDouble& pTime=0.0)
{
FbxTime lTime;
lTime.SetSecondDouble(pTime);
return lTime;
}
class FBXSDK_DLL FbxTimeSpan
{
public:
FbxTimeSpan() {}
FbxTimeSpan(FbxTime pStart, FbxTime pStop){ mStart = pStart; mStop = pStop; }
inline void Set(FbxTime pStart, FbxTime pStop){ mStart = pStart; mStop = pStop; }
inline void SetStart(FbxTime pStart){ mStart = pStart; }
inline void SetStop(FbxTime pStop){ mStop = pStop; }
inline FbxTime GetStart() const { return mStart; }
inline FbxTime GetStop() const { return mStop; }
inline FbxTime GetDuration() const { if( mStop > mStart ) return mStop - mStart; else return mStart - mStop; }
inline FbxTime GetSignedDuration() const { return mStop - mStart; }
inline int GetDirection() const { if( mStop >= mStart ) return FBXSDK_TIME_FORWARD; else return FBXSDK_TIME_BACKWARD; }
bool IsInside(FbxTime pTime) const;
FbxTimeSpan Intersect(const FbxTimeSpan& pTime) const;
bool operator!=(const FbxTimeSpan& pTime) const;
bool operator==(const FbxTimeSpan& pTime) const;
void UnionAssignment(const FbxTimeSpan& pSpan, int pDirection=FBXSDK_TIME_FORWARD);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
FbxTime mStart;
FbxTime mStop;
#endif
};
class FBXSDK_DLL FbxLocalTime
{
public:
FbxLocalTime();
int mYear;
int mMonth;
int mDay;
int mHour;
int mMinute;
int mSecond;
int mMillisecond;
};
FBXSDK_DLL void     FbxGetCurrentLocalTime(FbxLocalTime& pLocalTime);
FBXSDK_DLL FbxTime::EMode  FbxGetGlobalTimeMode();
FBXSDK_DLL FbxTimeModeObject* FbxGetGlobalTimeModeObject();
FBXSDK_DLL FbxTime::EProtocol FbxGetGlobalTimeFormat();
FBXSDK_DLL void     FbxSetGlobalTimeMode(FbxTime::EMode pTimeMode, double pFrameRate=0.0);
FBXSDK_DLL void     FbxSetGlobalTimeFormat(FbxTime::EProtocol pTimeFormat);
FBXSDK_DLL FbxTime::EOldMode  FbxGetOldTimeModeCorrespondance(FbxTime::EMode pMode);
FBXSDK_DLL FbxTime::EMode  FbxGetTimeModeFromOldValue(FbxTime::EOldMode pOldMode);
FBXSDK_DLL FbxTime::EMode  FbxGetTimeModeFromFrameRate(char* pFrameRate);
FBXSDK_DLL void     FbxGetControlStringList(char* pControlString, FbxTime::EProtocol pTimeFormat);
FBXSDK_DLL const char*   FbxGetGlobalFrameRateString(FbxTime::EMode pTimeMode);
FBXSDK_DLL const char*   FbxGetGlobalTimeModeString(FbxTime::EMode pTimeMode);
FBXSDK_DLL double    FbxGetFrameRate(FbxTime::EMode pTimeMode);
FBXSDK_DLL FbxTime::EProtocol FbxSelectionToTimeFormat(int pSelection);
FBXSDK_DLL FbxTime::EMode  FbxSelectionToTimeMode(int pSelection);
FBXSDK_DLL int     FbxTimeToSelection(FbxTime::EMode pTimeMode=FbxTime::eDefaultMode, int pTimeFormat=FbxTime::eDefaultProtocol);
FBXSDK_DLL const char*   FbxGetTimeModeName(FbxTime::EMode pTimeMode);
FBXSDK_DLL int     FbxGetFrameRateStringListIndex(FbxTime::EMode pTimeMode);
FBXSDK_DLL bool     FbxIsValidCustomFrameRate(double pFramerate);
FBXSDK_DLL bool     FbxGetNearestCustomFrameRate(double pFramerate, double& pNearestRate);
#endif