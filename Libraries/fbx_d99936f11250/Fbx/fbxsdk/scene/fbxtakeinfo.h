#ifndef _FBXSDK_SCENE_TAKEINFO_H_
#define _FBXSDK_SCENE_TAKEINFO_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/core/base/fbxtime.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxThumbnail
struct FbxTakeLayerInfo
	FbxString	mName
	int			mId
class FBXSDK_DLL FbxTakeInfo
public:
	FbxTakeInfo()
	virtual ~FbxTakeInfo()
	FbxTakeInfo(const FbxTakeInfo& pTakeInfo)
	FbxTakeInfo& operator=(const FbxTakeInfo& pTakeInfo)
	FbxString mName
	FbxString mImportName
	FbxString mDescription
	bool mSelect
	FbxTimeSpan mLocalTimeSpan
	FbxTimeSpan mReferenceTimeSpan
	FbxTime mImportOffset
	enum EImportOffsetType
		eAbsolute,
		eRelative
	EImportOffsetType mImportOffsetType
	void CopyLayers(const FbxTakeInfo& pTakeInfo)
	FbxArray<FbxTakeLayerInfo*>	mLayerInfoList
	int							mCurrentLayer
#include <fbxsdk/fbxsdk_nsend.h>
#endif 