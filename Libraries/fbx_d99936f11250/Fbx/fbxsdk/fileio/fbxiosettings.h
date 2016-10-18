#ifndef _FBXSDK_FILEIO_IO_SETTINGS_H_
#define _FBXSDK_FILEIO_IO_SETTINGS_H_
#if (defined(_MSC_VER) || defined(__MINGW32__)) && defined(mkdir)
#undef mkdir
#endif
#define IOSVisible    true
#define IOSHidden     false
#define IOSSavable    true
#define IOSNotSavable false
#define IOSEnabled    true
#define IOSDisabled   false
#define IOSBinary     0
#define IOSASCII      1
class FbxManager;
class FbxIOSettings;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
class FbxIOPropInfo
{
public:
FbxIOPropInfo();
~FbxIOPropInfo();
void*   UIWidget;
void*   cbValueChanged;
void*   cbDirty;
FbxStringList labels;
};
class FBXSDK_DLL FbxIOInfo
{
public:
enum EImpExp {eImport, eExport};
FbxIOInfo();
void Reset(EImpExp pImpExp);
void SetTimeMode(FbxTime::EMode pTimeMode, double pCustomFrameRate = 0.0);
FbxTime::EMode GetTimeMode(){ return mTimeMode; }
FbxTime GetFramePeriod();
void SetASFScene(FbxObject* pASFScene, bool pASFSceneOwned = false);
FbxObject* GetASFScene(){ return mASFScene; }
void Set_IOS(FbxIOSettings* pIOS){ios = pIOS;}
void SetImportExportMode(EImpExp pImpExp){mImpExp = pImpExp;}
private:
FbxTime::EMode mTimeMode;
FbxObject*  mASFScene;
EImpExp   mImpExp;
FbxIOSettings* ios;
};
#endif
class FBXSDK_DLL FbxIOSettings : public FbxObject
{
FBXSDK_OBJECT_DECLARE(FbxIOSettings, FbxObject);
public:
enum ELanguage
{
eENU,
eDEU,
eFRA,
eJPN,
eKOR,
eCHS,
eLanguageCount
};
FbxProperty AddPropertyGroup(const char* pName, const FbxDataType& pDataType=FbxDataType(), const char* pLabel="");
FbxProperty AddPropertyGroup(const FbxProperty& pParentProperty, const char* pName, const FbxDataType& pDataType = FbxDataType(),
const char* pLabel  = "", bool pVisible = true, bool pSavable = true, bool pEnabled = true );
FbxProperty AddProperty(const FbxProperty& pParentProperty, const char* pName, const FbxDataType& pDataType = FbxDataType(),
const char* pLabel = "", const void* pValue = NULL, bool pVisible = true,
bool pSavable = true, bool pEnabled = true );
FbxProperty AddPropertyMinMax(const FbxProperty& pParentProperty, const char* pName, const FbxDataType& pDataType = FbxDataType(),
const char* pLabel = "", const void* pValue = NULL, const double* pMinValue = NULL, const double* pMaxValue = NULL,
bool pVisible = true, bool pSavable = true, bool pEnabled = true );
FbxProperty GetProperty(const char* pName) const;
FbxProperty GetProperty(const FbxProperty& pParentProperty, const char* pName) const;
bool GetBoolProp(const char* pName, bool pDefValue) const;
void SetBoolProp(const char* pName, bool pValue);
double GetDoubleProp(const char* pName, double pDefValue) const;
void   SetDoubleProp(const char* pName, double pValue);
int    GetIntProp(const char* pName, int pDefValue) const;
void   SetIntProp(const char* pName, int pValue);
FbxTime  GetTimeProp(const char* pName, FbxTime pDefValue) const;
void   SetTimeProp(const char* pName, FbxTime pValue);
FbxString GetEnumProp(const char* pName, FbxString pDefValue) const;
int     GetEnumProp(const char* pName, int pDefValue) const;
int     GetEnumIndex(const char* pName, FbxString pValue) const;
void    SetEnumProp(const char* pName, FbxString pValue);
void    SetEnumProp(const char* pName, int pValue);
void    RemoveEnumPropValue(const char* pName, FbxString pValue);
void    EmptyEnumProp(const char* pName);
bool IsEnumExist(FbxProperty& pProp, const FbxString& enumString) const;
int  GetEnumIndex(FbxProperty& pProp, const FbxString& enumString, bool pNoCase = false) const;
bool    SetFlag(const char* pName, FbxPropertyFlags::EFlags propFlag, bool pValue);
FbxString GetStringProp(const char* pName, FbxString pDefValue) const;
void    SetStringProp(const char* pName, FbxString pValue);
virtual bool ReadXMLFile(const FbxString& path);
virtual bool WriteXMLFile(const FbxString& path);
bool WriteXmlPropToFile(const FbxString& pFullPath, const FbxString& propPath);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxIOPropInfo* GetPropInfo(FbxProperty &pProp);
ELanguage UILanguage;
FbxString GetLanguageLabel(FbxProperty& pProp);
void SetLanguageLabel(FbxProperty& pProp, FbxString& pLabel);
ELanguage Get_Max_Runtime_Language(FbxString pRegLocation);
FbxIOInfo impInfo;
FbxIOInfo expInfo;
static FbxString GetUserMyDocumentDir();
void SetPropVisible(FbxProperty& pProp, bool pWithChildren, bool pVisible);
bool ReadXmlPropFromMyDocument(const FbxString& subDir, const FbxString& filename);
bool WriteXmlPropToMyDocument(const FbxString& subDir, const FbxString& filename, const FbxString& propPath);
static const char* GetFileMergeDescription(int pIndex);
enum ELoadMode
{
eCreate,
eMerge,
eExclusiveMerge
};
enum EQuaternionMode   { eAsQuaternion, eAsEuler, eResample };
enum EObjectDerivation { eByLayer, eByEntity, eByBlock };
enum ESysUnits
{
eUnitsUser,
eUnitsInches,
eUnitsFeet,
eUnitYards,
eUnitsMiles,
eUnitsMillimeters,
eUnitsCentimeters,
eUnitsMeters,
eUnitsKilometers
};
enum ESysFrameRate
{
eFrameRateUser,
eFrameRateHours,
eFrameRateMinutes,
eFrameRateSeconds,
eFrameRateMilliseconds,
eFrameRateGames15,
eFrameRateFilm24,
eFrameRatePAL25,
eFrameRateNTSC30,
eFrameRateShowScan48,
eFrameRatePALField50,
eFrameRateNTSCField60
};
enum EEnveloppeSystem
{
eSkinModifier,
ePhysic,
eBonePro,
eEnveloppeSystemCount
};
enum EGeometryType
{
eTriangle,
eSimplifiedPoly,
ePolygon,
eNurbs,
ePatch,
eGeometryTypeCount
};
enum EIKType
{
eNone,
eFBIK,
eHumanIK
};
protected:
virtual void Construct(const FbxObject* pFrom);
virtual void ConstructProperties(bool pForceSet);
virtual void Destruct(bool pRecursive);
private:
void AddNewPropInfo(FbxProperty& pProp);
void DeletePropInfo(FbxProperty& pProp);
void DeleteAllPropInfo(FbxProperty& pProp);
#endif
};
#endif