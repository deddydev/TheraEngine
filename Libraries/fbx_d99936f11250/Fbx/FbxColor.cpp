#pragma once
#include "stdafx.h"
#include "FbxColor.h"



{
	namespace FbxSDK
	{		
		void FbxColor::CollectManagedMemory()
		{			
		}						
		FbxColor::FbxColor(double red, double green, double blue, double alpha)
		{
			_SetPointer(new KFbxColor(red,green,blue,alpha),true);			
		}
		FbxColor::FbxColor(double red, double green, double blue)
		{
			_SetPointer(new KFbxColor(red,green,blue),true);
		}
		void FbxColor::Set(double red, double green, double blue, double alpha)
		{
			_Ref()->Set(red,green,blue,alpha);
		}
		void FbxColor::Set(double red, double green, double blue)
		{
			_Ref()->Set(red,green,blue);
		}
		bool FbxColor::IsValid::get()
		{
			if(_Free && _Ref() )
				return _Ref()->IsValid();
			return false;
		}				
		void FbxColor::Set(FbxColor^ color)
		{
			*this->_FbxColor = *color->_FbxColor;
		}								
		double FbxColor::Red::get()
		{
			return _Ref()->mRed;
		}
		void FbxColor::Red::set(double value)
		{
			_Ref()->mRed = value;
		}								
		double FbxColor::Green::get()
		{
			return _Ref()->mGreen;
		}
		void FbxColor::Green::set(double value)
		{
			_Ref()->mGreen = value;
		}								
		double FbxColor::Blue::get()
		{
			return _Ref()->mBlue;
		}
		void FbxColor::Blue::set(double value)
		{
			_Ref()->mBlue = value;
		}								
		double FbxColor::Alpha::get()
		{
			return _Ref()->mAlpha;
		}
		void FbxColor::Alpha::set(double value)
		{
			_Ref()->mAlpha = value;
		}

	}
}