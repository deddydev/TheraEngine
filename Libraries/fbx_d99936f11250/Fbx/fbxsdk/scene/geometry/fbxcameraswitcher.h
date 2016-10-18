#ifndef _FBXSDK_SCENE_GEOMETRY_CAMERA_SWITCHER_H_
#define _FBXSDK_SCENE_GEOMETRY_CAMERA_SWITCHER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCameraSwitcher : public FbxNodeAttribute
    FBXSDK_OBJECT_DECLARE(FbxCameraSwitcher,FbxNodeAttribute)
    public:
        FbxPropertyT<FbxInt>        CameraIndex
    virtual FbxNodeAttribute::EType GetAttributeType() const
        int GetDefaultCameraIndex() const
        void SetDefaultCameraIndex(int pIndex)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
protected:
    virtual void Destruct(bool pRecursive)
    virtual void ConstructProperties(bool pForceSet)
public:
    void AddCameraName(char* pCameraName)
    char* GetCameraName(FbxUInt pIndex) const
    FbxUInt GetCameraNameCount() const
	void ClearCameraNames()
protected:
    FbxArray<FbxString*> mCameraNameList
#endif 