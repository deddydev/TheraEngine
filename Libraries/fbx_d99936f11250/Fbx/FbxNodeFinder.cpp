#pragma once
#include "stdafx.h"
#include "FbxNodeFinder.h"
#include "FbxNode.h"

namespace Skill
{
	namespace FbxSDK
	{		
		void FbxNodeFinder::CollectManagedMemory()
		{
		}
		array<FbxNode^>^ FbxNodeFinder::Apply(FbxNode^ searchRoot)
		{
			KArrayTemplate<KFbxNode*> arr = _Ref()->Apply(*searchRoot->_Ref());			
			if(arr.GetCount() > 0)
			{
				array<FbxNode^>^ nodes = gcnew array<FbxNode^>(arr.GetCount());
				for(int i=0; i < nodes->Length; i++)
				{
					nodes[i] = gcnew FbxNode(arr[i]);
				}
				return nodes;
			}			

			return nullptr;
		}

		void FbxNodeFinder::Reset()
		{
			_Ref()->Reset();
		}
	}
}