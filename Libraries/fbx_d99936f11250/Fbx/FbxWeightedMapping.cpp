#pragma once
#include "stdafx.h"
#include "FbxWeightedMapping.h"



{
	namespace FbxSDK
	{		
		void FbxWeightedMapping::CollectManagedMemory()
		{
		}
		FbxWeightedMapping::FbxWeightedMapping(int sourceSize, int destinationSize)
		{
			_SetPointer(new KFbxWeightedMapping(sourceSize,destinationSize),true);
		}
		void FbxWeightedMapping::Reset(int sourceSize, int destinationSize)
		{
			_Ref()->Reset(sourceSize,destinationSize);
		}
		void FbxWeightedMapping::Add(int sourceIndex, int destinationIndex, double weight)
		{
			_Ref()->Add(sourceIndex,destinationIndex,weight);
		}
		int FbxWeightedMapping::GetElementCount(Set set)
		{
			return _Ref()->GetElementCount((KFbxWeightedMapping::ESet)set);
		}
		int FbxWeightedMapping::GetRelationCount(Set set, int element)
		{
			return _Ref()->GetRelationCount((KFbxWeightedMapping::ESet)set,element);
		}

		FbxWeightedMapping::Element FbxWeightedMapping::GetRelation(Set set, int element, int index)
		{
			KFbxWeightedMapping::KElement e = _Ref()->GetRelation((KFbxWeightedMapping::ESet)set,element,index);
			FbxWeightedMapping::Element e1;
			e1.Index = e.mIndex;
			e1.Weight = e.mWeight;
			return e1;

		}
		int FbxWeightedMapping::GetRelationIndex(Set set, int elementInSet, int elementInOtherSet)
		{
			return _Ref()->GetRelationIndex((KFbxWeightedMapping::ESet)set,elementInSet,elementInOtherSet);
		}
		double FbxWeightedMapping::GetRelationSum(Set set, int element, bool absoluteValue)
		{
			return _Ref()->GetRelationSum((KFbxWeightedMapping::ESet)set,element,absoluteValue);
		}

		void FbxWeightedMapping::Normalize(Set set, bool absoluteValue)
		{
			_Ref()->Normalize((KFbxWeightedMapping::ESet)set,absoluteValue);
		}
	}
}