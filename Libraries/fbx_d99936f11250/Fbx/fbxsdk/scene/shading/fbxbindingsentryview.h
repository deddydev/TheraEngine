#ifndef _FBXSDK_SCENE_SHADING_BINDINGS_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_BINDINGS_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxBindingsEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxBindingsEntryView( FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	~FbxBindingsEntryView()
	const char* GetBindingTableName() const
	void SetBindingTableName(const char* pName)
	virtual const char* EntryType() const
#include <fbxsdk/fbxsdk_nsend.h>
#endif 