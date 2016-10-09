#pragma once
#include "stdafx.h"
#include "FbxExporter.h"
#include "FbxString.h"
#include "FbxStreamOptions.h"
#include "FbxDocument.h"
#include "FbxRenamingStrategy.h"
#include "FbxSdkManager.h"
#include "FbxClassId.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{	
			void FbxExporterManaged::CollectManagedMemory()
			{
				this->_ExportOptions = nullptr;
				FbxIOManaged::CollectManagedMemory();
			}

			FBXOBJECT_DEFINITION(FbxExporterManaged,FbxExporter);
			
			REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxExporterManaged,KFbxStreamOptions,GetExportOptions(),FbxStreamOptionsManaged,ExportOptions);
			bool FbxExporterManaged::Initialize(String^ fileName)
			{
				return FbxIOManaged::Initialize(fileName);
			}			

			bool FbxExporterManaged::Export(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions)
			{
				return _Ref()->Export(document->_Ref(),streamOptions->_Ref());
			}
			bool FbxExporterManaged::Export(FbxDocumentManaged^ document)
			{
				return _Ref()->Export(document->_Ref());
			}
			void FbxExporterManaged::ReleaseExportOptions(FbxStreamOptionsManaged^ streamOptions)
			{
				_Ref()->ReleaseExportOptions(streamOptions->_Ref());
			}
			VALUE_PROPERTY_GET_DEFINATION(FbxExporterManaged,GetFileFormat(),int,FileFormat);			
			void FbxExporterManaged::FileFormat::set(int value)
			{
				_Ref()->SetFileFormat(value);
			}
			bool FbxExporterManaged::IsFBX::get()
			{
				return _Ref()->IsFBX();
			}
			bool FbxExporterManaged::SetFileExportVersion(FbxStringManaged^ version, FbxSceneRenamerManaged::RenamingMode renamingMode)
			{
				return _Ref()->SetFileExportVersion(*version->_Ref(), (KFbxSceneRenamer::ERenamingMode)renamingMode);
			}			

#ifndef DOXYGEN_SHOULD_SKIP_THIS				
#endif // #ifndef DOXYGEN_SHOULD_SKIP_THIS
		}
	}
}