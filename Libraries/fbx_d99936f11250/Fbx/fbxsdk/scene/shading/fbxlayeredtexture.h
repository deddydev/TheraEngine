#ifndef _FBXSDK_SCENE_SHADING_LAYERED_TEXTURE_H_
#define _FBXSDK_SCENE_SHADING_LAYERED_TEXTURE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxtexture.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxLayeredTexture : public FbxTexture
	FBXSDK_OBJECT_DECLARE(FbxLayeredTexture, FbxTexture)
public:
	enum EBlendMode
		eTranslucent,
		eAdditive,
		eModulate,
		eModulate2,
        eOver,
        eNormal,		
        eDissolve,
        eDarken,			
        eColorBurn,
        eLinearBurn, 	
        eDarkerColor,
        eLighten,			
        eScreen,		
        eColorDodge,
        eLinearDodge,
        eLighterColor,
        eSoftLight,		
        eHardLight,		
        eVividLight,
        eLinearLight,
        ePinLight, 		
        eHardMix,		
        eDifference, 		
        eExclusion, 		
        eSubtract,
        eDivide,
        eHue, 			
        eSaturation,		
        eColor,		
        eLuminosity,
        eOverlay,
        eBlendModeCount
	bool operator==( const FbxLayeredTexture& pOther ) const
    bool SetTextureBlendMode( int pIndex, EBlendMode pMode )
    bool GetTextureBlendMode( int pIndex, EBlendMode& pMode ) const
    bool SetTextureAlpha( int pIndex, double pAlpha )
    bool GetTextureAlpha( int pIndex, double& pAlpha ) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
    struct InputData
        EBlendMode mBlendMode
        double mAlpha
public:
    FbxArray<InputData> mInputData
protected:
    virtual bool ConnectNotify (FbxConnectEvent const &pEvent)
    bool RemoveInputData( int pIndex )
#endif 