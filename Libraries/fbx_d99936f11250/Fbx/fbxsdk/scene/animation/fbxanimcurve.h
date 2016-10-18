#ifndef _FBXSDK_SCENE_ANIMATION_CURVE_H_
#define _FBXSDK_SCENE_ANIMATION_CURVE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/animation/fbxanimcurvebase.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class KFCurve
class FBXSDK_DLL FbxAnimCurveKey_Impl
public:
    virtual ~FbxAnimCurveKey_Impl() 
    virtual FbxAnimCurveKey_Impl& operator=(const FbxAnimCurveKey_Impl& pFKey) = 0
	virtual void Set(FbxTime pTime, float pValue) = 0
	virtual void SetTCB(FbxTime pTime, float pValue, float pData0 = 0.0f, float pData1 = 0.0f, float pData2 = 0.0f) = 0
    virtual float GetValue() const = 0
	virtual void SetValue(float pValue) = 0
    virtual FbxAnimCurveDef::EInterpolationType GetInterpolation() const = 0
	virtual void SetInterpolation (FbxAnimCurveDef::EInterpolationType pInterpolation) = 0
    virtual FbxAnimCurveDef::ETangentMode GetTangentMode(bool pIncludeOverrides = false) const = 0
	virtual void SetTangentMode (FbxAnimCurveDef::ETangentMode pTangentMode) = 0
	virtual FbxAnimCurveDef::EWeightedMode GetTangentWeightMode() const = 0
	virtual void SetTangentWeightMode(FbxAnimCurveDef::EWeightedMode pTangentWeightMode, FbxAnimCurveDef::EWeightedMode pMask = FbxAnimCurveDef::eWeightedAll ) = 0
    virtual void SetTangentWeightAndAdjustTangent(FbxAnimCurveDef::EDataIndex pIndex, double pWeight ) = 0
	virtual FbxAnimCurveDef::EVelocityMode GetTangentVelocityMode() const = 0
	virtual void SetTangentVelocityMode(FbxAnimCurveDef::EVelocityMode pTangentVelocityMode, FbxAnimCurveDef::EVelocityMode pMask = FbxAnimCurveDef::eVelocityAll ) = 0
	virtual FbxAnimCurveDef::EConstantMode GetConstantMode() const = 0
	virtual void SetConstantMode(FbxAnimCurveDef::EConstantMode pMode) = 0
	virtual float GetDataFloat(FbxAnimCurveDef::EDataIndex pIndex) const = 0
	virtual void SetDataFloat(FbxAnimCurveDef::EDataIndex pIndex, float pValue) = 0
	virtual void	SetTangentVisibility (FbxAnimCurveDef::ETangentVisibility pVisibility) = 0
	virtual FbxAnimCurveDef::ETangentVisibility GetTangentVisibility () const = 0
	virtual void SetBreak(bool pVal) = 0
	virtual bool GetBreak() const = 0
class FBXSDK_DLL FbxAnimCurveKey : public FbxAnimCurveKeyBase
public:
	FbxAnimCurveKey() : FbxAnimCurveKeyBase()
		FBX_ASSERT(mAllocatorFct != NULL)
		mImpl = (*mAllocatorFct)()
    FbxAnimCurveKey(FbxTime pTime) : FbxAnimCurveKeyBase()
		FBX_ASSERT(mAllocatorFct != NULL)
		mImpl = (*mAllocatorFct)()
        SetTime(pTime)
    FbxAnimCurveKey(FbxTime pTime, float pVal) : FbxAnimCurveKeyBase()
		FBX_ASSERT(mAllocatorFct != NULL)
		mImpl = (*mAllocatorFct)()
        Set(pTime, pVal)
	FbxAnimCurveKey(FbxAnimCurveKey const& pFKey) : FbxAnimCurveKeyBase()
		FBX_ASSERT(mCopyAllocatorFct != NULL)
		SetTime(pFKey.GetTime())
		mImpl = mCopyAllocatorFct(pFKey.GetImpl())
	~FbxAnimCurveKey()
		FBX_ASSERT(mDeallocatorFct != NULL)
		(*mDeallocatorFct)(mImpl)
    FbxAnimCurveKey& operator=(const FbxAnimCurveKey& pFKey)
		FBX_ASSERT(mImpl)
		if (mImpl)
			*mImpl = *(pFKey.GetImpl())
		SetTime(pFKey.GetTime())
		return *this
    FbxTime GetTime() const
		return FbxAnimCurveKeyBase::GetTime()
    void SetTime(const FbxTime& pTime)
		FbxAnimCurveKeyBase::SetTime(pTime)
	void Set(FbxTime pTime, float pValue)
		FbxAnimCurveKeyBase::SetTime(pTime)
		mImpl->Set(pTime, pValue)
	void SetTCB(FbxTime pTime, float pValue, float pData0 = 0.0f, float pData1 = 0.0f, float pData2 = 0.0f)
		FbxAnimCurveKeyBase::SetTime(pTime)
		mImpl->SetTCB(pTime, pValue, pData0, pData1, pData2)
    float GetValue() const
		return mImpl->GetValue()
	void SetValue(float pValue)
		mImpl->SetValue(pValue)
    FbxAnimCurveDef::EInterpolationType GetInterpolation()
		return mImpl->GetInterpolation()
	void SetInterpolation (FbxAnimCurveDef::EInterpolationType pInterpolation)
		mImpl->SetInterpolation(pInterpolation)
    FbxAnimCurveDef::ETangentMode GetTangentMode(bool pIncludeOverrides = false)
		return mImpl->GetTangentMode(pIncludeOverrides)
	void SetTangentMode (FbxAnimCurveDef::ETangentMode pTangentMode)
		mImpl->SetTangentMode(pTangentMode)
	FbxAnimCurveDef::EWeightedMode GetTangentWeightMode() const
		return mImpl->GetTangentWeightMode()
	void SetTangentWeightMode(FbxAnimCurveDef::EWeightedMode pTangentWeightMode, FbxAnimCurveDef::EWeightedMode pMask = FbxAnimCurveDef::eWeightedAll )
		mImpl->SetTangentWeightMode(pTangentWeightMode, pMask)
    void SetTangentWeightAndAdjustTangent(FbxAnimCurveDef::EDataIndex pIndex, double pWeight )
        mImpl->SetTangentWeightAndAdjustTangent(pIndex, pWeight)
	FbxAnimCurveDef::EVelocityMode GetTangentVelocityMode() const
		return mImpl->GetTangentVelocityMode()
	void SetTangentVelocityMode(FbxAnimCurveDef::EVelocityMode pTangentVelocityMode, FbxAnimCurveDef::EVelocityMode pMask = FbxAnimCurveDef::eVelocityAll )
		mImpl->SetTangentVelocityMode(pTangentVelocityMode, pMask)
	FbxAnimCurveDef::EConstantMode GetConstantMode() const
		return mImpl->GetConstantMode()
	void SetConstantMode(FbxAnimCurveDef::EConstantMode pMode)
		mImpl->SetConstantMode(pMode)
	float GetDataFloat(FbxAnimCurveDef::EDataIndex pIndex) const
		return mImpl->GetDataFloat(pIndex)
	void SetDataFloat(FbxAnimCurveDef::EDataIndex pIndex, float pValue)
		mImpl->SetDataFloat(pIndex, pValue)
	void	SetTangentVisibility (FbxAnimCurveDef::ETangentVisibility pVisibility)
		mImpl->SetTangentVisibility(pVisibility)
	FbxAnimCurveDef::ETangentVisibility GetTangentVisibility () const
		return mImpl->GetTangentVisibility()
	void SetBreak(bool pVal)
		mImpl->SetBreak(pVal)
	bool GetBreak() const
		return mImpl->GetBreak()
	FbxAnimCurveKey_Impl* GetImpl() const
		return mImpl
	static void SetAllocatorFct(FbxAnimCurveKey_Impl* (*pAllocatorFct)())
	static void SetCopyAllocatorFct(FbxAnimCurveKey_Impl* (*pCopyAllocatorFct)(FbxAnimCurveKey_Impl*))
	static void SetDeallocatorFct(void (*pDeallocatorFct)(FbxAnimCurveKey_Impl*))
private:
	static FbxAnimCurveKey_Impl* (*mAllocatorFct)()
	static FbxAnimCurveKey_Impl* (*mCopyAllocatorFct)(FbxAnimCurveKey_Impl*)
	static void (*mDeallocatorFct)(FbxAnimCurveKey_Impl*)
	FbxAnimCurveKey_Impl* mImpl
class FbxScene
class FBXSDK_DLL FbxAnimCurve : public FbxAnimCurveBase
    FBXSDK_ABSTRACT_OBJECT_DECLARE(FbxAnimCurve, FbxAnimCurveBase)
public:
		static FbxAnimCurve* Create(FbxScene* pContainer, const char* pName)
		virtual void ResizeKeyBuffer(int pKeyCount) = 0
		virtual void KeyModifyBegin () = 0
		virtual void KeyModifyEnd () = 0
		virtual void KeyClear () = 0
		virtual int KeyGetCount () const = 0
		virtual int KeyAdd (FbxTime pTime, FbxAnimCurveKeyBase& pKey, int* pLast = NULL) = 0
		virtual int KeyAdd (FbxTime pTime, int* pLast = NULL) = 0
		virtual bool KeySet(int pIndex,  FbxAnimCurveKeyBase& pKey) = 0
		virtual bool KeyRemove(int pIndex) = 0
		virtual bool KeyRemove(int pStartIndex, int pEndIndex) = 0
		virtual int KeyInsert ( FbxTime pTime, int* pLast = NULL ) = 0
		virtual double KeyFind (FbxTime pTime, int* pLast = NULL) = 0
		virtual bool KeyScaleValue (float pMultValue) = 0
		virtual bool KeyScaleValueAndTangent (float pMultValue) = 0
		virtual void KeySet(int pKeyIndex,FbxTime pTime, float pValue, FbxAnimCurveDef::EInterpolationType pInterpolation = FbxAnimCurveDef::eInterpolationCubic, FbxAnimCurveDef::ETangentMode pTangentMode = FbxAnimCurveDef::eTangentAuto, float pData0 = 0.0,float pData1 = 0.0,FbxAnimCurveDef::EWeightedMode pTangentWeightMode = FbxAnimCurveDef::eWeightedNone, float pWeight0 = FbxAnimCurveDef::sDEFAULT_WEIGHT,float pWeight1 = FbxAnimCurveDef::sDEFAULT_WEIGHT,float pVelocity0 = FbxAnimCurveDef::sDEFAULT_VELOCITY,float pVelocity1 = FbxAnimCurveDef::sDEFAULT_VELOCITY) = 0
		virtual void KeySetTCB(int pKeyIndex,FbxTime pTime, float pValue, float pData0 = 0.0f, float pData1 = 0.0f, float pData2 = 0.0f) = 0
		virtual FbxAnimCurveDef::EInterpolationType KeyGetInterpolation(int pKeyIndex) const = 0
		virtual void KeySetInterpolation(int pKeyIndex, FbxAnimCurveDef::EInterpolationType pInterpolation) = 0
		virtual FbxAnimCurveDef::EConstantMode KeyGetConstantMode(int pKeyIndex) const = 0
		virtual FbxAnimCurveDef::ETangentMode KeyGetTangentMode(int pKeyIndex, bool pIncludeOverrides = false ) const = 0
		virtual void KeySetConstantMode(int pKeyIndex, FbxAnimCurveDef::EConstantMode pMode) = 0
		virtual void KeySetTangentMode(int pKeyIndex, FbxAnimCurveDef::ETangentMode pTangent) = 0
		virtual FbxAnimCurveKey KeyGet(int pIndex) const = 0
		virtual float KeyGetValue(int pKeyIndex) const = 0
		virtual void KeySetValue(int pKeyIndex, float pValue) = 0
		virtual void KeyIncValue(int pKeyIndex, float pValue) = 0
		virtual void KeyMultValue(int pKeyIndex, float pValue) = 0
		virtual void KeyMultTangent(int pKeyIndex, float pValue) = 0
		virtual FbxTime KeyGetTime(int pKeyIndex) const = 0
		virtual void KeySetTime(int pKeyIndex, FbxTime pTime) = 0
		virtual void KeySetBreak(int pKeyIndex, bool pVal) = 0
		virtual bool KeyGetBreak(int pKeyIndex) const = 0
		virtual float KeyGetLeftDerivative(int pIndex) = 0
		virtual void KeySetLeftDerivative(int pIndex, float pValue) = 0
		virtual float KeyGetLeftAuto(int pIndex, bool pApplyOvershootProtection = false) = 0
		virtual FbxAnimCurveTangentInfo KeyGetLeftDerivativeInfo(int pIndex) = 0
		virtual void KeySetLeftDerivativeInfo(int pIndex, const FbxAnimCurveTangentInfo& pValue, bool pForceDerivative = false) = 0
		virtual float KeyGetRightDerivative(int pIndex) = 0
		virtual void KeySetRightDerivative(int pIndex, float pValue) = 0
		virtual float KeyGetRightAuto(int pIndex, bool pApplyOvershootProtection = false) = 0
		virtual FbxAnimCurveTangentInfo KeyGetRightDerivativeInfo(int pIndex) = 0
		virtual void KeySetRightDerivativeInfo(int pIndex, const FbxAnimCurveTangentInfo& pValue, bool pForceDerivative = false) = 0
		virtual bool KeyIsLeftTangentWeighted(int pIndex) const = 0
		virtual bool KeyIsRightTangentWeighted(int pIndex) const = 0
		virtual float KeyGetLeftTangentWeight(int pIndex) const = 0
		virtual float KeyGetRightTangentWeight(int pIndex) const = 0
		virtual void   KeySetLeftTangentWeight( int pIndex, float pWeight, bool pAdjustTan = false ) = 0
		virtual void   KeySetRightTangentWeight( int pIndex, float pWeight, bool pAdjustTan = false  ) = 0
		virtual float KeyGetLeftTangentVelocity( int pIndex) const = 0
		virtual float KeyGetRightTangentVelocity( int pIndex) const = 0
		virtual float Evaluate (FbxTime pTime, int* pLast = NULL) = 0
		virtual float EvaluateIndex( double pIndex) = 0
		virtual float EvaluateLeftDerivative (FbxTime pTime, int* pLast = NULL) = 0
		virtual float EvaluateRightDerivative (FbxTime pTime, int* pLast = NULL) = 0
		virtual bool GetTimeInterval(FbxTimeSpan& pTimeInterval) = 0
		virtual void CopyFrom(FbxAnimCurve& pSource, bool pWithKeys = true) = 0
		virtual float GetValue(int pCurveNodeIndex=0) = 0
		virtual void SetValue(float pValue, int pCurveNodeIndex=0) = 0
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual KFCurve* GetKFCurve() = 0
	virtual bool Store(FbxIO* pFileObject, bool pLegacyVersion=false) = 0
    virtual bool Retrieve(FbxIO* pFileObject) = 0
	virtual void ExtrapolationSyncCallback() = 0
#endif 