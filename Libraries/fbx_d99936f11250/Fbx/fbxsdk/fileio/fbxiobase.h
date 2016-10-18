#ifndef _FBXSDK_FILEIO_IO_BASE_H_
#define _FBXSDK_FILEIO_IO_BASE_H_
#define FBXSDK_IO_END_NODE_STR "_End"
class FBXSDK_DLL FbxIOBase : public FbxObject
{
FBXSDK_OBJECT_DECLARE(FbxIOBase, FbxObject);
public:
virtual bool Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
virtual FbxString GetFileName();
FbxStatus& GetStatus() { return mStatus; }
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
virtual void Construct(const FbxObject* pFrom);
int DetectReaderFileFormat(const char *pFileName);
int DetectWriterFileFormat(const char *pFileName);
FbxStatus   mStatus;
FbxString mFilename;
#endif
};
#endif