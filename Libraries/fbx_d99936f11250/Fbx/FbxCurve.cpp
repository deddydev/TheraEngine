#pragma once
#include "stdafx.h"
#include "FbxCurve.h"
#include "FbxTime.h"


namespace Skill
{
	namespace FbxSDK
	{							
		int FbxCurveEvent::KeyIndexStart::get()
		{
			return _Ref()->mKeyIndexStart;
		}
		void FbxCurveEvent::KeyIndexStart::set(int value)
		{
			_Ref()->mKeyIndexStart = value;
		}								
		int FbxCurveEvent::KeyIndexStop::get()
		{
			return _Ref()->mKeyIndexStop;
		}
		void FbxCurveEvent::KeyIndexStop::set(int value)
		{
			_Ref()->mKeyIndexStop = value;
		}							
		int FbxCurveEvent::EventCount::get()
		{
			return _Ref()->mEventCount;
		}
		void FbxCurveEvent::EventCount::set(int value)
		{
			_Ref()->mEventCount = value;
		}							
		void FbxCurveEvent::Clear ()
		{
			_Ref()->Clear();
		}							
		void FbxCurveEvent::Add (int what, int index)
		{
			_Ref()->Add(what,index);
		}								
		void FbxCurveTangentInfo::CollectManagedMemory()
		{
		}

		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mDerivative,kFCurveDouble,Derivative);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mWeight,kFCurveDouble,Weight);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mWeighted,bool,Weighted);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mVelocity,kFCurveDouble,Velocity);			
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mHasVelocity,bool,HasVelocity);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveTangentInfo,mAuto,kFCurveDouble,Auto);			

		void FbxCurveKey::CollectManagedMemory()
		{		
		}

		void FbxCurveKey::Set(FbxTime^ time,kFCurveDouble value,FbxCurveKey::KeyInterpolation interpolation, 
			FbxCurveKey::KeyTangentMode tangentMode, 
				kFCurveDouble data0,kFCurveDouble data1,
				FbxCurveKey::KeyTangentWeightMode tangentWeightMode, 
				kFCurveDouble weight0,kFCurveDouble weight1,
				kFCurveDouble velocity0,kFCurveDouble velocity1
				)
		{
			_Ref()->Set(*time->_Ref(),value,(kUInt)interpolation,(kUInt)tangentMode,data0,data1,(kUInt)tangentWeightMode,weight0,weight1,velocity0,velocity1);
		}

		void FbxCurveKey::SetTCB(FbxTime^ time, kFCurveDouble value,float data0, float data1,float data2)
		{
			_Ref()->SetTCB(*time->_Ref(),value,data0,data1,data2);
		}
		void FbxCurveKey::Set(FbxCurveKey^ source)
		{
			this->_Ref()->Set(*source->_Ref());
		}
		FbxCurveKey::KeyInterpolation FbxCurveKey::Interpolation::get()
		{
			return (FbxCurveKey::KeyInterpolation)_Ref()->GetInterpolation();
		}
		void FbxCurveKey::Interpolation::set(FbxCurveKey::KeyInterpolation value)
		{
			_Ref()->SetInterpolation((kUInt)value);
		}		

		FbxCurveKey::KeyConstantMode FbxCurveKey::ConstantMode::get()
		{
			return (FbxCurveKey::KeyConstantMode)_Ref()->GetConstantMode();
		}
		void FbxCurveKey::ConstantMode::set(FbxCurveKey::KeyConstantMode value)
		{
			return _Ref()->SetConstantMode((kFCurveConstantMode)value);
		}

		FbxCurveKey::KeyTangentMode FbxCurveKey::GetTangentMode( bool includeOverrides)
		{
			return (FbxCurveKey::KeyTangentMode)_Ref()->GetTangeantMode(includeOverrides);
		}

		FbxCurveKey::KeyTangentMode FbxCurveKey::TangentMode::get()
		{
			return (FbxCurveKey::KeyTangentMode)_Ref()->GetTangeantMode();
		}
		void FbxCurveKey::TangentMode::set(FbxCurveKey::KeyTangentMode value)
		{
			return _Ref()->SetTangeantMode((kUInt)value);
		}

		FbxCurveKey::KeyTangentWeightMode FbxCurveKey::TangentWeightMode::get()
		{
			return (FbxCurveKey::KeyTangentWeightMode)_Ref()->GetTangeantWeightMode();
		}
		FbxCurveKey::KeyTangentVelocityMode FbxCurveKey::TangentVelocityMode::get()
		{
			return (FbxCurveKey::KeyTangentVelocityMode)_Ref()->GetTangeantVelocityMode();
		}

		void FbxCurveKey::SetTangentWeightMode(FbxCurveKey::KeyTangentWeightMode tangentWeightMode, FbxCurveKey::KeyTangentWeightMode mask)
		{
			_Ref()->SetTangeantWeightMode((kUInt)tangentWeightMode,(kUInt)mask);
		}
		void FbxCurveKey::TangentWeightMode::set(FbxCurveKey::KeyTangentWeightMode value)
		{
			_Ref()->SetTangeantWeightMode((kUInt)value);
		}
		void FbxCurveKey::SetTangentVelocityMode(FbxCurveKey::KeyTangentVelocityMode  tangentVelocityMode, FbxCurveKey::KeyTangentVelocityMode mask)
		{
			_Ref()->SetTangeantVelocityMode((kUInt)tangentVelocityMode,(kUInt)mask);
		}
		void FbxCurveKey::TangentVelocityMode::set(FbxCurveKey::KeyTangentVelocityMode value)
		{
			_Ref()->SetTangeantVelocityMode((kUInt)value);
		}

		kFCurveDouble FbxCurveKey::GetDataDouble(FbxCurveDataIndex index)
		{
			return _Ref()->GetDataDouble((EKFCurveDataIndex)index);
		}
		void FbxCurveKey::SetDataDouble(FbxCurveDataIndex index, kFCurveDouble value)
		{
			_Ref()->SetDataDouble((EKFCurveDataIndex)index,value);
		}
		float FbxCurveKey::GetDataFloat(FbxCurveDataIndex index)
		{
			return _Ref()->GetDataFloat((EKFCurveDataIndex)index);
		}
		void FbxCurveKey::SetDataFloat(FbxCurveDataIndex index, float value)
		{
			_Ref()->SetDataFloat((EKFCurveDataIndex)index,value);
		}

		kFCurveDouble FbxCurveKey::Value::get()
		{
			return _Ref()->GetValue();
		}
		void FbxCurveKey::Value::set(kFCurveDouble value)
		{
			_Ref()->SetValue(value);
		}

		void FbxCurveKey::IncValue(kFCurveDouble value)
		{
			_Ref()->IncValue(value);
		}
		void FbxCurveKey::MultValue(kFCurveDouble value)
		{
			_Ref()->MultValue(value);
		}
		void FbxCurveKey::MultTangent(kFCurveDouble value)
		{
			_Ref()->MultTangeant(value);
		}
		FbxTime^ FbxCurveKey::Time::get()
		{
			return gcnew FbxTime(_Ref()->GetTime());
		}
		void FbxCurveKey::Time::set(FbxTime^ value)
		{
			_Ref()->SetTime(*value->_Ref());
		}

		void FbxCurveKey::IncTime(FbxTime^ time)
		{
			_Ref()->SetTime(*time->_Ref());
		}

		bool FbxCurveKey::Selected::get()
		{
			return _Ref()->GetSelected();
		}
		void FbxCurveKey::Selected::set(bool value)
		{
			_Ref()->SetSelected(value);
		}

		bool FbxCurveKey::MarkedForManipulation::get()
		{
			return _Ref()->GetMarkedForManipulation();
		}
		void FbxCurveKey::MarkedForManipulation::set(bool value)
		{
			_Ref()->SetMarkedForManipulation(value);
		}

		FbxCurveKey::KeyTangentVisibility FbxCurveKey::TangentVisibility::get()
		{
			return (FbxCurveKey::KeyTangentVisibility)_Ref()->GetTangeantVisibility();
		}

		void FbxCurveKey::TangentVisibility::set(FbxCurveKey::KeyTangentVisibility value)
		{
			return _Ref()->SetTangeantVisibility((kUInt)value);
		}

		bool FbxCurveKey::Break::get()
		{
			return _Ref()->GetBreak();
		}
		void FbxCurveKey::Break::set(bool value)
		{
			_Ref()->SetBreak(value);
		}

		void FbxCurveKey::Initialize()
		{
			_Ref()->Init();
		}

		void FbxCurve::CollectManagedMemory()
		{

		}
		void FbxCurve::Destroy(int local)
		{
			_Ref()->Destroy(local);
		}

		float FbxCurve::RColor::get()
		{
			return _Ref()->GetColor()[0];
		}
		void FbxCurve::RColor::set(float value)
		{
			_Ref()->GetColor()[0] = value;
		}

		float FbxCurve::GColor::get()
		{
			return _Ref()->GetColor()[1];
		}
		void FbxCurve::GColor::set(float value)
		{
			_Ref()->GetColor()[1] = value;
		}

		float FbxCurve::BColor::get()
		{
			return _Ref()->GetColor()[2];
		}
		void FbxCurve::BColor::set(float value)
		{
			_Ref()->GetColor()[2] = value;
		}

		kFCurveDouble FbxCurve::Value::get()
		{
			return _Ref()->GetValue();
		}

		void FbxCurve::Value::set(kFCurveDouble value)
		{
			_Ref()->SetValue(value);
		}


		void FbxCurve::ResizeKeyBuffer(int keyCount)
		{
			_Ref()->ResizeKeyBuffer(keyCount);
		}
		void FbxCurve::KeyModifyBegin()
		{
			_Ref()->KeyModifyBegin();
		}
		void FbxCurve::KeyModifyEnd()
		{
			_Ref()->KeyModifyEnd();
		}
		int FbxCurve::KeyCount::get()
		{
			return _Ref()->KeyGetCount();
		}

		int FbxCurve::KeySelectionCount::get()
		{
			return _Ref()->KeyGetSelectionCount();
		}
		void FbxCurve::KeySelectAll()
		{
			_Ref()->KeySelectAll();
		}
		void FbxCurve::KeyUnselectAll()
		{
			_Ref()->KeyUnselectAll();
		}
		FbxCurveKey^ FbxCurve::KeyGet(kFCurveIndex index)
		{
			return gcnew FbxCurveKey(_Ref()->KeyGet(index));
		}
		void FbxCurve::KeyClear()
		{
			_Ref()->KeyClear();
		}
		void FbxCurve::KeyShrink()
		{
			_Ref()->KeyShrink();
		}
		bool FbxCurve::KeySet(kFCurveIndex index, FbxCurveKey^ key)
		{
			return _Ref()->KeySet(index,*key->_Ref());
		}
		bool FbxCurve::KeySet(kFCurveIndex index, FbxCurve^ sourceCurve, int sourceIndex)
		{
			return _Ref()->KeySet(index,sourceCurve->_Ref(),sourceIndex);
		}

		int FbxCurve::KeyMove(kFCurveIndex index, FbxTime^ time)
		{
			return _Ref()->KeyMove(index,*time->_Ref());
		}
		bool FbxCurve::KeyMoveOf(bool selectedOnly,FbxTime^ deltaTime,kFCurveDouble deltaValue)
		{
			return _Ref()->KeyMoveOf(selectedOnly,*deltaTime->_Ref(),deltaValue);
		}
		bool FbxCurve::KeyMoveValueTo(bool selectedOnly, kFCurveDouble value)
		{
			return _Ref()->KeyMoveValueTo(selectedOnly,value);
		}
		bool FbxCurve::KeyScaleValue (bool selectedOnly, kFCurveDouble multValue)
		{
			return _Ref()->KeyScaleValue (selectedOnly,multValue);
		}
		bool FbxCurve::KeyScaleTangent (bool selectedOnly, kFCurveDouble multValue)
		{
			return _Ref()->KeyScaleTangeant(selectedOnly,multValue);
		}
		bool FbxCurve::KeyScaleValueAndTangent(bool selectedOnly, kFCurveDouble multValue)
		{
			return _Ref()->KeyScaleValueAndTangeant(selectedOnly,multValue);
		}
		bool FbxCurve::KeyRemove(kFCurveIndex index)
		{
			return _Ref()->KeyRemove(index);
		}


		int FbxCurve::KeyInsert( FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			int i = _Ref()->KeyInsert(*time->_Ref(),&l);
			last = l;
			return i;
		}
		int FbxCurve::KeyInsert( FbxTime^ time)
		{
			return _Ref()->KeyInsert(*time->_Ref());
		}
		int FbxCurve::KeyAdd (FbxTime^ time, FbxCurveKey^ key, kFCurveIndex %last)
		{
			int l;
			int i = _Ref()->KeyAdd(*time->_Ref(),*key->_Ref(),&l);
			last = l;
			return i;
		}
		int FbxCurve::KeyAdd (FbxTime^ time, FbxCurveKey^ key)
		{
			return _Ref()->KeyAdd(*time->_Ref(),*key->_Ref());
		}
		int FbxCurve::KeyAdd(FbxTime^ time, FbxCurve^ sourceCurve, int sourceIndex, kFCurveIndex %last)
		{
			int l;
			int i = _Ref()->KeyAdd(*time->_Ref(),sourceCurve->_Ref(),sourceIndex,&l);
			last = l;
			return i;
		}
		int FbxCurve::KeyAdd(FbxTime^ time, FbxCurve^ sourceCurve, int sourceIndex)
		{
			return _Ref()->KeyAdd(*time->_Ref(),sourceCurve->_Ref(),sourceIndex);
		}
		int FbxCurve::KeyAdd(FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			int i = _Ref()->KeyAdd(*time->_Ref(),&l);
			last = l;
			return i;
		}
		int FbxCurve::KeyAdd(FbxTime^ time)
		{
			return _Ref()->KeyAdd(*time->_Ref());
		}
		int FbxCurve::KeyAppend(FbxTime^ atTime, FbxCurve^ sourceCurve, int sourceIndex)
		{
			return _Ref()->KeyAppend(*atTime->_Ref(),sourceCurve->_Ref(),sourceIndex);
		}
		int FbxCurve::KeyAppendFast(FbxTime^ time, kFCurveDouble value)
		{
			return _Ref()->KeyAppendFast(*time->_Ref(),value);
		}
		double FbxCurve::KeyFind(FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			double d = _Ref()->KeyFind(*time->_Ref(),&l);
			last = l;
			return d;
		}
		double FbxCurve::KeyFind(FbxTime^ time)
		{
			return _Ref()->KeyFind(*time->_Ref());
		}
		void FbxCurve::KeySet(kFCurveIndex keyIndex,FbxTime^ time, 
				kFCurveDouble value, 
				FbxCurveKey::KeyInterpolation interpolation, 
				FbxCurveKey::KeyTangentMode tangentMode, 
				kFCurveDouble data0,kFCurveDouble data1,
				FbxCurveKey::KeyTangentWeightMode tangentWeightMode, 
				kFCurveDouble weight0,kFCurveDouble weight1,
				kFCurveDouble velocity0,
				kFCurveDouble velocity1)
		{
			_Ref()->KeySet(keyIndex,*time->_Ref(),
				value,(kFCurveInterpolation)interpolation,(kFCurveInterpolation)tangentMode, 
				data0,data1,(kFCurveInterpolation)tangentWeightMode, 
				weight0,weight1,
				velocity0,velocity1);
		}

		void FbxCurve::KeySet(kFCurveIndex keyIndex,FbxTime^ time, 
				kFCurveDouble value, 
				FbxCurveKey::KeyInterpolation interpolation)
		{
			_Ref()->KeySet(keyIndex,*time->_Ref(), 
				value,(kFCurveInterpolation)interpolation);
		}

		void FbxCurve::KeySet(kFCurveIndex keyIndex,
				FbxTime^ time, 
				kFCurveDouble value, 
				kFCurveInterpolation interpolation, 
				kFCurveTangeantMode tangentMode, 
				kFCurveDouble data0,
				kFCurveDouble data1,
				kFCurveTangeantWeightMode tangentWeightMode, 
				kFCurveDouble weight0,
				kFCurveDouble weight1,
				kFCurveDouble velocity0,
				kFCurveDouble velocity1)
		{
			_Ref()->KeySet(keyIndex,*time->_Ref(), 
				value,interpolation,tangentMode, 
				data0,data1,
				tangentWeightMode,
				weight0,weight1,
				velocity0,velocity1);
		}
			void FbxCurve::KeySet(
				kFCurveIndex keyIndex,
				FbxTime^ time, 
				kFCurveDouble value, 
				kFCurveInterpolation interpolation,
				kFCurveTangeantMode tangentMode)
			{
				_Ref()->KeySet(keyIndex,*time->_Ref(), 
				value,interpolation,tangentMode);
			}


		void FbxCurve::KeySetTCB(kFCurveIndex keyIndex,FbxTime^ time, 
			kFCurveDouble value, 
			float data0,float data1,float data2)
		{
			_Ref()->KeySetTCB(keyIndex,*time->_Ref(),value,data0,data1,data2);
		}

		FbxCurveKey::KeyInterpolation FbxCurve::KeyGetInterpolation(kFCurveIndex keyIndex)
		{
			return (FbxCurveKey::KeyInterpolation)_Ref()->KeyGetInterpolation(keyIndex);
		}

		void FbxCurve::KeySetInterpolation(kFCurveIndex keyIndex, FbxCurveKey::KeyInterpolation interpolation)
		{
			_Ref()->KeySetInterpolation(keyIndex,(kFCurveInterpolation)interpolation);
		}
		FbxCurveKey::KeyConstantMode FbxCurve::KeyGetConstantMode(kFCurveIndex keyIndex)
		{
			return (FbxCurveKey::KeyConstantMode)_Ref()->KeyGetConstantMode(keyIndex);
		}
		FbxCurveKey::KeyTangentMode FbxCurve::KeyGetTangentMode(kFCurveIndex keyIndex, bool includeOverrides)
		{
			return (FbxCurveKey::KeyTangentMode)_Ref()->KeyGetTangeantMode(keyIndex,includeOverrides);
		}

		FbxCurveKey::KeyTangentWeightMode FbxCurve::KeyGetTangentWeightMode(kFCurveIndex keyIndex)
		{
			return (FbxCurveKey::KeyTangentWeightMode)_Ref()->KeyGetTangeantWeightMode(keyIndex);
		}
		FbxCurveKey::KeyTangentVelocityMode FbxCurve::KeyGetTangentVelocityMode(kFCurveIndex keyIndex)
		{
			return (FbxCurveKey::KeyTangentVelocityMode)_Ref()->KeyGetTangeantVelocityMode(keyIndex);
		}
		void FbxCurve::KeySetConstantMode(kFCurveIndex keyIndex, FbxCurveKey::KeyConstantMode mode)
		{
			_Ref()->KeySetConstantMode(keyIndex,(kFCurveConstantMode)mode);
		}
		void FbxCurve::KeySetTangentMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentMode tangent)
		{
			_Ref()->KeySetTangeantMode(keyIndex,(kFCurveTangeantMode)tangent);
		}
		void FbxCurve::KeySetTangentWeightMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentWeightMode tangentWeightMode, FbxCurveKey::KeyTangentWeightMode mask)
		{
			_Ref()->KeySetTangeantWeightMode(keyIndex,(kFCurveTangeantWeightMode)tangentWeightMode,(kFCurveTangeantWeightMode)mask);
		}
		void FbxCurve::KeySetTangentWeightMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentWeightMode tangentWeightMode)
		{
			_Ref()->KeySetTangeantWeightMode(keyIndex,(kFCurveTangeantWeightMode)tangentWeightMode);
		}
		void FbxCurve::KeySetTangentVelocityMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVelocityMode tangentVelocityMode, FbxCurveKey::KeyTangentVelocityMode mask)
		{
			_Ref()->KeySetTangeantVelocityMode(keyIndex,(kFCurveTangeantVelocityMode)tangentVelocityMode,(kFCurveTangeantVelocityMode)mask);
		}
		void FbxCurve::KeySetTangentVelocityMode(kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVelocityMode tangentVelocityMode)
		{
			_Ref()->KeySetTangeantVelocityMode(keyIndex,(kFCurveTangeantVelocityMode)tangentVelocityMode);
		}
		kFCurveDouble FbxCurve::KeyGetDataDouble(kFCurveIndex keyIndex, FbxCurveDataIndex index)
		{
			return _Ref()->KeyGetDataDouble(keyIndex,(EKFCurveDataIndex)index);
		}
		void FbxCurve::KeySetDataDouble(kFCurveIndex keyIndex, FbxCurveDataIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetDataDouble(keyIndex,(EKFCurveDataIndex)index,value);
		}
		float FbxCurve::KeyGetDataFloat(kFCurveIndex keyIndex, FbxCurveDataIndex index)
		{
			return _Ref()->KeyGetDataFloat(keyIndex,(EKFCurveDataIndex)index);
		}
		void FbxCurve::KeySetDataFloat(kFCurveIndex keyIndex, FbxCurveDataIndex index, float value)
		{
			_Ref()->KeySetDataFloat(keyIndex,(EKFCurveDataIndex)index,value);
		}
		kFCurveDouble FbxCurve::KeyGetValue(kFCurveIndex keyIndex)
		{
			return _Ref()->KeyGetValue(keyIndex);
		}
		void FbxCurve::KeySetValue(kFCurveIndex keyIndex, kFCurveDouble value)
		{
			_Ref()->KeySetValue(keyIndex,value);
		}
		void FbxCurve::KeyIncValue(kFCurveIndex keyIndex, kFCurveDouble value)
		{
			_Ref()->KeyIncValue(keyIndex,value);
		}
		void FbxCurve::KeyMultValue(kFCurveIndex keyIndex, kFCurveDouble value)
		{
			_Ref()->KeyMultValue(keyIndex,value);
		}
		void FbxCurve::KeyMultTangent(kFCurveIndex keyIndex, kFCurveDouble value)
		{
			_Ref()->KeyMultTangeant(keyIndex,value);
		}
		FbxTime^ FbxCurve::KeyGetTime(kFCurveIndex keyIndex)
		{
			return gcnew FbxTime(_Ref()->KeyGetTime(keyIndex));
		}
		void FbxCurve::KeySetTime(kFCurveIndex keyIndex, FbxTime^ time)
		{
			_Ref()->KeySetTime(keyIndex,*time->_Ref());
		}
		void FbxCurve::KeyIncTime(kFCurveIndex keyIndex, FbxTime^ time)
		{
			_Ref()->KeyIncTime(keyIndex,*time->_Ref());
		}
		void FbxCurve::KeySetSelected(kFCurveIndex keyIndex, bool selected)
		{
			_Ref()->KeySetSelected(keyIndex,selected);
		}
		bool FbxCurve::KeyGetSelected(kFCurveIndex keyIndex)
		{
			return _Ref()->KeyGetSelected(keyIndex);
		}
		void FbxCurve::KeySetMarkedForManipulation(kFCurveIndex keyIndex, bool mark)
		{
			_Ref()->KeySetMarkedForManipulation(keyIndex,mark);
		}
		bool FbxCurve::KeyGetMarkedForManipulation(kFCurveIndex keyIndex)
		{
			return _Ref()->KeyGetMarkedForManipulation(keyIndex);
		}
		void FbxCurve::KeySetTangentVisibility (kFCurveIndex keyIndex, FbxCurveKey::KeyTangentVisibility visibility)
		{
			_Ref()->KeySetTangeantVisibility(keyIndex,(kUInt)visibility);
		}
		FbxCurveKey::KeyTangentVisibility FbxCurve::KeyGetTangentVisibility(kFCurveIndex keyIndex)
		{
			return (FbxCurveKey::KeyTangentVisibility)_Ref()->KeyGetTangeantVisibility((kUInt)keyIndex);
		}
		void FbxCurve::KeySetBreak(kFCurveIndex keyIndex, bool val)
		{
			_Ref()->KeySetBreak(keyIndex,val);
		}
		bool FbxCurve::KeyGetBreak(kFCurveIndex keyIndex)
		{
			return _Ref()->KeyGetBreak(keyIndex);
		}
		void FbxCurve::KeyTangentSetInterpolation(bool selectedOnly, FbxCurveKey::KeyInterpolation interpolation)
		{
			_Ref()->KeyTangeantSetInterpolation(selectedOnly,(kUInt)interpolation);
		}
		void FbxCurve::KeyTangentSetMode(bool selectedOnly, FbxCurveKey::KeyTangentMode tangentMode)
		{
			_Ref()->KeyTangeantSetMode(selectedOnly,(kUInt)tangentMode);
		}
		kFCurveDouble FbxCurve::KeyGetLeftDerivative(kFCurveIndex index)
		{
			return _Ref()->KeyGetLeftDerivative(index);
		}
		void FbxCurve::KeySetLeftDerivative(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetLeftDerivative(index,value);
		}
		kFCurveDouble FbxCurve::KeyGetLeftAuto(kFCurveIndex index, bool applyOvershootProtection)
		{
			return _Ref()->KeyGetLeftAuto(index,applyOvershootProtection);
		}
		void FbxCurve::KeySetLeftAuto(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetLeftAuto(index,value);
		}
		FbxCurveTangentInfo^ FbxCurve::KeyGetLeftDerivativeInfo(kFCurveIndex index)
		{
			return gcnew FbxCurveTangentInfo(_Ref()->KeyGetLeftDerivativeInfo(index));
		}
		void FbxCurve::KeySetLeftDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value, bool forceDerivative)
		{
			_Ref()->KeySetLeftDerivativeInfo(index,*value->_Ref(),forceDerivative);
		}
		void FbxCurve::KeyIncLeftDerivative(kFCurveIndex index, kFCurveDouble inc)
		{
			_Ref()->KeyIncLeftDerivative(index,inc);
		}
		kFCurveDouble FbxCurve::KeyGetRightDerivative(kFCurveIndex index)
		{
			return _Ref()->KeyGetRightDerivative(index);
		}
		void FbxCurve::KeySetRightDerivative(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetRightDerivative(index,value);
		}
		kFCurveDouble FbxCurve::KeyGetRightAuto(kFCurveIndex index, bool applyOvershootProtection)
		{
			return _Ref()->KeyGetRightAuto(index,applyOvershootProtection);
		}
		void FbxCurve::KeySetRightAuto(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetRightAuto(index,value);
		}
		FbxCurveTangentInfo^ FbxCurve::KeyGetRightDerivativeInfo(kFCurveIndex index)
		{
			return gcnew FbxCurveTangentInfo(_Ref()->KeyGetRightDerivativeInfo(index));
		}
		void FbxCurve::KeySetRightDerivativeInfo(kFCurveIndex index, FbxCurveTangentInfo^ value, bool forceDerivative)
		{
			_Ref()->KeySetRightDerivativeInfo(index,*value->_Ref(),forceDerivative);
		}
		void FbxCurve::KeyIncRightDerivative(kFCurveIndex index, kFCurveDouble inc)
		{
			_Ref()->KeyIncRightDerivative(index,inc);
		}
		kFCurveDouble FbxCurve::KeyGetRightBezierTangent(kFCurveIndex index)
		{
			return _Ref()->KeyGetRightBezierTangeant(index);
		}
		void FbxCurve::KeySetLeftBezierTangent(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetLeftBezierTangeant(index,value);
		}
		kFCurveDouble FbxCurve::KeyGetLeftBezierTangent(kFCurveIndex index)
		{
			return _Ref()->KeyGetLeftBezierTangeant(index);
		}
		void FbxCurve::KeySetRightBezierTangent(kFCurveIndex index, kFCurveDouble value)
		{
			_Ref()->KeySetRightBezierTangeant(index,value);
		}
		void FbxCurve::KeyMultDerivative(kFCurveIndex index, kFCurveDouble multValue)
		{
			_Ref()->KeyMultDerivative(index,multValue);
		}
		bool FbxCurve::KeyIsLeftTangentWeighted(kFCurveIndex index)
		{
			return _Ref()->KeyIsLeftTangeantWeighted(index);
		}
		bool FbxCurve::KeyIsRightTangentWeighted(kFCurveIndex index)
		{
			return _Ref()->KeyIsRightTangeantWeighted(index);
		}
		void FbxCurve::KeySetLeftTangentWeightedMode( kFCurveIndex index, bool weighted )
		{
			_Ref()->KeySetLeftTangeantWeightedMode(index,weighted );
		}
		void FbxCurve::KeySetRightTangentWeightedMode( kFCurveIndex index, bool weighted )
		{
			_Ref()->KeySetRightTangeantWeightedMode(index,weighted);
		}
		kFCurveDouble FbxCurve::KeyGetLeftTangentWeight(kFCurveIndex index)
		{
			return _Ref()->KeyGetLeftTangeantWeight(index);
		}
		kFCurveDouble FbxCurve::KeyGetRightTangentWeight(kFCurveIndex index)
		{
			return _Ref()->KeyGetRightTangeantWeight(index);
		}
		void FbxCurve::KeySetLeftTangentWeight( kFCurveIndex index, kFCurveDouble weight )
		{
			_Ref()->KeySetLeftTangeantWeight(index,weight);
		}
		void FbxCurve::KeySetRightTangentWeight( kFCurveIndex index, kFCurveDouble weight )
		{
			_Ref()->KeySetRightTangeantWeight(index,weight);
		}
		bool FbxCurve::KeyIsLeftTangentVelocity(kFCurveIndex index)
		{
			return _Ref()->KeyIsLeftTangeantVelocity(index);
		}
		bool FbxCurve::KeyIsRightTangentVelocity(kFCurveIndex index)
		{
			return _Ref()->KeyIsRightTangeantVelocity(index);
		}
		void FbxCurve::KeySetLeftTangentVelocityMode(kFCurveIndex index, bool velocity )
		{
			_Ref()->KeySetLeftTangeantVelocityMode(index,velocity );
		}
		void FbxCurve::KeySetRightTangentVelocityMode(kFCurveIndex index, bool velocity)
		{
			_Ref()->KeySetRightTangeantVelocityMode(index,velocity);
		}
		kFCurveDouble FbxCurve::KeyGetLeftTangentVelocity(kFCurveIndex index)
		{
			return _Ref()->KeyGetLeftTangeantVelocity(index);
		}
		kFCurveDouble FbxCurve::KeyGetRightTangentVelocity(kFCurveIndex index)
		{
			return _Ref()->KeyGetRightTangeantVelocity(index);
		}
		void FbxCurve::KeySetLeftTangentVelocity(kFCurveIndex index, kFCurveDouble velocity )
		{
			_Ref()->KeySetLeftTangeantVelocity(index,velocity);
		}
		void FbxCurve::KeySetRightTangentVelocity(kFCurveIndex index, kFCurveDouble velocity)
		{
			_Ref()->KeySetRightTangeantVelocity(index,velocity);
		}
		/*kFCurveExtrapolationMode FbxCurve::PreExtrapolation::get()
		{
			return _Ref()->GetPreExtrapolation();
		}*/
		/*void FbxCurve::PreExtrapolation::set(kFCurveExtrapolationMode value)
		{
			_Ref()->SetPreExtrapolation(value);
		}*/

		/*kULong FbxCurve::PreExtrapolationCount::get()
		{
			return _Ref()->GetPreExtrapolationCount();
		}*/
		/*void FbxCurve::PreExtrapolationCount::set(kULong value)
		{
			_Ref()->SetPreExtrapolationCount(value);
		}*/

		/*kFCurveExtrapolationMode FbxCurve::PostExtrapolation::get()
		{
			return _Ref()->GetPostExtrapolation();
		}*/
		/*void FbxCurve::PostExtrapolation::set(kFCurveExtrapolationMode value)
		{
			_Ref()->SetPostExtrapolation(value);
		}*/

		/*kULong FbxCurve::PostExtrapolationCount::get()
		{
			return _Ref()->GetPostExtrapolationCount();
		}*/
		/*void FbxCurve::PostExtrapolationCount::set(kULong value)
		{
			_Ref()->SetPostExtrapolationCount(value);
		}*/

		int FbxCurve::KeyGetCountAll()
		{
			return _Ref()->KeyGetCountAll();
		}
		double FbxCurve::KeyFindAll(FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			double d = _Ref()->KeyFindAll(*time->_Ref(),&l);
			last = l;
			return d;
		}
		double FbxCurve::KeyFindAll(FbxTime^ time)
		{
			return _Ref()->KeyFindAll(*time->_Ref());
		}
		kFCurveDouble FbxCurve::Evaluate (FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			kFCurveDouble d = _Ref()->Evaluate(*time->_Ref(),&l);
			last = l;
			return d;
		}
		kFCurveDouble FbxCurve::Evaluate (FbxTime^ time)
		{
			return _Ref()->Evaluate(*time->_Ref());
		}
		kFCurveDouble FbxCurve::EvaluateIndex( double index)
		{
			return _Ref()->EvaluateIndex(index);
		}
		kFCurveDouble FbxCurve::EvaluateLeftDerivative (FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			kFCurveDouble d = _Ref()->EvaluateLeftDerivative(*time->_Ref(),&l);
			last = l;
			return d;
		}
		kFCurveDouble FbxCurve::EvaluateLeftDerivative (FbxTime^ time)
		{
			return _Ref()->EvaluateLeftDerivative(*time->_Ref());
		}
		kFCurveDouble FbxCurve::EvaluateRightDerivative(FbxTime^ time, kFCurveIndex %last)
		{
			int l;
			kFCurveDouble d = _Ref()->EvaluateRightDerivative(*time->_Ref(),&l);
			last = l;
			return d;
		}
		kFCurveDouble FbxCurve::EvaluateRightDerivative(FbxTime^ time)
		{
			return _Ref()->EvaluateRightDerivative(*time->_Ref());
		}
		int FbxCurve::FindPeaks(kFCurveIndex leftKeyIndex, FbxTime^ peakTime1, FbxTime^ peakTime2)
		{
			return _Ref()->FindPeaks(leftKeyIndex,*peakTime1->_Ref(),*peakTime2->_Ref());
		}
		int FbxCurve::FindPeaks(kFCurveIndex leftKeyIndex, kFCurveDouble %peak1, kFCurveDouble %peak2)
		{
			kFCurveDouble p1,p2;
			int i = _Ref()->FindPeaks(leftKeyIndex,p1,p2);
			peak1 = p1;
			peak2 = p2;
			return i;
		}
		int FbxCurve::FindPeaks(kFCurveIndex leftKeyIndex, FbxTime^ peakTime1, kFCurveDouble %peak1, FbxTime^ peakTime2, kFCurveDouble %peak2)
		{
			kFCurveDouble p1,p2;
			int i = _Ref()->FindPeaks(leftKeyIndex,*peakTime1->_Ref(),p1,*peakTime2->_Ref(),p2);
			peak1 = p1;
			peak2 = p2;
			return i;
		}
		void FbxCurve::KeyGetPeriods(FbxTime^ averagePeriod,FbxTime^ minPeriod, FbxTime^ maxPeriod)
		{
			_Ref()->KeyGetPeriods(*averagePeriod->_Ref(),*minPeriod->_Ref(),*maxPeriod->_Ref());
		}
		FbxCurve^ FbxCurve::Copy(FbxTime^ start, FbxTime^ stop)
		{
			KFCurve* c = _Ref()->Copy(*start->_Ref(),*stop->_Ref());
			if(c)
				return gcnew FbxCurve(c);
			return nullptr;
		}
		void FbxCurve::CopyFrom(FbxCurve^ source, bool withKeys)
		{
			_Ref()->CopyFrom(*source->_Ref(),withKeys);
		}
		void FbxCurve::Replace(FbxCurve^ source, FbxTime^ start,FbxTime^ stop, bool useExactGivenSpan, bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset)
		{
			_Ref()->Replace(source->_Ref(),*start->_Ref(),*stop->_Ref(),useExactGivenSpan,keyStartEndOnNoKey,*timeSpanOffset->_Ref());
		}
		void FbxCurve::ReplaceForQuaternion(FbxCurve^ source, FbxTime^ start, FbxTime^ stop, kFCurveDouble scaleStart, kFCurveDouble scaleStop, bool useExactGivenSpan, bool keyStartEndOnNoKey,FbxTime^ timeSpanOffset)
		{
			_Ref()->ReplaceForQuaternion(source->_Ref(),*start->_Ref(),*stop->_Ref(),scaleStart,scaleStop,useExactGivenSpan,keyStartEndOnNoKey,*timeSpanOffset->_Ref());
		}
		void FbxCurve::ReplaceForEulerXYZ(FbxCurve^ source,FbxTime^ start, FbxTime^ stop, kFCurveDouble addFromStart, kFCurveDouble addAfterStop, bool valueSubOffsetAfterStart, bool valueSubOffsetAfterStop, bool useExactGivenSpan, bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset)
		{
			_Ref()->ReplaceForEulerXYZ(source->_Ref(),*start->_Ref(),*stop->_Ref(),addFromStart,addAfterStop,valueSubOffsetAfterStart,valueSubOffsetAfterStop,useExactGivenSpan,keyStartEndOnNoKey,*timeSpanOffset->_Ref());
		}
		void FbxCurve::Insert(FbxCurve^ source, FbxTime^ insertTime, kFCurveDouble firstKeyLeftDerivative, bool firstKeyIsWeighted, kFCurveDouble firstKeyWeight)
		{
			_Ref()->Insert(source->_Ref(),*insertTime->_Ref(),firstKeyLeftDerivative,firstKeyIsWeighted,firstKeyWeight);
		}
		void FbxCurve::Insert(FbxCurve^ source, FbxTime^ insertTime, FbxCurveTangentInfo^ firstKeyLeftDerivative )
		{
			_Ref()->Insert(source->_Ref(),*insertTime->_Ref(),*firstKeyLeftDerivative->_Ref());
		}
		bool FbxCurve::Delete(kFCurveIndex startIndex , kFCurveIndex stopIndex)
		{
			return _Ref()->Delete(startIndex ,stopIndex);
		}
		bool FbxCurve::Delete(FbxTime^ start, FbxTime^ stop)
		{
			return _Ref()->Delete(*start->_Ref(),*stop->_Ref());
		}
		bool FbxCurve::Delete(FbxTime^ start)
		{
			return _Ref()->Delete(*start->_Ref());
		}
		bool FbxCurve::Delete()
		{
			return _Ref()->Delete();
		}
		bool FbxCurve::IsKeyInterpolationPureCubicAuto(kFCurveIndex keyIndex)
		{
			return _Ref()->IsKeyInterpolationPureCubicAuto(keyIndex);
		}
	}
}