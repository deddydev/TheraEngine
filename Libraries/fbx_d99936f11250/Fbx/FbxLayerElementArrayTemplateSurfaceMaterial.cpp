#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateSurfaceMaterial.h"
#include "FbxVector2.h"
#include "FbxSurfaceMaterial.h"


{
	namespace FbxSDK
	{
		FbxLayerElementArrayTemplateSurfaceMaterial::FbxLayerElementArrayTemplateSurfaceMaterial(KFbxLayerElementArrayTemplate<KFbxSurfaceMaterial*>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateSurfaceMaterial::FbxLayerElementArrayTemplateSurfaceMaterial(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<KFbxSurfaceMaterial*>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateSurfaceMaterial::Add(FbxSurfaceMaterial^ item)
		{
			return _Ref()->Add(item->_Ref());
		}		
		int FbxLayerElementArrayTemplateSurfaceMaterial::InsertAt(int index,FbxSurfaceMaterial^ item)
		{
			return _Ref()->InsertAt(index,item->_Ref());
		}		
		void FbxLayerElementArrayTemplateSurfaceMaterial::SetAt(int index,FbxSurfaceMaterial^ item)
		{
			_Ref()->SetAt(index,item->_Ref());
		}		
		void FbxLayerElementArrayTemplateSurfaceMaterial::SetLast(FbxSurfaceMaterial^ item)
		{
			_Ref()->SetLast(item->_Ref());
		}	

		FbxSurfaceMaterial^ FbxLayerElementArrayTemplateSurfaceMaterial::RemoveAt(int index)
		{			
			KFbxSurfaceMaterial* t = _Ref()->RemoveAt(index);
			if(t)
				return FbxCreator::CreateFbxSurfaceMaterial(t);
			return nullptr;
		}

		FbxSurfaceMaterial^ FbxLayerElementArrayTemplateSurfaceMaterial::RemoveLast()
		{
			KFbxSurfaceMaterial* t = _Ref()->RemoveLast();
			if(t)
				return FbxCreator::CreateFbxSurfaceMaterial(t);
			return nullptr;
		}		
		bool FbxLayerElementArrayTemplateSurfaceMaterial::RemoveIt(FbxSurfaceMaterial^ item)
		{
			return _Ref()->RemoveIt(item->_Ref());
		}

		FbxSurfaceMaterial^ FbxLayerElementArrayTemplateSurfaceMaterial::GetAt(int index)
		{
			KFbxSurfaceMaterial* t = _Ref()->GetAt(index);
			if(t)
				return FbxCreator::CreateFbxSurfaceMaterial(t);
			return nullptr;
		}		
		FbxSurfaceMaterial^ FbxLayerElementArrayTemplateSurfaceMaterial::GetFirst()
		{
			KFbxSurfaceMaterial* t = _Ref()->GetFirst();
			if(t)
				return FbxCreator::CreateFbxSurfaceMaterial(t);
			return nullptr;
		}		
		FbxSurfaceMaterial^ FbxLayerElementArrayTemplateSurfaceMaterial::GetLast()
		{
			KFbxSurfaceMaterial* t = _Ref()->GetLast();
			if(t)
				return FbxCreator::CreateFbxSurfaceMaterial(t);
			return nullptr;
		}

		int FbxLayerElementArrayTemplateSurfaceMaterial::Find(FbxSurfaceMaterial^ item)
		{
			return _Ref()->Find(item->_Ref());
		}		
		int FbxLayerElementArrayTemplateSurfaceMaterial::FindAfter(int afterIndex, FbxSurfaceMaterial^ item)
		{
			return _Ref()->FindAfter(afterIndex,item->_Ref());
		}		
		int FbxLayerElementArrayTemplateSurfaceMaterial::FindBefore(int beforeIndex, FbxSurfaceMaterial^ item)
		{
			return _Ref()->FindBefore(beforeIndex,item->_Ref());
		}
		FbxSurfaceMaterial^  FbxLayerElementArrayTemplateSurfaceMaterial::default::get(int index)
		{
			return GetAt(index);
		}


	}
}