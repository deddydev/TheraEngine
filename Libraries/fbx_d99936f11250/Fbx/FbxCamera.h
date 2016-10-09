#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNodeAttribute.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxNodeAttribute;
		ref class FbxMatrix;
		ref class FbxXMatrix;
		ref class FbxVector4;
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxDouble1TypedProperty;
		ref class FbxDouble2TypedProperty;
		ref class FbxDouble3TypedProperty;
		/** \brief This node attribute contains methods for accessing the properties of a camera.
		* \nosubgrouping
		* A camera can be set to automatically point at and follow
		* another node in the hierarchy. To do this, the focus source
		* must be set to ECameraFocusSource::eCAMERA_INTEREST and the
		* followed node associated with function KFbxNode::SetTarget().
		*/
		public ref class FbxCamera : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxCamera);
		internal:
			FbxCamera(KFbxCamera* instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}

			FBXOBJECT_DECLARE(FbxCamera);
		public:
			//! Return the type of node attribute which is EAttributeType::eCAMERA.
			//virtual property AttributeType GetAttributeType() const;

			//! Reset the camera to default values.
			//void Reset();

			/**
			* \name Camera Position and Orientation Functions
			*/
			//@{			

			/** Camera projection types.
			* \enum ECameraProjectionType Camera projection types.
			* - \e ePERSPECTIVE
			* - \e eORTHOGONAL
			* \remarks     By default, the camera projection type is set to ePERSPECTIVE.
			*              If the camera projection type is set to eORTHOGONAL, the following options
			*              are not relevant:
			*                   - aperture format
			*                   - aperture mode
			*                   - aperture width and height
			*                   - angle of view/focal length
			*                   - squeeze ratio
			*/
			enum class CameraProjectionType
			{
				Perspective = KFbxCamera::ePERSPECTIVE,
				Orthogonal = KFbxCamera::eORTHOGONAL
			} ;			

			//@}

			/**
			* \name Viewing Area Functions
			*/
			//@{

			/** \enum ECameraFormat Camera formats.
			* - \e eCUSTOM_FORMAT
			* - \e eD1_NTSC
			* - \e eNTSC
			* - \e ePAL
			* - \e eD1_PAL
			* - \e eHD
			* - \e e640x480
			* - \e e320x200
			* - \e e320x240
			* - \e e128x128
			* - \e eFULL_SCREEN
			*/
			enum class CameraFormat
			{
				CustomFormat = KFbxCamera::eCUSTOM_FORMAT,
				D1Ntsc = KFbxCamera::eD1_NTSC,
				Ntsc = KFbxCamera::eNTSC,
				Pal = KFbxCamera::ePAL,
				D1Pal = KFbxCamera::eD1_PAL,
				Hd = KFbxCamera::eHD,
				R640x480 = KFbxCamera::e640x480,
				R320x200 = KFbxCamera::e320x200,
				R320x240 = KFbxCamera::e320x240,
				R128x128 = KFbxCamera::e128x128,
				FullScreen = KFbxCamera::eFULL_SCREEN
			} ;

			/** Get the camera format.
			* \return     The current camera format identifier.
			*/
			/** Set the camera format.
			* \param pFormat     The camera format identifier.
			* \remarks           Changing the camera format sets the camera aspect
			*                    ratio mode to eFIXED_RESOLUTION and modifies the aspect width
			*                    size, height size, and pixel ratio accordingly.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(CameraFormat,Format);			

			/** \enum ECameraAspectRatioMode Camera aspect ratio modes.
			* - \e eWINDOW_SIZE
			* - \e eFIXED_RATIO
			* - \e eFIXED_RESOLUTION
			* - \e eFIXED_WIDTH
			* - \e eFIXED_HEIGHT
			*/
			enum class CameraAspectRatioMode
			{
				WindowSize = KFbxCamera::eWINDOW_SIZE,
				FixedRatio = KFbxCamera::eFIXED_RATIO,
				FixedResolution = KFbxCamera::eFIXED_RESOLUTION,
				FixedWidth = KFbxCamera::eFIXED_WIDTH,
				FixedHeight = KFbxCamera::eFIXED_HEIGHT
			};

			/** Set the camera aspect.
			* \param pRatioMode     Camera aspect ratio mode.
			* \param pWidth         Camera aspect width, must be a positive value.
			* \param pHeight        Camera aspect height, must be a positive value.
			* \remarks              Changing the camera aspect sets the camera format to eCustom.
			*                            - If the ratio mode is eWINDOW_SIZE, both width and height values aren't relevant.
			*                            - If the ratio mode is eFIXED_RATIO, the height value is set to 1.0 and the width value is relative to the height value.
			*                            - If the ratio mode is eFIXED_RESOLUTION, both width and height values are in pixels.
			*                            - If the ratio mode is eFIXED_WIDTH, the width value is in pixels and the height value is relative to the width value.
			*                            - If the ratio mode is eFIXED_HEIGHT, the height value is in pixels and the width value is relative to the height value.
			*/
			void SetAspect(CameraAspectRatioMode ratioMode, double width, double height);

			/** Get the camera aspect ratio mode.
			* \return     The current aspect ratio identifier.
			*/
			VALUE_PROPERTY_GET_DECLARE(CameraAspectRatioMode,AspectRatioMode);			


			/** Get the pixel ratio.
			* \return     The current camera's pixel ratio value.
			*/
			/** Set the pixel ratio.
			* \param pRatio     The pixel ratio value.
			* \remarks          The value must be a positive number. Comprised between 0.05 and 20.0. Values
			*                   outside these limits will be clamped. Changing the pixel ratio sets the camera format to eCUSTOM_FORMAT.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,PixelRatio);

			/** Get the near plane distance from the camera.
			* The near plane is the minimum distance to render a scene on the camera display.
			* \return     The near plane value.
			*/
			/** Set the near plane distance from the camera.
			* The near plane is the minimum distance to render a scene on the camera display.
			* \param pDistance     The near plane distance value.
			* \remarks             The near plane value is limited to the range [0.001, 600000.0] and
			*                      must be inferior to the far plane value.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,NearPlane);																	

			/** Get the far plane distance from camera.
			* The far plane is the maximum distance to render a scene on the camera display.
			* \return     The far plane value.

			/** Set the far plane distance from camera.
			* The far plane is the maximum distance to render a scene on the camera display.
			* \param pDistance     The far plane distance value.
			* \remarks             The far plane value is limited to the range [0.001, 600000.0] and
			*                      must be superior to the near plane value.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,FarPlane);

			/** \enum ECameraApertureFormat Camera aperture formats.
			* - \e eCUSTOM_APERTURE_FORMAT
			* - \e e16MM_THEATRICAL
			* - \e eSUPER_16MM
			* - \e e35MM_ACADEMY
			* - \e e35MM_TV_PROJECTION
			* - \e e35MM_FULL_APERTURE
			* - \e e35MM_185_PROJECTION
			* - \e e35MM_ANAMORPHIC
			* - \e e70MM_PROJECTION
			* - \e eVISTAVISION
			* - \e eDYNAVISION
			* - \e eIMAX
			*/
			enum class CameraApertureFormat
			{
				CustomApertureFormat = KFbxCamera::eCUSTOM_APERTURE_FORMAT,
				A16mmTheatrical = KFbxCamera::e16MM_THEATRICAL,
				Super16mm = KFbxCamera::eSUPER_16MM,
				e35mmAcademy = KFbxCamera::e35MM_ACADEMY,
				e35mmTvProjection = KFbxCamera::e35MM_TV_PROJECTION,
				e35mmFullAperture = KFbxCamera::e35MM_FULL_APERTURE,
				e35mm185Projection = KFbxCamera::e35MM_185_PROJECTION,
				e35mmAnamorphic = KFbxCamera::e35MM_ANAMORPHIC,
				e70mmProjection = KFbxCamera::e70MM_PROJECTION,
				VistaVision = KFbxCamera::eVISTAVISION,
				Dynavision = KFbxCamera::eDYNAVISION,
				Imax = KFbxCamera::eIMAX
			};



			/** Get the camera aperture format.
			* \return     The camera's current aperture format identifier.
			*/			
			/** Set the camera aperture format.
			* \param pFormat     The camera aperture format identifier.
			* \remarks           Changing the aperture format modifies the aperture width, height, and squeeze ratio accordingly.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(CameraApertureFormat,ApertureFormat);

			/** \enum ECameraApertureMode
			* Camera aperture modes. The aperture mode determines which values drive the camera aperture. If the aperture mode is \e eHORIZONTAL_AND_VERTICAL,
			* \e eHORIZONTAL, or \e eVERTICAL, then the field of view is used. If the aperture mode is \e eFOCAL_LENGTH, then the focal length is used.
			* - \e eHORIZONTAL_AND_VERTICAL
			* - \e eHORIZONTAL
			* - \e eVERTICAL
			* - \e eFOCAL_LENGTH
			*/
			enum class CameraApertureMode
			{
				HorizontalAndVertical = KFbxCamera::eHORIZONTAL_AND_VERTICAL,
				Horizontal = KFbxCamera::eHORIZONTAL,
				Vertical = KFbxCamera::eVERTICAL,
				FocalLength = KFbxCamera::eFOCAL_LENGTH
			};						
			/** Get the camera aperture mode.
			* \return     The camera's current aperture mode identifier.
			*/			
			/** Set the camera aperture mode.
			* \param pMode     The camera aperture mode identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraApertureMode,ApertureMode);
			/** Get the camera aperture width in inches.
			* \return     The camera's current aperture width value in inches.
			*/
			/** Set the camera aperture width in inches.
			* \param pWidth     The aperture width value.
			* \remarks          Must be a positive value. The minimum accepted value is 0.0001.
			*                   Changing the aperture width sets the camera aperture format to eCUSTOM_FORMAT.
			*/						
			VALUE_PROPERTY_GETSET_DECLARE(double,ApertureWidth);			

			/** Get the camera aperture height in inches.
			* \return     The camera's current aperture height value in inches.
			*/			
			/** Set the camera aperture height in inches.
			* \param pHeight     The aperture height value.
			* \remarks           Must be a positive value. The minimum accepted value is 0.0001.
			*                    Changing the aperture height sets the camera aperture format to eCUSTOM_FORMAT.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,ApertureHeight);


			/** Get the camera squeeze ratio.
			* \return     The camera's current squeeze ratio value.
			*/			
			/** Set the squeeze ratio.
			* \param pRatio      The sqeeze ratio value.
			* \remarks           Must be a positive value. The minimum accepted value is 0.0001.
			*                    Changing the squeeze ratio sets the camera aperture format to eCUSTOM_FORMAT.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(double,SqueezeRatio);

			/** \enum ECameraGateFit
			* Camera gate fit modes.
			* - \e eNO_FIT            No resoluton gate fit.
			* - \e eVERTICAL_FIT      Fit the resolution gate vertically within the film gate.
			* - \e eHORIZONTAL_FIT    Fit the resolution gate horizontally within the film gate.
			* - \e eFILL_FIT          Fit the resolution gate within the film gate.
			* - \e eOVERSCAN_FIT      Fit the film gate within the resolution gate.
			* - \e eSTRETCH_FIT       Fit the resolution gate to the film gate.
			*/
			enum class CameraGateFit
			{
				NoFit = KFbxCamera::eNO_FIT,
				VerticalFit = KFbxCamera::eVERTICAL_FIT,
				HorizontalFit = KFbxCamera::eHORIZONTAL_FIT,
				FillFit = KFbxCamera::eFILL_FIT,
				OverscanFit = KFbxCamera::eOVERSCAN_FIT,
				StretchFit = KFbxCamera::eSTRETCH_FIT
			};


			/** Compute the angle of view based on the given focal length, the aperture width, and aperture height.
			* \param pFocalLength     The focal length in millimeters
			* \return                 The computed angle of view in degrees
			*/
			double ComputeFieldOfView(double focalLength);

			/** Compute the focal length based on the given angle of view, the aperture width, and aperture height.
			* \param pAngleOfView     The angle of view in degrees
			* \return                 The computed focal length in millimeters
			*/
			double ComputeFocalLength(double angleOfView);
			//@}

			/**
			* \name Background Functions
			*/
			//@{

			/** Get the background image file name.
			* \return     Pointer to the background filename string or \c NULL if not set.
			*/
			/** Set the associated background image file.
			* \param pFileName     The path of the background image file.
			* \remarks             The background image file name must be valid.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^,BackgroundFileName);
			//#ifdef KARCH_DEV_MACOSX_CFM
			//			bool SetBackgroundFile(const FSSpec &pMacFileSpec);
			//			bool SetBackgroundFile(const FSRef &pMacFileRef);
			//			bool SetBackgroundFile(const CFURLRef &pMacURL);
			//#endif		

			//#ifdef KARCH_DEV_MACOSX_CFM
			//			bool GetBackgroundFile(FSSpec &pMacFileSpec) const;
			//			bool GetBackgroundFile(FSRef &pMacFileRef) const;
			//			bool GetBackgroundFile(CFURLRef &pMacURL) const;
			//#endif

			/** Get the media name associated to the background image file.
			* \return     Pointer to the media name string or \c NULL if not set.
			*/
			/** Set the media name associated to the background image file.
			* \param pFileName     The media name of the background image file.
			* \remarks             The media name is a unique name used to identify the background image file.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^,BackgroundMediaName);

			/** \enum ECameraBackgroundDisplayMode Background display modes.
			* - \e eDISABLED
			* - \e eALWAYS
			* - \e eWHEN_MEDIA
			*/
			enum class CameraBackgroundDisplayMode
			{
				Disabled = KFbxCamera::eDISABLED,
				Always = KFbxCamera::eALWAYS,
				WhenMedia = KFbxCamera::eWHEN_MEDIA
			};			

			/** \enum ECameraBackgroundDrawingMode Background drawing modes.
			* - \e eBACKGROUND                 Image is drawn behind models.
			* - \e eFOREGROUND                 Image is drawn in front of models based on alpha channel.
			* - \e eBACKGROUND_AND_FOREGROUND  Image is drawn behind and in front of models depending on alpha channel.
			*/
			enum class CameraBackgroundDrawingMode
			{
				Background = KFbxCamera::eBACKGROUND,
				Foreground = KFbxCamera::eFOREGROUND,
				BackgroundAndForeground = KFbxCamera::eBACKGROUND_AND_FOREGROUND
			};			

			/** \enum ECameraBackgroundPlacementOptions Background placement options.
			* - \e eFIT
			* - \e eCENTER
			* - \e eKEEP_RATIO
			* - \e eCROP
			*/
			enum class CameraBackgroundPlacementOptions
			{
				Fit = 1<<0,
				Center = 1<<1,
				KeepRatio = 1<<2,
				Crop = 1<<3
			};

			/** Set background placement options.
			* \param pOptions     Bitwise concatenation of one or more background placement options.
			*/
			//K_DEPRECATED void SetBackgroundPlacementOptions(kUInt pOptions);

			/** Get background placement options.
			* \return     The bitwise concatenation of the currently set background placement options.
			*/
			VALUE_PROPERTY_GET_DECLARE(kUInt,BackgroundPlacementOptions);

			/** ECamerabackgroundDistanceMode Background distance modes.
			* - \e eRELATIVE_TO_INTEREST
			* - \e eABSOLUTE_FROM_CAMERA
			*/
			enum class CameraBackgroundDistanceMode
			{
				RelativeToInterest = KFbxCamera::eRELATIVE_TO_INTEREST,
				AbsoluteFromCamera = KFbxCamera::eABSOLUTE_FROM_CAMERA
			};			

			/** \enum ECameraSafeAreaStyle Camera safe area display styles.
			* - \e eROUND
			* - \e eSQUARE
			*/
			enum class CameraSafeAreaStyle
			{
				Round = 0,
				Square = 1
			};						

			//@}

			/**
			* \name Render Functions
			* It is the responsibility of the client application to perform the required tasks according to the state
			* of the options that are either set or returned by these methods.
			*/
			//@{

			/** \enum ECameraRenderOptionsUsageTime Render options usage time.
			* - \e eINTERACTIVE
			* - \e eAT_RENDER
			*/
			enum class CameraRenderOptionsUsageTime
			{
				Interactive = KFbxCamera::eINTERACTIVE,
				AtRender = KFbxCamera::eAT_RENDER
			};			

			/** \enum ECameraAntialiasingMethod Antialiasing methods.
			* - \e eOVERSAMPLING_ANTIALIASING
			* - \e eHARDWARE_ANTIALIASING
			*/
			enum class CameraAntialiasingMethod
			{
				OversamplingAntialiasing = KFbxCamera::eOVERSAMPLING_ANTIALIASING,
				HardwareAntialiasing = KFbxCamera::eHARDWARE_ANTIALIASING
			};			

			/** \enum ECameraSamplingType Oversampling types.
			* - \e eUNIFORM
			* - \e eSTOCHASTIC
			*/
			enum class CameraSamplingType
			{
				Uniform = KFbxCamera::eUNIFORM,
				Stochastic = KFbxCamera::eSTOCHASTIC
			};			

			/** \enum ECameraFocusDistanceSource Camera focus sources.
			* - \e eCAMERA_INTEREST
			* - \e eSPECIFIC_DISTANCE
			*/
			enum class CameraFocusDistanceSource
			{
				CameraInterest = KFbxCamera::eCAMERA_INTEREST,
				SpecificDistance = KFbxCamera::eSPECIFIC_DISTANCE
			};			

			//@}			

			/**
			* \name Utility Functions.
			*/
			//@{

			/** Determine if the given bounding box is in the camera's view. 
			* The input points do not need to be ordered in any particular way.
			* \param pWorldToScreen The world to screen transformation. See ComputeWorldToScreen.
			* \param pWorldToCamera The world to camera transformation. 
			Inverse matrix returned from KFbxNode::GetGlobalFromCurrentTake is suitable.
			See KFbxNodeAttribute::GetNode() and KFbxNode::GetGlobalFromCurrentTake().
			* \param pPoints 8 corners of the bounding box.
			* \return true if any of the given points are in the camera's view, false otherwise.
			*/
			/*bool IsBoundingBoxInView( FbxMatrix^ worldToScreen, 
			FbxMatrix% worldToCamera, 
			const KFbxVector4 pPoints[8] ) const;*/

			/** Determine if the given 3d point is in the camera's view. 
			* \param pWorldToScreen The world to screen transformation. See ComputeWorldToScreen.
			* \param pWorldToCamera The world to camera transformation. 
			Inverse matrix returned from KFbxNode::GetGlobalFromCurrentTake is suitable.
			See KFbxNodeAttribute::GetNode() and KFbxNode::GetGlobalFromCurrentTake().
			* \param pPoint World-space point to test.
			* \return true if the given point is in the camera's view, false otherwise.
			*/
			bool IsPointInView( FbxMatrix^ worldToScreen,FbxMatrix^ worldToCamera, FbxVector4^ point);

			/** Compute world space to screen space transformation matrix.
			* \param pPixelHeight The pixel height of the output image.
			* \param pPixelWidth The pixel height of the output image.
			* \param pWorldToCamera The world to camera affine transformation matrix.
			* \return The world to screen space matrix, or the identity matrix on error.
			*/
			FbxMatrix^ ComputeWorldToScreen(int pixelWidth, int pixelHeight, const FbxXMatrix^ worldToCamera);

			/** Compute the perspective matrix for this camera. 
			* Suitable for transforming camera space to normalized device coordinate space.
			* Also suitable for use as an OpenGL projection matrix. Note this fails if the
			* ProjectionType is not ePERSPECTIVE. 
			* \param pPixelHeight The pixel height of the output image.
			* \param pPixelWidth The pixel height of the output image.
			* \param pIncludePostPerspective Indicate that post-projection transformations (offset, roll) 
			*        be included in the output matrix.
			* \return A perspective matrix, or the identity matrix on error.
			*/
			FbxMatrix^ ComputePerspective( int pixelWidth, int pixelHeight, bool includePostPerspective );

			//@}

			//////////////////////////////////////////////////////////////////////////
			//
			// Properties
			//
			//////////////////////////////////////////////////////////////////////////

			// -----------------------------------------------------------------------
			// Geometrical
			// -----------------------------------------------------------------------

			/* This property handles the camera position (XYZ coordinates).
			/*
			/* To access this property do: Position.Get().
			/* To set this property do: Position.Set(fbxDouble3).
			/*
			/* \remarks Default Value is (0.0, 0.0, 0.0)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Position);			

			//** This property handles the camera Up Vector (XYZ coordinates).
			/*
			//* To access this property do: UpVector.Get().
			//* To set this property do: UpVector.Set(fbxDouble3).
			//*
			//* \remarks Default Value is (0.0, 1.0, 0.0)
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,UpVector);			

			///** This property handles the default point (XYZ coordinates) the camera is looking at.
			//*
			//* To access this property do: InterestPosition.Get().
			//* To set this property do: InterestPosition.Set(fbxDouble3).
			//*
			//* \remarks During the computations of the camera position
			//* and orientation, this property is overridden by the
			//* position of a valid target in the parent node.
			//*
			//* \remarks Default Value is (0.0, 0.0, 0.0)
			//*/			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,InterestPosition);

			///** This property handles the camera roll angle in degree(s).
			//*
			//* To access this property do: InterestPosition.Get().
			//* To set this property do: InterestPosition.Set(fbxDouble1).
			//*
			//* Default value is 0.
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,Roll);			

			///** This property handles the camera optical center X, in pixels.
			//* It parameter sets the optical center horizontal offset when the
			//* camera aperture mode is set to \e eHORIZONTAL_AND_VERTICAL. It
			//* has no effect otherwise.
			//*
			//* To access this property do: OpticalCenterX.Get().
			//* To set this property do: OpticalCenterX.Set(fbxDouble1).
			//*
			//* Default value is 0.
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,OpticalCenterX);			

			///** This property handles the camera optical center Y, in pixels.
			//* It sets the optical center horizontal offset when the
			//* camera aperture mode is set to \e eHORIZONTAL_AND_VERTICAL. This
			//* parameter has no effect otherwise.
			//*
			//* To access this property do: OpticalCenterY.Get().
			//* To set this property do: OpticalCenterY.Set(fbxDouble1).
			//*
			//* Default value is 0.
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,OpticalCenterY);			

			///** This property handles the camera RGB values of the background color.
			//*
			//* To access this property do: BackgroundColor.Get().
			//* To set this property do: BackgroundColor.Set(fbxDouble3).
			//*
			//* Default value is black (0, 0, 0)
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,BackgroundColor);			

			///** This property handles the camera turn table angle in degree(s)
			//*
			//* To access this property do: TurnTable.Get().
			//* To set this property do: TurnTable.Set(fbxDouble1).
			//*
			//* Default value is 0.
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,TurnTable);			

			///** This property handles a flags that indicates if the camera displays the
			//* Turn Table icon or not.
			//*
			//* To access this property do: DisplayTurnTableIcon.Get().
			//* To set this property do: DisplayTurnTableIcon.Set(fbxBool1).
			//*
			//* Default value is false (no display).
			//*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DisplayTurnTableIcon);

			//// -----------------------------------------------------------------------
			//// Motion Blur
			//// -----------------------------------------------------------------------

			///** This property handles a flags that indicates if the camera uses
			//* motion blur or not.
			//*
			//* To access this property do: UseMotionBlur.Get().
			//* To set this property do: UseMotionBlur.Set(fbxBool1).
			//*
			//* Default value is false (do not use motion blur).
			//*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseMotionBlur);			

			///** This property handles a flags that indicates if the camera uses
			//* real time motion blur or not.
			//*
			//* To access this property do: UseRealTimeMotionBlur.Get().
			//* To set this property do: UseRealTimeMotionBlur.Set(fbxBool1).
			//*
			//* Default value is false (use real time motion blur).
			//*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseRealTimeMotionBlur);			

			///** This property handles the camera motion blur intensity (in pixels).
			//*
			//* To access this property do: MotionBlurIntensity.Get().
			//* To set this property do: MotionBlurIntensity.Set(fbxDouble1).
			//*
			//* Default value is 1.
			//*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,MotionBlurIntensity);			

			// -----------------------------------------------------------------------
			// Optical
			// -----------------------------------------------------------------------

			/** This property handles the camera aspect ratio mode.
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetAspect() if you want to change its value.
			*
			* Default value is eWINDOW_SIZE.
			*
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(CameraAspectRatioMode,AspectRatioMode);

			/** This property handles the camera aspect width.
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetAspect() if you want to change its value.
			*
			* Default value is 320.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,AspectWidth);

			/** This property handles the camera aspect height.
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetAspect() if you want to change its value.
			*
			* Default value is 200.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,AspectHeight);

			/** This property handles the pixel aspect ratio.
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetPixelRatio() if you want to change its value.
			* Default value is 1.
			* \remarks Value range is [0.050, 20.0].
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,PixelAspectRatio);						

			/** This property handles the aperture mode.
			*
			* Default value is eVERTICAL.
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(CameraApertureMode,ApertureMode);

			/** This property handles the gate fit mode.
			*
			* To access this property do: GateFit.Get().
			* To set this property do: GateFit.Set(ECameraGateFit).
			*
			* Default value is eNO_FIT.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraGateFit,GateFit);						

			/** This property handles the field of view in degrees.
			*
			* To access this property do: FieldOfView.Get().
			* To set this property do: FieldOfView.Set(fbxDouble1).
			*
			* \remarks This property has meaning only when
			* property ApertureMode equals eHORIZONTAL or eVERTICAL.
			*
			* \remarks Default vaule is 40.
			* \remarks Value range is [1.0, 179.0].
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FieldOfView);						


			/** This property handles the X (horizontal) field of view in degrees.
			*
			* To access this property do: FieldOfViewX.Get().
			* To set this property do: FieldOfViewX.Set(fbxDouble1).
			*
			* \remarks This property has meaning only when
			* property ApertureMode equals eHORIZONTAL or eVERTICAL.
			*
			* Default value is 1.
			* \remarks Value range is [1.0, 179.0].
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FieldOfViewX);						

			/** This property handles the Y (vertical) field of view in degrees.
			*
			* To access this property do: FieldOfViewY.Get().
			* To set this property do: FieldOfViewY.Set(fbxDouble1).
			*
			* \remarks This property has meaning only when
			* property ApertureMode equals eHORIZONTAL or eVERTICAL.
			*
			* \remarks Default vaule is 1.
			* \remarks Value range is [1.0, 179.0].
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FieldOfViewY);						

			/** This property handles the focal length (in millimeters).
			*
			* To access this property do: FocalLength.Get().
			* To set this property do: FocalLength.Set(fbxDouble1).
			*
			* Default value is the result of ComputeFocalLength(40.0).
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FocalLength);						

			/** This property handles the camera format.
			*
			* To access this property do: CameraFormat.Get().
			* To set this property do: CameraFormat.Set(ECameraFormat).
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetFormat() if you want to change its value.
			* Default value is eCUSTOM_FORMAT.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraFormat,Camera_Format);

			// -----------------------------------------------------------------------
			// Frame
			// -----------------------------------------------------------------------

			/** This property stores a flag that indicates to use or not a color for
			* the frame.
			*
			* To access this property do: UseFrameColor.Get().
			* To set this property do: UseFrameColor.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseFrameColor);						

			/** This property handles the fame color
			*
			* To access this property do: FrameColor.Get().
			* To set this property do: FrameColor.Set(fbxDouble3).
			*
			* Default value is (0.3, 0.3, 0.3).
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,FrameColor);						

			// -----------------------------------------------------------------------
			// On Screen Display
			// -----------------------------------------------------------------------

			/** This property handles the show name flag.
			*
			* To access this property do: ShowName.Get().
			* To set this property do: ShowName.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowName);						

			/** This property handles the show info on moving flag.
			*
			* To access this property do: ShowInfoOnMoving.Get().
			* To set this property do: ShowInfoOnMoving.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowInfoOnMoving);						

			/** This property handles the draw floor grid flag
			*
			* To access this property do: ShowGrid.Get().
			* To set this property do: ShowGrid.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowGrid);						

			/** This property handles the show optical center flag
			*
			* To access this property do: ShowOpticalCenter.Get().
			* To set this property do: ShowOpticalCenter.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowOpticalCenter);						

			/** This property handles the show axis flag
			*
			* To access this property do: ShowAzimut.Get().
			* To set this property do: ShowAzimut.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowAzimut);						

			/** This property handles the show time code flag
			*
			* To access this property do: ShowTimeCode.Get().
			* To set this property do: ShowTimeCode.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowTimeCode);						

			/** This property handles the show audio flag
			*
			* To access this property do: ShowAudio.Get().
			* To set this property do: ShowAudio.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ShowAudio);						

			/** This property handles the show audio flag
			*
			* To access this property do: AudioColor.Get().
			* To set this property do: AudioColor.Set(fbxDouble3).
			*
			* Default value is (0.0, 1.0, 0.0)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,AudioColor);						

			// -----------------------------------------------------------------------
			// Clipping Planes
			// -----------------------------------------------------------------------

			/** This property handles the near plane distance.
			*
			* \remarks This Property is in a Read Only mode.
			* \remarks Please use function SetNearPlane() if you want to change its value.
			* Default value is 10.
			* \remarks Value range is [0.001, 600000.0].
			*/
			//REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,NearPlane);						

			/** This property handles the far plane distance.
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetPixelRatio() if you want to change its value
			* Default value is 4000
			* \remarks Value range is [0.001, 600000.0]
			*/
			//REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FarPlane);						


			/** This property indicates that the clip planes should be automatically computed.
			*
			* To access this property do: AutoComputeClipPlanes.Get().
			* To set this property do: AutoComputeClipPlanes.Set(fbxBool1).
			*
			* When this property is set to true, the NearPlane and FarPlane values are
			* ignored. Note that not all applications support this flag.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,AutoComputeClipPlanes);							


			// -----------------------------------------------------------------------
			// Camera Film Setting
			// -----------------------------------------------------------------------

			/** This property handles the film aperture width (in inches).
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetApertureWidth()
			* or SetApertureFormat() if you want to change its value
			* Default value is 0.8160
			* \remarks Value range is [0.0001, +inf[
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FilmWidth);							

			/** This property handles the film aperture height (in inches).
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetApertureHeight()
			* or SetApertureFormat() if you want to change its value
			* Default value is 0.6120
			* \remarks Value range is [0.0001, +inf[
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FilmHeight);							

			/** This property handles the film aperture aspect ratio.
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetApertureFormat() if you want to change its value
			* Default value is (FilmWidth / FilmHeight)
			* \remarks Value range is [0.0001, +inf[
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FilmAspectRatio);						

			/** This property handles the film aperture squeeze ratio.
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetSqueezeRatio()
			* or SetApertureFormat() if you want to change its value
			* Default value is 1.0
			* \remarks Value range is [0.0001, +inf[
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FilmSqueezeRatio);						

			/** This property handles the film aperture format.
			*
			* \remarks This Property is in a Read Only mode
			* \remarks Please use function SetApertureFormat()
			* if you want to change its value
			* Default value is eCUSTOM_APERTURE_FORMAT
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraApertureFormat,FilmFormat);

			/** This property handles the offset from the center of the film aperture,
			* defined by the film height and film width. The offset is measured
			* in inches.
			*
			* To access this property do: FilmOffset.Get().
			* To set this property do: FilmOffset.Set(fbxDouble2).
			*
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble2TypedProperty,FilmOffset);


			// -----------------------------------------------------------------------
			// Camera View Widget Option
			// -----------------------------------------------------------------------

			/** This property handles the view frustrum flag.
			*
			* To access this property do: ViewFrustum.Get().
			* To set this property do: ViewFrustum.Set(fbxBool1).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ViewFrustum);

			/** This property handles the view frustrum near and far plane flag.
			*
			* To access this property do: ViewFrustumNearFarPlane.Get().
			* To set this property do: ViewFrustumNearFarPlane.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ViewFrustumNearFarPlane);


			/** This property handles the view frustrum back plane mode.
			*
			* To access this property do: ViewFrustumBackPlaneMode.Get().
			* To set this property do: ViewFrustumBackPlaneMode.Set(ECameraBackgroundDisplayMode).
			*
			* Default value is eWHEN_MEDIA
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraBackgroundDisplayMode,ViewFrustumBackPlaneMode);

			/** This property handles the view frustrum back plane distance.
			*
			* To access this property do: BackPlaneDistance.Get().
			* To set this property do: BackPlaneDistance.Set(fbxDouble1).
			*
			* Default value is 100.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,BackPlaneDistance);

			/** This property handles the view frustrum back plane distance mode.
			*
			* To access this property do: BackPlaneDistanceMode.Get().
			* To set this property do: BackPlaneDistanceMode.Set(ECameraBackgroundDistanceMode).
			*
			* Default value is eRELATIVE_TO_INTEREST
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraBackgroundDistanceMode,BackPlaneDistanceMode);

			/** This property handles the view camera to look at flag.
			*
			* To access this property do: ViewCameraToLookAt.Get().
			* To set this property do: ViewCameraToLookAt.Set(fbxBool1).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,ViewCameraToLookAt);

			// -----------------------------------------------------------------------
			// Camera Lock Mode
			// -----------------------------------------------------------------------

			/** This property handles the lock mode.
			*
			* To access this property do: LockMode.Get().
			* To set this property do: LockMode.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,LockMode);

			/** This property handles the lock interest navigation flag.
			*
			* To access this property do: LockInterestNavigation.Get().
			* To set this property do: LockInterestNavigation.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,LockInterestNavigation);

			// -----------------------------------------------------------------------
			// Background Image Display Options
			// -----------------------------------------------------------------------

			/** This property handles the fit image flag.
			*
			* To access this property do: FitImage.Get().
			* To set this property do: FitImage.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FitImage);

			/** This property handles the crop flag.
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Crop);

			/** This property handles the center flag.
			*
			* To access this property do: Center.Get().
			* To set this property do: Center.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Center);

			/** This property handles the keep ratio flag.
			*
			* To access this property do: KeepRatio.Get().
			* To set this property do: KeepRatio.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,KeepRatio);

			/** This property handles the background mode flag.
			*
			* To access this property do: BackgroundMode.Get().
			* To set this property do: BackgroundMode.Set(ECameraBackgroundDrawingMode).
			*
			* Default value is eBACKGROUND.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraBackgroundDrawingMode,BackgroundMode);

			/** This property handles the background alpha threshold value.
			*
			* To access this property do: BackgroundAlphaTreshold.Get().
			* To set this property do: BackgroundAlphaTreshold.Set(fbxDouble1).
			*
			* Default value is 0.5.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,BackgroundAlphaTreshold);

			// -----------------------------------------------------------------------
			// Foreground Image Display Options
			// -----------------------------------------------------------------------

			/** This property handles the fit image for front plate flag.
			*
			* To access this property do: FrontPlateFitImage.Get().
			* To set this property do: FrontPlateFitImage.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FrontPlateFitImage);

			/** This property handles the front plane crop flag.
			*
			* To access this property do: FrontPlateCrop.Get().
			* To set this property do: FrontPlateCrop.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FrontPlateCrop);

			/** This property handles the front plane center flag.
			*
			* To access this property do: FrontPlateCenter.Get().
			* To set this property do: FrontPlateCenter.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FrontPlateCenter);

			/** This property handles the front plane keep ratio flag.
			*
			* To access this property do: FrontPlateKeepRatio.Get().
			* To set this property do: FrontPlateKeepRatio.Set(fbxBool1).
			*
			* Default value is true.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FrontPlateKeepRatio);


			/** This property handles the front plane show flag.
			*
			* To access this property do: ShowFrontPlate.Get().
			* To set this property do: ShowFrontPlate.Set(fbxBool1).
			*
			* Default value is false.
			* \remarks this replaces ForegroundTransparent 
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool, ShowFrontPlate);

			/** This property handles the view frustrum front plane mode.
			*
			* To access this property do: ViewFrustumFrontPlaneMode.Get().
			* To set this property do: ViewFrustumFrontPlaneMode.Set(ECameraBackgroundDisplayMode).
			*
			* Default value is eWHEN_MEDIA
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraBackgroundDisplayMode,ViewFrustumFrontPlaneMode);

			/** This property handles the view frustrum front plane distance.
			*
			* To access this property do: FrontPlaneDistance.Get().
			* To set this property do: FrontPlaneDistance.Set(fbxDouble1).
			*
			* Default value is 100.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FrontPlaneDistance);

			/** This property handles the view frustrum front plane distance mode.
			*
			* To access this property do: FrontPlaneDistanceMode.Get().
			* To set this property do: FrontPlaneDistanceMode.Set(ECameraBackgroundDistanceMode).
			*
			* Default value is eRELATIVE_TO_INTEREST
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraBackgroundDistanceMode,FrontPlaneDistanceMode);

			/** This property handles the foreground alpha value.
			*
			* To access this property do: ForegroundAlpha.Get().
			* To set this property do: ForegroundAlpha.Set(fbxDouble1).
			*
			* Default value is 0.5.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,ForegroundAlpha);


			/** This property has the foreground textures connected to it.
			*
			* To access this property do: ForegroundTexture.Get().
			* To set this property do: ForegroundTexture.Set(fbxReference).
			*
			* \remarks they are connected as source objects
			*/
			//KFbxTypedProperty<fbxReference> ForegroundTexture;

			/** This property has the background textures connected to it.
			*
			* To access this property do: BackgroundTexture.Get().
			* To set this property do: BackgroundTexture.Set(fbxReference).
			*
			* \remarks they are connected as source objects
			*/
			//KFbxTypedProperty<fbxReference> BackgroundTexture;


			// -----------------------------------------------------------------------
			// Safe Area
			// -----------------------------------------------------------------------

			/** This property handles the display safe area flag.
			*
			* To access this property do: DisplaySafeArea.Get().
			* To set this property do: DisplaySafeArea.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DisplaySafeArea);

			/** This property handles the display safe area on render flag.
			*
			* To access this property do: DisplaySafeAreaOnRender.Get().
			* To set this property do: DisplaySafeAreaOnRender.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DisplaySafeAreaOnRender);

			/** This property handles the display safe area display style.
			*
			* To access this property do: SafeAreaDisplayStyle.Get().
			* To set this property do: SafeAreaDisplayStyle.Set(ECameraSafeAreaStyle).
			*
			* Default value is eSQUARE
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraSafeAreaStyle,SafeAreaDisplayStyle);

			/** This property handles the display safe area aspect ratio.
			*
			* To access this property do: SafeAreaDisplayStyle.Get().
			* To set this property do: SafeAreaAspectRatio.Set(fbxDouble1).
			*
			* Default value is 1.33333333333333
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,SafeAreaAspectRatio);

			// -----------------------------------------------------------------------
			// 2D Magnifier
			// -----------------------------------------------------------------------

			/** This property handles the use 2d magnifier zoom flag.
			*
			* To access this property do: Use2DMagnifierZoom.Get().
			* To set this property do: Use2DMagnifierZoom.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,Use2DMagnifierZoom);

			/** This property handles the 2d magnifier zoom value.
			*
			* To access this property do: _2DMagnifierZoom.Get().
			* To set this property do: _2DMagnifierZoom.Set(fbxDouble1).
			*
			* Default value is 100.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,_2DMagnifierZoom);

			/** This property handles the 2d magnifier X value.
			*
			* To access this property do: _2DMagnifierX.Get().
			* To set this property do: _2DMagnifierX.Set(fbxDouble1).
			*
			* Default value is 50.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,_2DMagnifierX);						

			/** This property handles the 2d magnifier Y value.
			*
			* To access this property do: _2DMagnifierY.Get().
			* To set this property do: _2DMagnifierY.Set(fbxDouble1).
			*
			* Default value is 50.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,_2DMagnifierY);						

			// -----------------------------------------------------------------------
			// Projection Type: Ortho, Perspective
			// -----------------------------------------------------------------------

			/** This property handles the projection type
			*
			* To access this property do: ProjectionType.Get().
			* To set this property do: ProjectionType.Set(ECameraProjectionType).
			*
			* Default value is ePERSPECTIVE.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraProjectionType,ProjectionType);

			/** This property handles the otho zoom
			*
			* To access this property do: OrthoZoom.Get().
			* To set this property do: OrthoZoom.Set(fbxDouble1).
			*
			* Default value is 1.0.
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,OrthoZoom);						

			// -----------------------------------------------------------------------
			// Depth Of Field & Anti Aliasing
			// -----------------------------------------------------------------------

			/** This property handles the use real time DOF and AA flag
			*
			* To access this property do: UseRealTimeDOFAndAA.Get().
			* To set this property do: UseRealTimeDOFAndAA.Set(fbxBool1).
			*
			* Default value is false.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseRealTimeDOFAndAA);

			/** This property handles the use depth of field flag
			*
			* To access this property do: UseDepthOfField.Get().
			* To set this property do: UseDepthOfField.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseDepthOfField);

			/** This property handles the focus source
			*
			* To access this property do: FocusSource.Get().
			* To set this property do: FocusSource.Set(ECameraFocusDistanceSource).
			*
			* Default value is eCAMERA_INTEREST
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraFocusDistanceSource,FocusSource);

			/** This property handles the focus angle (in degrees)
			*
			* To access this property do: FocusAngle.Get().
			* To set this property do: FocusAngle.Set(fbxDouble1).
			*
			* Default value is 3.5
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FocusAngle);

			/** This property handles the focus distance
			*
			* To access this property do: FocusDistance.Get().
			* To set this property do: FocusDistance.Set(fbxDouble1).
			*
			* Default value is 200.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FocusDistance);

			/** This property handles the use anti aliasing flag
			*
			* To access this property do: UseAntialiasing.Get().
			* To set this property do: UseAntialiasing.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseAntialiasing);

			/** This property handles the anti aliasing intensity
			*
			* To access this property do: AntialiasingIntensity.Get().
			* To set this property do: AntialiasingIntensity.Set(fbxDouble1).
			*
			* Default value is 0.77777
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,AntialiasingIntensity);

			/** This property handles the anti aliasing method
			*
			* To access this property do: AntialiasingMethod.Get().
			* To set this property do: AntialiasingMethod.Set(ECameraAntialiasingMethod).
			*
			* Default value is eOVERSAMPLING_ANTIALIASING
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraAntialiasingMethod,AntialiasingMethod);

			// -----------------------------------------------------------------------
			// Accumulation Buffer
			// -----------------------------------------------------------------------

			/** This property handles the use accumulation buffer flag
			*
			* To access this property do: UseAccumulationBuffer.Get().
			* To set this property do: UseAccumulationBuffer.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseAccumulationBuffer);

			/** This property handles the frame sampling count
			*
			* To access this property do: FrameSamplingCount.Get().
			* To set this property do: FrameSamplingCount.Set(fbxInteger1).
			*
			* Default value is 7
			*/
			VALUE_PROPERTY_GETSET_DECLARE(int,FrameSamplingCount);

			/** This property handles the frame sampling type
			*
			* To access this property do: FrameSamplingType.Get().
			* To set this property do: FrameSamplingType.Set(ECameraSamplingType).
			*
			* Default value is eSTOCHASTIC
			*/
			VALUE_PROPERTY_GETSET_DECLARE(CameraSamplingType,FrameSamplingType);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS										
		public:

			// Clone
			CLONE_DECLARE();										
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}