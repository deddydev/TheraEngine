#pragma once
#include "stdafx.h"

namespace FbxSDK
{			
	public interface class IFbxNativePointer
	{
	public:			
		property IntPtr NativePointer
		{
			IntPtr get();
			void set(IntPtr value);
		}			
		property bool IsValid
		{
			bool get();
		}
		bool IsSameAs(IFbxNativePointer^ other);
	};
}