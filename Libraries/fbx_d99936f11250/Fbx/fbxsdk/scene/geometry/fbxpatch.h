#ifndef _FBXSDK_SCENE_GEOMETRY_PATCH_H_
#define _FBXSDK_SCENE_GEOMETRY_PATCH_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxPatch : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxPatch,FbxGeometry)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
        void SetSurfaceMode(FbxGeometry::ESurfaceMode pMode)
        inline FbxGeometry::ESurfaceMode GetSurfaceMode() const 
return mSurfaceMode
        enum EType
            eBezier,
            eBezierQuadric,
            eCardinal,
            eBSpline,
            eLinear
        void InitControlPoints(int pUCount, EType pUType, int pVCount, EType pVType)
        inline int GetUCount() const 
return mUCount
        inline int GetVCount() const 
return mVCount
        inline EType GetPatchUType() const 
return mUType
        inline EType GetPatchVType () const 
return mVType
        void SetStep(int pUStep, int pVStep)
        inline int GetUStep() const 
return mUStep
        inline int GetVStep() const 
return mVStep
        void SetClosed(bool pU, bool pV)
        inline bool GetUClosed() const 
return mUClosed
        inline bool GetVClosed() const 
return mVClosed
        void SetUCapped(bool pUBottom, bool pUTop)
        inline bool GetUCappedBottom() const 
return mUCappedBottom
        inline bool GetUCappedTop() const 
return mUCappedTop
        void SetVCapped(bool pVBottom, bool pVTop)
        inline bool GetVCappedBottom() const 
return mVCappedBottom
        inline bool GetVCappedTop() const 
return mVCappedTop
        virtual bool ContentWriteTo(FbxStream& pStream) const
        virtual bool ContentReadFrom(const FbxStream& pStream)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual void InitControlPoints(int pCount)                                  
 ParentClass::InitControlPoints(pCount)
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint , int pIndex)   
 ParentClass::SetControlPointAt(pCtrlPoint, pIndex)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    EType mUType, mVType
    int mUCount, mVCount
    int mUStep, mVStep
    bool mUClosed, mVClosed
    bool mUCappedBottom, mUCappedTop
    bool mVCappedBottom, mVCappedTop
    FbxGeometry::ESurfaceMode mSurfaceMode
#endif 