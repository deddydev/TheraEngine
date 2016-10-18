#ifndef _FBXSDK_SCENE_GEOMETRY_LOD_GROUP_H_
#define _FBXSDK_SCENE_GEOMETRY_LOD_GROUP_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/geometry/fbxnodeattribute.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxLODGroup : public FbxNodeAttribute
    FBXSDK_OBJECT_DECLARE(FbxLODGroup, FbxNodeAttribute)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
	enum EDisplayLevel
		eUseLOD,
		eShow,
		eHide
	FbxPropertyT<FbxBool>  ThresholdsUsedAsPercentage
		FbxPropertyT<FbxBool>       MinMaxDistance
		FbxPropertyT<FbxDouble>     MinDistance
		FbxPropertyT<FbxDouble>     MaxDistance
		FbxPropertyT<FbxBool>       WorldSpace
	int GetNumThresholds() const
	bool AddThreshold(const FbxDistance& pThreshValue)
	bool AddThreshold(FbxDouble pThreshValue)
	bool SetThreshold(int pEl, const FbxDistance& pThreshValue)
	bool SetThreshold(int pEl, FbxDouble pThreshValue)
	bool GetThreshold(int pEl, FbxDistance& pThreshValue) const
	bool GetThreshold(int pEl, FbxDouble& pThreshValue) const
	int GetNumDisplayLevels() const
	bool AddDisplayLevel(FbxLODGroup::EDisplayLevel pValue)
	bool SetDisplayLevel(int pEl, FbxLODGroup::EDisplayLevel pValue)
	bool GetDisplayLevel(int pEl, FbxLODGroup::EDisplayLevel& pValue) const
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
private:
	int mNbThresholds
	FbxProperty mThresholds
	bool RetrieveThreshold(int pEl, FbxDistance& pThreshValue) const
	bool StoreThreshold(int pEl, const FbxDistance& pThreshValue)
	int mNbDisplayLevels
	FbxProperty mDisplayLevels
	bool DisplayLevel(int pEl, FbxLODGroup::EDisplayLevel pValue)
public:
    virtual FbxStringList GetTypeFlags() const
#endif 