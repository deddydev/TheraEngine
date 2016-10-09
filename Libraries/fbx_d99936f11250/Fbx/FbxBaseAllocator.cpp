#pragma once
#include "stdafx.h"
#include "FbxBaseAllocator.h"

namespace Skill
{
	namespace FbxSDK
	{					
		/*FbxBaseAllocator::FbxBaseAllocator(KBaseAllocator* a)
		{
			allocator = a;
		}			
		FbxBaseAllocator::FbxBaseAllocator(size_t recordSize)					
		{
			allocator = new KBaseAllocator(recordSize);
			isNew = true;
		}
		FbxBaseAllocator::~FbxBaseAllocator()
		{
			this->!FbxBaseAllocator();
		}
		FbxBaseAllocator::!FbxBaseAllocator()
		{
			if(allocator && isNew)
				delete allocator;
			allocator = nullptr;
			isNew = false;
		}

		void FbxBaseAllocator::Reserve(size_t recordCount)
		{
			allocator->Reserve(recordCount);
		}				

		FbxHungryAllocator::FbxHungryAllocator(KHungryAllocator* h)
		{
			hungry = h;
			isNew = false;
		}

		FbxHungryAllocator::FbxHungryAllocator(size_t recordSize)
		{
			hungry = new KHungryAllocator(recordSize);
			isNew = true;
		}

		FbxHungryAllocator::FbxHungryAllocator(FbxHungryAllocator^ other)					
		{
			hungry = new KHungryAllocator(*other->hungry);
			isNew = true;
		}				
		FbxHungryAllocator::~FbxHungryAllocator()
		{
			this->!FbxHungryAllocator();
		}
		FbxHungryAllocator::!FbxHungryAllocator()
		{
			if(hungry && isNew)
				delete hungry;
			hungry = nullptr;
			isNew = false;
		}

		void FbxHungryAllocator::Reserve(size_t recordCount)
		{
			hungry->Reserve(recordCount);
		}				

		size_t FbxHungryAllocator::RecordSize::get()
		{
			return hungry->GetRecordSize();
		}			*/	

	}
}