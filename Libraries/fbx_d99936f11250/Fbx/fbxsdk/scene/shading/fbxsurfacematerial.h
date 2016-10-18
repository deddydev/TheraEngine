#ifndef _FBXSDK_SCENE_SHADING_SURFACE_MATERIAL_H_
#define _FBXSDK_SCENE_SHADING_SURFACE_MATERIAL_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSurfaceMaterial : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxSurfaceMaterial, FbxObject)
public:
	static const char* sShadingModel
	static const char* sMultiLayer
	static const char* sEmissive
	static const char* sEmissiveFactor
	static const char* sAmbient
	static const char* sAmbientFactor
	static const char* sDiffuse
	static const char* sDiffuseFactor
	static const char* sSpecular
	static const char* sSpecularFactor
	static const char* sShininess
	static const char* sBump
	static const char* sNormalMap
    static const char* sBumpFactor
	static const char* sTransparentColor
	static const char* sTransparencyFactor
	static const char* sReflection
	static const char* sReflectionFactor
    static const char* sDisplacementColor
    static const char* sDisplacementFactor
    static const char* sVectorDisplacementColor
    static const char* sVectorDisplacementFactor
	FbxPropertyT<FbxString> ShadingModel
	FbxPropertyT<FbxBool> MultiLayer
	static const FbxBool sMultiLayerDefault
	static const char*	sShadingModelDefault
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	bool SetColorParameter(FbxProperty pProperty, FbxColor const& pColor)
	bool GetColorParameter(FbxProperty pProperty, FbxColor& pColor) const
	bool SetDoubleParameter(FbxProperty pProperty, double pDouble)
	bool GetDoubleParameter(FbxProperty pProperty, double pDouble) const
	virtual void ConstructProperties(bool pForceSet)
#endif 