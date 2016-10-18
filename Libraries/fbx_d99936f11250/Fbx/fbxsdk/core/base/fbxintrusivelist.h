#ifndef _FBXSDK_CORE_BASE_INTRUSIVE_LIST_H_
#define _FBXSDK_CORE_BASE_INTRUSIVE_LIST_H_
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#define FBXSDK_INTRUSIVE_LIST_NODE(Class, NodeCount)\
public: inline FbxListNode<Class>& GetListNode(int index = 0){ return this->mNode[index]; }\
private: FbxListNode<Class> mNode[NodeCount];
template <typename T> class FbxListNode
{
typedef FbxListNode<T> NodeT;
public:
explicit FbxListNode(T* pData = 0):mNext(0),mPrev(0),mData(pData){}
~FbxListNode(){ Disconnect(); }
void Disconnect()
{
if ( mPrev != 0 )
mPrev->mNext = mNext;
if ( mNext != 0 )
mNext->mPrev = mPrev;
mPrev = mNext = 0;
}
NodeT* mNext;
NodeT* mPrev;
T*  mData;
};
template <typename T, int NodeIndex=0> class FbxIntrusiveList
{
public:
typedef T         allocator_type;
typedef T         value_type;
typedef T&        reference;
typedef const T&  const_reference;
typedef T*        pointer;
typedef const T*  const_pointer;
typedef FbxListNode<T> NodeT;
FbxIntrusiveList():mHead(0)
{
mHead.mNext = mHead.mPrev = &mHead;
}
~FbxIntrusiveList()
{
while(!Empty())
Begin().Get()->Disconnect();
};
bool Empty() const
{
return ((mHead.mNext==&mHead)&&(mHead.mPrev==&mHead));
}
void PushBack(T& pElement)
{
NodeT* pNode = &pElement.GetListNode(NodeIndex);
pNode->mData = &pElement;
if (Empty())
{
pNode->mNext = &mHead;
pNode->mPrev = &mHead;
mHead.mNext = pNode;
mHead.mPrev = pNode;
}
else
{
pNode->mNext = &mHead;
pNode->mPrev = mHead.mPrev;
pNode->mPrev->mNext = pNode;
mHead.mPrev = pNode;
}
}
void PushFront(T& pElement)
{
NodeT* pNode = &pElement.GetListNode(NodeIndex);
pNode->mData = &pElement;
if (Empty())
{
pNode->mNext = &mHead;
pNode->mPrev = &mHead;
mHead.mNext = pNode;
mHead.mPrev = pNode;
}
else
{
pNode->mNext = mHead.mNext;
pNode->mPrev = &mHead;
pNode->mNext->mPrev = pNode;
mHead.mNext = pNode;
}
}
void PopFront()
{
iterator begin = Begin();
Erase(begin);
}
void PopBack()
{
Erase(--(End()));
}
public:
class IntrusiveListIterator
{
public:
explicit IntrusiveListIterator(NodeT* ptr=0):mPtr(ptr){}
IntrusiveListIterator& operator++()
{
mPtr = mPtr->mNext;return (*this);
}
const IntrusiveListIterator operator++(int)
{
IntrusiveListIterator temp = *this;
++*this;
return (temp);
}
IntrusiveListIterator& operator--()
{
mPtr = mPtr->mPrev;return *this;
}
const IntrusiveListIterator operator--(int)
{
IntrusiveListIterator temp = *this;
--*this;
return (temp);
}
IntrusiveListIterator& operator=(const IntrusiveListIterator &other){mPtr = other.mPtr; return *this;}
reference operator*() const { return *(mPtr->mData); }
pointer operator->() const { return (&**this); }
bool operator==(const IntrusiveListIterator& other)const{ return mPtr==other.mPtr; }
bool operator!=(const IntrusiveListIterator& other)const{ return !(*this == other); }
inline NodeT* Get()const { return mPtr; }
private:
NodeT* mPtr;
};
class  IntrusiveListConstIterator
{
public:
explicit IntrusiveListConstIterator(const NodeT* ptr=0):mPtr(ptr){}
IntrusiveListConstIterator& operator++()
{
mPtr = mPtr->mNext;return (*this);
}
const IntrusiveListConstIterator operator++(int)
{
IntrusiveListConstIterator temp = *this;
++*this;
return (temp);
}
IntrusiveListConstIterator& operator--()
{
mPtr = mPtr->mPrev;return *this;
}
const IntrusiveListConstIterator operator--(int)
{
IntrusiveListConstIterator temp = *this;
--*this;
return (temp);
}
IntrusiveListConstIterator& operator=(const IntrusiveListConstIterator &other){mPtr = other.mPtr; return *this;}
const_reference operator*() const { return *(mPtr->mData); }
const_pointer operator->() const { return (&**this); }
bool operator==(const IntrusiveListConstIterator& other)const{ return mPtr==other.mPtr; }
bool operator!=(const IntrusiveListConstIterator& other)const{ return !(*this == other); }
inline const NodeT* Get()const { return mPtr; }
private:
mutable const NodeT* mPtr;
};
typedef IntrusiveListIterator iterator;
typedef IntrusiveListConstIterator const_iterator;
inline iterator Begin() { return iterator(mHead.mNext); }
inline const_iterator Begin() const { return const_iterator(mHead.mNext); }
inline iterator End() { return iterator(&mHead); }
inline const_iterator End() const { return const_iterator(&mHead); }
reference Front(){return (*Begin());}
const_reference Front() const { return (*Begin()); }
reference Back(){ return (*(--End())); }
const_reference Back() const{ return (*(--End())); }
iterator& Erase(iterator& it)
{
it.Get()->Disconnect();
return (++it);
}
private:
NodeT mHead;
FbxIntrusiveList(const FbxIntrusiveList&);
FbxIntrusiveList& operator=(const FbxIntrusiveList& Right){return (*this);}
};
#endif
#endif