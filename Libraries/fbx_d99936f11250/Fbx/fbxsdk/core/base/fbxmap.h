#ifndef _FBXSDK_CORE_BASE_MAP_H_
#define _FBXSDK_CORE_BASE_MAP_H_
class FbxObject;
template <typename Type> struct FbxLessCompare
{
inline int operator()(const Type& pLeft, const Type& pRight) const
{
return (pLeft < pRight) ? -1 : ((pRight < pLeft) ? 1 : 0);
}
};
template <typename Key, typename Type, typename Compare=FbxLessCompare<Key>, typename Allocator=FbxBaseAllocator> class FbxMap
{
protected:
class KeyValuePair : private FbxPair<const Key, Type>
{
#ifndef DOXYGEN_SHOULD_SKIP_THIS
public:
typedef const Key KeyType;
typedef const Key ConstKeyType;
typedef Type  ValueType;
typedef const Type ConstValueType;
KeyValuePair(const Key& pFirst, const Type& pSecond) : FbxPair<const Key, Type>(pFirst, pSecond){}
ConstKeyType& GetKey() const { return this->mFirst; }
KeyType& GetKey(){ return this->mFirst; }
ConstValueType& GetValue() const { return this->mSecond; }
ValueType& GetValue(){ return this->mSecond; }
#endif
};
typedef FbxRedBlackTree<KeyValuePair, Compare, Allocator> StorageType;
public:
typedef Type         ValueType;
typedef Key          KeyType;
typedef typename StorageType::RecordType  RecordType;
typedef typename StorageType::IteratorType  Iterator;
typedef typename StorageType::ConstIteratorType ConstIterator;
inline void Reserve(unsigned int pRecordCount)
{
mTree.Reserve(pRecordCount);
}
inline int GetSize() const
{
return mTree.GetSize();
}
inline FbxPair<RecordType*, bool> Insert(const KeyType& pKey, const ValueType& pValue)
{
return mTree.Insert(KeyValuePair(pKey, pValue));
}
inline bool Remove(const KeyType& pKey)
{
return mTree.Remove(pKey);
}
inline void Clear()
{
mTree.Clear();
}
inline bool Empty() const
{
return mTree.Empty();
}
Iterator Begin()
{
return Iterator(Minimum());
}
Iterator End()
{
return Iterator();
}
ConstIterator Begin() const
{
return ConstIterator(Minimum());
}
ConstIterator End() const
{
return ConstIterator();
}
inline const RecordType* Find(const KeyType& pKey) const
{
return mTree.Find(pKey);
}
inline RecordType* Find(const KeyType& pKey)
{
return mTree.Find(pKey);
}
inline const RecordType* UpperBound(const KeyType& pKey) const
{
return mTree.UpperBound(pKey);
}
inline RecordType* UpperBound(const KeyType& pKey)
{
return mTree.UpperBound(pKey);
}
inline ValueType& operator[](const KeyType& pKey)
{
RecordType* lRecord = Find(pKey);
if( !lRecord )
{
lRecord = Insert(pKey, ValueType()).mFirst;
}
return lRecord->GetValue();
}
inline const RecordType* Minimum() const
{
return mTree.Minimum();
}
inline RecordType* Minimum()
{
return mTree.Minimum();
}
inline const RecordType* Maximum() const
{
return mTree.Maximum();
}
inline RecordType* Maximum()
{
return mTree.Maximum();
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline FbxMap(){}
inline FbxMap(const FbxMap& pMap) : mTree(pMap.mTree){}
inline ~FbxMap(){ Clear(); }
private:
StorageType mTree;
#endif
};
template <class Key, class Type, class Compare> class FBXSDK_DLL FbxSimpleMap
{
public:
typedef typename FbxMap<Key, Type, Compare>::RecordType* Iterator;
inline void Add(const Key& pKey, const Type& pValue)
{
mMap.Insert(pKey, pValue);
}
inline Iterator Find(const Key& pKey) const
{
return (Iterator)mMap.Find(pKey);
}
inline Iterator Find(const Type& pValue) const
{
Iterator lIterator = GetFirst();
while( lIterator )
{
if( lIterator->GetValue() == pValue )
{
return lIterator;
}
lIterator = GetNext(lIterator);
}
return 0;
}
inline void Remove(Iterator pIterator)
{
if( pIterator ) mMap.Remove(pIterator->GetKey());
}
inline Iterator GetFirst() const
{
return (Iterator)mMap.Minimum();
}
inline Iterator GetNext(Iterator pIterator) const
{
return (Iterator)pIterator ? pIterator->Successor() : 0;
}
inline void Clear()
{
mMap.Clear();
}
inline void Reserve(int pSize)
{
mMap.Reserve(pSize);
}
inline int GetCount() const
{
return mMap.GetSize();
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline FbxSimpleMap(){}
private:
FbxMap<Key, Type, Compare> mMap;
#endif
};
template <class Type, class Compare> class FBXSDK_DLL FbxObjectMap : public FbxSimpleMap<Type, FbxObject*, Compare>
{
public:
inline FbxObjectMap(){}
inline FbxObject* Get(typename FbxSimpleMap<Type, FbxObject*, Compare>::Iterator pIterator)
{
return pIterator ? pIterator->GetValue() : 0;
}
};
class FBXSDK_DLL FbxObjectStringMap : public FbxObjectMap<FbxString, FbxStringCompare>
{
public:
inline FbxObjectStringMap(){}
};
template <typename K, typename V, typename C, typename A> inline void FbxMapFree(FbxMap<K, V, C, A>& pMap)
{
for( typename FbxMap<K, V, C, A>::Iterator i = pMap.Begin(); i != pMap.End(); ++i )
{
FbxFree(i->GetValue());
}
pMap.Clear();
}
template <typename K, typename V, typename C, typename A> inline void FbxMapDelete(FbxMap<K, V, C, A>& pMap)
{
for( typename FbxMap<K, V, C, A>::Iterator i = pMap.Begin(); i != pMap.End(); ++i )
{
FbxDelete(i->GetValue());
}
pMap.Clear();
}
template <typename K, typename V, typename C, typename A> inline void FbxMapDestroy(FbxMap<K, V, C, A>& pMap)
{
for( typename FbxMap<K, V, C, A>::Iterator i = pMap.Begin(); i != pMap.End(); ++i )
{
i->GetValue()->Destroy();
}
pMap.Clear();
}
template class FbxSimpleMap<FbxString, FbxObject*, FbxStringCompare>;
template class FbxObjectMap<FbxString, FbxStringCompare>;
#endif