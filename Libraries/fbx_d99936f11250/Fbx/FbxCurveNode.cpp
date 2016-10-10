#pragma once
#include "stdafx.h"
#include "FbxCurveNode.h"
#include "FbxCurve.h"
#include "FbxTime.h"
#include "FbxDataType.h"



{
	namespace FbxSDK
	{	
		void FbxExternalTimingInformation::CollectManagedMemory()
		{
			_Duration = nullptr;		
			_LclOffset = nullptr;			
		}

		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxExternalTimingInformation,mLclOffset,FbxTime,LclOffset);
		REF_PROPERTY_GETSET_DEFINATION_FROM_VALUE(FbxExternalTimingInformation,mDuration,FbxTime,Duration);		


		VALUE_PROPERTY_GETSET_DEFINATION(FbxCurveNodeEvent,mEventCount,int,EventCount);								
		IntPtr FbxCurveNodeEvent::Data::get()
		{
			if(_Ref()->mData)
				return IntPtr(_Ref()->mData);
			else
				return IntPtr::Zero;
		}
		void FbxCurveNodeEvent::Data::set(IntPtr value)
		{
			_Ref()->mData = value.ToPointer();
		}
		void FbxCurveNodeEvent::Clear ()
		{
			_Ref()->Clear();
		}
		void FbxCurveNodeEvent::Add(int what)
		{
			_Ref()->Add(what);
		}


		void FbxCurveNodeCandidateState::CollectManagedMemory()
		{
		}
		void FbxCurveNodeCandidateState::Dump(int level)
		{
			_Ref()->Dump(level);
		}
		/*void FbxCurveNodeCandidateState::SetCandidateTotalTime (FbxTime^ candidateTime )
		{
		_Ref()->SetCandidateTotalTime (*candidateTime->_Ref());
		}
		void FbxCurveNodeCandidateState::SetCandidateSpecificTime(FbxTime^ candidateTime )
		{
		_Ref()->SetCandidateSpecificTime(*candidateTime->_Ref());
		}*/



		void FbxCurveNode::CollectManagedMemory()
		{
			_FbxCurve = nullptr;
			_Parent = nullptr;
		}

		/*FbxCurveNode::FbxCurveNode(String^ nodeName, String^ timeWarpName, 
			FbxDataType^ dataType,int layerType,int	layerID);*/
		/*FbxCurveNode::FbxCurveNode(FbxCurveNode^ templateCurveNode)
		{
			*_FbxCurveNode = *templateCurveNode->_Ref();
		}*/

#ifdef K_PLUGIN
		void FbxCurveNode::Destroy (int local)
		{
			_Ref()->Destroy(local);
		}
#else
		IObject_Declare (Implementation) 
#endif

		void FbxCurveNode::CreateFCurve( )		
		{
			_Ref()->CreateFCurve();
		}
		bool FbxCurveNode::FCurveCreated::get()
		{
			return _Ref()->FCurveCreated();
		}
		FbxCurveNode^ FbxCurveNode::Clone(bool keepAttached)
		{
			KFCurveNode* c = _Ref()->Clone(keepAttached);
			FbxCurveNode^ curve = gcnew FbxCurveNode(c);
			curve->_Free = true;
			return curve;
		}
		FbxCurveNode^ FbxCurveNode::CloneTemplate(bool keepAttached, bool cloneFCurves, bool createCurves, int layerID)
		{
			KFCurveNode* c = _Ref()->CloneTemplate(keepAttached,cloneFCurves,createCurves,layerID);
			FbxCurveNode^ curve = gcnew FbxCurveNode(c);
			curve->_Free = true;
			return curve;
		}
		void FbxCurveNode::CopyFrom(FbxCurveNode^ source,bool transferCurve)
		{
			_Ref()->CopyFrom(source->_Ref(),transferCurve);
		}
		FbxCurveNode^ FbxCurveNode::Copy(FbxTime^ start, FbxTime^ stop)
		{
			KFCurveNode* c = _Ref()->Copy(*start->_Ref(),*stop->_Ref());
			FbxCurveNode^ curve = gcnew FbxCurveNode(c);
			curve->_Free = true;
			return curve;
		}
		String^ FbxCurveNode::Name::get()
		{
			return gcnew String(_Ref()->GetName());
		}
		String^ FbxCurveNode::TimeWarpName::get()
		{
			return gcnew String(_Ref()->GetTimeWarpName());
		}
		String^ FbxCurveNode::TypeName::get()
		{
			return gcnew String(_Ref()->GetTypeName());
		}
		FbxCurve^ FbxCurveNode::FCurveGet()
		{
			KFCurve* c = _Ref()->FCurveGet();
			if(c)
			{
				if(_FbxCurve != nullptr)
					_FbxCurve->_SetPointer(c,false);
				else
					_FbxCurve =  gcnew FbxCurve(c);
				return _FbxCurve;
			}
			return nullptr;
		}
		FbxCurve^ FbxCurveNode::FCurveSet(FbxCurve^ curve, bool destroyOldCurve)
		{			
			if(curve)
				_Ref()->FCurveSet(curve->_Ref(),destroyOldCurve);
			else
				_Ref()->FCurveSet(NULL,destroyOldCurve);
			FbxCurve^ temp = _FbxCurve;
			_FbxCurve = curve;
			if(destroyOldCurve)
				temp = nullptr;
			return temp;
		}
		void FbxCurveNode::FCurveReplace (FbxCurve^ curve)
		{
			if(curve)
				_Ref()->FCurveReplace(curve->_Ref());
			else
				_Ref()->FCurveReplace(NULL);			
			_FbxCurve = curve;
		}
		void FbxCurveNode::Clear()
		{
			_Ref()->Clear();
		}
		int FbxCurveNode::Add(FbxCurveNode^ curveNode)
		{
			return _Ref()->Add(curveNode->_Ref());
		}
		void FbxCurveNode::Remove(int index)
		{
			_Ref()->Remove(index);
		}
		void FbxCurveNode::Delete(int index)
		{
			_Ref()->Delete(index);
		}
		int FbxCurveNode::Count::get()
		{
			return _Ref()->GetCount();
		}
		FbxCurveNode^ FbxCurveNode::Get(int index)
		{
			return gcnew FbxCurveNode(_Ref()->Get(index));
		}
		bool FbxCurveNode::IsChild(FbxCurveNode^ curveNode, bool recursive)		
		{
			return _Ref()->IsChild(curveNode->_Ref(),recursive);
		}
		int FbxCurveNode::Find (String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);
			int i = _Ref()->Find(n);
			FREECHARPOINTER(n);
			return i;
		}
		int FbxCurveNode::Find(FbxCurveNode^ node)
		{
			return _Ref()->Find(node->_Ref());
		}
		FbxCurveNode^ FbxCurveNode::FindRecursive(String^ name)
		{
			STRINGTOCHAR_ANSI(n,name);						
			KFCurveNode* c = _Ref()->FindRecursive(n);			
			FREECHARPOINTER(n);
			if(c)
				return gcnew FbxCurveNode(c);
			return nullptr;
		}
		FbxCurveNode^ FbxCurveNode::FindOrCreate(String^ name, bool findOrCreateCurve)
		{
			STRINGTOCHAR_ANSI(n,name);						
			KFCurveNode* c = _Ref()->FindOrCreate(n,findOrCreateCurve);			
			FREECHARPOINTER(n);
			if(c)
				return gcnew FbxCurveNode(c);
			return nullptr;
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxCurveNode,KFCurveNode,GetParent(),FbxCurveNode,Parent);
		void FbxCurveNode::Parent::set(FbxCurveNode^ value)
		{
			_Parent = value;
			if(value)
				_Ref()->SetParent(value->_Ref());
			else
				_Ref()->SetParent(NULL);
		}
		int FbxCurveNode::KeyGetCount(bool recursiveInLayers)
		{
			return _Ref()->KeyGetCount(recursiveInLayers);
		}
		void FbxCurveNode::KeyGetCount([OutAttribute]int %curveCount, [OutAttribute]int %totalCount, [OutAttribute]int %minCount, [OutAttribute]int %maxCount)
		{
			int cc,tc,minc,maxc;
			_Ref()->KeyGetCount(cc,tc,minc,maxc);
			curveCount = cc;
			totalCount = tc;
			minCount = minc;
			maxCount = maxc;
		}
		bool FbxCurveNode::GetAnimationInterval (FbxTime^ start, FbxTime^ stop)
		{
			return _Ref()->GetAnimationInterval (*start->_Ref(),*stop->_Ref());
		}
		void FbxCurveNode::GetTimeSpan(FbxTime^ start, FbxTime^ stop)
		{
			_Ref()->GetTimeSpan(*start->_Ref(),*stop->_Ref());
		}
		void FbxCurveNode::Delete(FbxTime^ start, FbxTime^ stop)
		{
			_Ref()->Delete(*start->_Ref(),*stop->_Ref());
		}
		void FbxCurveNode::Replace(FbxCurveNode^ source, FbxTime^ start, FbxTime^ stop , bool useGivenSpan , bool keyStartEndOnNoKey, FbxTime^ timeSpanOffset)
		{
			_Ref()->Replace(source->_Ref(),*start->_Ref(),*stop->_Ref(),useGivenSpan ,keyStartEndOnNoKey,*timeSpanOffset->_Ref());
		}

//#ifndef DOXYGEN_SHOULD_SKIP_THIS
//
//		int FbxCurveNode::IncReferenceCount();
//		int FbxCurveNode::DecReferenceCount();
//		VALUE_PROPERTY_GET_DECLARE(int,FbxCurveNode::ReferenceCount);
//		VALUE_PROPERTY_GETSET_DECLARE(int,FbxCurveNode::TakeType);
//		VALUE_PROPERTY_GET_DECLARE(bool,FbxCurveNode::GetVisibility );
//		void FbxCurveNode::SetVisibility (bool visible, bool recursive, bool recurseLayer, int childIndex);
//		void FbxCurveNode::DataNodeSet(FbxCurveNode^ dataNode, bool recursive);
//		FbxCurveNode^ FbxCurveNode::DataNodeGet();
//		bool FbxCurveNode::SetPreExtrapolation(kUInt newPreExtrapolation, bool respectUserLock);
//		bool FbxCurveNode::SetPreExtrapolationCount(kUInt newPreExtrapolationCount, bool respectUserLock);
//		bool FbxCurveNode::SetPostExtrapolation(kUInt newPreExtrapolation, bool respectUserLock);
//		bool FbxCurveNode::SetPostExtrapolationCount(kUInt newPreExtrapolationCount, bool respectUserLock);
//		VALUE_PROPERTY_GETSET_DECLARE(int,FbxCurveNode::ContainerType);
//		VALUE_PROPERTY_GETSET_DECLARE(int,FbxCurveNode::InOutType);
//		VALUE_PROPERTY_GETSET_DECLARE(bool,FbxCurveNode::Expended);
//		VALUE_PROPERTY_GETSET_DECLARE(bool,FbxCurveNode::MultiLayer);
//		FbxCurveNode^ FbxCurveNode::GetLayerNode(int layerID);
//		bool FbxCurveNode::LookLikeSampledData(FbxTime^ thresholdPeriod);
//		VALUE_PROPERTY_GET_DECLARE(int,FbxCurveNode::UpdateId) ;
//		VALUE_PROPERTY_GET_DECLARE(int,FbxCurveNode::ValuesUpdateId) ;
//		int FbxCurveNode::GetNodeUpdateId();
//		bool FbxCurveNode::CallbackEnable (bool enable) ;
//		bool FbxCurveNode::CallbackIsEnable() ;
//		void FbxCurveNode::CallbackClear() ;
//		void FbxCurveNode::CallbackAddEvent(int what) ;
//		VALUE_PROPERTY_GETSET_DECLARE(bool,FbxCurveNode::UseQuaternion);				
//
//		void FbxCurveNode::GetCandidateState(FbxCurveNodeCandidateState^ state);
//		void FbxCurveNode::SetCandidateState(FbxCurveNodeCandidateState^ state, bool destroyMissingLayers);
//
//		int		FbxCurveNode::GetCandidateSpecificMethod();
//		int		FbxCurveNode::GetCandidateTotalMethod();
//		FbxTime^ FbxCurveNode::GetCandidateTotalTime();
//		int		FbxCurveNode::GetCandidateTotalValueSize();
//		void	FbxCurveNode::SetCandidateSpecificMethod(int method);
//		void	FbxCurveNode::SetCandidateTotalMethod(int method);
//		void	FbxCurveNode::SetCandidateTotalTime(FbxTime^ time);		
//		VALUE_PROPERTY_GETSET_DECLARE(int,FbxCurveNode::RotationOrder);							
//		VALUE_PROPERTY_GET_DECLARE(int,FbxCurveNode::LayerType);
//#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
	}
}