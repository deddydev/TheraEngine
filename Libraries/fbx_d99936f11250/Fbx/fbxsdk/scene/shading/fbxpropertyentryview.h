#ifndef _FBXSDK_SCENE_SHADING_PROPERTY_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_PROPERTY_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/scene/shading/fbxbindingtableentry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxPropertyEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxPropertyEntryView( FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	~FbxPropertyEntryView()
	const char* GetProperty() const
	void SetProperty(const char* pPropertyName)
	virtual const char* EntryType() const
#include <fbxsdk/fbxsdk_nsend.h>
#endif 