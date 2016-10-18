#ifndef _FBXSDK_SCENE_GEOMETRY_CAMERA_STEREO_H_
#define _FBXSDK_SCENE_GEOMETRY_CAMERA_STEREO_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxcamera.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxCameraStereo : public FbxCamera
    FBXSDK_OBJECT_DECLARE(FbxCameraStereo, FbxCamera)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
    enum EStereoType
        eNone,		
        eConverged,	
        eOffAxis,	
        eParallel	
    FbxCamera* GetLeftCamera() const
    FbxCamera* GetRightCamera() const
    bool SetLeftCamera(FbxCamera* pCamera)
    bool SetRightCamera(FbxCamera* pCamera)
    FbxAMatrix GetLeftCameraLocalMatrix() const
    FbxAMatrix GetLeftCameraGlobalMatrix() const
    FbxAMatrix GetRightCameraLocalMatrix() const
    FbxAMatrix GetRightCameraGlobalMatrix() const
    double ReevaluateLeftCameraFilmOffsetX() const
    double ReevaluateRightCameraFilmOffsetX() const
    FbxPropertyT<EStereoType>                    Stereo
    FbxPropertyT<FbxDouble>                       InteraxialSeparation
    FbxPropertyT<FbxDouble>                       ZeroParallax
    FbxPropertyT<FbxDouble>                       ToeInAdjust
    FbxPropertyT<FbxDouble>                       FilmOffsetRightCam
    FbxPropertyT<FbxDouble>                       FilmOffsetLeftCam
    FbxPropertyT<FbxReference>                     RightCamera
    FbxPropertyT<FbxReference>                     LeftCamera
    FbxPropertyT<FbxString>                        PrecompFileName
    FbxPropertyT<FbxString>                        RelativePrecompFileName
    bool ConnectProperties()
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
    virtual void ConstructProperties(bool pForceSet)
    virtual FbxStringList GetTypeFlags() const
#endif 