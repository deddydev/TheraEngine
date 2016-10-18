#ifndef _FBXSDK_SCENE_DISPLAY_LAYER_H_
#define _FBXSDK_SCENE_DISPLAY_LAYER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxcollectionexclusive.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxDisplayLayer : public FbxCollectionExclusive
    FBXSDK_OBJECT_DECLARE(FbxDisplayLayer, FbxCollectionExclusive)
public:
    FbxPropertyT<FbxDouble3>       Color
    FbxPropertyT<FbxBool>         Show
    FbxPropertyT<FbxBool>         Freeze
    FbxPropertyT<FbxBool>         LODBox
    static const FbxDouble3 sColorDefault
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
#endif 