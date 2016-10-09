#pragma once
#include "stdafx.h"
#include "FbxKRenamingStrategy.h"
#include "FbxName.h"

namespace Skill
{
	namespace FbxSDK
	{		
		void FbxKRenamingStrategy::CollectManagedMemory()
		{		
		}		
		void FbxKRenamingStrategy::Clear()
		{
			_Ref()->Clear();
		}
		bool FbxKRenamingStrategy::Rename(FbxName^ name)
		{
			return _Ref()->Rename(*name->_Ref());
		}
		FbxKRenamingStrategy^ FbxKRenamingStrategy::Clone()
		{
			return gcnew FbxKRenamingStrategy(_Ref()->Clone());
		}


		FbxNumberRenamingStrategy::FbxNumberRenamingStrategy() 
			:FbxKRenamingStrategy(new KNumberRenamingStrategy())
		{
			_Free = true;
		}
		FbxKRenamingStrategy^ FbxNumberRenamingStrategy::Clone()
		{
			return gcnew FbxNumberRenamingStrategy((KNumberRenamingStrategy*)_Ref()->Clone());
		}
	}
}