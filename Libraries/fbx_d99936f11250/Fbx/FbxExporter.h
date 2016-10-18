#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace FbxSDK
{
	ref class FbxIOManaged;
	ref class FbxIOFileHeaderInfoManaged;
	ref class FbxThreadManaged;
	ref class FbxWriterManaged;

	namespace fileio
	{
		public ref class FbxExporterManaged : FbxIOBaseM
		{
			
		private:
			FbxExporter* _FbxExporter;
		internal:
			FbxExporterManaged(FbxExporter* instance) : FbxIOBaseM(instance)
			{
				_Free = false;
			}

			//FBXOBJECT_DECLARE(FbxExporterManaged);

		public:

			//virtual void CollectManagedMemory() override;

			virtual bool Initialize(System::String^ pFileName, int pFileFormat, FbxIOSettingsM^ pIOSettings)
			{
				_FbxExporter->Initialize(pFileName, pFileFormat, pIOSettings->Ref());
			}
			virtual bool Initialize(FbxStreamM^ pStream, void* pStreamData, int pFileFormat, FbxIOSettingsM^ pIOSettings)
			{
				_FbxExporter->Initialize(pStream->Ref(), pStreamData, pFileFormat, pIOSettings->Ref());
			}
			bool GetExportOptions() 
			{ 
				return _FbxExporter->GetExportOptions(); 
			}
			FbxIOSettingsM^ GetIOSettings()
			{
				return gcnew FbxIOSettingsM(_FbxExporter->GetIOSettings());
			}
			void SetIOSettings(FbxIOSettingsM^ pIOSettings)
			{
				_FbxExporter->SetIOSettings(pIOSettings->Ref());
			}
			bool Export(FbxDocumentM^ pDocument, bool pNonBlocking)
			{
				return _FbxExporter->Export(pDocument, pNonBlocking);
			}
			bool IsExporting(bool% pExportResult)
			{
				return _FbxExporter->IsExporting(pExportResult);
			}
			float GetProgress(FbxStringM^ pStatus)
			{
				return _FbxExporter->GetProgress(pStatus);
			}
			void SetProgressCallback(FbxProgressCallbackM^ pCallback, void* pArgs)
			{
				_FbxExporter->SetProgressCallback(pCallback, pArgs);
			}
			int GetFileFormat()
			{
				return _FbxExporter->GetFileFormat(); 
			}
			property bool IsFBX
			{
				bool get()
				{
					return Ref()->IsFBX();
				}
			}
			bool IsFBX()
			{ 
				return ; 
			}
			char const* const* GetCurrentWritableVersions()
			{
				return _FbxExporter->GetCurrentWritableVersions();
			}
			bool SetFileExportVersion(FbxString pVersion, FbxSceneRenamerM::ERenamingMode pRenamingMode = FbxSceneRenamerM::ERenamingMode::eNone);
			void SetResamplingRate(double pResamplingRate)
			{
				_FbxExporter->SetResamplingRate(pResamplingRate);
			}
			void SetDefaultRenderResolution(FbxString pCamName, FbxString pResolutionMode, double pW, double pH)
			{

			}
			FbxIOFileHeaderInfo* GetFileHeaderInfo()
			{

			}
			bool GetExportOptions(FbxIOManaged* pFbxObject)
			{

			}
			bool Export(FbxDocumentManaged* pDocument, FbxIOManaged* pFbxObject)
			{

			}
		}
	}
}