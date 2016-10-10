#pragma once
#include "stdafx.h"
#include "FbxShadingNode.h"



{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxDouble3TypedProperty;
		ref class FbxStringManaged;
		ref class FbxVector4;

		/** A texture is the description of the mapping of an image over a geometry.
		* \nosubgrouping
		*/
		public ref class FbxTexture : FbxShadingNode
		{
			REF_DECLARE(FbxEmitter,KFbxTexture);
		internal:
			FbxTexture(KFbxTexture* instance)  : FbxShadingNode(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxTexture);

		protected:
			virtual void CollectManagedMemory() override;			
		public:			
			/**
			* \name Texture Properties
			*/
			//@{
			enum class UnifiedMappingType
			{ 
				UV = KFbxTexture::eUMT_UV, 
				XY = KFbxTexture::eUMT_XY, 
				YZ = KFbxTexture::eUMT_YZ, 
				XZ = KFbxTexture::eUMT_XZ, 
				Spherical = KFbxTexture::eUMT_SPHERICAL,
				Cylindrical = KFbxTexture::eUMT_CYLINDRICAL,
				Environment = KFbxTexture::eUMT_ENVIRONMENT,
				Projection = KFbxTexture::eUMT_PROJECTION,
				Box = KFbxTexture::eUMT_BOX, // deprecated
				Face = KFbxTexture::eUMT_FACE, // deprecated
				NoMapping = KFbxTexture::eUMT_NO_MAPPING,
			};

			enum class TextureUse6
			{
				Standard = KFbxTexture::eTEXTURE_USE_6_STANDARD,
				SphericalReflexionMap = KFbxTexture::eTEXTURE_USE_6_SPHERICAL_REFLEXION_MAP,
				SphereReflexionMap = KFbxTexture::eTEXTURE_USE_6_SPHERE_REFLEXION_MAP,
				ShadowMap = KFbxTexture::eTEXTURE_USE_6_SHADOW_MAP,
				LightMap = KFbxTexture::eTEXTURE_USE_6_LIGHT_MAP,
				BumpNormalMap = KFbxTexture::eTEXTURE_USE_6_BUMP_NORMAL_MAP
			};

			/** \enum EWrapMode Wrap modes.
			* - \e eREPEAT
			* - \e eCLAMP
			*/
			enum class WrapMode
			{
				Repeat = KFbxTexture::eREPEAT,
				Clamp = KFbxTexture::eCLAMP
			};

			/** \enum EBlendMode Blend modes.
			* - \e eTRANSLUCENT
			* - \e eADDITIVE
			* - \e eMODULATE
			* - \e eMODULATE2
			*/
			enum class BlendMode
			{
				Translucent = KFbxTexture::eTRANSLUCENT,
				Additive = KFbxTexture::eADDITIVE,
				Modulate = KFbxTexture::eMODULATE,
				Modulate2 = KFbxTexture::eMODULATE2
			};

			/** \enum EAlignMode Alignment modes.
			* - \e KFBXTEXTURE_LEFT
			* - \e KFBXTEXTURE_RIGHT
			* - \e KFBXTEXTURE_TOP
			* - \e KFBXTEXTURE_BOTTOM
			*/
			enum class AlignMode
			{
				Left = KFbxTexture::eLEFT,
				Right = KFbxTexture::eRIGHT,
				Top = KFbxTexture::eTOP,
				Bottom = KFbxTexture::eBOTTOM
			};

			/** \enum ECoordinates Texture coordinates.
			* - \e KFBXTEXTURE_U
			* - \e KFBXTEXTURE_V
			* - \e KFBXTEXTURE_W
			*/
			enum class Coordinates
			{
				U = KFbxTexture::eU,
				V = KFbxTexture::eV,
				W = KFbxTexture::eW
			};

			// Type description
			VALUE_PROPERTY_GETSET_DECLARE(TextureUse6,TextureTypeUse);
			VALUE_PROPERTY_GETSET_DECLARE(double,Alpha);			

			// Mapping information
			VALUE_PROPERTY_GETSET_DECLARE(UnifiedMappingType,CurrentMappingType);
			VALUE_PROPERTY_GETSET_DECLARE(WrapMode,WrapModeU);
			VALUE_PROPERTY_GETSET_DECLARE(WrapMode,WrapModeV);
			VALUE_PROPERTY_GETSET_DECLARE(bool,UVSwap);			

			// Texture positioning
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Translation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Rotation);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,Scaling);			
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,RotationPivot);
			REF_PROPERTY_GET_DECLARE(FbxDouble3TypedProperty,ScalingPivot);			

			// Material management
			VALUE_PROPERTY_GETSET_DECLARE(bool,UseMaterial);			

			// Blend mode
			VALUE_PROPERTY_GETSET_DECLARE(BlendMode,CurrentTextureBlendMode);			

			// UV set to use.
			VALUE_PROPERTY_GETSET_DECLARE(FbxStringManaged^,UVSet);			

			/** Reset the texture to its default values.
			* \remarks Texture file name is not reset.
			*/
			void Reset();

			/** Set the associated texture file. 
			* \param pName The absolute path of the texture file.   
			* \return Return \c true on success.
			*	\remarks The texture file name must be valid.
			*/
			bool SetFileName(String^ name);

			/** Set the associated texture file. 
			* \param pName The relative path of the texture file.   
			* \return Return \c true on success.
			*	\remarks The texture file name must be valid.
			*/
			bool SetRelativeFileName(String^ name);

#ifdef KARCH_DEV_MACOSX_CFM
			bool SetFile(const FSSpec &pMacFileSpec);
			bool SetFile(const FSRef &pMacFileRef);
			bool SetFile(const CFURLRef &pMacURL);
#endif

			/** Get the associated texture file path.
			* \return The associated texture file path.
			* \return An empty string if KFbxTexture::SetFileName() has not been called before.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,FileName);

			/** Get the associated texture file path.
			* \return The associated texture file path.
			* \return An empty string if KFbxTexture::SetRelativeFileName() has not been called before.
			*/
			VALUE_PROPERTY_GET_DECLARE(String^,RelativeFileName);

#ifdef KARCH_DEV_MACOSX_CFM
			bool GetFile(FSSpec &pMacFileSpec) const;
			bool GetFile(FSRef &pMacFileRef) const;
			bool GetFile(CFURLRef &pMacURL) const;
#endif

			/** Get the swap UV flag.
			* \return \c true if swap UV flag is enabled.
			* \remarks If swap UV flag is enabled, the texture's width and height are swapped.
			*/
			/** Set the swap UV flag.
			* \param pSwapUV Set to \c true if swap UV flag is enabled.
			* \remarks If swap UV flag is enabled, the texture's width and height are swapped.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(bool,SwapUV);

			/** \enum EAlphaSource Alpha sources.
			* - \e eNONE
			* - \e eRGB_INTENSITY
			* - \e eBLACK
			*/
			enum class AlphaSource
			{ 
				None = KFbxTexture::eNONE, 
				RgbIntensity = KFbxTexture::eRGB_INTENSITY, 
				Black = KFbxTexture::eBLACK 
			};				

			/** Get alpha source.
			* \return Alpha source identifier for this texture.
			*/
			/** Set alpha source.
			* \param pAlphaSource Alpha source identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::AlphaSource,AlphaSrc);

			/** Set cropping.
			* \param pLeft Left cropping value.
			* \param pTop  Top cropping value.
			* \param pRight Right cropping value.
			* \param pBottom Bottom cropping value.
			* \remarks The defined rectangle is not checked for invalid values.
			* It is the responsability of the caller to validate that the rectangle
			* is meaningful for this texture.
			*/
			void SetCropping(int left, int top, int right, int bottom);

			/** Get left cropping.
			* \return Left side of the cropping rectangle.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CroppingLeft);

			/** Get top cropping.
			* \return Top side of the cropping rectangle.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CroppingTop);			

			/** Get right cropping.
			* \return Right side of the cropping rectangle.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CroppingRight);			

			/** Get bottom cropping.
			* \return Bottom side of the cropping rectangle.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,CroppingBottom);			

			/** \enum EMappingType Texture mapping types.
			* - \e eNULL
			* - \e ePLANAR
			* - \e eSPHERICAL
			* - \e eCYLINDRICAL
			* - \e eBOX
			* - \e eFACE
			* - \e eUV
			* - \e eENVIRONMENT
			*/
			enum class MappingType
			{ 
				Null = KFbxTexture::eNULL, 
				Planar = KFbxTexture::ePLANAR, 
				Spherical = KFbxTexture::eSPHERICAL, 
				Cylindrical = KFbxTexture::eCYLINDRICAL, 
				Box = KFbxTexture::eBOX, 
				Face = KFbxTexture::eFACE,
				Uv = KFbxTexture::eUV,
				Environment = KFbxTexture::eENVIRONMENT
			};				

			/** Get mapping type.
			* \return Mapping type identifier.
			*/
			/** Set mapping type.
			* \param pMappingType Mapping type identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::MappingType,Mapping);				

			/** \enum EPlanarMappingNormal Planar mapping normal orientations.
			* - \e ePLANAR_NORMAL_X
			* - \e ePLANAR_NORMAL_Y
			* - \e ePLANAR_NORMAL_Z
			*/
			enum class PlanarMappingNormal
			{ 
				X = KFbxTexture::ePLANAR_NORMAL_X, 
				Y = KFbxTexture::ePLANAR_NORMAL_Y, 
				Z = KFbxTexture::ePLANAR_NORMAL_Z 
			};					

			/** Get planar mapping normal orientations.
			* \return Planar mapping normal orientation identifier.
			*/
			/** Set planar mapping normal orientations.
			* \param pPlanarMappingNormal Planar mapping normal orientation identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::PlanarMappingNormal,PlanarMappingNormalType);			

			/** \enum EMaterialUse Material usages.
			* - \e eMODEL_MATERIAL
			* - \e eDEFAULT_MATERIAL
			*/
			enum class MaterialUse
			{
				Model = KFbxTexture::eMODEL_MATERIAL,
				Default = KFbxTexture::eDEFAULT_MATERIAL
			};						

			/** Get material usage.
			* \return Material usage identifier.
			*/
			/** Set material usage.
			* \param pMaterialUse Material usage identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::MaterialUse,MaterialUseType);

			/** \enum ETextureUse Texture usages.
			* - \e eSTANDARD
			* - \e eSHADOW_MAP
			* - \e eLIGHT_MAP
			* - \e eSPHERICAL_REFLEXION_MAP
			* - \e eSPHERE_REFLEXION_MAP
			* - \e eBUMP_NORMAL_MAP
			*/
			enum class TextureUse
			{
				Standard = KFbxTexture::eSTANDARD,
				ShadowMap = KFbxTexture::eSHADOW_MAP,
				LightMap = KFbxTexture::eLIGHT_MAP,
				SphericalReflexionMap = KFbxTexture::eSPHERICAL_REFLEXION_MAP,
				SphereReflexionMap = KFbxTexture::eSPHERE_REFLEXION_MAP,
				BumpNormalMap = KFbxTexture::eBUMP_NORMAL_MAP
			};			

			/** Get texture usage.
			* \return Texture usage identifier.
			*/
			/** Set texture usage.
			* \param pTextureUse Texure usage identifier.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::TextureUse,TextureUseType);			


			/** Set wrap mode in U and V.
			* \param pWrapU Wrap mode identifier.
			* \param pWrapV Wrap mode identifier.
			*/
			void SetWrapMode(FbxTexture::WrapMode wrapU, FbxTexture::WrapMode wrapV);

			/** Get wrap mode in U.
			* \return U wrap mode identifier.
			*/
			//VALUE_PROPERTY_GET_DECLARE(FbxTexture::WrapMode,WrapModeU);

			/** Get wrap mode in V.
			* \return V wrap mode identifier.
			*/
			//VALUE_PROPERTY_GET_DECLARE(FbxTexture::WrapMode,WrapModeV);			

			/** Get blend mode.
			* \return Blend mode identifier.
			*/
			/** Set blend mode.
			* \param pBlendMode Blend mode identifier.
			*/
			//VALUE_PROPERTY_GETSET_DECLARE(FbxTexture::BlendMode,BlendMode);

			//@}

			/**
			* \name Default Animation Values
			* This set of functions provide direct access to default
			* animation values in the default take node. 
			*/
			//@{

			/** Set default translation vector. 
			* \param pT First element is the U translation applied to 
			* texture. A displacement of one unit is equal to the texture
			* width after the scaling in U is applied. Second element is the
			* V translation applied to texture. A displacement of one unit is 
			* equal to the texture height after the scaling in V is applied.
			* Third and fourth elements do not have an effect on texture 
			* translation.
			*/			

			/** Get default translation vector. 
			* \param pT First element is the U translation applied to 
			* texture. A displacement of one unit is equal to the texture 
			* width after the scaling in U is applied. Second element is the
			* V translation applied to texture. A displacement of one unit is 
			* equal to the texture height after the scaling in V is applied.
			* Third and fourth elements do not have an effect on the texture. 
			* translation.
			* \return Input parameter filled with appropriate data.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,DefaultT);

			/** Set default rotation vector. 
			* \param pR First element is the texture rotation around the 
			* U axis in degrees. Second element is the texture rotation 
			* around the V axis in degrees. Third element is the texture 
			* rotation around the W axis in degrees.
			* \remarks The W axis is oriented towards the result of the 
			* vector product of the U axis and V axis i.e. W = U x V.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,DefaultR);			

			/** Get default rotation vector. 
			* \param pR First element is the texture rotation around the 
			* U axis in degrees. Second element is the texture rotation 
			* around the V axis in degrees. Third element is the texture 
			* rotation around the W axis in degrees.
			* \return Input parameter filled with appropriate data.
			* \remarks The W axis is oriented towards the result of the 
			* vector product of the U axis and V axis i.e. W = U x V.
			*/			

			/** Set default scale vector. 
			* \param pS First element is scale applied to texture width. 
			* Second element is scale applied to texture height. Third 
			* and fourth elements do not have an effect on the texture. 
			* \remarks A scale value inferior to 1 means the texture is stretched.
			* A scale value superior to 1 means the texture is compressed.
			*/
			REF_PROPERTY_GETSET_DECLARE(FbxVector4,DefaultS);			

			/** Get default scale vector. 
			* \param pS First element is scale applied to texture width. 
			* Second element is scale applied to texture height. Third 
			* and fourth elements do not have an effect on the texture. 
			* \return Input parameter filled with appropriate data.
			* \remarks A scale value inferior to 1 means the texture is stretched.
			* A scale value superior to 1 means the texture is compressed.
			*/			

			/** Get default alpha.
			*	\return A value on a scale from 0 to 1, 0 meaning transparent.
			*/
			/** Set default alpha.
			*	\param pAlpha A value on a scale from 0 to 1, 0 meaning transparent.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DefaultAlpha);					

			//@}

			/**
			* \name Obsolete Functions
			* This set of functions is obsolete since animated parameters
			* are now supported. U, V and W coordinates are mapped to X, Y and Z
			* coordinates of the default vectors found in section "Default Animation 
			* Values".
			*/
			//@{

			/** Set translation.
			* \param pU Horizontal translation applied to texture. A displacement 
			* of one unit is equal to the texture's width after the scaling in 
			* U is applied.
			* \param pV Vertical translation applied to texture. A displacement 
			* of one unit is equal to the texture's height after the scaling in 
			* V is applied.
			*/
			void SetTranslation(double u,double v);

			/** Get translation applied to texture width.
			* \remarks A displacement of one unit is equal to the texture's width 
			* after the scaling in U is applied.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,TranslationU);

			/** Get translation applied to texture height.
			* \remarks A displacement of one unit is equal to the texture's height 
			* after the scaling in V is applied.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,TranslationV);			

			/** Set rotation.
			* \param pU Texture rotation around the U axis in degrees.
			* \param pV Texture rotation around the V axis in degrees.
			* \param pW Texture rotation around the W axis in degrees.
			* \remarks The W axis is oriented towards the result of the vector product of 
			* the U axis and V axis i.e. W = U x V.
			*/
			void SetRotation(double u, double v, double w);
			void SetRotation(double u, double v)
			{
				SetRotation(u,v,0);
			}

			//! Get texture rotation around the U axis in degrees.
			VALUE_PROPERTY_GET_DECLARE(double,RotationU);			

			//! Get texture rotation around the V axis in degrees.
			VALUE_PROPERTY_GET_DECLARE(double,RotationV);			

			//! Get texture rotation around the W axis in degrees.
			VALUE_PROPERTY_GET_DECLARE(double,RotationW);			

			/** Set scale.
			* \param pU Scale applied to texture width. 
			* \param pV Scale applied to texture height. 
			* \remarks A scale value inferior to 1 means the texture is stretched.
			* A scale value superior to 1 means the texture is compressed.
			*/
			void SetScale(double u,double v);

			/** Get scale applied to texture width. 
			* \remarks A scale value inferior to 1 means the texture is stretched.
			* A scale value superior to 1 means the texture is compressed.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,ScaleU);			

			/** Get scale applied to texture height. 
			* \remarks A scale value inferior to 1 means the texture is stretched.
			* A scale value superior to 1 means the texture is compressed.
			*/
			VALUE_PROPERTY_GET_DECLARE(double,ScaleV);			

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS			

			/*FbxString& GetMediaName();
			void SetMediaName(char const* pMediaName);

			void SetUVTranslation(KFbxVector2& pT);
			KFbxVector2& GetUVTranslation();
			void SetUVScaling(KFbxVector2& pS);
			KFbxVector2& GetUVScaling();

			FbxString GetTextureType();*/


			// Clone
			CLONE_DECLARE();
		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}