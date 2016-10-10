#pragma once
#include "stdafx.h"
#include "Fbx.h"


{
	namespace FbxSDK
	{
		namespace MP
		{
			/** Provides simple mutex lock functionality
			*/
			public ref class FbxMutex : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxMutex,kfbxmp::KFbxMutex);
				INATIVEPOINTER_DECLARE(FbxMutex,kfbxmp::KFbxMutex);			
			public:
				DEFAULT_CONSTRUCTOR(FbxMutex,kfbxmp::KFbxMutex);

				//! Acquires the lock. Blocks until acquired
				void Acquire();

				//! Releases the lock, possibly unblocking other threads.
				void Release();
			};

			//! Simple class that unlocks the mutex when it goes out of scope.
			public ref class FbxMutexHelper : IFbxNativePointer
			{
				BASIC_CLASS_DECLARE(FbxMutexHelper,kfbxmp::KFbxMutexHelper);
				INATIVEPOINTER_DECLARE(FbxMutexHelper,kfbxmp::KFbxMutexHelper);				
			public:
				FbxMutexHelper(FbxMutex^ mutex);				
			};
		}
	}
}