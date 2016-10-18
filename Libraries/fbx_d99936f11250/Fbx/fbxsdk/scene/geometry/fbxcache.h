#ifndef _FBXSDK_SCENE_GEOMETRY_CACHE_H_
#define _FBXSDK_SCENE_GEOMETRY_CACHE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/core/base/fbxstatus.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCache_internal
class FBXSDK_DLL FbxCache : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxCache, FbxObject)
public:
		void SetCacheFileFormat(EFileFormat pFileFormat, FbxStatus* pStatus=NULL)
		EFileFormat GetCacheFileFormat() const
		void SetCacheFileName(const char* pRelativeFileName_UTF8, const char* pAbsoluteFileName_UTF8, FbxStatus* pStatus=NULL)
		void GetCacheFileName(FbxString& pRelativeFileName_UTF8, FbxString& pAbsoluteFileName_UTF8) const
		bool OpenFileForRead(FbxStatus* pStatus=NULL)
		bool IsOpen(FbxStatus* pStatus=NULL) const
		bool Read(float** pBuffer, unsigned int& pBufferLength, const FbxTime& pTime, unsigned int pChannel=0)
		bool CloseFile(FbxStatus* pStatus=NULL)
		double GetSamplingFrameRate(FbxStatus* pStatus=NULL)
		FbxTime GetCacheTimePerFrame(FbxStatus* pStatus=NULL)
		int GetChannelCount(FbxStatus* pStatus=NULL)
		bool GetChannelName(int pChannelIndex, FbxString& pChannelName, FbxStatus* pStatus=NULL)
		enum EMCFileCount
			eMCOneFile,			
			eMCOneFilePerFrame	
		enum EMCDataType
			eUnknownData,		
			eDouble,			
			eDoubleArray,		
			eDoubleVectorArray,	
			eInt32Array,		
			eFloatArray,		
			eFloatVectorArray	
		enum EMCBinaryFormat
			eMCC,	
			eMCX	
		enum EMCSamplingType
			eSamplingRegular,	
			eSamplingIrregular	
		bool OpenFileForWrite(EMCFileCount pFileCount, double pSamplingFrameRate, const char* pChannelName, EMCBinaryFormat pBinaryFormat, EMCDataType pMCDataType=eDoubleVectorArray, const char* pInterpretation="Points", FbxStatus* pStatus=NULL)
		bool AddChannel(const char* pChannelName, EMCDataType pMCDataType, const char* pInterpretation, unsigned int& pChannelIndex, FbxStatus* pStatus=NULL)
		bool GetChannelDataType(int pChannelIndex, EMCDataType& pChannelType, FbxStatus* pStatus=NULL)
		int  GetChannelIndex(const char* pChannelName, FbxStatus* pStatus=NULL)
		bool Read(int pChannelIndex, FbxTime& pTime, double* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool Read(int pChannelIndex, FbxTime& pTime, float* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool Read(int pChannelIndex, FbxTime& pTime, int* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool BeginWriteAt( FbxTime& pTime, FbxStatus* pStatus=NULL )
		bool Write(int pChannelIndex, FbxTime& pTime, double* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool Write(int pChannelIndex, FbxTime& pTime, float* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool Write(int pChannelIndex, FbxTime& pTime, int* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool EndWriteAt(FbxStatus* pStatus=NULL)
		bool GetAnimationRange(int pChannelIndex, FbxTime &pTimeStart, FbxTime &pTimeEnd, FbxStatus* pStatus=NULL)
		bool GetCacheType(EMCFileCount& pFileCount, FbxStatus* pStatus=NULL)
		bool GetChannelInterpretation(int pChannelIndex, FbxString& pInterpretation, FbxStatus* pStatus=NULL)
		bool GetChannelSamplingType(int pChannelIndex, EMCSamplingType& pSamplingType, FbxStatus* pStatus=NULL)
		bool GetChannelSamplingRate(int pChannelIndex, FbxTime& pSamplingRate, FbxStatus* pStatus=NULL)
		bool GetChannelSampleCount(int pChannelIndex, unsigned int& pSampleCount, FbxStatus* pStatus=NULL)
		bool GetChannelPointCount(int pChannelIndex, FbxTime pTime, unsigned int& pPointCount, FbxStatus* pStatus=NULL)
		int GetCacheDataFileCount(FbxStatus* pStatus=NULL) const
		bool GetCacheDataFileName(int pIndex, FbxString& pRelativeFileName, FbxString& pAbsoluteFileName, FbxStatus* pStatus=NULL)
		bool EnableMultiChannelFetching(bool pMultiChannelFetching, FbxStatus* pStatus=NULL)
		bool GetNextTimeWithData(FbxTime pCurTime, FbxTime& pNextTime, int pChannelIndex = -1, FbxStatus* pStatus=NULL)
		int GetDataCount(int pChannelIndex, FbxStatus* pStatus=NULL)
		bool GetDataTime(int pChannelIndex, unsigned int pDataIndex, FbxTime& pTime, FbxStatus* pStatus=NULL)
		bool OpenFileForWrite(double pFrameStartOffset, double pSamplingFrameRate, unsigned int pSampleCount, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		unsigned int GetSampleCount(FbxStatus* pStatus=NULL)
		unsigned int GetPointCount(FbxStatus* pStatus=NULL)
		double GetFrameStartOffset(FbxStatus* pStatus=NULL)
		bool Read(unsigned int pFrameIndex, double* pBuffer, unsigned int pPointCount, FbxStatus* pStatus=NULL)
		bool Write(unsigned int pFrameIndex, double* pBuffer, FbxStatus* pStatus=NULL)
		bool ConvertFromPC2ToMC(EMCFileCount pFileCount, double pSamplingFrameRate, EMCBinaryFormat pBinaryFormat, FbxStatus* pStatus=NULL)
		bool ConvertFromMCToPC2(double pSamplingFrameRate, unsigned int pChannelIndex, FbxStatus* pStatus=NULL)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	enum EOpenFlag
		eReadOnly,
		eWriteOnly
protected:
	bool OpenFile(EOpenFlag pFlag, EMCFileCount pFileCount, double pSamplingFrameRate, const char* pChannelName, const char* pInterpretation, unsigned int pSampleCount, unsigned int pPointCount, double pFrameStartOffset, FbxStatus* pStatus, EMCDataType pMCDataType = eDoubleVectorArray, EMCBinaryFormat pBinaryFormat = eMCX)
	virtual void Construct( const FbxObject* pFrom )
	virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
	FbxCache_internal* mData
private:
	bool AllocateReadBuffer(unsigned int pTypeSize, unsigned int pTypeLength, unsigned int pLength, bool pAllocateConvertBuffer)
	bool ReadMayaCache(float** pBuffer, unsigned int& pBufferLength, const FbxTime& pTime, unsigned int pChannel)
	bool ReadMaxCache(float** pBuffer, unsigned int& pBufferLength, const FbxTime& pTime)
	bool ReadAlembicCache(float** pBuffer, unsigned int& pBufferLength, const FbxTime& pTime, unsigned int pChannel)
	FbxPropertyT<FbxString> CacheFile
	FbxPropertyT<FbxString> CacheFileAbsolutePath
	FbxPropertyT<FbxEnum> CacheFileType
	void*			mReadBuffer
	unsigned int	mReadBufferLength
	unsigned int	mReadBufferSize
	unsigned int	mReadTypeSize
	unsigned int	mReadTypeLength
	unsigned int	mReadLength
	void*			mConvertBuffer
#endif 