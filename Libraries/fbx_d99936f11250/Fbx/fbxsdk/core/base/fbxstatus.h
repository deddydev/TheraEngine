#ifndef _FBXSDK_CORE_BASE_STATUS_H_
#define _FBXSDK_CORE_BASE_STATUS_H_
class FBXSDK_DLL FbxStatus
{
public:
enum EStatusCode {
eSuccess = 0,
eFailure,
eInsufficientMemory,
eInvalidParameter,
eIndexOutOfRange,
ePasswordError,
eInvalidFileVersion,
eInvalidFile
};
FbxStatus();
FbxStatus(EStatusCode pCode);
FbxStatus(const FbxStatus& rhs);
FbxStatus&      operator=(const FbxStatus& rhs);
bool            operator==(const FbxStatus& rhs)    const   { return (mCode == rhs.mCode); }
bool            operator==(const EStatusCode pCode) const   { return (mCode == pCode); }
bool            operator!=(const FbxStatus& rhs)    const   { return (mCode != rhs.mCode); }
bool            operator!=(const EStatusCode rhs)   const   { return (mCode != rhs); }
operator        bool() const    { return mCode==eSuccess; }
bool            Error() const   { return !this->operator bool(); }
void            Clear();
EStatusCode     GetCode() const { return mCode; }
void            SetCode(const EStatusCode rhs);
void            SetCode(const EStatusCode rhs, const char* pErrorMsg, ...);
const char*     GetErrorString() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
EStatusCode     mCode;
FbxString       mErrorString;
#endif
};
#endif