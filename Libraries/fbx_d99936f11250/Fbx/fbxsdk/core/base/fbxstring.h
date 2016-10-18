#ifndef _FBXSDK_CORE_BASE_STRING_H_
#define _FBXSDK_CORE_BASE_STRING_H_
FBXSDK_DLL void FbxUTF8ToWC(const char* pInUTF8, wchar_t*& pOutWideChar, size_t* pOutWideCharSize=NULL);
FBXSDK_DLL void FbxWCToUTF8(const wchar_t* pInWideChar, char*& pOutUTF8, size_t* pOutUTF8Size=NULL);
#if defined(FBXSDK_ENV_WIN)
FBXSDK_DLL void FbxWCToAnsi(const wchar_t* pInWideChar, char*& pOutANSI, size_t* pOutANSISize=NULL);
FBXSDK_DLL void FbxAnsiToWC(const char* pInANSI, wchar_t*& pOutWideChar, size_t* pOutWideCharSize=NULL);
FBXSDK_DLL void FbxAnsiToUTF8(const char* pInANSI, char*& pOutUTF8, size_t* pOutUTF8Size=NULL);
FBXSDK_DLL void FbxUTF8ToAnsi(const char* pInUTF8, char*& pOutANSI, size_t* pOutANSISize=NULL);
#endif
class FBXSDK_DLL FbxString
{
public:
FbxString();
FbxString(const FbxString& pString);
FbxString(const char* pString);
FbxString(char pChar, size_t pNbRepeat=1);
FbxString(const char* pCharPtr, size_t pLength);
FbxString(const int pValue);
FbxString(const float pValue);
FbxString(const double pValue);
~FbxString();
size_t GetLen() const;
size_t Size() const;
bool IsEmpty() const;
FbxString& Clear();
char& operator[](int pIndex);
char operator[](int pIndex) const;
char* Buffer();
const char* Buffer()const;
const FbxString& operator=(const FbxString& pString);
const FbxString& operator=(char pChar);
const FbxString& operator=(const char* pString);
const FbxString& operator=(int pValue);
const FbxString& operator=(float pValue);
const FbxString& operator=(double pValue);
const FbxString& operator+=(const FbxString& pString);
const FbxString& operator+=(char pChar);
const FbxString& operator+=(const char* pString);
const FbxString& operator+=(int pValue);
const FbxString& operator+=(float pValue);
const FbxString& operator+=(double pValue);
bool operator== (const FbxString& pString) const;
bool operator!= (const FbxString& pString) const;
bool operator< (const FbxString& pString) const;
bool operator<= (const FbxString& pString) const;
bool operator>= (const FbxString& pString) const;
bool operator> (const FbxString& pString) const;
bool operator== (const char* pString) const;
bool operator!= (const char* pString) const;
bool operator< (const char* pString) const;
bool operator<= (const char* pString) const;
bool operator>= (const char* pString) const;
bool operator> (const char* pString) const;
friend FBXSDK_DLL FbxString operator+(const FbxString& pString1, const FbxString& pString2);
friend FBXSDK_DLL FbxString operator+(const FbxString& pString, char pChar);
friend FBXSDK_DLL FbxString operator+(char pChar, const FbxString& pString);
friend FBXSDK_DLL FbxString operator+(const FbxString& pString1, const char* pString2);
friend FBXSDK_DLL FbxString operator+(const char* pString1, const FbxString& pString2);
friend FBXSDK_DLL FbxString operator+(const FbxString& pString, int pValue);
friend FBXSDK_DLL FbxString operator+(int pValue, const FbxString& pString);
friend FBXSDK_DLL FbxString operator+(const FbxString& pString, float pValue);
friend FBXSDK_DLL FbxString operator+( float pValue, const FbxString& pString);
friend FBXSDK_DLL FbxString operator+(const FbxString& pString, double pValue);
operator const char*() const;
const FbxString& Copy(const char* pString, size_t pLength);
const FbxString& Append(const char* pString, size_t pLength);
int Compare(const char* pString) const;
int CompareNoCase(const char* pString) const;
void Swap(FbxString& pString);
FbxString Upper() const;
FbxString Lower() const;
FbxString Mid(size_t pFirst, size_t pCount) const;
FbxString Mid(size_t pFirst) const;
FbxString Left(size_t pCount) const;
FbxString Right(size_t pCount) const;
enum EPaddingType {eRight, eLeft, eBoth};
FbxString Pad(EPaddingType pPadding, size_t pLen, char pCar=' ') const;
FbxString UnPad(EPaddingType pPadding, char pCar='\0') const;
int Find(char pChar, size_t pStartPosition=0) const;
int Find(const char* pStrSub, size_t pStartPosition=0) const;
int ReverseFind(char pChar) const;
int FindOneOf(const char* pStrCharSet, size_t pStartPosition=0) const;
bool FindAndReplace(const char* pFind, const char* pReplaceBy, size_t pStartPosition=0);
bool ReplaceAll(const char* pFind, const char* pReplaceBy);
bool ReplaceAll(char pFind, char pReplaceBy);
int GetTokenCount(const char* pSpans) const;
FbxString GetToken(int pTokenIndex, const char* pSpans) const;
private:
char* mData;
FbxString(size_t pSrc1Len, const char* pSrc1Data, size_t pSrc2Len, const char* pSrc2Data);
void Init();
void Invalidate();
void FreeBuffer();
void FreeBuffer(char *&pOldData);
bool AllocCopy(FbxString& pDest, size_t pCopyLen, size_t pCopyIndex) const;
bool AllocBuffer(size_t pLen);
bool AllocBuffer(size_t pLen, char*& pOldData);
bool AssignCopy(size_t pSrcLen, const char* pSrcData);
bool ConcatInPlace(size_t pSrcLen, const char* pSrcData);
bool IsIn(char pChar, const char* pString) const;
bool InternalFindAndReplace(const char* pFind, const char* pReplaceBy, size_t& pStartPosition);
};
FBXSDK_INCOMPATIBLE_WITH_ARRAY(FbxString);
FBXSDK_DLL FbxString operator+(const FbxString& pString1, const FbxString& pString2);
FBXSDK_DLL FbxString operator+(const FbxString& pString, char pChar);
FBXSDK_DLL FbxString operator+(const FbxString& pString1, const char* pString2);
FBXSDK_DLL FbxString operator+(const FbxString& pString, int pValue);
FBXSDK_DLL FbxString operator+(const FbxString& pString, float pValue);
FBXSDK_DLL FbxString operator+(const FbxString& pString, double pValue);
struct FbxStringCompare { inline int operator()(const FbxString& pKeyA, const FbxString& pKeyB) const { return pKeyA.Compare(pKeyB); } };
struct FbxStringCompareNoCase { inline int operator()(const FbxString& pKeyA, const FbxString& pKeyB) const { return pKeyA.CompareNoCase(pKeyB); } };
struct FbxCharPtrCompare { inline int operator()(const char* pKeyA, const char* pKeyB) const { return strcmp(pKeyA, pKeyB); } };
struct FbxCharPtrCompareNoCase { inline int operator()(const char* pKeyA, const char* pKeyB) const { return FBXSDK_stricmp(pKeyA, pKeyB); } };
inline void FbxRemoveChar(FbxString& pString, char pToRemove)
{
int lPos = pString.ReverseFind(pToRemove);
while( lPos >= 0 )
{
pString = pString.Left(lPos) + pString.Mid(lPos + 1);
lPos = pString.ReverseFind(pToRemove);
}
}
#endif