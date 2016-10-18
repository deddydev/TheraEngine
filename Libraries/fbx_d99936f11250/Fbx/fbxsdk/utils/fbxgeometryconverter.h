#ifndef _FBXSDK_UTILS_GEOMETRY_CONVERTER_H_
#define _FBXSDK_UTILS_GEOMETRY_CONVERTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxManager
class FbxMesh
class FbxPatch
class FbxNurbs
class FbxNurbsSurface
class FbxNurbsCurve
class FbxWeightedMapping
class FbxSurfaceEvaluator
class FbxScene
class FbxNode
class FbxNodeAttribute
class FbxGeometry
class FBXSDK_DLL FbxGeometryConverter
public:
		bool Triangulate(FbxScene* pScene, bool pReplace, bool pLegacy=false)
		FbxNodeAttribute* Triangulate(FbxNodeAttribute* pNodeAttribute, bool pReplace, bool pLegacy=false)
		bool ComputeGeometryControlPointsWeightedMapping(FbxGeometry* pSrcGeom, FbxGeometry* pDstGeom, FbxWeightedMapping* pSrcToDstWeightedMapping, bool pSwapUV=false)
		FbxNurbs* ConvertPatchToNurbs(FbxPatch *pPatch)
		bool ConvertPatchToNurbsInPlace(FbxNode* pNode)
		FbxNurbsSurface* ConvertPatchToNurbsSurface(FbxPatch *pPatch)
		bool ConvertPatchToNurbsSurfaceInPlace(FbxNode* pNode)
		FbxNurbsSurface* ConvertNurbsToNurbsSurface( FbxNurbs* pNurbs )
		FbxNurbs* ConvertNurbsSurfaceToNurbs( FbxNurbsSurface* pNurbs )
		bool ConvertNurbsToNurbsSurfaceInPlace(FbxNode* pNode)
		bool ConvertNurbsSurfaceToNurbsInPlace(FbxNode* pNode)
		FbxNurbs* FlipNurbs(FbxNurbs* pNurbs, bool pSwapUV, bool pSwapClusters)
		FbxNurbsSurface* FlipNurbsSurface(FbxNurbsSurface* pNurbs, bool pSwapUV, bool pSwapClusters)
		bool EmulateNormalsByPolygonVertex(FbxMesh* pMesh)
		bool ComputeEdgeSmoothingFromNormals( FbxMesh* pMesh ) const
		bool ComputePolygonSmoothingFromEdgeSmoothing( FbxMesh* pMesh, int pIndex=0 ) const
		bool ComputeEdgeSmoothingFromPolygonSmoothing( FbxMesh* pMesh, int pIndex=0 ) const
		bool SplitMeshesPerMaterial(FbxScene* pScene, bool pReplace)
		bool SplitMeshPerMaterial(FbxMesh* pMesh, bool pReplace)
	bool RecenterSceneToWorldCenter(FbxScene* pScene, FbxDouble pThreshold)
	FbxNode* MergeMeshes(FbxArray<FbxNode*>& pMeshNodes, const char* pNodeName, FbxScene* pScene)
	void RemoveBadPolygonsFromMeshes(FbxScene* pScene, FbxArray<FbxNode*>* pAffectedNodes = NULL)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    FbxGeometryConverter(FbxManager* pManager)
    ~FbxGeometryConverter()
private:
	FbxMesh* TriangulateMeshInternal(const FbxMesh* pMesh)
	FbxMesh* TriangulateMeshInternalLegacy(const FbxMesh* pMesh)
	FbxMesh* TriangulatePatchInternal(const FbxPatch* pPatch)
	FbxMesh* TriangulateNurbsInternal(const FbxNurbs* pNurbs)
	bool AddAlternateGeometry(FbxNode* pNode, FbxGeometry* pSrcGeom, FbxGeometry* pAltGeom, FbxWeightedMapping* pSrcToAltWeightedMapping, bool pConvertDeformations)
	bool ConvertGeometryAnimation(FbxNode* pNode, FbxGeometry* pSrcGeom, FbxGeometry* pDstGeom)
	void ReplaceNodeAttribute(FbxNode* pNode, FbxNodeAttribute* pNewNodeAttr)
	bool AddTriangulatedMeshGeometry(FbxNode* pNode, int pUVStepCoeff)
	bool CreateAndCopyLayerElement(FbxMesh *pNewMesh, FbxMesh *pRefMesh)
	bool SetLayerElements(FbxMesh *pNewMesh, FbxMesh *pMesh, int pPolygonIndex, int pPolyPointIndex, int pLoopIndex, bool pIsSearched, bool pIsEndPolygon)
    static void FbxTriangulation(int *Index, int pNumSide)
    bool ComputePatchToMeshControlPointsWeightedMapping(FbxPatch* pSrcPatch, FbxMesh* pDstMesh, FbxWeightedMapping* pMapping, bool pSwapUV=false)
    bool ComputeNurbsToMeshControlPointsWeightedMapping(FbxNurbsSurface* pSrcNurbs, FbxMesh* pDstMesh, FbxWeightedMapping* pMapping, bool pRescaleUVs=false, bool pSwapUV=false)
    void InitializeWeightInControlPoints(FbxGeometryBase* pGeometry)
    void InitializeWeightInNormals(FbxLayerContainer* pLayerContainer)
    void TriangulateContinuousSurface(FbxMesh* pMesh, FbxSurfaceEvaluator* pSurface, FbxUInt pPointCountX, FbxUInt pPointCountY, bool ClockWise=false)
    void CheckForZeroWeightInShape(FbxGeometry *pGeometry)
    FbxMesh* CreateMeshFromParametricSurface(const FbxGeometry* pGeometry)
    FbxNurbs* CreateNurbsFromPatch(FbxPatch* pPatch)
    FbxNurbsSurface* CreateNurbsSurfaceFromPatch(FbxPatch* pPatch)
    void ConvertShapes(const FbxGeometry* pSource, FbxGeometry* pDestination, FbxSurfaceEvaluator* pEvaluator, int pUCount, int pVCount)
    void ConvertShapes(const FbxGeometry* pSource, FbxGeometry* pDestination, FbxWeightedMapping* pSourceToDestinationMapping)
    void ConvertClusters(const FbxGeometry* pSource, FbxGeometry* pDestination, FbxWeightedMapping* pSourceToDestinationMapping)
    void ConvertClusters(FbxArray<FbxCluster*> const& pSourceClusters, int pSourceControlPointsCount, FbxArray<FbxCluster*>& pDestinationClusters, int pDestinationControlPointsCount, FbxWeightedMapping* pSourceToDestinationMapping)
    void BuildClusterToSourceMapping(FbxArray<FbxCluster*> const& pSourceClusters, FbxWeightedMapping* pClusterToSourceMapping)
    void CheckClusterToSourceMapping(FbxWeightedMapping* pClusterToSourceMapping)
    void ConvertCluster(int pSourceClusterIndex, FbxWeightedMapping* pClusterToSourceMapping, FbxWeightedMapping* pSourceToDestinationMapping, FbxCluster* pDestinationCluster)
    void DuplicateControlPoints(FbxArray<FbxVector4>& pControlPoints, FbxArray<int>& pPolygonVertices)
    void UpdatePolygon(FbxMesh *pNewMesh, FbxMesh const *pRefMesh, int pPolygonIndex, int* pNewIndex, int &pVerticeIndexMeshTriangulated, int &pPolygonIndexMeshTriangulated)
    void UpdatePolygon(FbxMesh *pNewMesh, FbxMesh const *pRefMesh, int pPolygonIndex, int* pNewIndex, int &pVerticeIndexMeshTriangulated, int &pPolygonIndexMeshTriangulated, int pTriangleNum)
    void ResizePolygon(FbxMesh *pNewMesh, int pNewCountVertices = 0, int pNewCountPolygons =0, bool pClearFlag = true)
    template <class T1, class T2> void ConvertNurbs(T1* pNewNurbs, T2* pOldNurb)
    bool CopyAnimationCurves(FbxNode* pNode, FbxGeometry* pNewGeometry)
    bool FlipNurbsCurve(FbxNurbsCurve* pCurve) const
    void FlipControlPoints(FbxGeometryBase* pPoints, int pUCount, int pVCount) const
    bool ConvertMaterialReferenceMode(FbxMesh* pMeshRef) const
    void RevertMaterialReferenceModeConversion(FbxMesh* pMeshRef) const
    FbxManager* mManager
	friend class FbxWriter3ds
#endif 