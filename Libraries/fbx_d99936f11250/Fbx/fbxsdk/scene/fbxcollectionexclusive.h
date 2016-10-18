#ifndef _FBXSDK_SCENE_COLLECTION_EXCLUSIVE_H_
#define _FBXSDK_SCENE_COLLECTION_EXCLUSIVE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxcollection.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCollectionExclusive : public FbxCollection
    FBXSDK_OBJECT_DECLARE(FbxCollectionExclusive, FbxCollection)
public:
    bool AddMember(FbxObject* pMember)
#include <fbxsdk/fbxsdk_nsend.h>
#endif 