#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLayerElementArray.h"

namespace Skill
{
	namespace FbxSDK
	{				
		ref class FbxSurfaceMaterial;	

		public ref class FbxLayerElementArrayTemplateSurfaceMaterial : public FbxLayerElementArray
		{
			REF_DECLARE(FbxLayerElementArray,KFbxLayerElementArrayTemplate<KFbxSurfaceMaterial*>);
		internal:			
			FbxLayerElementArrayTemplateSurfaceMaterial(KFbxLayerElementArrayTemplate<KFbxSurfaceMaterial*>* instance);
		public:				
			FbxLayerElementArrayTemplateSurfaceMaterial(FbxType dataType);
			int Add(FbxSurfaceMaterial^ item);
			int InsertAt(int index,FbxSurfaceMaterial^ item);
			void SetAt(int index,FbxSurfaceMaterial^ item);
			void SetLast(FbxSurfaceMaterial^ item);			
			FbxSurfaceMaterial^ RemoveAt(int index);
			FbxSurfaceMaterial^ RemoveLast();
			bool RemoveIt(FbxSurfaceMaterial^ item);

			FbxSurfaceMaterial^ GetAt(int index);
			FbxSurfaceMaterial^ GetFirst();
			FbxSurfaceMaterial^ GetLast();

			int Find(FbxSurfaceMaterial^ item);
			int FindAfter(int afterIndex, FbxSurfaceMaterial^ item);
			int FindBefore(int beforeIndex, FbxSurfaceMaterial^ item);

			property FbxSurfaceMaterial^  default[int]
			{
				FbxSurfaceMaterial^ get(int index);
			}	
		};	


	}
}