#pragma once
#include "stdafx.h"
#include "FbxLayerElementArray.h"



{
	namespace FbxSDK
	{
		void FbxLayerElementArray::CollectManagedMemory()
		{
		}
		FbxLayerElementArray::FbxLayerElementArray(FbxType dataType)
		{
			_SetPointer(new KFbxLayerElementArray((EFbxType)dataType),true);
		}
		/*void FbxLayerElementArray::ClearStatus()
		{
		_Ref()->ClearStatus();
		}*/
		FbxLockAccessStatus::LockAccessStatus FbxLayerElementArray::Status::get()
		{
			return (FbxLockAccessStatus::LockAccessStatus)_Ref()->GetStatus();
		}	

		VALUE_PROPERTY_GET_DEFINATION(FbxLayerElementArray,IsWriteLocked(),bool,IsWriteLocked);
		VALUE_PROPERTY_GET_DEFINATION(FbxLayerElementArray,GetReadLockCount(),int,ReadLockCount);
		VALUE_PROPERTY_GET_DEFINATION(FbxLayerElementArray,IsInUse(),bool,IsInUse);		

		int FbxLayerElementArray::ReadLock()
		{
			return _Ref()->ReadLock();
		}
		int FbxLayerElementArray::ReadUnlock()
		{
			return _Ref()->ReadUnlock();
		}
		bool FbxLayerElementArray::WriteLock()
		{
			return _Ref()->WriteLock();
		}
		void FbxLayerElementArray::WriteUnlock()
		{
			return _Ref()->WriteUnlock();
		}
		bool FbxLayerElementArray::ReadWriteLock()
		{
			return _Ref()->ReadWriteLock();
		}
		void FbxLayerElementArray::ReadWriteUnlock()
		{
			return _Ref()->ReadWriteUnlock();
		}
		VALUE_PROPERTY_GET_DEFINATION(FbxLayerElementArray,GetStride(),size_t,Stride);

		int FbxLayerElementArray::Count::get()
		{
			return _Ref()->GetCount();
		}
		void FbxLayerElementArray::Count::set(int value)
		{
			_Ref()->SetCount(value);
		}
		void FbxLayerElementArray::Clear()
		{
			_Ref()->Clear();
		}
		void FbxLayerElementArray::Resize(int itemCount)
		{
			_Ref()->Resize(itemCount);
		}
		void FbxLayerElementArray::AddMultiple(int itemCount)
		{
			_Ref()->AddMultiple(itemCount);
		}

	}
}