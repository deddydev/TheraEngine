#ifndef _FBXSDK_CORE_PERIPHERAL_H_
#define _FBXSDK_CORE_PERIPHERAL_H_
class FbxObject;
class FBXSDK_DLL FbxPeripheral
{
public:
FbxPeripheral();
virtual ~FbxPeripheral();
virtual void Reset() = 0;
virtual bool UnloadContentOf(FbxObject* pObject) = 0;
virtual bool LoadContentOf(FbxObject* pObject) = 0;
virtual bool CanUnloadContentOf(FbxObject* pObject) = 0;
virtual bool CanLoadContentOf(FbxObject* pObject) = 0;
virtual void InitializeConnectionsOf(FbxObject* pObject) = 0;
virtual void UninitializeConnectionsOf(FbxObject* pObject) = 0;
};
extern FBXSDK_DLL FbxPeripheral* NULL_PERIPHERAL;
extern FBXSDK_DLL FbxPeripheral* TMPFILE_PERIPHERAL;
#endif