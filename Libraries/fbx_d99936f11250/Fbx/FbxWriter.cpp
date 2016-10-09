#pragma once
#include "stdafx.h"
#include "FbxWriter.h"
#include "FbxSdkManager.h"
#include "FbxString.h"
#include "FbxStreamOptions.h"
#include "FbxDocument.h"
#include "FbxRenamingStrategy.h"
#include "FbxError.h"
#include "FbxAxisSystem.h"
#include "FbxScene.h"
#include "FbxNode.h"
#include "FbxSystemUnit.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace IO
		{			

			void FbxWriter::CollectManagedMemory()
			{
				this->_WriteOptions = nullptr;
				this->_KError = nullptr;
			}			
			bool FbxWriter::FileCreate(String^ fileName)
			{				
				STRINGTOCHAR_ANSI(f,fileName);
				bool b = _Ref()->FileCreate(f);
				FREECHARPOINTER(f);
				return b;
			}
			bool FbxWriter::FileClose()				
			{
				return _Ref()->FileClose();
			}
			bool FbxWriter::IsFileOpen::get()
			{
				return _Ref()->IsFileOpen();
			}				

			REF_PROPERTY_GET_DEFINATION_FROM_REF(FbxWriter,KFbxStreamOptions,GetWriteOptions(),FbxStreamOptionsManaged,WriteOptions);							
			bool FbxWriter::Write(FbxDocumentManaged^ document, FbxStreamOptionsManaged^ streamOptions)
			{
				return _Ref()->Write(document->_Ref(),streamOptions->_Ref());
			}
			bool FbxWriter::PreprocessScene(FbxSceneManaged^ scene)
			{
				return _Ref()->PreprocessScene(*scene->_Ref());
			}
			bool FbxWriter::PostprocessScene(FbxSceneManaged^ scene)
			{
				return _Ref()->PostprocessScene(*scene->_Ref());
			}
			bool FbxWriter::SetFileExportVersion(FbxStringManaged^ version)
			{
				return _Ref()->SetFileExportVersion(*version->_Ref());
			}

			void FbxWriter::SetRenamingMode(FbxSceneRenamer::RenamingMode renamingMode)
			{
				_Ref()->SetRenamingMode((KFbxSceneRenamer::ERenamingMode)renamingMode);
			}
			void FbxWriter::SetResamplingRate(double resamplingRate)
			{
				_Ref()->SetResamplingRate(resamplingRate);
			}

			REF_PROPERTY_GET_DEFINATION_FROM_VALUE(FbxWriter,GetError(),FbxErrorManaged,KError);			
			FbxWriter::Error FbxWriter::LastErrorID::get()
			{
				return (Error)_Ref()->GetLastErrorID();
			}				
			String^ FbxWriter::LastErrorString::get()
			{
				return gcnew String(_Ref()->GetLastErrorString());
			}				
			void FbxWriter::GetMessage(FbxStringManaged^ message)
			{
				_Ref()->GetMessage(*message->_Ref());
			}
			void FbxWriter::ClearMessage()
			{
				_Ref()->ClearMessage();
			}				
			FbxNode^ FbxWriter::FindRootNode(FbxSceneManaged^ scene)
			{
				KFbxNode* n = _Ref()->FindRootNode(*scene->_Ref());				
				if(n)
					return gcnew FbxNode(n);				
				return nullptr;
			}
			bool FbxWriter::CheckSpaceInNodeNameRecursive(FbxNode^ node, FbxStringManaged^ nodeNameList)
			{
				return _Ref()->CheckSpaceInNodeNameRecursive(node->_Ref(),*nodeNameList->_Ref());
			}

		}
	}
}