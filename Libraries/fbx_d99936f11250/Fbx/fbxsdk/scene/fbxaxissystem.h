#ifndef _FBXSDK_SCENE_AXIS_SYSTEM_H_
#define _FBXSDK_SCENE_AXIS_SYSTEM_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxAxisSystem
public:
    enum EUpVector
        eXAxis = 1,
        eYAxis = 2,
        eZAxis = 3
    enum EFrontVector
        eParityEven = 1,
        eParityOdd = 2
    enum ECoordSystem
        eRightHanded,
        eLeftHanded
    enum EPreDefinedAxisSystem
        eMayaZUp,			
        eMax,				
        eOpenGL,			
        eLightwave			
		FbxAxisSystem()
		FbxAxisSystem(EUpVector pUpVector, EFrontVector pFrontVector, ECoordSystem pCoorSystem)
		FbxAxisSystem(const FbxAxisSystem& pAxisSystem)
		FbxAxisSystem(const EPreDefinedAxisSystem pAxisSystem)
		virtual ~FbxAxisSystem()
    bool operator==(const FbxAxisSystem& pAxisSystem)const
    bool operator!=(const FbxAxisSystem& pAxisSystem)const
    FbxAxisSystem& operator=(const FbxAxisSystem& pAxisSystem)
    static const FbxAxisSystem MayaZUp
    static const FbxAxisSystem MayaYUp
    static const FbxAxisSystem Max
    static const FbxAxisSystem Motionbuilder
    static const FbxAxisSystem OpenGL
    static const FbxAxisSystem DirectX
    static const FbxAxisSystem Lightwave
    void ConvertScene(FbxScene* pScene) const
    void ConvertScene(FbxScene* pScene, FbxNode* pFbxRoot) const
    EFrontVector GetFrontVector( int & pSign ) const
    EUpVector GetUpVector( int & pSign ) const
    ECoordSystem GetCoorSystem() const
	void GetMatrix(FbxAMatrix& pMatrix)
    void ConvertChildren(FbxNode* pRoot, const FbxAxisSystem& pSrcSystem) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	class AxisDef
	public:
		enum EAxis 
eXAxis, eYAxis, eZAxis
		bool operator==(const AxisDef& pAxis) const 
 return (mAxis == pAxis.mAxis) && (mSign == pAxis.mSign)
		EAxis	mAxis
		int		mSign
    AxisDef mUpVector
    AxisDef mFrontVector
    AxisDef mCoorSystem
    void ConvertTProperty(FbxArray<FbxNode*>& pNodes, const FbxAxisSystem& pFrom) const
    void ConvertCurveNodes(FbxArray<FbxAnimCurveNode*>& pCurveNodes, const FbxAxisSystem& pFrom) const
    void AdjustPreRotation(FbxNode* pNode, const FbxMatrix& pConversionRM) const
    void AdjustPivots(FbxNode* pNode, const FbxMatrix& pConversionRM) const
    void GetConversionMatrix(const FbxAxisSystem& pFrom, FbxMatrix& pConversionRM) const
    void AdjustLimits(FbxNode* pNode, const FbxMatrix& pConversionRM) const
    void AdjustPoses(FbxScene* pScene, const FbxMatrix& pConversionRM) const
    void AdjustCamera(FbxNode* pNode, const FbxMatrix& pConversionRM ) const
    void AdjustCluster(FbxNode* pNode, const FbxMatrix& pConversionRM) const
    void ConvertChildren(FbxNode* pRoot, const FbxAxisSystem& pSrcSystem, bool pSubChildrenOnly) const
    friend class FbxGlobalSettings
#endif 