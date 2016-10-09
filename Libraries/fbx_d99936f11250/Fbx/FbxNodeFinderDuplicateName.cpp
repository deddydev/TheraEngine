#pragma once
#include "stdafx.h"
#include "FbxNodeFinderDuplicateName.h"
#include "FbxNode.h"
#include "FbxScene.h"
#include "FbxSurfaceMaterial.h"
#include "FbxTexture.h"
#include "FbxVideo.h"
#include "FbxGenericNode.h"

#define GET_ARRAY_METHOD(ManagedType,MethodName) array<ManagedType^>^ FbxNodeFinderDuplicateName::MethodName(){\
	KArrayTemplate<K##ManagedType*> arr = _Ref()->MethodName();\
	if(arr.GetCount() > 0){\
	array<ManagedType^>^ nodes = gcnew array<ManagedType^>(arr.GetCount());\
	for(int i=0; i < nodes->Length; i++){nodes[i] = gcnew ManagedType(arr[i]);}\
	return nodes;}return nullptr;}

namespace Skill
{
	namespace FbxSDK
	{
		FbxNodeFinderDuplicateName::FbxNodeFinderDuplicateName(FbxSceneManaged^ destinationScene)
			:FbxNodeFinder(new KFbxNodeFinderDuplicateName(destinationScene->_Ref()))
		{
			_Free = true;
		}
		FbxNodeFinderDuplicateName::FbxNodeFinderDuplicateName()
			:FbxNodeFinder(new KFbxNodeFinderDuplicateName())
		{
			_Free = true;
		}

		bool FbxNodeFinderDuplicateName::GetState(int stateIndex)
		{
			return _Ref()->GetState(stateIndex);
		}
		void FbxNodeFinderDuplicateName::SetState(int stateIndex, bool value)
		{
			_Ref()->SetState(stateIndex,value);
		}
		GET_ARRAY_METHOD(FbxNode,GetNodeArray);
		GET_ARRAY_METHOD(FbxNode,GetDuplicateNodeArray);

		GET_ARRAY_METHOD(FbxSurfaceMaterial,GetMaterialArray);
		GET_ARRAY_METHOD(FbxSurfaceMaterial,GetDuplicateMaterialArray);

		GET_ARRAY_METHOD(FbxTexture,GetTextureArray);
		GET_ARRAY_METHOD(FbxTexture,GetDuplicateTextureArray);


		GET_ARRAY_METHOD(FbxVideo,GetVideoArray);
		GET_ARRAY_METHOD(FbxVideo,GetDuplicateVideoArray);

		GET_ARRAY_METHOD(FbxGenericNode,GetGenericNodeArray);
		GET_ARRAY_METHOD(FbxGenericNode,GetDuplicateGenericNodeArray);
	}
}