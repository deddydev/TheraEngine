#ifndef _FBXSDK_SCENE_GEOMETRY_MARKER_H_
#define _FBXSDK_SCENE_GEOMETRY_MARKER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxMarker : public FbxNodeAttribute
	FBXSDK_OBJECT_DECLARE(FbxMarker, FbxNodeAttribute)
public:
	virtual FbxNodeAttribute::EType GetAttributeType() const
	void Reset()
	enum EType
		eStandard, 
		eOptical, 
		eEffectorFK,
		eEffectorIK
	void SetType(EType pType)
	EType GetType() const
	enum ELook
		eCube, 
		eHardCross, 
		eLightCross, 
        eSphere,
        eCapsule,
        eBox,
        eBone,
        eCircle,
        eSquare,
        eStick,
		eNone
	double GetDefaultOcclusion() const
	void SetDefaultOcclusion(double pOcclusion)
	double GetDefaultIKReachTranslation() const
	void SetDefaultIKReachTranslation(double pIKReachTranslation)
	double GetDefaultIKReachRotation() const
	void SetDefaultIKReachRotation(double pIKReachRotation)
	double GetDefaultIKPull() const
	void SetDefaultIKPull(double pIKPull)
	double GetDefaultIKPullHips() const
	void SetDefaultIKPullHips(double pIKPullHips)
	FbxColor& GetDefaultColor(FbxColor& pColor) const
	void SetDefaultColor(FbxColor& pColor)
	FbxPropertyT<ELook> Look
    FbxPropertyT<FbxBool> DrawLink
	FbxPropertyT<FbxDouble> Size
	FbxPropertyT<FbxBool> ShowLabel
	FbxPropertyT<FbxDouble3> IKPivot
	FbxProperty GetOcclusion() const
	FbxProperty GetIKReachTranslation() const
	FbxProperty GetIKReachRotation() const
	FbxProperty GetIKPull() const
	FbxProperty GetIKPullHips() const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void ConstructProperties(bool pForceSet)
	virtual const char* GetTypeName() const
	virtual FbxStringList GetTypeFlags() const
	EType mType
#endif 