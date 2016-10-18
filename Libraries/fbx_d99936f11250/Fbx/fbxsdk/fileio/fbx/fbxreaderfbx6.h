#ifndef _FBXSDK_FILEIO_FBX_READER_FBX6_H_
#define _FBXSDK_FILEIO_FBX_READER_FBX6_H_
#include <fbxsdk.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxAnimStack
class FbxAnimLayer
class Fbx6ObjectTypeInfo
class Fbx6TypeReadReferences
class Fbx6ClassTemplateMap
public:
    Fbx6ClassTemplateMap()
    ~Fbx6ClassTemplateMap()
    bool AddClassId( FbxClassId pId, FbxObject* pTemplateObject )
    bool MergeWithTemplate( FbxObject* pObject ) const
    void Clear()
private:
    typedef FbxMap< FbxClassId, FbxObject*, FbxClassIdCompare > MapType
    MapType mClassMap
    bool HasModifiedFlags(FbxProperty lProp) const
    inline FbxPropertyFlags::EFlags IndexToFlag( int i ) const 
 return static_cast<FbxPropertyFlags::EFlags>(1 << i)
class FbxReaderFbx6 : public FbxReader
public:
    FbxReaderFbx6(FbxManager& pManager, FbxImporter& pImporter, int pID, FbxStatus& pStatus)
    virtual ~FbxReaderFbx6()
    virtual bool FileOpen(char* pFileName, EFileOpenSpecialFlags pFlags)
    virtual bool FileOpen(char* pFileName)
    virtual bool FileOpen(FbxFile * pFile)
	virtual bool FileOpen(FbxStream * pStream, void* pStreamData)
    virtual bool FileClose()
    virtual bool IsFileOpen()
    typedef enum
        eASCII,     
        eENCRYPTED  
    EImportMode GetImportMode()
    virtual void GetVersion(int& pMajor, int& pMinor, int& pRevision)
    virtual bool GetAxisInfo(FbxAxisSystem* pAxisSystem, FbxSystemUnit* pSystemUnits)
	virtual bool GetFrameRate(FbxTime::EMode &pTimeMode)
    virtual bool GetStatistics(FbxStatistics* pStats)
    virtual bool GetReadOptions(bool pParseFileAsNeeded = true)
    virtual bool Read(FbxDocument *pDocument)
    virtual bool GetReadOptions(FbxIO* pFbx, bool pParseFileAsNeeded = true)
    virtual bool Read(FbxDocument *pDocument, FbxIO* pFbx)
	virtual void PluginReadParameters(FbxObject& pParams)
    virtual FbxDocumentInfo*  GetSceneInfo() 
 return mSceneInfo
    virtual FbxArray<FbxTakeInfo*>* GetTakeInfo() 
 return &mTakeInfo
    virtual void SetProgressHandler(FbxProgress *pProgress)
	virtual void SetEmbeddingExtractionFolder(const char* pExtractFolder)
	virtual bool SupportsStreams() const 
 return true
private:
    FbxDocumentInfo* ReadSceneInfo()
    FbxDocumentInfo* ReadSceneInfo(FbxString& pType)
    void WriteSceneInfo(FbxDocumentInfo*)
    bool WriteThumbnail(FbxThumbnail*)
    FbxObject* CreateGenericObject(FbxDocument *pDocument, char* pObjectType, char* pObjectSubType, char* pObjectName, FbxObject::EObjectFlag pFlags=FbxObject::eSavable)
    bool ReadDescriptionSection(FbxDocument *pDocument, FbxString& pDocumentName)
    bool ReadReferenceSection(FbxDocument *pDocument, Fbx6TypeReadReferences& pDocReferences)
    bool ReadDefinitionSection(FbxDocument *pDocument, FbxArray<Fbx6ObjectTypeInfo*>& pObjectContent )
    bool ReadObjectSection(FbxDocument *pDocument, FbxArray<Fbx6ObjectTypeInfo*>& pObjectContent, Fbx6TypeReadReferences& pDocReferences )
    bool ReadObject(FbxDocument *pDocument, FbxString& pObjectType, FbxString& pObjectSubType, FbxString& pObjectName, FbxString& pObjectUniqueId, FbxObject* pReferencedObject, Fbx6TypeReadReferences& pDocReferences)
    bool ReadConnectionSection(FbxDocument *pDocument )
    bool ReadDocumentAnimation(FbxDocument *pDocument)
    void ReadObjectAnimation(FbxIO& pFileObject, FbxObject* pNode, FbxAnimStack& pAnimStack, int pExceptionFlag)
    void ReadPropertyAnimation(FbxIO& pFileObject, FbxProperty* pProp, FbxAnimStack& pAnimStack)
    bool ReadTakeAnimation(FbxScene& pScene, FbxTakeInfo* pTakeInfo)
    bool ReadNodeAnimation(FbxIO& pFileObject, FbxScene& pScene, FbxAnimStack& pAnimStack, FbxTakeInfo* pTakeInfo)
    void ReadLayers(FbxIO& pFileObject, FbxTakeInfo* pTakeInfo)
    void ReadTimeWarps(FbxIO& pFileObject, FbxMultiMap& pTimeWarpSet, FbxScene& pScene)
    FbxThumbnail* ReadThumbnail()
    bool TimeShiftNodeAnimation(FbxScene& pScene, FbxAnimStack& pAnimStack, int pTimeOffsetType, FbxTime pTimeOffset)
    void ReadCameraSwitcher(FbxScene& pScene)
    bool ReadCameraSwitcher( FbxCameraSwitcher& pCameraSwitcher )
    void ReorderCameraSwitcherIndices(FbxScene& pScene)
    void ReadGlobalLightSettings(FbxScene& pScene)
    void ReadGlobalTimeSettings(FbxScene& pScene)
    void ReadGlobalCameraSettings(FbxScene& pScene)
    void ReadShadowPlane(FbxScene& pScene)
    void ReadAmbientColor(FbxScene& pScene)
    void ReadFogOption(FbxScene& pScene)
    void ReadCharacter(FbxCharacter& pCharacter,int& pInputType, int& pInputIndex)
    void ReadCharacterLinkGroup(FbxCharacter& pCharacter, int pCharacterGroupId)
    void ReadCharacterLink(FbxCharacter& pCharacter, int pCharacterNodeId)
    void ReadCharacterLinkRotationSpace(FbxCharacterLink& pCharacterLink)
    bool ReadCharacterPose(FbxCharacterPose& pCharacterPose)
		bool ReadPose(FbxScene& pScene, FbxPose* pPose, bool pAsBindPose)
		bool ReadMedia(FbxDocument *pDocument, const char* pEmbeddedMediaDirectory = "")
		bool ReadGlobalSettings(FbxGlobalSettings& pGlobalSettings)
    bool ReadNode                       ( FbxNode& pNode, FbxString& pObjectSubType, Fbx6TypeReadReferences& pDocReferences )
	bool ReadContainer					( FbxContainer& pContainer )
    bool ReadGenericNode                ( FbxGenericNode& pNode )
    bool ReadNodeShading                ( FbxNode& pNode )
    bool ReadNodeCullingType            ( FbxNode& pNode )
    bool ReadNodeTarget                 ( FbxNode& pNode )
    bool ReadNodeAttribute              ( FbxNode& pNode , FbxString& pObjectSubType, bool& pCreatedAttribute, Fbx6TypeReadReferences& pDocReferences)
    FbxNodeAttribute* ReadNodeAttribute( FbxString& pObjectSubType, FbxString& pObjectName, FbxString& pObjectUniqueId, FbxObject* pReferencedObject)
    bool ReadNodeProperties             ( FbxNode& pNode, bool pReadNodeAttributeProperties )
    bool ReadLayeredTexture             ( FbxLayeredTexture& pTex )
    bool ReadGeometryLinks              ( FbxGeometry& pGeometry )
    bool ReadGeometryShapes             ( FbxGeometry& pGeometry )
    bool ReadNull                       ( FbxNull& pNull )
    bool ReadMarker                     ( FbxMarker& pMarker )
    bool ReadCamera                     ( FbxCamera& pCamera )
    bool ReadCameraStereo               ( FbxCameraStereo& pCameraStereo )
    bool ReadCameraStereoPrecomp        (FbxCameraStereo& pCameraStereo)
    bool ReadLight                      ( FbxLight& pLight )
    bool ReadBindingTable               ( FbxBindingTable& pTable )
    bool ReadBindingOperator            ( FbxBindingOperator& pOperator )
    bool ReadMesh                       ( FbxMesh& pMesh )
    bool ReadMeshSmoothness               ( FbxMesh& pMesh )
    bool ReadMeshVertices               ( FbxMesh& pMesh )
    bool ReadMeshPolygonIndex           ( FbxMesh& pMesh )
    bool ReadMeshEdges                  ( FbxMesh& pMesh )
    /
    bool ReadSubdiv( FbxSubDiv& pSubdiv)
    bool ReadDocument                   ( FbxDocument& pSubDocument )
    bool ReadCollection                 ( FbxCollection& pCollection )
    bool ReadSelectionSet                ( FbxSelectionSet& pSelectionSet)
    bool ReadSelectionNode             (FbxSelectionNode& pSelectionNode)
    bool ReadNurb                       ( FbxNurbs& pNurbs )
    bool ReadNurbsSurface               ( FbxNurbsSurface& pNurbs )
    bool ReadPatch                      ( FbxPatch& pPatch )
    int  ReadPatchType                  ( FbxPatch& pPatch )
    bool ReadNurbsCurve                 ( FbxNurbsCurve& pNurbsCurve )
    bool ReadTrimNurbsSurface           ( FbxTrimNurbsSurface& pNurbs )
    bool ReadBoundary                   ( FbxBoundary& pBoundary )
    bool ReadShape                      ( FbxShape& pShape, FbxGeometry& pGeometry)
    bool ReadImplementation             ( FbxImplementation& pImplementation )
    bool ReadFileTexture                    (FbxFileTexture& pTexture)
    FbxSurfaceMaterial* ReadSurfaceMaterial(const char* pName, const char* pMaterialType, FbxSurfaceMaterial* pReferencedMaterial)
    bool ReadVideo                      (FbxVideo& pVideo)
    bool ReadThumbnail                  (FbxThumbnail& pThumbnail)
    bool ReadLayerElements              (FbxGeometry& pGeometry)
    bool ReadLayerElementsMaterial      (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsMaterial)
    bool ReadLayerElementsNormal        (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsNormal)
    bool ReadLayerElementsTangent        (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsTangent)
    bool ReadLayerElementsBinormal        (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsBinormal)
    bool ReadLayerElementsVertexColor   (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsVertexColor)
    bool ReadLayerElementsTexture (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsTexture, FbxLayerElement::EType pTextureType)
    bool ReadLayerElementsChannelUV (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsUV, FbxLayerElement::EType pTextureType)
    bool ReadLayerElementsPolygonGroup  (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsPolygonGroup)
    bool ReadLayerElementsSmoothing     (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsSmoothing)
    bool ReadLayerElementsUserData      (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsUserData)
    bool ReadLayerElementsVisibility    (FbxGeometry* pGeometry, FbxArray<FbxLayerElement*>& pElementsVisibility)
    bool ReadLayerElementEdgeCrease     (FbxGeometry*pGeometry, FbxArray<FbxLayerElement*>& pElementsEdgeCrease)
    bool ReadLayerElementVertexCrease   (FbxGeometry*pGeometry, FbxArray<FbxLayerElement*>& pElementsVertexCrease)
    bool ReadLayerElementHole           (FbxGeometry*pGeometry, FbxArray<FbxLayerElement*>& pElementsHole)
    bool ReadGeometryWeightedMap(FbxGeometryWeightedMap& pGeometryWeightedMap)
    bool ReadLink(FbxCluster& pLink)
    bool ReadSkin(FbxSkin& pSkin)
    bool ReadVertexCacheDeformer(FbxVertexCacheDeformer& pDeformer)
    bool ReadCluster(FbxCluster& pCluster)
    bool ReadConstraint(FbxConstraint& pPosition)
    bool ReadCache(FbxCache& pCache)
	bool ResolveCameraBackgrounds(FbxScene& pScene)
    void RemoveDuplicateTextures(FbxScene& pScene)
    void ReplaceTextures(FbxArray<FbxTexture*> const& pTextureDuplicate,
                         FbxArray<FbxTexture*> const& pTextureReplacement,
                         FbxGeometry* pGeometry, FbxLayerElement::EType pTextureType)
    void RemoveDuplicateMaterials(FbxScene& pScene)
    FbxString ConvertCameraName(FbxString pCameraName)
    int  FindString(FbxString pString, FbxArray<FbxString*>& pStringArray)
    bool ReadPassword(FbxString pPassword)
    void PublishProperties(FbxObject& pObject)
    bool ReadProperties(FbxObject *pFbxObject, FbxIO *pFbxFileObject, bool pReadNodeAttributeProperties=true)
    bool ReadPropertiesAndFlags(FbxObject *pFbxObject, FbxIO *pFbxFileObject, bool pReadNodeAttributeProperties=true)
    bool ReadFlags(FbxObject *pFbxObject, FbxIO* pFbxFileObject)
    void RebuildTrimRegions(FbxScene& pScene) const
    void SetSubdivision(FbxScene& pScene) const
	void ConvertShapeDeformProperty(FbxScene& pScene) const
    void RebuildLayeredTextureAlphas(FbxScene& pScene) const
    void ReadOptionsInMainSection()
    void ReadTakeOptions()
    bool ReadOptionsInExtensionSection(int& pSectionIndex)
    bool WriteOptionsInExtensionSection(bool pOverwriteLastExtensionSection=false)
    void ReadGlobalSettingsInMainSection()
    void ReadDefinitionSectionForStats()
private:
    FbxReaderFbx6& operator=(FbxReaderFbx6 const&) 
 return *this
    FbxIO*                  mFileObject
    FbxImporter&            mImporter
    FbxCharPtrSet           mNodeArrayName
    FbxObjectStringMap      mObjectMap
    bool					mParseGlobalSettings
    FbxAxisSystem			mAxisSystem
    FbxSystemUnit			mSystemUnit
	FbxTime::EMode			mFrameRate
    bool					mRetrieveStats
    FbxStatistics*	        mDefinitionsStatistics
    FbxArray<FbxTakeInfo *> mTakeInfo
    FbxDocumentInfo*        mSceneInfo
    FbxAnimLayer*           mAnimLayer
	FbxMultiMap					mNickToKFCurveNodeTimeWarpsSet
	FbxMultiMap*					mNickToAnimCurveTimeWarpsSet
    Fbx6ClassTemplateMap    mClassTemplateMap
    FbxProgress*            mProgress
    bool                    mProgressPause
#include <fbxsdk/fbxsdk_nsend.h>
#endif 