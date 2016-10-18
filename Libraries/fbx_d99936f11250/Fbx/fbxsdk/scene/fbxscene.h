#ifndef _FBXSDK_SCENE_H_
#define _FBXSDK_SCENE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxmultimap.h>
#include <fbxsdk/core/base/fbxcharptrset.h>
#include <fbxsdk/scene/fbxdocument.h>
#include <fbxsdk/scene/animation/fbxanimevaluator.h>
#include <fbxsdk/scene/geometry/fbxlayer.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fileio/fbxiosettings.h>
#include <fbxsdk/fileio/fbxglobalsettings.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxGeometry
class FbxTexture
class FbxSurfaceMaterial
class FbxCharacter
class FbxControlSetPlug
class FbxGenericNode
class FbxPose
class FbxCharacterPose
class FbxVideo
class FbxGlobalLightSettings
class FbxGlobalCameraSettings
class FBXSDK_DLL FbxScene : public FbxDocument
	FBXSDK_OBJECT_DECLARE(FbxScene, FbxDocument)
public:
		void Clear()
		FbxNode* GetRootNode() const
		void FillTextureArray(FbxArray<FbxTexture*>& pTextureArray)
		void FillMaterialArray(FbxArray<FbxSurfaceMaterial*>& pMaterialArray)
		int GetGenericNodeCount() const
		FbxGenericNode* GetGenericNode(int pIndex)
		FbxGenericNode* GetGenericNode(char* pName)
		bool AddGenericNode(FbxGenericNode* pGenericNode)
		bool RemoveGenericNode(FbxGenericNode* pGenericNode)
		int GetCharacterCount() const
		FbxCharacter* GetCharacter(int pIndex)
		int CreateCharacter(const char* pName)
		void DestroyCharacter(int pIndex)
		int GetControlSetPlugCount() const
		FbxControlSetPlug* GetControlSetPlug(int pIndex)
		int CreateControlSetPlug(char* pName)
		void DestroyControlSetPlug(int pIndex)
		int GetCharacterPoseCount() const
		FbxCharacterPose* GetCharacterPose(int pIndex)
		int CreateCharacterPose(char* pName)
		void DestroyCharacterPose(int pIndex)
		int GetPoseCount() const
		FbxPose* GetPose(int pIndex)
		bool AddPose(FbxPose* pPose)
		bool RemovePose(FbxPose* pPose)
		bool RemovePose(int pIndex)
		FbxDocumentInfo* GetSceneInfo() 
 return GetDocumentInfo()
		void SetSceneInfo(FbxDocumentInfo* pSceneInfo) 
 SetDocumentInfo(pSceneInfo)
		FbxGlobalSettings& GetGlobalSettings()
		const FbxGlobalSettings& GetGlobalSettings() const
		void SetCurrentAnimationStack(FbxAnimStack* pAnimStack)
		FbxAnimStack* GetCurrentAnimationStack()
		void SetAnimationEvaluator(FbxAnimEvaluator* pEvaluator)
		FbxAnimEvaluator* GetAnimationEvaluator()
	void FillPoseArray(FbxArray<FbxPose*>& pPoseArray)
		int GetMaterialCount() const
		FbxSurfaceMaterial* GetMaterial(int pIndex)
		FbxSurfaceMaterial* GetMaterial(char* pName)
		bool AddMaterial(FbxSurfaceMaterial* pMaterial)
		bool RemoveMaterial(FbxSurfaceMaterial* pMaterial)
		int GetTextureCount() const
		FbxTexture* GetTexture(int pIndex)
		FbxTexture* GetTexture(char* pName)
		bool AddTexture(FbxTexture* pTexture)
		bool RemoveTexture(FbxTexture* pTexture)
		int GetNodeCount() const
		FbxNode* GetNode(int pIndex)
		bool AddNode(FbxNode* pNode)
		bool RemoveNode(FbxNode* pNode)
		int GetCurveOnSurfaceCount()
		FbxNode* FindNodeByName(const FbxString& pName)
		int GetGeometryCount() const
		FbxGeometry* GetGeometry(int pIndex)
		bool AddGeometry(FbxGeometry* pGeometry)
		bool RemoveGeometry(FbxGeometry* pGeometry)
		int GetVideoCount() const
		FbxVideo* GetVideo(int pIndex)
		bool AddVideo(FbxVideo* pVideo)
		bool RemoveVideo(FbxVideo* pVideo)
		void SyncShowPropertyForInstance()
		bool ComputeBoundingBoxMinMaxCenter(FbxVector4& pBBoxMin, FbxVector4& pBBoxMax, FbxVector4& pBBoxCenter, bool pSelected=false, const FbxTime& pTime=FBXSDK_TIME_INFINITE)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	void ConvertNurbsSurfaceToNurbs()
	void ConvertMeshNormals()
	void ConvertNurbsCurvesToNulls()
	void ConnectTextures()
	void BuildTextureLayersDirectArray()
	void FixInheritType(FbxNode *pNode)
	void UpdateScaleCompensate(FbxNode *pNode, FbxIOSettings& pIOS)
	FbxClassId ConvertAttributeTypeToClassID(FbxNodeAttribute::EType pAttributeType)
	FbxGlobalLightSettings&  GlobalLightSettings()  
 return *mGlobalLightSettings
	FbxGlobalCameraSettings& GlobalCameraSettings() 
 return *mGlobalCameraSettings
	virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
	virtual FbxObject& Copy(const FbxObject& pObject)
	void ConnectMaterials()
	void BuildMaterialLayersDirectArray()
	void ReindexMaterialConnections()
	FbxMultiMap* AddTakeTimeWarpSet(char *pTakeName)
	FbxMultiMap* GetTakeTimeWarpSet(char *pTakeName)
	void ForceKill()
private:
	virtual void Construct(const FbxObject* pFrom)
	virtual void Destruct(bool pRecursive)
	void ConnectTextureLayerElement(FbxLayerContainer* pLayerContainer, FbxLayerElement::EType pLayerType, FbxNode* pParentNode)
	void BuildTextureLayersDirectArrayForLayerType(FbxLayerContainer* pLayerContainer, FbxLayerElement::EType pLayerType)
	FbxNode*					mRootNode
	FbxGlobalLightSettings*		mGlobalLightSettings
	FbxGlobalCameraSettings*	mGlobalCameraSettings
	FbxAnimEvaluator*			mAnimationEvaluator
	FbxAnimStack*				mCurrentAnimationStack
	FbxCharPtrSet				mTakeTimeWarpSet
#endif 