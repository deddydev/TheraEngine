#ifndef _FBXSDK_SCENE_SHADING_SURFACE_LAMBERT_H_
#define _FBXSDK_SCENE_SHADING_SURFACE_LAMBERT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxsurfacematerial.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSurfaceLambert : public FbxSurfaceMaterial
	FBXSDK_OBJECT_DECLARE(FbxSurfaceLambert,FbxSurfaceMaterial)
public:
    FbxPropertyT<FbxDouble3> Emissive
    FbxPropertyT<FbxDouble> EmissiveFactor
    FbxPropertyT<FbxDouble3> Ambient
    FbxPropertyT<FbxDouble> AmbientFactor
    FbxPropertyT<FbxDouble3> Diffuse
    FbxPropertyT<FbxDouble> DiffuseFactor
   	FbxPropertyT<FbxDouble3> NormalMap
   	FbxPropertyT<FbxDouble3> Bump
    FbxPropertyT<FbxDouble> BumpFactor
    FbxPropertyT<FbxDouble3> TransparentColor
    FbxPropertyT<FbxDouble> TransparencyFactor
    FbxPropertyT<FbxDouble3> DisplacementColor
    FbxPropertyT<FbxDouble> DisplacementFactor
    FbxPropertyT<FbxDouble3> VectorDisplacementColor
    FbxPropertyT<FbxDouble> VectorDisplacementFactor
	static const FbxDouble3 sEmissiveDefault
	static const FbxDouble sEmissiveFactorDefault
	static const FbxDouble3 sAmbientDefault
	static const FbxDouble sAmbientFactorDefault
	static const FbxDouble3 sDiffuseDefault
	static const FbxDouble sDiffuseFactorDefault
	static const FbxDouble3 sBumpDefault
    static const FbxDouble3 sNormalMapDefault
    static const FbxDouble sBumpFactorDefault
	static const FbxDouble3 sTransparentDefault
	static const FbxDouble sTransparencyFactorDefault
    static const FbxDouble3 sDisplacementDefault
    static const FbxDouble sDisplacementFactorDefault
    static const FbxDouble3 sVectorDisplacementDefault
    static const FbxDouble sVectorDisplacementFactorDefault
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void ConstructProperties(bool pForceSet)
	void Init()
#endif 