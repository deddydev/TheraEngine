#pragma once
#include "stdafx.h"
#include "Fbx.h"
#include "FbxNativePointer.h"

namespace Skill
{
	namespace FbxSDK
	{
		public ref class FbxDouble3 : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxDouble3,fbxDouble3);
			INATIVEPOINTER_DECLARE(FbxDouble3,fbxDouble3);
		internal:
			FbxDouble3(fbxDouble3 v)
			{
				_SetPointer(new fbxDouble3(),true);
				*_FbxDouble3 = v;
			}
		public:
			DEFAULT_CONSTRUCTOR(FbxDouble3,fbxDouble3);
			FbxDouble3(double x,double y,double z);

			VALUE_PROPERTY_GETSET_DECLARE(double,X);
			VALUE_PROPERTY_GETSET_DECLARE(double,Y);
			VALUE_PROPERTY_GETSET_DECLARE(double,Z);

			virtual String^ ToString() override
			{
				return String::Format("X: {0}, Y: {1}, Z: {2}",X,Y,Z);				
			}

		};		
	}
}