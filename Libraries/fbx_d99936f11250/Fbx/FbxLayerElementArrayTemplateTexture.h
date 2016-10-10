#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"




{
	namespace FbxSDK
	{	
		ref class FbxTexture;		

		public ref class FbxLayerElementArrayTemplateTexture : public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<KFbxTexture*>);
		internal:			
			FbxLayerElementArrayTemplateTexture(KFbxLayerElementArrayTemplate<KFbxTexture*>* instance);
		public:				
			FbxLayerElementArrayTemplateTexture(FbxType dataType);
			int Add(FbxTexture^ item);
			int InsertAt(int index,FbxTexture^ item);
			void SetAt(int index,FbxTexture^ item);
			void SetLast(FbxTexture^ item);			
			FbxTexture^ RemoveAt(int index);
			FbxTexture^ RemoveLast();
			bool RemoveIt(FbxTexture^ item);

			FbxTexture^ GetAt(int index);
			FbxTexture^ GetFirst();
			FbxTexture^ GetLast();

			int Find(FbxTexture^ item);
			int FindAfter(int afterIndex, FbxTexture^ item);
			int FindBefore(int beforeIndex, FbxTexture^ item);

			property FbxTexture^  default[int]
			{
				FbxTexture^ get(int index);
			}	
		};	


	}
}