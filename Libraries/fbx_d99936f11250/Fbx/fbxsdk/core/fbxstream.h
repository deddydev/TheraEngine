#ifndef _FBXSDK_CORE_STREAM_H_
#define _FBXSDK_CORE_STREAM_H_
class FBXSDK_DLL FbxStream
{
public:
enum EState
{
eClosed,
eOpen,
eEmpty
};
virtual EState GetState() = 0;
virtual bool Open(void* pStreamData) = 0;
virtual bool Close() = 0;
virtual bool Flush() = 0;
virtual int Write(const void*  , int  ) = 0;
virtual int Read(void*  , int  ) const = 0;
virtual char* ReadString(char* pBuffer, int pMaxSize, bool pStopAtFirstWhiteSpace=false);
virtual int GetReaderID() const = 0;
virtual int GetWriterID() const = 0;
virtual void Seek(const FbxInt64& pOffset, const FbxFile::ESeekPos& pSeekPos)=0;
virtual long GetPosition() const = 0;
virtual void SetPosition(long pPosition)=0;
virtual int GetError() const = 0;
virtual void ClearError() = 0;
#ifndef DOXYGEN_SHOULD_SKIP_THIS
FbxStream(){};
virtual ~FbxStream(){};
int Write(const char* pData, int pSize){ return Write((void*)pData, pSize); }
int Write(const int* pData, int pSize){ return Write((void*)pData, pSize); }
int Read(char* pData, int pSize) const { return Read((void*)pData, pSize); }
int Read(int* pData, int pSize) const { return Read((void*)pData, pSize); }
#endif
};
#endif