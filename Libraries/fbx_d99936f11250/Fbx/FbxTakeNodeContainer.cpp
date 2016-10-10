#pragma once
#include "stdafx.h"
#include "FbxTakeNodeContainer.h"
#include "FbxTakeNode.h"
#include "FbxTime.h"
#include "FbxProperty.h"


{
	namespace FbxSDK
	{		

		FbxTakeNode^ FbxTakeNodeContainer::CreateTakeNode(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			KFbxTakeNode* tn = _Ref()->CreateTakeNode(n);
			FREECHARPOINTER(n);
			if(tn)
				return gcnew FbxTakeNode(tn);
			return nullptr;				
		}		
		bool FbxTakeNodeContainer::RemoveTakeNode(int index)
		{
			return _Ref()->RemoveTakeNode(index);
		}
		bool FbxTakeNodeContainer::RemoveTakeNode(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			bool b =_Ref()->RemoveTakeNode(n);
			FREECHARPOINTER(n);
			return b;
		}
		int FbxTakeNodeContainer::TakeNodeCount::get()
		{
			return _Ref()->GetTakeNodeCount();
		}
		String^ FbxTakeNodeContainer::GetTakeNodeName(int index)
		{
			char* n = _Ref()->GetTakeNodeName(index);
			if(n)
				return gcnew String(n);
			return nullptr;
		}
		bool FbxTakeNodeContainer::SetCurrentTakeNode(int index)
		{
			return _Ref()->SetCurrentTakeNode(index);
		}
		bool FbxTakeNodeContainer::SetCurrentTakeNode(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			bool b =_Ref()->SetCurrentTakeNode(n);
			FREECHARPOINTER(n);
			return b;
		}
		String^ FbxTakeNodeContainer::CurrentTakeNodeName::get()
		{
			return gcnew String(_Ref()->GetCurrentTakeNodeName());
		}
		int FbxTakeNodeContainer::CurrentTakeNodeIndex::get()
		{
			return _Ref()->GetCurrentTakeNodeIndex();
		}
		bool FbxTakeNodeContainer::GetAnimationInterval(FbxTime^ start, FbxTime^ stop)
		{
			return _Ref()->GetAnimationInterval(*start->_Ref(),*stop->_Ref());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS			

		void FbxTakeNodeContainer::Init()
		{
			_Ref()->Init();
		}
		void FbxTakeNodeContainer::Reset()
		{
			_Ref()->Reset();
		}
		void FbxTakeNodeContainer::PropertyAdded(FbxPropertyManaged^ prop)
		{
			_Ref()->PropertyAdded(prop->_Ref());
		}
		void FbxTakeNodeContainer::PropertyRemoved(FbxPropertyManaged^ prop)
		{
			_Ref()->PropertyRemoved(prop->_Ref());
		}

		void FbxTakeNodeContainer::UpdateFCurveFromProperty(FbxPropertyManaged^ prop,FbxTakeNode^ takeNode)
		{
			_Ref()->UpdateFCurveFromProperty(*prop->_Ref(),(takeNode)?takeNode->_Ref() : NULL);
		}
		void FbxTakeNodeContainer::CreateChannelsForProperty(FbxPropertyManaged^ prop,FbxTakeNode^ takeNode)
		{
			_Ref()->CreateChannelsForProperty(prop->_Ref(),(takeNode)?takeNode->_Ref() : NULL);
		}
		void FbxTakeNodeContainer::UnregisterAllDefaultTakeCallback()
		{
			_Ref()->UnregisterAllDefaultTakeCallback();
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		
	}
}