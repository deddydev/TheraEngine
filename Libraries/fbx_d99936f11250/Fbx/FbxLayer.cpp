#pragma once
#include "stdafx.h"
#include "FbxLayer.h"
#include "FbxSdkManager.h"
#include "FbxStream.h"
#include "FbxLayerElementNormal.h"
#include "FbxLayerElementMaterial.h"
#include "FbxLayerElementPolygonGroup.h"
#include "FbxLayerElementUV.h"
#include "FbxLayerElementVertexColor.h"
#include "FbxLayerElementSmoothing.h"
#include "FbxLayerElementUserData.h"
#include "FbxLayerElementVisibility.h"
#include "FbxLayerElementTexture.h"

namespace Skill
{
	namespace FbxSDK
	{						
		void FbxLayer::CollectManagedMemory()
		{
			_Normals = nullptr;
			_Materials = nullptr;
			_PolygonGroups = nullptr;
			_EmissiveUV = nullptr;
			_EmissiveFactorUV = nullptr;
			_AmbientUV = nullptr;
			_AmbientFactorUV = nullptr;
			_DiffuseUV = nullptr;
			_DiffuseFactorUV = nullptr;
			_SpecularFactorUV = nullptr;
			_SpecularUV = nullptr;
			_ShininessUV = nullptr;
			_NormalMapUV = nullptr;
			_BumpUV = nullptr;
			_TransparentUV = nullptr;
			_TransparencyFactorUV = nullptr;
			_ReflectionUV = nullptr;
			_ReflectionFactorUV = nullptr;
			_VertexColors = nullptr;
			_Smoothing = nullptr;
			_UserData = nullptr;
			_Visibility = nullptr;
			_EmissiveTextures = nullptr;
			_EmissiveFactorTextures = nullptr;
			_AmbientTextures = nullptr;
			_AmbientFactorTextures = nullptr;
			_DiffuseTextures = nullptr;
			_DiffuseFactorTextures = nullptr;
			_SpecularTextures = nullptr;
			_SpecularFactorTextures = nullptr;
			_ShininessTextures = nullptr;
			_NormalMapTextures = nullptr;
			_BumpTextures = nullptr;
			_TransparentTextures = nullptr;
			_TransparencyFactorTextures = nullptr;
			_ReflectionTextures = nullptr;
			_ReflectionFactorTextures = nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementNormal,GetNormals(),FbxLayerElementNormal,Normals);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetNormals,FbxLayerElementNormal,Normals);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementMaterial,GetMaterials(),FbxLayerElementMaterial,Materials);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetMaterials,FbxLayerElementMaterial,Materials);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementPolygonGroup,GetPolygonGroups(),FbxLayerElementPolygonGroup,PolygonGroups);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetPolygonGroups,FbxLayerElementPolygonGroup,PolygonGroups);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetEmissiveUV(),FbxLayerElementUV,EmissiveUV);		
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetEmissiveFactorUV(),FbxLayerElementUV,EmissiveFactorUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetAmbientUV(),FbxLayerElementUV,AmbientUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetAmbientFactorUV(),FbxLayerElementUV,AmbientFactorUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetDiffuseFactorUV(),FbxLayerElementUV,DiffuseFactorUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetDiffuseUV(),FbxLayerElementUV,DiffuseUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetSpecularFactorUV(),FbxLayerElementUV,SpecularFactorUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetSpecularUV(),FbxLayerElementUV,SpecularUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetShininessUV(),FbxLayerElementUV,ShininessUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetNormalMapUV(),FbxLayerElementUV,NormalMapUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetBumpUV(),FbxLayerElementUV,BumpUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetTransparentUV(),FbxLayerElementUV,TransparentUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetTransparencyFactorUV(),FbxLayerElementUV,TransparencyFactorUV);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetReflectionUV(),FbxLayerElementUV,ReflectionUV);		
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUV,GetReflectionFactorUV(),FbxLayerElementUV,ReflectionFactorUV);		

		FbxLayerElementUV^ FbxLayer::GetUVs(FbxLayerElement::LayerElementType typeIdentifier)
		{
			KFbxLayerElementUV* uv = _Ref()->GetUVs((KFbxLayerElement::ELayerElementType) typeIdentifier);
			if(uv)
				return gcnew FbxLayerElementUV(uv);
			return nullptr;
		}
		int FbxLayer::UVSetCount::get()
		{
			return _Ref()->GetUVSetCount();
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementVertexColor,GetVertexColors(),FbxLayerElementVertexColor,VertexColors);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetVertexColors,FbxLayerElementVertexColor,VertexColors);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementSmoothing,GetSmoothing(),FbxLayerElementSmoothing,Smoothing);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetSmoothing,FbxLayerElementSmoothing,Smoothing);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementUserData,GetUserData(),FbxLayerElementUserData,UserData);	
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetUserData,FbxLayerElementUserData,UserData);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementVisibility,GetVisibility(),FbxLayerElementVisibility,Visibility);			
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetVisibility,FbxLayerElementVisibility,Visibility);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetEmissiveTextures(),FbxLayerElementTexture,EmissiveTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetEmissiveTextures,FbxLayerElementTexture,EmissiveTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetEmissiveFactorTextures(),FbxLayerElementTexture,EmissiveFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetEmissiveFactorTextures,FbxLayerElementTexture,EmissiveFactorTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetAmbientTextures(),FbxLayerElementTexture,AmbientTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetAmbientTextures,FbxLayerElementTexture,AmbientTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetAmbientFactorTextures(),FbxLayerElementTexture,AmbientFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetAmbientFactorTextures,FbxLayerElementTexture,AmbientFactorTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetDiffuseTextures(),FbxLayerElementTexture,DiffuseTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetDiffuseTextures,FbxLayerElementTexture,DiffuseTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetDiffuseFactorTextures(),FbxLayerElementTexture,DiffuseFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetDiffuseFactorTextures,FbxLayerElementTexture,DiffuseFactorTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetSpecularTextures(),FbxLayerElementTexture,SpecularTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetSpecularTextures,FbxLayerElementTexture,SpecularTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetSpecularFactorTextures(),FbxLayerElementTexture,SpecularFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetSpecularFactorTextures,FbxLayerElementTexture,SpecularFactorTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetShininessTextures(),FbxLayerElementTexture,ShininessTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetShininessTextures,FbxLayerElementTexture,ShininessTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetNormalMapTextures(),FbxLayerElementTexture,NormalMapTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetNormalMapTextures,FbxLayerElementTexture,NormalMapTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetBumpTextures(),FbxLayerElementTexture,BumpTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetBumpTextures,FbxLayerElementTexture,BumpTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetTransparentTextures(),FbxLayerElementTexture,TransparentTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetTransparentTextures,FbxLayerElementTexture,TransparentTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetTransparencyFactorTextures(),FbxLayerElementTexture,TransparencyFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetTransparencyFactorTextures,FbxLayerElementTexture,TransparencyFactorTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetReflectionTextures(),FbxLayerElementTexture,ReflectionTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetReflectionTextures,FbxLayerElementTexture,ReflectionTextures);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxLayer,KFbxLayerElementTexture,GetReflectionFactorTextures(),FbxLayerElementTexture,ReflectionFactorTextures);
		REF_PROPERTY_SET_DEFINATION_BY_FUNC(FbxLayer,SetReflectionFactorTextures,FbxLayerElementTexture,ReflectionFactorTextures);


		FbxLayerElementTexture^ FbxLayer::GetTextures(FbxLayerElement::LayerElementType type)
		{
			KFbxLayerElementTexture* t = _Ref()->GetTextures((KFbxLayerElement::ELayerElementType)type);
			if(t)
				return gcnew FbxLayerElementTexture(t);
			return nullptr;
		}

		void FbxLayer::SetTextures(FbxLayerElement::LayerElementType type,FbxLayerElementTexture^ textures)
		{
			if(textures )
				_Ref()->SetTextures((KFbxLayerElement::ELayerElementType)type,textures->_Ref());
			else
				_Ref()->SetTextures((KFbxLayerElement::ELayerElementType)type,NULL);
		}

		FbxLayerElement^ FbxLayer::GetLayerElementOfType(FbxLayerElement::LayerElementType type, bool isUV)
		{
			KFbxLayerElement* e = _Ref()->GetLayerElementOfType((KFbxLayerElement::ELayerElementType)type,isUV);			
			return FbxCreator::CreateFbxLayerElement(e);			
		}
		void FbxLayer::SetUVs(FbxLayerElementUV^ UVs, FbxLayerElement::LayerElementType typeIdentifier)
		{
			if(UVs )
				_Ref()->SetUVs(UVs->_Ref(),(KFbxLayerElement::ELayerElementType)typeIdentifier);
			else
				_Ref()->SetUVs(NULL,(KFbxLayerElement::ELayerElementType)typeIdentifier);
		}

		FbxLayerElement^ FbxLayer::CreateLayerElementOfType(FbxLayerElement::LayerElementType type, bool isUV)
		{
			KFbxLayerElement* e = _Ref()->CreateLayerElementOfType((KFbxLayerElement::ELayerElementType)type,isUV);			
			return FbxCreator::CreateFbxLayerElement(e);			
		}

		void FbxLayer::Clone(FbxLayer^ srcLayer, FbxSdkManagerManaged^ sdkManager)
		{
			_Ref()->Clone(*srcLayer->_Ref(),sdkManager->_Ref());
		}

		bool FbxLayer::ContentWriteTo(FbxStream^ stream)
		{
			return _Ref()->ContentWriteTo(*stream->_Ref());
		}
		bool FbxLayer::ContentReadFrom(FbxStream^ stream)
		{
			return _Ref()->ContentReadFrom(*stream->_Ref());
		}				
		int FbxLayer::MemoryUsage::get()
		{
			return _Ref()->MemoryUsage();
		}
	}
}