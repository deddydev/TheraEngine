#ifndef _FBXSDK_FILEIO_COLLADA_WRITER_H_
#define _FBXSDK_FILEIO_COLLADA_WRITER_H_
#include <fbxsdk.h>
#include <fbxsdk/fileio/collada/fbxcolladautils.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxWriterCollada : public FbxWriter 
public:
    FbxWriterCollada(FbxManager& pManager, int pID, FbxStatus& pStatus)
    virtual ~FbxWriterCollada()
    virtual bool FileCreate(char* pFileName)
    virtual bool FileClose()
    virtual bool IsFileOpen()
	virtual void GetWriteOptions()
    virtual bool Write(FbxDocument* pDocument)
    virtual bool PreprocessScene(FbxScene &pScene)
    virtual bool PostprocessScene(FbxScene &pScene)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    xmlNode*	ExportAsset(xmlNode* pXmlNode, FbxDocumentInfo* pSceneInfo)
    xmlNode * ExportScene(FbxScene* pScene)
    bool		ExportLibraries(xmlNode* pXmlNode)
    xmlNode* ExportNodeRecursive(xmlNode* pXmlNode, const FbxNode* pNode)
    void ExportVisualSceneMAX3DExtension(xmlNode * pExtraElement, FbxScene * pScene)
    void ExportVisualSceneFCOLLADAExtension(xmlNode * pExtraElement, FbxScene * pScene)
    xmlNode* ExportNode(xmlNode* pXmlNode, const FbxNode* pNode)
    bool ExportTransform(xmlNode* pXmlNode, const FbxNode* pNode)
    bool ExportNodeAttribute(xmlNode* pXmlNode, const FbxNode* pNode)
    xmlNode* CreateMeshLibrary(const FbxNode* pNode)
    xmlNode* CreateCameraLibrary(const FbxNode* pNode)
    xmlNode* CreateLightLibrary(const FbxNode* pNode)
    xmlNode* ExportMesh(const FbxNode* pNode)
    xmlNode*	ExportShapeGeometry(FbxMesh* pMeshShape, FbxString pShapeId)
    xmlNode*	ExportVertexPositions(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pMeshName, bool pInGeometry, bool pExportControlPoints)
    xmlNode*	ExportLayerElements(xmlNode* pXmlMesh, FbxMesh* pMesh, FbxString pName)
    xmlNode*	ExportNormals(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pName, FbxString pExt, int pLayerIndex)
    xmlNode*	ExportUVs(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pName, int pLayerIndex)
    xmlNode*	ExportVertexColors(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pName, int pLayerIndex)
    xmlNode*	ExportVertices(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pName)
    xmlNode* ExportPolygons(xmlNode* pMeshElement, FbxMesh* pMesh, FbxString pMaterialName, int pMaterialIndexInNode, FbxString pName, bool pShape = false)
    bool		ExportMeshMaterials(FbxMesh *pMesh, int pNbMat)
    xmlNode*	ExportMaterial(FbxSurfaceMaterial *pMaterial)
    xmlNode*	ExportEffect(FbxSurfaceMaterial *pMaterial, FbxString pEffectId)
    bool		AddMaterialTextureInput(xmlNode *pXmlMaterial, FbxFileTexture *pTexture, FbxString pImageId, int pLayerIndex, int pLayerElementType)
    xmlNode*	ExportTexture(FbxFileTexture *pTexture, FbxString pImageId, int pLayerIndex)
    bool		ExportMeshTextures(FbxMesh *pMesh)
    xmlNode* ExportCamera(const FbxNode* pNode)
    xmlNode* ExportLight(const FbxNode* pNode)
    void ExportSceneAmbient(xmlNode * pVisualSceneElement)
    bool		ExportController(FbxMesh *pMesh)
    bool		ExportControllerShape(FbxMesh *pMesh)
    xmlNode*	ExportJointWeights(xmlNode* pXmlNode, FbxMesh* pMesh, FbxString pExt)
    bool		UpdateMeshLibraryWithShapes(xmlNode* pXmlNode)
    bool		ExportAnimation(FbxNode* pNode)
    bool		ExportAnimationCurves(FbxNode* pNode, xmlNode* pAnimationNode)
    const FbxString ExportImage(FbxFileTexture * pTexture)
    bool		ExportCurve(xmlNode* pAnimationNode, FbxAnimCurve* pCurve,
        const char* pChannelName, const char* pSubChannelName,
        bool pExportShape=false, bool pExportIntensity=false, bool pExportLib=false)
    bool		NotZero(FbxVector4 pV)
    bool		NotValue(FbxVector4 pV, double pValue)
    bool		NotZero(double pD)
    bool IsTranslationAnimated(const FbxNode *pNode)
    bool IsRotationAnimated(const FbxNode *pNode)
    bool IsRotationAnimated(const FbxNode *pNode, int pAxis)
    bool IsScaleAnimated(const FbxNode *pNode)
    void		CopyMesh(FbxMesh *lNewMesh, FbxMesh *lRefMesh)
    void		ConvertFocalLengthCurveToFOV(FbxAnimCurve *pFOVCurve, FbxAnimCurve *pFLCurve, FbxCamera *pCamera)
    void		PreprocessNodeRecursive(FbxNode* pNode)
    void ExportPropertyValue(const FbxProperty & pProperty,
                             xmlNode * pParentElement)
    void AddNotificationError( FbxString pError )
    void AddNotificationWarning( FbxString pWarning )
    FbxFile* mFileObject
    FbxString mFileName
    bool mStatus
    xmlDocPtr mXmlDoc
    FbxScene*	mScene
    FbxAnimStack* mAnimStack
    FbxAnimLayer* mAnimLayer
    xmlNode*	mLibraryAnimation
    xmlNode*	mLibraryCamera
    xmlNode*	mLibraryController
    xmlNode*	mLibraryGeometry
    xmlNode*	mLibraryImage
    xmlNode*	mLibraryLight
    xmlNode*	mLibraryMaterial
    xmlNode*	mLibraryEffect
    xmlNode*	mLibraryTexture
    xmlNode*	mLibraryVisualScene
    FbxStringList	*mShapeMeshesList
    bool mTriangulate
    bool mSingleMatrix
    FbxTime mSamplingPeriod
#endif 