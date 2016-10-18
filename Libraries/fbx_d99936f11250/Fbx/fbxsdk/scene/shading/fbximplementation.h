#ifndef _FBXSDK_SCENE_SHADING_IMPLEMENTATION_H_
#define _FBXSDK_SCENE_SHADING_IMPLEMENTATION_H_
#include <fbxsdk/fbxsdk_def.h>
#include <fbxsdk/core/fbxobject.h>
#include <fbxsdk/fbxsdk_nsbegin.h>
class FbxBindingOperator
class FbxBindingTable
class FBXSDK_DLL FbxImplementation : public FbxObject
	FBXSDK_OBJECT_DECLARE(FbxImplementation, FbxObject)
public:
	FbxString									RenderName
	FbxPropertyT<FbxString>			Language
	FbxPropertyT<FbxString>			LanguageVersion
	FbxPropertyT<FbxString>			RenderAPI
	FbxPropertyT<FbxString>			RenderAPIVersion
	FbxPropertyT<FbxString>			RootBindingName
	FbxProperty GetConstants() const
	FbxBindingTable* AddNewTable( const char* pTargetName, const char* pTargetType )
    const FbxBindingTable* GetRootTable() const
	FbxBindingTable* GetRootTable()
	int GetTableCount() const
	const FbxBindingTable* GetTable( int pIndex ) const
	FbxBindingTable* GetTable( int pIndex )
	const FbxBindingTable* GetTableByTargetName( const char* pName ) const
	FbxBindingTable* GetTableByTargetName( const char* pName )
	const FbxBindingTable* GetTableByTargetType( const char* pTargetName ) const
	FbxBindingTable* GetTableByTargetType( const char* pTargetName )
	FbxBindingOperator* AddNewBindingOperator( const char* pTargetName, const char* pFunctionName )
	int GetBindingOperatorCount() const
	const FbxBindingOperator* GetOperatorByTargetName( const char* pTargetName ) const
	static const char* sLanguage
	static const char* sLanguageVersion
	static const char* sRenderAPI
	static const char* sRenderAPIVersion
	static const char* sRootBindingName
	static const char* sConstants
	static const char* sDefaultType
	static const char* sDefaultLanguage
	static const char* sDefaultLanguageVersion
	static const char* sDefaultRenderAPI
	static const char* sDefaultRenderAPIVersion
	static const char* sDefaultRootBindingName
#ifndef DOXYGEN_SHOULD_SKIP_THIS
protected:
	virtual void ConstructProperties(bool pForceSet)
#endif 