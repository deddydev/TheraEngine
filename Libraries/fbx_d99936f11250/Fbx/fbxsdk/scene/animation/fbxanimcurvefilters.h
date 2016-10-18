#ifndef _FBXSDK_SCENE_ANIMATION_CURVE_FILTERS_H_
#define _FBXSDK_SCENE_ANIMATION_CURVE_FILTERS_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxtime.h>
#include <fbxsdk/core/base/fbxstatus.h>
#include <fbxsdk/scene/animation/fbxanimcurve.h>
#include <fbxsdk/fileio/fbxiosettings.h>
#include <fbxsdk/core/math/fbxtransforms.h>		
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxObject
class FbxAnimStack
class FbxRotationOrder
class FBXSDK_DLL FbxAnimCurveFilter
public:
    FbxAnimCurveFilter()
    virtual ~FbxAnimCurveFilter() 
    virtual const char* GetName() const 
return NULL
    FbxTime& GetStartTime() 
return mStart
    void SetStartTime(FbxTime& pTime) 
 mStart = pTime
    FbxTime& GetStopTime() 
return mStop
    void SetStopTime(FbxTime& pTime) 
 mStop = pTime
    int GetStartKey(FbxAnimCurve& pCurve) const
    int GetStopKey(FbxAnimCurve& pCurve) const
    virtual bool NeedApply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus = NULL) = 0
    virtual void Reset() 
        mStart= FBXSDK_TIME_MINUS_INFINITE
        mStop = FBXSDK_TIME_INFINITE
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	static bool GetContinuousOffset(FbxRotationOrder& pOrder, FbxVector4& pOffset, FbxVector4& pNew, FbxVector4& pOld)
protected:
    void GetKFCurvesFromAnimCurve(FbxAnimCurve** pSrc, int pSrcCount, KFCurve** pDst, int& pDstCount)
    virtual void UpdateProgressInformation(FbxTime 
class FBXSDK_DLL FbxAnimCurveFilterConstantKeyReducer : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterConstantKeyReducer()
    virtual ~FbxAnimCurveFilterConstantKeyReducer() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)  
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)          
 return FbxAnimCurveFilter::Apply(pCurve, pCount, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
    double GetDerivativeTolerance() const
    void SetDerivativeTolerance(double pValue)
    double GetValueTolerance() const
    void SetValueTolerance(double pValue)
    bool GetKeepFirstAndLastKeys() const
    void SetKeepFirstAndLastKeys( bool pKeepFirstAndLastKeys )
    bool GetKeepOneKey() const
    void SetKeepOneKey( bool pKeepOneKey )
	void SetKeepNotPureAutoKeys(bool pKeep)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    void SetTranslationThreshold    ( double pTranslationThreshold )
    void SetRotationThreshold       ( double pRotationThreshold )
    void SetScalingThreshold        ( double pScalingThreshold )
    void SetDefaultThreshold        ( double pDefaultThreshold )
    void SetModes(bool pExporting, FbxIOSettings& pIOS)
private:
    double  mDerTol
    double  mValTol
    double  mTranslationThreshold
    double  mRotationThreshold
    double  mScalingThreshold
    double  mDefaultThreshold
    bool   mKeepFirstAndLastKeys
    bool   mKeepOneKey
    bool   mKeepNotPureAutoKeys
    bool IsKeyConstant(FbxAnimCurve& pCurve, int pIndex, int pFirstIndex, int pLastIndex, double pMinValue, double pMaxValue, bool pOnlyCheckAutoKeys)
#endif 
class FBXSDK_DLL FbxAnimCurveFilterScaleCompensate : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterScaleCompensate()
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)  
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = NULL)             
 return FbxAnimCurveFilter::Apply(pCurveNode, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus = NULL)        
 return FbxAnimCurveFilter::Apply(pCurve, pCount, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxIOSettings& pIOS, FbxStatus* pStatus = NULL)
class FBXSDK_DLL FbxAnimCurveFilterGimbleKiller : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterGimbleKiller()
    virtual ~FbxAnimCurveFilterGimbleKiller()
    virtual const char* GetName() const
    virtual bool NeedApply(FbxAnimStack* 
    virtual bool NeedApply(FbxObject* 
    virtual bool NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve& 
    virtual bool Apply(FbxAnimStack* 
    virtual bool Apply(FbxObject* 
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus = NULL)
    virtual bool Apply(FbxAnimCurve& 
    virtual void Reset()
	bool GetApplyKeySyncFilter() const
	void SetApplyKeySyncFilter(bool pFlag)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    FbxRotationOrder*   mRotationOrder
    bool                mApplyKeySyncFilter
    int                 mRotationLayerType
#endif 
class FBXSDK_DLL FbxAnimCurveFilterKeyReducer : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterKeyReducer()
    virtual ~FbxAnimCurveFilterKeyReducer() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)  
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)               
 return FbxAnimCurveFilter::Apply(pCurveNode, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
	double GetPrecision() const
	void SetPrecision(double pPrecision)
	bool GetKeySync() const
	void SetKeySync(bool pKeySync)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    bool  KeyReducer(FbxAnimCurve& pSCurve, FbxAnimCurve& pTCurve, FbxTime pStart, FbxTime pStop)
    bool  Subdivise(FbxAnimCurve& pSCurve, FbxAnimCurve& pTCurve, int pLeft, int pRight)
    double FindMaxError(FbxAnimCurve& pSCurve, FbxAnimCurve& pTCurve, int pLeft, int pRight, int& pSplit)
    double  mPrecision
    int    mProgressCurrentRecurseLevel
    bool   mKeySync
#endif 
class FBXSDK_DLL FbxAnimCurveFilterKeySync : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterKeySync()
    virtual ~FbxAnimCurveFilterKeySync() 
    virtual const char* GetName() const
    virtual bool NeedApply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::NeedApply(pAnimStack, pStatus)
    virtual bool NeedApply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)  
 return FbxAnimCurveFilter::NeedApply(pObj, pAnimStack, pStatus)
    virtual bool NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)               
 return FbxAnimCurveFilter::NeedApply(pCurveNode, pStatus)
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                       
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)      
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::Apply(pCurveNode, pStatus)
    virtual bool NeedApply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve& 
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& 
class FBXSDK_DLL FbxAnimCurveFilterResample : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterResample()
    virtual ~FbxAnimCurveFilterResample() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                       
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)      
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)                   
 return FbxAnimCurveFilter::Apply(pCurveNode, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)              
 return FbxAnimCurveFilter::Apply(pCurve, pCount, pStatus)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
    void SetKeysOnFrame(bool pKeysOnFrame)
    bool GetKeysOnFrame() const
    FbxTime GetPeriodTime() const
    void SetPeriodTime(FbxTime &pPeriod)
    bool  GetIntelligentMode() const
    void  SetIntelligentMode( bool pIntelligent )
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    bool    mKeysOnFrame
    FbxTime   mPeriod
	bool    mIntelligent
#endif 
class FBXSDK_DLL FbxAnimCurveFilterScale : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterScale()
    virtual ~FbxAnimCurveFilterScale() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                       
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)      
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)              
 return FbxAnimCurveFilter::Apply(pCurve, pCount, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
	double GetScale() const
	void SetScale(double pScale)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    double mScale
#endif 
class FBXSDK_DLL FbxAnimCurveFilterScaleByCurve : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterScaleByCurve()
    virtual ~FbxAnimCurveFilterScaleByCurve() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                       
 return FbxAnimCurveFilter::Apply(pAnimStack, pStatus)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)      
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack, pStatus)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)              
 return FbxAnimCurveFilter::Apply(pCurve, pCount, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
	FbxAnimCurve* GetScale() const
	void SetScale(FbxAnimCurve* pScale)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    FbxAnimCurve* mScale
#endif 
class FBXSDK_DLL FbxAnimCurveFilterTSS : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterTSS()
    virtual ~FbxAnimCurveFilterTSS() 
    virtual const char* GetName() const
    virtual bool Apply(FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)                       
 FBX_UNUSED(pStatus)
 return FbxAnimCurveFilter::Apply(pAnimStack)
    virtual bool Apply(FbxObject* pObj, FbxAnimStack* pAnimStack, FbxStatus* pStatus=NULL)      
 FBX_UNUSED(pStatus)
 return FbxAnimCurveFilter::Apply(pObj, pAnimStack)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)                   
 FBX_UNUSED(pStatus)
 return FbxAnimCurveFilter::Apply(pCurveNode)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)              
 FBX_UNUSED(pStatus)
 return FbxAnimCurveFilter::Apply(pCurve, pCount)
    virtual bool Apply(FbxAnimCurve& pCurve, FbxStatus* pStatus=NULL)
    virtual void Reset()
	FbxTime GetShift() const
	void SetShift(FbxTime& pShift)
	double GetScale() const
	void SetScale(double pScale)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    FbxTime  mShift
    double  mScale
#endif 
class FBXSDK_DLL FbxAnimCurveFilterUnroll : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterUnroll()
    virtual ~FbxAnimCurveFilterUnroll() 
    virtual const char* GetName() const
    virtual bool NeedApply(FbxAnimStack* 
    virtual bool NeedApply(FbxObject* 
    virtual bool NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve& 
    virtual bool Apply(FbxAnimStack* 
    virtual bool Apply(FbxObject* 
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& 
    virtual void Reset()
    double GetQualityTolerance() const
    void SetQualityTolerance(double pQualityTolerance)
    bool GetTestForPath() const
    void SetTestForPath(bool pTestForPath)
    bool GetForceAutoTangents() const
    void SetForceAutoTangents(bool pForceAutoTangents)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    void SetRotationOrder(FbxEuler::EOrder pOrder)
private:
    double  InterpolationQualityFactor(FbxVector4& lV1, FbxVector4& lV2)
    double           mQualityTolerance
    bool             mTestForPath
    bool             mForceAutoTangents
    FbxEuler::EOrder mRotationOrder
    int              mRotationLayerType
#endif 
class FBXSDK_DLL FbxAnimCurveFilterMatrixConverter : public FbxAnimCurveFilter
public:
    FbxAnimCurveFilterMatrixConverter()
    virtual ~FbxAnimCurveFilterMatrixConverter()
    virtual const char* GetName() const
    virtual bool NeedApply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)          
 return FbxAnimCurveFilter::NeedApply(pCurve, pCount,pStatus)
    virtual bool NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus=NULL)               
 return FbxAnimCurveFilter::NeedApply(pCurveNode, pStatus)
    virtual bool Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = NULL)                 
 return FbxAnimCurveFilter::Apply(pCurveNode, pStatus)
    virtual bool NeedApply(FbxAnimStack* 
    virtual bool NeedApply(FbxObject* 
    virtual bool NeedApply(FbxAnimCurveNode* pCurveNode[3], FbxStatus* pStatus=NULL)
    virtual bool NeedApply(FbxAnimCurve& 
    virtual bool Apply(FbxAnimStack* 
    virtual bool Apply(FbxObject* 
    virtual bool Apply(FbxAnimCurveNode* pCurveNode[3], FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve** pCurve, int pCount, FbxStatus* pStatus=NULL)
    bool Apply(FbxAnimCurve** pCurve, double* pVals, FbxStatus* pStatus=NULL)
    virtual bool Apply(FbxAnimCurve& 
    virtual void Reset()
    enum EMatrixIndex
        ePreGlobal,
        ePreTranslate,
        ePostTranslate,
        ePreRotate,
        ePostRotate,
        ePreScale,
        ePostScale,
        ePostGlobal,
        eScaleOffset,
        eInactivePre,
        eInactivePost,
        eRotationPivot,
        eScalingPivot,
        eMatrixIndexCount
    void GetSourceMatrix(EMatrixIndex pIndex, FbxAMatrix& pMatrix) const
    void SetSourceMatrix(EMatrixIndex pIndex, FbxAMatrix& pMatrix)
    void GetDestMatrix(EMatrixIndex pIndex, FbxAMatrix& pMatrix) const
    void SetDestMatrix(EMatrixIndex pIndex, FbxAMatrix& pMatrix)
    FbxTime GetResamplingPeriod () const
    void SetResamplingPeriod (FbxTime& pResamplingPeriod)
    bool GetGenerateLastKeyExactlyAtEndTime() const
    void SetGenerateLastKeyExactlyAtEndTime(bool pFlag)
    bool GetResamplingOnFrameRateMultiple() const
    void SetResamplingOnFrameRateMultiple(bool pFlag)
    bool GetApplyUnroll() const
    void SetApplyUnroll(bool pFlag)
    bool GetApplyConstantKeyReducer() const
    void SetApplyConstantKeyReducer(bool pFlag)
    bool GetResampleTranslation() const
    void SetResampleTranslation(bool pFlag)
    void SetSrcRotateOrder(FbxEuler::EOrder pOrder)
    void SetDestRotateOrder(FbxEuler::EOrder pOrder)
    void SetForceApply(bool pVal)
    bool GetForceApply() const
    void SetTranslationLimits(FbxLimits &limit )
    void SetRotationLimits(FbxLimits &limit )
    void SetScalingLimits(FbxLimits &limit )
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    enum EAxisIndex 
eX, eY, eZ, eAxisCount
    class Cell
    bool MatricesEquivalence(FbxAMatrix pMatArrayA [eMatrixIndexCount], FbxAMatrix pMatArrayB [eMatrixIndexCount]) const
    bool DoConvert(FbxAnimCurve** pCurve, 
                    double pT[eAxisCount], 
                    double pR[eAxisCount], 
                    double pS[eAxisCount])
    void FindTimeInterval
    (
        FbxTime& pStart, 
        FbxTime& pEnd,
        FbxAnimCurve* pTFCurve [eAxisCount], 
        FbxAnimCurve* pRFCurve [eAxisCount], 
        FbxAnimCurve* pSFCurve [eAxisCount]
    )
    void ComputeTotalMatrix
    (
        FbxAMatrix& pGlobal, 
        Cell& pCell,
        FbxAMatrix& pTranslate,
        FbxAMatrix& pRotate,
        FbxAMatrix& pScale
	)
    void ExtractTransforms
    (
        FbxVector4& pScaleVector,
        FbxVector4& pRotateVector,
        FbxVector4& pTranslateVector,
        FbxAMatrix& pGlobal,
        Cell& pDest
    )
    void SetDestFCurve(FbxAnimCurve* pCurve [eAxisCount], 
                       int pIndex, 
                       FbxTime pTime, 
                       FbxVector4 pVector,
                       FbxAnimCurveDef::EInterpolationType pInterpMode[eAxisCount], 
                       FbxAnimCurveDef::ETangentMode pTangentMode[eAxisCount])
    void FillInterpAndTangeant(FbxTime& pTime, 
                               FbxAnimCurve* pSourceCurve[eAxisCount], 
                               FbxAnimCurveDef::EInterpolationType* pInterp, 
                               FbxAnimCurveDef::ETangentMode* pTangeant)
    void SetDestFCurveTangeant(FbxAnimCurve* pCurve [eAxisCount], 
                               int pIndex, 
                               FbxAnimCurveDef::ETangentMode pTangentMode[eAxisCount], 
                               FbxVector4 pKeyValue, 
                               FbxVector4 pNextKeyValue)
    Cell* mSource
    Cell* mDest
    FbxTime mResamplingPeriod
    bool mResamplingOnFrameRateMultiple
    bool mApplyUnroll
    bool mApplyConstantKeyReducer
    FbxRotationOrder* mSrcRotationOrder
    FbxRotationOrder* mDestRotationOrder
    bool mGenerateLastKeyExactlyAtEndTime
    bool mResampleTranslation
    bool mForceApply
    FbxLimits mTranslationLimits
    FbxLimits mRotationLimits
    FbxLimits mScalingLimits
    FbxAnimCurveNode* mRotationCurveNode
#endif 