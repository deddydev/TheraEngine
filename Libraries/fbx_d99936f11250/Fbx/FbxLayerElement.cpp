#pragma once
#include "stdafx.h"
#include "FbxLayerElement.h"
#include "FbxStream.h"
#include "FbxDataType.h"


namespace Skill
{
	namespace FbxSDK
	{		
		void FbxLayerElement::CollectManagedMemory()
		{
		}
		FbxLayerElement::MappingMode FbxLayerElement::Mapping_Mode::get()
		{
			return (FbxLayerElement::MappingMode)_Ref()->GetMappingMode();
		}
		void FbxLayerElement::Mapping_Mode::set(MappingMode value)
		{
			_Ref()->SetMappingMode((KFbxLayerElement::EMappingMode)value);
		}
		void FbxLayerElement::Reference_Mode::set(ReferenceMode value)
		{
			_Ref()->SetReferenceMode((KFbxLayerElement::EReferenceMode)value);
		}
		FbxLayerElement::ReferenceMode FbxLayerElement::Reference_Mode::get()
		{
			return (FbxLayerElement::ReferenceMode)_Ref()->GetReferenceMode();
		}

		String^ FbxLayerElement::Name::get()
		{
			return gcnew String(_Ref()->GetName());
		}
		void FbxLayerElement::Name::set(String^ value)
		{
			STRINGTO_CONSTCHAR_ANSI(v,value);
			_Ref()->SetName(v);
			FREECHARPOINTER(v);
		}
		void FbxLayerElement::Destroy()
		{
			_Ref()->Destroy();
			_Free = false;
		}
		void FbxLayerElement::SetType(FbxDataType^ type)
		{
			_Ref()->SetType(type->_Ref());
		}
		int FbxLayerElement::MemorySize::get()
		{
			return _Ref()->MemorySize();
		}
		bool FbxLayerElement::ContentWriteTo(FbxStream^ stream)
		{
			return _Ref()->ContentWriteTo(*stream->_Ref());
		}
		bool FbxLayerElement::ContentReadFrom(FbxStream^ stream)
		{
			return _Ref()->ContentReadFrom(*stream->_Ref());
		}
	}
}