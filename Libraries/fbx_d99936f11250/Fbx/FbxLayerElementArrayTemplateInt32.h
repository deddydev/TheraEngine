#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"


{
	namespace FbxSDK
	{		
		public ref class FbxLayerElementArrayTemplateInt32: public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<int>);
		internal:			
			FbxLayerElementArrayTemplateInt32(KFbxLayerElementArrayTemplate<int>* instance);
		public:				
			FbxLayerElementArrayTemplateInt32(FbxType dataType);
			int Add(int item);
			int InsertAt(int index,int item);
			void SetAt(int index,int item);
			void SetLast(int item);			
			int RemoveAt(int index);
			int RemoveLast();
			bool RemoveIt(int item);

			int GetAt(int index);
			int GetFirst();
			int GetLast();

			int Find(int item);
			int FindAfter(int afterIndex, int item);
			int FindBefore(int beforeIndex, int item);

			property int  default[int]
			{
				int get(int index);
			}
		};


	}
}