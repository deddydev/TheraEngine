#pragma once
#include "stdafx.h"
#include "FbxCollection.h"
#include "FbxObject.h"
#include "FbxQuery.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{				

		FBXOBJECT_DEFINITION(FbxCollectionManaged,KFbxCollection);

		void FbxCollectionManaged::Clear()
		{
			_Ref()->Clear();
		}			
		void FbxCollectionManaged::AddMember(FbxObject^ member)
		{
			_Ref()->AddMember(member->_Ref());
		}			
		void FbxCollectionManaged::RemoveMember(FbxObject^ member)
		{
			_Ref()->RemoveMember(member->_Ref());
		}			
		int FbxCollectionManaged::MemberCount::get()
		{
			return _Ref()->GetMemberCount();
		}			
		int FbxCollectionManaged::GetMemberCount(FbxCriteria^ criteria )
		{
			return _Ref()->GetMemberCount(*criteria->_Ref());
		}
		FbxObjectManaged^ FbxCollectionManaged::GetMember(int index)
		{
			return FbxCreator::CreateFbxObject(_Ref()->GetMember(index));
		}			
		FbxObjectManaged^ FbxCollectionManaged::GetMember(FbxCriteria^ criteria,int index)
		{
			return FbxCreator::CreateFbxObject(_Ref()->GetMember(*criteria->_Ref(), index));
		}			
		bool FbxCollectionManaged::IsMember(FbxObject^ member)
		{
			return _Ref()->IsMember(member->_Ref());
		}			
		void FbxCollectionManaged::SetSelectedAll(bool selection)
		{
			_Ref()->SetSelectedAll(selection);
		}			
		void FbxCollectionManaged::SetSelected(FbxObject^ obj,bool selection)
		{
			_Ref()->SetSelected(obj->_Ref(),selection);
		}			
		bool FbxCollectionManaged::GetSelected(FbxObject^ selection)
		{
			return _Ref()->GetSelected(selection->_Ref());
		}		


	}
}