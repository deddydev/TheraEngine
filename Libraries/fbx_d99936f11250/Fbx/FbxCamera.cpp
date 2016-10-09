#pragma once
#include "stdafx.h"
#include "FbxCamera.h"
#include "FbxNodeAttribute.h"
#include "FbxMatrix.h"
#include "FbxXMatrix.h"
#include "FbxVector4.h"
#include "FbxString.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxTypedProperty.h"


#define GETSET_FROM_TYPED_PROPERTY(PropType,PropName)\
	PropType FbxCamera::PropName::get(){	return _Ref()->PropName.Get();}\
	void FbxCamera::PropName::set(PropType value){_Ref()->PropName.Set(value);}

#define GETSET_FROM_Double_PROPERTY(PropType,PropName)\
	REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCamera,PropName,PropType,PropName);

#define GETSET_FROM_ENUM_TYPED_PROPERTY(PropType,PropName,NativeType)\
	PropType FbxCamera::PropName::get(){return (PropType)_Ref()->PropName.Get();}\
	void FbxCamera::PropName::set(PropType value){_Ref()->PropName.Set((NativeType)value);}


namespace Skill
{
	namespace FbxSDK
	{		
		FBXOBJECT_DEFINITION(FbxCamera,KFbxCamera);

		FbxCamera::CameraFormat FbxCamera::Format::get()
		{
			return (CameraFormat)_Ref()->GetFormat();
		}
		void FbxCamera::Format::set(CameraFormat value)
		{
			_Ref()->SetFormat((KFbxCamera::ECameraFormat)value);
		}			
		void FbxCamera::SetAspect(CameraAspectRatioMode ratioMode, double width, double height)
		{
			_Ref()->SetAspect((KFbxCamera::ECameraAspectRatioMode)ratioMode,width,height);
		}
		FbxCamera::CameraAspectRatioMode FbxCamera::AspectRatioMode::get()
		{
			return (CameraAspectRatioMode)_Ref()->GetAspectRatioMode(); 
		}						
		double FbxCamera::PixelRatio::get()
		{
			return _Ref()->GetPixelRatio(); 
		}
		void FbxCamera::PixelRatio::set(double value)
		{
			_Ref()->SetPixelRatio(value); 
		}											
		double FbxCamera::NearPlane::get()
		{
			return _Ref()->GetNearPlane(); 
		}
		void FbxCamera::NearPlane::set(double value)
		{
			_Ref()->SetNearPlane(value); 
		}						
		double FbxCamera::FarPlane::get()
		{
			return _Ref()->GetFarPlane(); 
		}
		void FbxCamera::FarPlane::set(double value)
		{
			_Ref()->SetFarPlane(value); 
		}			
		FbxCamera::CameraApertureFormat FbxCamera::ApertureFormat::get()
		{
			return(CameraApertureFormat)_Ref()->GetApertureFormat(); 
		}
		void FbxCamera::ApertureFormat::set(CameraApertureFormat value)
		{
			_Ref()->SetApertureFormat((KFbxCamera::ECameraApertureFormat)value);
		}			
		FbxCamera::CameraApertureMode FbxCamera::ApertureMode::get()
		{
			return(CameraApertureMode)_Ref()->GetApertureMode(); 
		}
		void FbxCamera::ApertureMode::set(CameraApertureMode value)
		{
			_Ref()->SetApertureMode((KFbxCamera::ECameraApertureMode)value);
		}	
		double FbxCamera::ApertureWidth::get()
		{
			return _Ref()->GetApertureWidth(); 
		}
		void FbxCamera::ApertureWidth::set(double value)
		{
			_Ref()->SetApertureWidth(value); 
		}						
		double FbxCamera::ApertureHeight::get()
		{
			return _Ref()->GetApertureHeight(); 
		}
		void FbxCamera::ApertureHeight::set(double value)
		{
			_Ref()->SetApertureHeight(value);
		}
		double FbxCamera::SqueezeRatio::get()
		{
			return _Ref()->GetSqueezeRatio();
		}
		void FbxCamera::SqueezeRatio::set(double value)
		{
			_Ref()->SetSqueezeRatio(value);
		}						
		double FbxCamera::ComputeFieldOfView(double focalLength)
		{
			return _Ref()->ComputeFieldOfView(focalLength);
		}
		double FbxCamera::ComputeFocalLength(double angleOfView)
		{
			return _Ref()->ComputeFocalLength(angleOfView);
		}			
		System::String^ FbxCamera::BackgroundFileName::get()
		{
			return gcnew System::String(_Ref()->GetBackgroundFileName());
		}
		void FbxCamera::BackgroundFileName::set(System::String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetBackgroundFileName(v);
			FREECHARPOINTER(v);
		}				
		System::String^ FbxCamera::BackgroundMediaName::get()
		{
			return gcnew System::String(_Ref()->GetBackgroundMediaName());
		}
		void FbxCamera::BackgroundMediaName::set(System::String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetBackgroundMediaName(v);
			FREECHARPOINTER(v);
		}
		kUInt FbxCamera::BackgroundPlacementOptions::get()
		{
			return _Ref()->GetBackgroundPlacementOptions();
		}			
		bool FbxCamera::IsPointInView( FbxMatrix^ worldToScreen,FbxMatrix^ worldToCamera, FbxVector4^ point )
		{
			return _Ref()->IsPointInView( *worldToScreen->_Ref(),*worldToCamera->_Ref(), *point->_Ref() );
		}
		FbxMatrix^ FbxCamera::ComputeWorldToScreen(int pixelWidth, int pixelHeight, const FbxXMatrix^ worldToCamera)
		{
			return gcnew FbxMatrix(_Ref()->ComputeWorldToScreen(pixelWidth, pixelHeight, *worldToCamera->_FbxXMatrix));
		}
		FbxMatrix^ FbxCamera::ComputePerspective( int pixelWidth, int pixelHeight, bool includePostPerspective )
		{
			return gcnew FbxMatrix(_Ref()->ComputePerspective(pixelWidth, pixelHeight,includePostPerspective));
		}

		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,Position);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,UpVector);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,InterestPosition);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,Roll);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,OpticalCenterX);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,OpticalCenterY);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,BackgroundColor);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,TurnTable);
		GETSET_FROM_TYPED_PROPERTY(bool,DisplayTurnTableIcon);
		GETSET_FROM_TYPED_PROPERTY(bool,UseMotionBlur);
		GETSET_FROM_TYPED_PROPERTY(bool,UseRealTimeMotionBlur);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,MotionBlurIntensity);

		//GETSET_FROM_ENUM_TYPED_PROPERTY(CameraAspectRatioMode,AspectRatioMode,KFbxCamera::camera);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,AspectWidth);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,AspectHeight);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,PixelAspectRatio);
		//GETSET_FROM_ENUM_TYPED_PROPERTY(CameraApertureMode,ApertureMode,KFbxCamera::ECameraApertureMode);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraGateFit,GateFit,KFbxCamera::ECameraGateFit);			


		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FieldOfView);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FieldOfViewX);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FieldOfViewY);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FocalLength);

		FbxCamera::CameraFormat FbxCamera::Camera_Format::get(){return (FbxCamera::CameraFormat)_Ref()->CameraFormat.Get();}
		void FbxCamera::Camera_Format::set(FbxCamera::CameraFormat value){_Ref()->CameraFormat.Set((KFbxCamera::ECameraFormat)value);}


		GETSET_FROM_TYPED_PROPERTY(bool,UseFrameColor);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,FrameColor);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowName);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowInfoOnMoving);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowGrid);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowOpticalCenter);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowAzimut);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowTimeCode);
		GETSET_FROM_TYPED_PROPERTY(bool,ShowAudio);
		GETSET_FROM_Double_PROPERTY(FbxDouble3TypedProperty,AudioColor);
		//GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,NearPlane);
		//GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FarPlane);
		GETSET_FROM_TYPED_PROPERTY(bool,AutoComputeClipPlanes);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FilmWidth);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FilmHeight);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FilmAspectRatio);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FilmSqueezeRatio);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraApertureFormat,FilmFormat,KFbxCamera::ECameraApertureFormat);




		GETSET_FROM_Double_PROPERTY(FbxDouble2TypedProperty,FilmOffset);
		GETSET_FROM_TYPED_PROPERTY(bool,ViewFrustum);
		GETSET_FROM_TYPED_PROPERTY(bool,ViewFrustumNearFarPlane);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraBackgroundDisplayMode,ViewFrustumBackPlaneMode,KFbxCamera::ECameraBackgroundDisplayMode);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,BackPlaneDistance);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraBackgroundDistanceMode,BackPlaneDistanceMode,KFbxCamera::ECameraBackgroundDistanceMode);
		GETSET_FROM_TYPED_PROPERTY(bool,ViewCameraToLookAt);
		GETSET_FROM_TYPED_PROPERTY(bool,LockMode);
		GETSET_FROM_TYPED_PROPERTY(bool,LockInterestNavigation);
		GETSET_FROM_TYPED_PROPERTY(bool,FitImage);
		GETSET_FROM_TYPED_PROPERTY(bool,Crop);
		GETSET_FROM_TYPED_PROPERTY(bool,Center);
		GETSET_FROM_TYPED_PROPERTY(bool,KeepRatio);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraBackgroundDrawingMode,BackgroundMode,KFbxCamera::ECameraBackgroundDrawingMode);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,BackgroundAlphaTreshold);
		GETSET_FROM_TYPED_PROPERTY(bool,FrontPlateFitImage);
		GETSET_FROM_TYPED_PROPERTY(bool,FrontPlateCrop);
		GETSET_FROM_TYPED_PROPERTY(bool,FrontPlateCenter);
		GETSET_FROM_TYPED_PROPERTY(bool,FrontPlateKeepRatio);
		GETSET_FROM_TYPED_PROPERTY(bool, ShowFrontPlate);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraBackgroundDisplayMode,ViewFrustumFrontPlaneMode,KFbxCamera::ECameraBackgroundDisplayMode);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FrontPlaneDistance);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraBackgroundDistanceMode,FrontPlaneDistanceMode,KFbxCamera::ECameraBackgroundDistanceMode);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,ForegroundAlpha);
		GETSET_FROM_TYPED_PROPERTY(bool,DisplaySafeArea);
		GETSET_FROM_TYPED_PROPERTY(bool,DisplaySafeAreaOnRender);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraSafeAreaStyle,SafeAreaDisplayStyle,KFbxCamera::ECameraSafeAreaStyle);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,SafeAreaAspectRatio);
		GETSET_FROM_TYPED_PROPERTY(bool,Use2DMagnifierZoom);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,_2DMagnifierZoom);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,_2DMagnifierX);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,_2DMagnifierY);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraProjectionType,ProjectionType,KFbxCamera::ECameraProjectionType);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,OrthoZoom);
		GETSET_FROM_TYPED_PROPERTY(bool,UseRealTimeDOFAndAA);
		GETSET_FROM_TYPED_PROPERTY(bool,UseDepthOfField);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraFocusDistanceSource,FocusSource,KFbxCamera::ECameraFocusDistanceSource);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FocusAngle);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,FocusDistance);
		GETSET_FROM_TYPED_PROPERTY(bool,UseAntialiasing);
		GETSET_FROM_Double_PROPERTY(FbxDouble1TypedProperty,AntialiasingIntensity);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraAntialiasingMethod,AntialiasingMethod,KFbxCamera::ECameraAntialiasingMethod);
		GETSET_FROM_TYPED_PROPERTY(bool,UseAccumulationBuffer);
		GETSET_FROM_TYPED_PROPERTY(int,FrameSamplingCount);
		GETSET_FROM_ENUM_TYPED_PROPERTY(FbxCamera::CameraSamplingType,FrameSamplingType,KFbxCamera::ECameraSamplingType);



#ifndef DOXYGEN_SHOULD_SKIP_THIS															
		CLONE_DEFINITION(FbxCamera,KFbxCamera);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}