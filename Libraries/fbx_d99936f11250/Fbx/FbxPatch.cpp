#pragma once
#include "stdafx.h"
#include "FbxPatch.h"
#include "FbxStream.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"

namespace Skill
{
	namespace FbxSDK
	{

		FBXOBJECT_DEFINITION(FbxPatch,KFbxPatch);		

		void FbxPatch::Reset()
		{
			_Ref()->Reset();
		}

		void FbxPatch::Surface_Mode::set(FbxGeometry::SurfaceMode value)
		{
			_Ref()->SetSurfaceMode((KFbxGeometry::ESurfaceMode)value);
		}

		FbxGeometry::SurfaceMode FbxPatch::Surface_Mode::get()
		{
			return (FbxGeometry::SurfaceMode) _Ref()->GetSurfaceMode();
		}

		void FbxPatch::InitControlPoints(int UCount, PatchType UType, int VCount, PatchType VType)
		{
			_Ref()->InitControlPoints(UCount, (KFbxPatch::EPatchType) UType,VCount, (KFbxPatch::EPatchType) VType);
		}

		int FbxPatch::UCount::get()
		{
			return _Ref()->GetUCount();
		}

		int FbxPatch::VCount::get()
		{
			return _Ref()->GetVCount();
		}

		FbxPatch::PatchType FbxPatch::PatchUType::get()
		{
			return (FbxPatch::PatchType)_Ref()->GetPatchUType();
		}


		FbxPatch::PatchType FbxPatch::PatchVType::get()
		{
			return (FbxPatch::PatchType)_Ref()->GetPatchVType();
		}

		void FbxPatch::SetStep(int UStep, int VStep)
		{
			_Ref()->SetStep(UStep,VStep);
		}


		int FbxPatch::UStep::get()
		{
			return _Ref()->GetUStep();
		}

		int FbxPatch::VStep::get()
		{
			return _Ref()->GetVStep();
		}


		void FbxPatch::SetClosed(bool U, bool V)
		{
			_Ref()->SetClosed(U,V);
		}

		bool FbxPatch::UClosed::get()			
		{
			return _Ref()->GetUClosed();
		}

		bool FbxPatch::VClosed::get()
		{
			return _Ref()->GetVClosed();
		}


		void FbxPatch::SetUCapped(bool UBottom, bool UTop)
		{
			_Ref()->SetUCapped(UBottom,UTop);
		}

		bool FbxPatch::UCappedBottom::get() 
		{
			return _Ref()->GetUCappedBottom();
		}

		bool FbxPatch::UCappedTop::get()
		{
			return _Ref()->GetUCappedTop();
		}
		void FbxPatch::SetVCapped(bool VBottom, bool VTop)
		{
			_Ref()->SetUCapped(VBottom,VTop);
		}

		bool FbxPatch::VCappedBottom::get() 
		{
			return _Ref()->GetVCappedBottom();
		}
		bool FbxPatch::VCappedTop::get() 
		{
			return _Ref()->GetVCappedTop();
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS
		CLONE_DEFINITION(FbxPatch,KFbxPatch);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}