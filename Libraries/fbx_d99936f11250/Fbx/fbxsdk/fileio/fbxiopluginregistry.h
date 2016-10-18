#ifndef _FBXSDK_FILEIO_IO_PLUGIN_REGISTRY_H_
#define _FBXSDK_FILEIO_IO_PLUGIN_REGISTRY_H_
class FBXSDK_DLL FbxIOPluginRegistry
{
public:
FbxIOPluginRegistry();
virtual ~FbxIOPluginRegistry();
#ifndef FBXSDK_ENV_WINSTORE
void RegisterReader(const char* pPluginPath,
int& pFirstPluginID,
int& pRegisteredCount,
bool pOverride = false);
#endif
void RegisterReader(FbxReader::CreateFuncType pCreateF,
FbxReader::GetInfoFuncType pInfoF,
int& pFirstPluginID,
int& pRegisteredCount,
FbxReader::IOSettingsFillerFuncType pIOSettingsFillerF = NULL,
bool pOverride = false);
#ifndef FBXSDK_ENV_WINSTORE
void RegisterWriter(const char* pPluginPath,
int& pFirstPluginID,
int& pRegisteredCount,
bool pOverride = false);
#endif
void RegisterWriter(FbxWriter::CreateFuncType pCreateF,
FbxWriter::GetInfoFuncType pInfoF,
int& pFirstPluginID,
int& pRegisteredCount,
FbxWriter::IOSettingsFillerFuncType pIOSettingsFillerF = NULL,
bool pOverride = false);
FbxReader* CreateReader(FbxManager& pManager,
FbxImporter& pImporter,
int pPluginID) const;
FbxWriter* CreateWriter(FbxManager& pManager,
FbxExporter& pExporter,
int pPluginID) const;
int FindReaderIDByExtension(const char* pExt) const;
int FindWriterIDByExtension(const char* pExt) const;
int FindReaderIDByDescription(const char* pDesc) const;
int FindWriterIDByDescription(const char* pDesc) const;
bool ReaderIsFBX(int pFileFormat) const;
bool WriterIsFBX(int pFileFormat) const;
bool ReaderIsGenuine(int pFileFormat) const;
bool WriterIsGenuine(int pFileFormat) const;
int GetReaderFormatCount() const;
int GetWriterFormatCount() const;
const char* GetReaderFormatDescription(int pFileFormat) const;
const char* GetWriterFormatDescription(int pFileFormat) const;
const char* GetReaderFormatExtension(int pFileFormat) const;
const char* GetWriterFormatExtension(int pFileFormat) const;
char const* const* GetWritableVersions(int pFileFormat) const;
bool DetectReaderFileFormat(const char* pFileName, int& pFileFormat) const;
bool DetectWriterFileFormat(const char* pFileName, int& pFileFormat) const;
int GetNativeReaderFormat();
int GetNativeWriterFormat();
void FillIOSettingsForReadersRegistered(FbxIOSettings & pIOS);
void FillIOSettingsForWritersRegistered(FbxIOSettings & pIOS);
#ifndef DOXYGEN_SHOULD_SKIP_THIS
};
};
#endif
};
#endif