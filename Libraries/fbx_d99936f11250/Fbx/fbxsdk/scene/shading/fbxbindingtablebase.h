#ifndef _FBXSDK_SCENE_SHADING_BINDING_TABLE_BASE_H_
#define _FBXSDK_SCENE_SHADING_BINDING_TABLE_BASE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/scene/shading/fbxbindingtableentry.h>
#include <fbxsdk/core/base/fbxdynamicarray.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBindingTableBase : public FbxObject
    FBXSDK_ABSTRACT_OBJECT_DECLARE(FbxBindingTableBase,FbxObject)
public:
    FbxBindingTableEntry& AddNewEntry()
    size_t GetEntryCount() const
    FbxBindingTableEntry const& GetEntry( size_t pIndex ) const
    FbxBindingTableEntry& GetEntry( size_t pIndex )
    const FbxBindingTableEntry* GetEntryForSource(const char* pSrcName) const
    const FbxBindingTableEntry* GetEntryForDestination(const char* pDestName) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
private:
    FbxDynamicArray<FbxBindingTableEntry> mEntries
#endif 