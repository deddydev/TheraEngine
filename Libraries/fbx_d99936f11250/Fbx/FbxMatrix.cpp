#pragma once
#include "stdafx.h"
#include "FbxMatrix.h"
#include "FbxVector4.h"
#include "FbxQuaternion.h"

namespace Skill
{
	namespace FbxSDK
	{
		void FbxMatrix::CollectManagedMemory()
		{
		}

		FbxMatrix::FbxMatrix(FbxMatrix^ m)
		{
			_SetPointer(new KFbxMatrix(*m->_Ref()),true);			
		}
		FbxMatrix::FbxMatrix(FbxVector4^ t,
			FbxVector4^ r,
			FbxVector4^ s)
		{
			_SetPointer(new KFbxMatrix(*t->_Ref(),*r->_Ref(),*s->_Ref()),true);			
		}
		FbxMatrix::FbxMatrix (FbxVector4^ t,
			FbxQuaternion^ q,				
			FbxVector4^ s)
		{
			_SetPointer(new KFbxMatrix(*t->_Ref(),*q->_Ref(),*s->_Ref()),true);						
		}
		FbxMatrix::FbxMatrix(FbxXMatrix^ m)
		{
			_SetPointer(new KFbxMatrix(*m->_Ref()),true);
		}
		double FbxMatrix::Get(int y, int x)
		{
			return _Ref()->Get(y,x);
		}

		double FbxMatrix::default::get(int y, int x)
		{
			return _Ref()->Get(y,x);
		}
		void FbxMatrix::default::set(int y, int x,double value)
		{
			return _Ref()->Set(y,x,value);
		}

		FbxVector4^ FbxMatrix::GetRow(int y)
		{
			KFbxVector4 v = _Ref()->GetRow(y);
			return gcnew FbxVector4(v);
		}
		FbxVector4^ FbxMatrix::GetColumn(int x)
		{
			KFbxVector4 v = _Ref()->GetColumn(x);
			return gcnew FbxVector4(v);
		}
		void FbxMatrix::Set(int y, int x, double value)
		{
			_Ref()->Set(y,x,value);
		}
		void FbxMatrix::SetIdentity()
		{
			_Ref()->SetIdentity();
		}
		void FbxMatrix::SetTRS(FbxVector4^ t,
			FbxVector4^ r,
			FbxVector4^ s)
		{
			_Ref()->SetTRS(*t->_Ref(),*r->_Ref(),*s->_Ref());			
		}
		void FbxMatrix::SetTQS(FbxVector4^ t,
			FbxQuaternion^ q,
			FbxVector4^ s)
		{
			_Ref()->SetTQS(*t->_Ref(),*q->_Ref(),*s->_Ref());
		}
		void FbxMatrix::SetRow(int y, FbxVector4^ row)
		{
			_Ref()->SetRow(y, *row->_Ref());
		}
		void FbxMatrix::SetColumn(int x, FbxVector4^ column)
		{
			_Ref()->SetColumn(x, *column->_Ref());
		}			
		void FbxMatrix::CopyFrom(FbxMatrix^ m)
		{
			*this->_FbxMatrix = *m->_Ref();
		}
		FbxMatrix^ FbxMatrix::Negate()
		{
			KFbxMatrix m = -(*_Ref());
			return gcnew FbxMatrix(m);
		}

		FbxMatrix^ FbxMatrix::Transpose()
		{
			KFbxMatrix m = _Ref()->Transpose();
			return gcnew FbxMatrix(m);
		}
		FbxVector4^ FbxMatrix::MultNormalize(FbxVector4^ vector)
		{
			KFbxVector4 v = _Ref()->MultNormalize(*vector->_Ref());
			return gcnew FbxVector4(v);
		}
		bool FbxMatrix::Equals(System::Object^ obj)
		{
			FbxMatrix^ o = dynamic_cast<FbxMatrix^>(obj);
			if(o)
				return *_Ref() == *o->_Ref();
			return false;
		}					
		bool FbxMatrix::EqualsWith(FbxXMatrix^ m)
		{				
			if(m)
				return *_Ref() == *m->_Ref();
			return false;
		}	
	}
}