#pragma once
#include "stdafx.h"
#include "FbxNodeIterator.h"
#include "FbxNode.h"


{
	namespace FbxSDK
	{		

		void FbxNodeIterator::CollectManagedMemory()
		{
			_Current = nullptr;
			_Next = nullptr;
			_Previous = nullptr;
		}		
		FbxNodeIterator::FbxNodeIterator(FbxNode^ rootNode, FbxNodeIterator::TraversalType type)
		{
			_SetPointer(new KFbxNodeIterator(rootNode->_Ref(),(KFbxNodeIterator::TraversalType)type),true);
		}
		FbxNodeIterator::FbxNodeIterator(FbxNodeIterator^ copy)
		{
			_SetPointer(new KFbxNodeIterator(*copy->_Ref()),true);
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNodeIterator,KFbxNode,Get(),FbxNode,Current);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNodeIterator,KFbxNode,Next(),FbxNode,Next);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNodeIterator,KFbxNode,Prev(),FbxNode,Previous);		

		void FbxNodeIterator::Reset()
		{
			_Ref()->Reset();
		}
	}
}