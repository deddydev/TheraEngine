#ifndef _FBXSDK_SCENE_GEOMETRY_SKELETON_H_
#define _FBXSDK_SCENE_GEOMETRY_SKELETON_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSkeleton : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxSkeleton,FbxNodeAttribute)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
	void Reset()
    enum EType
	    eRoot,			
	    eLimbNode,		
    void SetSkeletonType(EType pSkeletonType)
    EType GetSkeletonType() const
	bool GetSkeletonTypeIsSet() const
	EType GetSkeletonTypeDefaultValue() const
	double GetLimbLengthDefaultValue() const
	double GetLimbNodeSizeDefaultValue() const
	bool SetLimbNodeColor(const FbxColor& pColor)
	FbxColor GetLimbNodeColor() const
	bool GetLimbNodeColorIsSet() const
	FbxColor GetLimbNodeColorDefaultValue() const
    bool IsSkeletonRoot() const
	static const char*			sSize
	static const char*			sLimbLength
	static const FbxDouble		sDefaultSize
	static const FbxDouble		sDefaultLimbLength
	FbxPropertyT<FbxDouble>		Size
	FbxPropertyT<FbxDouble>			LimbLength
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual void ConstructProperties(bool pForceSet)
	void Reset( bool pResetProperties )
	virtual const char* GetTypeName() const
	virtual FbxStringList GetTypeFlags() const
    EType mSkeletonType
	bool mLimbLengthIsSet
	bool mLimbNodeSizeIsSet
	bool mLimbNodeColorIsSet
	bool mSkeletonTypeIsSet
#endif 