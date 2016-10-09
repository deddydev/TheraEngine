#pragma once
#include "stdafx.h"
#include "Fbx.h"


namespace Skill
{
	namespace FbxSDK
	{
		/** \brief This class provides services for memory management.
		* \nosubgrouping
		* The FBX SDK Manager uses an object of type KFbxMemoryAllocator
		* to allocate and free memory. Implement your own class if your
		* application requires custom memory management.
		*/
		public ref class FbxMemoryAllocator : IFbxNativePointer
		{
			BASIC_CLASS_DECLARE(FbxMemoryAllocator,KFbxMemoryAllocator);
			INATIVEPOINTER_DECLARE(FbxMemoryAllocator,KFbxMemoryAllocator)
	/*	internal:
			KFbxMemoryAllocator* allocator;
			bool isNew;
			FbxMemoryAllocator(KFbxMemoryAllocator* a);*/
		//public:
		//	/** Constructor
		//	* \param pMallocHandler      Pointer to a function implementing malloc. This function allocates memory blocks.
		//	* \param pCallocHandler      Pointer to a function implementing calloc. This function allocates an array in memory with elements initialized to 0.
		//	* \param pReallocHandler     Pointer to a function implementing realloc. This function reallocate memory blocks.
		//	* \param pFreeHandler        Pointer to a function implementing free. This function deallocates memory blocks.
		//	*/
		//	KFbxMemoryAllocator(void* (*pMallocHandler)(size_t),
		//		void* (*pCallocHandler)(size_t,size_t),
		//		void* (*pReallocHandler)(void*,size_t),
		//		void  (*pFreeHandler)(void*))
		//		: mMallocHandler(pMallocHandler)
		//		, mCallocHandler(pCallocHandler)
		//		, mReallocHandler(pReallocHandler)
		//		, mFreeHandler(pFreeHandler)
		//		, mMallocHandler_debug(0)
		//		, mCallocHandler_debug(0)
		//		, mReallocHandler_debug(0)
		//		, mFreeHandler_debug(0)
		//	{
		//	}
		//	KFbxMemoryAllocator(void* (*pMallocHandler)(size_t),
		//		void* (*pCallocHandler)(size_t,size_t),
		//		void* (*pReallocHandler)(void*,size_t),
		//		void  (*pFreeHandler)(void*),
		//		void* (*pMallocHandler_debug)(size_t,int,const char *,int),
		//		void* (*pCallocHandler_debug)(size_t, size_t,int,const char *,int),
		//		void* (*pReallocHandler_debug)(void*, size_t,int,const char *,int),
		//		void  (*pFreeHandler_debug)(void*,int)
		//		)
		//		: mMallocHandler(pMallocHandler)
		//		, mCallocHandler(pCallocHandler)
		//		, mReallocHandler(pReallocHandler)
		//		, mFreeHandler(pFreeHandler)
		//		, mMallocHandler_debug(pMallocHandler_debug)
		//		, mCallocHandler_debug(pCallocHandler_debug)
		//		, mReallocHandler_debug(pReallocHandler_debug)
		//		, mFreeHandler_debug(pFreeHandler_debug)
		//	{
		//	}
		//	virtual ~KFbxMemoryAllocator() = 0;	

		//	void* (*mMallocHandler)(size_t);
		//	void* (*mCallocHandler)(size_t,size_t);
		//	void* (*mReallocHandler)(void*,size_t);
		//	void  (*mFreeHandler)(void*);
		//	void* (*mMallocHandler_debug)(size_t,int,const char *,int);
		//	void* (*mCallocHandler_debug)(size_t, size_t,int,const char *,int);
		//	void* (*mReallocHandler_debug)(void*, size_t,int,const char *,int);
		//	void  (*mFreeHandler_debug)(void*,int);
		};

		/** Default implementation of memory allocator.
		* \nosubgrouping
		* This default implementation uses malloc, calloc, realloc, and free from the C runtime library.
		*/
		public ref class FbxDefaultMemoryAllocator : FbxMemoryAllocator
		{
		internal:
			FbxDefaultMemoryAllocator(KFbxDefaultMemoryAllocator* instance) :FbxMemoryAllocator(instance)
			{
				_Free = false;
			}
		/*public:
			KFbxDefaultMemoryAllocator();
			~KFbxDefaultMemoryAllocator();*/
		};

	}
}