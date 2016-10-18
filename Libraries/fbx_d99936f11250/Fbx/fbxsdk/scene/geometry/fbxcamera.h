#ifndef _FBXSDK_SCENE_GEOMETRY_CAMERA_H_
#define _FBXSDK_SCENE_GEOMETRY_CAMERA_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/core/math/fbxvector4.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxTexture
class FBXSDK_DLL FbxCamera : public FbxNodeAttribute
    FBXSDK_OBJECT_DECLARE(FbxCamera,FbxNodeAttribute)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
    enum EProjectionType
        ePerspective,	
        eOrthogonal		
        enum EFormat
            eCustomFormat,	
            eD1NTSC,		
            eNTSC,			
            ePAL,			
            eD1PAL,			
            eHD,			
            e640x480,		
            e320x200,		
            e320x240,		
            e128x128,		
            eFullscreen		
        void SetFormat(EFormat pFormat)
        EFormat GetFormat() const
        enum EAspectRatioMode
            eWindowSize,		
            eFixedRatio,		
            eFixedResolution,	
            eFixedWidth,		
            eFixedHeight		
        void SetAspect(EAspectRatioMode pRatioMode, double pWidth, double pHeight)
        EAspectRatioMode GetAspectRatioMode() const
        void SetPixelRatio(double pRatio)
        double GetPixelRatio() const
        void SetNearPlane(double pDistance)
        double GetNearPlane() const
        void SetFarPlane(double pDistance)
        double GetFarPlane() const
    enum EApertureFormat
		eCustomAperture,	
        e16mmTheatrical,	
        eSuper16mm,			
        e35mmAcademy,		
        e35mmTVProjection,	
        e35mmFullAperture,	
        e35mm185Projection,	
        e35mmAnamorphic,	
        e70mmProjection,	
        eVistaVision,		
        eDynaVision,		
        eIMAX				
    void SetApertureFormat(EApertureFormat pFormat)
    EApertureFormat GetApertureFormat() const
    enum EApertureMode
		eHorizAndVert,	
        eHorizontal,	
        eVertical,		
        eFocalLength	
    void SetApertureMode(EApertureMode pMode)
    EApertureMode GetApertureMode() const
    void SetApertureWidth(double pWidth)
    double GetApertureWidth() const
    void SetApertureHeight(double pHeight)
    double GetApertureHeight() const
    void SetSqueezeRatio(double pRatio)
    double GetSqueezeRatio() const
    enum EGateFit
        eFitNone,		
        eFitVertical,	
        eFitHorizontal,	
        eFitFill,		
        eFitOverscan,	
        eFitStretch		
    double ComputeFieldOfView(double pFocalLength) const
    double ComputeFocalLength(double pAngleOfView) const
    enum EFilmRollOrder
        eRotateFirst,	
        eTranslateFirst	
    void SetBackgroundFileName(const char* pFileName)
    const char* GetBackgroundFileName() const
    void SetBackgroundMediaName(const char* pFileName)
    const char* GetBackgroundMediaName() const
    void SetForegroundFileName(const char* pFileName)
    const char* GetForegroundFileName() const
    void SetForegroundMediaName(const char* pFileName)
    const char* GetForegroundMediaName() const
    enum EPlateDrawingMode
        ePlateBackground,	
        ePlateForeground,	
        ePlateBackAndFront	
    void SetBackgroundAlphaTreshold(double pThreshold)
    double GetBackgroundAlphaTreshold() const
    void SetBackPlateFitImage(bool pFitImage)
    bool GetBackPlateFitImage() const
    void SetBackPlateCrop(bool pCrop)
    bool GetBackPlateCrop() const
    void SetBackPlateCenter(bool pCenter)
    bool GetBackPlateCenter() const
    void SetBackPlateKeepRatio(bool pKeepRatio)
    bool GetBackPlateKeepRatio() const
    void SetShowFrontPlate(bool pEnable)
    bool GetShowFrontPlate() const
    void SetFrontPlateFitImage(bool pFrontPlateFitImage)
    bool GetFrontPlateFitImage() const
    void SetFrontPlateCrop(bool pFrontPlateCrop)
    bool GetFrontPlateCrop() const
    void SetFrontPlateCenter(bool pFrontPlateCenter)
    bool GetFrontPlateCenter() const
    void SetFrontPlateKeepRatio(bool pFrontPlateKeepRatio)
    bool GetFrontPlateKeepRatio() const
    void SetForegroundOpacity(double pOpacity)
    double GetForegroundOpacity() const
    void SetForegroundTexture(FbxTexture* pTexture)
    FbxTexture* GetForegroundTexture() const
    enum EFrontBackPlaneDistanceMode
        eRelativeToInterest,	
        eRelativeToCamera		
    void SetBackPlaneDistanceMode(EFrontBackPlaneDistanceMode pMode)
    EFrontBackPlaneDistanceMode GetBackPlaneDistanceMode() const
    void SetFrontPlaneDistance(double pDistance)
    double GetFrontPlaneDistance() const
    void SetFrontPlaneDistanceMode(EFrontBackPlaneDistanceMode pMode)
    EFrontBackPlaneDistanceMode GetFrontPlaneDistanceMode() const
    enum EFrontBackPlaneDisplayMode
        ePlanesDisabled,	
        ePlanesAlways,		
        ePlanesWhenMedia	
    void SetViewFrustumFrontPlaneMode(EFrontBackPlaneDisplayMode pMode)
    EFrontBackPlaneDisplayMode GetViewFrustumFrontPlaneMode() const
    void SetViewFrustumBackPlaneMode(EFrontBackPlaneDisplayMode pMode)
    EFrontBackPlaneDisplayMode GetViewFrustumBackPlaneMode() const
    void SetViewCameraInterest(bool pEnable)
    bool GetViewCameraInterest() const
    void SetViewNearFarPlanes(bool pEnable)
    bool GetViewNearFarPlanes() const
    enum ESafeAreaStyle
		eSafeAreaRound,	
        eSafeAreaSquare	
    enum ERenderOptionsUsageTime
        eInteractive,	
        eOnDemand		
    enum EAntialiasingMethod
        eAAOversampling,	
        eAAHardware			
    enum ESamplingType
		eSamplingUniform,	
		eSamplingStochastic	
    enum EFocusDistanceSource
        eFocusSrcCameraInterest,	
        eFocusSpecificDistance		
		FbxVector4 EvaluatePosition(const FbxTime& pTime=FBXSDK_TIME_ZERO) const
		FbxVector4 EvaluateLookAtPosition(const FbxTime& pTime=FBXSDK_TIME_ZERO) const
		FbxVector4 EvaluateUpDirection(const FbxVector4& pCameraPosition, const FbxVector4& pLookAtPosition, const FbxTime& pTime=FBXSDK_TIME_ZERO) const
		FbxMatrix ComputeProjectionMatrix(const int pWidth, const int pHeight, const bool pVerticalFOV = true) const
		bool IsBoundingBoxInView(const FbxMatrix& pWorldToScreen, const FbxMatrix& pWorldToCamera, const FbxVector4 pPoints[8]) const
		bool IsPointInView(const FbxMatrix& pWorldToScreen, const FbxMatrix& pWorldToCamera, const FbxVector4& pPoint) const
		FbxMatrix ComputeWorldToScreen(int pPixelWidth, int pPixelHeight, const FbxAMatrix& pWorldToCamera) const
		FbxVector4 ComputeScreenToWorld(float pX, float pY, float pWidth, float pHeight, const FbxTime& pTime=FBXSDK_TIME_INFINITE) const
    FbxPropertyT<FbxDouble3>                       Position
    FbxPropertyT<FbxDouble3>                       UpVector
    FbxPropertyT<FbxDouble3>                       InterestPosition
    FbxPropertyT<FbxDouble>                       Roll
    FbxPropertyT<FbxDouble>                       OpticalCenterX
    FbxPropertyT<FbxDouble>                       OpticalCenterY
    FbxPropertyT<FbxDouble3>                       BackgroundColor
    FbxPropertyT<FbxDouble>                       TurnTable
    FbxPropertyT<FbxBool>                         DisplayTurnTableIcon
    FbxPropertyT<FbxBool>                         UseMotionBlur
    FbxPropertyT<FbxBool>                         UseRealTimeMotionBlur
    FbxPropertyT<FbxDouble>                       MotionBlurIntensity
    FbxPropertyT<EAspectRatioMode>           AspectRatioMode
    FbxPropertyT<FbxDouble>                       AspectWidth
    FbxPropertyT<FbxDouble>                       AspectHeight
    FbxPropertyT<FbxDouble>                       PixelAspectRatio
    FbxPropertyT<EApertureMode>              ApertureMode
    FbxPropertyT<EGateFit>                   GateFit
    FbxPropertyT<FbxDouble>                       FieldOfView
    FbxPropertyT<FbxDouble>                       FieldOfViewX
    FbxPropertyT<FbxDouble>                       FieldOfViewY
    FbxPropertyT<FbxDouble>                       FocalLength
    FbxPropertyT<EFormat>                    CameraFormat
    FbxPropertyT<FbxBool>                         UseFrameColor
    FbxPropertyT<FbxDouble3>                       FrameColor
    FbxPropertyT<FbxBool>                         ShowName
    FbxPropertyT<FbxBool>                         ShowInfoOnMoving
    FbxPropertyT<FbxBool>                         ShowGrid
    FbxPropertyT<FbxBool>                         ShowOpticalCenter
    FbxPropertyT<FbxBool>                         ShowAzimut
    FbxPropertyT<FbxBool>                         ShowTimeCode
    FbxPropertyT<FbxBool>                         ShowAudio
    FbxPropertyT<FbxDouble3>                       AudioColor
    FbxPropertyT<FbxDouble>                       NearPlane
    FbxPropertyT<FbxDouble>                       FarPlane
    FbxPropertyT<FbxBool>                         AutoComputeClipPlanes
    FbxPropertyT<FbxDouble>                       FilmWidth
    FbxPropertyT<FbxDouble>                       FilmHeight
    FbxPropertyT<FbxDouble>                       FilmAspectRatio
    FbxPropertyT<FbxDouble>                       FilmSqueezeRatio
    FbxPropertyT<EApertureFormat>            FilmFormat
    FbxPropertyT<FbxDouble>                       FilmOffsetX
    FbxPropertyT<FbxDouble>                       FilmOffsetY
    FbxPropertyT<FbxDouble>                       PreScale
    FbxPropertyT<FbxDouble>                       FilmTranslateX
    FbxPropertyT<FbxDouble>                       FilmTranslateY
    FbxPropertyT<FbxDouble>                       FilmRollPivotX
    FbxPropertyT<FbxDouble>                       FilmRollPivotY
    FbxPropertyT<FbxDouble>                       FilmRollValue
    FbxPropertyT<EFilmRollOrder>             FilmRollOrder 
    FbxPropertyT<FbxBool>                         ViewCameraToLookAt
    FbxPropertyT<FbxBool>                         ViewFrustumNearFarPlane
    FbxPropertyT<EFrontBackPlaneDisplayMode>	ViewFrustumBackPlaneMode
    FbxPropertyT<FbxDouble>                       BackPlaneDistance
    FbxPropertyT<EFrontBackPlaneDistanceMode>	BackPlaneDistanceMode
    FbxPropertyT<EFrontBackPlaneDisplayMode>	ViewFrustumFrontPlaneMode
    FbxPropertyT<FbxDouble>                       FrontPlaneDistance
    FbxPropertyT<EFrontBackPlaneDistanceMode>	FrontPlaneDistanceMode
    FbxPropertyT<FbxBool>                         LockMode
    FbxPropertyT<FbxBool>                         LockInterestNavigation
    FbxPropertyT<FbxBool>                         BackPlateFitImage
    FbxPropertyT<FbxBool>                         BackPlateCrop
    FbxPropertyT<FbxBool>                         BackPlateCenter
    FbxPropertyT<FbxBool>                         BackPlateKeepRatio
    FbxPropertyT<FbxDouble>                       BackgroundAlphaTreshold
    FbxPropertyT<FbxDouble>                       BackPlaneOffsetX
    FbxPropertyT<FbxDouble>                       BackPlaneOffsetY
    FbxPropertyT<FbxDouble>                       BackPlaneRotation
    FbxPropertyT<FbxDouble>                       BackPlaneScaleX
    FbxPropertyT<FbxDouble>                       BackPlaneScaleY
    FbxPropertyT<FbxBool>                         ShowBackplate
    FbxPropertyT<FbxReference> BackgroundTexture
    FbxPropertyT<FbxBool> FrontPlateFitImage
    FbxPropertyT<FbxBool> FrontPlateCrop
    FbxPropertyT<FbxBool> FrontPlateCenter
    FbxPropertyT<FbxBool> FrontPlateKeepRatio
    FbxPropertyT<FbxBool> ShowFrontplate
    FbxPropertyT<FbxDouble>                       FrontPlaneOffsetX
    FbxPropertyT<FbxDouble>                       FrontPlaneOffsetY
    FbxPropertyT<FbxDouble>                       FrontPlaneRotation
    FbxPropertyT<FbxDouble>                       FrontPlaneScaleX
    FbxPropertyT<FbxDouble>                       FrontPlaneScaleY
    FbxPropertyT<FbxReference>						ForegroundTexture
    FbxPropertyT<FbxDouble>						ForegroundOpacity
    FbxPropertyT<FbxBool>                         DisplaySafeArea
    FbxPropertyT<FbxBool>                         DisplaySafeAreaOnRender
    FbxPropertyT<ESafeAreaStyle>             SafeAreaDisplayStyle
    FbxPropertyT<FbxDouble>                       SafeAreaAspectRatio
    FbxPropertyT<FbxBool>                         Use2DMagnifierZoom
    FbxPropertyT<FbxDouble>                       _2DMagnifierZoom
    FbxPropertyT<FbxDouble>                       _2DMagnifierX
    FbxPropertyT<FbxDouble>                       _2DMagnifierY
    FbxPropertyT<EProjectionType>            ProjectionType
    FbxPropertyT<FbxDouble>                       OrthoZoom
    FbxPropertyT<FbxBool>                         UseRealTimeDOFAndAA
    FbxPropertyT<FbxBool>                         UseDepthOfField
    FbxPropertyT<EFocusDistanceSource>       FocusSource
    FbxPropertyT<FbxDouble>                       FocusAngle
    FbxPropertyT<FbxDouble>                       FocusDistance
    FbxPropertyT<FbxBool>                         UseAntialiasing
    FbxPropertyT<FbxDouble>                       AntialiasingIntensity
    FbxPropertyT<EAntialiasingMethod>        AntialiasingMethod
    FbxPropertyT<FbxBool>                         UseAccumulationBuffer
    FbxPropertyT<FbxInt>                      FrameSamplingCount
    FbxPropertyT<ESamplingType>              FrameSamplingType
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
    virtual void ConstructProperties(bool pForceSet)
	virtual FbxStringList GetTypeFlags() const
private:
    double ComputePixelRatio(FbxUInt pWidth, FbxUInt pHeight, double pScreenRatio = 1.3333333333)
    FbxString mBackgroundMediaName
    FbxString mBackgroundFileName
    FbxString mForegroundMediaName
    FbxString mForegroundFileName
	FbxVector4 mLastUp
#endif 