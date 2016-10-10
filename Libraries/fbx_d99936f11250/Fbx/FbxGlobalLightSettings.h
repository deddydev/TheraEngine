#pragma once
#include "stdafx.h"
#include "Fbx.h"



{
	namespace FbxSDK
	{
		ref class FbxColor;
		ref class FbxVector4;
		ref class FbxErrorManaged;
		/** This class contains functions for accessing global light settings.
		* \nosubgrouping
		*/
		public ref class FbxGlobalLightSettings : IFbxNativePointer
		{	
		public:
			/**
			* \name Shadow Planes
			* The functions in this section are supported by FiLMBOX 2.7 and previous versions only.
			* FiLMBOX 3.0 supports shadow planes within a specific shader, which is not supported by the FBX SDK.
			*/
			//@{

			ref struct FbxShadowPlane : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxShadowPlane,KFbxGlobalLightSettings::KFbxShadowPlane);
				INATIVEPOINTER_DECLARE(FbxShadowPlane,KFbxGlobalLightSettings::KFbxShadowPlane);			
			public:
				DEFAULT_CONSTRUCTOR(FbxShadowPlane,KFbxGlobalLightSettings::KFbxShadowPlane);

				VALUE_PROPERTY_GETSET_DECLARE(bool,Enable);
				REF_PROPERTY_GETSET_DECLARE(FbxVector4,Origin);								
				REF_PROPERTY_GETSET_DECLARE(FbxVector4,Normal);
			};


		private:
			bool _disposed;
		internal:
			bool _Free;
			KFbxGlobalLightSettings* _FbxGlobalLightSettings;
			FbxGlobalLightSettings(KFbxGlobalLightSettings* instance);
			void _SetPointer(KFbxGlobalLightSettings* instance , bool free)
			{
				_FbxGlobalLightSettings = instance;
				_Free = free;
			}
		public:
			property bool Disposed {bool get(){return _disposed;}}
		protected:
			virtual void CollectManagedMemory();


			REF_DECLARE(FbxGlobalLightSettings,KFbxGlobalLightSettings);
			DESTRUCTOR_DECLARE_2(FbxGlobalLightSettings);
			INATIVEPOINTER_DECLARE(FbxGlobalLightSettings,KFbxGlobalLightSettings);		
		protected:
			System::Collections::Generic::List<FbxGlobalLightSettings::FbxShadowPlane^>^ _list;
		public:

			/**
			* \name Ambient Color
			*/
			//@{

			/** Get ambient color.
			* \return     The ambient color.
			*/
			/** Set ambient color.
			* \param pAmbientColor     The ambient color to set.
			* \remarks                 Only the RGB channels are used.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxColor^,AmbientColor);



			/**
			* \name Fog Option
			*/
			//@{

			/** Get the current state of the fog option.
			* \return     \c true if fog is enabled, \c false otherwise.
			*/			
			/** Enable or disable the fog.
			* \param pEnable     Set to \c true to enable the fog option. \c false disables the fog option.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,FogEnable);

			/** Get the fog color.
			* \return      The fog color.
			* \remarks     Only the RGB channels are used.
			*/
			/** Set the fog color.
			* \param pColor     The fog color.
			* \remarks          Only the RGB channels are used.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxColor^,FogColor);


			/** \enum EFogMode Fog types.
			* - \e eLINEAR
			- \e eEXPONENTIAL
			- \e eSQUAREROOT_EXPONENTIAL
			*/
			enum class LightFogMode
			{
				Linear,
				Exponential,
				SquarerootExponential
			};

			/** Get the fog mode.
			* \return     The currently set fog mode.
			*/			
			/** Set the fog mode.
			* \param pMode     The fog type.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(LightFogMode,FogMode);						

			/** Get the fog density.
			* \return      The currently set fog density.
			* \remarks     Only use this function when the fog mode is exponential or squareroot exponential.
			*/			
			/** Set the fog density.
			* \param pDensity     The density of the fog. Can be any double value, however it is
			*                     possible that other sections of FBX SDK may clamp values to reasonable values.
			* \remarks            Only use this function when the fog mode is exponential or squareroot exponential.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,FogDensity);

			/** Get the distance from the view where the fog starts.
			* \return      The distance from the view where the fog starts.
			* \remarks     Only use this function when the fog mode is linear.
			*/			
			/** Set the distance from the view where the fog starts.
			* \param pStart     Distance where the fog starts.
			* \remarks          Only use this function when the fog mode is linear. The new value is clamped to fit inside the interval [0, FogEnd()].
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double ,FogStart);



			/** Get the distance from the view where the fog ends.
			* \return      The distance from the view where the fog ends.
			* \remarks     Only use this function when the fog mode is linear.
			*/
			/** Set the distance from the view where the fog ends.
			* \param pEnd     Distance where the fog ends.
			* \remarks        Only use this function when the fog mode is linear. The new value is adjusted to fit within the interval [FogStart(), inf).
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,FogEnd);			
			
			/** Get the current state of the ShadowEnable flag.
			* \return     \c true if shadow planes are set to be displayed in the scene.
			*/
			/** Enable or disable the shadow planes display.
			* \param pShadowEnable     Set to \c true to display shadow planes in the scene.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool, ShadowEnable);

			/** Get the shadow intensity applied to all shadow planes.
			* \return      The intensity applied to all shadow planes in the scene.
			* \remarks     Range is from 0 to 300.
			*/			
			/** Set the shadow intensity applied to all shadow planes.
			* \param pShadowIntensity     Intensity applied to all the shadow planes.
			* \remarks                    Range is from 0 to 300.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,ShadowIntensity);

			/** Get the number of shadow planes.
			* \return     Number of shadow planes.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ShadowPlaneCount);

			/** Get a shadow plane.
			* \param pIndex     Index of shadow plane.
			* \return           Pointer the shadow plane, or \c NULL if the index is out of range.
			* \remarks          To identify the error, call KFbxGlobalLightSettings::GetLastErrorID() which returns eINDEX_OUT_OF_RANGE.
			*/
			FbxShadowPlane^ GetShadowPlane(int index);

			/** Add a shadow plane.
			* \param pShadowPlane     The shadow plane to add.
			*/
			void AddShadowPlane(FbxShadowPlane^ shadowPlane);

			//! Remove all shadow planes.
			void RemoveAllShadowPlanes();

			/**
			* \name Error Management
			*/
			//@{

			/** Retrieve error object.
			*  \return     Reference to error object.
			*/
			REF_PROPERTY_GET_DECLARE(FbxErrorManaged,KError);

			/** \enum EError Error identification.
			*  - \e eINDEX_OUT_OF_RANGE
			*  - \e eERROR_COUNT
			*/
			enum class Error
			{
				IndexOutOfRange,
				ErrorCount
			} EError;

			/** Get last error code.
			*  \return     Last error code.
			*/
			property Error LastErrorID
			{
				Error get();
			}

			/** Get last error string.
			*  \return     Textual description of the last error.
			*/
			property String^ LastErrorString
			{
				String^ get();
			}			

			//! Restore default settings.
			void RestoreDefaultSettings();

			//! Assignment operator.
			void CopyFrom(FbxGlobalLightSettings^ settings);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//  Anything beyond these lines may not be documented accurately and is
			//  subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS						

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}