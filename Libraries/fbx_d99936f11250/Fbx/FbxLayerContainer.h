#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"
#include "FbxLayerElement.h"


{
	namespace FbxSDK
	{		
		ref class FbxSdkManagerManaged;
		ref class FbxClassId;
		ref class FbxLayer;
		/** \brief KFbxLayerContainer is the base class for managing Layers. 
		* This class manages the creation and destruction of layers. 
		* A Layer contains Layer Element(s) of the following types: 
		*      \li Normals
		*      \li Materials
		*      \li Polygon Groups
		*      \li UVs
		*      \li Vertex Color
		*      \li Textures
		* See KFbxLayerElement for more details.
		* \nosubgrouping
		*/
		public ref class FbxLayerContainer : FbxNodeAttribute
		{
			REF_DECLARE(FbxEmitter,KFbxLayerContainer);
		internal:
			FbxLayerContainer(KFbxLayerContainer* instance) :FbxNodeAttribute(instance)
			{
				_Free = false;
			}
		public:

			FBXOBJECT_DECLARE(FbxLayerContainer);
			/** Return the type of node attribute.
			* This class is pure virtual.
			*/
			//virtual EAttributeType GetAttributeType() const { return eUNIDENTIFIED; } 

			/**
			* \name Layer Management 
			*/
			//@{

			/** Create a new layer on top of existing layers.
			* \return     Index of created layer or -1 if an error occured.
			*/
			int CreateLayer();

			//! Delete all layers.
			void ClearLayers();

			/** Get number of layers.
			* \return     Return the number of layers.
			*/
			property int LayerCount
			{
				int get();
			}

			/** Get number of layers containing the specified layer element type.
			* \param pType     The requested Layer Element type.
			* \param pUVCount  When \c true, request the number of UV layers connected to the specified Layer Element type.
			* \return          The number of layers containing a layer of type pType.
			*/
			int GetLayerCount(FbxLayerElement::LayerElementType type,  bool UVCount);
			int GetLayerCount(FbxLayerElement::LayerElementType type);

			/** Get the layer at given index.
			*	\param pIndex     Layer index.
			* \return           Pointer to the layer, or \c NULL if pIndex is out of range.
			*/
			FbxLayer^ GetLayer(int index);
//
//			/** Get the layer at given index.
//			*	\param pIndex     Layer index.
//			* \return           Pointer to the layer, or \c NULL if pIndex is out of range.
//			*/
//			KFbxLayer const* GetLayer(int pIndex) const;
//
			/** Get the n'th layer containing the specified layer element type.
			*	\param pIndex     Layer index.
			* \param pType      Layer element type.
			* \param pIsUV      When \c true, request the UV LayerElement connected to the specified Layer Element type.
			* \return           Pointer to the layer, or \c NULL if pIndex is out of range for the specified type (pType).
			*/
			FbxLayer^ GetLayer(int index, FbxLayerElement::LayerElementType type, bool IsUV);
			FbxLayer^ GetLayer(int index, FbxLayerElement::LayerElementType type)
			{
				return GetLayer(index,type,false);
			}
//
//			/** Get the n'th layer containing the specified layer element type.
//			*	\param pIndex     Layer index.
//			* \param pType      Layer element type.
//			* \param pIsUV      When \c true request the UV LayerElement connected to the specified Layer Element type.
//			* \return           Pointer to the layer, or \c NULL if pIndex is out of range for the specified type (pType).
//			*/
//			KFbxLayer const* GetLayer(int pIndex, KFbxLayerElement::ELayerElementType pType, bool pIsUV=false) const;
//
			/**	Get the index of n'th layer containing the specified layer element type.
			* \param pIndex     Layer index of the specified type.
			* \param pType      Layer type.
			* \param pIsUV      When \c true request the index of the UV LayerElement connected to the specified Layer Element type.
			* \return           Index of the specified layer type, or -1 if the layer is not found.
			* \remarks          The returned index is the position of the layer in the global array of layers.
			*                   You can use the returned index to call GetLayer(int pIndex).
			*/
			int GetLayerIndex(int index, FbxLayerElement::LayerElementType type, bool IsUV);
			int GetLayerIndex(int index, FbxLayerElement::LayerElementType type)
			{
				return GetLayerIndex(index,type,false);
			}

			/** Convert the global index of the layer to a type-specific index.
			* \param pGlobalIndex     The index of the layer in the global array of layers.
			* \param pType            The type uppon which the typed index will be returned.
			* \param pIsUV            When \c true request the index of the UV LayerElement connected to the specified Layer Element type.
			* \return                 Index of the requested layer element type, or -1 if the layer element type is not found.
			*/
			int GetLayerTypedIndex(int globalIndex, FbxLayerElement::LayerElementType type, bool IsUV);
			int GetLayerTypedIndex(int globalIndex, FbxLayerElement::LayerElementType type)
			{
				return GetLayerTypedIndex(globalIndex,type,false);
			}
			//@}

			/** Convert Direct to Index to Direct Reference Mode.
			* \param pLayer     The Layer to convert.
			* \return           \c true if conversion was successful, or \c false otherwise.
			*/
			bool ConvertDirectToIndexToDirect(int layer);

			///////////////////////////////////////////////////////////////////////////////
			//
			//  WARNING!
			//
			//	Anything beyond these lines may not be documented accurately and is 
			// 	subject to change without notice.
			//
			///////////////////////////////////////////////////////////////////////////////

#ifndef DOXYGEN_SHOULD_SKIP_THIS

			int  GTC(kUInt i, int j);
			//void* GT (int  i,    kUInt l, int j); 
			//int  AT (void* t,    kUInt l, int j);
			int  GTI(String^ n, kUInt l, int j);
			//int  GMC(kUInt i, void* n = NULL);
			//void* GM (int  i,    kUInt l, void* n = NULL);
			//int  AM (void* m,    kUInt l, void* n = NULL);
			//int  GMI(char const* n, kUInt l, void* d = NULL);

			int AddToLayerElementsList(FbxLayerElement^ LEl);
			void RemoveFromLayerElementsList(FbxLayerElement^ LEl);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

		};

	}
}