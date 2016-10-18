#ifndef _FBXSDK_SCENE_SHADING_IMPLEMENTATION_UTILS_H_
#define _FBXSDK_SCENE_SHADING_IMPLEMENTATION_UTILS_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbximplementation.h>
#include <fbxsdk/scene/shading/fbxbindingoperator.h>
#include <fbxsdk/scene/shading/fbxoperatorentryview.h>
#include <fbxsdk/scene/shading/fbxpropertyentryview.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
FBXSDK_DLL const FbxImplementation* GetImplementation( const FbxObject* pObject, const char* pImplementationTarget )
template <typename T> bool GetBoundPropertyValue(const FbxBindingTable* pBindingTable,
                                                 const char* pEntryName, 
                                                 const FbxImplementation* pImplementation,
                                                 const FbxObject* pBoundObject,
                                                 T& pValue)
    if ((NULL != pImplementation) && (NULL != pBindingTable) && (NULL != pBoundObject) && (NULL != pEntryName))
        const FbxBindingTableEntry* lEntry = pBindingTable->GetEntryForDestination(pEntryName)
        if (NULL != lEntry)
            if (strcmp(lEntry->GetEntryType(true), FbxPropertyEntryView::sEntryType) == 0)
                const char* lPropName = lEntry->GetSource()
                FbxProperty lProp = pBoundObject->FindPropertyHierarchical(lPropName)
                if (lProp.IsValid())
					pValue = lProp.Get<T>()
                    return true
            else if (strcmp(lEntry->GetEntryType(true), FbxOperatorEntryView::sEntryType) == 0)
                const char* lOperatorName = lEntry->GetSource()
                const FbxBindingOperator* lOp = pImplementation->GetOperatorByTargetName(lOperatorName)
                if (lOp)
                    return lOp->Evaluate(pBoundObject, &pValue)
    return false
#include <fbxsdk/fbxsdk_nsend.h>
#endif 