#pragma once
#include "stdafx.h"
#include "FbxNurbsSurface.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxNode.h"


namespace Skill
{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxNurbsSurface,KFbxNurbsSurface);


		void FbxNurbsSurface::Reset()
		{
			_Ref()->Reset();
		}

		FbxGeometry::SurfaceMode FbxNurbsSurface::Surface_Mode::get()
		{
			return (FbxGeometry::SurfaceMode)_Ref()->GetSurfaceMode();
		}
		void FbxNurbsSurface::Surface_Mode::set(FbxGeometry::SurfaceMode value)
		{
			_Ref()->SetSurfaceMode((KFbxGeometry::ESurfaceMode)value);
		}

		void FbxNurbsSurface::InitControlPoints(int uCount, NurbType uType, int vCount, NurbType vType)
		{
			_Ref()->InitControlPoints(uCount,(KFbxNurbsSurface::ENurbType)uType,vCount,(KFbxNurbsSurface::ENurbType)vType);
		}

		int FbxNurbsSurface::UCount::get()
		{
			return _Ref()->GetUCount();
		}
		int FbxNurbsSurface::VCount::get()
		{
			return _Ref()->GetVCount();
		}
		FbxNurbsSurface::NurbType FbxNurbsSurface::NurbUType::get()
		{
			return (FbxNurbsSurface::NurbType)_Ref()->GetNurbUType();
		}
		FbxNurbsSurface::NurbType FbxNurbsSurface::NurbVType::get()
		{
			return (FbxNurbsSurface::NurbType)_Ref()->GetNurbVType();
		}
		int FbxNurbsSurface::UKnotCount::get()
		{
			return _Ref()->GetUKnotCount();
		}
		
		IntPtr FbxNurbsSurface::UKnotVector::get()
		{
			return IntPtr(_Ref()->GetUKnotVector());
		}
		int FbxNurbsSurface::VKnotCount::get()
		{
			return _Ref()->GetVKnotCount();
		}
		IntPtr FbxNurbsSurface::VKnotVector::get()
		{
			return IntPtr(_Ref()->GetVKnotVector());
		}

		void FbxNurbsSurface::SetOrder(kUInt uOrder, kUInt vOrder)
		{
			_Ref()->SetOrder(uOrder,vOrder);
		}
		int FbxNurbsSurface::UOrder::get()
		{
			return _Ref()->GetUOrder();
		}
		int FbxNurbsSurface::VOrder::get()
		{
			return _Ref()->GetVOrder();
		}

		void FbxNurbsSurface::SetStep(int uStep, int vStep)
		{
			_Ref()->SetStep(uStep,vStep);
		}
		int FbxNurbsSurface::UStep::get()
		{
			return _Ref()->GetUStep();
		}
		int FbxNurbsSurface::VStep::get()
		{
			return _Ref()->GetVStep();
		}
		int FbxNurbsSurface::USpanCount::get()
		{
			return _Ref()->GetUSpanCount();
		}
		int FbxNurbsSurface::VSpanCount::get()
		{
			return _Ref()->GetVSpanCount();
		}

		bool FbxNurbsSurface::ApplyFlipUV::get()
		{
			return _Ref()->GetApplyFlipUV();
		}
		void FbxNurbsSurface::ApplyFlipUV::set(bool value)
		{
			_Ref()->SetApplyFlipUV(value);
		}

		bool FbxNurbsSurface::ApplyFlipLinks::get()
		{
			return _Ref()->GetApplyFlipLinks();
		}
		void FbxNurbsSurface::ApplyFlipLinks::set(bool value)
		{
			_Ref()->SetApplyFlipLinks(value);
		}
		
		bool FbxNurbsSurface::ApplyFlip::get()
		{
			return _Ref()->GetApplyFlip();
		}

		void FbxNurbsSurface::AddCurveOnSurface(FbxNode^ curve )
		{
			_Ref()->AddCurveOnSurface(curve->_Ref());
		}
		FbxNode^ FbxNurbsSurface::GetCurveOnSurface( int index )
		{
			return gcnew FbxNode(_Ref()->GetCurveOnSurface(index));
		}

		int FbxNurbsSurface::CurveOnSurfaceCount::get()
		{
			return _Ref()->GetCurveOnSurfaceCount();
		}
		bool FbxNurbsSurface::RemoveCurveOnSurface(FbxNode^ curve)
		{
			return _Ref()->RemoveCurveOnSurface(curve->_Ref());
		}
		bool FbxNurbsSurface::IsRational::get()
		{
			return _Ref()->IsRational();
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		bool FbxNurbsSurface::FlipNormals::get()
		{
			return _Ref()->GetFlipNormals();
		}
		void FbxNurbsSurface::FlipNormals::set(bool value)
		{
			_Ref()->SetFlipNormals(value);
		}

		CLONE_DEFINITION(FbxNurbsSurface,KFbxNurbsSurface);		
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}