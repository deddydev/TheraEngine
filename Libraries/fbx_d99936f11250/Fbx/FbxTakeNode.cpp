#pragma once
#include "stdafx.h"
#include "FbxTakeNode.h"
#include "FbxCurve.h"
#include "FbxCurveNode.h"
#include "FbxError.h"
#include "FbxTime.h"
#include "FbxVector4.h"

namespace Skill
{
	namespace FbxSDK
	{	
		void FbxTakeNode::CollectManagedMemory()
		{
			_CameraOpticalCenterX = nullptr;
			_KError = nullptr;
			_KFCurveNode = nullptr;
		}
		
		FbxTakeNode::FbxTakeNode(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			_SetPointer(new KFbxTakeNode(n),true);
			FREECHARPOINTER(n);
		}
		FbxTakeNode::FbxTakeNode(FbxTakeNode^ takeNode)
		{
			_SetPointer(new KFbxTakeNode(*takeNode->_Ref()),true);
		}

		String^ FbxTakeNode::Name::get()
		{
			return gcnew String(_Ref()->GetName());
		}
		void FbxTakeNode::Name::set(String^ value)
		{
			STRINGTOCHAR_ANSI(v,value);
			_Ref()->SetName(v);
			FREECHARPOINTER(v);			
		}
		FbxCurveNode^ FbxTakeNode::KFCurveNode::get()
		{
			fbxsdk_200901::KFCurveNode* k = _Ref()->GetKFCurveNode();
			if(k)
			{
				if(_KFCurveNode)
					_KFCurveNode->_SetPointer(k,false);
				else
					_KFCurveNode = gcnew FbxCurveNode(k);
				return _KFCurveNode;
			}
			return nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxTakeNode,KFCurve,GetCameraOpticalCenterX(),FbxCurve,CameraOpticalCenterX);		

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxTakeNode,GetError(),FbxErrorManaged,KError);		

		FbxTakeNode::Error FbxTakeNode::LastErrorID::get()
		{
			return (FbxTakeNode::Error)_Ref()->GetLastErrorID();
		}
		String^ FbxTakeNode::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}

		bool FbxTakeNode::GetAnimationInterval(FbxTime^ start, FbxTime^ stop)
		{
			return _Ref()->GetAnimationInterval(*start->_Ref(),*stop->_Ref());
		}
		bool FbxTakeNode::AddRotationToTranslation(FbxVector4^ rotation)
		{
			return _Ref()->AddRotationToTranslation(*rotation->_Ref());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		bool FbxTakeNode::IsChannelAnimated(String^ group, String^ subGroup,String^ name)
		{
			STRINGTOCHAR_ANSI(g,group);
			STRINGTOCHAR_ANSI(s,subGroup);
			STRINGTOCHAR_ANSI(n,name);
			bool b = _Ref()->IsChannelAnimated(g,s,n);
			FREECHARPOINTER(g);
			FREECHARPOINTER(s);
			FREECHARPOINTER(n);
			return b;
		}
		/*bool FbxTakeNode::IsChannelAnimated(String^ group,String^ subGroup, FbxDataType^ dataType)
		{			
			STRINGTOCHAR_ANSI(g,group);
			STRINGTOCHAR_ANSI(s,subGroup);			
			bool b = _Ref()->IsChannelAnimated(g,s,dataType->_Ref());
			FREECHARPOINTER(g);
			FREECHARPOINTER(s);			
			return b;
		}*/

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}