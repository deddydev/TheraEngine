#ifndef _FBXSDK_SCENE_GEOMETRY_LAYER_CONTAINER_H_
#define _FBXSDK_SCENE_GEOMETRY_LAYER_CONTAINER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/scene/geometry/fbxlayer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxLayerContainer : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxLayerContainer,FbxNodeAttribute)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
	int CreateLayer()
    void ClearLayers()
	int GetLayerCount() const
	int GetLayerCount(FbxLayerElement::EType pType,  bool pUVCount=false) const
	FbxLayer* GetLayer(int pIndex)
	const FbxLayer* GetLayer(int pIndex) const
	FbxLayer* GetLayer(int pIndex, FbxLayerElement::EType pType, bool pIsUV=false)
	const FbxLayer* GetLayer(int pIndex, FbxLayerElement::EType pType, bool pIsUV=false) const
	int GetLayerIndex(int pIndex, FbxLayerElement::EType pType, bool pIsUV=false) const
	int GetLayerTypedIndex(int pGlobalIndex, FbxLayerElement::EType pType, bool pIsUV=false) const
	bool ConvertDirectToIndexToDirect(int pLayer)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	int  GTC(FbxUInt i, int j)
	void* GT (int  i,    FbxUInt l, int j)
	int  AT (void* t,    FbxUInt l, int j)
	int  GTI(const char* n, FbxUInt l, int j)
	int  GMC(FbxUInt i, void* n = NULL)
	void* GM (int  i,    FbxUInt l, void* n = NULL)
	int  AM (void* m,    FbxUInt l, void* n = NULL, bool b = false)
	int  GMI(const char* n, FbxUInt l, void* d = NULL)
	int AddToLayerElementsList(FbxLayerElement* pLEl)
	void RemoveFromLayerElementsList(FbxLayerElement* pLEl)
protected:
	virtual void Destruct(bool pRecursive)
	void CopyLayers(const FbxLayerContainer* pLayerContainer)
	virtual void SetDocument(FbxDocument* pDocument)
	virtual	bool ConnectNotify (FbxConnectEvent const &pEvent)
	FbxArray<FbxLayer*> mLayerArray
	FbxArray<FbxLayerElement*> mLayerElementsList
#endif 