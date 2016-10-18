#ifndef _FBXSDK_CORE_SCOPED_LOADING_FILENAME_H_
#define _FBXSDK_CORE_SCOPED_LOADING_FILENAME_H_
#ifndef FBXSDK_ENV_WINSTORE
class FBXSDK_DLL FbxScopedLoadingFileName : public FbxLoadingStrategy
{
public:
explicit FbxScopedLoadingFileName(const char* pPath);
virtual ~FbxScopedLoadingFileName();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif
#endif