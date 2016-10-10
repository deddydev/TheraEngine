#pragma once
#include "stdafx.h"
#include "FbxSurfaceMaterial.h"


{
	namespace FbxSDK
	{	
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;		
		ref class FbxDouble3;
		/** Material settings.
		* \nosubgrouping
		* A material is attached to an instance of class KFbxGeometry
		* by calling KFbxGeometry::AddMaterial(). Materials can be shared among 
		* many instances of class KFbxGeometry.
		*/
		public ref class FbxSurfaceLambert : FbxSurfaceMaterial
		{
			REF_DECLARE(FbxEmitter,KFbxSurfaceLambert);
		internal:
			FbxSurfaceLambert(KFbxSurfaceLambert* instance) : FbxSurfaceMaterial(instance)
			{
				_Free = false;
			}
			
			FBXOBJECT_DECLARE(FbxSurfaceLambert);		
		public:			
			/**
			* \name Material properties
			*/
			//@{

			/** Get the emissive color property.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,EmissiveColor);

			/** Get the emissive factor property. This factor is used to
			* attenuate the emissive color.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,EmissiveFactor);			

			/** Get the ambient color property.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,AmbientColor);			

			/** Get the ambient factor property. This factor is used to
			* attenuate the ambient color.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,AmbientFactor);			

			/** Get the diffuse color property.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,DiffuseColor);			

			/** Get the diffuse factor property. This factor is used to
			* attenuate the diffuse color.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,DiffuseFactor);			

			/** Get the bump property. This property is used to perturb the
			* surface normal, creating the illusion of a bumpy surface.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,Bump);			

			/** Get the transparent color property. This property is used to make a
			* surface more or less transparent.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,TransparentColor);			

			/** Get the transparency property. This property is used to make a
			* surface more or less opaque (0 = opaque, 1 = transparent).
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,TransparencyFactor);			
//
//			//@}
//
//			//////////////////////////////////////////////////////////////////////////
//			// Static values
//			//////////////////////////////////////////////////////////////////////////
//
//			// Default property values
//			static fbxDouble3 sEmissiveDefault;
//			static fbxDouble1 sEmissiveFactorDefault;
//
//			static fbxDouble3 sAmbientDefault;
//			static fbxDouble1 sAmbientFactorDefault;
//
//			static fbxDouble3 sDiffuseDefault;
//			static fbxDouble1 sDiffuseFactorDefault;
//
//			static fbxDouble3 sBumpDefault;
//			static fbxDouble3 sNormalMapDefault;
//
//			static fbxDouble3 sTransparentDefault;
//			static fbxDouble1 sTransparencyFactorDefault;
//
//			///////////////////////////////////////////////////////////////////////////////
//			//
//			//  WARNING!
//			//
//			//	Anything beyond these lines may not be documented accurately and is 
//			// 	subject to change without notice.
//			//
//			///////////////////////////////////////////////////////////////////////////////
//
#ifndef DOXYGEN_SHOULD_SKIP_THIS

			 //Clon
			CLONE_DECLARE();

			//bool operator==(KFbxSurfaceLambert const& pMaterial) const;

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

	}
}