#ifndef _FBXSDK_UTILS_PROCESSOR_XREF_USERLIB_H_
#define _FBXSDK_UTILS_PROCESSOR_XREF_USERLIB_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/utils/fbxprocessorxref.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxProcessorXRefCopyUserLibrary : public FbxProcessorXRefCopy
    FBXSDK_OBJECT_DECLARE(FbxProcessorXRefCopyUserLibrary, FbxProcessorXRefCopy)
public:
        FbxPropertyT<FbxBool>     CopyAllAssets
        FbxPropertyT<FbxBool>     CopyExternalAssets
        FbxPropertyT<FbxBool>     CopyAbsoluteUrlAssets
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual bool ValidPropertyForXRefCopy(FbxObject* pObject, FbxProperty& lProperty) const
#endif 