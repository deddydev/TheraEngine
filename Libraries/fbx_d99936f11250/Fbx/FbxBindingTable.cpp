#pragma once
#include "stdafx.h"
#include "FbxBindingTable.h"
#include "FbxTypedProperty.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


{
	namespace FbxSDK
	{		

		void FbxBindingTable::CollectManagedMemory()
		{
			_CodeAbsoluteURL = nullptr;
			_CodeRelativeURL = nullptr;
			_CodeTAG = nullptr;
			_DescAbsoluteURL = nullptr;
			_DescRelativeURL = nullptr;
			_DescTAG = nullptr;
			_TargetName = nullptr;
			_TargetType = nullptr;

			FbxBindingTableBase::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxBindingTable,KFbxBindingTable);
		void FbxBindingTable::CopyFrom(FbxBindingTable^ table)
		{
			*this->_Ref() = *table->_Ref();			
		}			

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,TargetName,FbxStringTypedProperty,TargetName);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,TargetType,FbxStringTypedProperty,TargetType);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,DescRelativeURL,FbxStringTypedProperty,DescRelativeURL);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,DescAbsoluteURL,FbxStringTypedProperty,DescAbsoluteURL);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,DescTAG,FbxStringTypedProperty,DescTAG);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,CodeRelativeURL,FbxStringTypedProperty,CodeRelativeURL);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,CodeAbsoluteURL,FbxStringTypedProperty,CodeAbsoluteURL);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxBindingTable,CodeTAG,FbxStringTypedProperty,CodeTAG);
		
		/*System::String^ FbxBindingTable::STargetName::get()
		{
			if(sTargetName == nullptr)
				sTargetName = gcnew System::String(KFbxBindingTable::sTargetName);						
			return sTargetName;
		}


		System::String^ FbxBindingTable::STargetType::get()
		{
			if(sTargetType == nullptr)
				sTargetType = gcnew System::String(KFbxBindingTable::sTargetType);						
			return sTargetType;
		}

		System::String^ FbxBindingTable::SDescRelativeURL::get()
		{
			if(sDescRelativeURL == nullptr)
				sDescRelativeURL = gcnew System::String(KFbxBindingTable::sDescRelativeURL);						
			return sDescRelativeURL;
		}

		System::String^ FbxBindingTable::SDescAbsoluteURL::get()
		{
			if(sDescAbsoluteURL == nullptr)
				sDescAbsoluteURL = gcnew System::String(KFbxBindingTable::sDescAbsoluteURL);						
			return sDescAbsoluteURL;
		}			

		System::String^ FbxBindingTable::SDescTAG::get()
		{
			if(sDescTAG == nullptr)
				sDescTAG = gcnew System::String(KFbxBindingTable::sDescTAG);						
			return sDescTAG;
		}
		System::String^ FbxBindingTable::SCodeRelativeURL::get()
		{
			if(sCodeRelativeURL == nullptr)
				sCodeRelativeURL = gcnew System::String(KFbxBindingTable::sCodeRelativeURL);						
			return sCodeRelativeURL;
		}

		System::String^ FbxBindingTable::SCodeAbsoluteURL::get()
		{
			if(sCodeAbsoluteURL == nullptr)
				sCodeAbsoluteURL = gcnew System::String(KFbxBindingTable::sCodeAbsoluteURL);						
			return sCodeAbsoluteURL;
		}						
		System::String^ FbxBindingTable::SCodeTAG::get()
		{
			if(sCodeTAG == nullptr)
				sCodeTAG = gcnew System::String(KFbxBindingTable::sCodeTAG);						
			return sCodeTAG;
		}			
		System::String^ FbxBindingTable::DefaultTargetName::get()
		{
			if(sDefaultTargetName == nullptr)
				sDefaultTargetName = gcnew System::String(KFbxBindingTable::sDefaultTargetName);						
			return sDefaultTargetName;
		}			

		System::String^ FbxBindingTable::DefaultTargetType::get()
		{
			if(sDefaultTargetType == nullptr)
				sDefaultTargetType = gcnew System::String(KFbxBindingTable::sDefaultTargetType);						
			return sDefaultTargetType;
		}			

		System::String^ FbxBindingTable::DefaultDescRelativeURL::get()
		{
			if(sDefaultDescRelativeURL == nullptr)
				sDefaultDescRelativeURL = gcnew System::String(KFbxBindingTable::sDefaultDescRelativeURL);						
			return sDefaultDescRelativeURL;
		}			

		System::String^ FbxBindingTable::DefaultDescAbsoluteURL::get()
		{
			if(sDefaultDescAbsoluteURL == nullptr)
				sDefaultDescAbsoluteURL = gcnew System::String(KFbxBindingTable::sDefaultDescAbsoluteURL);						
			return sDefaultDescAbsoluteURL;
		}			

		System::String^ FbxBindingTable::DefaultDescTAG::get()
		{
			if(sDefaultDescTAG == nullptr)
				sDefaultDescTAG = gcnew System::String(KFbxBindingTable::sDefaultDescTAG);						
			return sDefaultDescTAG;
		}			

		System::String^ FbxBindingTable::DefaultCodeRelativeURL::get()
		{
			if(sDefaultCodeRelativeURL == nullptr)
				sDefaultCodeRelativeURL = gcnew System::String(KFbxBindingTable::sDefaultCodeRelativeURL);						
			return sDefaultCodeRelativeURL;
		}			

		System::String^ FbxBindingTable::DefaultCodeAbsoluteURL::get()
		{
			if(sDefaultCodeAbsoluteURL == nullptr)
				sDefaultCodeAbsoluteURL = gcnew System::String(KFbxBindingTable::sDefaultCodeAbsoluteURL);						
			return sDefaultCodeAbsoluteURL;
		}			
		System::String^ FbxBindingTable::DefaultCodeTAG::get()
		{
			if(sDefaultCodeTAG == nullptr)
				sDefaultCodeTAG = gcnew System::String(KFbxBindingTable::sDefaultCodeTAG);						
			return sDefaultCodeTAG;
		}		*/

	}
}