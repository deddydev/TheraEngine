#pragma once
#include "stdafx.h"
#include "FbxCluster.h"
#include "FbxSubDeformer.h"
#include "FbxNode.h"
#include "FbxString.h"
#include "FbxXMatrix.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{	

		FBXOBJECT_DEFINITION(FbxCluster,KFbxCluster);

		void FbxCluster::CollectManagedMemory()
		{
			_Link = nullptr;
			_AssociateModel = nullptr;
			_TransformAssociateModelMatrix = nullptr;
			_TransformLinkMatrix = nullptr;
			_TransformMatrix = nullptr;
			_TransformParentMatrix = nullptr;
			FbxSubDeformer::CollectManagedMemory();
		}		


		void FbxCluster::Reset()
		{
			_Ref()->Reset();
		}




		FbxCluster::LinkMode FbxCluster::Mode::get()				
		{
			return (FbxCluster::LinkMode)_Ref()->GetLinkMode();
		}
		void FbxCluster::Mode::set(FbxCluster::LinkMode value)
		{
			_Ref()->SetLinkMode((KFbxCluster::ELinkMode)value);
		}


		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCluster,KFbxNode,GetLink(),FbxNode,Link);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxCluster,SetLink,FbxNode,Link);				


		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCluster,KFbxNode,GetAssociateModel(),FbxNode,AssociateModel);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxCluster,SetAssociateModel,FbxNode,AssociateModel);		


		void FbxCluster::AddControlPointIndex(int index, double weight)
		{
			_Ref()->AddControlPointIndex(index,weight);
		}


		int FbxCluster::ControlPointIndicesCount::get()
		{
			return _Ref()->GetControlPointIndicesCount();
		}

		IntPtr FbxCluster::GetControlPointIndices()
		{
			return IntPtr(_Ref()->GetControlPointIndices());
		}
		array<int>^ FbxCluster::GetControlPointIndicesArray()
		{
			if(ControlPointIndicesCount > 0)
			{
				int* arr = _Ref()->GetControlPointIndices();
				array<int>^ Arr = gcnew array<int>(ControlPointIndicesCount);
				for(int i=0;i<Arr->Length;i++)
				{
					Arr[i] = arr[i];
				}
				return Arr;
			}
			return nullptr;
			
		}
		IntPtr FbxCluster::GetControlPointWeights()
		{
			return IntPtr(_Ref()->GetControlPointWeights());
		}
		array<double>^ FbxCluster::GetControlPointWeightsArray()
		{
			if(ControlPointIndicesCount > 0)
			{
				double* arr = _Ref()->GetControlPointWeights();
				array<double>^ Arr = gcnew array<double>(ControlPointIndicesCount);
				for(int i=0;i<Arr->Length;i++)
				{
					Arr[i] = arr[i];
				}
				return Arr;
			}
			return nullptr;
		}
			
		void FbxCluster::TransformMatrix::set(FbxXMatrix^ value)
		{
			if(value)
			{
				_Ref()->SetTransformMatrix(*value->_Ref());
				if(_TransformMatrix)
				*_TransformMatrix->_Ref() = *value->_Ref();
			}
		}
		FbxXMatrix^ FbxCluster::TransformMatrix::get()
		{
			if(!_TransformMatrix)
				_TransformMatrix = gcnew FbxXMatrix();
			_Ref()->GetTransformMatrix(*_TransformMatrix->_Ref());
			return _TransformMatrix;
		}


		void FbxCluster::TransformLinkMatrix::set(FbxXMatrix^ value)
		{
			if(value)
			{
				_Ref()->SetTransformLinkMatrix(*value->_Ref());
				if(_TransformLinkMatrix)
					*_TransformLinkMatrix->_Ref() = *value->_Ref();
			}
		}
		FbxXMatrix^ FbxCluster::TransformLinkMatrix::get()
		{
			if(!_TransformLinkMatrix)
				_TransformLinkMatrix = gcnew FbxXMatrix();
			_Ref()->GetTransformLinkMatrix(*_TransformLinkMatrix->_Ref());
			return _TransformLinkMatrix;
		}		

		void FbxCluster::TransformAssociateModelMatrix::set(FbxXMatrix^ value)
		{
			if(value)
			{
				_Ref()->SetTransformAssociateModelMatrix(*value->_Ref());
				if(_TransformAssociateModelMatrix)
					*_TransformAssociateModelMatrix->_Ref() = *value->_Ref();
			}
		}
		FbxXMatrix^ FbxCluster::TransformAssociateModelMatrix::get()
		{
			if(!_TransformAssociateModelMatrix)
				_TransformAssociateModelMatrix = gcnew FbxXMatrix();
			_Ref()->GetTransformAssociateModelMatrix(*_TransformAssociateModelMatrix->_Ref());
			return _TransformAssociateModelMatrix;
		}		
		
		void FbxCluster::TransformParentMatrix::set(FbxXMatrix^ value)
		{
			if(value)
			{
				_Ref()->SetTransformParentMatrix(*value->_Ref());
				if(_TransformParentMatrix)
					*_TransformParentMatrix->_Ref() = *value->_Ref();
			}
		}
		FbxXMatrix^ FbxCluster::TransformParentMatrix::get()
		{
			if(!_TransformParentMatrix)
				_TransformParentMatrix = gcnew FbxXMatrix();
			_Ref()->GetTransformParentMatrix(*_TransformParentMatrix->_Ref());
			return _TransformParentMatrix;
		}		

		bool FbxCluster::IsTransformParentSet::get()
		{
			return	_Ref()->IsTransformParentSet();
		}



		//			//@}
		//
		//			//!Assigment operator
		//			//KFbxCluster& operator=(KFbxCluster const& pCluster);
		//
		//			///////////////////////////////////////////////////////////////////////////////
		//			//
		//			//  WARNING!
		//			//
		//			//  Anything beyond these lines may not be documented accurately and is
		//			//  subject to change without notice.
		//			//
		//			///////////////////////////////////////////////////////////////////////////////
		//
#ifndef DOXYGEN_SHOULD_SKIP_THIS

		// Clone
		CLONE_DEFINITION(FbxCluster,KFbxCluster);


		void FbxCluster::SetUserData(FbxStringManaged^ userDataID, FbxStringManaged^ userData)
		{
			_Ref()->SetUserData(*userDataID->_Ref(),*userData->_Ref());

		}



		FbxStringManaged^ FbxCluster::UserDataID::get()
		{
			return gcnew FbxStringManaged(_Ref()->GetUserDataID());
		}

		FbxStringManaged^ FbxCluster::UserData::get()
		{
			return gcnew FbxStringManaged(_Ref()->GetUserData());
		}


		//! Get the user data by identifier.
		FbxStringManaged^ FbxCluster::GetUserData (FbxStringManaged^ userDataID)
		{
			return gcnew FbxStringManaged(_Ref()->GetUserData(*userDataID->_Ref()));
		}



#endif 


	}
}