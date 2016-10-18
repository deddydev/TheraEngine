#ifndef _FBXSDK_SCENE_THUMBNAIL_H_
#define _FBXSDK_SCENE_THUMBNAIL_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxThumbnailMembers
class FBXSDK_DLL FbxThumbnail : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxThumbnail, FbxObject)
public:
	FbxPropertyT<FbxInt> CustomHeight
	FbxPropertyT<FbxInt> CustomWidth
	enum EDataFormat
		eRGB_24, 
		eRGBA_32 
	void SetDataFormat(EDataFormat pDataFormat)
	EDataFormat GetDataFormat() const
	enum EImageSize
		eNotSet = 0,
		e64x64 = 64,
		e128x128 = 128,
		eCustomSize = -1
	void SetSize(EImageSize pImageSize)
	EImageSize GetSize() const
	unsigned long GetSizeInBytes() const
	bool SetThumbnailImage(const FbxUChar* pImage)
	FbxUChar* GetThumbnailImage() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject&	Copy(const FbxObject& pObject)
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	virtual void Destruct(bool pRecursive)
	FbxThumbnailMembers* mMembers
#endif 