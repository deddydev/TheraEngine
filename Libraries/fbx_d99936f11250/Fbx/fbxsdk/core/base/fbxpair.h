#ifndef _FBXSDK_CORE_BASE_PAIR_H_
#define _FBXSDK_CORE_BASE_PAIR_H_
template <typename First, typename Second> class FbxPair
{
public:
inline FbxPair() : mFirst(), mSecond() {}
inline FbxPair(const First& pFirst, const Second& pSecond) : mFirst(pFirst), mSecond(pSecond) {}
inline FbxPair<First, Second>& operator=(const FbxPair<First, Second>& pOther)
{
mFirst = pOther.mFirst;
mSecond = pOther.mSecond;
return *this;
}
inline bool operator==(const FbxPair<First, Second>& pOther)
{
return mFirst == pOther.mFirst && mSecond == pOther.mSecond;
}
inline bool operator!=(const FbxPair<First, Second>& pOther)
{
return !operator==(pOther);
}
First mFirst;
Second mSecond;
};
#endif