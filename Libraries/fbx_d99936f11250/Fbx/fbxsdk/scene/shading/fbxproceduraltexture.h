#ifndef _FBXSDK_SCENE_SHADING_TEXTURE_PROCEDURAL_H_
#define _FBXSDK_SCENE_SHADING_TEXTURE_PROCEDURAL_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/shading/fbxtexture.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxProceduralTexture : public FbxTexture
	FBXSDK_OBJECT_DECLARE(FbxProceduralTexture, FbxTexture)
	public:
	FbxPropertyT<FbxBlob>			BlobProp
	void Reset()
	void SetBlob(FbxBlob& pBlob)
	FbxBlob GetBlob() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
	bool operator==(FbxProceduralTexture const& pTexture) const
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	void Init()
#endif 