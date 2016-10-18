#ifndef _FBXSDK_SCENE_GEOMETRY_DEFORMER_H_
#define _FBXSDK_SCENE_GEOMETRY_DEFORMER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FBXSDK_DLL FbxDeformer : public FbxObject
    FBXSDK_OBJECT_DECLARE(FbxDeformer, FbxObject)
public:
        void SetMultiLayer(bool pMultiLayer)
        bool GetMultiLayer() const
        enum EDeformerType
            eUnknown,		
            eSkin,			
			eBlendShape,	
            eVertexCache	
        virtual EDeformerType GetDeformerType() const 
 return eUnknown
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void Construct(const FbxObject* pFrom)
	virtual FbxStringList GetTypeFlags() const 
 return FbxStringList()
	bool		mMultiLayer
#endif 