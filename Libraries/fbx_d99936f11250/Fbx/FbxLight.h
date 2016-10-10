#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"
#include "FbxString.h"
#include "FbxColor.h"


{
	namespace FbxSDK
	{
		ref class FbxTexture;
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxDouble1TypedProperty;
		ref class FbxDouble3TypedProperty;
		ref class FbxStringTypedProperty;
		/** \brief This node attribute contains methods for accessing the properties of a light.
		* \nosubgrouping
		*/
		public ref class FbxLight : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxLight);
		internal:
			FbxLight(KFbxLight* instance) : FbxNodeAttribute(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxLight);

		protected:
			virtual void CollectManagedMemory()override;
		public:

			//! Return the type of node attribute which is EAttributeType::eLIGHT.
			//virtual EAttributeType GetAttributeType() const;

			/**
			* \name Light Properties
			*/
			//@{

			/** \enum ELightType Light types.
			* - \e ePOINT
			* - \e eDIRECTIONAL
			* - \e eSPOT
			*/
			enum class LightType
			{
				Point = KFbxLight::ePOINT, 
				Directional= KFbxLight::eDIRECTIONAL, 
				Spot= KFbxLight::eSPOT
			};

			/** \enum EDecayType     Decay types. Used for setting the attenuation of the light.
			* - \e eNONE          No decay. The light's intensity will not diminish with distance.		
			* - \e eLINEAR        Linear decay. The light's intensity will diminish linearly with the distance from the light.
			* - \e eQUADRATIC     Quadratic decay. The light's intensity will diminish with the squared distance from the light.
			*                     This is the most physically accurate decay rate.
			* - \e eCUBIC         Cubic decay. The light's intensity will diminish with the cubed distance from the light.
			*/
			enum class DecayType
			{
				None = KFbxLight::eNONE,
				Linear= KFbxLight::eLINEAR,
				Quadratic= KFbxLight::eQUADRATIC,
				Cubic= KFbxLight::eCUBIC
			};		
		public:
			/** Get the light state.
			* \return     Pointer to the texture cast by the light shadow, or \c NULL if the shadow texture has not been set.
			*/
			/** Set the shadow texture for the light.
			* \param pTexture     The texture cast by the light shadow.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxTexture,ShadowTexture);
			/**
			* \name Gobo properties
			*/
			//@{
						

#ifdef KARCH_DEV_MACOSX_CFM
			bool GetFile(FSSpec &pMacFileSpec) const;
			bool GetFile(FSRef &pMacFileRef) const;
			bool GetFile(CFURLRef &pMacURL) const;
#endif


			//@}

			/**
			* \name Light Property Names
			*/
			//@{		
			 
			static const String^ SLightType = "LightType";
			static const String^ SCastLight = "CastLight";
			static const String^ SDrawVolumetricLight = "OnObjectDrawVolumetricLight";
			static const String^ SDrawGroundProjection = "DrawGroundProjection";
			static const String^ SDrawFrontFacingVolumetricLight  = "DrawFrontFacingVolumetricLight";
			static const String^ SColor =  "Color";
			static const String^ SIntensity = "Intensity";
			static const String^ SHotSpot = "HotSpot";
			static const String^ SConeAngle = "Cone angle";
			static const String^ SFog = "Fog";
			static const String^ SDecayTyp = "DecayTyp";
			static const String^ SDecayStart = "DecayStart";
			static const String^ SFileName = "FileName";
			static const String^ SEnableNearAttenuation = "EnableNearAttenuation";
			static const String^ SNearAttenuationStart = "NearAttenuationStart";
			static const String^ SNearAttenuationEnd = "NearAttenuationEnd";
			static const String^ SEnableFarAttenuation = "EnableFarAttenuation";
			static const String^ SFarAttenuationStart   = "FarAttenuationStart";
			static const String^ SFarAttenuationEnd = "FarAttenuationEnd";
			static const String^ SCastShadows = "CastShadows";
			static const String^ SShadowColor = "ShadowColor";
			//@}

			/**
			* \name Light Property Default Values
			*/

			static const LightType DefaultLightType = (LightType)KFbxLight::sDefaultLightType;
			static const bool DefaultCastLight = KFbxLight::sDefaultCastLight;
			static const bool DefaultDrawVolumetricLight = KFbxLight::sDefaultDrawVolumetricLight;
			static const bool DefaultDrawGroundProjection= KFbxLight::sDefaultDrawGroundProjection;
			static const bool DefaultDrawFrontFacingVolumetricLight= KFbxLight::sDefaultDrawFrontFacingVolumetricLight;
			static const FbxColor^ DefaultColor = gcnew FbxColor(KFbxLight::sDefaultColor.mData[0],
				KFbxLight::sDefaultColor.mData[1],KFbxLight::sDefaultColor.mData[2]);
			static const fbxDouble1	DefaultIntensity = KFbxLight::sDefaultIntensity;
			static const fbxDouble1	DefaultHotSpot= KFbxLight::sDefaultHotSpot;
			static const fbxDouble1	DefaultConeAngle= KFbxLight::sDefaultConeAngle;
			static const fbxDouble1	DefaultFog= KFbxLight::sDefaultFog;
			static const DecayType	DefaultDecayType = (DecayType)KFbxLight::sDefaultDecayType; 
			static const fbxDouble1	DefaultDecayStart= KFbxLight::sDefaultDecayStart;
			//static const FbxString^ DefaultFileName = gcnew FbxString(&KFbxLight::sDefaultFileName);
			static const fbxBool1	DefaultEnableNearAttenuation= KFbxLight::sDefaultEnableNearAttenuation;
			static const fbxDouble1	DefaultNearAttenuationStart= KFbxLight::sDefaultNearAttenuationStart;
			static const fbxDouble1	DefaultNearAttenuationEnd= KFbxLight::sDefaultNearAttenuationEnd;
			static const fbxBool1	DefaultEnableFarAttenuation= KFbxLight::sDefaultEnableFarAttenuation;
			static const fbxDouble1	DefaultFarAttenuationStart= KFbxLight::sDefaultFarAttenuationStart;
			static const fbxDouble1	DefaultFarAttenuationEnd= KFbxLight::sDefaultFarAttenuationEnd;
			static const fbxBool1	DefaultCastShadows= KFbxLight::sDefaultCastShadows;
			static const FbxColor^   DefaultShadowColor = gcnew FbxColor(KFbxLight::sDefaultShadowColor.mData[0],
				KFbxLight::sDefaultShadowColor.mData[1],KFbxLight::sDefaultShadowColor.mData[2]);			

			//////////////////////////////////////////////////////////////////////////
			//
			// Properties
			//
			//////////////////////////////////////////////////////////////////////////

			/**
			* \name Properties
			*/
			//@{	

			/** This property handles the light type.
			*
			* To access this property do: LightType.Get().
			* To set this property do: LightType.Set(ELightType).
			*
			* Default value is ePOINT
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxLight::LightType,Light_Type);


			/** This property handles the cast light on object flag.
			*
			* To access this property do: CastLight.Get().
			* To set this property do: CastLight.Set(fbxBool1).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,CastLight);

			/** This property handles the draw volumetric ligtht flag.
			*
			* To access this property do: DrawVolumetricLight.Get().
			* To set this property do: DrawVolumetricLight.Set(fbxBool1).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DrawVolumetricLight);

			/** This property handles the draw ground projection flag.
			*
			* To access this property do: DrawGroundProjection.Get().
			* To set this property do: DrawGroundProjection.Set(fbxBool1).
			*
			* Default value is true
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DrawGroundProjection);

			/** This property handles the draw facing volumetric projection flag.
			*
			* To access this property do: DrawFrontFacingVolumetricLight.Get().
			* To set this property do: DrawFrontFacingVolumetricLight.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,DrawFrontFacingVolumetricLight);

			/** This property handles the light color.
			*
			* To access this property do: Color.Get().
			* To set this property do: Color.Set(fbxDouble3).
			*
			* Default value is (1.0, 1.0, 1.0)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Color);

			/** This property handles the light intensity.
			*
			* To access this property do: Intensity.Get().
			* To set this property do: Intensity.Set(fbxDouble1).
			*
			* Default value is 100.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,Intensity);

			/** This property handles the light inner cone angle (in degrees). Also know as the HotSpot!
			*
			* To access this property do: HotSpot.Get().
			* To set this property do: HotSpot.Set(fbxDouble1).
			*
			* Default value is 45.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,HotSpot);

			/** This property handles the light outer cone angle (in degrees). Also known as the Falloff
			*
			* To access this property do: ConeAngle.Get().
			* To set this property do: ConeAngle.Set(fbxDouble1).
			*
			* Default value is 45.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,ConeAngle);

			/** This property handles the light fog intensity
			*
			* To access this property do: Fog.Get().
			* To set this property do: Fog.Set(fbxDouble1).
			*
			* Default value is 50.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,Fog);

			/** This property handles the decay type 
			*
			* To access this property do: DecayType.Get().
			* To set this property do: DecayType.Set(EDecayType).
			*
			* Default value is eNONE
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxLight::DecayType,Decay_Type);

			/** This property handles the decay start distance
			*
			* To access this property do: DecayStart.Get().
			* To set this property do: DecayStart.Set(fbxDouble1).
			*
			* Default value is 0.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,DecayStart);

			/** This property handles the gobo file name
			*
			* To access this property do: FileName.Get().
			* To set this property do: FileName.Set(fbxString).
			*
			* Default value is ""
			*/
			REF_PROPERTY_GET_DECLARE(FbxStringTypedProperty,FileName);

			/** This property handles the enable near attenuation flag
			*
			* To access this property do: EnableNearAttenuation.Get().
			* To set this property do: EnableNearAttenuation.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,EnableNearAttenuation);

			/** This property handles the near attenuation start distance
			*
			* To access this property do: NearAttenuationStart.Get().
			* To set this property do: NearAttenuationStart.Set(fbxDouble1).
			*
			* Default value is 0.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,NearAttenuationStart);

			/** This property handles the near end attenuation 
			*
			* To access this property do: NearAttenuationEnd.Get().
			* To set this property do: NearAttenuationEnd.Set(fbxDouble1).
			*
			* Default value is 0.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,NearAttenuationEnd);

			/** This property handles the enable far attenuation flag
			*
			* To access this property do: EnableFarAttenuation.Get().
			* To set this property do: EnableFarAttenuation.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,EnableFarAttenuation);

			/** This property handles the far attenuation start distance
			*
			* To access this property do: FarAttenuationStart.Get().
			* To set this property do: FarAttenuationStart.Set(fbxDouble1).
			*
			* Default value is 0.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FarAttenuationStart);

			/** This property handles the attenuation end distance
			*
			* To access this property do: FarAttenuationEnd.Get().
			* To set this property do: FarAttenuationEnd.Set(fbxDouble1).
			*
			* Default value is 0.0
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble1TypedProperty,FarAttenuationEnd);

			/** This property handles the cast shadow flag
			*
			* To access this property do: CastShadows.Get().
			* To set this property do: CastShadows.Set(fbxBool1).
			*
			* Default value is false
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,CastShadows);

			/** This property handles the shadow color
			*
			* To access this property do: ShadowColor.Get().
			* To set this property do: ShadowColor.Set(fbxDouble3).
			*
			* Default value is (0.0, 0.0, 0.0)
			*/
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ShadowColor);

			//@}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
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