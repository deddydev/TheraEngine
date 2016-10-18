#ifndef _FBXSDK_SCENE_SHADING_TEXTURE_H_
#define _FBXSDK_SCENE_SHADING_TEXTURE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxTexture : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxTexture, FbxObject)
public:
		enum EUnifiedMappingType
			eUMT_UV,			
			eUMT_XY,			
			eUMT_YZ,			
			eUMT_XZ,			
			eUMT_SPHERICAL,		
			eUMT_CYLINDRICAL,	
			eUMT_ENVIRONMENT,	
			eUMT_PROJECTION,	
			eUMT_BOX,			
			eUMT_FACE,			
			eUMT_NO_MAPPING,	
		enum ETextureUse6
			eTEXTURE_USE_6_STANDARD,				
			eTEXTURE_USE_6_SPHERICAL_REFLEXION_MAP,	
			eTEXTURE_USE_6_SPHERE_REFLEXION_MAP,	
			eTEXTURE_USE_6_SHADOW_MAP,				
			eTEXTURE_USE_6_LIGHT_MAP,				
			eTEXTURE_USE_6_BUMP_NORMAL_MAP			
		enum EWrapMode
			eRepeat,	
			eClamp		
		enum EBlendMode
			eTranslucent,	
			eAdditive,		
			eModulate,		
			eModulate2,		
			eOver			
        enum EAlignMode
            eLeft,	
            eRight,	
            eTop,	
            eBottom	
        enum ECoordinates
            eU,	
            eV,	
            eW	
		FbxPropertyT<ETextureUse6>			TextureTypeUse
		FbxPropertyT<FbxDouble>			Alpha
		FbxPropertyT<EUnifiedMappingType>	CurrentMappingType
		FbxPropertyT<EWrapMode>			WrapModeU
		FbxPropertyT<EWrapMode>			WrapModeV
		FbxPropertyT<FbxBool>				UVSwap
		FbxPropertyT<FbxBool>				PremultiplyAlpha
		FbxPropertyT<FbxDouble3>			Translation
		FbxPropertyT<FbxDouble3>			Rotation
		FbxPropertyT<FbxDouble3>			Scaling
		FbxPropertyT<FbxDouble3>			RotationPivot
		FbxPropertyT<FbxDouble3>			ScalingPivot
		FbxPropertyT<EBlendMode>	CurrentTextureBlendMode
		FbxPropertyT<FbxString>			UVSet
        static const char* sVectorSpace        
        static const char* sVectorSpaceWorld   
        static const char* sVectorSpaceObject  
        static const char* sVectorSpaceTangent 
        static const char* sVectorEncoding     
        static const char* sVectorEncodingFP   
        static const char* sVectorEncodingSE   
	virtual void Reset()
    void SetSwapUV(bool pSwapUV)
    bool GetSwapUV() const
    void SetPremultiplyAlpha(bool pPremultiplyAlpha)
    bool GetPremultiplyAlpha() const
    enum EAlphaSource
        eNone,			
        eRGBIntensity,	
        eBlack			
    void SetAlphaSource(EAlphaSource pAlphaSource)
	EAlphaSource GetAlphaSource() const
    void SetCropping(int pLeft, int pTop, int pRight, int pBottom)
    int GetCroppingLeft() const
    int GetCroppingTop() const
    int GetCroppingRight() const
    int GetCroppingBottom() const
    enum EMappingType
        eNull,			
        ePlanar,		
        eSpherical,		
        eCylindrical,	
        eBox,			
        eFace,			
        eUV,			
		eEnvironment	
    void SetMappingType(EMappingType pMappingType)
    EMappingType GetMappingType() const
    enum EPlanarMappingNormal
        ePlanarNormalX,	
        ePlanarNormalY,	
        ePlanarNormalZ	
    void SetPlanarMappingNormal(EPlanarMappingNormal pPlanarMappingNormal)
    EPlanarMappingNormal GetPlanarMappingNormal() const
	enum ETextureUse
		eStandard,					
		eShadowMap,					
		eLightMap,					
		eSphericalReflectionMap,	
		eSphereReflectionMap,		
		eBumpNormalMap				
    void SetTextureUse(ETextureUse pTextureUse)
    ETextureUse GetTextureUse() const
    void SetWrapMode(EWrapMode pWrapU, EWrapMode pWrapV)
    EWrapMode GetWrapModeU() const
	EWrapMode GetWrapModeV() const
	void SetBlendMode(EBlendMode pBlendMode)
	EBlendMode GetBlendMode() const
	inline void SetDefaultT(const FbxVector4& pT) 
 Translation.Set( pT )
	FbxVector4& GetDefaultT(FbxVector4& pT) const
	inline void SetDefaultR(const FbxVector4& pR) 
 Rotation.Set( FbxDouble3(pR[0],pR[1],pR[2]) )
	FbxVector4& GetDefaultR(FbxVector4& pR) const
	inline void SetDefaultS(const FbxVector4& pS) 
 Scaling.Set( FbxDouble3(pS[0],pS[1],pS[2]) )
	FbxVector4& GetDefaultS(FbxVector4& pS) const
	void SetDefaultAlpha(double pAlpha)
	double GetDefaultAlpha() const
	void SetTranslation(double pU,double pV)
    double GetTranslationU() const
    double GetTranslationV() const
    void SetRotation(double pU, double pV, double pW = 0.0)
    double GetRotationU() const
    double GetRotationV() const
    double GetRotationW() const
	void SetScale(double pU,double pV)
    double GetScaleU() const
    double GetScaleV() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	bool operator==(FbxTexture const& pTexture) const
	void SetUVTranslation(FbxVector2& pT)
	FbxVector2& GetUVTranslation()
	void SetUVScaling(FbxVector2& pS)
	FbxVector2& GetUVScaling()
	FbxString GetTextureType()
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
    virtual bool PropertyNotify(EPropertyNotifyType pType, FbxProperty& pProperty)
	void Init()
	int mCropping[4]
    EAlphaSource mAlphaSource
 not a prop
	EMappingType mMappingType
	EPlanarMappingNormal mPlanarMappingNormal
	FbxVector2 mUVScaling
	FbxVector2 mUVTranslation
#endif 