#ifndef _FBXSDK_SCENE_GEOMETRY_MESH_H_
#define _FBXSDK_SCENE_GEOMETRY_MESH_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxMesh : public FbxGeometry
	FBXSDK_OBJECT_DECLARE(FbxMesh, FbxGeometry)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
		void BeginPolygon(int pMaterial=-1, int pTexture=-1, int pGroup=-1, bool pLegacy=true)
		void BeginPolygonExt(int pMaterial, int* pTextures)
		void AddPolygon(int pIndex, int pTextureUVIndex = -1)
		void EndPolygon()
		inline int GetPolygonCount() const 
 return mPolygons.GetCount()
		inline int GetPolygonSize(int pPolygonIndex) const
			return ( pPolygonIndex >= 0 && pPolygonIndex < mPolygons.GetCount() ) ? mPolygons[pPolygonIndex].mSize : -1
		int GetPolygonGroup(int pPolygonIndex) const
		inline void SetPolygonGroup(int pPolygonIndex, int pGroup) const
			if( pPolygonIndex >= 0 && pPolygonIndex<mPolygons.GetCount() ) mPolygons[pPolygonIndex].mGroup = pGroup
		inline int GetPolygonVertex(int pPolygonIndex, int pPositionInPolygon) const
			return ( pPolygonIndex >= 0 && pPolygonIndex < mPolygons.GetCount() && pPositionInPolygon >= 0 && pPositionInPolygon < mPolygons[pPolygonIndex].mSize ) ?
				mPolygonVertices[mPolygons[pPolygonIndex].mIndex + pPositionInPolygon] : -1
		bool GetPolygonVertexNormal(int pPolyIndex, int pVertexIndex, FbxVector4& pNormal) const
		bool GetPolygonVertexNormals(FbxArray<FbxVector4>& pNormals) const
		bool GetPolygonVertexUV(int pPolyIndex, int pVertexIndex, const char* pUVSetName, FbxVector2& pUV, bool& pUnmapped) const
		bool GetPolygonVertexUVs(const char* pUVSetName, FbxArray<FbxVector2>& pUVs, FbxArray<int>* pUnmappedUVId = NULL) const
		int* GetPolygonVertices() const
		inline int GetPolygonVertexCount() const 
 return mPolygonVertices.Size()
		int GetPolygonVertexIndex(int pPolygonIndex) const
		int RemovePolygon(int pPolygonIndex)
		int RemoveDuplicatedEdges(FbxArray<int>& pEdgeIndexList)
		void InitTextureUV(int pCount, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		void AddTextureUV(FbxVector2 pUV, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		int GetTextureUVCount(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		int GetUVLayerCount() const
		FbxArray<FbxLayerElement::EType> GetAllChannelUV(int pLayer)
		void InitMaterialIndices(FbxLayerElement::EMappingMode pMappingMode)
		void InitTextureIndices(FbxLayerElement::EMappingMode pMappingMode, FbxLayerElement::EType pTextureType)
		void InitTextureUVIndices(FbxLayerElement::EMappingMode pMappingMode, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		int GetTextureUVIndex(int pPolygonIndex, int pPositionInPolygon, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		void SetTextureUVIndex(int pPolygonIndex, int pPositionInPolygon, int pIndex, FbxLayerElement::EType pTypeIdentifier)
		void Reset()
		bool GenerateNormals(bool pOverwrite=false, bool pByCtrlPoint = false, bool pCW=false)
		bool CheckIfVertexNormalsCCW()
		class DuplicateVertex
		public:
			DuplicateVertex() :
				lVertexPolyIndex(0),
				lNewVertexIndex(0),
				lNormal(0, 0, 0),
				lUV(0, 0),
				lEdgeIndex(0)
			int			lVertexPolyIndex
			int			lNewVertexIndex
			FbxVector4	lNormal
			FbxVector2	lUV
			int			lEdgeIndex
		class VertexNormalInfo
		public:
			VertexNormalInfo() :
				mTotalNormal(0, 0, 0),
				mNumNormal(0)
			FbxVector4	mTotalNormal
			int			mNumNormal
		bool CheckSamePointTwice() const
		int RemoveBadPolygons()
		bool SplitPoints(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
		bool BuildMergeList(FbxArray<int>& pMergeList, bool pExport=false)
		void MergePointsForPolygonVerteNormals(FbxArray<int> &pMergeList)
		void BuildMeshEdgeArray()
		int GetMeshEdgeCount() const
		int GetMeshEdgeIndex(int pStartVertexIndex, int pEndVertexIndex, bool& pReversed, int pExistedEdgeCount=-1)
		void BeginGetMeshEdgeIndexForPolygon()
		void EndGetMeshEdgeIndexForPolygon()
		int GetMeshEdgeIndexForPolygon(int pPolygon, int pPositionInPolygon)
		void GetMeshEdgeVertices(int pEdgeIndex, int& pStartVertexIndex, int& pEndVertexIndex) const
		void BeginGetMeshEdgeVertices()
		void EndGetMeshEdgeVertices()
		void SetMeshEdgeCount(int pEdgeCount)
		inline void SetMeshEdge(int pEdgeIndex, int pValue)
 if( pEdgeIndex >= 0 && pEdgeIndex < mEdgeArray.GetCount() ) mEdgeArray[pEdgeIndex] = pValue
		int AddMeshEdgeIndex(int pStartVertexIndex, int pEndVertexIndex, bool pCheckForDuplicates)
		int SetMeshEdgeIndex(int pEdgeIndex, int pStartVertexIndex, int pEndVertexIndex, bool pCheckForDuplicates, int pExistedEdgeCount=-1)
		void BeginAddMeshEdgeIndex()
		int AddMeshEdgeIndexForPolygon(int pPolygonIndex, int pPositionInPolygon)
		bool SetMeshEdgeIndex(int pEdgeIndex, int pPolygonIndex, int pPositionInPolygon)
		bool IsTriangleMesh() const
	inline void ReservePolygonCount(int pCount) 
 mPolygons.Reserve(pCount)
	inline void ReservePolygonVertexCount(int pCount) 
 mPolygonVertices.Reserve(pCount)
	bool GetTextureUV(FbxLayerElementArrayTemplate<FbxVector2>** pLockableArray, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse) const
	bool GetMaterialIndices(FbxLayerElementArrayTemplate<int>** pLockableArray) const
	bool GetTextureIndices(FbxLayerElementArrayTemplate<int>** pLockableArray, FbxLayerElement::EType pTextureType) const
		double GetEdgeCreaseInfo(int pEdgeIndex)
		bool GetEdgeCreaseInfoArray(FbxLayerElementArrayTemplate<double>** pCreaseArray)
		double GetVertexCreaseInfo(int pVertexIndex)
		bool GetVertexCreaseInfoArray(FbxLayerElementArrayTemplate<double>** pCreaseArray)
		bool SetEdgeCreaseInfo(int pEdgeIndex, double pWeight)
		bool SetEdgeCreaseInfoArray(FbxArray<double>* pWeightArray)
		bool SetVertexCreaseInfo(int pVertexIndex, double pWeight)
		bool SetVertexCreaseInfoArray(FbxArray<double>* pWeightArray)
		enum ESmoothness
			eHull,		
			eRough,		
			eMedium,	
			eFine		
		FbxMesh::ESmoothness GetMeshSmoothness() const
		void SetMeshSmoothness(FbxMesh::ESmoothness pSmoothness)
		int GetMeshPreviewDivisionLevels() const
		void SetMeshPreviewDivisionLevels(int pPreviewDivisionLevels)
		int GetMeshRenderDivisionLevels() const
		void SetMeshRenderDivisionLevels(int pRenderDivisionLevels)
		bool GetDisplaySubdivisions() const
		void SetDisplaySubdivisions(bool pDisplySubdivisions)
		EBoundaryRule GetBoundaryRule() const
		void SetBoundaryRule(EBoundaryRule pBoundaryRule)
		bool GetPreserveBorders() const
		void SetPreserveBorders(bool pPreserveBorders)
		bool GetPreserveHardEdges() const
		void SetPreserveHardEdges(bool pPreserveHardEdges)
		bool GetPropagateEdgeHardness() const
		void SetPropagateEdgeHardness(bool pPropagateEdgeHardness)
		bool GetPolyHoleInfo(int pFaceIndex)
		bool GetPolyHoleInfoArray(FbxLayerElementArrayTemplate<bool>** pHoleArray)
		bool SetPolyHoleInfo(int pFaceIndex, bool pIsHole)
		bool SetPolyHoleInfoArray(FbxArray<bool>* pHoleArray)
		bool GenerateTangentsData(const char* pUVSetName=NULL, bool pOverwrite=false)
		bool GenerateTangentsData(int pUVSetLayerIndex, bool pOverwrite=false)
		bool GenerateTangentsDataForAllUVSets(bool pOverwrite=false)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
	virtual void Compact()
	struct PolygonDef
 int mIndex
 int mSize
 int mGroup
    FbxArray<PolygonDef>	mPolygons
    FbxArray<int>			mPolygonVertices
    FbxArray<int>			mEdgeArray
	FbxArray<PolygonDef>*	mOriginalPolygons
	FbxArray<int>*			mOriginalPolygonVertices
	int						mOriginalControlPointsCount
    struct ComponentMap
        FbxArray<int> mData
        FbxArray<int> mOffsets
        int GetDataCount(int pIndex) 
 return mOffsets[pIndex + 1] - mOffsets[pIndex]
        int GetData(int pIndex, int pSubIndex) 
 return mData[ mOffsets[pIndex] + pSubIndex ]
        int GetComponentCount() 
 return mOffsets.GetCount() - 1
    void ComputeComponentMaps(ComponentMap& pEdgeToPolyMap, ComponentMap& pPolyToEdgeMap)
	class ControlPointToVerticesMap
	public:
		ControlPointToVerticesMap()
		~ControlPointToVerticesMap()
		bool Valid()
		void Fill(FbxMesh* pMesh)
		int  GetCount()
		bool Init(int pNbEntries)
		void Clear()
		FbxArray<int>* GetVerticesArray(int pControlPoint)
		FbxArray<int>* operator[](int pControlPoint)
	private:
		FbxArray< FbxArray<int>* > mMap
	void ComputeControlPointToVerticesMap(ControlPointToVerticesMap& pMap)
	bool ConformNormalsTo(const FbxMesh* pMesh)
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	virtual void ContentClear()
	void InitTextureIndices(FbxLayerElementTexture* pLayerElementTexture, FbxLayerElement::EMappingMode pMappingMode)
	void RemoveTextureIndex(FbxLayerElementTexture* pLayerElementTextures, int pPolygonIndex, int pOffset)
	void RemoveUVIndex(FbxLayerElementUV* pLayerElementUV, int pPolygonIndex, int pOffset)
	bool GetBadPolyIndices(FbxArray<int>& pArrayBadPolyIndices, bool pCheckOne) const
	struct SplitEdgeData 
 int mOriginalEdge
 bool mIsNew
	ESmoothness		mSmoothness
	int				mPreviewDivisionLevels
	int				mRenderDivisionLevels
	bool			mDisplaySubdivisions
	EBoundaryRule	mBoundaryRule
	bool			mPreserveBorders
	bool			mPreserveHardEdges
	bool			mPropagateEdgeHardness
	struct PolygonIndexDef 
 int mPolygonIndex
 int mSubPolygonIndex
	struct V2PVMap
		PolygonIndexDef* mV2PV
		int* mV2PVOffset
		int* mV2PVCount
		FbxArray<FbxSet<int>* > mPVEdge
		bool mValid
		FbxArray<int> mV2Edge
 mV2PVMap
	struct EdgeLookupDef 
 FbxArray<int> mPVFlags
 bool mValid
 mPVEndFlags
	int						FindPolygonIndex(int pEdgeIndex)
	static int				PolygonIndexCompare(const void* p1, const void* p2)
	void					PolySetTexture(FbxLayer* pLayer, int pTextureIndex, FbxLayerElement::EType pTextureType)
	template<class T> bool	GetPolygonVertexLayerElementIndex(const FbxLayerElementTemplate<T>* pLayerElement, int pPolyIndex, int pVertexIndex, int& pIndex) const
	template<class T> bool	GetPolygonVertexLayerElementValue(const FbxLayerElementTemplate<T>* pLayerElement, int pPolyIndex, int pVertexIndex, T& pValue, bool pAllowUnmapped) const
	friend class FbxGeometryConverter
private:
    bool GenerateTangentsData(FbxLayerElementUV* pUVSet, int pLayerIndex, bool pOverwrite=false)
    void FillMeshEdgeTable(FbxArray<int>& pTable, int* pValue, void (*FillFct)(FbxArray<int>& pTable, int pIndex, int* pValue))
    void ComputeNormalsPerCtrlPoint(FbxArray<VertexNormalInfo>& lNormalInfo, bool pCW=false)
    void ComputeNormalsPerPolygonVertex(FbxArray<VertexNormalInfo>& lNormalInfo, bool pCW=false)
    void GenerateNormalsByCtrlPoint(bool pCW)
#endif 