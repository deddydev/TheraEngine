#pragma once
#include "stdafx.h"
#include "FbxCloneManager.h"
#include "FbxObject.h"



{
	namespace FbxSDK
	{		
		void FbxCloneManager::CollectManagedMemory()
		{			
		}

		/*FbxCloneManager::~FbxCloneManager()
		{				
			this->!FbxCloneManager();
		}
		FbxCloneManager::!FbxCloneManager()
		{			
			if(isNew && manager)
				delete manager;
			manager = nullptr;
			isNew = false;
		}			
		int FbxCloneManager::MaximumCloneDepth::get()
		{
			return KFbxCloneManager::sMaximumCloneDepth;
		}					
		int FbxCloneManager::ConnectToOriginal::get()
		{
			return KFbxCloneManager::sConnectToOriginal;
		}			
		int FbxCloneManager::ConnectToClone::get()
		{
			return KFbxCloneManager::sConnectToClone;
		}									
		FbxCloneManager::CloneSetElement::CloneSetElement(KFbxCloneManager::CloneSetElement* s)
		{
			setClone = s;
			isNew = false;
		}			
		FbxCloneManager::CloneSetElement::CloneSetElement( int srcPolicy,
			int externalDstPolicy,
			FbxObject::CloneType cloneType)
		{
			setClone = new KFbxCloneManager::CloneSetElement(srcPolicy,externalDstPolicy,
				(KFbxObject::ECloneType)cloneType);
			isNew = true;
		}
		FbxCloneManager::CloneSetElement::CloneSetElement()
		{
			setClone = new KFbxCloneManager::CloneSetElement();
			isNew = true;
		}
		FbxCloneManager::CloneSetElement::~CloneSetElement()
		{
			this->!CloneSetElement();
		}
		FbxCloneManager::CloneSetElement::!CloneSetElement()
		{
			if(isNew && setClone)
				delete setClone;
			isNew = false;
			setClone = nullptr;
		}				
		FbxObject::CloneType FbxCloneManager::CloneSetElement::Type::get()
		{
			return (FbxObject::CloneType)setClone->mType;
		}
		void FbxCloneManager::CloneSetElement::Type::set(FbxObject::CloneType value)
		{
			setClone->mType = (KFbxObject::ECloneType)value;
		}								
		int FbxCloneManager::CloneSetElement::SrcPolicy::get()
		{
			return setClone->mSrcPolicy;
		}
		void FbxCloneManager::CloneSetElement::SrcPolicy::set(int value)
		{
			setClone->mSrcPolicy = value;
		}				
		int FbxCloneManager::CloneSetElement::ExternalDstPolicy::get()
		{
			return setClone->mExternalDstPolicy;
		}
		void FbxCloneManager::CloneSetElement::ExternalDstPolicy::set(int value)
		{
			setClone->mExternalDstPolicy = value;
		}				
		FbxObject^ FbxCloneManager::CloneSetElement::ObjectClone::get()
		{
			if(setClone->mObjectClone)
				return gcnew FbxObject(setClone->mObjectClone);
			return nullptr;
		}						
		int FbxCloneManager::FbxObjectCompare::Compare(FbxObject^ obj1,FbxObject^ obj2)
		{
			KFbxCloneManager::KFbxObjectCompare cmp;
			return cmp((KFbxObject*)obj1->emitter,(KFbxObject*)obj2->emitter);
		}			
		FbxCloneManager::FbxCloneManager()
		{
			manager = new KFbxCloneManager();
			isNew = true;
		}*/
	}
}