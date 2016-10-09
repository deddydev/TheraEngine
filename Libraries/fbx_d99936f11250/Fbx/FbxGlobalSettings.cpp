#pragma once
#include "stdafx.h"
#include "FbxGlobalSettings.h"
#include "FbxAxisSystem.h"
#include "FbxSystemUnit.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxScene.h"


namespace Skill
{
	namespace FbxSDK
	{

		FBXOBJECT_DEFINITION(FbxGlobalSettings,KFbxGlobalSettings);
		
		void FbxGlobalSettings::CopyFrom(FbxGlobalSettings^ settings)
		{
			*_Ref() = *settings->_Ref();			
		}
		FbxAxisSystem^ FbxGlobalSettings::AxisSystem::get()
		{
			return gcnew FbxAxisSystem(_Ref()->GetAxisSystem());
		}
		void FbxGlobalSettings::AxisSystem::set(FbxAxisSystem^ value)
		{
			_Ref()->SetAxisSystem(*value->_Ref());
		}
		FbxSystemUnit^ FbxGlobalSettings::SystemUnit::get()
		{
			return gcnew FbxSystemUnit(_Ref()->GetSystemUnit());
		}
		void FbxGlobalSettings::SystemUnit::set(FbxSystemUnit^ value)
		{
			_Ref()->SetSystemUnit(*value->_Ref());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxGlobalSettings,KFbxGlobalSettings);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 		

	}
}