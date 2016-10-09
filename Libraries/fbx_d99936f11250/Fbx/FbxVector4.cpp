#pragma once
#include "stdafx.h"
#include "FbxVector4.h"
#include "FbxDouble3.h"

namespace Skill
{
	namespace FbxSDK
	{		
		
		void FbxVector4::CollectManagedMemory()
		{
		}

		double FbxVector4::X::get()	{return _Ref()->GetAt(0);}
		void FbxVector4::X::set(double value){return _Ref()->SetAt(0,value);}

		double FbxVector4::Y::get()	{return _Ref()->GetAt(1);}
		void FbxVector4::Y::set(double value){return _Ref()->SetAt(1,value);}

		double FbxVector4::Z::get()	{return _Ref()->GetAt(2);}
		void FbxVector4::Z::set(double value){return _Ref()->SetAt(2,value);}

		double FbxVector4::W::get()	{return _Ref()->GetAt(3);}
		void FbxVector4::W::set(double value){return _Ref()->SetAt(3,value);}
						
		FbxVector4::FbxVector4(FbxVector4^ vector4)
		{
			_SetPointer(new KFbxVector4(*vector4->_Ref()),true);			
		}
		FbxVector4::FbxVector4(FbxDouble3^ value)
		{
			_SetPointer(new KFbxVector4(*value->_Ref()),true);
		}

		FbxVector4::FbxVector4(double x, double y, double z, double w)
		{
			_SetPointer(new KFbxVector4(x,y,z,w),true);			
		}
		FbxVector4::FbxVector4(double x, double y, double z)
		{
			_SetPointer(new KFbxVector4(x,y,z),true);
		}
		void FbxVector4::CopyFrom(FbxVector4^ v)
		{
			*this->_Ref() = *v->_Ref();
		}
		double FbxVector4::GetAt(int index)
		{
			return _Ref()->GetAt(index);
		}

		double FbxVector4::default::get(int index)
		{
			return _Ref()->GetAt(index);
		}

		void FbxVector4::default::set(int index , double value)
		{
			_Ref()->SetAt(index,value);
		}

		void FbxVector4::SetAt(int index , double value)
		{
			_Ref()->SetAt(index,value);
		}
		void FbxVector4::Set(double x, double y, double z, double w)
		{
			_Ref()->Set(x,y,z,w);
		}
		void FbxVector4::Set(double x, double y, double z)
		{
			_Ref()->Set(x,y,z);
		}			
		FbxVector4^ FbxVector4::Negate()
		{
			KFbxVector4 v = -(*_Ref());
			return gcnew FbxVector4(v);
		}			
		double FbxVector4::DotProduct(FbxVector4^ vector)
		{
			return this->_Ref()->DotProduct(*vector->_Ref());
		}			
		FbxVector4^ FbxVector4::CrossProduct(FbxVector4^ vector)
		{
			KFbxVector4 v = this->_Ref()->CrossProduct(*vector->_Ref());
			return gcnew FbxVector4(v);
		}
		bool FbxVector4::AxisAlignmentInEulerAngle(FbxVector4^ AB, 
			FbxVector4^ A, 
			FbxVector4^ B, 
			FbxVector4^ angles)
		{
			return KFbxVector4::AxisAlignmentInEulerAngle(*AB->_Ref(), 
				*A->_Ref(), 
				*B->_Ref(), 
				*angles->_Ref());
		}			
		double FbxVector4::Length()
		{
			return _Ref()->Length();
		}
		double FbxVector4::SquareLength()
		{
			return _Ref()->SquareLength();
		}
		double FbxVector4::Distance(FbxVector4^ vector)
		{
			return this->_Ref()->Distance(*vector->_Ref());
		}
		void FbxVector4::Normalize()
		{
			_Ref()->Normalize();
		}
	}
}