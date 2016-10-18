#ifndef _FBXSDK_CORE_BASE_SET_H_
#define _FBXSDK_CORE_BASE_SET_H_
template <typename Type, typename Compare=FbxLessCompare<Type>, typename Allocator=FbxBaseAllocator> class FbxSet
{
protected:
class Value
{
#ifndef DOXYGEN_SHOULD_SKIP_THIS
public:
typedef const Type KeyType;
typedef const Type ConstKeyType;
typedef const Type ValueType;
typedef const Type ConstValueType;
inline Value(const Type& pValue) : mValue(pValue){}
inline KeyType& GetKey() const { return mValue; }
inline ConstKeyType& GetKey(){ return mValue; }
inline ValueType& GetValue() const { return mValue; }
inline ConstValueType& GetValue(){ return mValue; }
protected:
ValueType mValue;
private:
Value& operator=(const Value&);
#endif
};
typedef FbxRedBlackTree<Value, Compare, Allocator> StorageType;
public:
typedef Type ValueType;
typedef typename StorageType::RecordType        RecordType;
typedef typename StorageType::IteratorType      Iterator;
typedef typename StorageType::ConstIteratorType ConstIterator;
inline void Reserve(unsigned int pRecordCount)
{
mTree.Reserve(pRecordCount);
}
inline int GetSize() const
{
return mTree.GetSize();
}
inline FbxPair<RecordType*, bool> Insert(const ValueType& pValue)
{
return mTree.Insert(Value(pValue));
}
inline int Remove(const ValueType& pValue)
{
return mTree.Remove(pValue);
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
inline const RecordType* Find(const ValueType& pValue) const
{
return mTree.Find(pValue);
}
inline RecordType* Find(const ValueType& pValue)
{
return mTree.Find(pValue);
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
inline bool operator==(const FbxSet<Type, Compare, Allocator>& pOther) const
{
return (this == &pOther) || (mTree == pOther.mTree);
}
inline bool operator != (const FbxSet<Type, Compare, Allocator>& pOther) const
{
return !(*this == pOther);
}
inline FbxSet Intersect(const FbxSet& pOther) const
{
FbxSet lReturn;
ConstIterator lBegin = Begin();
for (; lBegin != End(); ++lBegin)
{
if (pOther.Find(lBegin->GetValue()) != NULL)
lReturn.Insert(lBegin->GetValue());
}
return lReturn;
}
inline FbxSet Union(const FbxSet& pOther) const
{
FbxSet lReturn(*this);
ConstIterator lBegin = pOther.Begin();
for (; lBegin != End(); ++lBegin)
{
if (Find(lBegin->GetValue()) == NULL)
lReturn.Insert(lBegin->GetValue());
}
return lReturn;
}
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline FbxSet(){}
inline FbxSet(const FbxSet& pSet) : mTree(pSet.mTree){}
inline ~FbxSet(){ Clear(); }
private:
StorageType mTree;
#endif
};
#endif