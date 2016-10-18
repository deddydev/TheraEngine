#ifndef _FBXSDK_SCENE_GEOMETRY_BASE_H_
#define _FBXSDK_SCENE_GEOMETRY_BASE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/math/fbxvector4.h>
#include <fbxsdk/scene/geometry/fbxlayercontainer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxStatus
class FBXSDK_DLL FbxGeometryBase : public FbxLayerContainer
    FBXSDK_OBJECT_DECLARE(FbxGeometryBase, FbxLayerContainer)
public:
    virtual void InitControlPoints(int pCount)
    void InitNormals(int pCount = 0 )
    void InitNormals(FbxGeometryBase* pSrc)
    void InitTangents(int pCount = 0, const int pLayerIndex = 0, const char* pName = "" )
    void InitTangents(FbxGeometryBase* pSrc, const int pLayerIndex = 0)
    void InitBinormals(int pCount = 0, const int pLayerIndex = 0, const char* pName = "" )
    void InitBinormals(FbxGeometryBase* pSrc, const int pLayerIndex = 0)
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint , const FbxVector4 &pNormal , int pIndex, bool pI2DSearch = false)
    virtual void SetControlPointAt(const FbxVector4 &pCtrlPoint , int pIndex)
    virtual FbxVector4 GetControlPointAt(int pIndex) const
    virtual void SetControlPointNormalAt(const FbxVector4 &pNormal, int pIndex, bool pI2DSearch=false)
    virtual int GetControlPointsCount() const
    virtual FbxVector4* GetControlPoints(FbxStatus* pStatus=NULL) const
	virtual void SetControlPointCount(int pCount)
		FbxPropertyT<FbxBool> PrimaryVisibility
		FbxPropertyT<FbxBool> CastShadow
		FbxPropertyT<FbxBool> ReceiveShadow
        FbxPropertyT<FbxDouble3> BBoxMin
        FbxPropertyT<FbxDouble3> BBoxMax
        void ComputeBBox()
    FbxGeometryElementNormal* CreateElementNormal()
    bool RemoveElementNormal(FbxGeometryElementNormal* pElementNormal)
    FbxGeometryElementNormal* GetElementNormal(int pIndex = 0)
    const FbxGeometryElementNormal* GetElementNormal(int pIndex = 0) const
    int GetElementNormalCount() const
    FbxGeometryElementBinormal* CreateElementBinormal()
    bool RemoveElementBinormal(FbxGeometryElementBinormal* pElementBinormal)
    FbxGeometryElementBinormal* GetElementBinormal(int pIndex = 0)
    const FbxGeometryElementBinormal* GetElementBinormal(int pIndex = 0) const
    int GetElementBinormalCount() const
    FbxGeometryElementTangent* CreateElementTangent()
    bool RemoveElementTangent(FbxGeometryElementTangent* pElementTangent)
    FbxGeometryElementTangent* GetElementTangent(int pIndex = 0)
    const FbxGeometryElementTangent* GetElementTangent(int pIndex = 0) const
    int GetElementTangentCount() const
    FbxGeometryElementMaterial* CreateElementMaterial()
    bool RemoveElementMaterial(FbxGeometryElementMaterial* pElementMaterial)
    FbxGeometryElementMaterial* GetElementMaterial(int pIndex = 0)
    const FbxGeometryElementMaterial* GetElementMaterial(int pIndex = 0) const
    int GetElementMaterialCount() const
    FbxGeometryElementPolygonGroup* CreateElementPolygonGroup()
    bool RemoveElementPolygonGroup(FbxGeometryElementPolygonGroup* pElementPolygonGroup)
    FbxGeometryElementPolygonGroup* GetElementPolygonGroup(int pIndex = 0)
    const FbxGeometryElementPolygonGroup* GetElementPolygonGroup(int pIndex = 0) const
    int GetElementPolygonGroupCount() const
    FbxGeometryElementVertexColor* CreateElementVertexColor()
    bool RemoveElementVertexColor(FbxGeometryElementVertexColor* pElementVertexColor)
    FbxGeometryElementVertexColor* GetElementVertexColor(int pIndex = 0)
    const FbxGeometryElementVertexColor* GetElementVertexColor(int pIndex = 0) const
    int GetElementVertexColorCount() const
    FbxGeometryElementSmoothing* CreateElementSmoothing()
    bool RemoveElementSmoothing(FbxGeometryElementSmoothing* pElementSmoothing)
    FbxGeometryElementSmoothing* GetElementSmoothing(int pIndex = 0)
    const FbxGeometryElementSmoothing* GetElementSmoothing(int pIndex = 0) const
    int GetElementSmoothingCount() const
    FbxGeometryElementCrease* CreateElementVertexCrease()
    bool RemoveElementVertexCrease(FbxGeometryElementCrease* pElementCrease)
    FbxGeometryElementCrease* GetElementVertexCrease(int pIndex = 0)
    const FbxGeometryElementCrease* GetElementVertexCrease(int pIndex = 0) const
    int GetElementVertexCreaseCount() const
    FbxGeometryElementCrease* CreateElementEdgeCrease()
    bool RemoveElementEdgeCrease(FbxGeometryElementCrease* pElementCrease)
    FbxGeometryElementCrease* GetElementEdgeCrease(int pIndex = 0)
    const FbxGeometryElementCrease* GetElementEdgeCrease(int pIndex = 0) const
    int GetElementEdgeCreaseCount() const
    FbxGeometryElementHole* CreateElementHole()
    bool RemoveElementHole(FbxGeometryElementHole* pElementHole)
    FbxGeometryElementHole* GetElementHole(int pIndex = 0)
    const FbxGeometryElementHole* GetElementHole(int pIndex = 0) const
    int GetElementHoleCount() const
    FbxGeometryElementUserData* CreateElementUserData()
    bool RemoveElementUserData(FbxGeometryElementUserData* pElementUserData)
    FbxGeometryElementUserData* GetElementUserData(int pIndex = 0)
    const FbxGeometryElementUserData* GetElementUserData(int pIndex = 0) const
    int GetElementUserDataCount() const
    FbxGeometryElementVisibility* CreateElementVisibility()
    bool RemoveElementVisibility(FbxGeometryElementVisibility* pElementVisibility)
    FbxGeometryElementVisibility* GetElementVisibility(int pIndex = 0)
    const FbxGeometryElementVisibility* GetElementVisibility(int pIndex = 0) const
    int GetElementVisibilityCount() const
    FbxGeometryElementUV* CreateElementUV(const char* pUVSetName, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
    bool RemoveElementUV(FbxGeometryElementUV* pElementUV)
    FbxGeometryElementUV* GetElementUV(int pIndex = 0, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eUnknown)
    const FbxGeometryElementUV* GetElementUV(int pIndex = 0, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eUnknown) const
    int GetElementUVCount(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eUnknown) const
    FbxGeometryElementUV* GetElementUV(const char* pUVSetName)
    const FbxGeometryElementUV* GetElementUV(const char* pUVSetName) const
    void GetUVSetNames(FbxStringList& pUVSetNameList) const
        virtual bool ContentWriteTo(FbxStream& pStream) const
        virtual bool ContentReadFrom(const FbxStream& pStream)
        virtual int MemoryUsage() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	virtual void Compact()
    FbxArray<FbxVector4> mControlPoints
    bool GetNormals(FbxLayerElementArrayTemplate<FbxVector4>** pLockableArray) const
    bool GetNormalsIndices(FbxLayerElementArrayTemplate<int>** pLockableArray) const
    bool GetTangents(FbxLayerElementArrayTemplate<FbxVector4>** pLockableArray, const int pLayerIndex = 0) const
    bool GetTangentsIndices(FbxLayerElementArrayTemplate<int>** pLockableArray, const int pLayerIndex = 0) const
    bool GetBinormals(FbxLayerElementArrayTemplate<FbxVector4>** pLockableArray, const int pLayerIndex = 0) const
    bool GetBinormalsIndices(FbxLayerElementArrayTemplate<int>** pLockableArray, const int pLayerIndex = 0) const
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual void ContentClear()
#endif 