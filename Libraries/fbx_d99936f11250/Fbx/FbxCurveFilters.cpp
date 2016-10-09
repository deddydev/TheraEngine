#pragma once
#include "stdafx.h"
#include "FbxCurveFilters.h"
#include "FbxTime.h"
#include "FbxError.h"
#include "FbxCurve.h"
#include "FbxCurveNode.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxXMatrix.h"


namespace Skill
{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxCurveFilters,KFbxKFCurveFilters);	
		void FbxCurveFilters::CollectManagedMemory()
		{
			_KError = nullptr;
			FbxObjectManaged::CollectManagedMemory();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCurveFilters,GetStartTime(),FbxTime,StartTime);
		void FbxCurveFilters::StartTime::set(FbxTime^ value)
		{
			if(value)
			{
				_Ref()->SetStartTime(*value->_Ref());
			}
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCurveFilters,GetStopTime(),FbxTime,StopTime);
		void FbxCurveFilters::StopTime::set(FbxTime^ value)
		{
			if(value)
			{
				_Ref()->SetStopTime(*value->_Ref());
			}
		}

		int FbxCurveFilters::GetStartKey(FbxCurve^ curve)
		{
			return _Ref()->GetStartKey(*curve->_Ref());
		}

		int FbxCurveFilters::GetStopKey(FbxCurve^ curve)
		{
			return _Ref()->GetStopKey(*curve->_Ref());
		}
		bool FbxCurveFilters::NeedApply(FbxCurveNode^ curveNode, bool recursive)
		{
			return _Ref()->NeedApply(*curveNode->_Ref(),recursive);
		}
		bool FbxCurveFilters::NeedApply(FbxCurve^ curve)		
		{
			return _Ref()->NeedApply(*curve->_Ref());
		}
		bool FbxCurveFilters::Apply(FbxCurveNode^ curveNode, bool recursive)
		{
			return _Ref()->Apply(*curveNode->_Ref(),recursive);
		}
		bool FbxCurveFilters::Apply(FbxCurve^ curve)
		{
			return _Ref()->Apply(*curve->_Ref());
		}
		void FbxCurveFilters::Reset()
		{
			_Ref()->Reset();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxCurveFilters,GetError(),FbxErrorManaged,KError);
		int FbxCurveFilters::LastErrorID::get()
		{
			return _Ref()->GetLastErrorID();
		}
		String^ FbxCurveFilters::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}



		FBXOBJECT_DEFINITION(FbxCurveFilterConstantKeyReducer,KFbxKFCurveFilterConstantKeyReducer);
		double FbxCurveFilterConstantKeyReducer::DerivativeTolerance::get()
		{
			return _Ref()->GetDerivativeTolerance();
		}
		void FbxCurveFilterConstantKeyReducer::DerivativeTolerance::set(double value)
		{
			_Ref()->SetDerivativeTolerance(value);
		}

		double FbxCurveFilterConstantKeyReducer::ValueTolerance::get()
		{
			return _Ref()->GetValueTolerance();
		}
		void FbxCurveFilterConstantKeyReducer::ValueTolerance::set(double value)
		{
			_Ref()->SetValueTolerance(value);
		}


		bool FbxCurveFilterConstantKeyReducer::KeepFirstAndLastKeys::get()
		{
			return _Ref()->GetKeepFirstAndLastKeys();
		}
		void FbxCurveFilterConstantKeyReducer::KeepFirstAndLastKeys::set(bool value)
		{
			_Ref()->SetKeepFirstAndLastKeys(value);
		}

		bool FbxCurveFilterConstantKeyReducer::KeepOneKey::get()
		{
			return _Ref()->GetKeepOneKey();
		}
		void FbxCurveFilterConstantKeyReducer::KeepOneKey::set(bool value)
		{
			_Ref()->SetKeepOneKey(value);
		}		

#ifndef DOXYGEN_SHOULD_SKIP_THIS		
		void FbxCurveFilterConstantKeyReducer::SetTranslationThreshold ( double translationThreshold )
		{
			_Ref()->SetTranslationThreshold(translationThreshold);
		}
		void FbxCurveFilterConstantKeyReducer::SetRotationThreshold    ( double rotationThreshold )
		{
			_Ref()->SetRotationThreshold(rotationThreshold );
		}
		void FbxCurveFilterConstantKeyReducer::SetScalingThreshold     ( double scalingThreshold )
		{
			_Ref()->SetScalingThreshold(scalingThreshold);
		}
		void FbxCurveFilterConstantKeyReducer::SetDefaultThreshold     ( double defaultThreshold )
		{
			_Ref()->SetDefaultThreshold(defaultThreshold);
		}

#endif




		FBXOBJECT_DEFINITION(FbxCurveFilterMatrixConverter,KFbxKFCurveFilterMatrixConverter);

		void FbxCurveFilterMatrixConverter::GetSourceMatrix(MatrixID index, FbxXMatrix^ matrix)
		{
			_Ref()->GetSourceMatrix((KFbxKFCurveFilterMatrixConverter::EMatrixID)index,*matrix->_Ref());
		}
		void FbxCurveFilterMatrixConverter::SetSourceMatrix(MatrixID index, FbxXMatrix^ matrix)
		{
			_Ref()->SetSourceMatrix((KFbxKFCurveFilterMatrixConverter::EMatrixID)index,*matrix->_Ref());
		}
		void FbxCurveFilterMatrixConverter::GetDestMatrix(MatrixID index, FbxXMatrix^ matrix)
		{
			_Ref()->GetDestMatrix((KFbxKFCurveFilterMatrixConverter::EMatrixID)index,*matrix->_Ref());
		}
		void FbxCurveFilterMatrixConverter::SetDestMatrix(MatrixID index, FbxXMatrix^ matrix)
		{
			_Ref()->SetDestMatrix((KFbxKFCurveFilterMatrixConverter::EMatrixID)index,*matrix->_Ref());
		}
		FbxTime^ FbxCurveFilterMatrixConverter::ResamplingPeriod::get()
		{
			return gcnew FbxTime(_Ref()->GetResamplingPeriod());
		}
		void FbxCurveFilterMatrixConverter::ResamplingPeriod::set(FbxTime^ value)
		{
			_Ref()->SetResamplingPeriod(*value->_Ref());
		}

		bool FbxCurveFilterMatrixConverter::GenerateLastKeyExactlyAtEndTime::get()
		{
			return _Ref()->GetGenerateLastKeyExactlyAtEndTime();
		}
		void FbxCurveFilterMatrixConverter::GenerateLastKeyExactlyAtEndTime::set(bool value)
		{
			return _Ref()->SetGenerateLastKeyExactlyAtEndTime(value);
		}

		bool FbxCurveFilterMatrixConverter::ResamplingOnFrameRateMultiple::get()
		{
			return _Ref()->GetResamplingOnFrameRateMultiple();
		}
		void FbxCurveFilterMatrixConverter::ResamplingOnFrameRateMultiple::set(bool value)
		{
			return _Ref()->SetResamplingOnFrameRateMultiple(value);
		}

		bool FbxCurveFilterMatrixConverter::ApplyUnroll::get()
		{
			return _Ref()->GetApplyUnroll();
		}
		void FbxCurveFilterMatrixConverter::ApplyUnroll::set(bool value)
		{
			return _Ref()->SetApplyUnroll(value);
		}

		bool FbxCurveFilterMatrixConverter::ApplyConstantKeyReducer::get()
		{
			return _Ref()->GetApplyConstantKeyReducer();
		}
		void FbxCurveFilterMatrixConverter::ApplyConstantKeyReducer::set(bool value)
		{
			return _Ref()->SetApplyConstantKeyReducer(value);
		}


		bool FbxCurveFilterMatrixConverter::ResampleTranslation::get()
		{
			return _Ref()->GetResampleTranslation();
		}
		void FbxCurveFilterMatrixConverter::ResampleTranslation::set(bool value)
		{
			return _Ref()->SetResampleTranslation(value);
		}

		void FbxCurveFilterMatrixConverter::SetSrcRotateOrder(int order)
		{
			_Ref()->SetSrcRotateOrder(order);
		}
		void FbxCurveFilterMatrixConverter::SetDestRotateOrder(int order)
		{
			_Ref()->SetDestRotateOrder(order);
		}

		bool FbxCurveFilterMatrixConverter::ForceApply::get()
		{
			return _Ref()->GetForceApply();
		}
		void FbxCurveFilterMatrixConverter::ForceApply::set(bool value)
		{
			return _Ref()->SetForceApply(value);
		}





		FBXOBJECT_DEFINITION(FbxCurveFilterResample,KFbxKFCurveFilterResample);

		/*bool FbxCurveFilterResample::KeysOnFrame::get()
		{
		return _Ref()->GetKeysOnFrame();
		}
		void FbxCurveFilterResample::KeysOnFrame::set(bool value)
		{
		_Ref()->SetKeysOnFrame(value);
		}*/

		FbxTime^ FbxCurveFilterResample::PeriodTime::get()
		{
			return gcnew FbxTime(_Ref()->GetPeriodTime());
		}
		void FbxCurveFilterResample::PeriodTime::set(FbxTime^ value)
		{
			_Ref()->SetPeriodTime(*value->_Ref());
		}

		bool FbxCurveFilterResample::IntelligentMode::get()
		{
			return _Ref()->GetIntelligentMode();
		}
		void FbxCurveFilterResample::IntelligentMode::set(bool value)
		{
			_Ref()->SetIntelligentMode(value);
		}	



		FBXOBJECT_DEFINITION(FbxCurveFilterUnroll,KFbxKFCurveFilterUnroll);


		double FbxCurveFilterUnroll::QualityTolerance::get()
		{
			return _Ref()->GetQualityTolerance();
		}
		void FbxCurveFilterUnroll::QualityTolerance::set(double value)
		{
			_Ref()->SetQualityTolerance(value);
		}		


		bool FbxCurveFilterUnroll::TestForPath::get()
		{
			return _Ref()->GetTestForPath();
		}
		void FbxCurveFilterUnroll::TestForPath::set(bool value)
		{
			_Ref()->SetTestForPath(value);
		}
	}
}