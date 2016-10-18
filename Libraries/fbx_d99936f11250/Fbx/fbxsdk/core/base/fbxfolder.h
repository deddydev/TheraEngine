#ifndef _FBXSDK_CORE_BASE_FOLDER_H_
#define _FBXSDK_CORE_BASE_FOLDER_H_
#ifndef FBXSDK_ENV_WINSTORE
class FBXSDK_DLL FbxFolder
{
public:
enum EEntryType
{
eRegularEntry,
eFolderEntry
};
bool Open(const char* pFolderPath_UTF8);
bool Next();
EEntryType GetEntryType() const;
FbxString GetEntryName() const;
char* GetEntryExtension() const;
void Close();
bool IsOpen() const;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxFolder();
~FbxFolder();
#endif
};
#endif
#endif