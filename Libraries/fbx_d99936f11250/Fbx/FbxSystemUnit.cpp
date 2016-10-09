#pragma once
#include "stdafx.h"
#include "FbxSystemUnit.h"
#include "FbxScene.h"
#include "FbxNode.h"

namespace Skill
{
	namespace FbxSDK
	{	

		void FbxSystemUnit::CollectManagedMemory()
		{
		}

		FbxSystemUnit::FbxSystemUnit(double scaleFactor, double multiplier)
		{
			_SetPointer(new KFbxSystemUnit(scaleFactor,multiplier),true);
		}
		FbxSystemUnit::FbxSystemUnit(double scaleFactor)
		{
			_SetPointer(new KFbxSystemUnit(scaleFactor),true);
		}

		/*FbxSystemUnit::FbxSystemUnit(PredefinedSystemUnit type)
		{
			switch(type)
			{
			case PredefinedSystemUnit::Mm :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::mm;
				break;
			case PredefinedSystemUnit::Dm :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::dm;
				break;
			case PredefinedSystemUnit::Cm :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::cm;
				break;
			case PredefinedSystemUnit::M :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::m;
				break;
			case PredefinedSystemUnit::Km :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::km;
				break;
			case PredefinedSystemUnit::Inch :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::Inch;
				break;
			case PredefinedSystemUnit::Foot :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::Foot;
				break;
			case PredefinedSystemUnit::Mile :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::Mile;
				break;
			case PredefinedSystemUnit::Yard :
				_SetPointer(new KFbxSystemUnit(0),true);
				*_FbxSystemUnit = KFbxSystemUnit::Yard;
				break;
			}
		}*/		

		FbxSystemUnit::FbxUnitConversionOptions FbxSystemUnit::DefaultConversionOptions::get()
		{
			FbxSystemUnit::FbxUnitConversionOptions s;			
			s.ConvertLightIntensity = true;//KFbxSystemUnit::DefaultConversionOptions.mConvertLightIntensity;
			s.ConvertRrsNodes = false;//KFbxSystemUnit::DefaultConversionOptions.mConvertRrsNodes;
			return s;
		}
		
		void FbxSystemUnit::ConvertScene(FbxSceneManaged^ scene, FbxUnitConversionOptions options)
		{
			KFbxSystemUnit::KFbxUnitConversionOptions s;
			s.mConvertLightIntensity = options.ConvertLightIntensity;
			s.mConvertRrsNodes = options.ConvertRrsNodes;
			_Ref()->ConvertScene(scene->_Ref(),s);
		}
		
		void FbxSystemUnit::ConvertChildren(FbxNode^ root, FbxSystemUnit^ srcUnit,FbxUnitConversionOptions options)
		{
			KFbxSystemUnit::KFbxUnitConversionOptions s;
			s.mConvertLightIntensity = options.ConvertLightIntensity;
			s.mConvertRrsNodes = options.ConvertRrsNodes;
			_Ref()->ConvertChildren(root->_Ref(),*srcUnit->_Ref(),s);
		}

		
		void FbxSystemUnit::ConvertScene(FbxSceneManaged^ scene, FbxNode^ root,FbxUnitConversionOptions options)
		{
			KFbxSystemUnit::KFbxUnitConversionOptions s;
			s.mConvertLightIntensity = options.ConvertLightIntensity;
			s.mConvertRrsNodes = options.ConvertRrsNodes;
			_Ref()->ConvertScene(scene->_Ref(),root->_Ref(),s);
		}
		
		double FbxSystemUnit::ScaleFactor::get()
		{
			return _Ref()->GetScaleFactor();
		}
		
		String^ FbxSystemUnit::GetScaleFactorAsString(bool abbreviated)
		{
			FbxString kstr = _Ref()->GetScaleFactorAsString(abbreviated);
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}

		
		String^ FbxSystemUnit::GetScaleFactorAsString_Plurial()
		{
			FbxString kstr = _Ref()->GetScaleFactorAsString_Plurial();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		
		double FbxSystemUnit::Multiplier::get()
		{
			return _Ref()->GetMultiplier();
		}


		double FbxSystemUnit::GetConversionFactorTo(FbxSystemUnit^ target)
		{
			return _Ref()->GetConversionFactorTo(*target->_Ref());
		}
		
		double FbxSystemUnit::GetConversionFactorFrom(FbxSystemUnit^ source)
		{
			return _Ref()->GetConversionFactorFrom(*source->_Ref());
		}
	}
}