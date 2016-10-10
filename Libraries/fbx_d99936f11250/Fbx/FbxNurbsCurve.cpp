#pragma once
#include "stdafx.h"
#include "FbxNurbsCurve.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxNurbsCurve,KFbxNurbsCurve);

		void FbxNurbsCurve::InitControlPoints(int count, FbxNurbsCurve::Type vType)
		{
			_Ref()->InitControlPoints(count,(KFbxNurbsCurve::EType)vType);
		}
		IntPtr FbxNurbsCurve::KnotVector::get()
		{
			return IntPtr(_Ref()->GetKnotVector());
		}
		int FbxNurbsCurve::KnotCount::get()
		{
			return _Ref()->GetKnotCount();
		}
		int FbxNurbsCurve::Order::get()
		{
			return _Ref()->GetOrder();
		}
		FbxNurbsCurve::Dimension FbxNurbsCurve::DimensionType::get()
		{
			return (FbxNurbsCurve::Dimension)_Ref()->GetDimension();
		}
		void FbxNurbsCurve::DimensionType::set(FbxNurbsCurve::Dimension value)
		{
			_Ref()->SetDimension((KFbxNurbsCurve::EDimension)value);
		}
		bool FbxNurbsCurve::IsRational::get()
		{
			return _Ref()->IsRational();
		}

		int FbxNurbsCurve::SpanCount::get()
		{
			return _Ref()->GetSpanCount();
		}
		FbxNurbsCurve::Type FbxNurbsCurve::NurbsCurveType::get()
		{
			return (FbxNurbsCurve::Type)_Ref()->GetType();
		}
		/*bool FbxNurbsCurve::IsPolyline::get()
		{
			return _Ref()->IsPolyline();
		}*/
		bool FbxNurbsCurve::IsBezier::get()
		{
			return _Ref()->IsBezier();
		}

		CLONE_DEFINITION(FbxNurbsCurve,KFbxNurbsCurve);

		bool FbxNurbsCurve::FullMultiplicity()
		{
			return _Ref()->FullMultiplicity();
		}
	}
}