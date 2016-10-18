#ifndef _FBXSDK_SCENE_DOCUMENT_INFO_H_
#define _FBXSDK_SCENE_DOCUMENT_INFO_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxThumbnail
class FBXSDK_DLL FbxDocumentInfo : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxDocumentInfo, FbxObject)
public:
		FbxPropertyT<FbxString> LastSavedUrl
		FbxPropertyT<FbxString> Url
		FbxProperty Original
		FbxPropertyT<FbxString> Original_ApplicationVendor
		FbxPropertyT<FbxString> Original_ApplicationName
		FbxPropertyT<FbxString> Original_ApplicationVersion
		FbxPropertyT<FbxString> Original_FileName
		FbxPropertyT<FbxDateTime> Original_DateTime_GMT
		FbxProperty LastSaved
		FbxPropertyT<FbxString> LastSaved_ApplicationVendor
		FbxPropertyT<FbxString> LastSaved_ApplicationName
		FbxPropertyT<FbxString> LastSaved_ApplicationVersion
		FbxPropertyT<FbxDateTime> LastSaved_DateTime_GMT
		FbxPropertyT<FbxString> EmbeddedUrl
		FbxString mTitle
		FbxString mSubject
		FbxString mAuthor
		FbxString mKeywords
		FbxString mRevision
		FbxString mComment
		FbxThumbnail* GetSceneThumbnail()
		void SetSceneThumbnail(FbxThumbnail* pSceneThumbnail)
	void Clear()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void Destruct(bool pRecursive)
	virtual void ConstructProperties(bool pForceSet)
    FbxPropertyT<FbxReference> SceneThumbnail
#endif 