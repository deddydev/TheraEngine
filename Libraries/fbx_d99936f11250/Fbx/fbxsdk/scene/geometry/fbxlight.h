#ifndef _FBXSDK_SCENE_GEOMETRY_LIGHT_H_
#define _FBXSDK_SCENE_GEOMETRY_LIGHT_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxTexture
class FBXSDK_DLL FbxLight : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxLight, FbxNodeAttribute)
public:
		enum EType
			ePoint, 
			eDirectional, 
			eSpot,
			eArea,
			eVolume
		enum EDecayType
			eNone,
			eLinear,
			eQuadratic,
			eCubic
		enum EAreaLightShape
			eRectangle,
			eSphere
		void SetShadowTexture(FbxTexture* pTexture)
		FbxTexture* GetShadowTexture() const
		FbxPropertyT<EType> LightType
		FbxPropertyT<FbxBool> CastLight
		FbxPropertyT<FbxBool> DrawVolumetricLight
		FbxPropertyT<FbxBool> DrawGroundProjection
		FbxPropertyT<FbxBool> DrawFrontFacingVolumetricLight
		FbxPropertyT<FbxDouble3> Color
		FbxPropertyT<FbxDouble> Intensity
		FbxPropertyT<FbxDouble> InnerAngle
		FbxPropertyT<FbxDouble> OuterAngle
		FbxPropertyT<FbxDouble> Fog
		FbxPropertyT<EDecayType> DecayType
		FbxPropertyT<FbxDouble> DecayStart
		FbxPropertyT<FbxString> FileName
		FbxPropertyT<FbxBool> EnableNearAttenuation
		FbxPropertyT<FbxDouble> NearAttenuationStart
		FbxPropertyT<FbxDouble> NearAttenuationEnd
		FbxPropertyT<FbxBool> EnableFarAttenuation
		FbxPropertyT<FbxDouble> FarAttenuationStart
		FbxPropertyT<FbxDouble> FarAttenuationEnd
		FbxPropertyT<FbxBool> CastShadows
		FbxPropertyT<FbxDouble3> ShadowColor
		FbxPropertyT<EAreaLightShape> AreaLightShape
		FbxPropertyT<FbxFloat> LeftBarnDoor
		FbxPropertyT<FbxFloat> RightBarnDoor
		FbxPropertyT<FbxFloat> TopBarnDoor
		FbxPropertyT<FbxFloat> BottomBarnDoor
		FbxPropertyT<FbxBool> EnableBarnDoor
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxNodeAttribute::EType GetAttributeType() const
protected:
	virtual void ConstructProperties(bool pForceSet)
	virtual FbxStringList	GetTypeFlags() const
#endif 