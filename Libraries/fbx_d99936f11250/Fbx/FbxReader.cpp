#pragma once
#include "stdafx.h"
#include "FbxReader.h"
#include "FbxSdkManager.h"
#include "FbxImporter.h"
#include "FbxString.h"
#include "FbxStreamOptions.h"
#include "FbxDocument.h"
#include "FbxError.h"
#include "FbxAxisSystem.h"
#include "FbxSystemUnit.h"


{
	namespace FbxSDK
	{
		namespace IO
		{
			void FbxReaderManaged::CollectManagedMemory()
			{
				this->readOptions = nullptr;
				this->_KError = nullptr;
			}						
			Version^ FbxReaderManaged::GetVersion()
			{					
				int minor,major,revision,build = 0;
				_Ref()->GetVersion(major,minor,revision);
				return gcnew Version(major,minor,build,revision);
			}

			bool FbxReaderManaged::FileOpen(String^ fileName)
			{				
				STRINGTOCHAR_ANSI(f,fileName);
				bool b = _Ref()->FileOpen(f);
				FREECHARPOINTER(f);
				return b;
			}
			bool FbxReaderManaged::FileClose()				
			{
				return _Ref()->FileClose();
			}
			bool FbxReaderManaged::IsFileOpen::get()
			{
				return _Ref()->IsFileOpen();
			}				
			FbxStreamOptionsManaged^ FbxReaderManaged::GetReadOptions(bool parseFileAsNeeded)
			{
				if(readOptions)
					readOptions->_SetPointer(_Ref()->GetReadOptions(parseFileAsNeeded),false);
				else
					readOptions = FbxCreator::CreateFbxStreamOptions(_Ref()->GetReadOptions(parseFileAsNeeded));
				return readOptions;
			}
			FbxStreamOptionsManaged^ FbxReaderManaged::GetReadOptions()
			{
				return GetReadOptions(false);
			}
			bool FbxReaderManaged::Read(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions)
			{
				return _Ref()->Read(document->_Ref(), streamOptions->_Ref());
			}
			REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxReaderManaged,GetError(),FbxErrorManaged,KError);			
			FbxReaderManaged::Error FbxReaderManaged::LastErrorID::get()
			{
				return (Error)_Ref()->GetLastErrorID();
			}				
			String^ FbxReaderManaged::LastErrorString::get()
			{
				return gcnew String(_Ref()->GetLastErrorString());
			}				
			void FbxReaderManaged::GetMessage(FbxStringManaged^ message)
			{
				_Ref()->GetMessage(*message->_Ref());
			}
			void FbxReaderManaged::ClearMessage()
			{
				_Ref()->ClearMessage();
			}
			bool FbxReaderManaged::FileOpen(String^ fileName, FileOpenSpecialFlags flags)
			{				
				STRINGTOCHAR_ANSI(f,fileName);
				bool b = _Ref()->FileOpen(f,(FbxReader::EFileOpenSpecialFlags)flags);
				FREECHARPOINTER(f);
				return b;
			}
			bool FbxReaderManaged::GetAxisInfo(FbxAxisSystem^ axisSystem, FbxSystemUnit^ systemUnits) 
			{
				return _Ref()->GetAxisInfo(axisSystem->_Ref(),systemUnits->_Ref());
			}
		}
	}
}