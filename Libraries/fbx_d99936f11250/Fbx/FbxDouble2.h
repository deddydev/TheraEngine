#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNativePointer.h"


{
	namespace FbxSDK
	{
		public ref class FbxDouble2 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxDouble2,fbxDouble2);
			INATIVEPOINTER_DECLARE(FbxDouble2,fbxDouble2);
		internal:
			FbxDouble2(fbxDouble2 v)
			{
				_SetPointer(new fbxDouble2(),true);
				*_FbxDouble2 = v;
			}
		public:
			DEFAULT_CONSTRUCTOR(FbxDouble2,fbxDouble2);
			FbxDouble2(double x,double y);

			VALUE_PROPERTY_GETSET_DECLARE(double,X);
			VALUE_PROPERTY_GETSET_DECLARE(double,Y);			

		};		
	}
}