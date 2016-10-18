#ifndef _FBXSDK_SCENE_GEOMETRY_LAYER_H_
#define _FBXSDK_SCENE_GEOMETRY_LAYER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxdatatypes.h>
#include <fbxsdk/core/fbxstream.h>
#include <fbxsdk/scene/shading/fbxsurfacematerial.h>
#include <fbxsdk/scene/shading/fbxtexture.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxLayerElementArray
class FbxLayerContainer
class FBXSDK_DLL FbxLayerElement
public:
	enum EType
		eUnknown,
		eNormal,
        eBiNormal,
        eTangent,
		eMaterial,
		ePolygonGroup,
		eUV,
		eVertexColor,
		eSmoothing,
        eVertexCrease,
        eEdgeCrease,
        eHole,
		eUserData,
		eVisibility,
        eTextureDiffuse,
        eTextureDiffuseFactor,
		eTextureEmissive,
		eTextureEmissiveFactor,
		eTextureAmbient,
		eTextureAmbientFactor,
		eTextureSpecular,
        eTextureSpecularFactor,
        eTextureShininess,
		eTextureNormalMap,
		eTextureBump,
		eTextureTransparency,
		eTextureTransparencyFactor,
		eTextureReflection,
		eTextureReflectionFactor,
        eTextureDisplacement,
        eTextureDisplacementVector,
		eTypeCount
    const static int sTypeTextureStartIndex = int(eTextureDiffuse)
    const static int sTypeTextureEndIndex = int(eTypeCount) - 1
    const static int sTypeTextureCount = sTypeTextureEndIndex - sTypeTextureStartIndex + 1
    const static int sTypeNonTextureStartIndex = int(eNormal)
    const static int sTypeNonTextureEndIndex = int(eVisibility)
    const static int sTypeNonTextureCount = sTypeNonTextureEndIndex - sTypeNonTextureStartIndex + 1
    static const char* const sTextureNames[]
    static const char* const sTextureUVNames[]
    static const char* const sNonTextureNames[]
    static const FbxDataType sTextureDataTypes[]
    static const char* const sTextureChannelNames[]
	enum EMappingMode
		eNone,
		eByControlPoint,
		eByPolygonVertex,
		eByPolygon,
		eByEdge,
		eAllSame
	enum EReferenceMode
		eDirect,
		eIndex,
		eIndexToDirect
	void SetMappingMode(EMappingMode pMappingMode) 
 mMappingMode = pMappingMode
	void SetReferenceMode(EReferenceMode pReferenceMode) 
 mReferenceMode = pReferenceMode
	EMappingMode GetMappingMode() const 
 return mMappingMode
	EReferenceMode GetReferenceMode() const 
 return mReferenceMode
	void SetName(const char* pName) 
 mName = FbxString(pName)
	const char* GetName() const 
 return ((FbxLayerElement*)this)->mName.Buffer()
	bool operator==(const FbxLayerElement& pOther) const
		return (mName == pOther.mName) && 
			   (mMappingMode == pOther.mMappingMode) &&
			   (mReferenceMode == pOther.mReferenceMode)
	FbxLayerElement& operator=( FbxLayerElement const& pOther )
		mMappingMode = pOther.mMappingMode
		mReferenceMode = pOther.mReferenceMode
		return *this
	void Destroy()
    virtual bool Clear() 
        return true
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    void SetType(const FbxDataType* pType) 
 mType = pType
	const FbxLayerContainer* GetOwner() const 
 return mOwner
protected:
	FbxLayerElement() 
		: mMappingMode(eNone)
		, mReferenceMode(eDirect)
		, mName("")
		, mOwner(NULL)
	virtual ~FbxLayerElement()
	EMappingMode mMappingMode
	EReferenceMode mReferenceMode
	FbxString mName
	const FbxDataType* mType
	FbxLayerContainer* mOwner
	void Destruct() 
 FbxDelete(this)
	virtual void SetOwner(FbxLayerContainer* pOwner, int pInstance = 0)
    FBXSDK_FRIEND_NEW()
public:
	virtual int MemorySize() const 
 return 0
	virtual bool ContentWriteTo(FbxStream& pStream) const
	virtual bool ContentReadFrom(const FbxStream& pStream)
	friend class FbxLayerContainer
#endif 
class FBXSDK_DLL LockAccessStatus
public:
	enum ELockAccessStatus
		eSuccess,
		eUnsupportedDTConversion,
		eCorruptedCopyback,
		eBadValue,
		eLockMismatch,
		eNoWriteLock,
		eNoReadLock,
		eNotOwner,
		eDirectLockExist
typedef FbxHandle* FbxRefPtr
typedef FbxLayerElementArray* FbxLayerElementArrayPtr
typedef FbxSurfaceMaterial* FbxSurfaceMaterialPtr
typedef FbxTexture* FbxTexturePtr
inline EFbxType FbxTypeOf(const FbxRefPtr&)
 return eFbxReference
inline EFbxType FbxTypeOf(const FbxLayerElementArrayPtr&)
 return eFbxReference
inline EFbxType FbxTypeOf(const FbxSurfaceMaterialPtr&)
 return eFbxReference
inline EFbxType FbxTypeOf(const FbxTexturePtr&)
 return eFbxReference
class FBXSDK_DLL FbxLayerElementArray
public:
	FbxLayerElementArray(EFbxType pDataType)
	virtual ~FbxLayerElementArray()
	inline                                void ClearStatus()     
 mStatus = LockAccessStatus::eSuccess
	inline LockAccessStatus::ELockAccessStatus GetStatus() const 
 return mStatus
	inline bool IsWriteLocked() const 
 return mWriteLock
	inline int  GetReadLockCount() const 
 return mReadLockCount
	bool	IsInUse() const
	int     ReadLock() const
	int     ReadUnlock() const
	bool    WriteLock() const
	void    WriteUnlock() const
	bool    ReadWriteLock() const
	void    ReadWriteUnlock() const
	enum ELockMode
		eReadLock = 1,
		eWriteLock = 2,
		eReadWriteLock = 3
    virtual void*   GetLocked(ELockMode pLockMode, EFbxType pDataType)
	void*   GetLocked(ELockMode pLockMode=eReadWriteLock) 
 return GetLocked(pLockMode, mDataType)
	template <class T> inline T* GetLocked(T*, ELockMode pLockMode=eReadWriteLock) 
T v
 return (T*)GetLocked(pLockMode, FbxTypeOf(v))
	virtual void   Release(void** pDataPtr, EFbxType pDataType)
	void   Release(void** pDataPtr) 
 Release(pDataPtr, mDataType)
	template <class T> inline void Release(T** pDataPtr, T* dummy) 
		T*** voidPtr = &pDataPtr
        Release((void**)*voidPtr, FbxTypeOf(*dummy))
	virtual size_t GetStride() const
	int		GetCount() const
	void	SetCount(int pCount)
	void	Clear()
	void	Resize(int pItemCount)
	void	AddMultiple(int pItemCount)
	int     Add(const void* pItem, EFbxType pValueType)
	int		InsertAt(int pIndex, const void* pItem, EFbxType pValueType)
	void	SetAt(int pIndex, const void* pItem, EFbxType pValueType)
	void    SetLast(const void* pItem, EFbxType pValueType)
	void    RemoveAt(int pIndex, void** pItem, EFbxType pValueType)
	void    RemoveLast(void** pItem, EFbxType pValueType)
	bool    RemoveIt(void** pItem, EFbxType pValueType)
	bool    GetAt(int pIndex, void** pItem, EFbxType pValueType) const
	bool    GetFirst(void** pItem, EFbxType pValueType) const
	bool    GetLast(void** pItem, EFbxType pValueType) const
	int     Find(const void* pItem, EFbxType pValueType) const
	int     FindAfter(int pAfterIndex, const void* pItem, EFbxType pValueType) const
	int     FindBefore(int pBeforeIndex, const void* pItem, EFbxType pValueType) const
	bool    IsEqual(const FbxLayerElementArray& pArray) const
	template <class T> inline int  Add(T const& pItem)								 
 return Add((const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline int  InsertAt(int pIndex, T const& pItem)				 
 return InsertAt(pIndex, (const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline void SetAt(int pIndex, T const& pItem)				 
 SetAt(pIndex, (const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline void SetLast(T const& pItem)							 
 SetLast((const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline void RemoveAt(int pIndex, T* pItem)					 
		T** voidPtr = &pItem
        RemoveAt(pIndex, (void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline void RemoveLast(T* pItem)								 
		T** voidPtr = &pItem
        RemoveLast((void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline bool RemoveIt(T* pItem)								 
		T** voidPtr = &pItem
        return RemoveIt((void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline bool GetAt(int pIndex, T* pItem) const				 
		T** voidPtr = &pItem
        return GetAt(pIndex, (void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline bool GetFirst(T* pItem) const							 
		T** voidPtr = &pItem
        return GetFirst((void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline bool GetLast(T* pItem) const							 
		T** voidPtr = &pItem
        return GetLast((void**)voidPtr, FbxTypeOf(*pItem))
	template <class T> inline int Find(T const& pItem) const						 
 return Find((const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline int FindAfter(int pAfterIndex, T const& pItem) const   
 return FindAfter(pAfterIndex, (const void*)&pItem, FbxTypeOf(pItem))
	template <class T> inline int FindBefore(int pBeforeIndex, T const& pItem) const 
 return FindBefore(pBeforeIndex, (const void*)&pItem, FbxTypeOf(pItem))
	template<typename T> inline void CopyTo(FbxArray<T>& pDst)
		T src
		T* srcPtr = &src
		pDst.Clear()
		if (mDataType != FbxTypeOf(src))
			SetStatus(LockAccessStatus::eUnsupportedDTConversion)
			return
		pDst.Resize(GetCount())
		for (int i = 0
 i < GetCount()
 i++)
			if (GetAt(i, (void**)&srcPtr, mDataType))
				pDst.SetAt(i, src)
		SetStatus(LockAccessStatus::eSuccess)
protected:
	void*   GetDataPtr()
	void*   GetReference(int pIndex, EFbxType pValueType)
	void    GetReferenceTo(int pIndex, void** pRef, EFbxType pValueType)
	inline void SetStatus(LockAccessStatus::ELockAccessStatus pVal) const
		const_cast<FbxLayerElementArray*>(this)->mStatus = pVal
	        void   SetImplementation(void* pImplementation)
	inline 	void*  GetImplementation() 
 return mImplementation
	virtual void   ConvertDataType(EFbxType pDataType, void** pDataPtr, size_t* pStride)
	EFbxType mDataType
private:
	LockAccessStatus::ELockAccessStatus	mStatus
	int			  mReadLockCount
	bool		  mWriteLock
	void*		  mImplementation
	size_t        mStride
	int           mDirectLockOn
	bool          mDirectAccessOn
	FbxArray<void*>	mConvertedData
template <typename T>
struct FbxLayerElementArrayReadLock
    FbxLayerElementArrayReadLock(FbxLayerElementArray& pArray) : mArray(pArray)
        mLockedData = mArray.GetLocked((T*)NULL, FbxLayerElementArray::eReadLock)
    ~FbxLayerElementArrayReadLock()
        if( mLockedData )
            mArray.Release((void **) &mLockedData)
    const T* GetData() const
        return mLockedData
private:
    FbxLayerElementArray&  mArray
    T* mLockedData
class FbxLayerElementUserData
template <class T> class FbxLayerElementArrayTemplate : public FbxLayerElementArray
public:
	FbxLayerElementArrayTemplate(EFbxType pDataType) :
		FbxLayerElementArray(pDataType)
	inline int  Add( T const &pItem )						
 return FbxLayerElementArray::Add(pItem)
	inline int  InsertAt(int pIndex, T const &pItem)		
 return FbxLayerElementArray::InsertAt(pIndex, pItem)
	inline void SetAt(int pIndex, T const &pItem)			
 FbxLayerElementArray::SetAt(pIndex, pItem)
	inline void SetLast( T const &pItem)					
 FbxLayerElementArray::SetLast(pItem)
	inline T RemoveAt(int pIndex)			   				
 T lValue
 FbxLayerElementArray::RemoveAt(pIndex, &lValue)
 return lValue
	inline T RemoveLast()									
 T lValue
 FbxLayerElementArray::RemoveLast(&lValue)
 return lValue
	inline bool RemoveIt(T const &pItem)					
 return FbxLayerElementArray::RemoveIt(&pItem)
	inline T  GetAt(int pIndex) const						
 T lValue
 FbxLayerElementArray::GetAt(pIndex, &lValue)
 return lValue
	inline T  GetFirst() const								
 T lValue
 FbxLayerElementArray::GetFirst(&lValue)
 return lValue
	inline T  GetLast() const								
 T lValue
 FbxLayerElementArray::GetLast(&lValue)
 return lValue
	inline int Find(T const &pItem)							
 return FbxLayerElementArray::Find(pItem)
	inline int FindAfter(int pAfterIndex, T const &pItem)	
 return FbxLayerElementArray::FindAfter(pAfterIndex, pItem)
	inline int FindBefore(int pBeforeIndex, T const &pItem) 
 return FbxLayerElementArray::FindBefore(pBeforeIndex, pItem)
	T  operator[](int pIndex) const							
 T lValue
 FbxLayerElementArray::GetAt(pIndex, &lValue)
 return lValue
    FbxLayerElementArray& operator=(const FbxArray<T>& pArrayTemplate)
        SetStatus(LockAccessStatus::eNoWriteLock)
        if (WriteLock())
            SetCount(pArrayTemplate.GetCount())
            for (int i = 0
 i < pArrayTemplate.GetCount()
 i++)
                SetAt(i, pArrayTemplate.GetAt(i))
            WriteUnlock()
            SetStatus(LockAccessStatus::eSuccess)
        return *this
    FbxLayerElementArrayTemplate<T>& operator=(const FbxLayerElementArrayTemplate<T>& pArrayTemplate)
        if ( this != &pArrayTemplate )
            SetStatus(LockAccessStatus::eNoWriteLock)
            if (WriteLock())
                SetCount(pArrayTemplate.GetCount())
                for (int i = 0
 i < pArrayTemplate.GetCount()
 i++)
                    SetAt(i, pArrayTemplate.GetAt(i))
                WriteUnlock()
                SetStatus(LockAccessStatus::eSuccess)
        return *this
private:
-)
	friend class FbxLayerElementUserData
	T& AsReference(int pIndex) 		
 T* v = (T*)FbxLayerElementArray::GetReference(pIndex, mDataType)
 return (v)?*v:dummy
	T dummy
extern FBXSDK_DLL int RemapIndexArrayTo(FbxLayerElement* pLayerEl, 
							 FbxLayerElement::EMappingMode pNewMapping, 
							 FbxLayerElementArrayTemplate<int>* pIndexArray)
template <class Type> class FbxLayerElementTemplate : public FbxLayerElement
public:
	FbxLayerElementArrayTemplate<Type>& GetDirectArray() const
		FBX_ASSERT(mReferenceMode == FbxLayerElement::eDirect || mReferenceMode == FbxLayerElement::eIndexToDirect)
		return *mDirectArray
	FbxLayerElementArrayTemplate<Type>& GetDirectArray()
		FBX_ASSERT(mReferenceMode == FbxLayerElement::eDirect || mReferenceMode == FbxLayerElement::eIndexToDirect)
		return *mDirectArray
	FbxLayerElementArrayTemplate<int>& GetIndexArray() const
		FBX_ASSERT(mReferenceMode == FbxLayerElement::eIndex || mReferenceMode == FbxLayerElement::eIndexToDirect)
		return *mIndexArray
	FbxLayerElementArrayTemplate<int>& GetIndexArray()
		FBX_ASSERT(mReferenceMode == FbxLayerElement::eIndex || mReferenceMode == FbxLayerElement::eIndexToDirect)
		return *mIndexArray
	bool Clear()
		bool ret = true
		mDirectArray->Clear()
		ret = (mDirectArray->GetStatus() == LockAccessStatus::eSuccess)
		mIndexArray->Clear()
		ret |= (mIndexArray->GetStatus() == LockAccessStatus::eSuccess)
		return ret
public:
	bool operator==(const FbxLayerElementTemplate& pOther) const
		bool ret = true
        if (pOther.GetReferenceMode() == FbxLayerElement::eDirect || 
            pOther.GetReferenceMode() == FbxLayerElement::eIndexToDirect)
            const FbxLayerElementArrayTemplate<Type>& directArray = pOther.GetDirectArray()
            if( directArray.GetCount() != mDirectArray->GetCount() || 
                !directArray.ReadLock() || !mDirectArray->ReadLock() )
                ret = false
            if( ret && !mDirectArray->IsEqual(directArray) )
                ret = false
            directArray.ReadUnlock()
            mDirectArray->ReadUnlock()
        if (ret)
            if (pOther.GetReferenceMode() == FbxLayerElement::eIndex || 
                pOther.GetReferenceMode()  == FbxLayerElement::eIndexToDirect)
                const FbxLayerElementArrayTemplate<int>& indexArray = pOther.GetIndexArray()
                if( indexArray.GetCount() != mIndexArray->GetCount() ||
                    !indexArray.ReadLock() || !mIndexArray->ReadLock() )
                    ret = false
                if( ret && !mIndexArray->IsEqual(indexArray) )
                    ret = false
                indexArray.ReadUnlock()
                mIndexArray->ReadUnlock()
        if (ret == false)
            return false
        return FbxLayerElement::operator==(pOther)
	FbxLayerElementTemplate& operator=( FbxLayerElementTemplate const& pOther )
		FBX_ASSERT(mDirectArray != NULL)
		FBX_ASSERT(mIndexArray != NULL)
		if (pOther.GetReferenceMode() == FbxLayerElement::eDirect || 
			pOther.GetReferenceMode() == FbxLayerElement::eIndexToDirect)
			const FbxLayerElementArrayTemplate<Type>& directArray = pOther.GetDirectArray()
			*mDirectArray = directArray
		if (pOther.GetReferenceMode() == FbxLayerElement::eIndex || 
			pOther.GetReferenceMode()  == FbxLayerElement::eIndexToDirect)
			const FbxLayerElementArrayTemplate<int>& indexArray = pOther.GetIndexArray()
			*mIndexArray = indexArray
		FbxLayerElement* myself = (FbxLayerElement*)this
		FbxLayerElement* myOther = (FbxLayerElement*)&pOther
		*myself = *myOther
		return *this
	int RemapIndexTo(FbxLayerElement::EMappingMode pNewMapping)
		return RemapIndexArrayTo(this, pNewMapping, mIndexArray)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayerElementTemplate() 
		mDirectArray = NULL
		mIndexArray = NULL
	~FbxLayerElementTemplate() 
		FbxDelete(mDirectArray)
		FbxDelete(mIndexArray)
	virtual void AllocateArrays()
		mDirectArray = FbxNew< FbxLayerElementArrayTemplate<Type> >(mType->GetType())
		mIndexArray = FbxNew< FbxLayerElementArrayTemplate<int> >(FbxIntDT.GetType())
public:
	virtual int MemorySize() const
		int size = FbxLayerElement::MemorySize()
		size += (mDirectArray->GetCount()*sizeof(Type))
		size += (mIndexArray->GetCount()*sizeof(int))
		return size
	virtual bool ContentWriteTo(FbxStream& pStream) const
		void* a
		int s,v
		int count = 0
		count = mDirectArray->GetCount()
		s = pStream.Write(&count, sizeof(int))
		if (s != sizeof(int)) return false
		if (count > 0)
			a = mDirectArray->GetLocked()
			FBX_ASSERT(a != NULL)
			v = count*sizeof(Type)
			s = pStream.Write(a, v)
			mDirectArray->Release(&a)
			if (s != v) return false
		count = mIndexArray->GetCount()
		s = pStream.Write(&count, sizeof(int))
		if (s != sizeof(int)) return false
		if (count > 0)
			a = mIndexArray->GetLocked()
			FBX_ASSERT(a != NULL)
			v = count*sizeof(int)
			s = pStream.Write(a, v)
			mIndexArray->Release(&a)
			if (s != v) return false
		return FbxLayerElement::ContentWriteTo(pStream)
	virtual bool ContentReadFrom(const FbxStream& pStream)
		void* a
		int s,v
		int count = 0
		s = pStream.Read(&count, sizeof(int))
		if (s != sizeof(int)) return false
		mDirectArray->Resize(count)
		if (count > 0)
			a = mDirectArray->GetLocked()
			FBX_ASSERT(a != NULL)
			v = count*sizeof(Type)
			s = pStream.Read(a, v)
			mDirectArray->Release(&a)
			if (s != v) return false
		s = pStream.Read(&count, sizeof(int))
		if (s != sizeof(int)) return false
		mIndexArray->Resize(count)
		if (count > 0)
			a = mIndexArray->GetLocked()
			FBX_ASSERT(a != NULL)
			v = count*sizeof(int)
			s = pStream.Read(a, v)
			mIndexArray->Release(&a)
			if (s != v) return false
		return FbxLayerElement::ContentReadFrom(pStream)
    typedef Type ArrayElementType
    typedef FbxLayerElementArrayTemplate<Type> DirectArrayType
    typedef FbxLayerElementArrayTemplate<int> IndexArrayType
	FbxLayerElementArrayTemplate<Type>* mDirectArray
	FbxLayerElementArrayTemplate<int>*  mIndexArray
#endif 
class FBXSDK_DLL FbxLayerElementNormal : public FbxLayerElementTemplate<FbxVector4>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementNormal)
protected:
	FbxLayerElementNormal()
	~FbxLayerElementNormal()
class FBXSDK_DLL FbxLayerElementBinormal : public FbxLayerElementTemplate<FbxVector4>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementBinormal)
protected:
	FbxLayerElementBinormal()
	~FbxLayerElementBinormal()
class FBXSDK_DLL FbxLayerElementTangent : public FbxLayerElementTemplate<FbxVector4>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementTangent)
protected:
	FbxLayerElementTangent()
	~FbxLayerElementTangent()
class FBXSDK_DLL FbxLayerElementMaterial : public FbxLayerElementTemplate<FbxSurfaceMaterial*>
public:
	typedef FbxLayerElementTemplate<FbxSurfaceMaterial*> ParentClass
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementMaterial)
	class LayerElementArrayProxy : public FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>
	public:
		typedef FbxLayerElementArrayTemplate<FbxSurfaceMaterial*> ParentClass
		LayerElementArrayProxy(EFbxType pType)
		void SetContainer( FbxLayerContainer* pContainer, int pInstance = 0)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual void AllocateArrays()
	virtual void SetOwner( FbxLayerContainer* pOwner, int pInstance = 0)
	virtual void SetInstance( int pInstance ) 
 SetOwner( mOwner, pInstance )
protected:
	FbxLayerElementMaterial()
	~FbxLayerElementMaterial()
private:
    FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>& GetDirectArray() const
        return ParentClass::GetDirectArray()
    FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>& GetDirectArray()
        return ParentClass::GetDirectArray()
    friend class FbxLayerContainer
#endif 
class FBXSDK_DLL FbxLayerElementPolygonGroup : public FbxLayerElementTemplate<int>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementPolygonGroup)
protected:
	FbxLayerElementPolygonGroup()
	~FbxLayerElementPolygonGroup()
class FBXSDK_DLL FbxLayerElementUV : public FbxLayerElementTemplate<FbxVector2>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementUV)
protected:
	FbxLayerElementUV()
	~FbxLayerElementUV()
class FBXSDK_DLL FbxLayerElementVertexColor : public FbxLayerElementTemplate<FbxColor>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementVertexColor)
protected:
	FbxLayerElementVertexColor()
	~FbxLayerElementVertexColor()
template <class T> inline FbxLayerElementArrayTemplate<T>&       FbxGetDirectArray(FbxLayerElementUserData       *pLayerElement, int pIndex, bool* pStatus = NULL)
template <class T> inline FbxLayerElementArrayTemplate<T> const& FbxGetDirectArray(FbxLayerElementUserData const *pLayerElement, int pIndex, bool* pStatus = NULL)
template <class T> inline FbxLayerElementArrayTemplate<T>&       FbxGetDirectArray(FbxLayerElementUserData       *pLayerElement, const char* pName, bool* pStatus = NULL )
template <class T> inline FbxLayerElementArrayTemplate<T> const& FbxGetDirectArray(FbxLayerElementUserData const *pLayerElement, const char* pName, bool* pStatus = NULL )
class FBXSDK_DLL FbxLayerElementUserData : public FbxLayerElementTemplate<void*>
public:
    FBXSDK_FRIEND_NEW()
	static FbxLayerElementUserData* Create(FbxLayerContainer* pOwner, const char* pName, int pId, FbxArray<FbxDataType>& pDataTypes, FbxArray<const char*>& pDataNames)
	static FbxLayerElementUserData* Create(FbxLayerContainer* pOwner, FbxLayerElementUserData const& pOther )
	FbxLayerElementArrayTemplate<void*>* GetDirectArrayVoid( int pIndex, bool* pStatus = NULL)
		if( pIndex >= 0 || pIndex < GetDirectArray().GetCount() )
			if (pStatus) *pStatus = true
			return (FbxLayerElementArrayTemplate<void*>*)GetDirectArray().AsReference(pIndex)
		else
			if( pStatus ) *pStatus = false
			FBX_ASSERT_NOW("Index out of bounds")
			return (FbxLayerElementArrayTemplate<void*>*)NULL
	const FbxLayerElementArrayTemplate<void*>* GetDirectArrayVoid( int pIndex, bool* pStatus = NULL) const
		if( pIndex >= 0 || pIndex < GetDirectArray().GetCount() )
			if (pStatus) *pStatus = true
			return (FbxLayerElementArrayTemplate<void*>*)GetDirectArray().AsReference(pIndex)
		else
			if( pStatus ) *pStatus = false
			FBX_ASSERT_NOW("Index out of bounds")
			return (const FbxLayerElementArrayTemplate<void*>*)NULL
	FbxLayerElementArrayTemplate<void *>* GetDirectArrayVoid ( const char* pName, bool* pStatus = NULL )
		FbxString lName( pName )
		for( int i = 0
 i < mDataNames.GetCount()
 ++i )
			if( *mDataNames[i] == lName )
				return GetDirectArrayVoid(i, pStatus)
		if (pStatus) *pStatus = false
		return (FbxLayerElementArrayTemplate<void *>*)NULL
	const FbxLayerElementArrayTemplate<void*>* GetDirectArrayVoid ( const char* pName, bool* pStatus = NULL ) const
		FbxString lName( pName )
		for( int i = 0
 i < mDataNames.GetCount()
 ++i )
			if( *mDataNames[i] == lName )
				return GetDirectArrayVoid(i, pStatus)
		if (pStatus) *pStatus = false
		return (const FbxLayerElementArrayTemplate<void*>*)NULL
	FbxDataType GetDataType( int pIndex ) const
		if( pIndex < 0 || pIndex >= mDataTypes.GetCount() )
			return FbxUndefinedDT
		return mDataTypes[pIndex]
	FbxDataType GetDataType( const char* pName ) const
		FbxString lName( pName )
		for( int i = 0
 i < mDataNames.GetCount()
 ++i )
			if( *mDataNames[i] == lName )
				return mDataTypes[i]
		return FbxUndefinedDT
	const char* GetDataName( int pIndex ) const
		if( pIndex >= 0 && pIndex < mDataNames.GetCount() )
			return mDataNames[pIndex]->Buffer()
		return NULL
	void ResizeAllDirectArrays( int pSize )
		for( int i = 0
 i < GetDirectArray().GetCount()
 ++i )
			switch( mDataTypes[i].GetType() )
				case eFbxBool:	FbxGetDirectArray<bool>(this,i).Resize( pSize )  
 break
				case eFbxInt:	FbxGetDirectArray<int>(this,i).Resize( pSize )   
	break
				case eFbxFloat:	FbxGetDirectArray<float>(this,i).Resize( pSize ) 
	break
				case eFbxDouble:	FbxGetDirectArray<double>(this,i).Resize( pSize )
	break
	break
	break
	break
				default:
					FBX_ASSERT_NOW("unknown type" )
 break
	void RemoveFromAllDirectArrays( int pIndex )
		for( int i = 0
 i < GetDirectArray().GetCount()
 ++i )
			switch( mDataTypes[i].GetType() )
				case eFbxBool:	FbxGetDirectArray<bool>(this,i).RemoveAt( pIndex )  
 break
				case eFbxInt:	FbxGetDirectArray<int>(this,i).RemoveAt( pIndex )   
 break
				case eFbxFloat:	FbxGetDirectArray<float>(this,i).RemoveAt( pIndex ) 
 break
				case eFbxDouble:	FbxGetDirectArray<double>(this,i).RemoveAt( pIndex )
 break
	break
	break
	break
				default:
					FBX_ASSERT_NOW("unknown type" )
 break
	int GetArrayCount( int pIndex ) const 
		if( pIndex >= 0 && pIndex < GetDirectArray().GetCount() )
			switch( mDataTypes[pIndex].GetType() )
				case eFbxBool:	return FbxGetDirectArray<bool>(this,pIndex).GetCount()
				case eFbxInt:	return FbxGetDirectArray<int>(this,pIndex).GetCount()
				case eFbxFloat:	return FbxGetDirectArray<float>(this,pIndex).GetCount()
				case eFbxDouble:	return FbxGetDirectArray<double>(this,pIndex).GetCount()
				default:
					FBX_ASSERT_NOW("Unknown type" )
 break
		return -1
	int GetId() const 
 return mId
	int GetDirectArrayCount() const 
 return GetDirectArray().GetCount()
    FbxLayerElementUserData& operator=( FbxLayerElementUserData const& pOther )
        if (this == &pOther)
            return *this
        Clear()
        mId = pOther.mId
        mDataTypes = pOther.mDataTypes
        mDataNames.Resize(pOther.mDataNames.GetCount())
        for(int i = 0
 i < pOther.mDataNames.GetCount()
 ++i)
            mDataNames.SetAt(i,  FbxNew< FbxString >( *pOther.mDataNames[i] ) )
        Init()
        for(int i = 0
 i < pOther.GetDirectArrayCount()
 ++i)
            switch (mDataTypes[i].GetType())
            case eFbxBool:
                FbxGetDirectArray<bool>(this, i) = FbxGetDirectArray<bool>(&pOther, i)
                break
            case eFbxInt:
                FbxGetDirectArray<int>(this, i) = FbxGetDirectArray<int>(&pOther, i)
                break
            case eFbxFloat:
                FbxGetDirectArray<float>(this, i) = FbxGetDirectArray<float>(&pOther, i)
                break
            case eFbxDouble:
                FbxGetDirectArray<double>(this, i) = FbxGetDirectArray<double>(&pOther, i)
                break
            default:
                FBX_ASSERT_NOW("Unknown type" )
                break
        if ( ( mReferenceMode == FbxLayerElement::eIndex || 
               mReferenceMode == FbxLayerElement::eIndexToDirect) &&
             ( pOther.GetReferenceMode() == FbxLayerElement::eIndex || 
               pOther.GetReferenceMode()  == FbxLayerElement::eIndexToDirect))
            GetIndexArray() = pOther.GetIndexArray()
        return *this
	bool Clear()
		int i
		const int lCount = GetDirectArray().GetCount()
		FbxLayerElementArray** directArray = NULL
		directArray = GetDirectArray().GetLocked(directArray)
		for( i = 0
 directArray != NULL && i < lCount
 ++i )
			if( directArray[i] )
				FbxDelete(directArray[i])
		FbxLayerElementArray*** ptr = &directArray
		GetDirectArray().Release((void**)ptr)
		for( i = 0
 i < mDataNames.GetCount()
 ++i )
			FBX_SAFE_DELETE(mDataNames[i])
		mDataNames.Clear()
		mDataTypes.Clear()
		FbxLayerElementTemplate<void*>::Clear()
        return true
	virtual int MemorySize() const
		int size = FbxLayerElementTemplate<void*>::MemorySize()
		size += sizeof(mId)
        for(int i = 0
 i < mDataTypes.GetCount()
 i++)
            size += sizeof(mDataTypes[i])
        size += (mDataNames.GetCount() * sizeof(FbxString*))
		return size
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayerElementUserData( int pId, FbxArray<FbxDataType>& pDataTypes, FbxArray<const char*>& pDataNames )
		:
		mId( pId ),
		mDataTypes( pDataTypes )
		FBX_ASSERT( pDataTypes.GetCount() == pDataNames.GetCount() )
		for( int i = 0
 i < pDataNames.GetCount()
 ++i )
			mDataNames.Add( FbxNew< FbxString >( pDataNames[i] ) )
    FbxLayerElementUserData( FbxLayerElementUserData const& pOther ) : mId(pOther.mId), mDataTypes(pOther.mDataTypes)
        for (int lIndex = 0
 lIndex < pOther.mDataNames.GetCount()
 ++lIndex)
            mDataNames.Add(FbxNew<FbxString>(*(pOther.mDataNames[lIndex])))
        SetType(&FbxLayerElementUserDataDT)
        AllocateArrays()
        for(int i = 0
 i < pOther.GetDirectArrayCount()
 ++i)
            switch (mDataTypes[i].GetType())
            case eFbxBool:
                FbxGetDirectArray<bool>(this, i) = FbxGetDirectArray<bool>(&pOther, i)
                break
            case eFbxInt:
                FbxGetDirectArray<int>(this, i) = FbxGetDirectArray<int>(&pOther, i)
                break
            case eFbxFloat:
                FbxGetDirectArray<float>(this, i) = FbxGetDirectArray<float>(&pOther, i)
                break
            case eFbxDouble:
                FbxGetDirectArray<double>(this, i) = FbxGetDirectArray<double>(&pOther, i)
                break
            default:
                FBX_ASSERT_NOW("Unknown type" )
                break
        if ( ( mReferenceMode == FbxLayerElement::eIndex || 
            mReferenceMode == FbxLayerElement::eIndexToDirect) &&
            ( pOther.GetReferenceMode() == FbxLayerElement::eIndex || 
            pOther.GetReferenceMode()  == FbxLayerElement::eIndexToDirect))
            GetIndexArray() = pOther.GetIndexArray()
	~FbxLayerElementUserData()
		Clear()
	virtual void AllocateArrays()
		FbxLayerElementTemplate<void*>::AllocateArrays()
		Init()
private:
	void Init()
	    int i
		GetDirectArray().Resize( mDataTypes.GetCount() )
		for( i = 0
 i < mDataTypes.GetCount()
 ++i )
			FbxHandle** dst = NULL
			dst = GetDirectArray().GetLocked(dst)
			if (dst)
				switch( mDataTypes[i].GetType() )
					case eFbxBool:	dst[i] = (FbxHandle*)FbxNew< FbxLayerElementArrayTemplate<bool> >(mDataTypes[i].GetType())
	break
					case eFbxInt:	dst[i] = (FbxHandle*)FbxNew< FbxLayerElementArrayTemplate<int> >(mDataTypes[i].GetType())
	break
					case eFbxFloat:	dst[i] = (FbxHandle*)FbxNew< FbxLayerElementArrayTemplate<float> >(mDataTypes[i].GetType())
	break
					case eFbxDouble:	dst[i] = (FbxHandle*)FbxNew< FbxLayerElementArrayTemplate<double> >(mDataTypes[i].GetType())
	break
					default:
						FBX_ASSERT_NOW("Trying to assign an unknown type" )
 break
				FbxHandle*** ptr = &dst
				GetDirectArray().Release((void**)ptr)
	int mId
	FbxArray<FbxDataType> mDataTypes
	FbxArray<FbxString*> mDataNames
#endif 
template <class T>
inline FbxLayerElementArrayTemplate<T>& FbxGetDirectArray( FbxLayerElementUserData *pLayerElement,int pIndex, bool* pStatus)
	return *(FbxLayerElementArrayTemplate<T>*)pLayerElement->GetDirectArrayVoid(pIndex,pStatus)
template <class T>
inline FbxLayerElementArrayTemplate<T> const& FbxGetDirectArray(FbxLayerElementUserData const *pLayerElement, int pIndex, bool* pStatus)
	return *(const FbxLayerElementArrayTemplate<T>*)pLayerElement->GetDirectArrayVoid(pIndex,pStatus)
template <class T>
inline FbxLayerElementArrayTemplate<T>& FbxGetDirectArray( FbxLayerElementUserData *pLayerElement,const char* pName, bool* pStatus )
	return *(FbxLayerElementArrayTemplate<T>*)pLayerElement->GetDirectArrayVoid(pName,pStatus)
template <class T>
inline FbxLayerElementArrayTemplate<T> const& FbxGetDirectArray(FbxLayerElementUserData const *pLayerElement, const char* pName, bool* pStatus )
	return *(const FbxLayerElementArrayTemplate<T>*)pLayerElement->GetDirectArrayVoid(pName,pStatus)
class FBXSDK_DLL FbxLayerElementSmoothing : public FbxLayerElementTemplate<int>
public:
    FBXSDK_FRIEND_NEW()
	static FbxLayerElementSmoothing* Create(FbxLayerContainer* pOwner, const char* pName)
	void SetReferenceMode( FbxLayerElement::EReferenceMode pMode )
		if( pMode != FbxLayerElement::eDirect )
			FBX_ASSERT_NOW( "Smoothing layer elements must be direct mapped" )
			return
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayerElementSmoothing()
		mReferenceMode = FbxLayerElement::eDirect
#endif 
class FBXSDK_DLL FbxLayerElementCrease : public FbxLayerElementTemplate<double>
public:
    FBXSDK_FRIEND_NEW()
	static FbxLayerElementCrease* Create(FbxLayerContainer* pOwner, const char* pName)
	void SetReferenceMode( FbxLayerElement::EReferenceMode pMode )
		if( pMode != FbxLayerElement::eDirect )
			FBX_ASSERT_NOW( "Crease layer elements must be direct mapped" )
			return
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayerElementCrease()
		mReferenceMode = FbxLayerElement::eDirect
#endif 
class FBXSDK_DLL FbxLayerElementHole : public FbxLayerElementTemplate<bool>
public:
    FBXSDK_FRIEND_NEW()
        static FbxLayerElementHole* Create(FbxLayerContainer* pOwner, const char* pName)
    void SetReferenceMode( FbxLayerElement::EReferenceMode pMode )
        if( pMode != FbxLayerElement::eDirect )
            FBX_ASSERT_NOW( "hole layer elements must be direct mapped" )
            return
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    FbxLayerElementHole()
        mReferenceMode = FbxLayerElement::eDirect
#endif 
class FBXSDK_DLL FbxLayerElementVisibility : public FbxLayerElementTemplate<bool>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementVisibility)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayerElementVisibility()
	~FbxLayerElementVisibility()
#endif 
class FBXSDK_DLL FbxLayerElementTexture : public FbxLayerElementTemplate<FbxTexture*>
public:
	FBXSDK_LAYER_ELEMENT_CREATE_DECLARE(LayerElementTexture)
	enum EBlendMode
		eTranslucent,
		eAdd,
		eModulate,
		eModulate2,
        eOver,
        eNormal,		
        eDissolve,
        eDarken,			
        eColorBurn,
        eLinearBurn, 	
        eDarkerColor,
        eLighten,			
        eScreen,		
        eColorDodge,
        eLinearDodge,
        eLighterColor,
        eSoftLight,		
        eHardLight,		
        eVividLight,
        eLinearLight,
        ePinLight, 		
        eHardMix,		
        eDifference, 		
        eExclusion, 		
        eSubtract,
        eDivide,
        eHue, 			
        eSaturation,		
        eColor,		
        eLuminosity,
        eOverlay,
		eBlendModeCount
	void       SetBlendMode(EBlendMode pBlendMode) 
 mBlendMode = pBlendMode
    void       SetAlpha(double pAlpha)
        if (pAlpha > 1.0)
            mAlpha = 1.0
        else if (pAlpha < 0.0)
            mAlpha = 0.0
        else
            mAlpha = pAlpha
	EBlendMode GetBlendMode() const                      
 return mBlendMode
	double     GetAlpha() const                          
 return mAlpha
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual int MemorySize() const
		int size = FbxLayerElementTemplate<FbxTexture*>::MemorySize()
		size += sizeof(mBlendMode)
		size += sizeof(mAlpha)
		return size
protected:
	FbxLayerElementTexture() : mBlendMode(eTranslucent)
		mReferenceMode = eIndexToDirect
		mAlpha         = 1.0
private:
	EBlendMode mBlendMode
	double     mAlpha
#endif 
class FBXSDK_DLL FbxLayer
public:
    FBXSDK_FRIEND_NEW()
	FbxLayerElementNormal* GetNormals()
	const FbxLayerElementNormal* GetNormals() const
    FbxLayerElementTangent* GetTangents()
    const FbxLayerElementTangent* GetTangents() const
    FbxLayerElementBinormal* GetBinormals()
    const FbxLayerElementBinormal* GetBinormals() const
	FbxLayerElementMaterial* GetMaterials()
	const FbxLayerElementMaterial* GetMaterials() const
	FbxLayerElementPolygonGroup* GetPolygonGroups()
	const FbxLayerElementPolygonGroup* GetPolygonGroups() const
	FbxLayerElementUV* GetUVs(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
	const FbxLayerElementUV* GetUVs(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse) const
	int GetUVSetCount() const
	FbxArray<FbxLayerElement::EType> GetUVSetChannels() const
	FbxArray<const FbxLayerElementUV*> GetUVSets() const
	FbxLayerElementVertexColor* GetVertexColors()
	const FbxLayerElementVertexColor* GetVertexColors() const
	FbxLayerElementSmoothing* GetSmoothing()
	const FbxLayerElementSmoothing* GetSmoothing() const
	FbxLayerElementCrease* GetVertexCrease()
	const FbxLayerElementCrease* GetVertexCrease() const
    FbxLayerElementCrease* GetEdgeCrease()
    const FbxLayerElementCrease* GetEdgeCrease() const
    FbxLayerElementHole* GetHole()
    const FbxLayerElementHole* GetHole() const
	FbxLayerElementUserData* GetUserData()
	const FbxLayerElementUserData* GetUserData() const
	FbxLayerElementVisibility* GetVisibility()
	const FbxLayerElementVisibility* GetVisibility() const
	FbxLayerElementTexture* GetTextures(FbxLayerElement::EType pType)
	const FbxLayerElementTexture* GetTextures(FbxLayerElement::EType pType) const
    void SetTextures(FbxLayerElement::EType pType, FbxLayerElementTexture* pTextures)
	FbxLayerElement* GetLayerElementOfType(FbxLayerElement::EType pType, bool pIsUV=false)
	const FbxLayerElement* GetLayerElementOfType(FbxLayerElement::EType pType, bool pIsUV=false) const
	void SetNormals(FbxLayerElementNormal* pNormals)
    void SetBinormals(FbxLayerElementBinormal* pBinormals)
    void SetTangents(FbxLayerElementTangent* pTangents)
	void SetMaterials(FbxLayerElementMaterial* pMaterials)
	void SetPolygonGroups(FbxLayerElementPolygonGroup* pPolygonGroups)
	void SetUVs(FbxLayerElementUV* pUVs, FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse)
	void SetVertexColors (FbxLayerElementVertexColor* pVertexColors)
	void SetSmoothing (FbxLayerElementSmoothing* pSmoothing)
	void SetVertexCrease (FbxLayerElementCrease* pCrease)
    void SetEdgeCrease (FbxLayerElementCrease* pCrease)
    void SetHole (FbxLayerElementHole* pHole)
	void SetUserData (FbxLayerElementUserData* pUserData)
	void SetVisibility( FbxLayerElementVisibility* pVisibility )
	void SetLayerElementOfType(FbxLayerElement* pLayerElement, FbxLayerElement::EType pType, bool pIsUV=false)
	FbxLayerElement* CreateLayerElementOfType(FbxLayerElement::EType pType, bool pIsUV=false)
	void Clone(FbxLayer const& pSrcLayer)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxLayer& operator=(FbxLayer const& pSrcLayer)
private:
	FbxLayer(FbxLayerContainer& pOwner)
	virtual ~FbxLayer()
	void Clear()
	FbxLayerContainer& mOwner
    FbxLayerElement*             mNonTexturesArray[FbxLayerElement::sTypeNonTextureCount]
    FbxLayerElementUV*           mUVsArray[FbxLayerElement::sTypeTextureCount]
    FbxLayerElementTexture*      mTexturesArray[FbxLayerElement::sTypeTextureCount]
	friend class FbxLayerContainer
public:
		bool ContentWriteTo(FbxStream& pStream) const
		bool ContentReadFrom(const FbxStream& pStream)
	virtual int MemoryUsage() const
#endif 
#define FBXSDK_FOR_EACH_TEXTURE(lLayerIndex) for((lLayerIndex)=0
(lLayerIndex)<FbxLayerElement::sTypeTextureCount
(lLayerIndex)++)
#define FBXSDK_FOR_EACH_NON_TEXTURE(lLayerIndex) for((lLayerIndex)=0
(lLayerIndex)<FbxLayerElement::sTypeNonTextureCount
(lLayerIndex)++)
#define FBXSDK_TEXTURE_INDEX(ElementType) (int(ElementType)-FbxLayerElement::sTypeTextureStartIndex)
#define FBXSDK_TEXTURE_TYPE(TextureIndex) (FbxLayerElement::EType((TextureIndex)+FbxLayerElement::sTypeTextureStartIndex))
#define FBXSDK_NON_TEXTURE_INDEX(ElementType) (int(ElementType)-FbxLayerElement::sTypeNonTextureStartIndex)
#define FBXSDK_NON_TEXTURE_TYPE(Index) (FbxLayerElement::EType((Index)+FbxLayerElement::sTypeNonTextureStartIndex))
typedef FbxLayerElement FbxGeometryElement
typedef FbxLayerElementNormal FbxGeometryElementNormal
typedef FbxLayerElementBinormal FbxGeometryElementBinormal
typedef FbxLayerElementTangent FbxGeometryElementTangent
typedef FbxLayerElementMaterial FbxGeometryElementMaterial
typedef FbxLayerElementPolygonGroup FbxGeometryElementPolygonGroup
typedef FbxLayerElementUV FbxGeometryElementUV
typedef FbxLayerElementVertexColor FbxGeometryElementVertexColor
typedef FbxLayerElementUserData FbxGeometryElementUserData
typedef FbxLayerElementSmoothing FbxGeometryElementSmoothing
typedef FbxLayerElementCrease FbxGeometryElementCrease
typedef FbxLayerElementHole FbxGeometryElementHole
typedef FbxLayerElementVisibility FbxGeometryElementVisibility
#undef FBXSDK_LAYER_ELEMENT_CREATE_DECLARE
#include <fbxsdk/fbxsdk_nsend.h>
#endif 