#pragma once
#include "stdafx.h"
#include "FbxEqualityComparer.h"


{
	namespace FbxSDK
	{		
		generic<class T> where T : IFbxNativePointer
			bool FbxEqualityComparer<T>::Equals(T x,T y)
		{
			if(!y || !x)
				return false;
			return x->NativePointer.ToPointer() == y->NativePointer.ToPointer();
		}
		generic<class T> where T : IFbxNativePointer
			int FbxEqualityComparer<T>::GetHashCode(T obj)
		{
			return (int)obj->NativePointer;
		}
	}
}