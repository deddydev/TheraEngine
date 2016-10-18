#ifndef _FBXSDK_CORE_BASE_MULTIMAP_H_
#define _FBXSDK_CORE_BASE_MULTIMAP_H_
class FBXSDK_DLL FbxMultiMap
{
public:
struct Pair
{
FbxHandle mKey;
FbxHandle mItem;
};
bool Add(FbxHandle pKey, FbxHandle pItem);
bool Remove(FbxHandle pKey);
bool RemoveItem(FbxHandle pItem);
bool SetItem(FbxHandle pKey, FbxHandle pItem);
FbxHandle Get(FbxHandle pKey, int* pIndex=NULL);
void Clear();
FbxHandle GetFromIndex(int pIndex, FbxHandle* pKey=NULL);
bool RemoveFromIndex(int pIndex);
int GetCount() const { return mSetCount; }
void Swap();
void Sort();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxMultiMap(int pItemPerBlock=20);
FbxMultiMap(const FbxMultiMap& pOther);
~FbxMultiMap();
FbxMultiMap& operator=(const FbxMultiMap&);
private:
Pair* FindEqual(FbxHandle pKey) const;
Pair* mSetArray;
int  mSetCount;
int  mBlockCount;
int  mItemPerBlock;
bool mIsChanged;
#endif
};
#endif