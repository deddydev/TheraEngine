#pragma once
#include "stdafx.h"
#include "FbxSurfacePhong.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxDouble3.h"


{
	namespace FbxSDK
	{	

		FBXOBJECT_DEFINITION(FbxSurfacePhong,KFbxSurfacePhong);


		FbxDouble3^ FbxSurfacePhong::SpecularColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetSpecularColor().Get());			
		}
		void FbxSurfacePhong::SpecularColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetSpecularColor().Set(*value->_Ref());
			}
		}
		double FbxSurfacePhong::SpecularFactor::get()
		{
			return _Ref()->GetSpecularFactor().Get();
		}
		void FbxSurfacePhong::SpecularFactor::set(double value)
		{
			_Ref()->GetSpecularFactor().Set(value);
		}

		double FbxSurfacePhong::Shininess::get()
		{
			return _Ref()->GetShininess().Get();
		}
		void FbxSurfacePhong::Shininess::set(double value)
		{
			_Ref()->GetShininess().Set(value);
		}

		FbxDouble3^ FbxSurfacePhong::ReflectionColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetReflectionColor().Get());			
		}
		void FbxSurfacePhong::ReflectionColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetReflectionColor().Set(*value->_Ref());
			}
		}

		double FbxSurfacePhong::ReflectionFactor::get()
		{
			return _Ref()->GetReflectionFactor().Get();
		}
		void FbxSurfacePhong::ReflectionFactor::set(double value)
		{
			_Ref()->GetReflectionFactor().Set(value);
		}			

#ifndef DOXYGEN_SHOULD_SKIP_THIS			
		CLONE_DEFINITION(FbxSurfacePhong,KFbxSurfacePhong);			

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
	}
}