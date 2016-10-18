#ifndef _FBXSDK_UTILS_ROOT_NODE_UTILITY_H_
#define _FBXSDK_UTILS_ROOT_NODE_UTILITY_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxsystemunit.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxScene
class FbxAxisSystem
class FbxNode
class FBXSDK_DLL FbxRootNodeUtility 
public:
	static const char* sFbxRootNodePrefix
	static bool RemoveAllFbxRoots( FbxScene* pScene )
	static bool InsertFbxRoot(  FbxScene* pScene, 
								const FbxAxisSystem& pDstAxis, 
								const FbxSystemUnit& pDstUnit,
								const FbxSystemUnit::ConversionOptions& pUnitOptions = FbxSystemUnit::DefaultConversionOptions )
	static bool IsFbxRootNode(FbxNode* pNode)
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	FbxRootNodeUtility()
	FbxRootNodeUtility(const FbxRootNodeUtility& pOther)
	~FbxRootNodeUtility()
#endif 