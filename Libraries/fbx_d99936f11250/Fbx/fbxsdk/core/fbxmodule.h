#ifndef _FBXSDK_CORE_MODULE_H_
#define _FBXSDK_CORE_MODULE_H_
#ifndef FBXSDK_ENV_WINSTORE
typedef void* FbxModule;
FBXSDK_DLL FbxModule FbxModuleLoad(const char* pFilePath);
FBXSDK_DLL void* FbxModuleGetProc(FbxModule pModuleHandle, const char* pProcName);
FBXSDK_DLL bool FbxModuleFree(FbxModule pModuleHandle);
#endif
#endif