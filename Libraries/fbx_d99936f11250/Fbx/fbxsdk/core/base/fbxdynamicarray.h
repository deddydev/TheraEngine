#ifndef _FBXSDK_CORE_BASE_DYNAMICARRAY_H_
#define _FBXSDK_CORE_BASE_DYNAMICARRAY_H_
template <typename Type, typename Allocator=FbxBaseAllocator> class FbxDynamicArray
{
public:
FbxDynamicArray() :
mArray(NULL),
mCapacity(0),
mSize(0),
mAllocator(sizeof(Type))
{
}
FbxDynamicArray(const size_t pInitialSize) :
mArray(NULL),
mCapacity(0),
mSize(0),
mAllocator(sizeof(Type))
{
Reserve(pInitialSize);
}
FbxDynamicArray(const FbxDynamicArray& pArray) :
mArray(NULL),
mCapacity(0),
mSize(0),
mAllocator(sizeof(Type))
{
Reserve(pArray.mCapacity);
CopyArray(mArray, pArray.mArray, pArray.mSize);
mSize = pArray.mSize;
}
~FbxDynamicArray()
{
for( size_t i = 0; i < mSize; ++i )
{
mArray[i].~Type();
}
mAllocator.FreeMemory(mArray);
}
size_t Capacity() const
{
return mCapacity;
}
size_t Size() const
{
return mSize;
}
void Reserve(const size_t pCount)
{
if( pCount > mCapacity )
{
Type* lNewArray = (Type*)mAllocator.AllocateRecords(pCount);
MoveArray(lNewArray, mArray, mSize);
mAllocator.FreeMemory(mArray);
mArray = lNewArray;
mCapacity = pCount;
}
}
void PushBack(const Type& pItem, const size_t pNCopies = 1)
{
if( mSize + pNCopies > mCapacity )
{
size_t lNewSize = mCapacity + mCapacity / 2;
if( mSize + pNCopies > lNewSize )
{
lNewSize = mSize + pNCopies;
}
Reserve(lNewSize);
}
FBX_ASSERT(mSize + pNCopies <= mCapacity);
Fill(mArray + mSize, pItem, pNCopies);
mSize += pNCopies;
}
void Insert(const size_t pIndex, const Type& pItem, const size_t pNCopies=1)
{
FBX_ASSERT(pIndex >= 0);
FBX_ASSERT(pIndex <= mSize);
Type lValue = pItem;
if( pNCopies == 0 )
{
}
else if( pIndex >= mSize )
{
PushBack(pItem, pNCopies);
}
else if( mSize + pNCopies > mCapacity )
{
size_t lNewSize = mCapacity + mCapacity / 2;
if( mSize + pNCopies > lNewSize )
{
lNewSize = mSize + pNCopies;
}
Type* lNewArray = (Type*)mAllocator.AllocateRecords(lNewSize);
MoveArray(lNewArray, mArray, pIndex);
Fill(lNewArray + pIndex, pItem, pNCopies);
MoveArray(lNewArray + pIndex + pNCopies, mArray + pIndex, mSize - pIndex);
mAllocator.FreeMemory(mArray);
mArray = lNewArray;
mSize += pNCopies;
mCapacity = lNewSize;
}
else
{
MoveArrayBackwards(mArray + pIndex + pNCopies, mArray + pIndex, mSize - pIndex);
Fill(mArray + pIndex, pItem, pNCopies);
mSize += pNCopies;
}
}
void PopBack(size_t pNElements=1)
{
FBX_ASSERT(pNElements <= mSize);
for( size_t i = mSize - pNElements; i < mSize; ++i )
{
mArray[i].~Type();
}
mSize -= pNElements;
}
void Remove(const size_t pIndex, size_t pNElements=1)
{
FBX_ASSERT(pIndex >= 0);
FBX_ASSERT(pIndex <= mSize);
FBX_ASSERT(pIndex + pNElements <= mSize);
if( pIndex + pNElements >= mSize )
{
PopBack(pNElements);
}
else
{
for( size_t i = pIndex; i < pIndex + pNElements; ++i )
{
mArray[i].~Type();
}
MoveOverlappingArray(&mArray[pIndex], &mArray[pIndex + pNElements], mSize - pIndex - pNElements);
mSize -= pNElements;
}
}
Type& operator[](const size_t pIndex)
{
return mArray[pIndex];
}
const Type& operator[](const size_t pIndex) const
{
return mArray[pIndex];
}
Type& First()
{
return operator[](0);
}
const Type& First() const
{
return operator[](0);
}
Type& Last()
{
return operator[](mSize-1);
}
const Type& Last() const
{
return operator[](mSize-1);
}
size_t Find(const Type& pItem, const size_t pStartIndex=0) const
{
for( size_t i = pStartIndex; i < mSize; ++i )
{
if( operator[](i) == pItem ) return i;
}
return -1;
}
FbxDynamicArray& operator=(const FbxDynamicArray& pArray)
{
Reserve(pArray.mCapacity);
CopyArray(mArray, pArray.mArray, pArray.mSize);
mSize = pArray.mSize;
return *this;
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
static void CopyArray(Type* pDest, const Type* pSrc, size_t pCount)
{
for( int i = 0; i < int(pCount); i++ )
{
new(&(pDest[i])) Type(pSrc[i]);
}
}
static void MoveArray(Type* pDest, const Type* pSrc, size_t pCount)
{
for( int i = 0; i < int(pCount); i++ )
{
new(&(pDest[i])) Type(pSrc[i]);
}
for( int i = 0; i < int(pCount); i++ )
{
pSrc[i].~Type();
}
}
static void MoveOverlappingArray(Type* pDest, const Type* pSrc, size_t pCount)
{
for( int i = 0; i < int(pCount); i++ )
{
new(&(pDest[i])) Type(pSrc[i]);
pSrc[i].~Type();
}
}
static void MoveArrayBackwards(Type* pDest, const Type* pSrc, size_t pCount)
{
for( int i = 0; i < int(pCount); ++i )
{
new(&(pDest[pCount-1-i])) Type(pSrc[pCount-1-i]);
pSrc[pCount-1-i].~Type();
}
}
static void Fill(Type* pDest, const Type& pItem, size_t pCount)
{
for( int i = 0; i < int(pCount); i++ )
{
new(&(pDest[i])) Type(pItem);
}
}
Type*  mArray;
size_t  mCapacity;
size_t  mSize;
Allocator mAllocator;
#endif
};
#endif