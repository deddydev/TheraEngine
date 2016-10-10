#pragma once
#include "stdafx.h"
#include "FbxGeometry.h"
#include "FbxDeformer.h"
#include "FbxGeometryWeightedMap.h"
#include "FbxShape.h"
#include "FbxSkin.h"
#include "FbxCurve.h"
#include "FbxXMatrix.h"
#include "FbxString.h"
#include "FbxError.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"



{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxGeometry,KFbxGeometry);		

		void FbxGeometry::CollectManagedMemory()
		{
			_KError = nullptr;
			_SourceGeometryWeightedMap = nullptr;
			FbxGeometryBase::CollectManagedMemory();
		}		

		int FbxGeometry::AddDeformer(FbxDeformer^ deformer)
		{
			return _Ref()->AddDeformer(deformer->_Ref());
		}
		int FbxGeometry::DeformerCount::get()
		{
			return _Ref()->GetDeformerCount();
		}			
		FbxDeformer^ FbxGeometry::GetDeformer(int index)
		{			
			KFbxDeformer* d = _Ref()->GetDeformer(index);
			if(d)
				return FbxCreator::CreateFbxDeformer(d);
			return nullptr;
		}
		int FbxGeometry::GetDeformerCount(FbxDeformer::DeformerType type)
		{
			return _Ref()->GetDeformerCount((KFbxDeformer::EDeformerType)type);
		}
		FbxDeformer^ FbxGeometry::GetDeformer(int index, FbxDeformer::DeformerType type)
		{			
			if(type == FbxDeformer::DeformerType::Skin)
			{
				KFbxSkin* skin = (KFbxSkin*)_Ref()->GetDeformer(index, (KFbxDeformer::EDeformerType)type);
				if(skin)
					return gcnew FbxSkin(skin);				
			}
			KFbxDeformer* deformer = _Ref()->GetDeformer(index, (KFbxDeformer::EDeformerType)type);			
			return FbxCreator::CreateFbxDeformer(deformer);						
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGeometry,KFbxGeometryWeightedMap,GetSourceGeometryWeightedMap(),FbxGeometryWeightedMap,SourceGeometryWeightedMap);

		int FbxGeometry::DestinationGeometryWeightedMapCount::get()
		{
			return _Ref()->GetDestinationGeometryWeightedMapCount();
		}			
		FbxGeometryWeightedMap^ FbxGeometry::GetDestinationGeometryWeightedMap(int index)
		{
			KFbxGeometryWeightedMap* g = _Ref()->GetDestinationGeometryWeightedMap(index);
			if(g)
				return gcnew FbxGeometryWeightedMap(g);
			return nullptr;
		}
		int FbxGeometry::AddShape(FbxShape^ shape, String^ shapeName)
		{
			STRINGTO_CONSTCHAR_ANSI(n,shapeName);
			int i = _Ref()->AddShape(shape->_Ref(),n);
			FREECHARPOINTER(n);
			return i;
		}
		void FbxGeometry::ClearShape()
		{
			_Ref()->ClearShape();
		}
		int FbxGeometry::ShapeCount::get()
		{
			return _Ref()->GetShapeCount();
		}			
		FbxShape^ FbxGeometry::GetShape(int index)
		{
			return gcnew FbxShape(_Ref()->GetShape(index));
		}
		String^ FbxGeometry::GetShapeName(int index)
		{
			return gcnew String(_Ref()->GetShapeName(index));
		}
		FbxCurve^ FbxGeometry::GetShapeChannel(String^ shapeName, bool createAsNeeded, String^ takeName)
		{			
			STRINGTO_CONSTCHAR_ANSI(n,shapeName);
			FbxCurve^ curve = nullptr;

			if(takeName )
			{
				STRINGTO_CONSTCHAR_ANSI(t,takeName);
				KFCurve* k = _Ref()->GetShapeChannel(n,createAsNeeded,t);
				if(k)
					curve = gcnew FbxCurve(k);
				FREECHARPOINTER(t);
			}
			else
			{
				KFCurve* k = _Ref()->GetShapeChannel(n,createAsNeeded);
				if(k)
					curve = gcnew FbxCurve(k);
			}
			FREECHARPOINTER(n);
			return curve;
		}
		FbxCurve^ FbxGeometry::GetShapeChannel(String^ shapeName, bool createAsNeeded)
		{
			STRINGTO_CONSTCHAR_ANSI(n,shapeName);
			FbxCurve^ curve = nullptr;			
			KFCurve* k = _Ref()->GetShapeChannel(n,createAsNeeded);
			if(k)
				curve = gcnew FbxCurve(k);
			FREECHARPOINTER(n);
			return curve;
		}
		FbxCurve^ FbxGeometry::GetShapeChannel(int index, bool createAsNeeded ,String^ takeName)
		{
			FbxCurve^ curve = nullptr;
			if(takeName )
			{
				STRINGTO_CONSTCHAR_ANSI(t,takeName);
				KFCurve* k = _Ref()->GetShapeChannel(index,createAsNeeded,t);
				if(k)
					curve = gcnew FbxCurve(k);
				FREECHARPOINTER(t);
			}
			else
				curve = gcnew FbxCurve(_Ref()->GetShapeChannel(index,createAsNeeded));
			return curve;
		}
		FbxXMatrix^ FbxGeometry::GetPivot(FbxXMatrix^ matrix)
		{
			return gcnew FbxXMatrix(&_Ref()->GetPivot(*matrix->_Ref()));
		}
		void FbxGeometry::SetPivot(FbxXMatrix^ matrix)
		{
			_Ref()->SetPivot(*matrix->_Ref());
		}
		void FbxGeometry::ApplyPivot()
		{
			_Ref()->ApplyPivot();
		}
		void FbxGeometry::SetDefaultShape(int index, double percent)
		{
			_Ref()->SetDefaultShape(index,percent);
		}
		void FbxGeometry::SetDefaultShape(String^ shapeName, double percent)
		{
			STRINGTO_CONSTCHAR_ANSI(n,shapeName);											
			_Ref()->SetDefaultShape(n,percent);
			FREECHARPOINTER(n);
		}
		double FbxGeometry::GetDefaultShape(int index)
		{
			return _Ref()->GetDefaultShape(index);
		}
		double FbxGeometry::GetDefaultShape(String^ shapeName)
		{
			STRINGTO_CONSTCHAR_ANSI(n,shapeName);
			double d = _Ref()->GetDefaultShape(n);
			FREECHARPOINTER(n);
			return d;
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGeometry,GetError(),FbxErrorManaged,KError);
		
		FbxGeometry::Error FbxGeometry::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}
		String^ FbxGeometry::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}		

		CLONE_DEFINITION(FbxGeometry,KFbxGeometry); 

#ifndef DOXYGEN_SHOULD_SKIP_THIS							

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		

	}
}