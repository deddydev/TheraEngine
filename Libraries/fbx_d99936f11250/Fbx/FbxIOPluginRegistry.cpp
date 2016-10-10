#pragma once
#include "stdafx.h"
#include "FbxIOPluginRegistry.h"
#include "FbxString.h"
#include "FbxImporter.h"
#include "FbxExporter.h"
#include "FbxReader.h"
#include "FbxWriter.h"
#include "FbxSDKManager.h"



{
	namespace FbxSDK
	{

		void FbxIOPluginRegistry::CollectManagedMemory()
		{			
		}				
		void FbxIOPluginRegistry::RegisterReader(String^ pluginPath,
			int %firstPluginID,
			int %registeredCount)
		{			
			STRINGTO_CONSTCHAR_ANSI(p,pluginPath);
			int f,r;
			_Ref()->RegisterReader(p,f,r);
			FREECHARPOINTER(p);
			firstPluginID = f;
			registeredCount = r;

		}						
		void FbxIOPluginRegistry::RegisterWriter(String^ pluginPath,
			int %firstPluginID,
			int %registeredCount)
		{			
			STRINGTO_CONSTCHAR_ANSI(p,pluginPath);
			int f,r;
			_Ref()->RegisterWriter(p,f,r);
			FREECHARPOINTER(p);
			firstPluginID = f;
			registeredCount = r;

		}

		FbxReader^ FbxIOPluginRegistry::CreateReader(FbxSdkManagerManaged^ manager, 
				FbxImporter^ importer, 
				int pluginID)
		{
			return gcnew FbxReader(_Ref()->CreateReader(*manager->_Ref(),*importer->_Ref(),pluginID));
		}
		FbxWriter^ FbxIOPluginRegistry::CreateWriter(FbxSdkManagerManaged^ manager, 
				FbxExporter^ exporter,
				int pluginID)
		{
			return gcnew FbxWriter(_Ref()->CreateWriter(*manager->_Ref(),*exporter->_Ref(),pluginID));
		}

		int FbxIOPluginRegistry::FindReaderIDByExtension(String^ ext)
		{			
			STRINGTO_CONSTCHAR_ANSI(e,ext);
			int r = _Ref()->FindReaderIDByExtension(e);
			FREECHARPOINTER(e);
			return r;
		}						
		int FbxIOPluginRegistry::FindWriterIDByExtension(String^ ext)
		{			
			STRINGTO_CONSTCHAR_ANSI(e,ext);
			int r = _Ref()->FindWriterIDByExtension(e);
			FREECHARPOINTER(e);
			return r;
		}
		int FbxIOPluginRegistry::FindReaderIDByDescription(String^ desc)
		{			
			STRINGTO_CONSTCHAR_ANSI(d,desc);
			int r = _Ref()->FindReaderIDByDescription(d);
			FREECHARPOINTER(d);
			return r;
		}
		int FbxIOPluginRegistry::FindWriterIDByDescription(String^ desc)
		{			
			STRINGTO_CONSTCHAR_ANSI(d,desc);
			int r = _Ref()->FindWriterIDByDescription(d);
			FREECHARPOINTER(d);
			return r;
		}
		bool FbxIOPluginRegistry::ReaderIsFBX(int fileFormat)
		{
			return _Ref()->ReaderIsFBX(fileFormat);
		}
		bool FbxIOPluginRegistry::WriterIsFBX(int fileFormat)
		{
			return _Ref()->WriterIsFBX(fileFormat);
		}
		int FbxIOPluginRegistry::ReaderFormatCount::get()
		{
			return _Ref()->GetReaderFormatCount();
		}

		int FbxIOPluginRegistry::WriterFormatCount::get()
		{
			return _Ref()->GetWriterFormatCount();
		}			
		String^ FbxIOPluginRegistry::GetReaderFormatDescription(int fileFormat)
		{
			return gcnew String(_Ref()->GetReaderFormatDescription(fileFormat));
		}
		String^ FbxIOPluginRegistry::GetWriterFormatDescription(int fileFormat)
		{
			return gcnew String(_Ref()->GetWriterFormatDescription(fileFormat));
		}
		String^ FbxIOPluginRegistry::GetReaderFormatExtension(int fileFormat)
		{
			return gcnew String(_Ref()->GetReaderFormatExtension(fileFormat));
		}						
		String^ FbxIOPluginRegistry::GetWriterFormatExtension(int fileFormat)
		{
			return gcnew String(_Ref()->GetWriterFormatExtension(fileFormat));
		}
		bool FbxIOPluginRegistry::DetectFileFormat(String^ fileName, int %fileFormat)
		{			
			STRINGTO_CONSTCHAR_ANSI(f,fileName);
			int format = -1;
			bool b = _Ref()->DetectFileFormat(f,format);
			if(b)
				fileFormat = format;
			FREECHARPOINTER(f);
			return b;
		}
		int FbxIOPluginRegistry::NativeReaderFormat::get()
		{
			return _Ref()->GetNativeReaderFormat();
		}			
		int FbxIOPluginRegistry::NativeWriterFormat::get()
		{
			return _Ref()->GetNativeWriterFormat();
		}

#ifndef DOXYGEN_SHOULD_SKIP_THIS

		void FbxIOPluginRegistry::RegisterInternalIOPlugins()
		{
			_Ref()->RegisterInternalIOPlugins();
		}
#endif //DOXYGEN
	}
}