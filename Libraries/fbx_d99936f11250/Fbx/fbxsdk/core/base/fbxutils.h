#ifndef _FBXSDK_CORE_BASE_UTILITIES_H_
#define _FBXSDK_CORE_BASE_UTILITIES_H_
#ifndef FBXSDK_ENV_WINSTORE
FBXSDK_DLL FbxString FbxGetEnv(const char* pEnvVar);
FBXSDK_DLL FbxString FbxGetApplicationDirectory();
#endif
FBXSDK_DLL FbxString FbxGetSystemTempPath();
FBXSDK_DLL bool FbxSetSystemTempPath(const char* pPathUTF8);
FBXSDK_DLL FbxString FbxGetCurrentWorkPath();
FBXSDK_DLL void FbxSetCurrentWorkPath(const char* pPath_UTF8);
class FBXSDK_DLL FbxPathUtils
{
public:
static FbxString Bind(const char* pRootPath, const char* pFilePath, bool pCleanPath=true);
static FbxString GetFolderName(const char* pFilePath);
static FbxString GetFileName(const char* pFilePath, bool pWithExtension=true);
static FbxString GetExtensionName(const char* pFilePath);
static FbxString ChangeExtension(const char* pFilePath, const char* pExtension);
static bool IsRelative(const char* pPath);
static FbxString GetRelativePath(const char* pRootPath, const char* pNewPath);
static FbxString GetRelativeFilePath(const char* pRootPath, const char* pNewFilePath);
static FbxString Resolve(const char* pRelPath);
static FbxString Clean(const char* pPath);
static FbxString GenerateFileName(const char* pFolder, const char* pPrefix);
static bool Exist(const char* pFolderPathUTF8);
static bool Create(const char* pFolderPathUTF8);
static bool Delete(const char* pFolderPathUTF8);
#ifndef FBXSDK_ENV_WINSTORE
static bool IsEmpty(const char* pFolderPath_UTF8);
#endif
};
class FBXSDK_DLL FbxStatusGlobal
{
public:
static FbxStatus& GetRef()
{
if( !mStatusPtr )
{
mStatusPtr = FbxNew<FbxStatus>();
}
return *mStatusPtr;
}
FbxStatusGlobal(){ mStatusPtr = NULL; }
~FbxStatusGlobal(){ FbxDelete<FbxStatus>(mStatusPtr); }
};
#endif