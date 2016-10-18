#ifndef _FBXSDK_FILEIO_PROGRESS_H_
#define _FBXSDK_FILEIO_PROGRESS_H_
typedef bool (*FbxProgressCallback)(void* pArgs, float pPercentage, const char* pStatus);
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
class FbxSpinLock;
#endif
class FBXSDK_DLL FbxProgress
{
public:
void SetProgressCallback(FbxProgressCallback pCallback, void* pArgs=NULL);
void SetTotal(float pTotal);
void SetThreshold(float pThreshold);
void Update(float pDelta, const char* pStatus=NULL);
void Reset();
float GetProgress(FbxString* pStatus=NULL);
void Complete(const char* pStatus=NULL);
void Cancel();
inline bool IsCanceled() const { return mCanceled; }
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxProgress();
~FbxProgress();
private:
void Acquire();
void Release();
float GetPercent() const;
bool ExecuteCallback() const;
#if !defined(FBXSDK_ENV_WINSTORE) && !defined(FBXSDK_ENV_EMSCRIPTEN)
FbxSpinLock*  mLock;
#endif
float    mCurrent;
float    mPrevious;
float    mTotal;
float    mThreshold;
FbxString   mStatus;
FbxProgressCallback mCallback;
void*    mCallbackArgs;
bool    mCanceled;
#endif
};
#endif