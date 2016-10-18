#ifndef _FBXSDK_UTILS_MATERIAL_CONVERTER_H_
#define _FBXSDK_UTILS_MATERIAL_CONVERTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxlayer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class LayerConfig
class FbxMaterialConverter_Impl
class FBXSDK_DLL FbxMaterialConverter
public:
	FbxMaterialConverter( FbxManager& mManager, FbxSurfaceMaterial* pDefaultMaterial = NULL)
	~FbxMaterialConverter()
	bool ConnectTexturesToMaterials( FbxScene& pScene )
	bool ConnectTexturesToMaterials( FbxNode* pNode )
	bool AssignTexturesToLayerElements( FbxScene& pScene )
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
	typedef FbxPair<FbxTexture*, FbxLayerElementTexture::EBlendMode> TexData
	FbxMaterialConverter& operator=(const FbxMaterialConverter&)
	FbxManager& mManager
	FbxSurfaceMaterial* mDefaultMaterial
    FbxMaterialConverter_Impl* mImpl
	void GetTextures( int pComponent, FbxLayer* pLayer, LayerConfig& pLayerConfig ) const
	FbxSurfaceMaterial* GetMaterial( int pComponent, FbxLayer* pLayer, FbxNode* pNode, bool pLookOnNode )
	int                  GetMaterialOrder( int pComponent, FbxLayer* pLayer, FbxNode* pNode, bool pLookOnNode )
	bool HasGoodMappingModes( FbxNode* pNode, FbxGeometry* pGeom ) const
	void ConnectTextures( FbxSurfaceMaterial* pMat, FbxObject* pTexture, int pTextureType ) const
	bool HasPerFaceMaterialMapping( FbxGeometry* pGeom ) const
	void SetTextureUVSets( FbxGeometry* pGeom ) const
	bool HasTextures( FbxGeometry* pGeom ) const
	void GetTextureList( FbxArray<TexData>& pTextures, FbxLayeredTexture* pTex ) const
	FbxLayer* FindLayerForTexture( FbxTexture* pTex, 
								  FbxLayerElement::EType pTexType, 
                                  FbxLayerElementTexture::EBlendMode pTexBlendMode, 
								  FbxGeometry* pGeom, 
								  int lComponentIndex, 
								  int lStartIndex = 0 ) const
	void InitTextureElement( FbxLayerElementTexture* pTexElm, int pComponentCount,
        FbxLayerElementTexture::EBlendMode pMode) const
	bool AssignTexturesToLayerElements( FbxNode* pNode)
	bool HasTextureLayerElements( FbxGeometry& pGeom ) const
	void ConvertToPerFaceMapping( FbxMesh* pGeom ) const
#endif 