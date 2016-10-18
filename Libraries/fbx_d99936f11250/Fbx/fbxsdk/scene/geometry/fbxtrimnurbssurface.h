#ifndef _FBXSDK_SCENE_GEOMETRY_TRIM_NURBS_SURFACE_H_
#define _FBXSDK_SCENE_GEOMETRY_TRIM_NURBS_SURFACE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/scene/geometry/fbxnurbscurve.h>
#include <fbxsdk/scene/geometry/fbxnurbssurface.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBoundary : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxBoundary, FbxGeometry)
public:
    static const char* sOuterFlag
    FbxPropertyT<FbxBool> OuterFlag
    void AddCurve( FbxNurbsCurve* pCurve )
    int GetCurveCount() const
    FbxNurbsCurve* GetCurve( int pIndex )
    const FbxNurbsCurve* GetCurve( int pIndex ) const
    virtual FbxNodeAttribute::EType GetAttributeType() const
    bool IsPointInControlHull(const FbxVector4& pPoint )
    FbxVector4 ComputePointInBoundary()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    void ClearCurves()
    void CopyCurves( FbxBoundary const& pOther )
    bool IsValid(bool mustClosed = true)
    bool IsCounterClockwise()
protected:
    virtual void ConstructProperties(bool pForceSet)
    void Reset()
    bool LineSegmentIntersect(const FbxVector4 & pStart1, const FbxVector4 & pEnd1, const FbxVector4 & pStart2, const FbxVector4 & pEnd2 ) const
#endif 
class FBXSDK_DLL FbxTrimNurbsSurface : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxTrimNurbsSurface,FbxGeometry)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    int GetTrimRegionCount() const
    void BeginTrimRegion()
    void EndTrimRegion()
    bool              AddBoundary( FbxBoundary* pBoundary )
    FbxBoundary*     GetBoundary( int pIndex, int pRegionIndex = 0 )
    const FbxBoundary*     GetBoundary( int pIndex, int pRegionIndex = 0 ) const
    int               GetBoundaryCount(int pRegionIndex = 0) const
    void       SetNurbsSurface( const FbxNurbsSurface* pNurbs )
    FbxNurbsSurface* GetNurbsSurface()
    const FbxNurbsSurface* GetNurbsSurface() const
    inline void SetFlipNormals( bool pFlip ) 
 mFlipNormals = pFlip
    inline bool GetFlipNormals() const 
 return  mFlipNormals
    virtual int GetControlPointsCount() const
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint, const FbxVector4 &pNormal , int pIndex, bool pI2DSearch = false)
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint, int pIndex) 
 ParentClass::SetControlPointAt(pCtrlPoint, pIndex)
    virtual FbxVector4* GetControlPoints(FbxStatus* pStatus = NULL) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    bool IsValid(bool mustClosed = true)
    void ClearBoundaries()
    void CopyBoundaries( FbxTrimNurbsSurface const& pOther )
    bool IsValid(int pRegion, bool mustClosed = true)
    void RebuildRegions()
protected:
	virtual void Construct(const FbxObject* pFrom)
private:
    bool			mFlipNormals
    FbxArray<int>	mRegionIndices
    bool			mNewRegion
#endif 