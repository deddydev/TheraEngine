#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNativePointer.h"


{
	namespace FbxSDK
	{
		public ref class FbxDouble4 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxDouble4,fbxDouble4);
			INATIVEPOINTER_DECLARE(FbxDouble4,fbxDouble4);
		internal:
			FbxDouble4(fbxDouble4 v)
			{
				_SetPointer(new fbxDouble4(),true);
				*_FbxDouble4 = v;
			}
		public:
			DEFAULT_CONSTRUCTOR(FbxDouble4,fbxDouble4);
			FbxDouble4(double x,double y,double z,double w);

			VALUE_PROPERTY_GETSET_DECLARE(double,X);
			VALUE_PROPERTY_GETSET_DECLARE(double,Y);
			VALUE_PROPERTY_GETSET_DECLARE(double,Z);
			VALUE_PROPERTY_GETSET_DECLARE(double,W);

			virtual String^ ToString() override
			{
				return String::Format("X: {0}, Y: {1}, Z: {2},W: {3}",X,Y,Z,W);				
			}

		};		
	}
}