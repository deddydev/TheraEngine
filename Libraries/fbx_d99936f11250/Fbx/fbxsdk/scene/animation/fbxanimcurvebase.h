#ifndef _FBXSDK_SCENE_ANIMATION_CURVE_BASE_H_
#define _FBXSDK_SCENE_ANIMATION_CURVE_BASE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxIO
class FBXSDK_DLL FbxAnimCurveKeyBase
public:
    FbxTime mTime
    FbxAnimCurveKeyBase()
        mTime = FBXSDK_TIME_ZERO
    virtual ~FbxAnimCurveKeyBase() 
    virtual FbxTime GetTime() const
		return mTime
    virtual void  SetTime(const FbxTime& pTime) 
		mTime = pTime
class FBXSDK_DLL FbxAnimCurveBase : public FbxObject
    FBXSDK_ABSTRACT_OBJECT_DECLARE(FbxAnimCurveBase, FbxObject)
public:
        virtual void KeyClear () = 0
        virtual int KeyGetCount () const = 0
        virtual int KeyAdd (FbxTime pTime, FbxAnimCurveKeyBase& pKey, int* pLast = NULL) = 0
        virtual bool KeySet(int pIndex, FbxAnimCurveKeyBase& pKey) = 0
        virtual bool KeyRemove(int pIndex) = 0
        virtual bool KeyRemove(int pStartIndex, int pEndIndex) = 0
        virtual FbxTime KeyGetTime(int 
        virtual void KeySetTime(int pKeyIndex, FbxTime pTime) = 0
        enum EExtrapolationType
            eConstant = 1,
            eRepetition = 2,
            eMirrorRepetition = 3,
            eKeepSlope = 4,
            eRelativeRepetition = 5
        void SetPreExtrapolation(EExtrapolationType pExtrapolation)
        EExtrapolationType GetPreExtrapolation() const 
 return mPreExtrapolation
        void SetPreExtrapolationCount(unsigned long pCount)
        unsigned long GetPreExtrapolationCount() const 
 return mPreExtrapolationCount
        void SetPostExtrapolation(EExtrapolationType pExtrapolation)
        EExtrapolationType GetPostExtrapolation() const 
 return mPostExtrapolation
        void SetPostExtrapolationCount(unsigned long pCount)
        unsigned long GetPostExtrapolationCount() const 
 return mPostExtrapolationCount
          virtual float Evaluate (FbxTime pTime, int* pLast = NULL) = 0
        virtual float EvaluateIndex( double pIndex) = 0
        virtual bool GetTimeInterval(FbxTimeSpan& pTimeInterval)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual bool Store(FbxIO* pFileObject, bool pLegacyVersion=false) = 0
    virtual bool Retrieve(FbxIO* pFileObject) = 0
	virtual void ExtrapolationSyncCallback() = 0
protected:
	virtual void Construct(const FbxObject* pFrom)
private:
    EExtrapolationType mPreExtrapolation
    unsigned long      mPreExtrapolationCount
    EExtrapolationType mPostExtrapolation
    unsigned long      mPostExtrapolationCount
#endif 