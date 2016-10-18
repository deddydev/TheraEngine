#ifndef _FBXSDK_FILEIO_COLLADA_NAMESPACE_H_
#define _FBXSDK_FILEIO_COLLADA_NAMESPACE_H_
#include <fbxsdk.h>
#include <components/libxml2-2.7.8/include/libxml/globals.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
struct FbxColladaNamespace
public:
    void Push(xmlNode * pElement)
    void Pop()
    xmlNode * FindParamDefinition(const char * pSID) const
    xmlNode * FindParamModification(const char * pSID) const
    int GetParamModificationCount() const
    xmlNode * GetParamModification(int pIndex) const
private:
    FbxArray<xmlNode*> mParamDefinition
    FbxArray<int> mParamDefinitionCount
    FbxArray<xmlNode*> mParamModification
    FbxArray<int> mParamModificationCount
#include <fbxsdk/fbxsdk_nsend.h>
#endif 