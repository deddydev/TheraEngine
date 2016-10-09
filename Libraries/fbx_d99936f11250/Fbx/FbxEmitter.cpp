#pragma once
#include "stdafx.h"
#include "FbxEmitter.h"
#include "FbxEventHandler.h"

namespace Skill
{
	namespace FbxSDK
	{
		namespace Events
		{	
			void FbxEmitter::CollectManagedMemory()
			{
			}
			void FbxEmitter::AddListener(FbxEventHandler^ Handler)
			{
				_Ref()->AddListener(*Handler->_Ref());
			}				
			void FbxEmitter::RemoveListener(FbxEventHandler^ Handler)
			{
				_Ref()->RemoveListener(*Handler->_Ref());
			}			
		}
	}
}
