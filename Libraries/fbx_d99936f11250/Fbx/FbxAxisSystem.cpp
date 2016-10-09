#pragma once
#include "stdafx.h"
#include "FbxAxisSystem.h"
#include "FbxScene.h"
#include "FbxNode.h"

namespace Skill
{
	namespace FbxSDK
	{		

		void FbxAxisSystem::CollectManagedMemory()
		{
		}		

		FbxAxisSystem::FbxAxisSystem(UpVector upVector, FrontVector frontVector, CoordinateSystem coorSystem)
		{
			_SetPointer(new KFbxAxisSystem((KFbxAxisSystem::eUpVector)upVector,
				(KFbxAxisSystem::eFrontVector)frontVector,(KFbxAxisSystem::eCoorSystem)coorSystem),true);			
		}
		FbxAxisSystem::FbxAxisSystem(FbxAxisSystem^ axisSystem)
		{
			_SetPointer(new KFbxAxisSystem(*axisSystem->_Ref()),true);			
		}
		FbxAxisSystem::FbxAxisSystem(PreDefinedAxisSystem axisSystem)
		{
			_SetPointer(new KFbxAxisSystem((KFbxAxisSystem::ePreDefinedAxisSystem)axisSystem),true);			
		}		

		void FbxAxisSystem::CopyFrom(FbxAxisSystem^ axisSystem)
		{
			*_Ref()= *axisSystem->_Ref();
		}

		void FbxAxisSystem::ConvertScene(FbxSceneManaged^ scene)
		{
			_Ref()->ConvertScene(scene->_Ref());
		}
		void FbxAxisSystem::ConvertScene(FbxSceneManaged^ scene, FbxNode^ fbxRoot)
		{
			_Ref()->ConvertScene(scene->_Ref(),fbxRoot->_Ref());
		}
		FbxAxisSystem::UpVector FbxAxisSystem::GetUpVector(int %sign)
		{
			int s;
			UpVector u = (UpVector)_Ref()->GetUpVector(s);
			sign = s;
			return u;
		}

		FbxAxisSystem::CoordinateSystem FbxAxisSystem::Coordinate_System ::get()
		{
			return (CoordinateSystem)_Ref()->GetCoorSystem();
		}			
		void FbxAxisSystem::ConvertChildren(FbxNode^ root, FbxAxisSystem^ srcSystem)
		{
			_Ref()->ConvertChildren(root->_Ref(),*srcSystem->_Ref());
		}
	}
}