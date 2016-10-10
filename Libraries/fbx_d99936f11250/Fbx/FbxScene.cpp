#pragma once
#include "stdafx.h"
#include "FbxScene.h"
#include "FbxSdkManager.h"
#include "FbxNode.h"
#include "FbxGenericNode.h"
#include "FbxCharacter.h"
#include "FbxCharacterPose.h"
#include "FbxControlSet.h"
#include "FbxPose.h"
#include "FbxSurfaceMaterial.h"
#include "FbxGlobalLightSettings.h"
#include "FbxGlobalCameraSettings.h"
#include "FbxGlobalTimeSettings.h"
#include "FbxGlobalSettings.h"
#include "FbxTexture.h"
#include "FbxGeometry.h"
#include "FbxVideo.h"
#include "FbxString.h"
#include "FbxDocumentInfo.h"
#include "FbxTakeNodeContainer.h"
#include "FbxClassId.h"


{
	namespace FbxSDK
	{	


		void FbxSceneManaged::CollectManagedMemory()
		{
			_GlobalCameraSettings = nullptr;
			_GlobalLightSettings = nullptr;
			_GlobalTimeSettings = nullptr;
			_GlobalSettings = nullptr;
			_RootNode = nullptr;
			_SceneInfo = nullptr;
			FbxDocumentManaged::CollectManagedMemory();
		}

		FBXOBJECT_DEFINITION(FbxSceneManaged,KFbxScene);

		void FbxSceneManaged::Clear()
		{
			_Ref()->Clear();
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSceneManaged,KFbxNode,GetRootNode(),FbxNode,RootNode);		
		int FbxSceneManaged::GenericNodeCount::get()
		{
			return _Ref()->GetGenericNodeCount();
		}			
		FbxGenericNode^ FbxSceneManaged::GetGenericNode(int index)
		{
			KFbxGenericNode* node = _Ref()->GetGenericNode(index);
			if(node)
				return gcnew FbxGenericNode(node);
			return nullptr;
		}
		FbxGenericNode^ FbxSceneManaged::GetGenericNode(String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			FbxGenericNode^ g = gcnew FbxGenericNode(_Ref()->GetGenericNode(n));
			FREECHARPOINTER(n);
			return g;
		}
		bool FbxSceneManaged::AddGenericNode(FbxGenericNode^ genericNode)
		{
			return _Ref()->AddGenericNode(genericNode->_Ref());
		}
		bool FbxSceneManaged::RemoveGenericNode(FbxGenericNode^ genericNode)
		{
			return _Ref()->RemoveGenericNode(genericNode->_Ref());
		}
		int FbxSceneManaged::CharacterCount::get()
		{
			return _Ref()->GetCharacterCount();
		}			
		FbxCharacter^ FbxSceneManaged::GetCharacter(int index)
		{
			return gcnew FbxCharacter(_Ref()->GetCharacter(index));
		}
		int FbxSceneManaged::GetCharacter(String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			int r = _Ref()->CreateCharacter(n);
			FREECHARPOINTER(n);
			return r;
		}
		void FbxSceneManaged::DestroyCharacter(int index)
		{
			_Ref()->DestroyCharacter(index);
		}
		int FbxSceneManaged::ControlSetPlugCount::get()
		{
			return _Ref()->GetControlSetPlugCount();
		}			
		FbxControlSetPlug^ FbxSceneManaged::GetControlSetPlug(int index)
		{
			return gcnew FbxControlSetPlug(_Ref()->GetControlSetPlug(index));
		}
		int FbxSceneManaged::CreateControlSetPlug(String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			int r =_Ref()->CreateControlSetPlug(n);
			FREECHARPOINTER(n);
			return r;
		}
		void FbxSceneManaged::DestroyControlSetPlug(int index)
		{
			_Ref()->DestroyControlSetPlug(index);
		}
		int FbxSceneManaged::CharacterPoseCount::get()
		{
			return _Ref()->GetCharacterPoseCount();
		}

		FbxCharacterPose^ FbxSceneManaged::GetCharacterPose(int index)
		{
			return gcnew FbxCharacterPose(_Ref()->GetCharacterPose(index));
		}
		int FbxSceneManaged::CreateCharacterPose(String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			int r = _Ref()->CreateCharacterPose(n);
			FREECHARPOINTER(n);
			return r;
		}
		void FbxSceneManaged::DestroyCharacterPose(int index)
		{
			_Ref()->DestroyCharacterPose(index);
		}
		int FbxSceneManaged::PoseCount::get()
		{
			return _Ref()->GetPoseCount();
		}			
		FbxPose^ FbxSceneManaged::GetPose(int index)
		{
			return gcnew FbxPose(_Ref()->GetPose(index));
		}
		bool FbxSceneManaged::AddPose(FbxPose^ pose)
		{
			return _Ref()->AddPose(pose->_Ref());
		}
		bool FbxSceneManaged::RemovePose(FbxPose^ pose)
		{
			return _Ref()->RemovePose(pose->_Ref());
		}
		bool FbxSceneManaged::RemovePose(int index)
		{
			return _Ref()->RemovePose(index);
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxSceneManaged,KFbxDocumentInfo,GetSceneInfo(),FbxDocumentInfo,SceneInfo);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxSceneManaged,SetSceneInfo,FbxDocumentInfo,SceneInfo);		

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSceneManaged,GetGlobalSettings(),FbxGlobalSettings,GlobalSettings);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSceneManaged,GetGlobalLightSettings(),FbxGlobalLightSettings,GlobalLightSettings);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSceneManaged,GetGlobalCameraSettings(),FbxGlobalCameraSettings,GlobalCameraSettings);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxSceneManaged,GetGlobalTimeSettings(),FbxGlobalTimeSettings,GlobalTimeSettings);

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		void FbxSceneManaged::ConnectMaterials()
		{
			_Ref()->ConnectMaterials();
		}

		void FbxSceneManaged::BuildMaterialLayersDirectArray()
		{
			_Ref()->BuildMaterialLayersDirectArray();
		}

		void FbxSceneManaged::ReindexMaterialConnections()
		{
			_Ref()->ReindexMaterialConnections();
		}
		CLONE_DEFINITION(FbxSceneManaged,KFbxScene);
		int FbxSceneManaged::MaterialCount::get()
		{
			return _Ref()->GetMaterialCount();
		}

		FbxSurfaceMaterial^ FbxSceneManaged::GetMaterial (int index)
		{
			return FbxCreator::CreateFbxSurfaceMaterial(_Ref()->GetMaterial (index));
		}
		FbxSurfaceMaterial^ FbxSceneManaged::GetMaterial (String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			FbxSurfaceMaterial^ sm= FbxCreator::CreateFbxSurfaceMaterial(_Ref()->GetMaterial(n));
			FREECHARPOINTER(n);
			return sm;

		}
		bool FbxSceneManaged::AddMaterial (FbxSurfaceMaterial^ material)
		{
			return _Ref()->AddMaterial(material->_Ref());
		}
		bool FbxSceneManaged::RemoveMaterial (FbxSurfaceMaterial^ material)
		{
			return _Ref()->RemoveMaterial(material->_Ref());
		}			
		int FbxSceneManaged::TextureCount::get()
		{
			return _Ref()->GetTextureCount();
		}			
		FbxTexture^ FbxSceneManaged::GetTexture(int index)
		{
			return FbxCreator::CreateFbxTexture(_Ref()->GetTexture(index));
		}
		FbxTexture^ FbxSceneManaged::GetTexture(String^ name)
		{			
			STRINGTOCHAR_ANSI(n,name);
			FbxTexture^ t = FbxCreator::CreateFbxTexture(_Ref()->GetTexture(n));
			FREECHARPOINTER(n);
			return t;
		}
		bool FbxSceneManaged::AddTexture (FbxTexture^texture)
		{
			return _Ref()->AddTexture(texture->_Ref());
		}
		bool FbxSceneManaged::RemoveTexture(FbxTexture^ texture)
		{
			return _Ref()->RemoveTexture(texture->_Ref());
		}			
		int FbxSceneManaged::NodeCount::get()
		{
			return _Ref()->GetNodeCount();
		}			
		FbxNode^ FbxSceneManaged::GetNode (int index)
		{
			return gcnew FbxNode(_Ref()->GetNode(index));
		}			
		bool FbxSceneManaged::AddNode (FbxNode^node)
		{
			return _Ref()->AddNode(node->_Ref());
		}
		bool FbxSceneManaged::RemoveNode (FbxNode^ node)
		{
			return _Ref()->RemoveNode(node->_Ref());
		}			
		int FbxSceneManaged::CurveOnSurfaceCount::get()
		{
			return _Ref()->GetCurveOnSurfaceCount();
		}			
		FbxNode^ FbxSceneManaged::FindNodeByName (FbxString^ name)
		{
			KFbxNode* node =_Ref()->FindNodeByName(*name->_Ref());
			if(node)
			{
				return gcnew FbxNode(node);
			}
			return nullptr;
		}			
		int FbxSceneManaged::GeometryCount::get()
		{
			return _Ref()->GetGeometryCount();
		}			
		FbxGeometry^ FbxSceneManaged::GetGeometry(int index)
		{
			KFbxGeometry* g = _Ref()->GetGeometry(index);
			if(g)
				return FbxCreator::CreateFbxGeometry(g);
			return nullptr;
		}
		bool FbxSceneManaged::AddGeometry(FbxGeometry^ geometry)
		{
			return _Ref()->AddGeometry(geometry->_Ref());
		}
		bool FbxSceneManaged::RemoveGeometry(FbxGeometry^ geometry)
		{
			return _Ref()->RemoveGeometry(geometry->_Ref());
		}
		int FbxSceneManaged::VideoCount::get()
		{
			return _Ref()->GetVideoCount();
		}

		FbxVideo^ FbxSceneManaged::GetVideo (int index)
		{
			return gcnew FbxVideo(_Ref()->GetVideo(index));
		}
		bool FbxSceneManaged::AddVideo (FbxVideo^ video)
		{
			return _Ref()->AddVideo(video->_Ref());
		}
		bool FbxSceneManaged::RemoveVideo(FbxVideo^ video)
		{
			return _Ref()->RemoveVideo(video->_Ref());
		}
		int FbxSceneManaged::TakeNodeContainerCount::get()
		{
			return _Ref()->GetTakeNodeContainerCount();
		}			
		FbxTakeNodeContainer^  FbxSceneManaged::GetTakeNodeContainer(int index)
		{
			KFbxTakeNodeContainer* n = _Ref()->GetTakeNodeContainer(index);
			if(n)
				return FbxCreator::CreateFbxTakeNodeContainer(n);
			return nullptr;
		}
		bool FbxSceneManaged::AddTakeNodeContainer(FbxTakeNodeContainer^takeNodeContainer)
		{
			return _Ref()->AddTakeNodeContainer(takeNodeContainer->_Ref());
		}
		bool FbxSceneManaged::RemoveTakeNodeContainer(FbxTakeNodeContainer^ takeNodeContainer)
		{
			return _Ref()->RemoveTakeNodeContainer(takeNodeContainer->_Ref());
		}
		void FbxSceneManaged::ConvertNurbsSurfaceToNurb()
		{
			_Ref()->ConvertNurbsSurfaceToNurb();
		}
		void FbxSceneManaged::ConvertMeshNormals()
		{
			_Ref()->ConvertMeshNormals();
		}
		void FbxSceneManaged::ConvertNurbCurvesToNulls()
		{
			_Ref()->ConvertNurbCurvesToNulls();
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS		

	}
}