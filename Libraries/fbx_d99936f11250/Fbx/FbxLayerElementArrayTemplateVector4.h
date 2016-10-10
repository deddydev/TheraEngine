#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"


{
	namespace FbxSDK
	{			
		ref class FbxVector4;		
				
		public ref class FbxLayerElementArrayTemplateVector4 : public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<KFbxVector4>);
		internal:			
			FbxLayerElementArrayTemplateVector4(KFbxLayerElementArrayTemplate<KFbxVector4>* instance);
		public:				
			FbxLayerElementArrayTemplateVector4(FbxType dataType);
			int Add(FbxVector4^ item);
			int InsertAt(int index,FbxVector4^ item);
			void SetAt(int index,FbxVector4^ item);
			void SetLast(FbxVector4^ item);			
			FbxVector4^ RemoveAt(int index);
			FbxVector4^ RemoveLast();
			bool RemoveIt(FbxVector4^ item);

			FbxVector4^ GetAt(int index);
			FbxVector4^ GetFirst();
			FbxVector4^ GetLast();

			int Find(FbxVector4^ item);
			int FindAfter(int afterIndex, FbxVector4^ item);
			int FindBefore(int beforeIndex, FbxVector4^ item);

			property FbxVector4^  default[int]
			{
				FbxVector4^ get(int index);
			}	
		};	




	}
}