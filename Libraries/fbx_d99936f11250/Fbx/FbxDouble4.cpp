#pragma once
#include "stdafx.h"
#include "FbxDouble4.h"


{
	namespace FbxSDK
	{
		FbxDouble4::FbxDouble4(double x,double y,double z,double w)
		{
			_SetPointer(new fbxDouble4(x,y,z,w),true);			
		}
		void FbxDouble4::CollectManagedMemory()
		{
		}

		double FbxDouble4::X::get()	{return (*_FbxDouble4)[0];}
		void FbxDouble4::X::set(double value){(*_FbxDouble4)[0] = value;}

		double FbxDouble4::Y::get()	{return (*_FbxDouble4)[1];}
		void FbxDouble4::Y::set(double value){(*_FbxDouble4)[1] = value;}

		double FbxDouble4::Z::get()	{return (*_FbxDouble4)[2];}
		void FbxDouble4::Z::set(double value){(*_FbxDouble4)[2] = value;}		

		double FbxDouble4::W::get()	{return (*_FbxDouble4)[3];}
		void FbxDouble4::W::set(double value){(*_FbxDouble4)[3] = value;}
	}
}