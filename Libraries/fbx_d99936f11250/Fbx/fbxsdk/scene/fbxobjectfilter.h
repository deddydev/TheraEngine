#ifndef _FBXSDK_SCENE_OBJECT_FILTER_H_
#define _FBXSDK_SCENE_OBJECT_FILTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxObjectFilter
public:
    virtual ~FbxObjectFilter() 
	virtual bool Match(const FbxObject * pObjectPtr) const = 0
	virtual bool NotMatch(const FbxObject * pObjectPtr) const 
 return !Match(pObjectPtr)
class FBXSDK_DLL FbxNameFilter : public FbxObjectFilter
public:
    inline FbxNameFilter( const char* pTargetName ) : mTargetName( pTargetName ) 
    virtual ~FbxNameFilter() 
    virtual bool Match(const FbxObject * pObjectPtr) const 
 return pObjectPtr ? mTargetName == pObjectPtr->GetName() : false
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    FbxString mTargetName
#endif 