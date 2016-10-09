#pragma once
#include "stdafx.h"
#include "FbxStringRefArray.h"
#include "FbxString.h"

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStringManaged;

		namespace Arrays
		{
			void FbxStringRefArray::CollectManagedMemory()
			{
				if(_list)
					_list->Clear();
				_list = nullptr;
			}
			int FbxStringRefArray::Add(RefStringClass^ item)
			{
				int i = _Ref()->Add(item->_Ref());
				if(i>=0)
					_list->Add(item);
				return i;
			}
			void FbxStringRefArray::Clear()
			{
				_Ref()->Clear();
				_list->Clear();
			}
			void FbxStringRefArray::Empty()
			{
				_Ref()->Empty();
				_list->Clear();
			}
			int FbxStringRefArray::Find(RefStringClass^ item)
			{
				return _Ref()->Find(item->_Ref());
			}
			int FbxStringRefArray::FindAfter(int afterIndex,RefStringClass^ item)
			{
				return _Ref()->FindAfter(afterIndex,item->_Ref());
			}
			int FbxStringRefArray::FindBefore(int beforeIndex,RefStringClass^ item)
			{
				return _Ref()->FindBefore(beforeIndex,item->_Ref());
			}

			RefStringClass^ FbxStringRefArray::GetAt(int index)
			{
				if(index<0||index > _list->Count)
					return nullptr;
				if(!_list[index])
				{
					RefStringValue k = (*_FbxStringRefArray)[index];
					if(k)
					{
						_list[index] = gcnew RefStringClass(k);
					}
				}
				return _list[index];
			}
			void FbxStringRefArray::SetAt(int index,RefStringClass^ item)
			{
				if(index<0||index > _list->Count)
					return;
				_list[index] = item;
				if(item)				
					_Ref()->SetAt(index,item->_Ref());				
				else
					_Ref()->SetAt(index,NULL);
			}

			RefStringClass^ FbxStringRefArray::default::get(int index)
			{
				return GetAt(index);
			}
			void FbxStringRefArray::default::set(int index,RefStringClass^ value)
			{
				SetAt(index,value);
			}
			int FbxStringRefArray::Count::get()
			{
				return _Ref()->GetCount();
			}

			RefStringClass^ FbxStringRefArray::First::get()
			{
				return GetAt(0);
			}
			RefStringClass^ FbxStringRefArray::Last::get()
			{
				return GetAt(_list->Count-1);
			}

			int FbxStringRefArray::InsertAt(int index,RefStringClass^ item)
			{
				int i = _Ref()->InsertAt(index,item->_Ref());
				if(i>=0)
					_list->Insert(index,item);
				return i;
			}
			RefStringClass^ FbxStringRefArray::RemoveAt(int index)
			{
				if(_Ref()->RemoveAt(index))
				{
					RefStringClass^ item = GetAt(index);
					_list->RemoveAt(index);
					return item;
				}
				return nullptr;
			}
			bool FbxStringRefArray::RemoveIt(RefStringClass^ item)
			{
				bool b = _Ref()->RemoveIt(item->_Ref());
				if(b)
					_list->Remove(item);
				return b;
			}
			RefStringClass^ FbxStringRefArray::RemoveLast()
			{
				if(_Ref()->RemoveLast())
				{
					RefStringClass^ item = Last;
					_list->RemoveAt(_list->Count-1);
					return item;
				}
				return nullptr;
			}

			void FbxStringRefArray::DeleteAndClear()
			{
				kUInt lItemCount = this->Count;
				while(lItemCount)
				{
					lItemCount--;
					RefStringClass^ item = GetAt(lItemCount);					
					item->_Free = true;
					item->~FbxStringManaged();
					item = nullptr;
				}
				_Ref()->Clear();
				_list->Clear();
			}
		}
	}
}