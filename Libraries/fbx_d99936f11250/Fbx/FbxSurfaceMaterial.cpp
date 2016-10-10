#pragma once
#include "stdafx.h"
#include "FbxSurfaceMaterial.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"
#include "FbxTypedProperty.h"


{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxSurfaceMaterial,KFbxSurfaceMaterial);		

		String^ FbxSurfaceMaterial::ShadingModel::get()
		{
			FbxString kstr = _Ref()->GetShadingModel().Get();
			CONVERT_FbxString_TO_STRING(kstr,str);
			return str;
		}
		void FbxSurfaceMaterial::ShadingModel::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			FbxString kstr(v);						
			_Ref()->GetShadingModel().Set(kstr);
			FREECHARPOINTER(v);
		}

		bool FbxSurfaceMaterial::MultiLayer::get()
		{
			return _Ref()->GetMultiLayer().Get();			
		}
		void FbxSurfaceMaterial::MultiLayer::set(bool value)
		{
			_Ref()->GetMultiLayer().Set(value);
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		// Clone
		CLONE_DEFINITION(FbxSurfaceMaterial,KFbxSurfaceMaterial);		

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS 
	}
}