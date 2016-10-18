#ifndef _FBXSDK_SCENE_GEOMETRY_NURBS_H_
#define _FBXSDK_SCENE_GEOMETRY_NURBS_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxNurbs : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxNurbs, FbxGeometry)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
    void SetSurfaceMode(FbxGeometry::ESurfaceMode pMode)
    inline ESurfaceMode GetSurfaceMode() const 
return mSurfaceMode
    enum EType
        ePeriodic,	
        eClosed,	
        eOpen		
    void InitControlPoints(int pUCount, EType pUType, int pVCount, EType pVType)
    inline int GetUCount() const 
return mUCount
    inline int GetVCount() const 
return mVCount
    inline EType GetNurbsUType() const 
return mUType
    inline EType GetNurbsVType() const 
return mVType
    int GetUKnotCount() const
    double* GetUKnotVector() const
    int GetVKnotCount() const
    double* GetVKnotVector() const
    int* GetUMultiplicityVector() const
    int* GetVMultiplicityVector() const
    void SetOrder(FbxUInt pUOrder, FbxUInt pVOrder)
    inline int GetUOrder() const 
return mUOrder
    inline int GetVOrder() const 
return mVOrder
    void SetStep(int pUStep, int pVStep)
    inline int GetUStep() const 
return mUStep
    inline int GetVStep() const 
return mVStep
    int GetUSpanCount() const
    int GetVSpanCount() const
    void SetApplyFlipUV(bool pFlag)
    bool GetApplyFlipUV() const
    void SetApplyFlipLinks(bool pFlag)
    bool GetApplyFlipLinks() const
    bool GetApplyFlip() const 
 return GetApplyFlipUV() || GetApplyFlipLinks()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    enum EErrorCode
        eNurbsTypeUnknown,
        eWrongNumberOfControlPoint,
        eWeightTooSmall,
        eUMultiplicityVectorError,
        eVMultiplicityVectorError,
        eUKnotVectorError,
        eVKnotVectorError,
        eErrorCount
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint , int pIndex) 
 ParentClass::SetControlPointAt(pCtrlPoint, pIndex)
    virtual void InitControlPoints(int pCount)                                
 ParentClass::InitControlPoints(pCount)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    FbxUInt mUOrder, mVOrder
    int mUCount, mVCount
    int mUStep, mVStep
    EType mUType, mVType
    double* mUKnotVector
    double* mVKnotVector
    int* mUMultiplicityVector
    int* mVMultiplicityVector
    ESurfaceMode mSurfaceMode
    bool mApplyFlipUV
    bool mApplyFlipLinks
    friend class FbxGeometryConverter
#endif 