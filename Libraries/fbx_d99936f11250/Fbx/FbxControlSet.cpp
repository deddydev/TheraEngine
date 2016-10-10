#pragma once
#include "stdafx.h"
#include "FbxControlSet.h"
#include "FbxString.h"
#include "FbxNode.h"
#include "FbxError.h"
#include "FbxCharacter.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"



{
	namespace FbxSDK
	{							
		void FbxControlSetLink::CollectManagedMemory()
		{			
			_Node = nullptr;
		}		

		FbxControlSetLink::FbxControlSetLink(FbxControlSetLink^ controlSetLink)
		{
			_SetPointer(new KFbxControlSetLink(*controlSetLink->_Ref()),true);			
		}			
		void FbxControlSetLink::CopyFrom(FbxControlSetLink^ controlSetLink)
		{
			*this->_Ref() = *controlSetLink->_Ref();
		}
		void FbxControlSetLink::Reset()
		{
			_Ref()->Reset();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxControlSetLink,KFbxNode,mNode,FbxNode,Node);		
		void FbxControlSetLink::Node::set(FbxNode^ value)
		{
			_Node = value;
			if(value)				
				_Ref()->mNode = _Node->_Ref();
			else
				_Ref()->mNode = nullptr;
		}			
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxControlSetLink,mTemplateName,FbxStringManaged,TemplateName);


		void FbxEffector::CollectManagedMemory()
		{
			_Node = nullptr;
		}

		void FbxEffector::CopyFrom(FbxEffector^ e)
		{
			*_Ref() = *e->_Ref();
		}
		void FbxEffector::Reset()
		{
			_Ref()->Reset();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxEffector,KFbxNode,mNode,FbxNode,Node);		
		void FbxEffector::Node::set(FbxNode^ value)
		{
			_Node = value;
			if(value)				
				_Ref()->mNode = _Node->_Ref();
			else
				_Ref()->mNode = nullptr;
		}		

		VALUE_PROPERTY_GETSET_DEFINATION(FbxEffector,mShow,bool,Show);					

#ifndef DOXYGEN_SHOULD_SKIP_THIS					
		VALUE_PROPERTY_GETSET_DEFINATION(FbxEffector,mTActive,bool,TActive);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxEffector,mRActive,bool,RActive);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxEffector,mCandidateTActive,bool,CandidateTActive);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxEffector,mCandidateRActive,bool,CandidateRActive);
#endif		



		void FbxControlSet::CollectManagedMemory()
		{
		}				
				void FbxControlSet::Reset()
				{
					_Ref()->Reset();
				}
				FbxControlSet::ControlSetType FbxControlSet::Type::get()
				{
					return (ControlSetType)_Ref()->GetType();
				}
				void FbxControlSet::Type::set(ControlSetType value)
				{
					_Ref()->SetType((KFbxControlSet::EType)value);
				}			
				bool FbxControlSet::UseAxis::get()
				{
					return _Ref()->GetUseAxis();
				}
				void FbxControlSet::UseAxis::set(bool value)
				{
					_Ref()->SetUseAxis(value);
				}			
				bool FbxControlSet::LockTransform::get()
				{
					return _Ref()->GetLockTransform();
				}
				void FbxControlSet::LockTransform::set(bool value)
				{
					_Ref()->SetLockTransform(value);
				}																								
				bool FbxControlSet::Lock3DPick::get()
				{
					return _Ref()->GetLock3DPick();
				}
				void FbxControlSet::Lock3DPick::set(bool value)
				{
					_Ref()->SetLock3DPick(value);
				}			
				FbxControlSet^ FbxControlSet::GetControlSet(FbxCharacter^ ch)
				{				
					return gcnew FbxControlSet(&ch->_Ref()->GetControlSet());				
				}
				bool FbxControlSet::SetControlSetLink(FbxCharacterNodeId characterNodeId, FbxControlSetLink^ controlSetLink)
				{
					return _Ref()->SetControlSetLink((ECharacterNodeId)characterNodeId,*controlSetLink->_Ref());
				}
				bool FbxControlSet::GetControlSetLink(FbxCharacterNodeId characterNodeId, FbxControlSetLink^ controlSetLink)
				{
					return _Ref()->GetControlSetLink((ECharacterNodeId)characterNodeId,controlSetLink->_Ref());
				}			
				bool FbxControlSet::SetEffector(FbxEffectorNodeId effectorNodeId, FbxEffector^ effector)
				{
					return _Ref()->SetEffector((EEffectorNodeId)effectorNodeId, *effector->_Ref());
				}
				bool FbxControlSet::GetEffector(FbxEffectorNodeId effectorNodeId, FbxEffector^ effector)
				{
					return _Ref()->GetEffector((EEffectorNodeId)effectorNodeId, effector->_Ref());
				}
				bool FbxControlSet::SetEffectorAux(FbxEffectorNodeId effectorNodeId, FbxNode^ node,FbxEffectorSetId effectorSetId)
				{
					return _Ref()->SetEffectorAux((EEffectorNodeId)effectorNodeId,node->_Ref(),(EEffectorSetId)effectorSetId);
				}
				System::String^ FbxControlSet::GetEffectorNodeName(FbxEffectorNodeId effectorNodeId)
				{
					return gcnew System::String(KFbxControlSet::GetEffectorNodeName((EEffectorNodeId)effectorNodeId));
				}
				FbxEffectorNodeId FbxControlSet::GetEffectorNodeId(System::String^ effectorNodeName)
				{
					STRINGTOCHAR_ANSI(e,effectorNodeName);										
					FbxEffectorNodeId i = (FbxEffectorNodeId) KFbxControlSet::GetEffectorNodeId(e);
					FREECHARPOINTER(e);
					return i;
				}	

#ifndef DOXYGEN_SHOULD_SKIP_THIS			

				void FbxControlSet::FromPlug(FbxControlSetPlug^ plug)
				{
					_Ref()->FromPlug(plug->_Ref());
				}
				void FbxControlSet::ToPlug(FbxControlSetPlug^ plug)
				{
					_Ref()->ToPlug(plug->_Ref());
				}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		
		
				FBXOBJECT_DEFINITION(FbxControlSetPlug,KFbxControlSetPlug);

				void FbxControlSetPlug::CollectManagedMemory()
				{
					_KError = nullptr;
					FbxTakeNodeContainer::CollectManagedMemory();
				}				
				FbxControlSet::ControlSetType FbxControlSetPlug::ControlSet_Type::get()
				{
					return (FbxControlSet::ControlSetType)_Ref()->ControlSetType.Get();
				}
				void FbxControlSetPlug::ControlSet_Type::set(FbxControlSet::ControlSetType value)
				{
					_Ref()->ControlSetType.Set((KFbxControlSet::EType)value);
				}

				bool FbxControlSetPlug::UseAxis::get()
				{
					return _Ref()->UseAxis.Get();
				}
				void FbxControlSetPlug::UseAxis::set(bool value)
				{
					_Ref()->UseAxis.Set(value);
				}

				REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxControlSetPlug,GetError(),FbxErrorManaged,KError);				
		
				FbxControlSetPlug::Error FbxControlSetPlug::LastErrorID::get()
				{
					return (Error)_Ref()->GetLastErrorID();
				}				
				System::String^ FbxControlSetPlug::LastErrorString::get()
				{
					return gcnew System::String(_Ref()->GetLastErrorString());
				}				
				CLONE_DEFINITION(FbxControlSetPlug,KFbxControlSetPlug);			

	}
}