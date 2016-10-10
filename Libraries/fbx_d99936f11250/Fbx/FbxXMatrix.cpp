#pragma once
#include "stdafx.h"
#include "FbxXMatrix.h"
#include "FbxVector4.h"
#include "FbxQuaternion.h"


{
	namespace FbxSDK
	{		
		void FbxXMatrix::CollectManagedMemory()
		{
			this->_T = nullptr;
			this->_R = nullptr;
			this->_S = nullptr;
			this->_Q = nullptr;
		}
		FbxXMatrix::FbxXMatrix(FbxXMatrix^ xMatrix)
		{
			_SetPointer(new KFbxXMatrix(*xMatrix->_Ref()),true);
		}
		FbxXMatrix::FbxXMatrix(FbxVector4^ t,FbxVector4^ r,FbxVector4^ s)
		{
			_SetPointer(new KFbxXMatrix(*t->_Ref(),*r->_Ref(),*s->_Ref()),true);
		}
		double FbxXMatrix::Get(int y, int x)
		{
			return _Ref()->Get(y,x);
		}

		double FbxXMatrix::default::get(int y , int x)
		{
			return _Ref()->Get(y,x);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxXMatrix,GetT(),FbxVector4,T);
		void FbxXMatrix::T::set(FbxVector4^ value)
		{
			if(value)
			{				
				_Ref()->SetT(*value->_Ref());
			}
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxXMatrix,GetR(),FbxVector4,R);
		void FbxXMatrix::R::set(FbxVector4^ value)
		{
			if(value)
			{				
				_Ref()->SetR(*value->_Ref());
			}
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxXMatrix,GetQ(),FbxQuaternion,Q);
		void FbxXMatrix::Q::set(FbxQuaternion^ value)
		{
			if(value)
			{				
				_Ref()->SetQ(*value->_Ref());
			}
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxXMatrix,GetS(),FbxVector4,S);
		void FbxXMatrix::S::set(FbxVector4^ value)
		{
			if(value)
			{				
				_Ref()->SetS(*value->_Ref());
			}
		}

		FbxVector4^ FbxXMatrix::GetRow(int y)
		{
			return gcnew FbxVector4(_Ref()->GetRow(y));
		}

		FbxVector4^ FbxXMatrix::GetColumn(int x)
		{
			return gcnew FbxVector4(_Ref()->GetColumn(x));
		}
		void FbxXMatrix::SetIdentity()
		{
			_Ref()->SetIdentity();
		}

		void FbxXMatrix::SetTRS(FbxVector4^ t,
			FbxVector4^ r,
			FbxVector4^ s)
		{
			_Ref()->SetTRS(*t->_Ref(),*r->_Ref(),*s->_Ref());
		}
		void FbxXMatrix::SetTQS(FbxVector4^ t,
			FbxQuaternion^ q,
			FbxVector4^ s)
		{
			_Ref()->SetTQS(*t->_Ref(),*q->_Ref(),*s->_Ref());
		}
		bool FbxXMatrix::Equals(System::Object^ obj)
		{
			FbxXMatrix^ t = dynamic_cast<FbxXMatrix^>(obj);
			if(t)				
				return *_Ref() == *t->_Ref();
			return false;
		}

		FbxVector4^ FbxXMatrix::MultT(FbxVector4^ vector4)
		{
			return gcnew FbxVector4(_Ref()->MultT(*vector4->_Ref()));
		}
		FbxVector4^ FbxXMatrix::MultR(FbxVector4^ vector4)
		{
			return gcnew FbxVector4(_Ref()->MultR(*vector4->_Ref()));
		}
		FbxQuaternion^ FbxXMatrix::MultQ(FbxQuaternion^ quaternion)
		{
			return gcnew FbxQuaternion(_Ref()->MultQ(*quaternion->_Ref()));
		}
		FbxVector4^ FbxXMatrix::MultS(FbxVector4^ vector4)
		{
			return gcnew FbxVector4(_Ref()->MultS(*vector4->_Ref()));
		}
		FbxXMatrix^ FbxXMatrix::Negate()
		{
			return gcnew FbxXMatrix(-*_Ref());
		}
		FbxXMatrix^ FbxXMatrix::Inverse()
		{
			return gcnew FbxXMatrix(_Ref()->Inverse());
		}
		FbxXMatrix^ FbxXMatrix::Transpose()
		{
			return gcnew FbxXMatrix(_Ref()->Transpose());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS	
			void FbxXMatrix::CreateKFbxXMatrixRotation(double x, double y, double z)
			{
				_Ref()->CreateKFbxXMatrixRotation(x,y,z);
			}
			void FbxXMatrix::V2M(FbxXMatrix^ matrix, FbxVector4^ vector, FbxRotationOrder rotationOrder)
			{
				_Ref()->V2M(*matrix->_Ref(),*vector->_Ref(),(ERotationOrder)rotationOrder);
			}
			void FbxXMatrix::M2V(FbxVector4^ vector, FbxXMatrix^ matrix, FbxRotationOrder rotationOrder)
			{
				_Ref()->M2V(*vector->_Ref(),*matrix->_Ref(),(ERotationOrder)rotationOrder);
			}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}