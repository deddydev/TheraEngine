#ifndef _FBXSDK_SCENE_ANIMATION_LAYER_H_
#define _FBXSDK_SCENE_ANIMATION_LAYER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxcollection.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxAnimCurveNode
class FBXSDK_DLL FbxAnimLayer : public FbxCollection
    FBXSDK_OBJECT_DECLARE(FbxAnimLayer, FbxCollection)
public:
    FbxPropertyT<FbxDouble>       Weight
    FbxPropertyT<FbxBool>         Mute
    FbxPropertyT<FbxBool>         Solo
    FbxPropertyT<FbxBool>         Lock
    FbxPropertyT<FbxDouble3>       Color
    FbxPropertyT<FbxEnum>			BlendMode
    FbxPropertyT<FbxEnum>			RotationAccumulationMode
    FbxPropertyT<FbxEnum>			ScaleAccumulationMode
	void Reset()
		void SetBlendModeBypass(EFbxType pType, bool pState)
		bool GetBlendModeBypass(EFbxType pType)
	enum EBlendMode
		eBlendAdditive,	
		eBlendOverride,	
		eBlendOverridePassthrough	
	enum ERotationAccumulationMode
		eRotationByLayer,	
		eRotationByChannel	
	enum EScaleAccumulationMode
		eScaleMultiply,	
		eScaleAdditive	
        FbxAnimCurveNode* CreateCurveNode(FbxProperty& pProperty)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void ConstructProperties(bool pForceSet)
	virtual FbxAnimLayer* GetAnimLayer()
private:
    FbxPropertyT<FbxULongLong>	mBlendModeBypass
#endif 