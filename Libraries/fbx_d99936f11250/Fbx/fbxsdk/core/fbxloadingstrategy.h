#ifndef _FBXSDK_CORE_LOADING_STRATEGY_H_
#define _FBXSDK_CORE_LOADING_STRATEGY_H_
#ifndef FBXSDK_ENV_WINSTORE
class FBXSDK_DLL FbxLoadingStrategy : public FbxPluginContainer
{
public:
enum EState
{
eAllLoaded,
eNoneLoaded,
eAllFailed,
eSomeFailed
};
EState Load(FbxPluginData& pData);
void Unload();
};
#endif
#endif