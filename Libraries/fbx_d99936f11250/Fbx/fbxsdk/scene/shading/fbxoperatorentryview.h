#ifndef _FBXSDK_SCENE_SHADING_OPERATOR_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_OPERATOR_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxBindingTableEntry
class FBXSDK_DLL FbxOperatorEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxOperatorEntryView( FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	~FbxOperatorEntryView()
	const char* GetOperatorName() const
	void SetOperatorName(const char* pName)
	virtual const char* EntryType() const
#include <fbxsdk/fbxsdk_nsend.h>
#endif 