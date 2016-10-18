#ifndef _FBXSDK_CORE_BASE_BITSET_H_
#define _FBXSDK_CORE_BASE_BITSET_H_
class FBXSDK_DLL FbxBitSet
{
public:
FbxBitSet(const FbxUInt pInitialSize=0);
virtual ~FbxBitSet();
void SetBit(const FbxUInt pBitIndex);
void SetAllBits(const bool pValue);
void UnsetBit(const FbxUInt pBitIndex);
bool GetBit(const FbxUInt pBitIndex) const;
FbxUInt GetFirstSetBitIndex() const;
FbxUInt GetLastSetBitIndex() const;
FbxUInt GetNextSetBitIndex(const FbxUInt pBitIndex) const;
FbxUInt GetPreviousSetBitIndex(const FbxUInt pBitIndex) const;
};
#endif