#ifndef _FBXSDK_SCENE_SHADING_TEXTURE_FILE_H_
#define _FBXSDK_SCENE_SHADING_TEXTURE_FILE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxtexture.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxFileTexture : public FbxTexture
	FBXSDK_OBJECT_DECLARE(FbxFileTexture, FbxTexture)
public:
		FbxPropertyT<FbxBool>				UseMaterial
		FbxPropertyT<FbxBool>				UseMipMap
	void Reset()
    bool SetFileName(const char* pName)
    bool SetRelativeFileName(const char* pName)
    const char* GetFileName () const
    const char* GetRelativeFileName() const
    enum EMaterialUse
        eModelMaterial,		
        eDefaultMaterial	
    void SetMaterialUse(EMaterialUse pMaterialUse)
    EMaterialUse GetMaterialUse() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	bool operator==(FbxFileTexture const& pTexture) const
	FbxString& GetMediaName()
	void SetMediaName(const char* pMediaName)
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	void Init()
	void SyncVideoFileName(const char* pFileName)
	void SyncVideoRelativeFileName(const char* pFileName)
	FbxString mFileName
	FbxString mRelativeFileName
	FbxString mMediaName
#endif 