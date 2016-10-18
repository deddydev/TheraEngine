#ifndef _FBXSDK_SCENE_GEOMETRY_SUB_DIV_H_
#define _FBXSDK_SCENE_GEOMETRY_SUB_DIV_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxMesh
#ifndef DOXYGEN_SHOULD_SKIP_THIS
class FBXSDK_DLL FbxSubDiv : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxSubDiv, FbxGeometry)
public:
    enum EScheme
        eCatmullClark,  
        eDooCSabin,      
        eLoop,           
        eLinear,         
    enum ETesselationPattern
        eOddFractional,
        eEvenFractional,
        eInteger,
        ePower2,      
	enum EDisplaySmoothness
		eHull,
		eRough,
		eMedium,
		eFine,
    void InitSubdivLevel(int pLevelCount, 
        EScheme pScheme = eCatmullClark, 
        ETesselationPattern pPattern = ePower2)
    virtual FbxNodeAttribute::EType GetAttributeType() const
    static const int MAX_SUBDIV_LEVEL = 16
    FbxArray<FbxMesh*> mSubDivLevel
    FbxMesh* GetBaseMesh() const
    FbxMesh* GetFinestMesh() const
    bool SetFinestMesh(FbxMesh* pMesh)
    bool SetBaseMesh(FbxMesh* pMesh)
    FbxMesh* GetMesh(int pLevel) const
    void SetSubdivLevelMesh(int pLevel, FbxMesh* pMesh)
    int GetLevelCount() const
    void SetLevelCount(int pLevelCount)
    int GetCurrentLevel() const
    void SetCurrentLevel(int pCurrentLevel)
	FbxMesh* GetCurrentMesh() const
    FbxSubDiv::EScheme GetSubdivScheme() const
    FbxSubDiv::ETesselationPattern GetTessPattern() const
    void SetSubdivScheme(FbxSubDiv::EScheme pScheme)
    void SetTessPattern(FbxSubDiv::ETesselationPattern pPattern)
	FbxSubDiv::EDisplaySmoothness GetDisplaySmoothness() const
	void SetDisplaySmoothness(FbxSubDiv::EDisplaySmoothness pSmoothness)
private:
    FbxMesh* mBaseMesh
    FbxMesh* mFinestMesh
    int mCurrLevel
    int mLevelCount
    EScheme mScheme
    ETesselationPattern mPattern
	EDisplaySmoothness mSmoothness
#endif 