#ifndef _FBXSDK_CORE_BASE_ARRAY_H_
#define _FBXSDK_CORE_BASE_ARRAY_H_
template <class T> class FbxArray
{
public:
typedef int (*CompareFunc)(const void*, const void*);
FbxArray() : mSize(0), mCapacity(0), mArray(NULL){}
FbxArray(const int pCapacity) : mSize(0), mCapacity(0), mArray(NULL){ if( pCapacity > 0 ) Reserve(pCapacity); }
FbxArray(const FbxArray& pArray) : mSize(0), mCapacity(0), mArray(NULL){ *this = pArray; }
~FbxArray(){ Clear(); }
inline int InsertAt(const int pIndex, const T& pElement, bool pCompact=false)
{
FBX_ASSERT_RETURN_VALUE(pIndex >= 0, -1);
int lIndex = FbxMin(pIndex, mSize);
if( mSize >= mCapacity )
{
T lElement = pElement;
int lNewCapacity = FbxMax(pCompact ? mCapacity + 1 : mCapacity * 2, 1);
T* lArray = Allocate(lNewCapacity);
FBX_ASSERT_RETURN_VALUE(lArray, -1);
mArray = lArray;
mCapacity = lNewCapacity;
return InsertAt(pIndex, lElement);
}
if( lIndex < mSize )
{
if( (&pElement >= &mArray[lIndex]) && (&pElement < &mArray[mSize]) )
{
T lElement = pElement;
return InsertAt(pIndex, lElement);
}
memmove(&mArray[lIndex + 1], &mArray[lIndex], (mSize - lIndex) * sizeof(T));
}
memcpy(&mArray[lIndex], &pElement, sizeof(T));
mSize++;
return lIndex;
}
inline int Add(const T& pElement)
{
return InsertAt(mSize, pElement);
}
inline int AddUnique(const T& pElement)
{
int lIndex = Find(pElement);
return ( lIndex == -1 ) ? Add(pElement) : lIndex;
}
inline int AddCompact(const T& pElement)
{
return InsertAt(mSize, pElement, true);
}
inline int Size() const { return mSize; }
inline int Capacity() const { return mCapacity; }
inline T& operator[](const int pIndex) const
{
#ifdef _DEBUG
FBX_ASSERT_MSG(pIndex >= 0, "Index is out of range!");
if( pIndex >= mSize )
{
if( pIndex < mCapacity )
{
FBX_ASSERT_NOW("Index is out of range, but not outside of capacity! Call SetAt() to use reserved memory.");
}
else FBX_ASSERT_NOW("Index is out of range!");
}
#endif
return (T&)mArray[pIndex];
}
inline T GetAt(const int pIndex) const
{
return operator[](pIndex);
}
inline T GetFirst() const
{
return GetAt(0);
}
inline T GetLast() const
{
return GetAt(mSize-1);
}
inline int Find(const T& pElement, const int pStartIndex=0) const
{
FBX_ASSERT_RETURN_VALUE(pStartIndex >= 0, -1);
for( int i = pStartIndex; i < mSize; ++i )
{
if( operator[](i) == pElement ) return i;
}
return -1;
}
inline int FindReverse(const T& pElement, const int pStartIndex=FBXSDK_INT_MAX) const
{
for( int i = FbxMin(pStartIndex, mSize-1); i >= 0; --i )
{
if( operator[](i) == pElement ) return i;
}
return -1;
}
inline bool Reserve(const int pCapacity)
{
FBX_ASSERT_RETURN_VALUE(pCapacity > 0, false);
if( pCapacity > mCapacity )
{
T* lArray = Allocate(pCapacity);
FBX_ASSERT_RETURN_VALUE(lArray, false);
mArray = lArray;
mCapacity = pCapacity;
memset(&mArray[mSize], 0, (mCapacity - mSize) * sizeof(T));
}
return true;
}
inline void SetAt(const int pIndex, const T& pElement)
{
FBX_ASSERT_RETURN(pIndex < mCapacity);
if( pIndex >= mSize ) mSize = pIndex + 1;
if( mArray ) memcpy(&mArray[pIndex], &pElement, sizeof(T));
}
inline void SetFirst(const T& pElement)
{
SetAt(0, pElement);
}
inline void SetLast(const T& pElement)
{
SetAt(mSize-1, pElement);
}
inline T RemoveAt(const int pIndex)
{
T lElement = GetAt(pIndex);
if( pIndex + 1 < mSize )
{
memmove(&mArray[pIndex], &mArray[pIndex + 1], (mSize - pIndex - 1) * sizeof(T));
}
mSize--;
return lElement;
}
inline T RemoveFirst()
{
return RemoveAt(0);
}
inline T RemoveLast()
{
return RemoveAt(mSize-1);
}
inline bool RemoveIt(const T& pElement)
{
int Index = Find(pElement);
if( Index >= 0 )
{
RemoveAt(Index);
return true;
}
return false;
}
inline void RemoveRange(const int pIndex, const int pCount)
{
FBX_ASSERT_RETURN(pIndex >= 0);
FBX_ASSERT_RETURN(pCount >= 0);
if( pIndex + pCount < mSize )
{
memmove(&mArray[pIndex], &mArray[pIndex + pCount], (mSize - pIndex - pCount) * sizeof(T));
}
mSize -= pCount;
}
inline bool Resize(const int pSize)
{
if( pSize == mSize && mSize == mCapacity ) return true;
if( pSize == 0 )
{
Clear();
return true;
}
FBX_ASSERT_RETURN_VALUE(pSize > 0, false);
if( pSize != mCapacity )
{
T* lArray = Allocate(pSize);
FBX_ASSERT_RETURN_VALUE(lArray, false);
mArray = lArray;
}
if( pSize > mCapacity )
{
memset(&mArray[mSize], 0, (pSize - mSize) * sizeof(T));
}
mSize = pSize;
mCapacity = pSize;
return true;
}
inline bool Grow(const int pSize)
{
return Resize(mSize + pSize);
}
inline bool Shrink(const int pSize)
{
return Resize(mSize - pSize);
}
inline bool Compact()
{
return Resize(mSize);
}
inline void Clear()
{
if( mArray != NULL )
{
mSize = 0;
mCapacity = 0;
FbxFree(mArray);
mArray = NULL;
}
}
inline void Sort(CompareFunc pCompareFunc)
{
qsort(mArray, mSize, sizeof(T), pCompareFunc);
}
inline T* GetArray() const { return mArray ? (T*)mArray : NULL; }
inline operator T* (){ return mArray ? (T*)mArray : NULL; }
inline void AddArray(const FbxArray<T>& pOther)
{
if( Grow(pOther.mSize) )
{
memcpy(&mArray[mSize - pOther.mSize], pOther.mArray, pOther.mSize * sizeof(T));
}
}
inline void AddArrayNoDuplicate(const FbxArray<T>& pOther)
{
for( int i = 0, c = pOther.mSize; i < c; ++i )
{
AddUnique(pOther[i]);
}
}
inline void RemoveArray(const FbxArray<T>& pOther)
{
for( int i = 0, c = pOther.mSize; i < c; ++i )
{
RemoveIt(pOther[i]);
}
}
inline FbxArray<T>& operator=(const FbxArray<T>& pOther)
{
if( this != &pOther )
{
if( Resize(pOther.mSize) )
{
memcpy(mArray, pOther.mArray, pOther.mSize * sizeof(T));
}
}
return *this;
}
inline bool operator==(const FbxArray<T>& pOther) const
{
if( this == &pOther ) return true;
if( mSize != pOther.mSize ) return false;
return memcmp(mArray, pOther.mArray, sizeof(T) * mSize) == 0;
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline int GetCount() const { return mSize; }
private:
inline T* Allocate(const int pCapacity)
{
return (T*)FbxRealloc(mArray, pCapacity * sizeof(T));
}
int mSize;
int mCapacity;
T* mArray;
#if defined(FBXSDK_COMPILER_MSC)
FBX_ASSERT_STATIC(FBXSDK_IS_SIMPLE_TYPE(T) || __is_enum(T) || (__has_trivial_constructor(T)&&__has_trivial_destructor(T)) || !FBXSDK_IS_INCOMPATIBLE_WITH_ARRAY(T));
#endif
#endif
};
template <class T> inline void FbxArrayFree(FbxArray<T>& pArray)
{
for( int i = 0, c = pArray.Size(); i < c; ++i )
{
FbxFree(pArray[i]);
}
pArray.Clear();
}
template <class T> inline void FbxArrayDelete(FbxArray<T>& pArray)
{
for( int i = 0, c = pArray.Size(); i < c; ++i )
{
FbxDelete(pArray[i]);
}
pArray.Clear();
}
template <class T> inline void FbxArrayDestroy(FbxArray<T>& pArray)
{
for( int i = 0, c = pArray.Size(); i < c; ++i )
{
(pArray[i])->Destroy();
}
pArray.Clear();
}
template <class T> FBXSDK_INCOMPATIBLE_WITH_ARRAY_TEMPLATE(FbxArray<T>);
#endif