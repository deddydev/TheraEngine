#pragma once
#include "stdafx.h"
#include "FbxMutex.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace MP
		{
			void FbxMutex::CollectManagedMemory()
			{
			}
			void FbxMutex::Acquire()
			{
				_Ref()->Acquire();
			}
			void FbxMutex::Release()
			{
				_Ref()->Release();
			}						
			
			void FbxMutexHelper::CollectManagedMemory()
			{
			}
			FbxMutexHelper::FbxMutexHelper(FbxMutex^ mutex)
			{
				_SetPointer(new kfbxmp::KFbxMutexHelper(*mutex->_Ref()),true);
			}			
		}
	}
}