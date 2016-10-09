#pragma once
#include "stdafx.h"
#include "FbxLayerElement.h"

#define CREATE_DECLARE(classDesc)\
	static Fbx##classDesc^ Create(FbxLayerContainer^ owner, String^ name);

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxSdkManagerManaged;
		ref class FbxStream;
		ref class FbxLayerElementNormal;
		ref class FbxLayerElementMaterial;
		ref  class FbxLayerElementPolygonGroup;
		ref class FbxLayerElementUV;
		ref class FbxLayerElementVertexColor;
		ref class FbxLayerElementSmoothing;
		ref class FbxLayerElementUserData;
		ref class FbxLayerElementVisibility;
		ref class FbxLayerElementTexture;

		//		/** \brief KFbxLayer class provides the base for the layering mechanism.
		//		* 
		//		* A layer can contain one or more of the following layer elements:
		//		*      \li Normals
		//		*      \li UVs
		//		*      \li Textures (diffuse, ambient, specular, etc.)
		//		*      \li Materials
		//		*      \li Polygon Groups
		//		*      \li Vertex Colors and
		//		*      \li Smoothing information
		//		*      \li Custom User Data
		//		* 
		//		* A typical layer for a Mesh will contain Normals, UVs and Materials. A typical layer for Nurbs will contain only Materials. If a texture
		//		* is applied to a Mesh, a typical layer will contain the Textures along with the UVs. In the case of the Nurbs, the Nurbs' parameterization is
		//		* used for the UVs; there should be no UVs specified.  The mapping of a texture is completely defined within the layer; there is no 
		//		* cross-layer management.
		//		* 
		//		* In most cases, a single layer is sufficient to describe a geometry. Many applications will only support what is defined on the first layer. 
		//		* This should be taken into account when filling the layer. For example, it is totaly legal to define the Layer 0 with the Textures and UVs and
		//		* define the model's Normals on layer 1. However a file constructed this way may not be imported correctly in other applications. The user
		//		* should put the Normals in Layer 0.
		//		* 
		//		* Texture layering is achieved by defining more than one layer containing Textures and UVs elements. For example, a Mesh may have Textures and
		//		* the corresponding UVs elements on Layer 0 for the primary effect, and another set of Textures and UVs on Layer 1. The way the texture blending
		//		* is done is defined in the Texture layer element.
		//		* \nosubgrouping
		//		*/
		public ref class FbxLayer : IFbxNativePointer
		{
			INTERNAL_CLASS_DECLARE(FbxLayer,KFbxLayer);
			REF_DECLARE(FbxLayer,KFbxLayer);
			DESTRUCTOR_DECLARE_2(FbxLayer);
			INATIVEPOINTER_DECLARE(FbxLayer,KFbxLayer);
		public:				
			/**
			* \name Layer Element Management
			*/
			//@{

			/** Get the Normals description for this layer.
			* \return      Pointer to the Normals layer element, or \c NULL if no Normals are defined for this layer.
			* \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Normals defined.
			*/
			/** Set the Normals description for this layer.
				  * \param pNormals     Pointer to the Normals layer element, or \c NULL to remove the Normals definition.
				  * \remarks            A geometry of type KFbxNurb or KFbxPatch should not have Normals defined.
				  */
			REF_PROPERTY_GETSET_DECLARE(FbxLayerElementNormal,Normals);	

			/** Get the Normals description for this layer.
			* \return      Pointer to the Normals layer element, or \c NULL if no Normals are defined for this layer.
			* \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Normals defined.
			*/
			//KFbxLayerElementNormal const* GetNormals() const;	
			
				/** Get the Materials description for this layer.
				  * \return     Pointer to the Materials layer element, or \c NULL if no Materials are defined for this layer.
				  */
			/** Set the Materials description for this layer.
				  * \param pMaterials     Pointer to the Materials layer element, or \c NULL to remove the Material definition.
				  */
			REF_PROPERTY_GETSET_DECLARE(FbxLayerElementMaterial,Materials);				
			
				/** Get the Materials description for this layer.
				  * \return     Pointer to the Materials layer element, or \c NULL if no Materials are defined for this layer.
				  */
				//KFbxLayerElementMaterial const* GetMaterials() const;
			
				/** Get the Polygon Groups description for this layer.
				  * \return     Pointer to the Polygon Groups layer element, or \c NULL if no Polygon Groups are defined for this layer.
				  */
			/** Set the Polygon Groups description for this layer.
				  * \param pPolygonGroups     Pointer to the Polygon Groups layer element, or \c NULL to remove the Polygon Group definition.
				  */
			REF_PROPERTY_GETSET_DECLARE(FbxLayerElementPolygonGroup,PolygonGroups);			
			
				/** Get the Polygon Groups description for this layer.
				  * \return     Pointer to the Polygon Groups layer element, or \c NULL if no Polygon Groups are defined for this layer.
				  */
				//KFbxLayerElementPolygonGroup const* GetPolygonGroups() const;
			
			
				   /** Get the EmissiveUV description for this layer.
				  * \return     Pointer to the EmissiveUV layer element, or \c NULL if no EmissiveUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,EmissiveUV);				
			
				/** Get the EmissiveUV description for this layer.
				  * \return     Pointer to the EmissiveUV layer element, or \c NULL if no EmissiveUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetEmissiveUV() const;
			
			    /** Get the EmissiveFactorUV description for this layer.
				  * \return     Pointer to the EmissiveFactorUV layer element, or \c NULL if no EmissiveFactorUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,EmissiveFactorUV);			
			
				/** Get the EmissiveFactorUV description for this layer.
				  * \return     Pointer to the EmissiveFactorUV layer element, or \c NULL if no EmissiveFactorUV are defined for this layer.
				  */
			//KFbxLayerElementUV const* GetEmissiveFactorUV() const;
			
			    /** Get the EmissiveFactorUV description for this layer.
				  * \return     Pointer to the AmbientUV layer element, or \c NULL if no AmbientUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,AmbientUV);				
			
				/** Get the AmbientUV description for this layer.
				  * \return     Pointer to the AmbientUV layer element, or \c NULL if no AmbientUV are defined for this layer.
				  */
			//KFbxLayerElementUV const* GetAmbientUV() const;
			
			    /** Get the AmbientUV description for this layer.
				  * \return     Pointer to the AmbientFactorUV layer element, or \c NULL if no AmbientFactorUV are  defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,AmbientFactorUV);			
			
				/** Get the AmbientFactorUV description for this layer.
				  * \return     Pointer to the AmbientFactorUV layer element, or \c NULL if no AmbientFactorUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetAmbientFactorUV() const;
			    
			    /** Get the AmbientFactorUV description for this layer.
			     * \return     Pointer to the DiffuseUV layer element, or \c NULL if no DiffuseUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,DiffuseUV);				
			
				/** Get the DiffuseUV description for this layer.
				  * \return     Pointer to the DiffuseUV layer element, or \c NULL if no DiffuseUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetDiffuseUV() const;
			
			    /** Get the DiffuseUV description for this layer.
				  * \return     Pointer to the DiffuseFactorUV layer element, or \c NULL if no DiffuseFactorUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,DiffuseFactorUV);				
			
				/** Get the DiffuseFactorUV description for this layer.
				  * \return     Pointer to the DiffuseFactorUV layer element, or \c NULL if no DiffuseFactorUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetDiffuseFactorUV() const;
			
			    /** Get the DiffuseFactorUV description for this layer.
				  * \return     Pointer to the SpecularUV layer element, or \c NULL if no SpecularUV are defined for this layer.
				  */
			REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,SpecularUV);			
			
				/** Get the SpecularUV description for this layer.
				  * \return     Pointer to the SpecularUV layer element, or \c NULL if no SpecularUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetSpecularUV() const;
			
			    /** Get the SpecularUV description for this layer.
				  * \return     Pointer to the SpecularFactorUV layer element, or \c NULL if no SpecularFactorUV are defined for this layer.
				  */				
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,SpecularFactorUV);
			
				/** Get the SpecularFactorUV description for this layer.
				  * \return     Pointer to the SpecularFactorUV layer element, or \c NULL if no SpecularFactorUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetSpecularFactorUV() const;
			
			    /** Get the SpecularFactorUV description for this layer.
				  * \return     Pointer to the ShininessUV layer element, or \c NULL if no ShininessUV are defined for this layer.
				  */				
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,ShininessUV);
			
				/** Get the ShininessUV description for this layer.
				  * \return     Pointer to the ShininessUV layer element, or \c NULL if no ShininessUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetShininessUV() const;
			
				/** Get the NormalMapUV description for this layer.
				  * \return     Pointer to the BumpUV layer element, or \c NULL if no BumpUV are defined for this layer.
				  */
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,NormalMapUV);
				//KFbxLayerElementUV* GetNormalMapUV();
			
				/** Get the NormalMapUV description for this layer.
				  * \return     Pointer to the BumpUV layer element, or \c NULL if no BumpUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetNormalMapUV() const;
			
			    /** Get the BumpUV description for this layer.
				  * \return     Pointer to the BumpUV layer element, or \c NULL if no BumpUV are defined for this layer.
				  */
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,BumpUV);				
			
				/** Get the BumpUV description for this layer.
				  * \return     Pointer to the BumpUV layer element, or \c NULL if no BumpUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetBumpUV() const;
			
			    /** Get the TransparentUV description for this layer.
				  * \return     Pointer to the TransparentUV layer element, or \c NULL if no TransparentUV are defined for this layer.
				  */				
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,TransparentUV);
			
				/** Get the TransparentUV description for this layer.
				  * \return     Pointer to the TransparentUV layer element, or \c NULL if no TransparentUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetTransparentUV() const;
			
			    /** Get the TransparencyFactorUV description for this layer.
				  * \return     Pointer to the TransparencyFactorUV layer element, or \c NULL if no TransparencyFactorUV are defined for this layer.
				  */
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,TransparencyFactorUV);				
			
				/** Get the TransparencyFactorUV description for this layer.
				  * \return     Pointer to the TransparencyFactorUV layer element, or \c NULL if no TransparencyFactorUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetTransparencyFactorUV() const;
			
			    /** Get the ReflectionUV description for this layer.
				  * \return     Pointer to the ReflectionUV layer element, or \c NULL if no ReflectionUV are defined for this layer.
				  */
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,ReflectionUV);				
			
				/** Get the ReflectionUV description for this layer.
				  * \return     Pointer to the ReflectionUV layer element, or \c NULL if no ReflectionUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetReflectionUV() const;
			
			    /** Get the ReflectionFactorUV description for this layer.
				  * \return     Pointer to the ReflectionFactorUV layer element, or \c NULL if no ReflectionFactorUV are defined for this layer.
				  */
				REF_PROPERTY_GET_DECLARE(FbxLayerElementUV,ReflectionFactorUV);				
			
				/** Get the ReflectionFactorUV description for this layer.
				  * \return     Pointer to the ReflectionFactorUV layer element, or \c NULL if no ReflectionFactorUV are defined for this layer.
				  */
				//KFbxLayerElementUV const* GetReflectionFactorUV() const;
			
				/** Get the UVs description for this layer.
				  * \return      Pointer to the UVs layer element, or \c NULL if no UV are defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have UVs defined. 
				  *              The Nurbs/Patch parameterization is used as UV parameters to map a texture.
				  */
				FbxLayerElementUV^ GetUVs(FbxLayerElement::LayerElementType typeIdentifier);
				FbxLayerElementUV^ GetUVs()
				{
					return GetUVs(FbxLayerElement::LayerElementType::DiffuseTextures);
				}

			
				/** Get the UVs description for this layer.
				  * \return      Pointer to the UVs layer element, or \c NULL if no UV are defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have UVs defined.
				  *              The Nurbs/Patch parameterization is used as UV parameters to map a texture.
				  */
				//KFbxLayerElementUV const* GetUVs(KFbxLayerElement::ELayerElementType pTypeIdentifier=KFbxLayerElement::eDIFFUSE_TEXTURES) const;
			
			
				/** Get the number of different UV set for this layer.
				  */
				property int UVSetCount
				{
					int get();
				}
				
				/** Get an array describing which UV sets are on this layer.
				  */
				//KArrayTemplate<KFbxLayerElement::ELayerElementType> GetUVSetChannels() const;
			
				/** Get an array of UV sets for this layer.
				  */
				//KArrayTemplate<KFbxLayerElementUV const*> GetUVSets() const;
			
				/** Get the Vertex Colors description for this layer.
				  * \return      Pointer to the Vertex Colors layer element, or \c NULL if no Vertex Color are defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Vertex Colors defined, since no vertex exists.
				  */
				/** Set the Vertex Colors description for this layer.
				  * \param pVertexColors     Pointer to the Vertex Colors layer element, or \c NULL to remove the Vertex Color definition.
				  * \remarks                 A geometry of type KFbxNurb or KFbxPatch should not have Vertex Colors defined, since no vertex exists.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementVertexColor,VertexColors);				
			
				/** Get the Vertex Colors description for this layer.
				  * \return      Pointer to the Vertex Colors layer element, or \c NULL if no Vertex Color are defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Vertex Colors defined, since no vertex exists.
				  */
				//KFbxLayerElementVertexColor const* GetVertexColors() const;
			
				/** Get the Smoothing description for this layer.
				  * \return      Pointer to the Smoothing layer element, or \c NULL if no Smoothing is defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Smoothing defined.
				  */		
				/** Set the Smoothing description for this layer.
				  * \param pSmoothing     Pointer to the Smoothing layer element, or \c NULL to remove the Smoothing definition.
				  * \remarks              A geometry of type KFbxNurb or KFbxPatch should not have Smoothing defined.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementSmoothing,Smoothing);
			
				/** Get the Smoothing description for this layer.
				  * \return      Pointer to the Smoothing layer element, or \c NULL if no Smoothing is defined for this layer.
				  * \remarks     A geometry of type KFbxNurb or KFbxPatch should not have Smoothing defined.
				  */
				//KFbxLayerElementSmoothing const* GetSmoothing() const;
			
				/** Get the User Data for this layer.
				  * \return     Pointer to the User Data layer element, or \c NULL if no User Data is defined for this layer.
				  */				
				/** Set the User Data for this layer.
				  * \param pUserData     Pointer to the User Data layer element, or \c NULL to remove the User Data.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementUserData,UserData);				
			
				/** Get the User Data for this layer.
				  * \return     Pointer to the User Data layer element, or \c NULL if no User Data is defined for this layer.
				  */
			//KFbxLayerElementUserData const* GetUserData() const;
			
				/** Get the visibility for this layer.
				  * \return     Pointer to the visibility layer element, or \c NULL if no visibility is defined for this layer.
				  */
				/** Set the visibility for this layer.
				  * \param pVisibility     Pointer to the visibility layer element, or \c NULL to remove the visibility.
				  */				
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementVisibility,Visibility);				
			
				/** Get the visibility for this layer.
				  * \return     Pointer to the visibility layer element, or \c NULL if no visibility is defined for this layer.
				  */
				//KFbxLayerElementVisibility const* GetVisibility() const;
			
			    /** Get the EmissiveTextures description for this layer.
				  * \return     Pointer to the EmissiveTextures layer element, or \c NULL if no EmissiveTextures are defined for this layer.
				  */
				/** Set the EmissiveTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,EmissiveTextures);				
			
				/** Get the EmissiveTextures description for this layer.
				  * \return     Pointer to the EmissiveTextures layer element, or \c NULL if no EmissiveTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetEmissiveTextures() const;
			
			    /** Get the EmissiveFactorTextures description for this layer.
				  * \return     Pointer to the EmissiveFactorTextures layer element, or \c NULL if no EmissiveFactorTextures are defined for this layer.
				  */
				/** Set the EmissiveFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */				
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,EmissiveFactorTextures);				
			
				/** Get the EmissiveFactorTextures description for this layer.
				  * \return     Pointer to the EmissiveFactorTextures layer element, or \c NULL if no EmissiveFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetEmissiveFactorTextures() const;
			
			    /** Get the AmbientTextures description for this layer.
				  * \return     Pointer to the AmbientTextures layer element, or \c NULL if no AmbientTextures are defined for this layer.
				  */
				/** Set the AmbientTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */				
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,AmbientTextures);				
			
				/** Get the AmbientTextures description for this layer.
				  * \return     Pointer to the AmbientTextures layer element, or \c NULL if no AmbientTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetAmbientTextures() const;
			
			    /** Get the AmbientFactorTextures description for this layer.
				  * \return     Pointer to the AmbientFactorTextures layer element, or \c NULL if no AmbientFactorTextures are defined for this layer.
				  */
				/** Set the AmbientFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,AmbientFactorTextures);				
			
				/** Get the AmbientFactorTextures description for this layer.
				  * \return     Pointer to the AmbientFactorTextures layer element, or \c NULL if no AmbientFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetAmbientFactorTextures() const;
			    
			    /** Get the DiffuseTextures description for this layer.
			     * \return     Pointer to the DiffuseTextures layer element, or \c NULL if no DiffuseTextures are defined for this layer.
				  */
				/** Set the DiffuseTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,DiffuseTextures);				
			
				/** Get the DiffuseTextures description for this layer.
				  * \return     Pointer to the DiffuseTextures layer element, or \c NULL if no DiffuseTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetDiffuseTextures() const;
			
			    /** Get the DiffuseFactorTextures description for this layer.
				  * \return     Pointer to the DiffuseFactorTextures layer element, or \c NULL if no DiffuseFactorTextures are defined for this layer.
				  */
				/** Set the DiffuseFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,DiffuseFactorTextures);
				
			
				/** Get the DiffuseFactorTextures description for this layer.
				  * \return     Pointer to the DiffuseFactorTextures layer element, or \c NULL if no DiffuseFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetDiffuseFactorTextures() const;
			
			    /** Get the SpecularTextures description for this layer.
				  * \return     Pointer to the SpecularTextures layer element, or \c NULL if no SpecularTextures are defined for this layer.
				  */
				/** Set the SpecularTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,SpecularTextures);				
			
				/** Get the SpecularTextures description for this layer.
				  * \return     Pointer to the SpecularTextures layer element, or \c NULL if no SpecularTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetSpecularTextures() const;
			
			    /** Get the SpecularFactorTextures description for this layer.
				  * \return     Pointer to the SpecularFactorTextures layer element, or \c NULL if no SpecularFactorTextures are defined for this layer.
				  */
				/** Set the SpecularFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,SpecularFactorTextures);				
			
				/** Get the SpecularFactorTextures description for this layer.
				  * \return     Pointer to the SpecularFactorTextures layer element, or \c NULL if no SpecularFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetSpecularFactorTextures() const;
			
			    /** Get the ShininessTextures description for this layer.
				  * \return     Pointer to the ShininessTextures layer element, or \c NULL if no ShininessTextures are defined for this layer.
				  */
				/** Set the ShininessTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */				
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,ShininessTextures);				
			
				/** Get the ShininessTextures description for this layer.
				  * \return     Pointer to the ShininessTextures layer element, or \c NULL if no ShininessTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetShininessTextures() const;
			
				/** Get the NormalMapTextures description for this layer.
				  * \return     Pointer to the BumpTextures layer element, or \c NULL if no BumpTextures are defined for this layer.
				  */				
				/** Set the NormalMapTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,NormalMapTextures);				
			
				/** Get the NormalMapTextures description for this layer.
				  * \return     Pointer to the BumpTextures layer element, or \c NULL if no BumpTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetNormalMapTextures() const;
			
			    /** Get the BumpTextures description for this layer.
				  * \return     Pointer to the BumpTextures layer element, or \c NULL if no BumpTextures are defined for this layer.
				  */
				/** Set the BumpTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,BumpTextures);				
			
				/** Get the BumpTextures description for this layer.
				  * \return     Pointer to the BumpTextures layer element, or \c NULL if no BumpTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetBumpTextures() const;
			
			    /** Get the TransparentTextures description for this layer.
				  * \return     Pointer to the TransparentTextures layer element, or \c NULL if no TransparentTextures are defined for this layer.
				  */
				/** Set the TransparentTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */				
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,TransparentTextures);				
			
				/** Get the TransparentTextures description for this layer.
				  * \return     Pointer to the TransparentTextures layer element, or \c NULL if no TransparentTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetTransparentTextures() const;
			
			    /** Get the TransparencyFactorTextures description for this layer.
				  * \return     Pointer to the TransparencyFactorTextures layer element, or \c NULL if no TransparencyFactorTextures are defined for this layer.
				  */
				/** Set the TransparencyFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,TransparencyFactorTextures);				
			
				/** Get the TransparencyFactorTextures description for this layer.
				  * \return     Pointer to the TransparencyFactorTextures layer element, or \c NULL if no TransparencyFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetTransparencyFactorTextures() const;
			
			    /** Get the ReflectionTextures description for this layer.
				  * \return     Pointer to the ReflectionTextures layer element, or \c NULL if no ReflectionTextures are defined for this layer.
				  */
				/** Set the ReflectionTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,ReflectionTextures);				
			
				/** Get the ReflectionTextures description for this layer.
				  * \return     Pointer to the ReflectionTextures layer element, or \c NULL if no ReflectionTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetReflectionTextures() const;
			
			    /** Get the ReflectionFactorTextures description for this layer.
				  * \return     Pointer to the ReflectionFactorTextures layer element, or \c NULL if no ReflectionFactorTextures are defined for this layer.
				  */
				/** Set the ReflectionFactorTextures description for this layer.
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Texture definition.
				  */
				REF_PROPERTY_GETSET_DECLARE(FbxLayerElementTexture,ReflectionFactorTextures);				
			
				/** Get the ReflectionFactorTextures description for this layer.
				  * \return     Pointer to the ReflectionFactorTextures layer element, or \c NULL if no ReflectionFactorTextures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetReflectionFactorTextures() const;
			
				/** Get the Textures description for this layer.
				  * \return     Pointer to the Textures layer element, or \c NULL if no Textures are defined for this layer.
				  */
				FbxLayerElementTexture^ GetTextures(FbxLayerElement::LayerElementType type);
			
				/** Get the Textures description for this layer.
				  * \return     Pointer to the Textures layer element, or \c NULL if no Textures are defined for this layer.
				  */
				//KFbxLayerElementTexture const* GetTextures(KFbxLayerElement::ELayerElementType pType) const;
			
				/** Set the Textures description for this layer.
				  * \param pType         layer element type
				  * \param pTextures     Pointer to the Textures layer element, or \c NULL to remove the Textures definition.
				  */
				void SetTextures(FbxLayerElement::LayerElementType type,FbxLayerElementTexture^ textures);
			
				/** Get the layer element description of the specified type for this layer.
				  * \param pType     The Layer element type required. Supported values are KFbxLayerElement::eNORMAL, KFbxLayerElement::eMATERIAL,
				  *                  KFbxLayerElement::eTEXTURE, KFbxLayerElement::ePOLYGON_GROUP, KFbxLayerElement::eUV and KFbxLayerElement::eVERTEX_COLOR. 
				  *                       - Calling with eNORMAL is equivalent to calling GetNormals().
				  *                       - Calling with eMATERIAL is equivalent to calling GetMaterials().
				  *                       - Calling with ePOLYGON_GROUP is equivalent to calling GetPolygonGroups().
				  *                       - Calling with eUV is equivalent to calling GetUVs().
				  *                       - Calling with eVERTEX_COLOR is equivalent to calling GetVertexColors().
				  *                       - Calling with eSMOOTHING is equivalent to calling GetSmoothing().
				  *                       - Calling with eUSER_DATA is equivalent to calling GetUserData().
			      *                       - Calling with eEMISSIVE_TEXTURES is equivalent to calling GetEmissiveTextures().
			      *                       - Calling with eEMISSIVE_FACTOR_TEXTURES is equivalent to calling GetEmissiveFactorTextures().
			      *                       - Calling with eAMBIENT_TEXTURES is equivalent to calling GetAmbientTextures().
			      *                       - Calling with eAMBIENT_FACTOR_TEXTURES is equivalent to calling GetAmbientFactorTextures().
			      *                       - Calling with eDIFFUSE_TEXTURES is equivalent to calling GetDiffuseTextures().
			      *                       - Calling with eDIFFUSE_FACTOR_TEXTURES is equivalent to calling GetDiffuseFactorTextures().
			      *                       - Calling with eSPECULAR_TEXTURES is equivalent to calling GetSpecularTextures().
			      *                       - Calling with eSPECULAR_FACTOR_TEXTURES is equivalent to calling GetSpecularFactorTextures().
			      *                       - Calling with eSHININESS_TEXTURES is equivalent to calling GetShininessTextures().
			      *                       - Calling with eBUMP_TEXTURES is equivalent to calling GetBumpTextures().
				  *                       - Calling with eNORMALMAP_TEXTURES is equivalent to calling GetNormalMapTextures().
			      *                       - Calling with eTRANSPARENT_TEXTURES is equivalent to calling GetTransparentTextures().
			      *                       - Calling with eTRANSPARENCY_FACTOR_TEXTURES is equivalent to calling GetTransparencyFactorTextures().
			      *                       - Calling with eREFLECTION_TEXTURES is equivalent to calling GetReflectionTextures().
			      *                       - Calling with eREFLECTION_FACTOR_TEXTURES is equivalent to calling GetReflectionFactorTextures().
			      * \param pIsUV     When \c true, request the UV LayerElement corresponding to the specified Layer Element type.
				  * \return          Pointer to the requested layer element, or \e NULL if the layer element is not defined for this layer.
				  */
				FbxLayerElement^ GetLayerElementOfType(FbxLayerElement::LayerElementType type, bool isUV);
				FbxLayerElement^ GetLayerElementOfType(FbxLayerElement::LayerElementType type)
				{
					return GetLayerElementOfType(type,false);
				}
			
				/** Get the layer element description of the specified type for this layer.
				  * \param pType     The Layer element type required. Supported values are KFbxLayerElement::eNORMAL, KFbxLayerElement::eMATERIAL, 
				  *                  KFbxLayerElement::eTEXTURE, KFbxLayerElement::ePOLYGON_GROUP, KFbxLayerElement::eUV and KFbxLayerElement::eVERTEX_COLOR. 
				  *                       - Calling with eNORMAL is equivalent to calling GetNormals().
				  *                       - Calling with eMATERIAL is equivalent to calling GetMaterials().
				  *                       - Calling with ePOLYGON_GROUP is equivalent to calling GetPolygonGroups().
				  *                       - Calling with eUV is equivalent to calling GetUVs().
				  *                       - Calling with eVERTEX_COLOR is equivalent to calling GetVertexColors().
				  *                       - Calling with eSMOOTHING is equivalent to calling GetSmoothing().
				  *                       - Calling with eUSER_DATA is equivalent to calling GetUserData().
			      *                       - Calling with eEMISSIVE_TEXTURES is equivalent to calling GetEmissiveTextures().
			      *                       - Calling with eEMISSIVE_FACTOR_TEXTURES is equivalent to calling GetEmissiveFactorTextures().
			      *                       - Calling with eAMBIENT_TEXTURES is equivalent to calling GetAmbientTextures().
			      *                       - Calling with eAMBIENT_FACTOR_TEXTURES is equivalent to calling GetAmbientFactorTextures().
			      *                       - Calling with eDIFFUSE_TEXTURES is equivalent to calling GetDiffuseTextures().
			      *                       - Calling with eDIFFUSE_FACTOR_TEXTURES is equivalent to calling GetDiffuseFactorTextures().
			      *                       - Calling with eSPECULAR_TEXTURES is equivalent to calling GetSpecularTextures().
			      *                       - Calling with eSPECULAR_FACTOR_TEXTURES is equivalent to calling GetSpecularFactorTextures().
			      *                       - Calling with eSHININESS_TEXTURES is equivalent to calling GetShininessTextures().
			      *                       - Calling with eBUMP_TEXTURES is equivalent to calling GetBumpTextures().
				  *                       - Calling with eNORMALMAP_TEXTURES is equivalent to calling GetNormalMapTextures().
			      *                       - Calling with eTRANSPARENT_TEXTURES is equivalent to calling GetTransparentTextures().
			      *                       - Calling with eTRANSPARENCY_FACTOR_TEXTURES is equivalent to calling GetTransparencyFactorTextures().
			      *                       - Calling with eREFLECTION_TEXTURES is equivalent to calling GetReflectionTextures().
			      *                       - Calling with eREFLECTION_FACTOR_TEXTURES is equivalent to calling GetReflectionFactorTextures().
			      * \param pIsUV     When \c true, request the UV LayerElement corresponding to the specified Layer Element type.
				  * \return          Pointer to the requested layer element, or \e NULL if the layer element is not defined for this layer.
				  */
				//KFbxLayerElement const* GetLayerElementOfType(KFbxLayerElement::ELayerElementType pType, bool pIsUV=false) const;																							
			
				/** Set the UVs description for this layer.
				  * \param pUVs     Pointer to the UVs layer element, or \c NULL to remove the UV definition.
				  * \param pTypeIdentifier         layer element type
				  * \remarks        A geometry of type KFbxNurb or KFbxPatch should not have UVs defined.
				  *                 The Nurbs/Patch parameterization is used as UV parameters to map a texture.
				  */
			void SetUVs(FbxLayerElementUV^ UVs, FbxLayerElement::LayerElementType typeIdentifier);											
			

				/** Create the layer element description of the specified type for this layer.
				  * \param pType     The Layer element type required. Supported values are KFbxLayerElement::eNORMAL, KFbxLayerElement::eMATERIAL,
				  *                  KFbxLayerElement::eTEXTURE, KFbxLayerElement::ePOLYGON_GROUP, KFbxLayerElement::eUV and KFbxLayerElement::eVERTEX_COLOR. 
			      * \param pIsUV     When \c true, request the UV LayerElement corresponding to the specified Layer Element type (only applies to
				  *                  the TEXTURE types layer elements).
				  * \return          Pointer to the newly created layer element, or \e NULL if the layer element has not been created for this layer.
				  */
			FbxLayerElement^ CreateLayerElementOfType(FbxLayerElement::LayerElementType type, bool isUV);
			FbxLayerElement^ CreateLayerElementOfType(FbxLayerElement::LayerElementType type)
			{
				return CreateLayerElementOfType(type,false);
			}
			
			
			void Clone(FbxLayer^ srcLayer, FbxSdkManagerManaged^ sdkManager);	
				
				//! Assignment operator.
				//KFbxLayer& operator=(KFbxLayer const& pSrcLayer);	
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
				/**
				  * \name Serialization section
				  */
				//@{
				bool ContentWriteTo(FbxStream^ stream);
				bool ContentReadFrom(FbxStream^ stream);
				//@}
				virtual property int MemoryUsage
				{
					int get();
				}
			
			#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}