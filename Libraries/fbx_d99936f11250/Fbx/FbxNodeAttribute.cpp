#pragma once
#include "stdafx.h"
#include "FbxNodeAttribute.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxDouble3.h"
#include "FbxTypedProperty.h"
#include "FbxNode.h"

namespace Skill
{
	namespace FbxSDK
	{
		void FbxNodeAttribute::CollectManagedMemory()
		{
			this->_Color = nullptr; 
			this->_Node = nullptr;
		}

		FbxNodeAttribute::AttributeType FbxNodeAttribute::AttribType::get()
		{
			return (AttributeType)_Ref()->GetAttributeType();
		}
		FBXOBJECT_DEFINITION(FbxNodeAttribute,KFbxNodeAttribute);		

		/*String^ FbxNodeAttribute::ColorName::get()
		{
			return gcnew String(KFbxNodeAttribute::sColor);
		}*/
		/*FbxDouble3^ FbxNodeAttribute::SDefaultColor::get()
		{
			if(_SDefaultColor == nullptr)
				_SDefaultColor = gcnew FbxDouble3(KFbxNodeAttribute::sDefaultColor[0],
				KFbxNodeAttribute::sDefaultColor[1],
				KFbxNodeAttribute::sDefaultColor[2]);			
			return _SDefaultColor;
		}	*/			
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNodeAttribute,Color,FbxDouble3TypedProperty,Color);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNodeAttribute,KFbxNode,GetNode(),FbxNode,Node);		
		CLONE_DEFINITION(FbxNodeAttribute,KFbxNodeAttribute);
	}
}