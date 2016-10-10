#pragma once
#include "stdafx.h"
#include "FbxSkin.h"
#include "FbxClassID.h"
#include "FbxSdkManager.h"
#include "FbxNode.h"
#include "FbxGeometry.h"
#include "FbxCluster.h"


{
	namespace FbxSDK
	{	
		FBXOBJECT_DEFINITION(FbxSkin,KFbxSkin);

		void FbxSkin::CollectManagedMemory()
		{
			if(_list)
				_list->Clear();
			_list = nullptr;
			FbxDeformer::CollectManagedMemory();
		}
		FbxSkin::FbxSkin(KFbxSkin* instance) : FbxDeformer(instance)
		{
			_Free = false;
			_list = gcnew System::Collections::Generic::List<FbxCluster^>();
			for(int i=0;i<_Ref()->GetClusterCount();i++)
			{
				KFbxCluster* c = _Ref()->GetCluster(i);
				if(c)
					_list->Add(gcnew FbxCluster(c));
				else
					_list->Add(nullptr);
			}
		}

		double FbxSkin::DeformAccuracy::get()
		{
			return _Ref()->GetDeformAccuracy();
		}
		void FbxSkin::DeformAccuracy::set(double value)
		{
			_Ref()->SetDeformAccuracy(value);
		}

		//REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSkin,KFbxNode,GetNode(),FbxNode,Node);
		//REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxSkin,SetNode,FbxNode,Node);

		
/*		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSkin,KFbxGeometry,GetGeometry(),FbxGeometry,Geometry);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxSkin,SetGeometry,FbxGeometry,Geometry);	*/	

		bool FbxSkin::AddCluster(FbxCluster^ cluster)
		{
			if(Disposed)
				return false;
			if(!_list->Contains(cluster))
			{
				bool b = _Ref()->AddCluster(cluster->_Ref());
				if(b)
					_list->Add(cluster);
				return b;
			}
			return false;			
		}
		FbxCluster^ FbxSkin::RemoveCluster(FbxCluster^ cluster)
		{
			if(Disposed)
				return nullptr;
			if(!_list->Contains(cluster))
			{
				if(_Ref()->RemoveCluster(cluster->_Ref()))
				{
					_list->Remove(cluster);
					return cluster;
				}
			}
			return nullptr;
		}

		
		int FbxSkin::ClusterCount::get()
		{
			return _Ref()->GetClusterCount();
		}

		FbxCluster^ FbxSkin::GetCluster(int index)
		{
			return _list[index];
		}		

#ifndef DOXYGEN_SHOULD_SKIP_THIS		

		CLONE_DEFINITION(FbxSkin,KFbxSkin);

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}

}