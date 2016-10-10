#pragma once
#include "stdafx.h"
#include "FbxImporter.h"
#include "FbxStreamOptions.h"
#include "FbxDocument.h"
#include "FbxTakeInfo.h"
#include "FbxDocumentInfo.h"
#include "FbxAxisSystem.h"
#include "FbxSystemUnit.h"
#include "FbxSdkManager.h"
#include "FbxString.h"
#include "FbxClassId.h"


{
	namespace FbxSDK
	{
		namespace IO
		{	
			void FbxImporterManaged::CollectManagedMemory()
			{
				this->_ImportOptions = nullptr;
				this->_SceneInfo = nullptr;
			}

			FBXOBJECT_DEFINITION(FbxImporterManaged,KFbxImporter);
			bool FbxImporterManaged::Initialize(String^ fileName)
			{
				return FbxIOManaged::Initialize(fileName);
			}			
			Version^ FbxImporterManaged::FileVersion::get()
			{
				int minor,major,revision,build = 0;
				_Ref()->GetFileVersion(major,minor,revision);
				return gcnew Version(major,minor,build,revision);
			}				
			bool FbxImporterManaged::GetDefaultRenderResolution(FbxStringManaged^ camName, FbxStringManaged^ resolutionMode, double^ %w, double^ %h)
			{
				double pw,ph;
				bool b = _Ref()->GetDefaultRenderResolution(*camName->_Ref(),*resolutionMode->_Ref(),pw,ph);
				w = pw;
				h =  ph;
				return b;
			}
			FbxStreamOptionsManaged^ FbxImporterManaged::GetImportOptions(FbxImporterManaged::StreamOptionsGeneration streamOptionsGeneration)
			{
				if(_ImportOptions )
					_ImportOptions->_SetPointer(_Ref()->GetImportOptions((KFbxImporter::EStreamOptionsGeneration)streamOptionsGeneration),false);
				else
					_ImportOptions = FbxCreator::CreateFbxStreamOptions(_Ref()->GetImportOptions((KFbxImporter::EStreamOptionsGeneration)streamOptionsGeneration));
				return _ImportOptions;
			}
			FbxStreamOptionsManaged^ FbxImporterManaged::GetImportOptions()
			{
				if(_ImportOptions )
					_ImportOptions->_SetPointer(_Ref()->GetImportOptions(),false);
				else
					_ImportOptions = FbxCreator::CreateFbxStreamOptions(_Ref()->GetImportOptions());
				return _ImportOptions;
			}
			bool FbxImporterManaged::Import(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions)
			{
				return _Ref()->Import(document->_Ref(), streamOptions->_Ref());
			}
			bool FbxImporterManaged::Import(FbxDocumentManaged^ document)
			{
				return _Ref()->Import(document->_Ref());
			}
			void FbxImporterManaged::ReleaseImportOptions(FbxStreamOptionsManaged^ streamOptions)
			{
				_Ref()->ReleaseImportOptions(streamOptions->_Ref());
			}
			int FbxImporterManaged::TakeCount::get()
			{
				return _Ref()->GetTakeCount();
			}				
			FbxTakeInfo^ FbxImporterManaged::GetTakeInfo(int index)
			{
				KFbxTakeInfo* i = _Ref()->GetTakeInfo(index);
				if(i)
					return gcnew FbxTakeInfo(i);
				return nullptr;
			}
			String^ FbxImporterManaged::CurrentTakeName::get()
			{
				return gcnew String(_Ref()->GetCurrentTakeName());
			}
			REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxImporterManaged,KFbxDocumentInfo,GetSceneInfo(),FbxDocumentInfo,SceneInfo);							
			int  FbxImporterManaged::FileFormat::get()
			{
				return _Ref()->GetFileFormat();
			}
			void FbxImporterManaged::FileFormat::set(int value)
			{
				_Ref()->SetFileFormat(value);
			}				
			bool FbxImporterManaged::IsFBX::get()
			{
				return _Ref()->IsFBX();
			}			
			void FbxImporterManaged::ParseForGlobalSettings(bool state)
			{
				_Ref()->ParseForGlobalSettings(state);
			}
			bool FbxImporterManaged::GetAxisInfo(FbxAxisSystem^ axisSystem, FbxSystemUnit^ systemUnits)
			{
				return _Ref()->GetAxisInfo(axisSystem->_Ref(),systemUnits->_Ref());
			}

			void FbxImporterManaged::ParseForStatistics(bool state)
			{
				_Ref()->ParseForStatistics(state);
			}

#ifndef DOXYGEN_SHOULD_SKIP_THIS
			bool FbxImporterManaged::UpdateImportOptions(FbxStreamOptionsManaged^ streamOptions)
			{
				return _Ref()->UpdateImportOptions(streamOptions->_Ref());
			}

#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS			

		}
	}
}