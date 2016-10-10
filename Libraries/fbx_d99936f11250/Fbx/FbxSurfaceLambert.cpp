#pragma once
#include "stdafx.h"
#include "FbxSurfaceLambert.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxDouble3.h"



{
	namespace FbxSDK
	{	

		FBXOBJECT_DEFINITION(FbxSurfaceLambert,KFbxSurfaceLambert);		

		FbxDouble3^ FbxSurfaceLambert::EmissiveColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetEmissiveColor().Get());			
		}
		void FbxSurfaceLambert::EmissiveColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetEmissiveColor().Set(*value->_Ref());
			}
		}
		double FbxSurfaceLambert::EmissiveFactor::get()
		{
			return _Ref()->GetEmissiveFactor().Get();
		}
		void FbxSurfaceLambert::EmissiveFactor::set(double value)
		{
			_Ref()->GetEmissiveFactor().Set(value);
		}

		FbxDouble3^ FbxSurfaceLambert::AmbientColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetAmbientColor().Get());			
		}
		void FbxSurfaceLambert::AmbientColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetAmbientColor().Set(*value->_Ref());
			}
		}

		double FbxSurfaceLambert::AmbientFactor::get()
		{
			return _Ref()->GetAmbientFactor().Get();
		}
		void FbxSurfaceLambert::AmbientFactor::set(double value)
		{
			_Ref()->GetAmbientFactor().Set(value);
		}			

		FbxDouble3^ FbxSurfaceLambert::DiffuseColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetDiffuseColor().Get());			
		}
		void FbxSurfaceLambert::DiffuseColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetDiffuseColor().Set(*value->_Ref());
			}
		}					

		double FbxSurfaceLambert::DiffuseFactor::get()
		{
			return _Ref()->GetDiffuseFactor().Get();
		}
		void FbxSurfaceLambert::DiffuseFactor::set(double value)
		{
			_Ref()->GetDiffuseFactor().Set(value);
		}


		FbxDouble3^ FbxSurfaceLambert::Bump::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetBump().Get());			
		}
		void FbxSurfaceLambert::Bump::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetBump().Set(*value->_Ref());
			}
		}			

		FbxDouble3^ FbxSurfaceLambert::TransparentColor::get()
		{						
			return gcnew FbxDouble3(_Ref()->GetTransparentColor().Get());			
		}
		void FbxSurfaceLambert::TransparentColor::set(FbxDouble3^ value)
		{
			if(value)
			{				
				_Ref()->GetTransparentColor().Set(*value->_Ref());
			}
		}	

		double FbxSurfaceLambert::TransparencyFactor::get()
		{
			return _Ref()->GetTransparencyFactor().Get();
		}
		void FbxSurfaceLambert::TransparencyFactor::set(double value)
		{
			_Ref()->GetTransparencyFactor().Set(value);
		}			

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxSurfaceLambert,KFbxSurfaceLambert);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
	}
}