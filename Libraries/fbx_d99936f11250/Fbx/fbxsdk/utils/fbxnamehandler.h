#ifndef _FBXSDK_UTILS_NAMEHANDLER_H_
#define _FBXSDK_UTILS_NAMEHANDLER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxNameHandler
public:
    FbxNameHandler(const char* pInitialName = "")
    FbxNameHandler(FbxNameHandler const& pName)
    ~FbxNameHandler()
    void SetInitialName(const char* pInitialName)
    const char* GetInitialName() const
    void SetCurrentName(const char* pNewName)
    const char* GetCurrentName() const
    void SetNameSpace(const char* pNameSpace)
    const char* GetNameSpace() const
    bool IsRenamed() const
    FbxNameHandler& operator= (FbxNameHandler const& pName)
    void SetParentName(const char* pParentName)
    const char* GetParentName() const
    FbxArray<FbxString*> GetNameSpaceArray(char identifier)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
private:
    FbxString mParentName
    FbxString mInitialName
    FbxString mCurrentName
    FbxString mNameSpace
#endif 