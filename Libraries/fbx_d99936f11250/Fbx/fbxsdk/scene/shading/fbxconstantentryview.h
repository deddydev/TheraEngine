#ifndef _FBXSDK_SCENE_SHADING_CONSTANT_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_CONSTANT_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxBindingTableEntry
class FBXSDK_DLL FbxConstantEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxConstantEntryView( FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	~FbxConstantEntryView()
	const char* GetConstantName() const
	void SetConstantName(const char* pName)
	virtual const char* EntryType() const
#include <fbxsdk/fbxsdk_nsend.h>
#endif 