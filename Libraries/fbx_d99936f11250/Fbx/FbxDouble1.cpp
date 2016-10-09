#pragma once
#include "stdafx.h"
#include "FbxDouble1.h"

namespace Skill
{
	namespace FbxSDK
	{
		FbxDouble1::FbxDouble1(double value)
		{
			_SetPointer(new fbxDouble1(value),true);			
		}
		void FbxDouble1::CollectManagedMemory()
		{
		}
		double FbxDouble1::Value::get()	{return *_FbxDouble1;}
		void FbxDouble1::Value::set(double value){ *_FbxDouble1 = value;}		
	}
}