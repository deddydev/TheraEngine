#pragma once
#include "stdafx.h"
#include "FbxTakeInfo.h"
#include "FbxString.h"
#include "FbxTime.h"
#include "FbxThumbnail.h"
#include "FbxLayerInfoRefArray.h"


#define REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE_2(Class,NativePtr,PropType,PropName)\
	PropType^ Class::PropName::get() {\
	if(_##PropName) _##PropName->_SetPointer(&_Ref()->NativePtr,false);\
	else _##PropName = gcnew PropType(&_Ref()->NativePtr);return _##PropName;}\
	void Class::PropName::set(PropType^ value){_Ref()->NativePtr = *value->_Ref();}

namespace Skill
{
	namespace FbxSDK
	{	

		void FbxLayerInfo::CollectManagedMemory()
		{
			_Name = nullptr;
		}		
		FbxLayerInfo::FbxLayerInfo(FbxLayerInfo^ layerInfo)
		{
			_SetPointer(new KLayerInfo(*layerInfo->_Ref()),true);			
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE_2(FbxLayerInfo,mName,FbxStringManaged,Name);

		VALUE_PROPERTY_GETSET_DEFINATION(FbxLayerInfo,mId,int,ID);

		void FbxTakeInfo::CollectManagedMemory()
		{
			this->_Name = nullptr;
			this->_ImportName= nullptr;
			this->_Description =nullptr;
			this->_LocalTimeSpan = nullptr;
			this->_ReferenceTimeSpan = nullptr;
			this->_ImportOffset = nullptr;
			this->_TakeThumbnail = nullptr;
			this->_LayerInfoList = nullptr;
		}		
		FbxTakeInfo::FbxTakeInfo(FbxTakeInfo^ takeInfo)
		{
			_SetPointer(new KFbxTakeInfo(*takeInfo->_Ref()),true);			
		}
		void FbxTakeInfo::CopyFrom(FbxTakeInfo^ takeInfo)
		{
			*this->_FbxTakeInfo = *takeInfo->_Ref();
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE_2(FbxTakeInfo,mName,FbxStringManaged,Name);		
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE_2(FbxTakeInfo,mImportName,FbxStringManaged,ImportName);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE_2(FbxTakeInfo,mDescription,FbxStringManaged,Description);				

		VALUE_PROPERTY_GETSET_DEFINATION(FbxTakeInfo,mSelect,bool,Select);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTakeInfo,mLocalTimeSpan,FbxTimeSpan,LocalTimeSpan);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTakeInfo,mReferenceTimeSpan,FbxTimeSpan,ReferenceTimeSpan);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTakeInfo,mImportOffset,FbxTime,ImportOffset);

		FbxTakeInfo::TakeInfoImportOffsetType FbxTakeInfo::ImportOffsetType::get()
		{
			return (TakeInfoImportOffsetType)_Ref()->mImportOffsetType;			
		}
		void FbxTakeInfo::ImportOffsetType::set(TakeInfoImportOffsetType value)
		{
			_Ref()->mImportOffsetType = (KFbxTakeInfo::EImportOffsetType)value;
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxTakeInfo,KFbxThumbnail,GetTakeThumbnail(),FbxThumbnail,TakeThumbnail);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxTakeInfo,SetTakeThumbnail,FbxThumbnail,TakeThumbnail);
		
		void FbxTakeInfo::CopyLayers(FbxTakeInfo^ takeInfo)
		{
			_Ref()->CopyLayers(*takeInfo->_Ref());			
		}		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTakeInfo,mLayerInfoList,Skill::FbxSDK::Arrays::FbxLayerInfoRefArray,LayerInfoList);		

		VALUE_PROPERTY_GETSET_DEFINATION(FbxTakeInfo,mCurrentLayer,int,CurrentLayer);		
	}
}