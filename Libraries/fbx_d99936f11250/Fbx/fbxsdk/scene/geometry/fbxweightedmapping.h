#ifndef _FBXSDK_SCENE_GEOMETRY_WEIGHTED_MAPPING_H_
#define _FBXSDK_SCENE_GEOMETRY_WEIGHTED_MAPPING_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxWeightedMapping
public:
    enum ESet
		eSource,        
		eDestination    
	FbxWeightedMapping(int pSourceSize, int pDestinationSize)
	~FbxWeightedMapping()
	void Reset(int pSourceSize, int pDestinationSize)
	void Add(int pSourceIndex, int pDestinationIndex, double pWeight)
	int GetElementCount(ESet pSet) const
	int GetRelationCount(ESet pSet, int pElement) const
	Element& GetRelation(ESet pSet, int pElement, int pIndex)
	int GetRelationIndex(ESet pSet, int pElementInSet, int pElementInOtherSet) const
	double GetRelationSum(ESet pSet, int pElement, bool pAbsoluteValue) const
	void Normalize(ESet pSet, bool pAbsoluteValue)
    FbxWeightedMapping& operator=(const FbxWeightedMapping& pWMap)
private:
	void Clear()
	FbxArray<FbxArray<Element>*> mElements[2]
typedef class FbxArray<FbxWeightedMapping::Element> FbxArrayTemplateElement
typedef class FbxArray<FbxArray<FbxWeightedMapping::Element>*> FbxArrayTemplateArrayTemplateElement
#include <fbxsdk/fbxsdk_nsend.h>
#endif 