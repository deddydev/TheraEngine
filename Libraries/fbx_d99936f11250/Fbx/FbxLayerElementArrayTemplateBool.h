#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"


{
	namespace FbxSDK
	{		
		public ref class FbxLayerElementArrayTemplateBool: public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<bool>);
		internal:			
			FbxLayerElementArrayTemplateBool(KFbxLayerElementArrayTemplate<bool>* instance);
		public:				
			FbxLayerElementArrayTemplateBool(FbxType dataType);
			int Add(bool item);
			int InsertAt(int index,bool item);
			void SetAt(int index,bool item);
			void SetLast(bool item);			
			bool RemoveAt(int index);
			bool RemoveLast();
			bool RemoveIt(bool item);

			bool GetAt(int index);
			bool GetFirst();
			bool GetLast();

			int Find(bool item);
			int FindAfter(int afterIndex, bool item);
			int FindBefore(int beforeIndex, bool item);

			property bool  default[int]
			{
				bool get(int index);
			}
		};
	}
}