#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateTexture.h"
#include "FbxTexture.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxLayerElementArrayTemplateTexture::FbxLayerElementArrayTemplateTexture(KFbxLayerElementArrayTemplate<KFbxTexture*>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateTexture::FbxLayerElementArrayTemplateTexture(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<KFbxTexture*>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateTexture::Add(FbxTexture^ item)
		{
			return _Ref()->Add(item->_Ref());
		}		
		int FbxLayerElementArrayTemplateTexture::InsertAt(int index,FbxTexture^ item)
		{
			return _Ref()->InsertAt(index,item->_Ref());
		}		
		void FbxLayerElementArrayTemplateTexture::SetAt(int index,FbxTexture^ item)
		{
			_Ref()->SetAt(index,item->_Ref());
		}		
		void FbxLayerElementArrayTemplateTexture::SetLast(FbxTexture^ item)
		{
			_Ref()->SetLast(item->_Ref());
		}	

		FbxTexture^ FbxLayerElementArrayTemplateTexture::RemoveAt(int index)
		{			
			KFbxTexture* t = _Ref()->RemoveAt(index);			
			return FbxCreator::CreateFbxTexture(t);			
		}

		FbxTexture^ FbxLayerElementArrayTemplateTexture::RemoveLast()
		{
			KFbxTexture* t = _Ref()->RemoveLast();			
			return FbxCreator::CreateFbxTexture(t);			
		}		
		bool FbxLayerElementArrayTemplateTexture::RemoveIt(FbxTexture^ item)
		{
			return _Ref()->RemoveIt(item->_Ref());
		}

		FbxTexture^ FbxLayerElementArrayTemplateTexture::GetAt(int index)
		{
			KFbxTexture* t = _Ref()->GetAt(index);			
			return FbxCreator::CreateFbxTexture(t);			
		}		
		FbxTexture^ FbxLayerElementArrayTemplateTexture::GetFirst()
		{
			KFbxTexture* t = _Ref()->GetFirst();			
			return FbxCreator::CreateFbxTexture(t);			
		}		
		FbxTexture^ FbxLayerElementArrayTemplateTexture::GetLast()
		{
			KFbxTexture* t = _Ref()->GetLast();			
			return FbxCreator::CreateFbxTexture(t);			
		}

		int FbxLayerElementArrayTemplateTexture::Find(FbxTexture^ item)
		{
			return _Ref()->Find(item->_Ref());
		}		
		int FbxLayerElementArrayTemplateTexture::FindAfter(int afterIndex, FbxTexture^ item)
		{
			return _Ref()->FindAfter(afterIndex,item->_Ref());
		}		
		int FbxLayerElementArrayTemplateTexture::FindBefore(int beforeIndex, FbxTexture^ item)
		{
			return _Ref()->FindBefore(beforeIndex,item->_Ref());
		}
		FbxTexture^  FbxLayerElementArrayTemplateTexture::default::get(int index)
		{
			return GetAt(index);
		}
	}
}