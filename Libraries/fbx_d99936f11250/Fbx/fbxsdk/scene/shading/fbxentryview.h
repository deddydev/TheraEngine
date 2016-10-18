#ifndef _FBXSDK_SCENE_SHADING_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxBindingTableEntry
class FBXSDK_DLL FbxEntryView
public:
	static const char* sEntryType
	FbxEntryView( FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	virtual ~FbxEntryView()
	virtual bool IsValid() const
	virtual bool Create()
	virtual const char* EntryType() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	bool mAsSource
	FbxBindingTableEntry* mEntry
#endif 