#ifndef _FBXSDK_SCENE_REFERENCE_H_
#define _FBXSDK_SCENE_REFERENCE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSceneReference : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxSceneReference, FbxObject)
public:
    FbxPropertyT< FbxString >	 	  ReferenceFilePath
    FbxPropertyT< FbxString >	 	  ReferenceNameSpace
    FbxPropertyT< FbxString >	 	  ReferenceNodeName
    FbxPropertyT< FbxInt>	 		      ReferenceDepth
    FbxPropertyT< FbxBool >	 		  IsLoaded
    FbxPropertyT< FbxBool >	 		  IsLocked
    	FbxPropertyT< FbxBool >	 		  IsOriginalProxy
    	FbxPropertyT< FbxBool >	 		  IsActiveProxy
    	FbxPropertyT< FbxString >	 	  ProxyManagerName
		FbxPropertyT< FbxString >	 	  ProxyTag
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
#endif 