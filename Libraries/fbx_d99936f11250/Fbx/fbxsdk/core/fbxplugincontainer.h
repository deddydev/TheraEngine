#ifndef _FBXSDK_CORE_PLUGIN_CONTAINER_H_
#define _FBXSDK_CORE_PLUGIN_CONTAINER_H_
#ifndef FBXSDK_ENV_WINSTORE
class FBXSDK_DLL FbxPluginContainer : public FbxEmitter
{
public:
typedef FbxIntrusiveList<FbxPlugin> PluginList;
void Register(FbxPlugin& pPlugin);
void Unregister(FbxPlugin& pPlugin);
const PluginList& GetPlugins() const;
PluginList& GetPlugins();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif
#endif