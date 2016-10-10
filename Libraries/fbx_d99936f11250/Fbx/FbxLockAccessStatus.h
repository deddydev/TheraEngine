#pragma once
#include "stdafx.h"
#include "Fbx.h"



{
	namespace FbxSDK
	{
		/** \brief Identify what error occured when manipulating the data arrays.
		* \nosubgrouping
		*/

		public ref class FbxLockAccessStatus
		{			
		public:
			/** \enum ELockAccessStatus	Identify what error occured when manipulating the data arrays.
			* - \e eSuccess					Operation Successfull.
			* - \e eUnsupportedDTConversion   Attempting to convert to an unsupported DataType.
			* - \e eCorruptedCopyback         The Release of a converted buffer failed and corrupted the main data.
			* - \e eBadValue                  Invalid value.
			* - \e eLockMismatch              Attempting to change to an incompatible lock.
			* - \e eNoWriteLock               A write operation as been attempted but no WriteLock is available.
			* - \e eNoReadLock                A read operation as been attempted but the WriteLock is active.
			* - \e eNotOwner                  Attempting to release a lock on an invalid data buffer pointer.
			* - \e eDirectLockExist           A direct access lock is still active.
			*/
			enum class LockAccessStatus
			{
				Success = fbxsdk_200901::LockAccessStatus::eSuccess,
				UnsupportedDTConversion = fbxsdk_200901::LockAccessStatus::eUnsupportedDTConversion,
				CorruptedCopyback = fbxsdk_200901::LockAccessStatus::eCorruptedCopyback,
				BadValue = fbxsdk_200901::LockAccessStatus::eBadValue,
				LockMismatch = fbxsdk_200901::LockAccessStatus::eLockMismatch,
				NoWriteLock = fbxsdk_200901::LockAccessStatus::eNoWriteLock,
				NoReadLock = fbxsdk_200901::LockAccessStatus::eNoReadLock,
				NotOwner = fbxsdk_200901::LockAccessStatus::eNotOwner,
				DirectLockExist = fbxsdk_200901::LockAccessStatus::eDirectLockExist
			};
		};

	}
}