#ifndef _FBXSDK_SCENE_GEOMETRY_H_
#define _FBXSDK_SCENE_GEOMETRY_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxgeometrybase.h>
#include <fbxsdk/scene/geometry/fbxdeformer.h>
#include <fbxsdk/scene/geometry/fbxshape.h>
#include <fbxsdk/scene/geometry/fbxblendshape.h>
#include <fbxsdk/scene/geometry/fbxblendshapechannel.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxStatus
class FbxGeometryWeightedMap
class FBXSDK_DLL FbxGeometry : public FbxGeometryBase
    FBXSDK_OBJECT_DECLARE(FbxGeometry,FbxGeometryBase)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    int AddDeformer(FbxDeformer* pDeformer)
	FbxDeformer* RemoveDeformer(int pIndex, FbxStatus* pStatus = NULL)
    int GetDeformerCount() const
    FbxDeformer* GetDeformer(int pIndex, FbxStatus* pStatus = NULL) const
    int GetDeformerCount(FbxDeformer::EDeformerType pType) const
    FbxDeformer* GetDeformer(int pIndex, FbxDeformer::EDeformerType pType, FbxStatus* pStatus = NULL) const
    FbxGeometryWeightedMap* GetSourceGeometryWeightedMap()
    int GetDestinationGeometryWeightedMapCount() const
    FbxGeometryWeightedMap* GetDestinationGeometryWeightedMap(int pIndex)
	bool AddShape(int pBlendShapeIndex, int pBlendShapeChannelIndex, FbxShape* pShape, double pPercent = 100, FbxStatus* pStatus = NULL)
	void ClearShape()
	int GetShapeCount() const
	int GetShapeCount(int pBlendShapeIndex, int pBlendShapeChannelIndex, FbxStatus* pStatus = NULL) const
	FbxShape* GetShape(int pBlendShapeIndex, int pBlendShapeChannelIndex, int pShapeIndex, FbxStatus* pStatus = NULL)
	const FbxShape* GetShape(int pBlendShapeIndex, int pBlendShapeChannelIndex, int pShapeIndex, FbxStatus* pStatus = NULL) const
	FbxAnimCurve* GetShapeChannel(int pBlendShapeIndex, int pBlendShapeChannelIndex, FbxAnimLayer* pLayer, bool pCreateAsNeeded = false, FbxStatus* pStatus = NULL)
    enum ESurfaceMode
        eRaw,			
        eLowNoNormals,	
        eLow,			
		eHighNoNormals,	
        eHigh			
    FbxAMatrix& GetPivot(FbxAMatrix& pXMatrix) const
    void SetPivot(FbxAMatrix& pXMatrix)
    void ApplyPivot()
	void SetDefaultShape(int pBlendShapeIndex, int pBlendShapeChannelIndex, double pPercent)
	void SetDefaultShape(FbxBlendShapeChannel* pBlendShapeChannel,  double pPercent)
	double GetDefaultShape(int pBlendShapeIndex, int pBlendShapeChannelIndex) const
	double GetDefaultShape(FbxBlendShapeChannel* pBlendShapeChannel) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
    virtual FbxObject* Clone(FbxObject::ECloneType pCloneType=eDeepClone, FbxObject* pContainer=NULL, void* pSet = NULL) const
    void CleanShapeChannels(FbxAnimLayer* pAnimLayer)
    void CleanShapeChannel(FbxAnimLayer* pAnimLayer, int pShapeIndex)
	void CreateShapeChannelProperties(FbxString& pShapeName)
    void ConvertShapeNamesToV5Format(FbxString pTakeNodeName)
    void ConvertShapeNamesToV5Format(FbxString pTakeNodeName, int pShapeIndex)
    void RevertShapeNamesToV6Format(FbxString pTakeNodeName)
    void RevertShapeNamesToV6Format(FbxString pTakeNodeName, int pShapeIndex)
    void ClearTemporaryShapeNames()
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    virtual void SetDocument(FbxDocument* pDocument)
    FbxString CreateShapeChannelName(int pShapeIndex)
    FbxString CreateShapeChannelName(FbxString pShapeName)
    void CopyDeformers(const FbxGeometry* pGeometry)
    void CopyPivot(const FbxGeometry* pSource)
    FbxArray<FbxString*> mShapeNameArrayV6
    FbxArray<FbxString*> mShapeNameArrayV5
    FbxArray<FbxString*> mShapeChannelNameArrayV5
    FbxAMatrix* mPivot
#endif 