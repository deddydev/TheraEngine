#ifndef _FBXSDK_SCENE_SHADING_BINDING_TABLE_ENTRY_H_
#define _FBXSDK_SCENE_SHADING_BINDING_TABLE_ENTRY_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBindingTableEntry 
public:
        FbxBindingTableEntry()
        FbxBindingTableEntry(const FbxBindingTableEntry& pEntry)
        ~FbxBindingTableEntry()
        void SetSource( const char* pSource )
        const char* GetSource() const
        void SetDestination( const char* pDestination )
        const char* GetDestination() const
        void SetEntryType( const char* pType, bool pAsSource )
        const char* GetEntryType( bool pAsSource ) const
        void* GetUserDataPtr()
        const void* GetUserDataPtr() const
        void SetUserDataPtr(void* pData )
		FbxBindingTableEntry& operator=(const FbxBindingTableEntry& pEntry)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    FbxString	mSource
    FbxString	mDestination
    FbxString	mSourceType
    FbxString	mDestinationType
    void*		mData
#endif 