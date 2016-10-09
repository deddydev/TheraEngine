#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxType.h"
#include "FbxNativePointer.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStream;
		ref class FbxDataType;
		ref class FbxObjectManaged;
		/** \brief KFbxLayerElement is the base class for Layer Elements. 
		* It describes how a Layer Element is mapped on a geometry surface and how the 
		* mapping information is arranged in memory.
		* \nosubgrouping
		*/
		public ref class FbxLayerElement : IFbxNativePointer
		{
			INTERNAL_CLASS_DECLARE(FbxLayerElement,KFbxLayerElement);
			REF_DECLARE(FbxLayerElement,KFbxLayerElement);
			DESTRUCTOR_DECLARE_2(FbxLayerElement);
			INATIVEPOINTER_DECLARE(FbxLayerElement,KFbxLayerElement);
		public:

			/** \enum ELayerElementType     Type identifier for Layer Elements.
			* - \e eUNDEFINED                        Undefined Layer Element class.
			* - \e eNORMAL                           Layer Element of type KFbxLayerElementNormal.
			* - \e eMATERIAL                         Layer Element of type KFbxLayerElementMaterial.
			* - \e eDIFFUSE_TEXTURES                 Layer Element of type KFbxLayerElementTexture.
			* - \e ePOLYGON_GROUP                    Layer Element of type KFbxLayerElementPolygonGroup.
			* - \e eUV                               Layer Element of type KFbxLayerElementUV.
			* - \e eVERTEX_COLOR                     Layer Element of type KFbxLayerElementVertexColor.
			* - \e eSMOOTHING                        Layer Element of type KFbxLayerElementSmoothing.
			* - \e eUSER_DATA                        Layer Element of type KFbxLayerElementUserData.
			* - \e eVISIBILITY                       Layer Element of type KFbxLayerElementVisibility.
			* - \e eEMISSIVE_TEXTURES                Layer Element of type KFbxLayerElementTexture.
			* - \e eEMISSIVE_FACTOR_TEXTURES         Layer Element of type KFbxLayerElementTexture.
			* - \e eAMBIENT_TEXTURES                 Layer Element of type KFbxLayerElementTexture.
			* - \e eAMBIENT_FACTOR_TEXTURES          Layer Element of type KFbxLayerElementTexture.
			* - \e eDIFFUSE_FACTOR_TEXTURES          Layer Element of type KFbxLayerElementTexture.
			* - \e eSPECULAR_TEXTURES                Layer Element of type KFbxLayerElementTexture.
			* - \e eNORMALMAP_TEXTURES               Layer Element of type KFbxLayerElementTexture.
			* - \e eSPECULAR_FACTOR_TEXTURES         Layer Element of type KFbxLayerElementTexture.
			* - \e eSHININESS_TEXTURES               Layer Element of type KFbxLayerElementTexture.
			* - \e eBUMP_TEXTURES                    Layer Element of type KFbxLayerElementTexture.
			* - \e eTRANSPARENT_TEXTURES             Layer Element of type KFbxLayerElementTexture.
			* - \e eTRANSPARENCY_FACTOR_TEXTURES     Layer Element of type KFbxLayerElementTexture.
			* - \e eREFLECTION_TEXTURES              Layer Element of type KFbxLayerElementTexture.
			* - \e eREFLECTION_FACTOR_TEXTURES       Layer Element of type KFbxLayerElementTexture.
			* - \e eLAST_ELEMENT_TYPE
			*/
			enum class LayerElementType
			{
				Undefined = KFbxLayerElement::eUNDEFINED,
				Normal = KFbxLayerElement::eNORMAL,
				Material = KFbxLayerElement::eMATERIAL,
				DiffuseTextures = KFbxLayerElement::eDIFFUSE_TEXTURES,
				PolygonGroup = KFbxLayerElement::ePOLYGON_GROUP,
				UV = KFbxLayerElement::eUV,
				VertexColor = KFbxLayerElement::eVERTEX_COLOR,
				Smoothing = KFbxLayerElement::eSMOOTHING,
				UserData = KFbxLayerElement::eUSER_DATA,
				Visibility = KFbxLayerElement::eVISIBILITY,
				EmissiveTextures = KFbxLayerElement::eEMISSIVE_TEXTURES,
				EmissiveFactorTextures = KFbxLayerElement::eEMISSIVE_FACTOR_TEXTURES,
				AmbientTextures = KFbxLayerElement::eAMBIENT_TEXTURES,
				AmbientFactorTextures = KFbxLayerElement::eAMBIENT_FACTOR_TEXTURES,
				DiffuseFactorTextures = KFbxLayerElement::eDIFFUSE_FACTOR_TEXTURES,
				SpecularTextures = KFbxLayerElement::eSPECULAR_TEXTURES,
				NormalmapTextures = KFbxLayerElement::eNORMALMAP_TEXTURES,
				SpecularFactorTextures = KFbxLayerElement::eSPECULAR_FACTOR_TEXTURES ,
				ShininessTextures = KFbxLayerElement::eSHININESS_TEXTURES,
				BumpTextures = KFbxLayerElement::eBUMP_TEXTURES,
				TransparentTextures = KFbxLayerElement::eTRANSPARENT_TEXTURES,
				TransparencyFactorTextures = KFbxLayerElement::eTRANSPARENCY_FACTOR_TEXTURES,
				ReflectionTextures = KFbxLayerElement::eREFLECTION_TEXTURES ,
				ReflectionFactorTextures = KFbxLayerElement::eREFLECTION_FACTOR_TEXTURES ,
				LastElementType = KFbxLayerElement::eLAST_ELEMENT_TYPE
			};

			/**	\enum EMappingMode     Determine how the element is mapped on a surface.
			* - \e eNONE                  The mapping is undetermined.
			* - \e eBY_CONTROL_POINT      There will be one mapping coordinate for each surface control point/vertex.
			* - \e eBY_POLYGON_VERTEX     There will be one mapping coordinate for each vertex, for each polygon it is part of.
			This means that a vertex will have as many mapping coordinates as polygons it is part of.
			* - \e eBY_POLYGON            There can be only one mapping coordinate for the whole polygon.
			* - \e eBY_EDGE               There will be one mapping coordinate for each unique edge in the mesh.
			This is meant to be used with smoothing layer elements.
			* - \e eALL_SAME              There can be only one mapping coordinate for the whole surface.
			*/
			enum class MappingMode
			{
				None = KFbxLayerElement::eNONE,
				ByControlPoint = KFbxLayerElement::eBY_CONTROL_POINT,
				ByPolygonVertex = KFbxLayerElement::eBY_POLYGON_VERTEX,
				ByPolygon = KFbxLayerElement::eBY_POLYGON,
				ByEdge = KFbxLayerElement::eBY_EDGE,
				AllSame = KFbxLayerElement::eALL_SAME
			};

			/** \enum EReferenceMode     Determine how the mapping information is stored in the array of coordinate.
			* - \e eDIRECT              This indicates that the mapping information for the n'th element is found in the n'th place of 
			KFbxLayerElementTemplate::mDirectArray.
			* - \e eINDEX,              This symbol is kept for backward compatibility with FBX v5.0 files. In FBX v6.0 and higher, 
			this symbol is replaced with eINDEX_TO_DIRECT.
			* - \e eINDEX_TO_DIRECT     This indicates that the KFbxLayerElementTemplate::mIndexArray
			contains, for the n'th element, an index in the KFbxLayerElementTemplate::mDirectArray
			array of mapping elements. eINDEX_TO_DIRECT is usually useful to store coordinates
			for eBY_POLYGON_VERTEX mapping mode elements. Since the same coordinates are usually
			repeated a large number of times, it saves spaces to store the coordinate only one time
			and refer to them with an index. Materials and Textures are also referenced with this
			mode and the actual Material/Texture can be accessed via the KFbxLayerElementTemplate::mDirectArray
			*/
			enum class ReferenceMode
			{
				Direct = KFbxLayerElement::eDIRECT,
				Index = KFbxLayerElement::eINDEX,
				IndexToDirect = KFbxLayerElement::eINDEX_TO_DIRECT
			};


			/** Get the Mapping Mode
			* \return     The current Mapping Mode.
			*/
			/** Set the Mapping Mode
			* \param pMappingMode     Specify the way the layer element is mapped on a surface.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(MappingMode,Mapping_Mode);						

			/** Get the Reference Mode
			* \return     The current Reference Mode.
			*/
			/** Set the Reference Mode
			* \param pReferenceMode     Specify the reference mode.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(ReferenceMode,Reference_Mode);				

			/** Get the name of this object
			* \return     The current name of this LayerElement object.
			*/											
			/** Set the name of this object
			* \param pName     Specify the name of this LayerElement object.
			*/
			VALUE_PROPERTY_GETSET_DECLARE(String^,Name);												


			virtual bool Equals(Object^ obj) override
			{
				FbxLayerElement^ o = dynamic_cast<FbxLayerElement^>(obj);
				if(o)
					return *_Ref() == *o->_Ref();
				return false;
			}			


			//KFbxLayerElement& operator=( KFbxLayerElement const& pOther )
			//{
			//	mMappingMode = pOther.mMappingMode;
			//	mReferenceMode = pOther.mReferenceMode;
			//	// name, type and owner should not be copied because they are
			//	// initialized when this object is created
			//	return *this;
			//}

			/** Delete this object
			*/
			void Destroy();

			void CopyFrom(FbxLayerElement^ other)
			{
				*_FbxLayerElement = *other->_FbxLayerElement;
			}

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			void SetType(FbxDataType^ type);
			//const KFbxLayerContainer* GetOwner() const { return mOwner; }									
		public:
			/** called to query the amount of memory used by this
			* object AND its content (does not consider the content pointed)
			* \return           the amount of memory used.
			*/
			virtual VALUE_PROPERTY_GET_DECLARE(int,MemorySize);

			/**
			* \name Serialization section
			*/
			//@{
			virtual bool ContentWriteTo(FbxStream^ stream);
			virtual bool ContentReadFrom(FbxStream^ stream);
			//@}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		};

	}
}