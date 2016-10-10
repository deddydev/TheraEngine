#pragma once
#include "stdafx.h"
#include "FbxRootNodeUtility.h"
#include "FbxScene.h"
#include "FbxAxisSystem.h"
#include "FbxNode.h"


{
	namespace FbxSDK
	{		
		bool FbxRootNodeUtility::RemoveAllFbxRoots(FbxSceneManaged^ scene)
		{
			return KFbxRootNodeUtility::RemoveAllFbxRoots(scene->_Ref());
		}
		bool FbxRootNodeUtility::InsertFbxRoot(FbxSceneManaged^ scene, 
			FbxAxisSystem^ dstAxis, 
			FbxSystemUnit^ dstUnit,
			FbxSystemUnit::FbxUnitConversionOptions unitOptions)
		{
			KFbxSystemUnit::KFbxUnitConversionOptions op;
			op.mConvertLightIntensity = unitOptions.ConvertLightIntensity;
			op.mConvertRrsNodes = unitOptions.ConvertRrsNodes;
			return KFbxRootNodeUtility::InsertFbxRoot(scene->_Ref(),*dstAxis->_Ref(),*dstUnit->_Ref(),op);
		}
		bool FbxRootNodeUtility::InsertFbxRoot(FbxSceneManaged^ scene, 
			FbxAxisSystem^ dstAxis, 
			FbxSystemUnit^ dstUnit)
		{			
			return KFbxRootNodeUtility::InsertFbxRoot(scene->_Ref(),*dstAxis->_Ref(),*dstUnit->_Ref());
		}
		bool FbxRootNodeUtility::IsFbxRootNode(FbxNode^ node)
		{
			return KFbxRootNodeUtility::IsFbxRootNode(node->_Ref());
		}
	}
}