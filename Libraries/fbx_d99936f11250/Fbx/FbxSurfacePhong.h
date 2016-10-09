#pragma once
#include "stdafx.h"
#include "FbxSurfaceLambert.h"

namespace Skill
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
		public ref class FbxSurfacePhong : FbxSurfaceLambert
		{
			REF_DECLARE(FbxEmitter,KFbxSurfacePhong);
		internal:
			FbxSurfacePhong(KFbxSurfacePhong* instance) : FbxSurfaceLambert(instance)
			{
				_Free = false;
			}
			FBXOBJECT_DECLARE(FbxSurfacePhong);		
		public:			
			/**
			* \name Material properties
			*/
			//@{

			/** Get the specular color property.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,SpecularColor);			

			/** Get the specular factor property. This factor is used to
			* attenuate the specular color.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,SpecularFactor);			

			/** Get the shininess property. This property controls the aspect
			* of the shiny spot. It is the specular exponent in the Phong
			* illumination model.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,Shininess);			

			/** Get the reflection color property. This property is used to
			* implement reflection mapping.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(FbxDouble3^,ReflectionColor);			

			/** Get the reflection factor property. This property is used to
			* attenuate the reflection color.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(double,ReflectionFactor);			
//
//			//@}
//
//			//////////////////////////////////////////////////////////////////////////
//			// Static values
//			//////////////////////////////////////////////////////////////////////////
//
//			// Default property values
//			static fbxDouble3 sSpecularDefault;
//			static fbxDouble1 sSpecularFactorDefault;
//
//			static fbxDouble1 sShininessDefault;
//
//			static fbxDouble3 sReflectionDefault;
//			static fbxDouble1 sReflectionFactorDefault;
//
//			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			// Clone
			CLONE_DECLARE();

			//bool operator==(KFbxSurfacePhong const& pMaterial) const;

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

	}
}