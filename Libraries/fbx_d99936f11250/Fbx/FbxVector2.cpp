#pragma once
#include "stdafx.h"
#include "FbxVector2.h"

namespace Skill
{
	namespace FbxSDK
	{
		void FbxVector2::CollectManagedMemory()
		{
		}
		FbxVector2::FbxVector2(FbxVector2^ vector2)
		{
			_SetPointer(new KFbxVector2(*vector2->_Ref()),true);
		}
		FbxVector2::FbxVector2(double x, double y)
		{
			_SetPointer(new KFbxVector2(x,y),true);
		}
		void FbxVector2::CopyFrom(FbxVector2^ other)
		{
			*_Ref() = *other->_Ref();
		}

		double FbxVector2::X::get()	{return _Ref()->GetAt(0);}
		void FbxVector2::X::set(double value){return _Ref()->SetAt(0,value);}

		double FbxVector2::Y::get()	{return _Ref()->GetAt(1);}
		void FbxVector2::Y::set(double value){return _Ref()->SetAt(1,value);}

		double FbxVector2::default::get(int index)
		{
			return _Ref()->GetAt(index);
		}
		void FbxVector2::default::set(int index, double value)
		{
			_Ref()->SetAt(index,value);
		}

		double FbxVector2::GetAt(int index)
		{
			return _Ref()->GetAt(index);
		}
		void FbxVector2::SetAt(int index, double value)
		{
			_Ref()->SetAt(index,value);
		}
		void FbxVector2::Set(double x, double y)
		{
			_Ref()->Set(x,y);
		}
		FbxVector2^ FbxVector2::Negate()
		{
			return gcnew FbxVector2(-*_Ref());
		}
		double FbxVector2::DotProduct(FbxVector2^ vector)
		{
			return _Ref()->DotProduct(*vector->_Ref());
		}
		double FbxVector2::Length()
		{
			return _Ref()->Length();
		}
		double FbxVector2::SquareLength()
		{
			return _Ref()->SquareLength();
		}
		double FbxVector2::Distance(FbxVector2^ vector)
		{
			return _Ref()->Distance(*vector->_Ref());
		}
		void FbxVector2::Normalize()
		{
			_Ref()->Normalize();
		}
	}
}