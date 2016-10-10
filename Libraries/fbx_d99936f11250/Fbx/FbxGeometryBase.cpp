#pragma once
#include "stdafx.h"
#include "FbxGeometryBase.h"
#include "FbxVector4.h"
#include "FbxStream.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxTypedProperty.h"
#include "FbxLayerElementArrayTemplateVector4.h"
#include "FbxLayerElementArrayTemplateInt32.h"


#define FbxGeometryBase_PROPDOUBLE3_GET_DEFINE(Name)FbxDouble3TypedProperty^ FbxGeometryBase::Name::get(){\
	if(!_##Name) _##Name = gcnew FbxDouble3TypedProperty(&_Ref()->Name);\
	else _##Name->pro = &_Ref()->Name ;return _##Name;}\


{
	namespace FbxSDK
	{

		FBXOBJECT_DEFINITION(FbxGeometryBase,KFbxGeometryBase);
		void FbxGeometryBase::CollectManagedMemory()
		{
			_BBoxMin = nullptr;
			_BBoxMax = nullptr;
			_Normals = nullptr;
			_NormalsIndices = nullptr;
			FbxLayerContainer::CollectManagedMemory();
		}				
		void FbxGeometryBase::InitControlPoints(int count)
		{
			_Ref()->InitControlPoints(count);
		}
		void FbxGeometryBase::InitNormals(int count )
		{
			_Ref()->InitNormals(count);
		}
		void FbxGeometryBase::InitNormals(FbxGeometryBase^ src)
		{
			_Ref()->InitNormals(src->_Ref());
		}
		void FbxGeometryBase::SetControlPointAt(FbxVector4^ ctrlPoint , FbxVector4^ normal , int index, bool pI2DSearch)
		{
			_Ref()->SetControlPointAt(*ctrlPoint->_Ref() , *normal->_Ref() ,index, pI2DSearch);
		}
		void FbxGeometryBase::SetControlPointAt(FbxVector4^ ctrlPoint , int index)
		{
			_Ref()->SetControlPointAt(*ctrlPoint->_Ref() ,index);
		}
		void FbxGeometryBase::SetControlPointNormalAt(FbxVector4^ ctrlPoint, int index, bool pI2DSearch)
		{
			_Ref()->SetControlPointNormalAt(*ctrlPoint->_Ref(),index,pI2DSearch);
		}

		int FbxGeometryBase::ControlPointsCount::get()
		{
			return _Ref()->GetControlPointsCount();
		}

		array<FbxVector4^>^ FbxGeometryBase::ControlPoints::get()
		{			
			array<FbxVector4^>^ arr = gcnew array<FbxVector4^>(_Ref()->GetControlPointsCount());
			KFbxVector4* cps = _Ref()->GetControlPoints();
			for(int i=0;i<arr->Length;i++)
				arr[i] = gcnew FbxVector4(cps[i]);
			return arr;
		}
		void FbxGeometryBase::ControlPoints::set(array<FbxVector4^>^ value)
		{
			if(_Ref()->GetControlPointsCount() != value->Length)
				throw gcnew System::ArgumentException(" size of given array missmatch by size of internal array");
			KFbxVector4* cps = _Ref()->GetControlPoints();
			for(int i=0;i<value->Length;i++)
				cps[i] = *value[i]->_Ref();			
		}

		FbxVector4^ FbxGeometryBase::GetControlPointAt(int index)
		{
			KFbxVector4* cps = _Ref()->GetControlPoints();			
			return gcnew FbxVector4(cps[index]);
		}
		void FbxGeometryBase::SetControlPointAt(int index ,FbxVector4^ point)
		{
			KFbxVector4* cps = _Ref()->GetControlPoints();			
			cps[index] = *point->_Ref();
		}		
		/*FbxVector4^ FbxGeometryBase::GetNormals()
		{
			return gcnew FbxVector4(_Ref()->GetNormals());
		}*/
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGeometryBase,BBoxMin,FbxDouble3TypedProperty,BBoxMin);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGeometryBase,BBoxMax,FbxDouble3TypedProperty,BBoxMax);		
		void FbxGeometryBase::ComputeBBox()
		{
			_Ref()->ComputeBBox();			
		}		
		int FbxGeometryBase::MemoryUsage::get()
		{
			return _Ref()->MemoryUsage();
		}			
#ifndef DOXYGEN_SHOULD_SKIP_THIS

		FbxLayerElementArrayTemplateVector4^ FbxGeometryBase::Normals::get()
		{
			KFbxLayerElementArrayTemplate<KFbxVector4>* normals;
			bool b = _Ref()->GetNormals(&normals);
			if(b)
			{
				if(_Normals)
					_Normals->_SetPointer(normals,false);
				else
					_Normals = gcnew FbxLayerElementArrayTemplateVector4(normals);
				return _Normals;
			}
			return nullptr;
		}
		FbxLayerElementArrayTemplateInt32^ FbxGeometryBase::NormalsIndices::get()
		{
			KFbxLayerElementArrayTemplate<int>* normalsIndices;
			bool b = _Ref()->GetNormalsIndices(&normalsIndices);
			if(b)
			{
				if(_NormalsIndices)
					_NormalsIndices->_SetPointer(normalsIndices,false);
				else
					_NormalsIndices = gcnew FbxLayerElementArrayTemplateInt32(normalsIndices);
				return _NormalsIndices;
			}
			return nullptr;
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}