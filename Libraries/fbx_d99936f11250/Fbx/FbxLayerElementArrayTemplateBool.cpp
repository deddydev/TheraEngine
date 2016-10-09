#pragma once
#include "stdafx.h"
#include "FbxLayerElementArrayTemplateBool.h"

namespace Skill
{
	namespace FbxSDK
	{				

		FbxLayerElementArrayTemplateBool::FbxLayerElementArrayTemplateBool(KFbxLayerElementArrayTemplate<bool>* instance)
			: FbxLayerElementArray(instance)
		{
			_Free = false;
		}		
		FbxLayerElementArrayTemplateBool::FbxLayerElementArrayTemplateBool(FbxType dataType)
			:FbxLayerElementArray(new KFbxLayerElementArrayTemplate<bool>((EFbxType)dataType))
		{
			_Free = true;
		}

		int FbxLayerElementArrayTemplateBool::Add(bool item)
		{
			return _Ref()->Add(item);
		}		
		int FbxLayerElementArrayTemplateBool::InsertAt(int index,bool item)
		{
			return _Ref()->InsertAt(index,item);
		}		
		void FbxLayerElementArrayTemplateBool::SetAt(int index,bool item)
		{
			_Ref()->SetAt(index,item);
		}		
		void FbxLayerElementArrayTemplateBool::SetLast(bool item)
		{
			_Ref()->SetLast(item);
		}	

		bool FbxLayerElementArrayTemplateBool::RemoveAt(int index)
		{			
			return  _Ref()->RemoveAt(index);			
		}

		bool FbxLayerElementArrayTemplateBool::RemoveLast()
		{
			return  _Ref()->RemoveLast();			
		}		
		bool FbxLayerElementArrayTemplateBool::RemoveIt(bool item)
		{
			return _Ref()->RemoveIt(item);
		}

		bool FbxLayerElementArrayTemplateBool::GetAt(int index)
		{
			return _Ref()->GetAt(index);			
		}		
		bool FbxLayerElementArrayTemplateBool::GetFirst()
		{
			return _Ref()->GetFirst();			
		}		
		bool FbxLayerElementArrayTemplateBool::GetLast()
		{
			return  _Ref()->GetLast();			
		}

		int FbxLayerElementArrayTemplateBool::Find(bool item)
		{
			return _Ref()->Find(item);
		}		
		int FbxLayerElementArrayTemplateBool::FindAfter(int afterIndex, bool item)
		{
			return _Ref()->FindAfter(afterIndex,item);
		}		
		int FbxLayerElementArrayTemplateBool::FindBefore(int beforeIndex,bool item)
		{
			return _Ref()->FindBefore(beforeIndex,item);
		}
		bool  FbxLayerElementArrayTemplateBool::default::get(int index)
		{
			return GetAt(index);
		}
	}
}