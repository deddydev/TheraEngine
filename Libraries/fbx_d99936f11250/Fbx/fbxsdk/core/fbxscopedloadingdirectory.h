#ifndef _FBXSDK_CORE_SCOPED_LOADING_DIRECTORY_H_
#define _FBXSDK_CORE_SCOPED_LOADING_DIRECTORY_H_
#ifndef FBXSDK_ENV_WINSTORE
class FbxPluginHandle;
class FBXSDK_DLL FbxScopedLoadingDirectory : public FbxLoadingStrategy
{
public:
FbxScopedLoadingDirectory(const char* pDirectoryPath, const char* pPluginExtension);
virtual ~FbxScopedLoadingDirectory();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif
#endif