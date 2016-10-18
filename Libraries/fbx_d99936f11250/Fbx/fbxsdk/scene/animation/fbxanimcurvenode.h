#ifndef _FBXSDK_SCENE_ANIMATION_CURVE_NODE_H_
#define _FBXSDK_SCENE_ANIMATION_CURVE_NODE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
#define FBXSDK_CURVENODE_TRANSFORM		"Transform"
#define FBXSDK_CURVENODE_TRANSLATION	"T"
#define FBXSDK_CURVENODE_ROTATION		"R"
#define FBXSDK_CURVENODE_SCALING		"S"
#define FBXSDK_CURVENODE_COMPONENT_X	"X"
#define FBXSDK_CURVENODE_COMPONENT_Y	"Y"
#define FBXSDK_CURVENODE_COMPONENT_Z	"Z"
#define FBXSDK_CURVENODE_COLOR			"Color"
#define FBXSDK_CURVENODE_COLOR_RED		FBXSDK_CURVENODE_COMPONENT_X
#define FBXSDK_CURVENODE_COLOR_GREEN	FBXSDK_CURVENODE_COMPONENT_Y
#define FBXSDK_CURVENODE_COLOR_BLUE		FBXSDK_CURVENODE_COMPONENT_Z
class FbxAnimStack
class FbxAnimCurve
class FbxMultiMap
class KFCurveNode
class FBXSDK_DLL FbxAnimCurveNode : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxAnimCurveNode, FbxObject)
public:
        bool IsAnimated(bool pRecurse=false) const
        bool GetAnimationInterval(FbxTimeSpan& pTimeInterval) const
        bool IsComposite() const
        FbxAnimCurveNode* Find(const char* pName)
        static FbxAnimCurveNode* CreateTypedCurveNode(FbxProperty& pProperty, FbxScene* pScene)
        unsigned int GetChannelsCount() const
        int GetChannelIndex(const char* pChannelName) const
        FbxString GetChannelName(int pChannelId) const
        void ResetChannels()
        template <class T> bool AddChannel(const char* pChnlName, T const &pValue)
            if (!pChnlName || strlen(pChnlName)==0) return false
            FbxProperty c = GetChannel(pChnlName)
            if (c.IsValid()) 
                return false
            mChannels.BeginCreateOrFindProperty()
            FbxDataType dt = FbxGetDataTypeFromEnum(FbxTypeOf(pValue))
            c = FbxProperty::Create(mChannels, dt, pChnlName)
            c.Set(pValue)
            mChannels.EndCreateOrFindProperty()
            return true
        template <class T> void SetChannelValue(const char* pChnlName, T pValue)
            FbxProperty c = GetChannel(pChnlName)
            if( c.IsValid() ) c.Set(pValue)
        template <class T> void SetChannelValue(unsigned int pChnlId, T pValue)
            FbxProperty c = GetChannel(pChnlId)
            if( c.IsValid() ) c.Set(pValue)
        template <class T> T GetChannelValue(const char* pChnlName, T pInitVal)
            T v = pInitVal
            FbxProperty c = GetChannel(pChnlName)
            if( c.IsValid() ) v = c.Get<T>()
            return v
        template <class T> T GetChannelValue(unsigned int pChnlId, T pInitVal)
            T v = pInitVal
            FbxProperty c = GetChannel(pChnlId)
            if( c.IsValid() ) v = c.Get<T>()
            return v
        bool DisconnectFromChannel(FbxAnimCurve* pCurve, unsigned int pChnlId)
        bool ConnectToChannel(FbxAnimCurve* pCurve, const char* pChnl, bool pInFront = false)
        bool ConnectToChannel(FbxAnimCurve* pCurve, unsigned int pChnlId, bool pInFront = false)
        FbxAnimCurve* CreateCurve(const char* pCurveNodeName, const char* pChannel)
        FbxAnimCurve* CreateCurve(const char* pCurveNodeName, unsigned int pChannelId = 0)
        int GetCurveCount(unsigned int pChannelId, const char* pCurveNodeName = NULL)
        FbxAnimCurve* GetCurve(unsigned int pChannelId, unsigned int pId = 0, const char* pCurveNodeName = NULL)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
    static const char* CurveNodeNameFrom(const char* pName)
	static bool EvaluateChannels(FbxAnimCurveNode* pCurveNode, double* pData, unsigned int pCount, FbxTime pTime)
    void ReleaseKFCurveNode()
    void SyncChannelsWithKFCurve()
    inline bool UseQuaternionInterpolation() 
return mQuaternionInterpolation != 0
	bool SetQuaternionInterpolation(unsigned short pVal)
    unsigned short GetQuaternionInterpolation() 
 return mQuaternionInterpolation
    void SetKFCurveNodeLayerType(FbxProperty& pProp)
	KFCurveNode* GetKFCurveNode(bool pNoCreate=false)
private:
	friend class FbxAnimCurveFilterMatrixConverter
	friend class FbxAnimEvalClassic
    void Evaluate(double* pData, FbxTime pTime)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void Destruct(bool pRecursive)
    virtual void ConstructProperties(bool pForceSet)
	virtual bool ConnectNotify(const FbxConnectEvent& pEvent)
    FbxAnimCurveNode* Find(FbxAnimCurveNode* pRoot, const FbxString& pName)
private:
    FbxProperty GetChannel(const char* pChnl)
    FbxProperty GetChannel(unsigned int pChnlId)
	friend void CollectAnimFromCurveNode(void **lSrc, void *fcn, unsigned int nbCrvs, FbxAnimCurveNode *cn, FbxMultiMap* pNickToAnimCurveTimeWarpsSet, FbxMultiMap& pNickToKFCurveNodeWarpSet)
    unsigned char	mNonRemovableChannels
    FbxProperty		mChannels
    FbxProperty*	mCurrentlyProcessed
    KFCurveNode*	mFCurveNode
    bool*			mOwnedKFCurve
    int				mKFCurveNodeLayerType
    unsigned short  mQuaternionInterpolation
	int*			mDirectIndexes
	int				mDirectIndexesSize
    FbxAnimCurve* GetCurve(unsigned int pChannelId, unsigned int pId, FbxAnimCurveNode* pCurveNode)
    bool ConnectToChannel(FbxProperty& p, FbxAnimCurve* pCurve, bool pInFront)
    void ResetKFCurveNode()
    void SyncKFCurveValue(FbxAnimCurve* pCurve, double pVal)
	void ReleaseOwnershipOfKFCurve(int pIndex)
	template <class T> FbxAnimCurve* CreateCurveGeneral(const char* pCurveNodeName, T pChannel)
#endif 