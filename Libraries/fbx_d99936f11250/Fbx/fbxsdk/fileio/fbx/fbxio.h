#ifndef _FBXSDK_FILEIO_FBX_IO_H_
#define _FBXSDK_FILEIO_FBX_IO_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxstring.h>
#include <fbxsdk/core/base/fbxtime.h>
#include <fbxsdk/core/base/fbxstatus.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxIO
class FbxReader
class FbxWriter
class FbxFile
class FbxStream
class FbxXRefManager
#define FBX_FILE_VERSION_2000		2000	
#define FBX_FILE_VERSION_2001		2001	
#define FBX_FILE_VERSION_3000		3000	
#define FBX_FILE_VERSION_3001		3001	
#define FBX_FILE_VERSION_4000		4000	
#define FBX_FILE_VERSION_4001		4001	
#define FBX_FILE_VERSION_4050		4050	
#define FBX_FILE_VERSION_5000		5000	
#define FBX_FILE_VERSION_5800		5800	
#define FBX_FILE_VERSION_6000		6000	
#define FBX_FILE_VERSION_6100		6100	
#define FBX_FILE_VERSION_7000		7000	
#define FBX_FILE_VERSION_7099		7099	
#define FBX_FILE_VERSION_7100		7100	
#define FBX_FILE_VERSION_7200		7200	
#define FBX_FILE_VERSION_7300		7300	
#define FBX_FILE_VERSION_7400		7400	
#define FBX_FILE_VERSION_7500		7500	
#define FBX_53_MB55_COMPATIBLE		"FBX53_MB55"
#define FBX_60_COMPATIBLE			"FBX60_MB60"
#define FBX_2005_08_COMPATIBLE		"FBX200508_MB70"
#define FBX_2006_02_COMPATIBLE		"FBX200602_MB75"
#define FBX_2006_08_COMPATIBLE		"FBX200608"
#define FBX_2006_11_COMPATIBLE		"FBX200611"
#define FBX_2009_00_COMPATIBLE		"FBX200900"
#define FBX_2009_00_V7_COMPATIBLE	"FBX200900v7"
#define FBX_2010_00_COMPATIBLE		"FBX201000"
#define FBX_2011_00_COMPATIBLE		"FBX201100"
#define FBX_2012_00_COMPATIBLE		"FBX201200"
#define FBX_2013_00_COMPATIBLE		"FBX201300"
#define FBX_2014_00_COMPATIBLE		"FBX201400"
#define FBX_2016_00_COMPATIBLE		"FBX201600"
#define FBX_DEFAULT_FILE_VERSION		FBX_FILE_VERSION_7500
#define FBX_DEFAULT_FILE_COMPATIBILITY	FBX_2016_00_COMPATIBLE
FBXSDK_DLL int FbxFileVersionStrToInt(const char* pFileVersion)
class FBXSDK_DLL FbxIODefaultRenderResolution
public:
    FbxString mCameraName
    double mResolutionW
    FbxIODefaultRenderResolution()
    void Reset()
class FBXSDK_DLL FbxIOFileHeaderInfo
public:
    FbxIOFileHeaderInfo()
    virtual ~FbxIOFileHeaderInfo()
	virtual void				Reset()
    virtual bool				ReadExtendedHeaderInformation(FbxIO*)
    FbxIODefaultRenderResolution    mDefaultRenderResolution
    int                         mFileVersion
    FbxLocalTime                  mCreationTimeStamp
    FbxString                     mCreator
    bool                        mPLE
class FBXSDK_DLL FbxIO
public:
    struct FbxAutoResetXRefManager
        FbxIO*                   mFbx
        const FbxXRefManager*  mXRefManager
        ~FbxAutoResetXRefManager()
            if( mFbx )
                mFbx->ProjectSetXRefManager(mXRefManager)
	enum BinaryType
		BinaryNormal,	
		BinaryLarge		
    static FbxIO* Create(BinaryType pBinaryType, FbxStatus& pStatus)
 return FbxNew< FbxIO >(pBinaryType, pStatus)
    virtual ~FbxIO()
    bool ProjectOpen(void* pAddress, FbxULong pMaxLength, FbxReader* pReader, bool pCheckCRC = false, bool pOpenMainSection = true, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectOpen(const char* pName, FbxReader* pReader, bool pCheckCRC = false, bool pOpenMainSection = true, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectOpen(FbxStream* pStream, void* pStreamData, FbxReader* pReader, bool pCheckCRC = false, bool pOpenMainSection = true, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectOpenDirect(const char* pName, FbxReader* pReader, bool pCheckCRC = false, bool pOpenMainSection = true, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectCreate(void* pAddress, FbxUInt pSize, FbxWriter* pWriter, bool pBinary, bool pEncrypted, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectCreate(const char* pName, FbxWriter* pWriter, bool pBinary, bool pEncrypted, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectCreate(FbxStream* pStream, void* pStreamData, FbxWriter* pWriter, bool pBinary, bool pEncrypted, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectCreateDirect(const char* pName, FbxWriter* pWriter, bool pBinary, bool pEncrypted, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
    bool ProjectCreateEmpty(const char* pName, FbxWriter* pWriter, int pVersion, bool pBinary, bool pEncrypted)
    bool ProjectCreateEmpty(FbxStream* pStream, void* pStreamData, FbxWriter* pWriter, int pVersion, bool pBinary, bool pEncrypted)
    bool ProjectWrite_BeginFileHeader()
    bool ProjectWrite_BeginExtendedHeader()
    bool ProjectWrite_WriteExtendedHeader(const FbxIOFileHeaderInfo* pExtendedHeader)
    bool ProjectWrite_EndExtendedHeader()
    bool ProjectWrite_EndFileHeader()
    bool ProjectClose(void** pData=0, size_t* pSize=0)
    void ProjectSetXRefManager(const FbxXRefManager*)
    const FbxXRefManager* ProjectGetXRefManager() const
    bool ProjectCreateEmbeddedFolder(const FbxXRefManager& pXRefManager, FbxString& pCreatedFolder, const char* pUserDefinedFolder = NULL)
    void SetEmbedded(bool pValue)
    void SetEmbeddingExtractionFolder(const char* pExtractionFolder)
    const char* GetEmbeddingExtractionFolder()
    bool IsEmbedded() const
    bool IsBinary() const
    bool IsEncrypted () const
    bool CheckCRC()
    FbxUInt32 GetFileVersionNumber() const
    void CacheSize(FbxUInt32 pCacheSize)
    FbxUInt32 CacheSize() const
    bool Fbx7Support() const
    void Fbx7Support(bool pSupport)
    bool CompressArrays() const
    void CompressArrays(bool pCompress)
    int  CompressMinimumSize() const
    void CompressMinimumSize(int pSize)
    int  CompressLevel() const
    void CompressLevel(int pLevel)
    bool ProjectOpenMainSection()
    int ProjectGetExtensionSectionCount() const
    bool ProjectOpenExtensionSection(int pExtensionSectionIndex)
    bool ProjectCreateExtensionSection(bool pOverwriteLastExtensionSection = false)
    void ProjectCloseSection()
    int ProjectGetCurrentSection() const
    int ProjectGetCurrentSectionMode() const
    int ProjectGetCurrentSectionVersion() const
    int ProjectGetSectionVersion(int pSection) const
    static void ProjectConvertVersionNumber(int pVersion, int& pMajor, int& pMinor, int& pRevision)
    bool IsPasswordProtected() const
    bool CheckPassword(const char* pPassword)
    bool WritePassword(const char* pPassword)
    const char* GetFilename() const
    FbxString GetDataDirectory(bool pAutoCreate = true)
    FbxString GetMediaDirectory(bool pCreate = false, const char* pUserDefinedFolder = NULL)
    FbxString GetContainerTemplateDirectory(const char* pTemplateName, bool pCreate)
    char* GetRelativePath(const char* pPath)
    char* GetRelativeFilePath(const char* pFilePath)
    char* GetFullPath(const char* pRelativePath)
    char* GetFullFilePath(const char* pRelativeFilePath)
    char* GetTmpProjectName(const char* pName) const
    bool SwapFromTmpProject(const char* pName, char* pError=NULL, int pErrorSize=0)
    void FieldReadResetPosition()
    int FieldGetCount() const
    const char* FieldGetName(int pFieldIndex) const
    int FieldGetInstanceCount(const char* pFieldName) const
    bool FieldReadBegin(int pFieldIndex, int pInstance)
    bool FieldReadBegin(const char* pFieldName)
    bool FieldReadBegin(const char* pFieldName, int pInstance)
    void FieldReadEnd()
    bool FieldReadIsBlock()
    bool FieldReadBlockBegin()
    void FieldReadBlockEnd()
    int FieldReadGetCount() const
    int FieldReadGetRemain() const
    char FieldReadGetType() const
    char FieldReadCH()
    char FieldReadCH(const char* pFieldName, char pDefault=0)
    const char* FieldReadC()
    const char* FieldReadC(const char* pFieldName, const char* pDefault="")
    const char* FieldReadS()
    const char* FieldReadS(const char* pFieldName, const char* pDefault="")
    bool FieldReadB()
    bool FieldReadB(const char* pFieldName, bool pDefault = false)
    int FieldReadI()
int FieldReadI(const char* pFieldName, int pDefault=0)
    FbxLongLong FieldReadLL()
    FbxLongLong FieldReadLL(const char* pFieldName, FbxLongLong pDefault=0)
    float FieldReadF()
    float FieldReadF(const char* pFieldName, float pDefault=0)
    double FieldReadD()
    double FieldReadD(const char* pFieldName, double pDefault=0)
    FbxTime FieldReadT(const char* pFieldName)
    FbxTime FieldReadT()
    FbxTimeSpan FieldReadTS(const char* pFieldName)
    FbxTimeSpan FieldReadTS()
    void FieldReadFn(float* pValue, FbxUInt pn)
    void FieldRead3F(float* pValue)
    void FieldRead4F(float* pValue)
    void FieldReadFn(const char* pFieldName, float* pValue, const float *pDefault, FbxUInt pn)
    void FieldRead3F(const char* pFieldName, float* pValue, const float* pDefault=NULL)
    void FieldRead4F(const char* pFieldName, float* pValue, const float* pDefault=NULL)
    void FieldReadDn(double* pValue, FbxUInt pn)
    void FieldRead3D(double* pValue)
    void FieldRead4D(double* pValue)
    void FieldReadDn(const char* pFieldName, double* pValue, const double *pDefault, FbxUInt pn)
    void FieldRead3D(const char* pFieldName, double* pValue, const double* pDefault=NULL)
    void FieldRead4D(const char* pFieldName, double* pValue, const double* pDefault=NULL)
    void* FieldReadR(int* pByteSize)
    void* FieldReadR(const char* pFieldName,int* pByteSize)
    FbxChar FieldReadByte()
    FbxChar FieldReadByte(const char* pFieldName, FbxChar pDefault=0)
    FbxUChar FieldReadUByte()
    FbxUChar FieldReadUByte(const char* pFieldName, FbxUChar pDefault=0)
    FbxShort FieldReadShort()
    FbxShort FieldReadShort(const char* pFieldName, FbxShort pDefault=0)
    FbxUShort FieldReadUShort()
    FbxUShort FieldReadUShort(const char* pFieldName, FbxUShort pDefault=0)
    unsigned int FieldReadUI()
    unsigned int FieldReadUI(const char* pFieldName, unsigned int pDefault=0)
    FbxULongLong FieldReadULL()
    FbxULongLong FieldReadULL(const char* pFieldName, FbxULongLong pDefault=0)
	const FbxChar*		FieldReadArraySBytes( int &pCount )
	const FbxShort*		FieldReadArrayShort	( int &pCount )
	const FbxUShort*		FieldReadArrayUShort( int &pCount )
	const unsigned int*	FieldReadArrayUI	( int &pCount )
	const FbxULongLong*	FieldReadArrayULL	( int &pCount )
	const FbxChar*        FieldReadArray(int &pCount, const FbxChar*)
	const FbxShort*		FieldReadArray(int &pCount, const FbxShort*)
	const FbxUShort*		FieldReadArray(int &pCount, const FbxUShort*)
	const unsigned int* FieldReadArray(int &pCount, const unsigned int*)
	const FbxULongLong*	FieldReadArray(int &pCount, const FbxULongLong*)
    virtual bool FieldReadEmbeddedFile (FbxString& pFileName, FbxString& pRelativeFileName, const char* pEmbeddedMediaDirectory = "", bool *pIsFileCreated=NULL)
    const double*   FieldReadArrayD( int &pCount )
    const float*    FieldReadArrayF( int &pCount )
    const int*      FieldReadArrayI( int &pCount )
    const FbxLongLong*FieldReadArrayLL(int &pCount )
    const bool*     FieldReadArrayB( int &pCount )
    const FbxUChar*   FieldReadArrayBytes( int &pCount )
    const int*    FieldReadArray(int& pCount, const int*)
    const float*  FieldReadArray(int& pCount, const float*)
    const double* FieldReadArray(int& pCount, const double*)
    const FbxLongLong* FieldReadArray(int& pCount, const FbxLongLong*)
    const bool* FieldReadArray(int& pCount, const bool*)
    const FbxUChar* FieldReadArray(int& pCount, const FbxUChar*)
    void FieldWriteBegin(const char* pFieldName)
    void FieldWriteEnd()
    void FieldWriteBlockBegin()
    void FieldWriteObjectBegin(const char* pObjectType, const char* pName, const char* pSubType=NULL)
    void FieldWriteObjectEnd()
    void FieldWriteBlockBegin(const char* pFileName)
    void FieldWriteBlockEnd ()
    void FieldWriteCH(char pValue)
    void FieldWriteCH(const char* pFieldName, char pValue)
    void FieldWriteC(const char* pValue)
    void FieldWriteC(const char* pFieldName, const char* pValue)
    void FieldWriteS(const char* pValue)
    void FieldWriteS(const FbxString& pValue)
    void FieldWriteS(const char* pFieldName, const char* pValue)
    void FieldWriteS(const char* pFieldName, const FbxString& pValue)
    void FieldWriteB(bool pValue)
    void FieldWriteB(const char* pFieldName, bool pValue)
    void FieldWriteI(int pValue)
    void FieldWriteI(const char* pFieldName, int pValue)
    void FieldWriteLL(FbxLongLong pValue)
    void FieldWriteLL(const char* pFieldName, FbxLongLong pValue)
    void FieldWriteF(float pValue)
    void FieldWriteF(const char* pFieldName, float pValue)
    void FieldWriteD(double  pValue)
    void FieldWriteD(const char* pFieldName, double pValue)
    void FieldWriteT(FbxTime pTime)
    void FieldWriteT(const char* pFieldName,FbxTime pValue)
    void FieldWriteTS(FbxTimeSpan pTimeSpan)
    void FieldWriteTS(const char* pFieldName,FbxTimeSpan pValue)
    void FieldWriteFn(const float* pValue, FbxUInt pn)
    void FieldWriteFn(const char* pFieldName, const float* pValue, FbxUInt pn)
    void FieldWrite3F(const float* pValue)
    void FieldWrite3F(const char* pFieldName, const float* pValue)
    void FieldWrite4F(const float* pValue)
    void FieldWrite4F(const char* pFieldName, const float* pValue)
    void FieldWriteDn(const double* pValue, FbxUInt pn)
    void FieldWriteDn(const char* pFieldName, const double* pValue, FbxUInt pn)
    void FieldWrite3D(const double* pValue)
    void FieldWrite3D(const char* pFieldName, const double* pValue)
    void FieldWrite4D(const double* pValue)
    void FieldWrite4D(const char* pFieldName, const double* pValue)
 if the stride is zero, values are tighly packed together.
    void FieldWriteArrayD( int n, const double*     pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayF( int n, const float*      pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayI( int n, const int*        pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayLL(int n, const FbxLongLong*  pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayB( int n, const bool*       pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayBytes( int n, const FbxUChar* pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteR(const void* pRawData, int pByteSize)
    void FieldWriteR(const char* pFieldName, const void* pRawData, int pByteSize)
    void FieldWriteByte(FbxChar pValue)
    void FieldWriteByte(const char* pFieldName, FbxChar pValue)
    void FieldWriteUByte(FbxUChar pValue)
    void FieldWriteUByte(const char* pFieldName, FbxUChar pValue)
    void FieldWriteShort(FbxShort pValue)
    void FieldWriteShort(const char* pFieldName, FbxShort pValue)
    void FieldWriteUShort(FbxUShort pValue)
    void FieldWriteUShort(const char* pFieldName, FbxUShort pValue)
    void FieldWriteUI(unsigned int pValue)
    void FieldWriteUI(const char* pFieldName, unsigned int pValue)
    void FieldWriteULL(FbxULongLong pValue)
    void FieldWriteULL(const char* pFieldName, FbxULongLong pValue)
	void FieldWriteArraySBytes( int n, const FbxChar* pValue, int pSize = 1, int pStride = 0 )
	void FieldWriteArrayShort( int n, const FbxShort* pValue, int pSize = 1, int pStride = 0 )
	void FieldWriteArrayUShort( int n, const FbxUShort* pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayUI( int n, const unsigned int*        pValue, int pSize = 1, int pStride = 0 )
    void FieldWriteArrayULL(int n, const FbxULongLong*  pValue, int pSize = 1, int pStride = 0 )
    int GetFieldRMaxChunkSize() const
    void FieldWriteObjectReference(const char* pName)
    void FieldWriteObjectReference(const char* pFieldName, const char* pName)
    bool FieldWriteEmbeddedFile (FbxString pFileName, FbxString pRelativeFileName)
    void WriteComments(const char* pFieldName)
#ifdef _DEBUG
    void StdoutDump()
#endif
    bool GetHaveLoadedEmbededFile() const
    void GetMemoryFileInfo(void** pMemPtr, size_t& pSize) const
    bool    IsBeforeVersion6() const
    void    SetIsBeforeVersion6(bool pIsBeforeVersion6)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    bool ProjectOpen (FbxFile * pFile, FbxReader* pReader, bool pCheckCRC = false, bool pOpenMainSection = true, FbxIOFileHeaderInfo* pFileHeaderInfo = NULL)
private:
    FbxIO& operator=(const FbxIO& pOther)
    FbxStatus& mStatus
    struct InternalImpl
	struct InternalImpl32
	struct InternalImpl64
    InternalImpl* mImpl
    void ProjectClear()
    void ProjectReset()
    bool ProjectReadHeader(bool pCheckASCIIHeader, bool pCheckCRC, bool pOpenMainSection, FbxIOFileHeaderInfo* pFileHeaderInfo)
    bool ProjectReadExtendedHeader(FbxInt64& pExtendedHeaderEnd, FbxIOFileHeaderInfo* pFileHeaderInfo)
    bool BinaryReadHeader()
    bool BinaryReadSectionPosition()
    bool ASCIIReadHeader()
    bool ASCIIReadSectionPosition()
    bool ProjectWriteHeader(FbxIOFileHeaderInfo* pFileHeaderInfo)
    bool ProjectWriteExtendedHeader(FbxIOFileHeaderInfo* pFileHeaderInfo)
    void BinaryWriteHeader()
    void ASCIIWriteHeader()
    void ReadEncryptionKey(char* pEncryptionKey)
    void WriteEncryptionKey(char* pEncryptionKey)
    bool ProjectClearSection()
    bool ProjectOpenSection(int pSection)
    bool BinaryReadSectionHeader()
    FbxInt64 BinaryReadSectionFooter(char* pSourceCheck)
    bool BinaryReadExtensionCode(FbxInt64 pFollowingSectionStart, FbxInt64& pSectionStart, FbxUInt32& pSectionVersion)
    void BinaryReadSectionPassword()
    bool ProjectWriteSectionHeader()
    void BinaryWriteSectionFooter()
    bool BinaryWriteExtensionCode(FbxInt64 pSectionStart, FbxUInt32 pSectionVersion)
    FbxString GetCreationTime() const
    void SetCreationTime(FbxString pCreationTime)
    void CreateSourceCheck(char* lSourceCheck)
    bool TestSourceCheck(char* pSourceCheck, char* pSourceCompany)
    FbxString GetMangledCreationTime()
    void EncryptSourceCheck(char* pSourceCheck, char* pEncryptionData)
    void DecryptSourceCheck(char* pSourceCheck, const char* pEncryptionData)
    void EncryptPasswordV1(FbxString pOriginalPassword, FbxString &pEncryptedPassword)
    void DecryptPasswordV1(FbxString pEncryptedPassword, FbxString &pDecryptedPassword)
    void CheckValidityOfFieldName(const char* pFieldName)
    void GetUnusedEmbeddedName(const FbxString& pDirectory, const FbxString& pName, FbxString& pResult, bool pCreateSubdirectory)
    FbxString GetDirectory(bool pAutoCreate, const char* pExtension)
#endif 