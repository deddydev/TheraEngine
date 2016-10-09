#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateVector4.h"
#include "FbxVector4.h"

namespace Skill
{
	namespace FbxSDK
	{		
		FbxLayerElementArrayTemplateVector4::FbxLayerElementArrayTemplateVector4(KFbxLayerElementArrayTemplate<KFbxVector4>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateVector4::FbxLayerElementArrayTemplateVector4(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<KFbxVector4*>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateVector4::Add(FbxVector4^ item)
		{
			return _Ref()->Add(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector4::InsertAt(int index,FbxVector4^ item)
		{
			return _Ref()->InsertAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateVector4::SetAt(int index,FbxVector4^ item)
		{
			_Ref()->SetAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateVector4::SetLast(FbxVector4^ item)
		{
			_Ref()->SetLast(*item->_Ref());
		}	

		FbxVector4^ FbxLayerElementArrayTemplateVector4::RemoveAt(int index)
		{			
			KFbxVector4 t = _Ref()->RemoveAt(index);			
			return gcnew FbxVector4(t);			
		}

		FbxVector4^ FbxLayerElementArrayTemplateVector4::RemoveLast()
		{
			KFbxVector4 t = _Ref()->RemoveLast();			
			return gcnew FbxVector4(t);			
		}		
		bool FbxLayerElementArrayTemplateVector4::RemoveIt(FbxVector4^ item)
		{
			return _Ref()->RemoveIt(*item->_Ref());
		}

		FbxVector4^ FbxLayerElementArrayTemplateVector4::GetAt(int index)
		{
			KFbxVector4 t = _Ref()->GetAt(index);			
			return gcnew FbxVector4(t);			
		}		
		FbxVector4^ FbxLayerElementArrayTemplateVector4::GetFirst()
		{
			KFbxVector4 t = _Ref()->GetFirst();			
			return gcnew FbxVector4(t);			
		}		
		FbxVector4^ FbxLayerElementArrayTemplateVector4::GetLast()
		{
			KFbxVector4 t = _Ref()->GetLast();			
			return gcnew FbxVector4(t);			
		}

		int FbxLayerElementArrayTemplateVector4::Find(FbxVector4^ item)
		{
			return _Ref()->Find(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector4::FindAfter(int afterIndex, FbxVector4^ item)
		{
			return _Ref()->FindAfter(afterIndex,*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector4::FindBefore(int beforeIndex, FbxVector4^ item)
		{
			return _Ref()->FindBefore(beforeIndex,*item->_Ref());
		}
		FbxVector4^  FbxLayerElementArrayTemplateVector4::default::get(int index)
		{
			return GetAt(index);
		}
	}
}