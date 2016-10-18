#ifndef _FBXSDK_CORE_XREF_H_
#define _FBXSDK_CORE_XREF_H_
class FbxProperty;
class FbxDocument;
class FbxXRefManagerProject;
class FBXSDK_DLL FbxXRefManager
{
public:
FbxXRefManager();
virtual ~FbxXRefManager();
static const char* sTemporaryFileProject;
static const char* sConfigurationProject;
static const char* sLocalizationProject;
static const char* sEmbeddedFileProject;
static int     GetUrlCount(FbxProperty const &pProperty);
static int     GetUrlCount(FbxString const& pUrl);
static bool IsRelativeUrl  (FbxProperty const &pProperty,int pIndex);
static FbxString GetUrl(FbxProperty const &pProperty,int pIndex);
bool GetResolvedUrl (FbxProperty const &pProperty,int pIndex,FbxString & pResolvedPath) const;
bool GetResolvedUrl (const char* pUrl, FbxDocument* pDoc, FbxString& pResolvedPath) const;
bool GetFirstMatchingUrl(const char* pPrefix, const char* pOptExt, const FbxDocument* pDoc, FbxString& pResolvedPath) const;
bool        AddXRefProject   (const char *pName,const char *pUrl);
bool        AddXRefProject   (const char *pName,const char *pExtension,const char *pUrl);
bool        AddXRefProject   (FbxDocument* pDoc);
bool        RemoveXRefProject(const char *pName);
bool        RemoveAllXRefProjects();
int         GetXRefProjectCount() const;
const char *GetXRefProjectName(int pIndex) const;
const char* GetXRefProjectUrl(const char* pName);
const char* GetXRefProjectUrl(const char* pName) const;
const char* GetXRefProjectUrl(int pIndex) const;
inline bool HasXRefProject( const char* pName ) { return GetXRefProjectUrl(pName) != NULL; }
bool GetResolvedUrl (const char* pUrl,FbxString & pResolvePath) const;
private:
FbxArray<FbxXRefManagerProject*>    mProjects;
static bool UrlExist(const char* pUrl);
};
#endif