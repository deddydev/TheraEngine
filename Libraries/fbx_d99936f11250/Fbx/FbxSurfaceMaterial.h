#pragma once
#include "stdafx.h"
#include "FbxShadingObject.h"


{
	namespace FbxSDK
	{	
		ref class FbxClassId;
		ref class FbxSdkManagerManaged;
		ref class FbxStringTypedProperty;

		/** Material settings.
		* \nosubgrouping
		* A material is attached to an instance of class KFbxGeometry
		* by calling KFbxGeometry::AddMaterial(). Materials can be shared among 
		* many instances of class KFbxGeometry.
		*/
		public ref class FbxSurfaceMaterial : FbxShadingObject
		{
			REF_DECLARE(FbxEmitter,KFbxSurfaceMaterial);			
		internal:
			FbxSurfaceMaterial(KFbxSurfaceMaterial* instance) : FbxShadingObject(instance)
			{
				_Free = false;
			}
			
			FBXOBJECT_DECLARE(FbxSurfaceMaterial);		
		public:			
			/**
			* \name Standard Material Property Names
			*/
			//@{	


			static const String^ SShadingModel = "ShadingModel";
			static const String^ SMultiLayer = "MultiLayer";

			static const String^ SEmissive = "EmissiveColor";
			static const String^ SEmissiveFactor = "EmissiveFactor";

			static const String^ SAmbient = "AmbientColor";
			static const String^ SAmbientFactor = "AmbientFactor";

			static const String^ SDiffuse = "DiffuseColor";
			static const String^ SDiffuseFactor = "DiffuseFactor";

			static const String^ SSpecular = "SpecularColor";
			static const String^ SSpecularFactor = "SpecularFactor";
			static const String^ SShininess = "Shininess";

			static const String^ SBump = "Bump";
			static const String^ SNormalMap = "NormalMap";

			static const String^ STransparentColor = "TransparentColor";
			static const String^ STransparencyFactor = "TransparencyFactor";

			static const String^ SReflection = "ReflectionColor";
			static const String^ SReflectionFactor = "ReflectionFactor";
			

			/**
			* \name Material Properties
			*/
			//@{	

			//! Reset the material to default values.
			//void Reset();

			/** Get material shading model.
			* \return The shading model type string.
			*/			
			VALUE_PROPERTY_GETSET_DECLARE(String^,ShadingModel);

			/**	Get multilayer state.
			* \return The state of the multi-layer flag.
			*/
			//KFbxPropertyBool1 GetMultiLayer() const;
			VALUE_PROPERTY_GETSET_DECLARE(bool,MultiLayer);

			//@}	

			//////////////////////////////////////////////////////////////////////////
			// Static values
			//////////////////////////////////////////////////////////////////////////

			// Default property values
			//static fbxBool1		sMultiLayerDefault;
			//static char const*	sShadingModelDefault;

			///////////////////////////////////////////////////////////////////////////////
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

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 

		};

	}
}