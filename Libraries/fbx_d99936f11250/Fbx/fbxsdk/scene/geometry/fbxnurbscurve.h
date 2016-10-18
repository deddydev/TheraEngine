#ifndef _FBXSDK_SCENE_GEOMETRY_NURBS_CURVE_H_
#define _FBXSDK_SCENE_GEOMETRY_NURBS_CURVE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/scene/geometry/fbxline.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxNurbsCurve : public FbxGeometry 
	FBXSDK_OBJECT_DECLARE(FbxNurbsCurve,FbxGeometry)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
	enum EDimension
		e2D = 2,
		e3D
	enum EType
		eOpen,
		eClosed,
		ePeriodic
	void InitControlPoints( int pCount, EType pVType )
	inline double* GetKnotVector() const 
 return mKnotVector
	int GetKnotCount() const
	inline void SetOrder( int pOrder ) 
 mOrder = pOrder
	inline int GetOrder() const 
 return mOrder
    inline void SetStep( int pStep ) 
 mStep = pStep
    inline int GetStep() const 
 return mStep
	inline void SetDimension( EDimension pDimension ) 
 mDimension = pDimension
	inline EDimension GetDimension() const 
 return mDimension
	bool IsRational()
	int GetSpanCount() const
	inline EType GetType() const 
 return mNurbsType
	inline bool IsPolyline() const 
 return ( GetOrder() == 2 )
	bool IsBezier() const
    int TessellateCurve(FbxArray<FbxVector4>& pPointArray, int pStep = 16)
    FbxLine* TessellateCurve(int pStep = 16)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
	bool FullMultiplicity() const
	enum EErrorCode
		eNurbsCurveTypeUnknown,
		eWeightTooSmall,
		eKnotVectorError,
        eWrongNumberOfControlPoint,
		eErrorCount
	bool mIsRational
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint , int pIndex) 
 ParentClass::SetControlPointAt(pCtrlPoint, pIndex)
    virtual void InitControlPoints(int pCount)                                
 ParentClass::InitControlPoints(pCount)
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	void Reset()
private:
	double*		mKnotVector
	EType		mNurbsType
	int			mOrder
	EDimension	mDimension
    int			mStep
#endif 