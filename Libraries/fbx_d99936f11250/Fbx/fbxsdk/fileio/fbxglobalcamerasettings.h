#ifndef _FBXSDK_FILEIO_GLOBAL_CAMERA_SETTINGS_H_
#define _FBXSDK_FILEIO_GLOBAL_CAMERA_SETTINGS_H_
class FbxStatus;
class FbxManager;
class FbxScene;
class FbxCamera;
class FbxCameraSwitcher;
#define FBXSDK_CAMERA_PERSPECTIVE "Producer Perspective"
#define FBXSDK_CAMERA_TOP   "Producer Top"
#define FBXSDK_CAMERA_FRONT   "Producer Front"
#define FBXSDK_CAMERA_BACK   "Producer Back"
#define FBXSDK_CAMERA_RIGHT   "Producer Right"
#define FBXSDK_CAMERA_LEFT   "Producer Left"
#define FBXSDK_CAMERA_BOTTOM  "Producer Bottom"
#define FBXSDK_CAMERA_SWITCHER  "Camera Switcher"
class FBXSDK_DLL FbxGlobalCameraSettings
{
FBXSDK_FRIEND_NEW();
public:
enum EViewingMode
{
eStandard,
eXRay,
eModelsOnly
};
bool SetDefaultCamera(const char* pCameraName, FbxStatus* pStatus=NULL);
const char* GetDefaultCamera() const;
void RestoreDefaultSettings();
void SetDefaultViewingMode(EViewingMode pViewingMode);
EViewingMode GetDefaultViewingMode() const;
void CreateProducerCameras();
void DestroyProducerCameras();
bool IsProducerCamera(FbxCamera* pCamera) const;
FbxCamera* GetCameraProducerPerspective() const;
FbxCamera* GetCameraProducerFront() const;
FbxCamera* GetCameraProducerBack() const;
FbxCamera* GetCameraProducerLeft() const;
FbxCamera* GetCameraProducerRight() const;
FbxCamera* GetCameraProducerTop() const;
FbxCamera* GetCameraProducerBottom() const;
FbxCameraSwitcher* GetCameraSwitcher() const;
void SetCameraSwitcher(FbxCameraSwitcher* pSwitcher);
const FbxGlobalCameraSettings& operator=(const FbxGlobalCameraSettings& pGlobalCameraSettings);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
bool CopyProducerCamera(const char* pCameraName, const FbxCamera* pCamera) const;
#endif
};
#endif