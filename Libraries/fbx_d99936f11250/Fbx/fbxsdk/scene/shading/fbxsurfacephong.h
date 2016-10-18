#ifndef _FBXSDK_SCENE_SHADING_SURFACE_PHONG_H_
#define _FBXSDK_SCENE_SHADING_SURFACE_PHONG_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxsurfacelambert.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSurfacePhong : public FbxSurfaceLambert
	FBXSDK_OBJECT_DECLARE(FbxSurfacePhong, FbxSurfaceLambert)
public:
    FbxPropertyT<FbxDouble3> Specular
	FbxPropertyT<FbxDouble> SpecularFactor
	FbxPropertyT<FbxDouble> Shininess
	FbxPropertyT<FbxDouble3> Reflection
	FbxPropertyT<FbxDouble> ReflectionFactor
	static const FbxDouble3 sSpecularDefault
	static const FbxDouble sSpecularFactorDefault
	static const FbxDouble sShininessDefault
	static const FbxDouble3 sReflectionDefault
	static const FbxDouble sReflectionFactorDefault
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void ConstructProperties(bool pForceSet)
	void Init()
#endif 