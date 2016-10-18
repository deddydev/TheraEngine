#ifndef _FBXSDK_CORE_PROPERTY_TYPES_H_
#define _FBXSDK_CORE_PROPERTY_TYPES_H_
enum EFbxType
{
eFbxUndefined,
eFbxChar,
eFbxUChar,
eFbxShort,
eFbxUShort,
eFbxUInt,
eFbxLongLong,
eFbxULongLong,
eFbxHalfFloat,
eFbxBool,
eFbxInt,
eFbxFloat,
eFbxDouble,
eFbxDouble2,
eFbxDouble3,
eFbxDouble4,
eFbxDouble4x4,
eFbxEnum  = 17,
eFbxEnumM  =-17,
eFbxString  = 18,
eFbxTime,
eFbxReference,
eFbxBlob,
eFbxDistance,
eFbxDateTime,
eFbxTypeCount = 24
};
class FBXSDK_DLL FbxColor
{
public:
FbxColor();
FbxColor(const double pRed, const double pGreen, const double pBlue, const double pAlpha=1.0);
FbxColor(const FbxDouble3& pRGB, const double pAlpha=1.0);
FbxColor(const FbxDouble4& pRGBA);
~FbxColor();
void Set(const double pRed, const double pGreen, const double pBlue, const double pAlpha=1.0);
bool IsValid() const;
double& operator[](int pIndex);
const double& operator[](int pIndex) const;
FbxColor& operator=(const FbxColor& pColor);
FbxColor& operator=(const FbxDouble3& pColor);
FbxColor& operator=(const FbxDouble4& pColor);
bool operator==(const FbxColor& pColor) const;
bool operator!=(const FbxColor& pColor) const;
double mRed;
double mGreen;
double mBlue;
double mAlpha;
};
class FBXSDK_DLL FbxHalfFloat
{
public:
FbxHalfFloat();
FbxHalfFloat(float pVal);
FbxHalfFloat(const FbxHalfFloat& pVal);
FbxHalfFloat& operator=(const FbxHalfFloat& pValue);
bool operator==(const FbxHalfFloat& pRHS) const;
bool operator!=(const FbxHalfFloat& pRHS) const;
const float value() const;
unsigned const short internal_value() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
typedef unsigned short half;
#endif
};
};
};
};
inline EFbxType FbxTypeOf(const FbxChar&){ return eFbxChar; }
inline EFbxType FbxTypeOf(const FbxUChar&){ return eFbxUChar; }
inline EFbxType FbxTypeOf(const FbxShort&){ return eFbxShort; }
inline EFbxType FbxTypeOf(const FbxUShort&){ return eFbxUShort; }
inline EFbxType FbxTypeOf(const FbxUInt&){ return eFbxUInt; }
inline EFbxType FbxTypeOf(const FbxLongLong&){ return eFbxLongLong; }
inline EFbxType FbxTypeOf(const FbxULongLong&){ return eFbxULongLong; }
inline EFbxType FbxTypeOf(const FbxHalfFloat&){ return eFbxHalfFloat; }
inline EFbxType FbxTypeOf(const FbxBool&){ return eFbxBool; }
inline EFbxType FbxTypeOf(const FbxInt&){ return eFbxInt; }
inline EFbxType FbxTypeOf(const FbxFloat&){ return eFbxFloat; }
inline EFbxType FbxTypeOf(const FbxDouble&){ return eFbxDouble; }
inline EFbxType FbxTypeOf(const FbxDouble2&){ return eFbxDouble2; }
inline EFbxType FbxTypeOf(const FbxDouble3&){ return eFbxDouble3; }
inline EFbxType FbxTypeOf(const FbxDouble4&){ return eFbxDouble4; }
inline EFbxType FbxTypeOf(const FbxDouble4x4&){ return eFbxDouble4x4; }
inline EFbxType FbxTypeOf(const FbxVector2&){ return eFbxDouble2; }
inline EFbxType FbxTypeOf(const FbxVector4&){ return eFbxDouble4; }
inline EFbxType FbxTypeOf(const FbxQuaternion&){ return eFbxDouble4; }
inline EFbxType FbxTypeOf(const FbxMatrix&){ return eFbxDouble4x4; }
inline EFbxType FbxTypeOf(const FbxAMatrix&){ return eFbxDouble4x4; }
inline EFbxType FbxTypeOf(const FbxString&){ return eFbxString; }
inline EFbxType FbxTypeOf(const FbxTime&){ return eFbxTime; }
inline EFbxType FbxTypeOf(const FbxReference&){ return eFbxReference; }
inline EFbxType FbxTypeOf(const FbxBlob&){ return eFbxBlob; }
inline EFbxType FbxTypeOf(const FbxColor&){ return eFbxDouble4; }
inline EFbxType FbxTypeOf(const FbxDistance&){ return eFbxDistance; }
inline EFbxType FbxTypeOf(const FbxDateTime&){ return eFbxDateTime; }
template <class T> inline EFbxType FbxTypeOf(const T&){ FBX_ASSERT_NOW("Unknown type!"); return eFbxUndefined; }
template<class T1, class T2> inline bool FbxTypeCopy(T1&, const T2&){ FBX_ASSERT_NOW("Incompatible type assignment!" ); return false; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxChar& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxUChar& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxShort& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxUShort& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxUInt& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxLongLong& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxULongLong& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxHalfFloat& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxBool& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxInt& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxFloat& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDouble& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxDouble2& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxDouble3& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble4& pDst, const FbxDouble4& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble4x4& pDst, const FbxDouble4x4& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxString& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxTime& pDst, const FbxTime& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxReference& pDst, const FbxReference& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxBlob& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDistance& pDst, const FbxDistance& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDateTime& pDst, const FbxDateTime& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxChar& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxUChar& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxShort& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxUShort& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxUInt& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxLongLong& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxULongLong& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxInt& pSrc){ pDst = pSrc == 0 ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxFloat& pSrc){ pDst = pSrc == 0.f ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxDouble& pSrc){ pDst = pSrc == 0. ? false : true; return true; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxBool& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxBool&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxBool&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxUChar& pSrc){ pDst = (FbxChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxChar&  , const FbxShort&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxBool& pSrc){ pDst = (FbxChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxInt& pSrc){ pDst = (FbxChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxFloat& pSrc){ pDst = (FbxChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxDouble& pSrc){ pDst = (FbxChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxChar& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxChar&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxChar&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxChar& pSrc){ pDst = (FbxUChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxShort&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxBool& pSrc){ pDst = (FbxUChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxInt& pSrc){ pDst = (FbxUChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxFloat& pSrc){ pDst = (FbxUChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxDouble& pSrc){ pDst = (FbxUChar)pSrc; return true; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxUChar&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxUChar&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxChar& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxUChar& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxBool& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxInt& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxFloat& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxDouble& pSrc){ pDst = (FbxShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxShort& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxShort&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxShort&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxChar& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxUChar& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxShort& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxBool& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxInt& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxFloat& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxDouble& pSrc){ pDst = (FbxUShort)pSrc; return true; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxUShort&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxUShort&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxChar& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxUChar& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxShort& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxUShort& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxUInt& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxLongLong& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxULongLong& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxBool& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxFloat& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxDouble& pSrc){ pDst = (FbxInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxInt& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxInt&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxInt&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxChar& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxUChar& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxShort& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxUShort& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxLongLong& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxULongLong& pSrc)  { pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxBool& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxInt& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxFloat& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxDouble& pSrc){ pDst = (FbxUInt)pSrc; return true; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxUInt&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxUInt&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxChar& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxUChar& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxShort& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxUShort& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxUInt& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxULongLong& pSrc)  { pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxBool& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxInt& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxFloat& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxDouble& pSrc){ pDst = (FbxLongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxLongLong&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxChar& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxUChar& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxShort& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxUShort& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxUInt& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxLongLong& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxBool& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxInt& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxFloat& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxDouble& pSrc){ pDst = (FbxULongLong)pSrc; return true; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxULongLong&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxChar& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxUChar& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxShort& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxUShort& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxUInt& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxLongLong& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxULongLong& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxBool& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxInt& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxFloat& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxDouble& pSrc){ FbxHalfFloat hf((float)pSrc); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxString&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxHalfFloat& pDst, const FbxDistance& pSrc){ FbxHalfFloat hf(pSrc.internalValue()); pDst = hf; return true; }
inline bool FbxTypeCopy(FbxHalfFloat&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxChar& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxUChar& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxShort& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxUShort& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxUInt& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxHalfFloat& pSrc){ pDst = pSrc.value()   ; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxBool& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxInt& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxDouble& pSrc){ pDst = (FbxFloat)pSrc; return true; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxString&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxFloat& pDst, const FbxDistance& pSrc){ pDst = pSrc.internalValue(); return true; }
inline bool FbxTypeCopy(FbxFloat&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxUChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxUShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxUInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxLongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxULongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxHalfFloat& pSrc){ pDst = (FbxDouble)pSrc.value(); return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxBool& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxFloat& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDouble2& pSrc){ pDst = (FbxDouble)pSrc[0];     return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDouble3& pSrc){ pDst = (FbxDouble)pSrc[0];     return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDouble4& pSrc){ pDst = (FbxDouble)pSrc[0];     return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDouble4x4& pSrc){ pDst = (FbxDouble)pSrc[0][0];  return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxString& pSrc){ return FbxTypeCopyStr(pDst, pSrc); }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxTime& pSrc){ pDst = (FbxDouble)pSrc.GetSecondDouble();  return true; }
inline bool FbxTypeCopy(FbxDouble& pDst, const FbxDistance& pSrc){ pDst = pSrc.internalValue(); return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxUChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxUShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxUInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxLongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxULongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxHalfFloat& pSrc){ pDst = (FbxDouble)pSrc.value(); return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxBool& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxFloat& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble2& pDst, const FbxDouble& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxUChar& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxUShort& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxUInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxLongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxULongLong& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxHalfFloat& pSrc){ pDst = (FbxDouble)pSrc.value(); return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxBool& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxInt& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxFloat& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxDouble& pSrc){ pDst = (FbxDouble)pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxDouble2&  ){ return false;  }
inline bool FbxTypeCopy(FbxDouble3& pDst, const FbxDouble4& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxString&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble3&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxUChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxBool&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxDouble&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4& pDst, const FbxDouble3& pSrc){ pDst = pSrc; return true; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxString&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxDouble4&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxChar& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxUChar& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxShort& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxUShort& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxUInt& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxLongLong& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxULongLong& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxHalfFloat& pSrc){ pDst=FbxString((float)pSrc.value()); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxBool& pSrc){ pDst=pSrc ? "true" : "false"; return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxInt& pSrc){ pDst=FbxString((int)pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxFloat& pSrc){ pDst=FbxString(pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDouble& pSrc){ pDst=FbxString(pSrc); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDouble2& pSrc){ pDst=FbxString(pSrc[0])+","+FbxString(pSrc[1]); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDouble3& pSrc){ pDst=FbxString(pSrc[0])+","+FbxString(pSrc[1])+","+FbxString(pSrc[2]); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDouble4& pSrc){ pDst=FbxString(pSrc[0])+","+FbxString(pSrc[1])+","+FbxString(pSrc[2])+","+FbxString(pSrc[3]); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDouble4x4& pSrc){ pDst=FbxString(pSrc[0][0])+","+FbxString(pSrc[0][1])+","+FbxString(pSrc[0][2])+","+FbxString(pSrc[0][3]); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxTime& pSrc){ char lTimeStr[128]; pSrc.GetTimeString(lTimeStr, FbxUShort(128)); pDst=lTimeStr; return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxReference&  ){ pDst="<reference>"; return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxBlob&  ){ pDst="<blob>"; return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDistance& pSrc){ pDst= FbxString(pSrc.value()) + " " +pSrc.unitName(); return true; }
inline bool FbxTypeCopy(FbxString& pDst, const FbxDateTime& pSrc){ pDst= pSrc.toString(); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxChar& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxUChar& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxShort& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxUShort& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxUInt& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxLongLong& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxULongLong& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxHalfFloat& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxBool& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxInt& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxFloat& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxDouble& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxDouble2& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxDouble3& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxDouble4& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxDouble4x4& pSrc){ pDst.Assign(&pSrc, sizeof(pSrc)); return true; }
}
inline bool FbxTypeCopy(FbxBlob& pDst, const FbxTime& pSrc){ FbxLongLong t = pSrc.Get(); pDst.Assign( &t, sizeof(t)); return true; }
inline bool FbxTypeCopy(FbxBlob&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxBlob&  , const FbxDistance&  ){ return false; }
inline bool FbxTypeCopy(FbxBlob&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxUChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxBool&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxDouble&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const  FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxString&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxDistance&  , const FbxDateTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxUChar&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxUShort&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxUInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxLongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxULongLong&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxHalfFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxBool&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxInt&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxFloat&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDouble&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDouble2&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDouble3&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDouble4&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDouble4x4&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime& pDst, const FbxString& pSrc){ return pDst.fromString(pSrc); }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxTime&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxReference&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxBlob&  ){ return false; }
inline bool FbxTypeCopy(FbxDateTime&  , const FbxDistance&  ){ return false; }
}
}
}
}
}
}
#endif