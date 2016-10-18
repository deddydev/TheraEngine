#ifndef _FBXSDK_SCENE_SHADING_LAYER_ENTRY_VIEW_H_
#define _FBXSDK_SCENE_SHADING_LAYER_ENTRY_VIEW_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxentryview.h>
#include <fbxsdk/scene/shading/fbxbindingtableentry.h>
#include <fbxsdk/scene/geometry/fbxlayer.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxLayerContainer
class FBXSDK_DLL FbxLayerEntryView : public FbxEntryView
public:
	static const char* sEntryType
	FbxLayerEntryView(FbxBindingTableEntry* pEntry, bool pAsSource, bool pCreate = false )
	virtual ~FbxLayerEntryView()
	void SetLayerElement( int pLayerIndex, FbxLayerElement::EType pType, bool pUVSet )
	void GetLayerElement( int &pLayerIndex, FbxLayerElement::EType& pType, bool& pUVSet ) const
	FbxLayerElement* GetLayerElement( FbxLayerContainer* pContainer ) const
	virtual const char* EntryType() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	static const char* sDelimiter
#endif 