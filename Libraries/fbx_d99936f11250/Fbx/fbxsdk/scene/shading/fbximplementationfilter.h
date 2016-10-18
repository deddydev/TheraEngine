#ifndef _FBXSDK_SCENE_SHADING_IMPLEMENTATION_FILTER_H_
#define _FBXSDK_SCENE_SHADING_IMPLEMENTATION_FILTER_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/scene/fbxobjectfilter.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxCriteria
class FBXSDK_DLL FbxImplementationFilter : public FbxObjectFilter
public:
	static const char * sCHR_ANY_SHADING_API
	static const char * sCHR_ANY_SHADING_API_VERSION
	static const char * sCHR_ANY_SHADING_LANGUAGE
	static const char * sCHR_ANY_SHADING_LANGUAGE_VERSION
	FbxImplementationFilter(
		const char * pShadingAPI				= sCHR_ANY_SHADING_API,
		const char * pShadingAPIVersion			= sCHR_ANY_SHADING_API_VERSION,
		const char * pShadingLanguage			= sCHR_ANY_SHADING_LANGUAGE,
		const char * pShadingLanguageVersion	= sCHR_ANY_SHADING_LANGUAGE_VERSION
	)
    virtual ~FbxImplementationFilter()
	virtual bool Match(const FbxObject * pObjectPtr) const
	FbxString mShadingAPI
	FbxString mShadingAPIVersion
	FbxString mShadingLanguage
	FbxString mShadingLanguageVersion
#ifndef DOXYGEN_SHOULD_SKIP_THIS
	static bool IsShadingObject( const FbxObject* pObject )
	static FbxCriteria Criteria()
#endif 