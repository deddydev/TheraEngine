#pragma once
#include "stdafx.h"
#include "FbxCharacterPose.h"
#include "FbxCharacter.h"
#include "FbxXMatrix.h"
#include "FbxNode.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"



{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxCharacterPose,KFbxCharacterPose);

		void FbxCharacterPose::CollectManagedMemory()
		{
			_RootNode = nullptr;
			_Character = nullptr;
			FbxObjectManaged::CollectManagedMemory();
		}		

		void FbxCharacterPose::Reset()
		{
			_Ref()->Reset();
		}		

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCharacterPose,KFbxNode,GetRootNode(),FbxNode,RootNode);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCharacterPose,KFbxCharacter,GetCharacter(),FbxCharacter,Character);		

#ifndef DOXYGEN_SHOULD_SKIP_THIS		
		CLONE_DEFINITION(FbxCharacterPose,KFbxCharacterPose);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		

	}
}