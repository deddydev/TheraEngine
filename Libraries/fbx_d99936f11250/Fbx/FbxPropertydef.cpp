#pragma once
#include "stdafx.h"
#include "FbxPropertyDef.h"

namespace Skill
{
	namespace FbxSDK
	{

		int FbxPropertyFlags::FlagCount::get()
		{
			return fbxsdk_200901::FbxPropertyFlags::GetFlagCount();
		}		

		FbxPropertyFlags::FbxPropertyFlagsType FbxPropertyFlags::AllFlags::get()
		{
			return (FbxPropertyFlags::FbxPropertyFlagsType)fbxsdk_200901::FbxPropertyFlags::AllFlags();
		}


		void FbxConnectionPointFilter::CollectManagedMemory()
		{
			_Reference = nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxConnectionPointFilter,KFbxConnectionPointFilter,Ref(),FbxConnectionPointFilter,Reference);

		void FbxConnectionPointFilter::Unref()
		{
			_Ref()->Unref();
		}
		kFbxFilterId FbxConnectionPointFilter::UniqueId::get()
		{
			return _Ref()->GetUniqueId();
		}
	}
}