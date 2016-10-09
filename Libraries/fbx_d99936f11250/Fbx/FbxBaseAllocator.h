#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{		
			public ref class FbxBaseAllocator
			{
			//internal:
			//	KBaseAllocator* allocator;
			//	bool isNew;
			//	FbxBaseAllocator(KBaseAllocator* a);
			//public:
			//	FbxBaseAllocator(size_t recordSize);
			//	~FbxBaseAllocator();
			//	!FbxBaseAllocator();
			//	void Reserve(size_t recordCount);

			//	/*void* AllocateRecords(size_t const pRecordCount = 1)
			//	{
			//		return malloc(pRecordCount * mRecordSize);
			//	}

			//	void FreeMemory(void* pRecord)
			//	{
			//		free(pRecord);
			//	}*/

			//	/*property size_t RecordSize
			//	{
			//		size_t get() {return allocator->GetRecordSize(); }
			//	}*/				

			//};

			///*
			//This allocator only frees the allocated memory when it is deleted.
			//This is a good allocator for building dictionaries, where we only
			//add things to a container, but never remove them.
			//*/
			//public ref class FbxHungryAllocator
			//{
			//internal:
			//	KHungryAllocator* hungry;
			//	bool isNew;
			//	FbxHungryAllocator(KHungryAllocator* h);
			//public:
			//	FbxHungryAllocator(size_t recordSize);

			//	FbxHungryAllocator(FbxHungryAllocator^ other);
			//	~FbxHungryAllocator();
			//	!FbxHungryAllocator();

			//	void Reserve(size_t recordCount);

			//	/*void* AllocateRecords(size_t const pRecordCount = 1)
			//	{
			//		MemoryBlock* lBlock = mData;
			//		void* lRecord = NULL;

			//		while ((lBlock != NULL) &&
			//			((lRecord = lBlock->GetChunk(pRecordCount * mRecordSize)) == NULL))
			//		{
			//			lBlock = lBlock->mNextBlock;
			//		}

			//		if (lRecord == NULL)
			//		{
			//			size_t lNumRecordToAllocate = mRecordPoolSize / 8 == 0 ? 2 : mRecordPoolSize / 8;
			//			if (lNumRecordToAllocate < pRecordCount)
			//			{
			//				lNumRecordToAllocate = pRecordCount;
			//			}
			//			Reserve(lNumRecordToAllocate);
			//			lRecord = AllocateRecords(pRecordCount);
			//		}

			//		return lRecord;
			//	}

			//	void FreeMemory(void* pRecord)
			//	{
			//	}*/

			//	property size_t RecordSize
			//	{
			//		size_t  get();
			//	}

			//	//KHungryAllocator& operator=(const KHungryAllocator& pOther)
			//	//{
			//	//	if( this != &pOther )
			//	//	{
			//	//		// The next call to AllocateRecords() may skip over currently reserved
			//	//		// records if the size changes drastically, but otherwise GetChunk()
			//	//		// is size-oblivious.
			//	//		if( mRecordSize < pOther.mRecordSize )
			//	//		{
			//	//			mRecordPoolSize = 0;
			//	//		}

			//	//		mRecordSize = pOther.mRecordSize;
			//	//	}

			//	//	return(*this);
			//	//}			
			};
		
	}
}