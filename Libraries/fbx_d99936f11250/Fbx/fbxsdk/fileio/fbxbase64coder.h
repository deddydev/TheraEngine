#ifndef _FBXSDK_FILEIO_BASE64CODER_H_
#define _FBXSDK_FILEIO_BASE64CODER_H_
class FBXSDK_DLL FbxBase64Decoder
{
public:
int Decode(const void* pInBuffer, int pInSize, void* pOutBuffer, int pOutSize);
int Decode(const char* pInBuffer, void* pOutBuffer, int pOutSize);
};
class FBXSDK_DLL FbxBase64Encoder
{
public:
int Encode(const void* pInBuffer, int pInSize, void* pOutBuffer, int pOutSize);
int Encode(const void* pInBuffer, int pInSize, FbxString& pOutBuffer);
};
#endif