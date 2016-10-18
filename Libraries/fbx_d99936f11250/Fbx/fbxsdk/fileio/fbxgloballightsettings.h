#ifndef _FBXSDK_FILEIO_GLOBAL_LIGHT_SETTINGS_H_
#define _FBXSDK_FILEIO_GLOBAL_LIGHT_SETTINGS_H_
class FbxGlobalLightSettingsProperties;
class FBXSDK_DLL FbxGlobalLightSettings
{
public:
FBXSDK_FRIEND_NEW();
void SetAmbientColor(FbxColor pAmbientColor);
FbxColor GetAmbientColor() const;
void SetFogEnable(bool pEnable);
bool GetFogEnable() const;
void SetFogColor(FbxColor pColor);
FbxColor GetFogColor() const;
enum EFogMode
{
eLinear,
eExponential,
eExponentialSquareRoot
};
void SetFogMode(EFogMode pMode);
EFogMode GetFogMode() const;
void SetFogDensity(double pDensity);
double GetFogDensity() const;
void SetFogStart(double pStart);
double GetFogStart() const;
void SetFogEnd(double pEnd);
double GetFogEnd() const;
struct FBXSDK_DLL ShadowPlane
{
ShadowPlane();
bool mEnable;
FbxVector4 mOrigin;
FbxVector4 mNormal;
};
void SetShadowEnable(bool pShadowEnable);
bool GetShadowEnable() const;
void SetShadowIntensity(double pShadowIntensity);
double GetShadowIntensity() const;
int GetShadowPlaneCount() const;
ShadowPlane* GetShadowPlane(int pIndex, FbxStatus* pStatus=NULL);
void AddShadowPlane(ShadowPlane pShadowPlane);
void RemoveAllShadowPlanes();
void RestoreDefaultSettings();
const FbxGlobalLightSettings& operator=(const FbxGlobalLightSettings& pGlobalLightSettings);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
#endif
};
#endif