#ifndef _FBXSDK_FILEIO_GLOBAL_SETTINGS_H_
#define _FBXSDK_FILEIO_GLOBAL_SETTINGS_H_
class FBXSDK_DLL FbxGlobalSettings : public FbxObject
{
FBXSDK_OBJECT_DECLARE(FbxGlobalSettings, FbxObject);
public:
void SetAxisSystem(const FbxAxisSystem& pAxisSystem);
FbxAxisSystem GetAxisSystem();
void SetOriginalUpAxis(const FbxAxisSystem& pAxisSystem);
int GetOriginalUpAxis() const;
void SetSystemUnit(const FbxSystemUnit& pOther);
FbxSystemUnit GetSystemUnit() const;
void SetOriginalSystemUnit(const FbxSystemUnit& pOther);
FbxSystemUnit GetOriginalSystemUnit() const;
void SetAmbientColor(FbxColor pAmbientColor);
FbxColor GetAmbientColor() const;
bool SetDefaultCamera(const char* pCameraName);
FbxString GetDefaultCamera() const;
void SetTimeMode(FbxTime::EMode pTimeMode);
FbxTime::EMode GetTimeMode() const;
void SetTimeProtocol(FbxTime::EProtocol pTimeProtocol);
FbxTime::EProtocol GetTimeProtocol() const;
enum ESnapOnFrameMode
{
eNoSnap,
eSnapOnFrame,
ePlayOnFrame,
eSnapAndPlayOnFrame
};
void SetSnapOnFrameMode(ESnapOnFrameMode pSnapOnFrameMode);
ESnapOnFrameMode GetSnapOnFrameMode() const;
void SetTimelineDefaultTimeSpan(const FbxTimeSpan& pTimeSpan);
void GetTimelineDefaultTimeSpan(FbxTimeSpan& pTimeSpan) const;
void SetCustomFrameRate(double pCustomFrameRate);
double GetCustomFrameRate() const;
struct FBXSDK_DLL TimeMarker
{
TimeMarker();
TimeMarker(const TimeMarker& pTimeMarker);
TimeMarker& operator=(const TimeMarker& pTimeMarker);
FbxString mName;
FbxTime mTime;
bool mLoop;
};
int GetTimeMarkerCount() const;
TimeMarker GetTimeMarker(int pIndex, FbxStatus* pStatus=NULL) const;
void AddTimeMarker(const TimeMarker& pTimeMarker, FbxStatus* pStatus=NULL);
void ReplaceTimeMarker(int pIndex, const TimeMarker& pTimeMarker, FbxStatus* pStatus=NULL);
void RemoveAllTimeMarkers();
bool SetCurrentTimeMarker(int pIndex, FbxStatus* pStatus=NULL);
int GetCurrentTimeMarker() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
virtual FbxObject& Copy(const FbxObject& pObject);
#endif
};
inline EFbxType FbxTypeOf(const FbxTime::EMode&){ return eFbxEnum; }
#endif