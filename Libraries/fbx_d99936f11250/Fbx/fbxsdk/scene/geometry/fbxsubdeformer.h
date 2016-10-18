#ifndef _FBXSDK_SCENE_GEOMETRY_SUB_DEFORMER_H_
#define _FBXSDK_SCENE_GEOMETRY_SUB_DEFORMER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxSubDeformer : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxSubDeformer, FbxObject)
    public:
        void SetMultiLayer(bool pMultiLayer)
        bool GetMultiLayer() const
        enum EType
            eUnknown,			
            eCluster,			
            eBlendShapeChannel	
        virtual EType GetSubDeformerType() const 
 return eUnknown
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual FbxStringList GetTypeFlags() const 
 return FbxStringList()
	bool		mMultiLayer
#endif 