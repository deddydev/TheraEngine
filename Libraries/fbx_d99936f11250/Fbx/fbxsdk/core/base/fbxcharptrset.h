#ifndef _FBXSDK_CORE_BASE_CHARPTRSET_H_
#define _FBXSDK_CORE_BASE_CHARPTRSET_H_
class FBXSDK_DLL FbxCharPtrSet
{
public:
FbxCharPtrSet(int pItemPerBlock=20);
~FbxCharPtrSet();
void Add(const char* pReference, FbxHandle pItem);
bool Remove(const char* pReference);
FbxHandle Get(const char* pReference, int* PIndex=NULL);
FbxHandle& operator[](int pIndex);
FbxHandle GetFromIndex(int pIndex, const char** pReference=NULL);
void RemoveFromIndex(int pIndex);
inline int GetCount() const { return mCharPtrSetCount; }
void Sort();
void Clear();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
struct CharPtrSet;
inline void SetCaseSensitive(bool pIsCaseSensitive){ mIsCaseSensitive = pIsCaseSensitive; }
private:
CharPtrSet* FindEqual(const char* pReference) const;
CharPtrSet* mCharPtrSetArray;
int   mCharPtrSetCount;
int   mBlockCount;
int   mItemPerBlock;
bool  mIsChanged;
bool  mIsCaseSensitive;
#endif
};
#endif