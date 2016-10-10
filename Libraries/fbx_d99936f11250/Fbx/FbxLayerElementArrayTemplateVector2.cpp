#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateVector2.h"
#include "FbxVector2.h"


{
	namespace FbxSDK
	{

		FbxLayerElementArrayTemplateVector2::FbxLayerElementArrayTemplateVector2(KFbxLayerElementArrayTemplate<KFbxVector2>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateVector2::FbxLayerElementArrayTemplateVector2(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<KFbxVector2*>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateVector2::Add(FbxVector2^ item)
		{
			return _Ref()->Add(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector2::InsertAt(int index,FbxVector2^ item)
		{
			return _Ref()->InsertAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateVector2::SetAt(int index,FbxVector2^ item)
		{
			_Ref()->SetAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateVector2::SetLast(FbxVector2^ item)
		{
			_Ref()->SetLast(*item->_Ref());
		}	

		FbxVector2^ FbxLayerElementArrayTemplateVector2::RemoveAt(int index)
		{			
			KFbxVector2 t = _Ref()->RemoveAt(index);			
			return gcnew FbxVector2(t);			
		}

		FbxVector2^ FbxLayerElementArrayTemplateVector2::RemoveLast()
		{
			KFbxVector2 t = _Ref()->RemoveLast();			
			return gcnew FbxVector2(t);			
		}		
		bool FbxLayerElementArrayTemplateVector2::RemoveIt(FbxVector2^ item)
		{
			return _Ref()->RemoveIt(*item->_Ref());
		}

		FbxVector2^ FbxLayerElementArrayTemplateVector2::GetAt(int index)
		{
			KFbxVector2 t = _Ref()->GetAt(index);			
			return gcnew FbxVector2(t);			
		}		
		FbxVector2^ FbxLayerElementArrayTemplateVector2::GetFirst()
		{
			KFbxVector2 t = _Ref()->GetFirst();			
			return gcnew FbxVector2(t);			
		}		
		FbxVector2^ FbxLayerElementArrayTemplateVector2::GetLast()
		{
			KFbxVector2 t = _Ref()->GetLast();			
			return gcnew FbxVector2(t);			
		}

		int FbxLayerElementArrayTemplateVector2::Find(FbxVector2^ item)
		{
			return _Ref()->Find(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector2::FindAfter(int afterIndex, FbxVector2^ item)
		{
			return _Ref()->FindAfter(afterIndex,*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateVector2::FindBefore(int beforeIndex, FbxVector2^ item)
		{
			return _Ref()->FindBefore(beforeIndex,*item->_Ref());
		}
		FbxVector2^  FbxLayerElementArrayTemplateVector2::default::get(int index)
		{
			return GetAt(index);
		}
	}
}