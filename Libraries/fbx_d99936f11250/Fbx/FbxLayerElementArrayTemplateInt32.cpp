#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateInt32.h"


{
	namespace FbxSDK
	{				

		FbxLayerElementArrayTemplateInt32::FbxLayerElementArrayTemplateInt32(KFbxLayerElementArrayTemplate<int>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateInt32::FbxLayerElementArrayTemplateInt32(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<int>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateInt32::Add(int item)
		{
			return _Ref()->Add(item);
		}		
		int FbxLayerElementArrayTemplateInt32::InsertAt(int index,int item)
		{
			return _Ref()->InsertAt(index,item);
		}		
		void FbxLayerElementArrayTemplateInt32::SetAt(int index,int item)
		{
			_Ref()->SetAt(index,item);
		}		
		void FbxLayerElementArrayTemplateInt32::SetLast(int item)
		{
			_Ref()->SetLast(item);
		}	

		int FbxLayerElementArrayTemplateInt32::RemoveAt(int index)
		{			
			return  _Ref()->RemoveAt(index);			
		}

		int FbxLayerElementArrayTemplateInt32::RemoveLast()
		{
			return  _Ref()->RemoveLast();			
		}		
		bool FbxLayerElementArrayTemplateInt32::RemoveIt(int item)
		{
			return _Ref()->RemoveIt(item);
		}

		int FbxLayerElementArrayTemplateInt32::GetAt(int index)
		{
			return _Ref()->GetAt(index);			
		}		
		int FbxLayerElementArrayTemplateInt32::GetFirst()
		{
			return _Ref()->GetFirst();			
		}		
		int FbxLayerElementArrayTemplateInt32::GetLast()
		{
			return  _Ref()->GetLast();			
		}

		int FbxLayerElementArrayTemplateInt32::Find(int item)
		{
			return _Ref()->Find(item);
		}		
		int FbxLayerElementArrayTemplateInt32::FindAfter(int afterIndex, int item)
		{
			return _Ref()->FindAfter(afterIndex,item);
		}		
		int FbxLayerElementArrayTemplateInt32::FindBefore(int beforeIndex,int item)
		{
			return _Ref()->FindBefore(beforeIndex,item);
		}
		int  FbxLayerElementArrayTemplateInt32::default::get(int index)
		{
			return GetAt(index);
		}
	}
}