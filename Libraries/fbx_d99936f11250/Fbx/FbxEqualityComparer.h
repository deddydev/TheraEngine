#pragma once
#include "stdafx.h"
#include "Fbx.h"

namespace Skill
{
	namespace FbxSDK
	{
		generic<class T> where T : IFbxNativePointer
			public ref class FbxEqualityComparer : System::Collections::Generic::IEqualityComparer<T>
		{		
		public:
			virtual bool Equals(T x,T y);
			virtual int GetHashCode(T obj);
		};
	}
}