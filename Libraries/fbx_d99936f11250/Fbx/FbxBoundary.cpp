#pragma once
#include "stdafx.h"
#include "FbxBoundary.h"
#include "FbxGeometry.h"
#include "FbxShape.h"
#include "FbxNurbsCurve.h"
#include "FbxVector4.h"
#include "FbxNurbsSurface.h"
#include "FbxCurve.h"
#include "FbxString.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{	
		void FbxBoundary::CollectManagedMemory()
		{
			if(_list)
				_list->Clear();
			_list = nullptr;
			FbxGeometry::CollectManagedMemory();
		}
		FBXOBJECT_DEFINITION(FbxBoundary,KFbxBoundary);

		FbxBoundary::FbxBoundary(KFbxBoundary* instance) : FbxGeometry(instance)
		{
			_Free = false;
			_list = gcnew System::Collections::Generic::List<FbxNurbsCurve^>();
			for(int i = 0;i< _Ref()->GetCurveCount();i++)
			{
				KFbxNurbsCurve* k = _Ref()->GetCurve(i);
				if(k)
					_list->Add(gcnew FbxNurbsCurve(k));
				else
					_list->Add(nullptr);
			}
		}								
		void FbxBoundary::AddCurve(FbxNurbsCurve^ curve )
		{
			_Ref()->AddCurve(curve->_Ref());
			_list->Add(curve);
		}
		int FbxBoundary::CurveCount::get()
		{
			return _Ref()->GetCurveCount();
		}	
		FbxNurbsCurve^ FbxBoundary::GetCurve(int index)
		{
			if(!_list[index])
			{
				KFbxNurbsCurve* k = _Ref()->GetCurve(index);	
				if(k)
					_list[index] = gcnew FbxNurbsCurve(k);				
			}
			return _list[index];
		}						
		bool FbxBoundary::IsPointInControlHull( FbxVector4^ point )
		{
			return _Ref()->IsPointInControlHull(*point->_Ref());
		}

		FbxVector4^ FbxBoundary::ComputePointInBoundary()			
		{
			return gcnew FbxVector4(_Ref()->ComputePointInBoundary());
		}
		CLONE_DEFINITION(FbxBoundary,KFbxBoundary);

		void FbxBoundary::ClearCurves()
		{
			_Ref()->ClearCurves();
		}

		void FbxBoundary::CopyCurves(FbxBoundary^ other )
		{
			_Ref()->CopyCurves(*other->_Ref());
			_list->Clear();
			for(int i=0;i<other->_list->Count;i++)
				_list->Add(other->_list[i]);
		}

		bool FbxBoundary::IsValid::get()
		{
			return _Ref()->IsValid();
		}

		bool FbxBoundary::IsCounterClockwise::get()
		{
			return _Ref()->IsCounterClockwise();
		}


		FBXOBJECT_DEFINITION(FbxTrimNurbsSurface,KFbxTrimNurbsSurface);

		void FbxTrimNurbsSurface::CollectManagedMemory()
		{
			_NurbsSurface = nullptr;
			FbxGeometry::CollectManagedMemory();
		}

		int FbxTrimNurbsSurface::TrimRegionCount::get()
		{
			return _Ref()->GetTrimRegionCount();
		}
		void FbxTrimNurbsSurface::BeginTrimRegion()
		{
			_Ref()->BeginTrimRegion();
		}			
		void FbxTrimNurbsSurface::EndTrimRegion()
		{
			_Ref()->EndTrimRegion();
		}			
		bool FbxTrimNurbsSurface::AddBoundary(FbxBoundary^ boundary)
		{
			return _Ref()->AddBoundary(boundary->_Ref());
		}			
		FbxBoundary^  FbxTrimNurbsSurface::GetBoundary(int index, int regionIndex )
		{
			KFbxBoundary* k = _Ref()->GetBoundary(index,regionIndex);
			if(k)
				return gcnew FbxBoundary(k);
			return nullptr;
		}					
		int FbxTrimNurbsSurface::GetBoundaryCount(int regionIndex)
		{
			return _Ref()->GetBoundaryCount(regionIndex);
		}
		int FbxTrimNurbsSurface::GetBoundaryCount()
		{
			return _Ref()->GetBoundaryCount();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxTrimNurbsSurface,KFbxNurbsSurface,GetNurbsSurface(),FbxNurbsSurface,NurbsSurface);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxTrimNurbsSurface,SetNurbsSurface,FbxNurbsSurface,NurbsSurface);

		bool FbxTrimNurbsSurface::FlipNormals::get()
		{
			return _Ref()->GetFlipNormals(); 
		}
		void FbxTrimNurbsSurface::FlipNormals::set(bool value)
		{
			_Ref()->SetFlipNormals(value); 
		}																					

		FbxCurve^ FbxTrimNurbsSurface::GetShapeChannel(int index)
		{							
			KFCurve* k = _Ref()->GetShapeChannel(index);
			if(k)
				return gcnew FbxCurve(k);
			return nullptr;
		}									
		void FbxTrimNurbsSurface::SetControlPointAt(FbxVector4^ ctrlPoint, FbxVector4^ normal , int index)
		{
			_Ref()->SetControlPointAt(*ctrlPoint->_Ref(),*normal->_Ref() , index);
		}
		CLONE_DEFINITION(FbxTrimNurbsSurface,KFbxTrimNurbsSurface);

		bool FbxTrimNurbsSurface::IsValid::get()
		{
			if(FbxGeometry::IsValid)
				return _Ref()->IsValid();
			return false;
		}														
	}
}