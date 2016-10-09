#pragma once
#include "stdafx.h"
#include "FbxEvaluationInfo.h"
#include "FbxTime.h"
#include "FbxSdkManager.h"

namespace Skill
{
	namespace FbxSDK
	{

		void FbxEvaluationInfo::CollectManagedMemory()
		{
		}

		/*FbxEvaluationInfo^ FbxEvaluationInfo::Create(FbxSdkManager^ manager)		
		{
			return gcnew FbxEvaluationInfo(KFbxEvaluationInfo::Create(manager->_Ref()));
		}
		void FbxEvaluationInfo::Destroy()
		{
			_Ref()->Destroy();
		}
		FbxTime^ FbxEvaluationInfo::Time::get()
		{
			return gcnew FbxTime(_Ref()->GetTime());
		}
		void FbxEvaluationInfo::Time::set(FbxTime^ value)
		{
			_Ref()->SetTime(*value->_Ref());
		}
		kFbxEvaluationId FbxEvaluationInfo::EvaluationId::get()
		{
			return _Ref()->GetEvaluationId();
		}
		
		void FbxEvaluationInfo::UpdateEvaluationId()
		{
			_Ref()->UpdateEvaluationId();
		}*/
	}
}