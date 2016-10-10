#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		public ref class FbxDouble1 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxDouble1,fbxDouble1);
			INATIVEPOINTER_DECLARE(FbxDouble1,fbxDouble1);
		public:
			DEFAULT_CONSTRUCTOR(FbxDouble1,fbxDouble1);
			FbxDouble1(double value);

			VALUE_PROPERTY_GETSET_DECLARE(double,Value);
		};		
	}
}