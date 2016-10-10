#pragma once
#include "stdafx.h"
#include "FbxQuaternion.h"
#include "FbxVector4.h"


{
	namespace FbxSDK
	{		
		void FbxQuaternion::CollectManagedMemory()
		{
		}				
					
		FbxQuaternion::FbxQuaternion(FbxQuaternion^ q)
		{
			_SetPointer(new KFbxQuaternion(*q->_Ref()),true);			
		}
		FbxQuaternion::FbxQuaternion(double x, double y, double z, double w)
		{
			_SetPointer(new KFbxQuaternion(x,y,z,w),true);			
		}
		FbxQuaternion::FbxQuaternion(double x, double y, double z)
		{
			_SetPointer(new KFbxQuaternion(x,y,z),true);
		}
		void FbxQuaternion::CopyFrom(FbxQuaternion^ q)
		{
			*this->_Ref() = *q->_Ref();
		}
		double FbxQuaternion::GetAt(int index)
		{
			return _Ref()->GetAt(index);
		}
		void FbxQuaternion::SetAt(int index, double value)
		{
			_Ref()->SetAt(index,value);
		}

		double FbxQuaternion::default::get(int index)
		{
			return _Ref()->GetAt(index);
		}
		void FbxQuaternion::default::set(int index,double value)
		{
			_Ref()->SetAt(index,value);
		}

		double FbxQuaternion::X::get(){	return _Ref()->GetAt(0);}
		void FbxQuaternion::X::set(double value){_Ref()->SetAt(0,value);}

		double FbxQuaternion::Y::get(){	return _Ref()->GetAt(1);}
		void FbxQuaternion::Y::set(double value){_Ref()->SetAt(1,value);}

		double FbxQuaternion::Z::get(){	return _Ref()->GetAt(2);}
		void FbxQuaternion::Z::set(double value){_Ref()->SetAt(2,value);}

		double FbxQuaternion::W::get(){	return _Ref()->GetAt(3);}
		void FbxQuaternion::W::set(double value){_Ref()->SetAt(3,value);}



		void FbxQuaternion::Set(double x, double y, double z, double w )
		{
			_Ref()->Set(x,y,z,w);
		}
		void FbxQuaternion::Set(double x, double y, double z)
		{
			_Ref()->Set(x,y,z);
		}

		FbxQuaternion^ FbxQuaternion::Negate()
		{
			KFbxQuaternion q = -(*_Ref());
			return gcnew FbxQuaternion(q);
		}

		FbxQuaternion^ FbxQuaternion::Product(FbxQuaternion^ q)
		{
			KFbxQuaternion q1 = _Ref()->Product(*q->_Ref());
			return gcnew FbxQuaternion(q1);
		}
		void FbxQuaternion::Normalize()
		{
			_Ref()->Normalize();
		}
		void FbxQuaternion::Conjugate()
		{
			_Ref()->Conjugate();
		}

		void FbxQuaternion::ComposeSphericalXYZ(FbxVector4^ euler)
		{
			_Ref()->ComposeSphericalXYZ(*euler->_Ref());
		}
		FbxVector4^ FbxQuaternion::DecomposeSphericalXYZ()
		{
			KFbxVector4 v = _Ref()->DecomposeSphericalXYZ();
			return gcnew FbxVector4(v);
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS	
		void FbxQuaternion::GetKFbxQuaternionFromPositionToPosition(FbxVector4^ p0,FbxVector4^ p1)
		{
			_Ref()->GetKFbxQuaternionFromPositionToPosition(*p0->_Ref(),*p1->_Ref());
		}
#endif //doxygen


	}
}