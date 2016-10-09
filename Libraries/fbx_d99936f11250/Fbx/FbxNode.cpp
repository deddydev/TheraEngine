#pragma once
#include "stdafx.h"
#include "FbxNode.h"
#include "FbxDouble3.h"
#include "FbxTypedProperty.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"
#include "FbxVector4.h"
#include "FbxNodeAttribute.h"
#include "FbxNull.h"		
#include "FbxMarker.h"		
#include "FbxSkeleton.h"		
#include "FbxCharacter.h"		
#include "FbxGeometry.h"	
#include "FbxMesh.h"		
#include "FbxNurb.h"		
#include "FbxNurbsSurface.h"		
#include "FbxNurbsCurve.h"		
#include "FbxBoundary.h"		
#include "FbxPatch.h"		
#include "FbxShape.h"		
#include "FbxCamera.h"		
#include "FbxCameraSwitcher.h"		
#include "FbxLight.h"		
#include "FbxOpticalReference.h"
#include "FbxXMatrix.h"
#include "FbxTime.h"
#include "FbxSurfaceMaterial.h"
#include "FbxSurfaceLambert.h"
#include "FbxSurfacePhong.h"
#include "FbxError.h"
#include "FbxLimits.h"

#define FBXNODE_PROP_GETSET_DEFINE(Type,Name)Type FbxNode::Name::get(){	return _Ref()->Name.Get();}\
	void FbxNode::Name::set(Type value){_Ref()->Name.Set(value);}\

namespace Skill
{
	namespace FbxSDK
	{

		void FbxNode::CollectManagedMemory()
		{
			this->_AimVector = nullptr;
			this->_GeometricRotation = nullptr;
			this->_GeometricScaling = nullptr;
			this->_GeometricTranslation = nullptr;
			this->_GlobalState = nullptr;
			this->_GlobalTransform = nullptr;
			this->_KError = nullptr;
			this->_LclRotation = nullptr;
			this->_LclScaling = nullptr;
			this->_LclTranslation = nullptr;
			this->_Limits = nullptr;
			this->_MaxDampRange = nullptr;
			this->_MaxDampStrength = nullptr;
			this->_MinDampRange = nullptr;
			this->_MinDampStrength = nullptr;
			this->_NodeAttribute = nullptr;
			this->_Parent = nullptr;
			this->_PoleVector = nullptr;
			this->_PostRotation = nullptr;
			this->_PostTargetRotation = nullptr;
			this->_PreferedAngle = nullptr;
			this->_PreRotation = nullptr;
			this->_RotationMax = nullptr;
			this->_RotationMin = nullptr;
			this->_RotationOffset = nullptr;
			this->_RotationPivot = nullptr;
			this->_RotationStiffness = nullptr;
			this->_Scaling = nullptr;
			this->_ScalingMax = nullptr;
			this->_ScalingMin = nullptr;
			this->_ScalingOffset = nullptr;
			this->_ScalingPivot = nullptr;
			this->_Target = nullptr;
			this->_TargetUp = nullptr;
			this->_TargetUpVector = nullptr;
			this->_Translation = nullptr;
			this->_TranslationMax = nullptr;
			this->_TranslationMin = nullptr;
			this->_UpVector = nullptr;
			this->_WorldUpVector = nullptr;

			this->_Null= nullptr;
			this->_Marker= nullptr;
			this->_Skeleton= nullptr;
			this->_Geometry= nullptr;
			this->_Mesh= nullptr;
			this->_Nurb= nullptr;
			this->_NurbsSurface= nullptr;
			this->_NurbsCurve= nullptr;
			this->_TrimNurbsSurface= nullptr;
			this->_Patch= nullptr;
			this->_Camera= nullptr;
			this->_CameraSwitcher= nullptr;
			this->_Light= nullptr;
			this->_OpticalReference= nullptr;
			FbxTakeNodeContainer::CollectManagedMemory();
		}		
		FBXOBJECT_DEFINITION(FbxNode,KFbxNode);

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNode,GetParent(),FbxNode,Parent);		
		
		bool FbxNode::AddChild(FbxNode^ node)
		{
			return _Ref()->AddChild(node->_Ref());
		}

		bool FbxNode::RemoveChild(FbxNode^ node)
		{
			KFbxNode* n = _Ref()->RemoveChild(node->_Ref());
			if(n)
				return true;
			else
				return false;
		}
		int FbxNode::GetChildCount(bool recursive)
		{
			return _Ref()->GetChildCount(recursive);
		}
		FbxNode^ FbxNode::GetChild(int index)
		{
			KFbxNode* node = _Ref()->GetChild(index);
			if(node)
				return gcnew FbxNode(node);
			else
				return nullptr;
		}
		FbxNode^ FbxNode::FindChild(String^ name, bool recursive, bool initial)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			KFbxNode* node = _Ref()->FindChild(n,recursive,initial);
			FREECHARPOINTER(n);
			if(node)
				return gcnew FbxNode(node);
			else
				return nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNode,GetTarget(),FbxNode,Target);		
		void FbxNode::Target::set(FbxNode^ value)
		{
			if(value)
			{
				_Target = value;
				_Ref()->SetTarget(_Target->_Ref());
			}
		}

		bool FbxNode::Visible::get()
		{
			return _Ref()->GetVisibility();
		}
		void FbxNode::Visible::set(bool value)
		{
			_Ref()->SetVisibility(value);
		}		

		FbxNodeAttribute^ FbxNode::NodeAttribute::get()
		{
			KFbxNodeAttribute* k = _Ref()->GetNodeAttribute();
			if(k)
			{
				if(_NodeAttribute)
					_NodeAttribute->_SetPointer(k,false);
				else
				{
					FbxNodeAttribute::AttributeType type = (FbxNodeAttribute::AttributeType)k->GetAttributeType();
					switch(type)
					{
					case FbxNodeAttribute::AttributeType::Boundary:
						_NodeAttribute = gcnew FbxBoundary((KFbxBoundary*)k);
						break;
					case FbxNodeAttribute::AttributeType::Camera:
						_NodeAttribute = gcnew FbxCamera((KFbxCamera*)k);
						break;
					case FbxNodeAttribute::AttributeType::CameraSwitcher:
						_NodeAttribute = gcnew FbxCameraSwitcher((KFbxCameraSwitcher*)k);
						break;
					case FbxNodeAttribute::AttributeType::Constraint:
						//_NodeAttribute = FbxCreator::CreateFbxConstraint((KFbxConstraint*)k);
						break;
					case FbxNodeAttribute::AttributeType::Light:
						_NodeAttribute = gcnew FbxLight((KFbxLight*)k);
						break;
					case FbxNodeAttribute::AttributeType::Marker:
						_NodeAttribute = gcnew FbxMarker((KFbxMarker*)k);
						break;
					case FbxNodeAttribute::AttributeType::Mesh:
						_NodeAttribute = gcnew FbxMesh((KFbxMesh*)k);
						break;
					case FbxNodeAttribute::AttributeType::Null:
						_NodeAttribute = gcnew FbxNull((KFbxNull*)k);
						break;
					case FbxNodeAttribute::AttributeType::Nurb:
						_NodeAttribute = gcnew FbxNurb((KFbxNurb*)k);
						break;
					case FbxNodeAttribute::AttributeType::NurbsCurve:
						_NodeAttribute = gcnew FbxNurbsCurve((KFbxNurbsCurve*)k);
						break;
					case FbxNodeAttribute::AttributeType::NurbsSurface:
						_NodeAttribute = gcnew FbxNurbsSurface((KFbxNurbsSurface*)k);
						break;
					case FbxNodeAttribute::AttributeType::OpticalMarker:
						
						break;
					case FbxNodeAttribute::AttributeType::OpticalReference:
						_NodeAttribute = gcnew FbxOpticalReference((KFbxOpticalReference*)k);
						break;
					case FbxNodeAttribute::AttributeType::Patch:
						_NodeAttribute = gcnew FbxPatch((KFbxPatch*)k);
						break;
					case FbxNodeAttribute::AttributeType::Shape:
						_NodeAttribute = gcnew FbxShape((KFbxShape*)k);
						break;
					case FbxNodeAttribute::AttributeType::Skeleton:
						_NodeAttribute = gcnew FbxSkeleton((KFbxSkeleton*)k);
						break;
					case FbxNodeAttribute::AttributeType::TrimNurbsSurface:
						_NodeAttribute = gcnew FbxTrimNurbsSurface((KFbxTrimNurbsSurface*)k);
						break;
					case FbxNodeAttribute::AttributeType::Unidentified:
						_NodeAttribute = nullptr;
						break;
					}					
				}
				return _NodeAttribute;
			}
			return nullptr;
		}
		void FbxNode::NodeAttribute::set(FbxNodeAttribute^ value)
		{
			if(value)
			{
				_NodeAttribute = value;
				_Ref()->SetNodeAttribute(_NodeAttribute->_Ref());
			}
			else
			{
				_NodeAttribute = nullptr;
				_Ref()->SetNodeAttribute(NULL);
			}
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GetPostTargetRotation(),FbxVector4,PostTargetRotation);		
		void FbxNode::PostTargetRotation::set(FbxVector4^ value)
		{
			if(value )
			{				
				_Ref()->SetPostTargetRotation(*value->_Ref());
			}
		}
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNode,GetTargetUp(),FbxNode,TargetUp);		
		void FbxNode::TargetUp::set(FbxNode^ value)
		{
			_TargetUp = value;
			if(_TargetUp )							
				_Ref()->SetTargetUp(_TargetUp->_Ref());			
			else			
				_Ref()->SetTargetUp(NULL);			
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GetTargetUpVector(),FbxVector4,TargetUpVector);		
		void FbxNode::TargetUpVector::set(FbxVector4^ value)
		{
			if(value )
			{				
				_Ref()->SetTargetUpVector(*value->_Ref());
			}
		}
		FbxNode::ShadingMode FbxNode::Shading_Mode::get()
		{
			return (FbxNode::ShadingMode)_Ref()->GetShadingMode();
		}
		void FbxNode::Shading_Mode::set(FbxNode::ShadingMode value)
		{
			_Ref()->SetShadingMode((KFbxNode::EShadingMode)value);
		}			
		bool FbxNode::MultiLayer::get()		
		{
			return _Ref()->GetMultiLayer();
		}
		void FbxNode::MultiLayer::set(bool value)
		{
			_Ref()->SetMultiLayer(value);
		}
		FbxNode::MultiTakeMode FbxNode::MultiTake_Mode::get()
		{
			return (FbxNode::MultiTakeMode)_Ref()->GetMultiTakeMode();
		}
		void FbxNode::MultiTake_Mode::set(MultiTakeMode value)
		{
			_Ref()->SetMultiTakeMode((KFbxNode::EMultiTakeMode)value);
		}
		int FbxNode::Attribute_Count::get()
		{
			return _Ref()->GetNodeAttributeCount();
		}		

		int FbxNode::DefaultNodeAttributeIndex::get()
		{
			return _Ref()->GetDefaultNodeAttributeIndex();
		}
		bool FbxNode::SetDefaultNodeAttributeIndex(int index)
		{
			return _Ref()->SetDefaultNodeAttributeIndex(index);
		}

		FbxNodeAttribute^ FbxNode::GetNodeAttributeByIndex(int index)
		{
			KFbxNodeAttribute* na = _Ref()->GetNodeAttributeByIndex(index);			
			return FbxCreator::CreateFbxNodeAttribute(na);						
		}

		int FbxNode::GetNodeAttributeIndex(FbxNodeAttribute^ nodeAttribute)
		{
			return _Ref()->GetNodeAttributeIndex(nodeAttribute->_Ref());
		}

		bool FbxNode::AddNodeAttribute(FbxNodeAttribute^ nodeAttribute)
		{
			return _Ref()->AddNodeAttribute(nodeAttribute->_Ref());
		}
		bool FbxNode::RemoveNodeAttribute(FbxNodeAttribute^ nodeAttribute)
		{
			if(_Ref()->RemoveNodeAttribute(nodeAttribute->_Ref()))
				return true;
			else
				return false;
		}
		FbxNodeAttribute^ FbxNode::RemoveNodeAttributeByIndex(int index)
		{
			KFbxNodeAttribute* na = _Ref()->RemoveNodeAttributeByIndex(index);			
			return FbxCreator::CreateFbxNodeAttribute(na);						
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNull,GetNull(),FbxNull,Null);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxMarker,GetMarker(),FbxMarker,Marker);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxSkeleton,GetSkeleton(),FbxSkeleton,Skeleton);
		REF_PROPERTY_GET_DEFINATION_FROM_REF_BY_FBXCREATOR(FbxNode,KFbxGeometry,GetGeometry(),FbxGeometry,Geometry);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxMesh,GetMesh(),FbxMesh,Mesh);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNurb,GetNurb(),FbxNurb,Nurb);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNurbsSurface,GetNurbsSurface(),FbxNurbsSurface,NurbsSurface);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxNurbsCurve,GetNurbsCurve(),FbxNurbsCurve,NurbsCurve);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxTrimNurbsSurface,GetTrimNurbsSurface(),FbxTrimNurbsSurface,TrimNurbsSurface);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxPatch,GetPatch(),FbxPatch,Patch);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxCamera,GetCamera(),FbxCamera,Camera);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxCameraSwitcher,GetCameraSwitcher(),FbxCameraSwitcher,CameraSwitcher);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxLight,GetLight(),FbxLight,Light);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxNode,KFbxOpticalReference,GetOpticalReference(),FbxOpticalReference,OpticalReference);
		

		void FbxNode::GetDefaultT(FbxVector4^ t)
		{
			_Ref()->GetDefaultT(*t->_Ref());
		}
		void FbxNode::SetDefaultT(FbxVector4^ t)
		{
			_Ref()->SetDefaultT(*t->_Ref());
		}

		void FbxNode::GetDefaultR(FbxVector4^ r)
		{
			_Ref()->GetDefaultR(*r->_Ref());
		}
		void FbxNode::SetDefaultR(FbxVector4^ r)
		{
			_Ref()->SetDefaultR(*r->_Ref());
		}

		void FbxNode::GetDefaultS(FbxVector4^ s)
		{
			_Ref()->GetDefaultS(*s->_Ref());
		}
		void FbxNode::SetDefaultS(FbxVector4^ s)
		{
			_Ref()->SetDefaultS(*s->_Ref());
		}

		double FbxNode::DefaultVisibility::get()
		{
			return _Ref()->GetDefaultVisibility();
		}
		void FbxNode::DefaultVisibility::set(double value)
		{
			return _Ref()->SetDefaultVisibility(value);
		}

		FbxTransformInheritType FbxNode::TransformationInheritType::get()
		{
			ETransformInheritType t;
			_Ref()->GetTransformationInheritType(t);
			return (FbxTransformInheritType)t;
		}
		void FbxNode::TransformationInheritType::set(FbxTransformInheritType value)
		{
			_Ref()->SetTransformationInheritType((ETransformInheritType)value);
		}

		void FbxNode::SetPivotState(PivotSet pivotSet,PivotState pivotState)
		{
			_Ref()->SetPivotState((KFbxNode::EPivotSet)pivotSet,(KFbxNode::EPivotState)pivotState);
		}
		void FbxNode::GetPivotState(PivotSet pivotSet, PivotState %pivotState)
		{
			KFbxNode::EPivotState e;
			_Ref()->GetPivotState((KFbxNode::EPivotSet)pivotSet,e);
			pivotState = (PivotState)e;
		}
		void FbxNode::SetRotationOrder(PivotSet pivotSet, FbxRotationOrder rotationOrder)
		{
			_Ref()->SetRotationOrder((KFbxNode::EPivotSet)pivotSet,(ERotationOrder)rotationOrder);
		}
		void FbxNode::GetRotationOrder(PivotSet pivotSet, FbxRotationOrder %rotationOrder)
		{
			ERotationOrder e;
			_Ref()->GetRotationOrder((KFbxNode::EPivotSet)pivotSet,e);
			rotationOrder = (FbxRotationOrder)e;
		}
		void FbxNode::SetUseRotationSpaceForLimitOnly(PivotSet pivotSet, bool useForLimitOnly)
		{
			_Ref()->SetUseRotationSpaceForLimitOnly((KFbxNode::EPivotSet)pivotSet,useForLimitOnly);
		}
		bool FbxNode::GetUseRotationSpaceForLimitOnly(PivotSet pivotSet)
		{
			return _Ref()->GetUseRotationSpaceForLimitOnly((KFbxNode::EPivotSet)pivotSet);
		}

		bool FbxNode::RotationActive::get()
		{
			return _Ref()->GetRotationActive();
		}
		void FbxNode::RotationActive::set(bool value)
		{
			_Ref()->SetRotationActive(value);
		}		
		void FbxNode::SetUseQuaternionForInterpolation(PivotSet pivotSet, bool useQuaternion)
		{
			_Ref()->SetUseRotationSpaceForLimitOnly((KFbxNode::EPivotSet)pivotSet,useQuaternion);
		}
		bool FbxNode::GetUseQuaternionForInterpolation(PivotSet pivotSet)
		{
			return _Ref()->GetUseQuaternionForInterpolation((KFbxNode::EPivotSet)pivotSet);
		}
		FbxVector4^ FbxNode::RotationStiffness::get()
		{
			return gcnew FbxVector4(_Ref()->GetRotationStiffness());
		}
		void FbxNode::RotationStiffness::set(FbxVector4^ value)
		{
			_Ref()->SetRotationStiffness(*value->_Ref());
		}		
		FbxVector4^ FbxNode::MinDampRange::get()
		{
			return gcnew FbxVector4(_Ref()->GetMinDampRange());
		}
		void FbxNode::MinDampRange::set(FbxVector4^ value)
		{
			_Ref()->SetMinDampRange(*value->_Ref());
		}		
		FbxVector4^ FbxNode::MaxDampRange::get()
		{
			return gcnew FbxVector4(_Ref()->GetMaxDampRange());
		}
		void FbxNode::MaxDampRange::set(FbxVector4^ value)
		{
			_Ref()->SetMaxDampRange(*value->_Ref());
		}

		FbxVector4^ FbxNode::MinDampStrength::get()
		{
			return gcnew FbxVector4(_Ref()->GetMinDampStrength());
		}
		void FbxNode::MinDampStrength::set(FbxVector4^ value)
		{
			_Ref()->SetMinDampStrength(*value->_Ref());
		}		
		FbxVector4^ FbxNode::MaxDampStrength::get()
		{
			return gcnew FbxVector4(_Ref()->GetMaxDampStrength());
		}
		void FbxNode::MaxDampStrength::set(FbxVector4^ value)
		{
			_Ref()->SetMaxDampStrength(*value->_Ref());
		}

		FbxVector4^ FbxNode::PreferedAngle::get()
		{
			return gcnew FbxVector4(_Ref()->GetPreferedAngle());
		}
		void FbxNode::PreferedAngle::set(FbxVector4^ value)
		{
			_Ref()->SetPreferedAngle(*value->_Ref());
		}

		void FbxNode::SetRotationOffset(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetRotationOffset((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}

		FbxVector4^ FbxNode::GetRotationOffset(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetRotationOffset((KFbxNode::EPivotSet)pivotSet));
		}
		void FbxNode::SetRotationPivot(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetRotationPivot((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetRotationPivot(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetRotationPivot((KFbxNode::EPivotSet)pivotSet));
		}
		void FbxNode::SetPreRotation(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetPreRotation((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetPreRotation(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetPreRotation((KFbxNode::EPivotSet)pivotSet));
		}
		void FbxNode::SetPostRotation(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetPostRotation((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetPostRotation(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetPostRotation((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::SetScalingOffset(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetScalingOffset((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetScalingOffset(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetScalingOffset((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::SetScalingPivot(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetScalingPivot((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetScalingPivot(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetScalingPivot((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::SetGeometricTranslation(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetGeometricTranslation((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetGeometricTranslation(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetGeometricTranslation((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::SetGeometricRotation(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetGeometricRotation((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetGeometricRotation(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetGeometricRotation((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::SetGeometricScaling(PivotSet pivotSet, FbxVector4^ vector)
		{
			_Ref()->SetGeometricScaling((KFbxNode::EPivotSet)pivotSet,*vector->_Ref());
		}
		FbxVector4^ FbxNode::GetGeometricScaling(PivotSet pivotSet)
		{
			return gcnew FbxVector4(_Ref()->GetGeometricScaling((KFbxNode::EPivotSet)pivotSet));
		}

		void FbxNode::ConvertPivotAnimation(PivotSet conversionTarget, double frameRate, bool keyReduce)
		{
			_Ref()->ConvertPivotAnimation((KFbxNode::EPivotSet)conversionTarget,frameRate,keyReduce);
		}
		void FbxNode::ConvertPivotAnimationRecursive(PivotSet conversionTarget, double frameRate, bool keyReduce)
		{
			_Ref()->ConvertPivotAnimationRecursive((KFbxNode::EPivotSet)conversionTarget,frameRate,keyReduce);
		}

		void FbxNode::ResetPivotSet(PivotSet pivotSet)
		{
			_Ref()->ResetPivotSet((KFbxNode::EPivotSet)pivotSet);
		}

		void FbxNode::ResetPivotSetAndConvertAnimation( double frameRate, bool keyReduce)
		{
			_Ref()->ResetPivotSetAndConvertAnimation(frameRate,keyReduce);
		}

		FbxVector4^ FbxNode::GetLocalTFromDefaultTake(bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalTFromDefaultTake(applyLimits));
		}

		FbxVector4^ FbxNode::GetLocalRFromDefaultTake(bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalRFromDefaultTake(applyLimits));
		}		
		FbxVector4^ FbxNode::GetLocalSFromDefaultTake(bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalSFromDefaultTake(applyLimits));
		}

		FbxXMatrix^ FbxNode::GetGlobalFromDefaultTake(PivotSet pivotSet, bool applyTarget)
		{
			return gcnew FbxXMatrix(_Ref()->GetGlobalFromDefaultTake((KFbxNode::EPivotSet)pivotSet,applyTarget));
		}

		FbxVector4^ FbxNode::GetLocalTFromCurrentTake(FbxTime^ time, bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalTFromCurrentTake(*time->_Ref(),applyLimits));
		}

		FbxVector4^ FbxNode::GetLocalRFromCurrentTake(FbxTime^ time, bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalRFromCurrentTake(*time->_Ref(),applyLimits));
		}

		FbxVector4^ FbxNode::GetLocalSFromCurrentTake(FbxTime^ time, bool applyLimits)
		{
			return gcnew FbxVector4(_Ref()->GetLocalSFromCurrentTake(*time->_Ref(),applyLimits));
		}

		FbxXMatrix^ FbxNode::GetGlobalFromCurrentTake(FbxTime^ time, PivotSet pivotSet, bool applyTarget)
		{
			return gcnew FbxXMatrix(_Ref()->GetGlobalFromCurrentTake(*time->_Ref(),(KFbxNode::EPivotSet)pivotSet,applyTarget));
		}
		int FbxNode::CharacterLinkCount::get()
		{
			return _Ref()->GetCharacterLinkCount();
		}

		int FbxNode::FindCharacterLink(FbxCharacter^ character, int characterLinkType, int nodeId, int nodeSubId)
		{
			return _Ref()->FindCharacterLink(character->_Ref(),characterLinkType, nodeId,nodeSubId);
		}		

		int FbxNode::AddMaterial(FbxSurfaceMaterial^ material)
		{
			return _Ref()->AddMaterial(material->_Ref());
		}		 
		bool FbxNode::RemoveMaterial(FbxSurfaceMaterial^ material)
		{
			return _Ref()->RemoveMaterial(material->_Ref());
		}

		int FbxNode::MaterialCount::get()
		{
			return _Ref()->GetMaterialCount();
		}
		FbxSurfaceMaterial^ FbxNode::GetMaterial( int index )
		{
			KFbxSurfaceMaterial* m = _Ref()->GetMaterial(index);
			if(m)
			{
				return FbxCreator::CreateFbxSurfaceMaterial(m);
			}
			return nullptr;
		}
		void FbxNode::RemoveAllMaterials()
		{
			_Ref()->RemoveAllMaterials();
		}

		int FbxNode::GetMaterialIndex(String^ name)
		{
			STRINGTO_CONSTCHAR_ANSI(n,name);
			int i = _Ref()->GetMaterialIndex(n);
			FREECHARPOINTER(n);
			return i;
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GetError(),FbxErrorManaged,KError);		
		FbxNode::Error FbxNode::LastErrorID::get()
		{
			return (FbxNode::Error)_Ref()->GetLastErrorID();
		}		
		String^ FbxNode::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}	

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,LclTranslation,FbxDouble3TypedProperty,LclTranslation);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,LclRotation,FbxDouble3TypedProperty,LclRotation);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,LclScaling,FbxDouble3TypedProperty,LclScaling);		


		FbxXMatrix^ FbxNode::GlobalTransform::get()
		{
			if(_GlobalTransform )
				*_GlobalTransform->_FbxXMatrix = _Ref()->GlobalTransform.Get();
			else
				_GlobalTransform = gcnew FbxXMatrix(_Ref()->GlobalTransform.Get());
			return _GlobalTransform;
		}
		void FbxNode::GlobalTransform::set(FbxXMatrix^ value)
		{
			if(value)
			{
				*_GlobalTransform->_FbxXMatrix = *value->_Ref();
				_Ref()->GlobalTransform.Set(*value->_Ref());
			}
		}

		FBXNODE_PROP_GETSET_DEFINE(double,Visibility);
		FBXNODE_PROP_GETSET_DEFINE(double,Weight);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,PoleVector,FbxDouble3TypedProperty,PoleVector);
		FBXNODE_PROP_GETSET_DEFINE(double,Twist);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,WorldUpVector,FbxDouble3TypedProperty,WorldUpVector);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,UpVector,FbxDouble3TypedProperty,UpVector);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,AimVector,FbxDouble3TypedProperty,AimVector);		
		FBXNODE_PROP_GETSET_DEFINE(bool,QuaternionInterpolate);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,RotationOffset,FbxDouble3TypedProperty,RotationOffset);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,RotationPivot,FbxDouble3TypedProperty,RotationPivot);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,ScalingOffset,FbxDouble3TypedProperty,ScalingOffset);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,ScalingPivot,FbxDouble3TypedProperty,ScalingPivot);		
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationActive);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,Translation,FbxDouble3TypedProperty,Translation);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,TranslationMin,FbxDouble3TypedProperty,TranslationMin);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,TranslationMax,FbxDouble3TypedProperty,TranslationMax);		
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMinX);
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMinY);
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMinZ);
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMaxX);
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMaxY);
		FBXNODE_PROP_GETSET_DEFINE(bool,TranslationMaxZ);


		FbxRotationOrder FbxNode::RotationOrder::get()
		{
			return (FbxRotationOrder)_Ref()->RotationOrder.Get();			
		}
		void FbxNode::RotationOrder::set(FbxRotationOrder value)
		{
			_Ref()->RotationOrder.Set((ERotationOrder)value);			
		}

		FBXNODE_PROP_GETSET_DEFINE(bool,RotationSpaceForLimitOnly);
		FBXNODE_PROP_GETSET_DEFINE(double,RotationStiffnessX);
		FBXNODE_PROP_GETSET_DEFINE(double,RotationStiffnessY);
		FBXNODE_PROP_GETSET_DEFINE(double,RotationStiffnessZ);
		FBXNODE_PROP_GETSET_DEFINE(double,AxisLen);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,PreRotation,FbxDouble3TypedProperty,PreRotation);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,PostRotation,FbxDouble3TypedProperty,PostRotation);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,RotationMin,FbxDouble3TypedProperty,RotationMin);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,RotationMax,FbxDouble3TypedProperty,RotationMax);		
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMinX);
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMinY);
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMinZ);			
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMaxX);
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMaxY);
		FBXNODE_PROP_GETSET_DEFINE(bool,RotationMaxZ);

		FbxTransformInheritType FbxNode::InheritType::get()
		{
			return (FbxTransformInheritType)_Ref()->InheritType.Get();			
		}
		void FbxNode::InheritType::set(FbxTransformInheritType value)
		{
			_Ref()->InheritType.Set((ETransformInheritType)value);			
		}

		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingActive);
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,Scaling,FbxDouble3TypedProperty,Scaling);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,ScalingMin,FbxDouble3TypedProperty,ScalingMin);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,ScalingMax,FbxDouble3TypedProperty,ScalingMax);		
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMinX);
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMinY);
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMinZ);
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMaxX);
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMaxY);
		FBXNODE_PROP_GETSET_DEFINE(bool,ScalingMaxZ);

		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GeometricTranslation,FbxDouble3TypedProperty,GeometricTranslation);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GeometricRotation,FbxDouble3TypedProperty,GeometricRotation);		
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxNode,GeometricScaling,FbxDouble3TypedProperty,GeometricScaling);		

		FBXNODE_PROP_GETSET_DEFINE(double,MinDampRangeX);
		FBXNODE_PROP_GETSET_DEFINE(double,MinDampRangeY);
		FBXNODE_PROP_GETSET_DEFINE(double,MinDampRangeZ);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampRangeX);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampRangeY);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampRangeZ);
		FBXNODE_PROP_GETSET_DEFINE(double,MinDampStrengthX);
		FBXNODE_PROP_GETSET_DEFINE(double,MinDampStrengthY);
		FBXNODE_PROP_GETSET_DEFINE(double,MinDampStrengthZ);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampStrengthX);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampStrengthY);
		FBXNODE_PROP_GETSET_DEFINE(double,MaxDampStrengthZ);
		FBXNODE_PROP_GETSET_DEFINE(double,PreferedAngleX);
		FBXNODE_PROP_GETSET_DEFINE(double,PreferedAngleY);
		FBXNODE_PROP_GETSET_DEFINE(double,PreferedAngleZ);

		FBXNODE_PROP_GETSET_DEFINE(bool,Show);
		FBXNODE_PROP_GETSET_DEFINE(bool,NegativePercentShapeSupport);
		//
		FBXNODE_PROP_GETSET_DEFINE(int,DefaultAttributeIndex);


#ifndef DOXYGEN_SHOULD_SKIP_THIS
		void FbxNode::SetLocalStateFromDefaultTake(bool recursive, bool applyLimits)
		{
			_Ref()->SetLocalStateFromDefaultTake(recursive,applyLimits);
		}
		void FbxNode::SetDefaultTakeFromLocalState(bool recursive)
		{
			_Ref()->SetDefaultTakeFromLocalState(recursive);
		}
		void FbxNode::SetLocalStateFromCurrentTake(FbxTime^ time, bool recursive,bool applyLimits)
		{
			_Ref()->SetLocalStateFromCurrentTake(*time->_Ref(),recursive,applyLimits);
		}		
		void FbxNode::SetCurrentTakeFromLocalState(FbxTime^ time, bool recursive)
		{
			_Ref()->SetCurrentTakeFromLocalState(*time->_Ref(),recursive);
		}
		void FbxNode::ComputeGlobalState(kUInt updateId, bool recursive, PivotSet pivotSet, bool applyTarget)
		{
			_Ref()->ComputeGlobalState(updateId,recursive,(KFbxNode::EPivotSet)pivotSet,applyTarget);
		}		
		void FbxNode::ComputeLocalState(kUInt updateId, bool recursive)
		{
			_Ref()->ComputeLocalState(updateId,recursive);
		}
		
		FbxXMatrix^ FbxNode::GlobalState::get()
		{
			if(_GlobalState )
				*_GlobalState->_FbxXMatrix = _Ref()->GetGlobalState();
			else
				_GlobalState = gcnew FbxXMatrix(_Ref()->GetGlobalState());
			return _GlobalState;
		}
		void FbxNode::GlobalState::set(FbxXMatrix^ value)
		{
			if(value)
			{
				*_GlobalState->_FbxXMatrix = *value->_Ref();
				_Ref()->SetGlobalState(*value->_Ref());
			}
		}

		void FbxNode::SetLocalState(FbxVector4^ LT,FbxVector4^ LR,FbxVector4^ LS)
		{
			_Ref()->SetLocalState(*LT->_Ref(),*LR->_Ref(),*LS->_Ref());
		}
		void FbxNode::GetLocalState(FbxVector4^ LT,FbxVector4^ LR,FbxVector4^ LS)
		{
			_Ref()->GetLocalState(*LT->_Ref(),*LR->_Ref(),*LS->_Ref());
		}

		void FbxNode::SetGlobalStateId(kUInt updateId, bool recursive)
		{
			_Ref()->SetGlobalStateId(updateId,recursive);
		}

		kUInt FbxNode::GlobalStateId::get()
		{
			return _Ref()->GetGlobalStateId();
		}

		void FbxNode::SetLocalStateId(kUInt updateId, bool recursive)
		{
			_Ref()->SetLocalStateId(updateId,recursive);
		}

		kUInt FbxNode::LocalStateId::get()
		{
			return _Ref()->GetLocalStateId();
		}

		CLONE_DEFINITION(FbxNode,KFbxNode);

		FbxNodeLimits^ FbxNode::Limits::get()
		{
			if(!_Limits)
				_Limits = gcnew FbxNodeLimits(&_Ref()->GetLimits());
			else
				_Limits->_FbxNodeLimits = &_Ref()->GetLimits();
			return _Limits;
		}

		void FbxNode::UpdatePivotsAndLimitsFromProperties()
		{
			_Ref()->UpdatePivotsAndLimitsFromProperties();
		}
		void FbxNode::UpdatePropertiesFromPivotsAndLimits()
		{
			_Ref()->UpdatePropertiesFromPivotsAndLimits();
		}
		void FbxNode::SetRotationActiveProperty(bool val)
		{
			_Ref()->SetRotationActiveProperty(val);
		}

		void FbxNode::PivotSetToMBTransform(PivotSet pivotSet)
		{
			_Ref()->PivotSetToMBTransform((KFbxNode::EPivotSet)pivotSet);
		}
		FbxXMatrix^ FbxNode::GetLXFromLocalState( bool T, bool R, bool S, bool soff )
		{
			return gcnew FbxXMatrix(_Ref()->GetLXFromLocalState(T,R,S,soff));
		}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}