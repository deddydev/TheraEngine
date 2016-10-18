#ifndef _FBXSDK_UTILS_PROCESSOR_H_
#define _FBXSDK_UTILS_PROCESSOR_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCollection
class FBXSDK_DLL FbxProcessor : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxProcessor, FbxObject)
public:
		bool					ProcessCollection(FbxCollection *pCollection=0)
		bool					ProcessObject	 (FbxObject *pCollection=0)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual bool				internal_ProcessCollectionBegin (FbxCollection *pCollection)
    virtual bool				internal_ProcessCollectionEnd 	(FbxCollection *pCollection)
    virtual bool				internal_ProcessObject  		(FbxObject*	 pObject)
    virtual bool				internal_ProcessCollection		(FbxCollection* pCollection)
#endif 