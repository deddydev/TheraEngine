#pragma once
#include "stdafx.h"
#include "FbxType.h"
#include "FbxLockAccessStatus.h"



{
	namespace FbxSDK
	{
		/**FBX SDK layer element array class
		* \nosubgrouping
		*/

		public ref class FbxLayerElementArray : IFbxNativePointer
		{		
			BASIC_CLASS_DECLARE(FbxLayerElementArray,KFbxLayerElementArray);
			INATIVEPOINTER_DECLARE(FbxLayerElementArray,KFbxLayerElementArray);
		public:
			/**
			* \name Constructor and Destructor
			*/
			//@{
			//!Constructor.
			FbxLayerElementArray(FbxType dataType);

			//@}

			/**
			* \name Status handling
			*/
			//@{

			//!Clear access state and set it to success.
			//void ClearStatus();

			//!Retrieve access state.
			VALUE_PROPERTY_GET_DECLARE(FbxLockAccessStatus::LockAccessStatus,Status);
			//@}

			/** 
			* \name Locks handling
			*/
			//@{

			/** Get whether write is locked.
			* \return        \c true if write is locked, \c false otherwise.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsWriteLocked);

			/**Retrieve the read lock count.
			* \return           the read lock count.
			*/
			VALUE_PROPERTY_GET_DECLARE(int,ReadLockCount);				
			//@}

			/** Returns \c true if this Array is accessed in any way.
			*/
			VALUE_PROPERTY_GET_DECLARE(bool,IsInUse);				

			/** Increments the number of locks on this array.
			* \return the current number of locks (including the one just grabbed) or 0 if a write lock is active.
			*/
			int ReadLock();

			/** Release a read lock on this array.
			* \return the remaining locks or -1 if a write lock is active.
			*/
			int ReadUnlock();

			/** Locks this array for writing. The data in the array is wiped out.
			* \return true if a write lock has been successully granted or false if one or more read locks
			are active.
			*/
			bool WriteLock();

			/** Release the write lock on this array.
			*/
			void WriteUnlock();

			/** Locks this array for writing. The data already existing in the array is kept and is valid.
			* \return true if a write lock has been successully granted or false if one or more read locks
			are active.
			*/
			bool ReadWriteLock();

			/** Release the write lock on this array.
			*/
			void ReadWriteUnlock();


			// Data access
			enum class LockMode{
				ReadLock = 1,
				WriteLock = 2,
				ReadWriteLock = 3
			};

			/** Grants a locked access to the data buffer. 
			* \param pLockMode Access mode to the data buffer.
			* \param pDataType If defined, try to return the data as this type.
			* \return A pointer to the data buffer or NULL if a failure occurred.
			* \remark In case of a failure, the Status is updated accordingly with the
			reason of the failure. Also, when a conversion of types occurs, a second buffer 
			of the new type is allocated. In this case, the LockMode does not apply on the
			returned buffer since it is a copy but it does apply on the internal data of this
			object. The returned buffer still remains property of this object and will be
			deleted when the pointer is released or this object is destroyed. At that moment,
			the values in this buffer are copied back into this object.
			*/
			//virtual void*   GetLocked(ELockMode pLockMode, EFbxType pDataType);
			//void*   GetLocked(ELockMode pLockMode=eREADWRITE_LOCK) { return GetLocked(pLockMode, mDataType); }
			//template <class T> inline T* GetLocked(T* dummy=NULL, ELockMode pLockMode=eREADWRITE_LOCK) {T v; return (T*)GetLocked(pLockMode, _FbxTypeOf(v)); }

			/** Release the lock on the data buffer.
			* \param pDataPtr The buffer we want to release.
			* \param pDataType           data type
			* \remark The passed pointer mjust be the one obtained by the call to GetLocked().
			* Any other pointer will cause this method to fail and the Status is updated with
			the reason of the failure. In case the passed pointer is referencing a converted data
			buffer (see comment of the GetLocked), this method will copy GetCount() items 
			of the received buffer back into this object. Any other items that may have been added
			using a realloc call will be ignored.
			*/    
			//virtual void   Release(void** pDataPtr, EFbxType pDataType);
			//void   Release(void** pDataPtr) { Release(pDataPtr, mDataType); }
			//template <class T> inline void Release(T** pDataPtr, T* dummy) { Release((void**)pDataPtr, _FbxTypeOf(*dummy)); }

			/** Return the Stride size.
			*/
			VALUE_PROPERTY_GET_DECLARE(size_t,Stride);

			/**
			* \name Data array manipulation
			*/
			//@{

			VALUE_PROPERTY_GETSET_DECLARE(int,Count);			
			void Clear();
			void Resize(int itemCount);
			void AddMultiple(int itemCount);

			//	int     Add(void const* pItem, EFbxType pValueType);
			//	int		InsertAt(int pIndex, void const* pItem, EFbxType pValueType);
			//	void	SetAt(int pIndex, void const* pItem, EFbxType pValueType);
			//	void    SetLast(void const* pItem, EFbxType pValueType);

			//	void    RemoveAt(int pIndex, void** pItem, EFbxType pValueType);
			//	void    RemoveLast(void** pItem, EFbxType pValueType);
			//	bool    RemoveIt(void** pItem, EFbxType pValueType);

			//	bool    GetAt(int pIndex, void** pItem, EFbxType pValueType) const;
			//	bool    GetFirst(void** pItem, EFbxType pValueType) const;
			//	bool    GetLast(void** pItem, EFbxType pValueType) const;

			//	int     Find(void const* pItem, EFbxType pValueType) const;
			//	int     FindAfter(int pAfterIndex, void const* pItem, EFbxType pValueType) const;
			//	int     FindBefore(int pBeforeIndex, void const* pItem, EFbxType pValueType) const;

			//	bool    IsEqual(const KFbxLayerElementArray& pArray);

			//	template <class T> inline int  Add(T const& pItem)								 { return Add((void const*)&pItem, _FbxTypeOf(pItem)); }
			//	template <class T> inline int  InsertAt(int pIndex, T const& pItem)				 { return InsertAt(pIndex, (void const*)&pItem, _FbxTypeOf(pItem)); }
			//	template <class T> inline void SetAt(int pIndex, T const& pItem)				 { SetAt(pIndex, (void const*)&pItem, _FbxTypeOf(pItem)); }
			//	template <class T> inline void SetLast(T const& pItem)							 { SetLast((void const*)&pItem, _FbxTypeOf(pItem)); }

			//	template <class T> inline void RemoveAt(int pIndex, T* pItem)					 { RemoveAt(pIndex, (void**)&pItem, _FbxTypeOf(*pItem)); }
			//	template <class T> inline void RemoveLast(T* pItem)								 { RemoveLast((void**)&pItem, _FbxTypeOf(*pItem)); }
			//	template <class T> inline bool RemoveIt(T* pItem)								 { return RemoveIt((void**)&pItem, _FbxTypeOf(*pItem)); }

			//	template <class T> inline bool GetAt(int pIndex, T* pItem) const				 { return GetAt(pIndex, (void**)&pItem, _FbxTypeOf(*pItem)); }
			//	template <class T> inline bool GetFirst(T* pItem) const							 { return GetFirst((void**)&pItem, _FbxTypeOf(*pItem)); }
			//	template <class T> inline bool GetLast(T* pItem) const							 { return GetLast((void**)&pItem, _FbxTypeOf(*pItem)); }

			//	template <class T> inline int Find(T const& pItem) const						 { return Find((void const*)&pItem, _FbxTypeOf(pItem)); }
			//	template <class T> inline int FindAfter(int pAfterIndex, T const& pItem) const   { return FindAfter(pAfterIndex, (void const*)&pItem, _FbxTypeOf(pItem)); }
			//	template <class T> inline int FindBefore(int pBeforeIndex, T const& pItem) const { return FindBefore(pBeforeIndex, (void const*)&pItem, _FbxTypeOf(pItem)); }


			//	template<typename T> inline void CopyTo(KArrayTemplate<T>& pDst)
			//	{
			//		T src;
			//		T* srcPtr = &src;

			//		pDst.Clear();
			//		if (mDataType != _FbxTypeOf(src))
			//		{
			//			SetStatus(LockAccessStatus::eUnsupportedDTConversion);
			//			return;
			//		}

			//		pDst.SetCount(GetCount());
			//		for (int i = 0; i < GetCount(); i++)
			//		{
			//			if (GetAt(i, (void**)&srcPtr, mDataType))
			//			{
			//				pDst.SetAt(i, src);
			//			}
			//		}
			//		SetStatus(LockAccessStatus::eSuccess);
			//	}
			//	//@}
		};
		//
		//

	}
}