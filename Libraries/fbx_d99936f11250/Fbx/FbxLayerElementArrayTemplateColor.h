#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"

namespace Skill
{
	namespace FbxSDK
	{			
		ref class FbxColor;		
				
		public ref class FbxLayerElementArrayTemplateColor : public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<KFbxColor>);
		internal:			
			FbxLayerElementArrayTemplateColor(KFbxLayerElementArrayTemplate<KFbxColor>* instance);
		public:				
			FbxLayerElementArrayTemplateColor(FbxType dataType);
			int Add(FbxColor^ item);
			int InsertAt(int index,FbxColor^ item);
			void SetAt(int index,FbxColor^ item);
			void SetLast(FbxColor^ item);			
			FbxColor^ RemoveAt(int index);
			FbxColor^ RemoveLast();
			bool RemoveIt(FbxColor^ item);

			FbxColor^ GetAt(int index);
			FbxColor^ GetFirst();
			FbxColor^ GetLast();

			int Find(FbxColor^ item);
			int FindAfter(int afterIndex, FbxColor^ item);
			int FindBefore(int beforeIndex, FbxColor^ item);

			property FbxColor^  default[int]
			{
				FbxColor^ get(int index);
			}	
		};	

	}
}