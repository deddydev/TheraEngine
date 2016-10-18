#ifndef _FBXSDK_FILEIO_COLLADA_IO_STREAM_H_
#define _FBXSDK_FILEIO_COLLADA_IO_STREAM_H_
#include <fbxsdk.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
template <typename T> bool FromString(T * pDest, const char * pSourceBegin, const char ** pSourceEnd = NULL)
template <> bool FromString(int * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(double * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxString * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxDouble2 * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxDouble3 * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxDouble4 * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxVector4 * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxAMatrix * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <> bool FromString(FbxAMatrix * pDest, const char * pSourceBegin, const char ** pSourceEnd)
template <typename TYPE> int FromStringToArray(const char * pString, TYPE * pArray, int pSourceUnitOffset, int pSourceValidUnitCount, int pSourceGroupSize, int pDestUnitOffset, int pDestValidUnitCount, int pDestGroupSize, TYPE pDefaultValue = TYPE())
    if (pString == 0 || pArray == 0)
        return 0
    FBX_ASSERT(pSourceUnitOffset >= 0 && pSourceUnitOffset < pSourceGroupSize)
    FBX_ASSERT(pSourceValidUnitCount >= 0 && pSourceUnitOffset + pSourceValidUnitCount <= pSourceGroupSize)
    FBX_ASSERT(pDestUnitOffset >= 0 && pDestUnitOffset < pDestGroupSize)
    FBX_ASSERT(pDestValidUnitCount >= 0 && pDestUnitOffset + pDestValidUnitCount <= pDestGroupSize)
    const char * lSource = pString
    TYPE * lDest = pArray
    int lReadCount = 0
    int lSourceCounter = 0
    int lDestCounter = 0
    const int lSourceUnitValidEnd = pSourceUnitOffset + pSourceValidUnitCount
    const int lDestUnitGap = pDestGroupSize - pDestValidUnitCount - pDestUnitOffset
    while (lSource && *lSource)
        TYPE lData
        const char * lSourceStart = lSource
        if (FromString(&lData, lSource, &lSource) && lSourceCounter >= pSourceUnitOffset && lSourceCounter < lSourceUnitValidEnd)
            if (lDestCounter == 0)
                for (int lIndex = 0
 lIndex < pDestUnitOffset
 ++lIndex)
                    *(lDest++) = pDefaultValue
            *lDest++ = lData
            ++lReadCount
            ++lDestCounter
            if (lDestCounter == pDestValidUnitCount)
                lDestCounter = 0
                for (int lIndex = 0
 lIndex < lDestUnitGap
 ++lIndex)
                    *lDest++ = pDefaultValue
        else
            if (lSource == lSourceStart)
                break
        ++lSourceCounter
        if (lSourceCounter == pSourceGroupSize)
            lSourceCounter = 0
    return lReadCount
template <typename T>
const FbxString ToString(const T & pValue)
    return FbxString(pValue)
template <>
const FbxString ToString(const FbxVector4 & pValue)
template <>
const FbxString ToString(const FbxAMatrix & pValue)
const FbxString DecodePercentEncoding(const FbxString & pEncodedString)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 