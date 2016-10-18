#ifndef _FBXSDK_CORE_PLUGIN_H_
#define _FBXSDK_CORE_PLUGIN_H_
#ifndef FBXSDK_ENV_WINSTORE
class FbxManager;
class FbxPluginContainer;
#define FBXSDK_PLUGIN_DECLARE(Plugin)\
FBXSDK_FRIEND_NEW();\
public:\
static Plugin * Create(const FbxPluginDef& pDefinition, FbxModule pModuleHandle);\
void Destroy();
#define FBXSDK_PLUGIN_IMPLEMENT(Plugin)\
Plugin* Plugin::Create(const FbxPluginDef& pDefinition, FbxModule pModuleHandle){ return FbxNew<Plugin>(pDefinition, pModuleHandle); }\
void Plugin::Destroy(){ FbxDelete(this); }
struct FBXSDK_DLL FbxPluginDef
{
FbxPluginDef() :
mName("Unknown Name"),
mVersion("Unknown Version")
{
}
FbxString mName;
FbxString mVersion;
};
struct FBXSDK_DLL FbxPluginData
{
FbxPluginData() :
mQueryEmitter(NULL),
mSDKManager(NULL),
mPluginContainer(NULL)
{
}
explicit FbxPluginData(const FbxPluginData& pOther) :
mQueryEmitter(pOther.mQueryEmitter),
mSDKManager(pOther.mSDKManager),
mPluginContainer(pOther.mPluginContainer)
{
}
FbxEmitter*   mQueryEmitter;
FbxManager*   mSDKManager;
FbxPluginContainer* mPluginContainer;
};
class FBXSDK_DLL FbxPlugin : public FbxListener
{
FBXSDK_INTRUSIVE_LIST_NODE(FbxPlugin, 1);
public:
virtual bool SpecificInitialize()=0;
virtual bool SpecificTerminate()=0;
virtual void WriteBegin(FbxScene& pScene);
virtual void WriteParameters(FbxObject& pParams);
virtual void WriteEnd(FbxScene& pScene);
virtual void ReadBegin(FbxScene& pScene);
virtual void ReadParameters(FbxObject& pParams);
virtual void ReadEnd(FbxScene& pScene);
const FbxPluginDef& GetDefinition() const;
FbxModule GetModuleHdl();
#ifndef DOXYGEN_SHOULD_SKIP_THIS
inline FbxObject& GetPluginSettings() { return *mPluginSettings; }
inline const FbxObject& GetPluginSettings() const { return *mPluginSettings; }
}
#endif
};
#endif
#endif