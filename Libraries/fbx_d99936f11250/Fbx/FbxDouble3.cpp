#pragma once
#include "stdafx.h"
#include "FbxDouble3.h"


{
	namespace FbxSDK
	{
		FbxDouble3::FbxDouble3(double x,double y,double z)
		{
			_SetPointer(new fbxDouble3(x,y,z),true);			
		}
		void FbxDouble3::CollectManagedMemory()
		{
		}

		double FbxDouble3::X::get()	{return (*_FbxDouble3)[0];}
		void FbxDouble3::X::set(double value){(*_FbxDouble3)[0] = value;}

		double FbxDouble3::Y::get()	{return (*_FbxDouble3)[1];}
		void FbxDouble3::Y::set(double value){(*_FbxDouble3)[1] = value;}

		double FbxDouble3::Z::get()	{return (*_FbxDouble3)[2];}
		void FbxDouble3::Z::set(double value){(*_FbxDouble3)[2] = value;}		
	}
}