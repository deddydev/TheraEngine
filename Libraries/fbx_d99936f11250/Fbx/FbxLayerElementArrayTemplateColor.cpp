#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateColor.h"
#include "FbxColor.h"


{
	namespace FbxSDK
	{		
		FbxLayerElementArrayTemplateColor::FbxLayerElementArrayTemplateColor(KFbxLayerElementArrayTemplate<KFbxColor>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateColor::FbxLayerElementArrayTemplateColor(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<KFbxColor*>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateColor::Add(FbxColor^ item)
		{
			return _Ref()->Add(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateColor::InsertAt(int index,FbxColor^ item)
		{
			return _Ref()->InsertAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateColor::SetAt(int index,FbxColor^ item)
		{
			_Ref()->SetAt(index,*item->_Ref());
		}		
		void FbxLayerElementArrayTemplateColor::SetLast(FbxColor^ item)
		{
			_Ref()->SetLast(*item->_Ref());
		}	

		FbxColor^ FbxLayerElementArrayTemplateColor::RemoveAt(int index)
		{			
			KFbxColor t = _Ref()->RemoveAt(index);			
			return gcnew FbxColor(t);			
		}

		FbxColor^ FbxLayerElementArrayTemplateColor::RemoveLast()
		{
			KFbxColor t = _Ref()->RemoveLast();			
			return gcnew FbxColor(t);			
		}		
		bool FbxLayerElementArrayTemplateColor::RemoveIt(FbxColor^ item)
		{
			return _Ref()->RemoveIt(*item->_Ref());
		}

		FbxColor^ FbxLayerElementArrayTemplateColor::GetAt(int index)
		{
			KFbxColor t = _Ref()->GetAt(index);			
			return gcnew FbxColor(t);			
		}		
		FbxColor^ FbxLayerElementArrayTemplateColor::GetFirst()
		{
			KFbxColor t = _Ref()->GetFirst();			
			return gcnew FbxColor(t);			
		}		
		FbxColor^ FbxLayerElementArrayTemplateColor::GetLast()
		{
			KFbxColor t = _Ref()->GetLast();			
			return gcnew FbxColor(t);			
		}

		int FbxLayerElementArrayTemplateColor::Find(FbxColor^ item)
		{
			return _Ref()->Find(*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateColor::FindAfter(int afterIndex, FbxColor^ item)
		{
			return _Ref()->FindAfter(afterIndex,*item->_Ref());
		}		
		int FbxLayerElementArrayTemplateColor::FindBefore(int beforeIndex, FbxColor^ item)
		{
			return _Ref()->FindBefore(beforeIndex,*item->_Ref());
		}
		FbxColor^  FbxLayerElementArrayTemplateColor::default::get(int index)
		{
			return GetAt(index);
		}
	}
}