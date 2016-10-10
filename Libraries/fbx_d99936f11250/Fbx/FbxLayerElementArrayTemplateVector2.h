#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"


{
	namespace FbxSDK
	{		
		ref class FbxVector2;		
				
		public ref class FbxLayerElementArrayTemplateVector2 : public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<KFbxVector2>);
		internal:			
			FbxLayerElementArrayTemplateVector2(KFbxLayerElementArrayTemplate<KFbxVector2>* instance);
		public:				
			FbxLayerElementArrayTemplateVector2(FbxType dataType);
			int Add(FbxVector2^ item);
			int InsertAt(int index,FbxVector2^ item);
			void SetAt(int index,FbxVector2^ item);
			void SetLast(FbxVector2^ item);			
			FbxVector2^ RemoveAt(int index);
			FbxVector2^ RemoveLast();
			bool RemoveIt(FbxVector2^ item);

			FbxVector2^ GetAt(int index);
			FbxVector2^ GetFirst();
			FbxVector2^ GetLast();

			int Find(FbxVector2^ item);
			int FindAfter(int afterIndex, FbxVector2^ item);
			int FindBefore(int beforeIndex, FbxVector2^ item);

			property FbxVector2^  default[int]
			{
				FbxVector2^ get(int index);
			}	
		};	





	}
}