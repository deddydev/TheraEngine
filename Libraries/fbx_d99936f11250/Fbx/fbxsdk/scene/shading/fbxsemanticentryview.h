#ifndef _FBXSDK_SCENE_SHADING_SEMANTIC_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_SEMANTIC_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/scene/shading/fbxbindingtableentry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSemanticEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxSemanticEntryView (FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false)
	virtual ~FbxSemanticEntryView()
	void SetSemantic( const char* pSemantic )
	FbxString GetSemantic(bool pAppendIndex = true) const
	int GetIndex() const
	virtual const char* EntryType() const
#include <fbxsdk/fbxsdk_nsend.h>
#endif 