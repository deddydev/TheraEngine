#ifndef _FBXSDK_FILEIO_COLLADA_READER_H_
#define _FBXSDK_FILEIO_COLLADA_READER_H_
#include <fbxsdk.h>
#include <fbxsdk/fileio/collada/fbxcolladautils.h>
#include <fbxsdk/fileio/collada/fbxcolladanamespace.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxReaderCollada : public FbxReader 
public:
    FbxReaderCollada(FbxManager& pManager, int pID, FbxStatus& pStatus)
    virtual ~FbxReaderCollada()
    virtual bool FileOpen(char* pFileName)
	virtual bool FileClose()
	virtual bool IsFileOpen()
	virtual bool GetReadOptions(bool pParseFileAsNeeded = true)
 return true
    virtual bool GetAxisInfo(FbxAxisSystem* pAxisSystem, FbxSystemUnit* pSystemUnits)
    virtual FbxArray<FbxTakeInfo*>* GetTakeInfo()
	virtual bool Read(FbxDocument* pDocument)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
	bool ReadCollada(FbxScene &pScene, xmlNode* pXmlNode)
    bool ImportVisualScene(xmlNode* pXmlNode, FbxScene * pScene)
    bool ImportVisualSceneMax3DExtension(xmlNode * pTechniqueElement, FbxScene * pScene)
    bool ImportVisualSceneFCOLLADAExtension(xmlNode * pTechniqueElement, FbxScene * pScene)
    bool ImportVisualSceneMayaExtension(xmlNode * pTechniqueElement, FbxScene * pScene)
    bool ImportAsset(xmlNode* pXmlNode, FbxGlobalSettings & pGlobalSettings, FbxDocumentInfo &pSceneInfo)
    FbxNode * ImportNode(xmlNode* pXmlNode)
    bool ImportNodeFCOLLADAExtension(xmlNode* pTechniqueElement, FbxNode * pNode)
    bool ImportNodeXSIExtension(xmlNode* pTechniqueElement, FbxNode * pNode)
    bool ImportNodeFBXExtension(xmlNode* pTechniqueElement, FbxNode * pNode)
    FbxGeometry * ImportGeometry(const FbxString & pGeometryID, const FbxDynamicArray<FbxString> & pMaterialSequence)
    bool ImportSkin(xmlNode* pSkinElement)
    FbxGeometry * ImportMorph(xmlNode * pMorphElement, const FbxDynamicArray<FbxString> & pMaterialSequence)
    FbxGeometry * ImportController(const FbxString & pControllerID, const FbxDynamicArray<FbxString> & pMaterialSequence)
    FbxCamera * ImportCamera(xmlNode* pXmlNode)
    FbxLight * ImportLight(xmlNode* pXmlNode)
    FbxSurfaceMaterial * ImportMaterial(xmlNode* pXmlNode)
    FbxSurfaceMaterial * ImportEffect(xmlNode* pEffectElement)
    FbxSurfaceMaterial * ImportEffectNVidiaExtension(xmlNode * pEffectElement)
    FbxFileTexture * ImportTexture(xmlNode* pXmlNode)
    FbxFileTexture * ImportImage(xmlNode* pXmlNode)
    FbxGeometry * ImportMesh(xmlNode* pXmlNode, const FbxDynamicArray<FbxString> & pMaterialSequence, FbxArray<FbxObject*>& pObjects)
    bool ImportVertices(xmlNode* pVerticesElement, FbxGeometry * pGeometry)
	bool ImportPolygons(xmlNode* pXmlNode, FbxMesh& pMesh, const FbxDynamicArray<FbxString> & pMaterialSequence)
	bool ImportTransforms(xmlNode* pXmlNode, FbxNode* pNode)
    int ImportRotationElement(xmlNode* pXmlNode, FbxVector4& pRotationVector)
    void SetRotationOrder(FbxNode * pNode, const FbxArray<int> & pRotationOrder)
    bool ImportLookAt(xmlNode* pXmlNode, FbxVector4& lCameraPosition, 
										 FbxVector4& lInterestPosition, FbxVector4& lUpVector,
										 FbxAMatrix& lCameraTransformMatrix)
	bool IsNodeExportable(FbxString lId)
    bool CheckColladaVersion(const FbxString & pVersionString)
	void AddNotificationError( FbxString pError )
	void AddNotificationWarning( FbxString pWarning )
    bool ImportScene(xmlNode * pColladaNode)
    void Preprocess(xmlNode * pColladaElement)
    void BuildUpLibraryMap()
    void BuildUpLibraryMap(xmlNode * pElement, const FbxString & pElementTag)
    bool ConnectMaterialsToNode(FbxNode * pNode, xmlNode * pElement, FbxDynamicArray<FbxString> & pMaterialSequence)
    bool ImportTransparent(xmlNode * pElement, FbxSurfaceLambert * pSurfaceMaterial)
    struct LibraryTypeTraits
        FbxString library_tag
        FbxString element_tag
    FbxObject * GetLibrary(const LibraryTypeTraits & pTypeTraits, const FbxString & pID)
    FbxObject * GetLibrary(const LibraryTypeTraits & pTypeTraits, xmlNode * pElement)
    bool ImportMatrixAnimation(FbxNode * pNode, const FbxString & pAnimationChannelID)
    bool ImportPropertyAnimation(FbxProperty & pProperty, const FbxString & pAnimationChannelID, const char * pChannelName = NULL)
    FbxAnimLayer * GetAnimLayer(const FbxString & pAnimationID)
    double GetLocalUnitConversion(xmlNode * pElement)
    void SetProperty(xmlNode* pPropertyElement, FbxProperty & pProperty)
    void ImportPropertyValue(FbxObject * pObject, const char * pPropertyName,
                             xmlNode * pPropertyValueElement)
    FbxFile*		mFileObject
    FbxString		mFileName
    xmlDocPtr		mXmlDoc
    FbxAnimLayer*	mAnimLayer
    FbxScene*		mScene
    FbxGlobalSettings * mGlobalSettings
    FbxDocumentInfo * mDocumentInfo
    FbxArray<FbxTakeInfo*> mTakeInfo
    xmlNode * mColladaElement
    struct ColladaElementData
        explicit ColladaElementData(xmlNode * pElement = NULL)
            : mColladaElement(pElement), mFBXObject(NULL) 
        xmlNode * mColladaElement
        FbxObject * mFBXObject
		FbxArray<FbxObject*> mFBXObjects
	typedef FbxMap<FbxString, ColladaElementData> ColladaElementMapType
    ColladaElementMapType mColladaElements
    LibraryTypeTraits mEffectTypeTraits
    LibraryTypeTraits mMaterialTypeTraits
    LibraryTypeTraits mImageTypeTraits
    LibraryTypeTraits mGeometryTypeTraits
    LibraryTypeTraits mControllerTypeTraits
    LibraryTypeTraits mLightTypeTraits
    LibraryTypeTraits mCameraTypeTraits
    LibraryTypeTraits mNodeTypeTraits
    LibraryTypeTraits mAnimationTypeTraits
	typedef FbxMap<FbxString, FbxArray<xmlNode*> > AnimationMapType
    AnimationMapType mAnimationElements
    SourceElementMapType mSourceElements
	struct AnimationClipData
		AnimationClipData(const FbxString & pID) : mID(pID), mAnimLayer(NULL) 
		AnimationClipData(const AnimationClipData& pOther)
 *this = pOther
		AnimationClipData& operator=(const AnimationClipData& pOther)
 mID = pOther.mID
 mAnimationElementIDs = pOther.mAnimationElementIDs
 mAnimLayer = pOther.mAnimLayer
 return *this
		FbxString mID
		FbxSet<FbxString> mAnimationElementIDs
		FbxAnimLayer * mAnimLayer
	FbxDynamicArray<AnimationClipData> mAnimationClipData
    SkinMapType mSkinElements
    typedef FbxMap<FbxString, FbxNode *> NodeMapType
    NodeMapType mIDNamespaceNodes
    NodeMapType mSIDNamespaceNodes
	typedef FbxMap<FbxNode *, FbxString> TargetIDMapType
    TargetIDMapType mTargetIDs
    FbxColladaNamespace mNamespace
#endif 