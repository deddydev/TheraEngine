#pragma once
#include "stdafx.h"
#include "FbxDouble2.h"


{
	namespace FbxSDK
	{
		FbxDouble2::FbxDouble2(double x,double y)
		{
			_SetPointer(new fbxDouble2(x,y),true);			
		}
		void FbxDouble2::CollectManagedMemory()
		{
		}

		double FbxDouble2::X::get()	{return (*_FbxDouble2)[0];}
		void FbxDouble2::X::set(double value){(*_FbxDouble2)[0] = value;}

		double FbxDouble2::Y::get()	{return (*_FbxDouble2)[1];}
		void FbxDouble2::Y::set(double value){(*_FbxDouble2)[1] = value;}		
	}
}