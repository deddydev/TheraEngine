#pragma once
#include "stdafx.h"
#include "FbxGeometryWeightedMap.h"
#include "FbxError.h"
#include "FbxWeightedMapping.h"
#include "FbxGeometry.h"
#include "FbxClassId.h"
#include "FbxSdkManager.h"


namespace Skill
{
	namespace FbxSDK
	{
		FBXOBJECT_DEFINITION(FbxGeometryWeightedMap,KFbxGeometryWeightedMap);

		void FbxGeometryWeightedMap::CollectManagedMemory()
		{
			_KError = nullptr;
			_SourceGeometry = nullptr;
			_DestinationGeometry = nullptr;
			FbxObjectManaged::CollectManagedMemory();			
		}
		REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxGeometryWeightedMap,GetError(),FbxErrorManaged,KError);

		FbxGeometryWeightedMap::Error FbxGeometryWeightedMap::LastErrorID::get()
		{
			return (Error)_Ref()->GetLastErrorID();
		}			
		String^ FbxGeometryWeightedMap::LastErrorString::get()
		{
			return gcnew String(_Ref()->GetLastErrorString());
		}			
		FbxWeightedMapping^ FbxGeometryWeightedMap::SetValues(FbxWeightedMapping^ weightedMappingTable)
		{
			return gcnew FbxWeightedMapping(_Ref()->SetValues(weightedMappingTable->_Ref()));
		}
		FbxWeightedMapping^ FbxGeometryWeightedMap::GetValues()
		{
			return gcnew FbxWeightedMapping(_Ref()->GetValues());
		}

		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGeometryWeightedMap,KFbxGeometry,GetSourceGeometry(),FbxGeometry,SourceGeometry);
		REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxGeometryWeightedMap,KFbxGeometry,GetDestinationGeometry(),FbxGeometry,DestinationGeometry);		

#ifndef DOXYGEN_SHOULD_SKIP_THIS		
		CLONE_DEFINITION(FbxGeometryWeightedMap,KFbxGeometryWeightedMap);
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS


	}
}