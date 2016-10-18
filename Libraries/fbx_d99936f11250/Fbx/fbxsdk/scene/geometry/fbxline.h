#ifndef _FBXSDK_SCENE_GEOMETRY_LINE_H_
#define _FBXSDK_SCENE_GEOMETRY_LINE_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/base/fbxarray.h>
#include <fbxsdk/scene/geometry/fbxgeometry.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxLine : public FbxGeometry
    FBXSDK_OBJECT_DECLARE(FbxLine, FbxGeometry)
public:
    virtual FbxNodeAttribute::EType GetAttributeType() const
    void Reset()
    void SetIndexArraySize(int pCount)
    int GetIndexArraySize() const
    inline FbxArray<int>* GetIndexArray() 
 return &mPointArray
    bool SetPointIndexAt(int pValue, int pIndex, bool pAsEndPoint = false)
    int GetPointIndexAt(int pIndex) const
    bool AddPointIndex(int pValue, bool pAsEndPoint = false)
    inline FbxArray<int>* GetEndPointArray() 
 return &mEndPointArray
    bool AddEndPoint(int pPointIndex)
    int GetEndPointAt(int pEndPointIndex) const
    int GetEndPointCount() const
    FbxPropertyT<FbxBool> Renderable
#ifndef DOXYGEN_SHOULD_SKIP_THIS
    virtual FbxObject& Copy(const FbxObject& pObject)
protected:
	virtual void Construct(const FbxObject* pFrom)
    virtual void ConstructProperties(bool pForceSet)
    virtual void Destruct(bool pRecursive)
private:
    FbxArray<int> mPointArray
    FbxArray<int> mEndPointArray
#endif 