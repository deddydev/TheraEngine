#pragma once
#include "stdafx.h"
#include "FbxPose.h"
#include "FbxNode.h"
#include "FbxMatrix.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"
#include "FbxName.h"
#include "FbxError.h"

namespace Skill
{
	namespace FbxSDK
	{		
		void FbxPoseInfo::CollectManagedMemory()
		{
			_Matrix = nullptr;
			_Node = nullptr;
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxPoseInfo,mMatrix,FbxMatrix,Matrix);
		VALUE_PROPERTY_GETSET_DEFINATION(FbxPoseInfo,mMatrixIsLocal,bool,MatrixIsLocal);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxPoseInfo,KFbxNode,mNode,FbxNode,Node);
		void FbxPoseInfo::Node::set(FbxNode^ value)
		{
			_Node = value;
			if(_Node)
			{				
				_Ref()->mNode = _Node->_Ref();
			}
			else 
				_Ref()->mNode = nullptr;
		}


		FBXOBJECT_DEFINITION(FbxPose,KFbxPose);

		void FbxPose::CollectManagedMemory()
		{
			if(_List)
				_List->Clear();
			_List = nullptr;
			FbxObjectManaged::CollectManagedMemory();
		}

		FbxPose::FbxPose(KFbxPose* instance) : FbxObjectManaged(instance)
		{
			_Free = false;
			_List = gcnew System::Collections::Generic::List<FbxNode^>();			
		}

		bool FbxPose::IsBindPose::get()
		{
			return _Ref()->IsBindPose();
		}
		void FbxPose::IsBindPose::set(bool value)
		{
			_Ref()->SetIsBindPose(value);
		}

		bool FbxPose::IsRestPose::get()
		{
			return _Ref()->IsRestPose();
		}

		int FbxPose::Count::get()
		{
			return _Ref()->GetCount();
		}

		int FbxPose::Add(FbxNode^ node,FbxMatrix^ matrix, bool localMatrix, bool multipleBindPose)
		{
			if(_List->Contains(node))
				return -1;
			int i = _Ref()->Add(node->_Ref(),*matrix->_Ref(),localMatrix,multipleBindPose);
			_List->Add(node);
			return i;
		}
		int FbxPose::Add(FbxNode^ node,FbxMatrix^ matrix)
		{
			if(_List->Contains(node))
				return -1;
			int i = _Ref()->Add(node->_Ref(),*matrix->_Ref());
			_List->Add(node);
			return i;
		}
		void FbxPose::Remove(int index)
		{
			if(index< 0 || index > _List->Count - 1)
				return;
			_List->RemoveAt(index);
			_Ref()->Remove(index);
		}

		FbxName^ FbxPose::GetNodeName(int index)
		{
			KName n = _Ref()->GetNodeName(index);
			return gcnew FbxName(n);
		}

		FbxNode^ FbxPose::GetNode(int index)		
		{
			if(index< 0 || index>_List->Count-1)
				return nullptr;
			return _List[index];
		}

		FbxMatrix^ FbxPose::GetMatrix(int index)
		{
			KFbxMatrix m = _Ref()->GetMatrix(index);
			return gcnew FbxMatrix(m);
		}

		bool FbxPose::IsLocalMatrix(int index)
		{
			return _Ref()->IsLocalMatrix(index);
		}

		int FbxPose::Find(FbxName^ nodeName, char compareWhat)
		{
			return _Ref()->Find(*nodeName->_Ref(),compareWhat);
		}
		int FbxPose::Find(FbxName^ nodeName)
		{
			return _Ref()->Find(*nodeName->_Ref());
		}

		int FbxPose::Find(FbxNode^ node)
		{
			return _Ref()->Find(node->_Ref());
		}

		bool FbxPose::IsValidBindPose(FbxNode^ root, double matrixCmpTolerance)
		{
			return _Ref()->IsValidBindPose(root->_Ref(),matrixCmpTolerance);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxPose,GetError(),FbxErrorManaged,KError);

		FbxPose::Error FbxPose::LastErrorID::get()
		{
			return (FbxPose::Error)_Ref()->GetLastErrorID();
		}
		String^ FbxPose::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		CLONE_DEFINITION(FbxPose,KFbxPose);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS

	}
}