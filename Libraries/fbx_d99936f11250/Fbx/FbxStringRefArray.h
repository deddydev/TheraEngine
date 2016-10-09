#pragma once
#include "stdafx.h"
#include "Fbx.h"


namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStringManaged;
	}
}

using Skill::FbxSDK::FbxStringManaged;

typedef  FbxString*  RefStringValue;
typedef  FbxStringManaged RefStringClass;

using namespace System::Collections::Generic;

namespace Skill
{
	namespace FbxSDK
	{
		ref class FbxStringManaged;

		namespace Arrays
		{
			public ref class FbxStringRefArray : IFbxNativePointer
			{				
				REF_DECLARE(FbxStringRefArray,KArrayTemplate<RefStringValue>);
				DESTRUCTOR_DECLARE(FbxStringRefArray);
				INATIVEPOINTER_DECLARE(FbxStringRefArray,KArrayTemplate<RefStringValue>);
			private: 
				bool _disposed;
			internal: 
				bool _Free;
				KArrayTemplate<RefStringValue>* _FbxStringRefArray;				
				void _SetPointer(KArrayTemplate<RefStringValue>* instance , bool free)
				{
					_FbxStringRefArray = instance;_Free = free;
				}

				FbxStringRefArray(KArrayTemplate<RefStringValue>* instance)
				{
					_FbxStringRefArray = instance;
					_Free = false;
					_list = gcnew List<RefStringClass^>();
					for(int i =0;i< instance->GetCount();i++)
						_list->Add(nullptr);
				}				

				void UpdateInternalList()
				{
					int c = _list->Count;
					if( _Ref()->GetCount() > c)
					{						
						for(int i = c ;i< _Ref()->GetCount();i++)
							_list->Add(nullptr);
					}
					else if( _Ref()->GetCount() < c)
					{						
						while( c >_Ref()->GetCount())
						{
							_list->RemoveAt(c-1);
							c = _list->Count;
						}
					}
				}

			public:
				property bool Disposed {bool get(){return _disposed;}}
			protected: virtual void CollectManagedMemory();	
					   List<RefStringClass^>^ _list;
					   RefStringClass^ GetAt(int index);
					   void SetAt(int index,RefStringClass^ item);
			public:
				FbxStringRefArray()
				{
					_SetPointer(new KArrayTemplate<RefStringValue>(),true);
					_list = gcnew List<RefStringClass^>();					
				}

				int Add(RefStringClass^ item);
				void Clear();
				void Empty();
				int Find(RefStringClass^ item);
				int FindAfter(int afterIndex,RefStringClass^ item);
				int FindBefore(int beforeIndex,RefStringClass^ item);

				property RefStringClass^ default[int]
				{
					RefStringClass^ get(int index);
					void set(int index,RefStringClass^ value);
				}
				property int Count
				{
					int get();
				}

				VALUE_PROPERTY_GET_DECLARE(RefStringClass^,First);
				VALUE_PROPERTY_GET_DECLARE(RefStringClass^,Last);				

				int InsertAt(int index,RefStringClass^ item);
				RefStringClass^ RemoveAt(int index);
				bool RemoveIt(RefStringClass^ item);
				RefStringClass^ RemoveLast();
				void DeleteAndClear();

			};
		}
	}
}