#pragma once
#include "stdafx.h"
#include "FbxStatics.h"
#include "FbxObject.h"
#include "FbxClassID.h"
#include "FbxObjectMetaData.h"
#include "FbxMesh.h"



#define DEFINITION_SRCDST_TEMPLATES(Class,NativeClass)\
	bool FbxStatics::_##Class::FbxDisconnectAllSrc(FbxObject^ obj){return KFbxDisconnectAllSrc<NativeClass>(obj->_Ref());}\
	int FbxStatics::_##Class::FbxGetSrcCount(FbxObject^ obj){	return KFbxGetSrcCount<NativeClass>(obj->_Ref());}\
	int FbxStatics::_##Class::FbxGetSrcCount(FbxObject^ obj,FbxClassId^ classId){return KFbxGetSrcCount<NativeClass>(obj->_Ref(),*classId->_Ref());}\
	Class^ FbxStatics::_##Class::FbxGetSrc(FbxObject^ obj,int index){	NativeClass* d = KFbxGetSrc<NativeClass>(obj->_Ref(),index);if(d) return gcnew Class(d); return nullptr;}\
	Class^ FbxStatics::_##Class::FbxGetSrc(FbxObject^ obj,int index,FbxClassId^ classId){	NativeClass* d = KFbxGetSrc<NativeClass>(obj->_Ref(),index,*classId->_Ref());if(d) return gcnew Class(d); return nullptr;}\
	Class^ FbxStatics::_##Class::FbxFindSrc(FbxObject^ obj,String^ name ,int index){STRINGTO_CONSTCHAR_ANSI(n,name);NativeClass* d = KFbxFindSrc<NativeClass>(obj->_Ref(),n,index);FREECHARPOINTER(n);if(d) return gcnew Class(d);return nullptr;}\
	Class^ FbxStatics::_##Class::FbxFindSrc(FbxObject^ obj,String^ name,FbxClassId^ classId ,int index){STRINGTO_CONSTCHAR_ANSI(n,name);NativeClass* d = KFbxFindSrc<NativeClass>(obj->_Ref(),n,*classId->_Ref(),index);FREECHARPOINTER(n);if(d) return gcnew Class(d);return nullptr;}\
	int FbxStatics::_##Class::FbxGetDstCount(FbxObject^ obj){return KFbxGetDstCount<NativeClass>(obj->_Ref());}\
	int FbxStatics::_##Class::FbxGetDstCount(FbxObject^ obj,FbxClassId^ classId){return KFbxGetDstCount<NativeClass>(obj->_Ref(),*classId->_Ref());}\
	Class^ FbxStatics::_##Class::FbxGetDst(FbxObject^ obj,int index){	NativeClass* d = KFbxGetDst<NativeClass>(obj->_Ref(),index);if(d) return gcnew Class(d);return nullptr;}\
	Class^ FbxStatics::_##Class::FbxGetDst(FbxObject^ obj,int index,FbxClassId^ classId){NativeClass* d = KFbxGetDst<NativeClass>(obj->_Ref(),index,*classId->_Ref());if(d)return gcnew Class(d);	return nullptr;}


namespace Skill
{
	namespace FbxSDK
	{		
		DEFINITION_SRCDST_TEMPLATES(FbxObjectMetaData,KFbxObjectMetaData);
		DEFINITION_SRCDST_TEMPLATES(FbxMesh,KFbxMesh);


		bool FbxStatics::FbxConnectSrc(FbxObjectManaged^ dstObject,FbxObjectManaged^ srcObject)
		{
			return KFbxConnectSrc(dstObject->_Ref(),srcObject->_Ref());
		}
		bool FbxStatics::FbxConnectDst(FbxObjectManaged^ srcObject,FbxObjectManaged^ dstObject)
		{
			return KFbxConnectDst(srcObject->_Ref(),dstObject->_Ref());
		}
	}
}