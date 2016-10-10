#pragma once
#include "stdafx.h"
#include "FbxNurb.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxNurb,KFbxNurb);

		void FbxNurb::Reset()
		{
			_Ref()->Reset();
		}
		FbxGeometry::SurfaceMode FbxNurb::Surface_Mode::get()
		{
			return (FbxGeometry::SurfaceMode)_Ref()->GetSurfaceMode();
		}
		void FbxNurb::Surface_Mode::set(FbxGeometry::SurfaceMode value)
		{
			_Ref()->SetSurfaceMode((KFbxGeometry::ESurfaceMode)value);
		}

		void FbxNurb::InitControlPoints(int uCount, FbxNurb::NurbType uType, int vCount, FbxNurb::NurbType vType)
		{
			_Ref()->InitControlPoints(uCount, (KFbxNurb::ENurbType)uType,vCount, (KFbxNurb::ENurbType)vType);
		}

		int FbxNurb::UCount::get()
		{
			return _Ref()->GetUCount();
		}
		int FbxNurb::VCount::get()
		{
			return _Ref()->GetVCount();
		}

		FbxNurb::NurbType FbxNurb::NurbUType::get()
		{
			return (FbxNurb::NurbType)_Ref()->GetNurbUType();
		}
		FbxNurb::NurbType FbxNurb::NurbVType::get()
		{
			return (FbxNurb::NurbType)_Ref()->GetNurbVType();
		}

		int FbxNurb::UKnotCount::get()
		{
			return _Ref()->GetUKnotCount();
		}
		IntPtr FbxNurb::UKnotVector::get()
		{
			return IntPtr(_Ref()->GetUKnotVector()); 
		}
		array<double>^ FbxNurb::UKnotVectorArray::get()
		{
			double* arr = _Ref()->GetUKnotVector();
			if(arr)
			{
				array<double>^ Arr = gcnew array<double>(_Ref()->GetUKnotCount());
				for(int i =0;i<Arr->Length;i++)
					Arr[i] = arr[i];
				return Arr;
			}
			return nullptr;
		}

		int FbxNurb::VKnotCount::get()
		{
			return _Ref()->GetVKnotCount();
		}
		IntPtr FbxNurb::VKnotVector::get()
		{
			return IntPtr(_Ref()->GetVKnotVector()); 
		}
		array<double>^ FbxNurb::VKnotVectorArray::get()
		{
			double* arr = _Ref()->GetVKnotVector();
			if(arr)
			{
				array<double>^ Arr = gcnew array<double>(_Ref()->GetVKnotCount());
				for(int i =0;i<Arr->Length;i++)
					Arr[i] = arr[i];
				return Arr;
			}
			return nullptr;
		}

		IntPtr FbxNurb::UMultiplicityVector::get()
		{
			return IntPtr(_Ref()->GetUMultiplicityVector()); 
		}
		array<int>^ FbxNurb::UMultiplicityVectorArray::get()
		{
			int* arr = _Ref()->GetUMultiplicityVector();
			if(arr)
			{
				array<int>^ Arr = gcnew array<int>(_Ref()->GetUCount());
				for(int i =0;i<Arr->Length;i++)
					Arr[i] = arr[i];
				return Arr;
			}
			return nullptr;
		}

		IntPtr FbxNurb::VMultiplicityVector::get()
		{
			return IntPtr(_Ref()->GetVMultiplicityVector()); 
		}
		array<int>^ FbxNurb::VMultiplicityVectorArray::get()
		{
			int* arr = _Ref()->GetVMultiplicityVector();
			if(arr)
			{
				array<int>^ Arr = gcnew array<int>(_Ref()->GetVCount());
				for(int i =0;i<Arr->Length;i++)
					Arr[i] = arr[i];
				return Arr;
			}
			return nullptr;
		}

		void FbxNurb::SetOrder(kUInt uOrder, kUInt vOrder)		
		{
			_Ref()->SetOrder(uOrder, vOrder);
		}

		int FbxNurb::UOrder::get()
		{
			return _Ref()->GetUOrder();
		}
		int FbxNurb::VOrder::get()
		{
			return _Ref()->GetVOrder();
		}

		void FbxNurb::SetStep(int uStep, int vStep)
		{
			_Ref()->SetStep(uStep,vStep);
		}
		int FbxNurb::UStep::get()
		{
			return _Ref()->GetUStep();
		}
		int FbxNurb::VStep::get()
		{
			return _Ref()->GetVStep();
		}
		int FbxNurb::USpanCount::get()
		{
			return _Ref()->GetUSpanCount();
		}
		int FbxNurb::VSpanCount::get()
		{
			return _Ref()->GetVSpanCount();
		}

		bool FbxNurb::ApplyFlipUV::get()
		{
			return _Ref()->GetApplyFlipUV();
		}
		void FbxNurb::ApplyFlipUV::set(bool value)
		{
			return _Ref()->SetApplyFlipUV(value);
		}

		bool FbxNurb::ApplyFlipLinks::get()
		{
			return _Ref()->GetApplyFlipLinks();
		}
		void FbxNurb::ApplyFlipLinks::set(bool value)
		{
			return _Ref()->SetApplyFlipLinks(value);
		}

		bool FbxNurb::ApplyFlip::get()
		{
			return _Ref()->GetApplyFlip();
		}		

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxNurb,KFbxNurb);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}