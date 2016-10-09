#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxTakeInfo.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Arrays
		{
			public ref class FbxLayerInfoRefArray : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxLayerInfoRefArray,KArrayTemplate<KLayerInfo*>);
				INATIVEPOINTER_DECLARE(FbxLayerInfoRefArray,KArrayTemplate<KLayerInfo*>);			
			};
		}
	}
}