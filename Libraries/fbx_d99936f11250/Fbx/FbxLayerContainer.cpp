#pragma once
#include "stdafx.h"
#include "FbxLayerContainer.h"
#include "FbxLayer.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"



{
	namespace FbxSDK
	{

		FBXOBJECT_DEFINITION(FbxLayerContainer,KFbxLayerContainer);					

		int FbxLayerContainer::CreateLayer()
		{
			return _Ref()->CreateLayer();
		}
		void FbxLayerContainer::ClearLayers()
		{
			_Ref()->ClearLayers();
		}
		int FbxLayerContainer::LayerCount::get()
		{
			return _Ref()->GetLayerCount();
		}			
		int FbxLayerContainer::GetLayerCount(FbxLayerElement::LayerElementType type,  bool UVCount)
		{
			return _Ref()->GetLayerCount((KFbxLayerElement::ELayerElementType)type,   UVCount);
		}
		int FbxLayerContainer::GetLayerCount(FbxLayerElement::LayerElementType type)
		{
			return _Ref()->GetLayerCount((KFbxLayerElement::ELayerElementType)type);
		}

		FbxLayer^ FbxLayerContainer::GetLayer(int index)
		{
			KFbxLayer* l = _Ref()->GetLayer(index);
			if(l)
				return gcnew FbxLayer(l);
			return nullptr;
		}

		FbxLayer^ FbxLayerContainer::GetLayer(int index, FbxLayerElement::LayerElementType type, bool IsUV)
		{
			KFbxLayer* l = _Ref()->GetLayer(index, (KFbxLayerElement::ELayerElementType)type,IsUV);
			if(l)
				return gcnew FbxLayer(l);
			return nullptr;
		}
		int FbxLayerContainer::GetLayerIndex(int index, FbxLayerElement::LayerElementType type, bool IsUV)
		{
			return _Ref()->GetLayerIndex(index, (KFbxLayerElement::ELayerElementType)type,IsUV);
		}
		int FbxLayerContainer::GetLayerTypedIndex(int globalIndex, FbxLayerElement::LayerElementType type, bool IsUV)
		{
			return _Ref()->GetLayerTypedIndex(globalIndex, (KFbxLayerElement::ELayerElementType)type,IsUV);
		}

		bool FbxLayerContainer::ConvertDirectToIndexToDirect(int layer)
		{
			return _Ref()->ConvertDirectToIndexToDirect(layer);
		}


		#ifndef DOXYGEN_SHOULD_SKIP_THIS

			int  FbxLayerContainer::GTC(kUInt i, int j)
			{
				return _Ref()->GTC(i,j);
			}
			int  FbxLayerContainer::GTI(String^ n, kUInt l, int j)
			{				
				STRINGTO_CONSTCHAR_ANSI(n1,n);
				int i = _Ref()->GTI(n1,l,j);			
				FREECHARPOINTER(n1);
				return i;

			}
			int FbxLayerContainer::AddToLayerElementsList(FbxLayerElement^ LEl)
			{
				return _Ref()->AddToLayerElementsList(LEl->_Ref());
			}
			void FbxLayerContainer::RemoveFromLayerElementsList(FbxLayerElement^ LEl)
			{
				_Ref()->RemoveFromLayerElementsList(LEl->_Ref());
			}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}